using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

[ApiExplorerSettings(IgnoreApi = true)]
public class ErrorsController : ControllerBase
{
    [Route("error")]
    public Error Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error; // Your exception
        var code = 500; // Internal Server Error by default

         if      (exception is NotFoundException)     code = 404; // Not Found
        // else if (exception is MyUnauthorizedException) code = 401; // Unauthorized
        // else if (exception is MyException)             code = 400; // Bad Request

        Response.StatusCode = code; // You can use HttpStatusCode enum instead

        return new Error(exception); // Your error model
    }
}