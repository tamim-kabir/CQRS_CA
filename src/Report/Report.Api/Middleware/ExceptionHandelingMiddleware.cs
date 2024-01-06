using Api.Premetives;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace Api.Middleware;

internal sealed class ExceptionHandelingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandelingMiddleware> _logger;
    public ExceptionHandelingMiddleware(ILogger<ExceptionHandelingMiddleware> logger) => _logger = logger;    
    
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
		try
		{
			await next(context);
		}
        catch (HttpStatusCodeException e)
        {
            _logger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
        catch (Exception e)
		{
			_logger.LogError(e, e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, HttpStatusCodeException e)
    {
        string result = string.Empty;
        context.Response.ContentType = "application/json";
        if (e is HttpStatusCodeException)
        {
            result = new
            {
                Message = e.Message,
                StatusCode = (int)e.StatusCode
            }.ToString();
            context.Response.StatusCode = (int)e.StatusCode;
        }
        else
        {
            result = new 
            {
                Message = "Runtime Error",
                StatusCode = StatusCodes.Status400BadRequest
            }.ToString();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
        }
        return context.Response.WriteAsync(result);
    }
    private static async Task HandleExceptionAsync(HttpContext context, Exception e)
	{
		context.Response.ContentType = "application/json";
		context.Response.StatusCode = e switch
		{
             ValidationException => StatusCodes.Status400BadRequest,
			_ => StatusCodes.Status500InternalServerError
		};
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { Message = e.Message, StatusCode = context.Response.StatusCode }));
    }
}
