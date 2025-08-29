using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BuildingBlocks.Behaviors;

// LoggingBehavior es una clase generica que aplica a cualquier tipo de request y response (es decir no conoce el tipo exacto con el que trabajara pero acepta TRequest y TResponse)
// IPipelineBehavior es una interfaz de MediatR que se ejecutará antes de invocar el handler real
// IPipelineBehavior<TRequest, TResponse> TRequest y TResponse son genericos y se definen en la clase LoggingBehavior para que pueda trabajar con cualquier tipo de request y response
public class LoggingBehavior<TRequest, TResponse> (ILogger<LoggingBehavior<TRequest,TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
    // TRequest debe implementar IRequest<TResponse> para asegurar que es un request valido de MediatR
    where TRequest : notnull, IRequest<TResponse>
    // TResponse no puede ser nulo
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        logger.LogInformation("[START] Handle request={Request} - Response={Response} - RequestData={RequestData}", typeof(TRequest).Name, typeof(TResponse).Name, request);

        // Mide el tiempo que tarda en procesar la request
        var timer = new Stopwatch();
        // Inicia el temporizador
        timer.Start();

        // Llama al siguiente comportamiento en la cadena (o al handler real si no hay más comportamientos)
        var response = await next();

        // Detiene el temporizador
        timer.Stop();

        // Si el tiempo que tarda en procesar la request es mayor a 3 segundos, loguea una advertencia
        var timeTaken = timer.Elapsed;
        if (timeTaken.Seconds > 3)
        {
            logger.LogWarning("[PERFORMANCE] The request {Request} took {TimeTaken} seconds.", typeof(TRequest).Name, timeTaken.TotalSeconds);
        }

        logger.LogInformation("[END] Handled {Request} with {Response}", typeof(TRequest).Name, typeof(TResponse).Name);

        return response;
    }
}
