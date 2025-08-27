using MediatR;

namespace BuildingBlocks.CQRS;

// Estas interfaces sirven para manejar el comando ya sea no dando respuesta como el TCommand o dando respuesta como el TCommand, TResponse
// Se espeficica que TCommand debe ser ICommand<Unit> o ICommand<Response>
// El in en C# significa contravarianza, puedes agregar comandos como parámetros de entrada siempre y cuando sus tipos deriven de una misma base
// Supongamos que tienes esta jerarquía:
/*
    public interface IBaseCommand {}
    public class CreateOrderCommand : IBaseCommand {}
    public class DeleteOrderCommand : IBaseCommand {}

    public interface ICommandHandler<in TCommand>
    where TCommand : IBaseCommand
    {
        void Handle(TCommand command);
    }

    List<ICommandHandler<IBaseCommand>> handlers = new()
    {
        new CreateOrderHandler(),
        new DeleteOrderHandler()
    };

    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand>
    {
        public void Handle(CreateOrderCommand command) { ... }
    }

    public class DeleteOrderHandler : ICommandHandler<DeleteOrderCommand>
    {
        public void Handle(DeleteOrderCommand command) { ... }
    }
 */
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
    where TResponse : notnull
{
}
