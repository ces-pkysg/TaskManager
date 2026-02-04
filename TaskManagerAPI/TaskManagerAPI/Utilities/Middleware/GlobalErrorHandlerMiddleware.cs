using System.Net;
using System.Text.Json;
using TaskManagerAPI.Utilities.Exceptions;

public class GlobalErrorHandlerMiddleware
{
    private readonly RequestDelegate _next; //REPRESENTA CUAL ES EL SIGUIENTE PASO EN EJECUCION, PUEDE SER UTRO MI
    private readonly ILogger<GlobalErrorHandlerMiddleware> _logger; //PARA REGISTRAR ERRORES EN UN LOG (EL LOG SE GUARDA EL DETALLE TÉCNICO DE QUE OCURRIÓ)

    public GlobalErrorHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context) //SE EJECUTA EN CADA PETICION HTTP
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
        //EL CONTEXTO DE LA RESPUESTA Y LA EXCEPCION, SE GESTIONA LA RESPUESTA DE LO QUE ESTA CONSUMIENDO
    {
        context.Response.ContentType = "application/json";
        var statusCode = exception is BusinessException be
        ? be.StatusCode
        : StatusCodes.Status500InternalServerError;
        context.Response.StatusCode = statusCode;

        var response = new //DECLARAR UN OBJETO ANONIMO
        {
            message = "Ocurrió un error inesperado.",
            detail = exception.Message
        };

        return context.Response.WriteAsync(
            JsonSerializer.Serialize(response));
    }
}
