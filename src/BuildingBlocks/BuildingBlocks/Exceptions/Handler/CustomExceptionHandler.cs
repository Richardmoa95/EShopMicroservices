using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Exceptions.Handler;

public class CustomExceptionHandler (ILogger<CustomExceptionHandler> logger) : IExceptionHandler
{
    // TryHandleAsync se encarga de manejar excepciones específicas y devolver respuestas HTTP adecuadas.
    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError("Error Message: {exceptionMessage}, Time of occurrence: {time}", exception.Message, DateTime.UtcNow);

        // Mapea diferentes tipos de excepciones a detalles específicos para la respuesta HTTP.
        (string Detail, string Title, int StatusCode) details = exception switch
        {
            InternalServerException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status500InternalServerError),
            ValidationException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity),
            BadRequestException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status400BadRequest),
            NotFoundException => (exception.Message, exception.GetType().Name, context.Response.StatusCode = StatusCodes.Status404NotFound),
            _ => (exception.Message, "Failure", context.Response.StatusCode = StatusCodes.Status500InternalServerError)
        };

        // Crea un objeto ProblemDetails para estructurar la respuesta HTTP.
        var problemDetails = new ProblemDetails
        {
            Title = details.Title,
            Detail = details.Detail,
            Status = details.StatusCode,
            Instance = context.Request.Path
        };

        // Agrega información adicional a las extensiones de ProblemDetails.
        problemDetails.Extensions.Add("traceId", context.TraceIdentifier);

        // Si la excepción es de tipo ValidationException, agrega los errores de validación a las extensiones.
        if (exception is ValidationException validationException)
        {
            problemDetails.Extensions.Add("ValidationErrors", validationException.Errors);
        }

        // Escribe la respuesta HTTP en formato JSON.
        await context.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        // Indica que la excepción ha sido manejada.
        return true;
    }
}
