using Fortuity.Api.Contracts;
using Fortuity.Application.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace Fortuity.Api.Controllers;

[Route("api/customers")]
public sealed class CustomersController(ICustomerStore customerStore, IPolicyStore policyStore) : FortuityControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<CustomerResponse>>> List(CancellationToken cancellationToken)
    {
        var customers = await customerStore.ListAsync(cancellationToken);
        return Ok(customers.Select(CustomerResponse.From).ToList());
    }

    [HttpGet("{customerNumber}")]
    public async Task<ActionResult<CustomerDetailResponse>> Get(string customerNumber, CancellationToken cancellationToken)
    {
        var customer = await customerStore.FindByNumberAsync(customerNumber, cancellationToken);
        if (customer is null)
        {
            return Problem(title: "customer.not_found", detail: $"No customer exists with number {customerNumber}.", statusCode: StatusCodes.Status404NotFound);
        }

        var policies = await policyStore.ListForCustomerAsync(customer.Id, cancellationToken);

        return Ok(new CustomerDetailResponse(
            CustomerResponse.From(customer),
            policies.Select(PolicyResponse.From).ToList()));
    }
}
