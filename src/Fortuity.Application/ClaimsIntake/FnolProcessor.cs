using FluentValidation;
using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Domain.Claims;
using Fortuity.Domain.Policies;

namespace Fortuity.Application.ClaimsIntake;

public sealed class FnolProcessor(
    IPolicyStore policyStore,
    IClaimStore claimStore,
    IValidator<FnolReport> validator,
    IClock clock) : IFnolProcessor
{
    public async Task<Result<Claim>> RegisterAsync(FnolReport report, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(report);

        var validation = await validator.ValidateAsync(report, cancellationToken).ConfigureAwait(false);
        if (!validation.IsValid)
        {
            return Result.Failure<Claim>(Error.Validation("fnol.invalid_report", validation.ToString("; ")));
        }

        PolicyNumber policyNumber;
        try
        {
            policyNumber = PolicyNumber.From(report.PolicyNumber);
        }
        catch (FormatException exception)
        {
            return Result.Failure<Claim>(Error.Validation("fnol.malformed_policy_number", exception.Message));
        }

        var policy = await policyStore.FindByNumberAsync(policyNumber, cancellationToken).ConfigureAwait(false);
        if (policy is null)
        {
            return Result.Failure<Claim>(Error.NotFound(
                "policy.not_found",
                $"No policy exists with number {policyNumber}."));
        }

        if (!policy.IsActiveOn(report.IncidentDate))
        {
            return Result.Failure<Claim>(Error.Conflict(
                "policy.not_active",
                $"Policy {policyNumber} was not active on {report.IncidentDate:yyyy-MM-dd}."));
        }

        var year = clock.Today.Year;
        var sequence = await claimStore.NextSequenceAsync(year, cancellationToken).ConfigureAwait(false);

        var claim = new Claim
        {
            Number = ClaimNumber.Create(year, sequence),
            PolicyId = policy.Id,
            Type = report.Type,
            IncidentDate = report.IncidentDate,
            ReportedAt = clock.UtcNow,
            ReserveAmount = InitialReserves.For(report.Type),
        };

        await claimStore.AddAsync(claim, cancellationToken).ConfigureAwait(false);

        return Result.Success(claim);
    }
}
