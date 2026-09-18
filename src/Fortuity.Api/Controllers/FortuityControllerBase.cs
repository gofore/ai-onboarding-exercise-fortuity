using Fortuity.Application.Common;
using Microsoft.AspNetCore.Mvc;

namespace Fortuity.Api.Controllers;

[ApiController]
public abstract class FortuityControllerBase : ControllerBase
{
    protected ActionResult Failure(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var statusCode = error.Kind switch
        {
            ErrorKind.NotFound => StatusCodes.Status404NotFound,
            ErrorKind.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return Problem(title: error.Code, detail: error.Message, statusCode: statusCode);
    }
}
