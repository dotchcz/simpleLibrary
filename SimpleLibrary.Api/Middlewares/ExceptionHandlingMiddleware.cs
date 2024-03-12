using System.Net;

namespace SimpleLibrary.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }
    
    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode status;
        string message;
        
        if (exception is ArgumentException)
        {
            status = HttpStatusCode.BadRequest; // Příklad pro specifickou výjimku
            message = exception.Message;
        }
        else
        {
            status = HttpStatusCode.InternalServerError; // Obecná chyba serveru
            message = "Internal Server Error.";
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)status;

        var response = new
        {
            error = message,
            statusCode = context.Response.StatusCode
        };
        
        return context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
}