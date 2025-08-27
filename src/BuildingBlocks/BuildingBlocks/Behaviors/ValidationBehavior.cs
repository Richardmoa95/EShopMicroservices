using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.Behaviors;

// ValidationBehavior es una clase generica que aplica a cualquier tipo de request y response (es decir no conoce el tipo exacto con el que trabajara pero acepta TRequest y TResponse)
// IEnumerable<IValidator<TRequest>> se encarga de recibir varias validaciones para un comando es decir,
// public class CreateProductCommandNameValidator : AbstractValidator<CreateProductCommand>
// public class CreateProductCommandPriceValidator : AbstractValidator<CreateProductCommand>
// se registran ambas validaciones cuando se usa el IEnumerable
// IPipelineBehavior es una interfaz de MediatR que se ejecutará antes de invocar el handler real
// La restricción where asegura que solo se aplique a comandos (ICommand<TResponse>, no queries en este caso)
public class ValidationBehavior<TRequest, TResponse> (IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : ICommand<TResponse>
{
    // Handle es el método que se ejecuta antes de invocar el handler real
    // request el comando/query que llego
    // next delegado que llama al siguiente paso del pipeline (el handler real)
    // cancellationToken token para cancelar la operación si es necesario
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Se crea un contexto de validación para FluentValidation con la request que llegó.
        var context = new ValidationContext<TRequest>(request);

        // Ejecuta todos los validadores registrados para este TRequest en paralelo (Task.WhenAll).
        // Cada IValidator<TRequest> devuelve un ValidationResult.
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Junta todos los errores de los validadores en una sola lista.
        var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

        // Si hay errores de validación, lanza una excepción ValidationException.
        if (failures.Count != 0)
        {
            throw new ValidationException(failures);
        }

        // Si no hay errores de validación, llama a next(), que ejecuta el Handler real.
        return await next();
    }
}
