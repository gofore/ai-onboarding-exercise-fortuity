using System.Net.Http.Json;
using System.Text.Json;

namespace Fortuity.Web.Api;

public sealed class FortuityApiClient(HttpClient http)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public async Task<ApiCall<IReadOnlyList<CustomerSummary>>> ListCustomersAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var customers = await http.GetFromJsonAsync<IReadOnlyList<CustomerSummary>>(
                new Uri("api/customers", UriKind.Relative), JsonOptions, cancellationToken);
            return new(customers ?? [], null);
        }
        catch (HttpRequestException exception)
        {
            return new(null, $"Could not reach the Fortuity API: {exception.Message}");
        }
    }

    public async Task<ApiCall<CustomerDetail>> GetCustomerAsync(string customerNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            var detail = await http.GetFromJsonAsync<CustomerDetail>(
                new Uri($"api/customers/{Uri.EscapeDataString(customerNumber)}", UriKind.Relative), JsonOptions, cancellationToken);
            return new(detail, detail is null ? "Customer not found." : null);
        }
        catch (HttpRequestException exception)
        {
            return new(null, $"Could not load customer {customerNumber}: {exception.Message}");
        }
    }

    public Task<ApiCall<QuoteResult>> DraftQuoteAsync(DraftQuotePayload payload, CancellationToken cancellationToken = default) =>
        PostAsync<DraftQuotePayload, QuoteResult>("api/quotes", payload, cancellationToken);

    public Task<ApiCall<IndicationResult>> GetIndicationAsync(IndicationPayload payload, CancellationToken cancellationToken = default) =>
        PostAsync<IndicationPayload, IndicationResult>("api/quotes/indication", payload, cancellationToken);

    public Task<ApiCall<ClaimSummary>> RegisterClaimAsync(FnolPayload payload, CancellationToken cancellationToken = default) =>
        PostAsync<FnolPayload, ClaimSummary>("api/claims", payload, cancellationToken);

    public async Task<ApiCall<IReadOnlyList<ClaimSummary>>> ListOpenClaimsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var claims = await http.GetFromJsonAsync<IReadOnlyList<ClaimSummary>>(
                new Uri("api/claims/open", UriKind.Relative), JsonOptions, cancellationToken);
            return new(claims ?? [], null);
        }
        catch (HttpRequestException exception)
        {
            return new(null, $"Could not load open claims: {exception.Message}");
        }
    }

    public async Task<ApiCall<SyncReceiptModel>> RequestClaimsSyncAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await http.PostAsync(new Uri("api/claims/sync", UriKind.Relative), content: null, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return new(null, await ReadProblemAsync(response, cancellationToken));
            }

            var receipt = await response.Content.ReadFromJsonAsync<SyncReceiptModel>(JsonOptions, cancellationToken);
            return new(receipt, receipt is null ? "The API returned an empty response." : null);
        }
        catch (HttpRequestException exception)
        {
            return new(null, $"Could not reach the Fortuity API: {exception.Message}");
        }
    }

    public async Task<ApiCall<IReadOnlyList<RepairShopStatus>>> ListRepairShopStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var shops = await http.GetFromJsonAsync<IReadOnlyList<RepairShopStatus>>(
                new Uri("api/claims/sync/status", UriKind.Relative), JsonOptions, cancellationToken);
            return new(shops ?? [], null);
        }
        catch (HttpRequestException exception)
        {
            return new(null, $"Could not load repair shop status: {exception.Message}");
        }
    }

    private async Task<ApiCall<TResponse>> PostAsync<TRequest, TResponse>(
        string route,
        TRequest payload,
        CancellationToken cancellationToken)
    {
        try
        {
            var response = await http.PostAsJsonAsync(new Uri(route, UriKind.Relative), payload, JsonOptions, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                var problem = await ReadProblemAsync(response, cancellationToken);
                return new(default, problem);
            }

            var body = await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions, cancellationToken);
            return new(body, body is null ? "The API returned an empty response." : null);
        }
        catch (HttpRequestException exception)
        {
            return new(default, $"Could not reach the Fortuity API: {exception.Message}");
        }
    }

    private static async Task<string> ReadProblemAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            var problem = await response.Content.ReadFromJsonAsync<ProblemBody>(JsonOptions, cancellationToken);
            return problem?.Detail ?? problem?.Title ?? $"Request failed with status {(int)response.StatusCode}.";
        }
        catch (JsonException)
        {
            return $"Request failed with status {(int)response.StatusCode}.";
        }
    }

    private sealed record ProblemBody(string? Title, string? Detail);
}
