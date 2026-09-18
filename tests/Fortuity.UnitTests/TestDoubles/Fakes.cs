using Fortuity.Application.Abstractions;
using Fortuity.Application.Common;
using Fortuity.Application.PartnerSync;
using Fortuity.Application.Underwriting;
using Fortuity.Domain.Claims;
using Fortuity.Domain.Common;
using Fortuity.Domain.Customers;
using Fortuity.Domain.Partners;
using Fortuity.Domain.Policies;
using Fortuity.Domain.Quotes;
using Fortuity.Domain.Underwriting;

namespace Fortuity.UnitTests.TestDoubles;

internal sealed class FakeClock : IClock
{
    public DateTimeOffset UtcNow { get; set; } = new(2026, 8, 6, 12, 0, 0, TimeSpan.Zero);

    public DateOnly Today { get; set; } = new(2026, 8, 6);
}

internal sealed class FakeCustomerStore : ICustomerStore
{
    public List<Customer> Customers { get; } = [];

    public Task<Customer?> FindByNumberAsync(string customerNumber, CancellationToken cancellationToken = default) =>
        Task.FromResult(Customers.Find(customer => customer.CustomerNumber == customerNumber));

    public Task<IReadOnlyList<Customer>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Customer>>(Customers);
}

internal sealed class FakePolicyStore : IPolicyStore
{
    public List<Policy> Policies { get; } = [];

    public Task<Policy?> FindByNumberAsync(PolicyNumber number, CancellationToken cancellationToken = default) =>
        Task.FromResult(Policies.Find(policy => policy.Number == number));

    public Task<IReadOnlyList<Policy>> ListForCustomerAsync(int customerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Policy>>(Policies.FindAll(policy => policy.CustomerId == customerId));
}

internal sealed class FakeQuoteStore : IQuoteStore
{
    public List<Quote> Added { get; } = [];

    public Task AddAsync(Quote quote, CancellationToken cancellationToken = default)
    {
        Added.Add(quote);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Quote>> ListForCustomerAsync(int customerId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Quote>>(Added.FindAll(quote => quote.CustomerId == customerId));
}

internal sealed class FakeClaimStore : IClaimStore
{
    public List<Claim> Added { get; } = [];

    public Task AddAsync(Claim claim, CancellationToken cancellationToken = default)
    {
        Added.Add(claim);
        return Task.CompletedTask;
    }

    public Task<int> NextSequenceAsync(int year, CancellationToken cancellationToken = default) =>
        Task.FromResult(Added.Count + 1);

    public Task<IReadOnlyList<Claim>> ListOpenAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Claim>>(Added.FindAll(claim => claim.IsOpen));

    public Task<IReadOnlyList<Claim>> ListOpenForShopAsync(int repairShopId, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<Claim>>(
            Added.FindAll(claim => claim.IsOpen && claim.AssignedRepairShopId == repairShopId));
}

internal sealed class FakeTariffSource : ITariffSource
{
    public Dictionary<(string Region, PolicyType Type, RatingBasis Basis), decimal> Multipliers { get; } = [];

    public Task<decimal?> ResolveMultiplierAsync(
        RegionCode region,
        PolicyType policyType,
        RatingBasis basis,
        DateOnly asOf,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(Multipliers.TryGetValue((region.Value, policyType, basis), out var multiplier)
            ? (decimal?)multiplier
            : null);
}

internal sealed class RecordingAuditTrail : IAuditTrail
{
    public List<(string Action, string Subject, string Details)> Entries { get; } = [];

    public void Record(string action, string subject, string details) =>
        Entries.Add((action, subject, details));
}

internal sealed class FakeRepairShopStore : IRepairShopStore
{
    public List<RepairShop> Shops { get; } = [];

    public int SaveCount { get; private set; }

    public Task<IReadOnlyList<RepairShop>> ListAsync(CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RepairShop>>(Shops);

    public Task<IReadOnlyList<RepairShop>> ListByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default) =>
        Task.FromResult<IReadOnlyList<RepairShop>>(Shops.FindAll(shop => ids.Contains(shop.Id)));

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SaveCount++;
        return Task.CompletedTask;
    }
}

internal sealed class FakeSyncBacklog : ISyncBacklog
{
    public List<SyncTicket> Enqueued { get; } = [];

    public ValueTask EnqueueAsync(SyncTicket ticket, CancellationToken cancellationToken = default)
    {
        Enqueued.Add(ticket);
        return ValueTask.CompletedTask;
    }

    public async IAsyncEnumerable<SyncTicket> DequeueAllAsync(
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var ticket in Enqueued)
        {
            yield return ticket;
        }

        await Task.CompletedTask;
    }
}
