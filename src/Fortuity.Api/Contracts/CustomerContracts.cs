using Fortuity.Domain.Customers;
using Fortuity.Domain.Policies;

namespace Fortuity.Api.Contracts;

public sealed record CustomerResponse(
    string CustomerNumber,
    string Name,
    string City,
    string Region,
    string PricingBasis)
{
    internal static CustomerResponse From(Customer customer) => new(
        customer.CustomerNumber,
        customer.Name,
        customer.City,
        customer.Region.Value,
        customer.Preferences.PricingBasis.ToString());
}

public sealed record PolicyResponse(
    string Number,
    string Type,
    string Coverage,
    string Region,
    string Status,
    DateOnly StartsOn,
    DateOnly? EndsOn,
    decimal AnnualPremium)
{
    internal static PolicyResponse From(Policy policy) => new(
        policy.Number.Value,
        policy.Type.ToString(),
        policy.Coverage.ToString(),
        policy.Region.Value,
        policy.Status.ToString(),
        policy.StartsOn,
        policy.EndsOn,
        policy.AnnualPremium.Amount);
}

public sealed record CustomerDetailResponse(
    CustomerResponse Customer,
    IReadOnlyList<PolicyResponse> Policies);
