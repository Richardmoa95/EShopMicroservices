using MediatR;

namespace BuildingBlocks.CQRS;

// Esta línea de código es un atajo para dejar de escribir en cada comando ICommand<Unit> y poder usar solo ICommand
// por ejemplo, en lugar de CreateOrderCommand : ICommand<Unit> sería CreateOrderCommand : ICommand
// y así no tener que escribir Unit en cada comando “void”.
// Mencionar que ICommand hereda de ICommand<Unit> que sería la interfaz de abajo ICommand<out TResponse> que hereda de IRequest<TResponse> el cual MeditarR lee.
public interface ICommand : ICommand<Unit>
{
}

// En C# out en genericos (TResponse) significa covarianza, lo cual permite que TResponse pueda ser un tipo más derivado que el especificado en la interfaz.
// Este patrón suele suceder cuando se tiene listas o colecciones de comandos que devuelven diferentes tipos (ICommand<TDerivado> -> ICommand<TBase>):
/* Si en un comando se retorna diferentes tipos, el out sirve para controlar eso ya que con out el valor a retornar se trata como su base en este caso string y int 
 * derivan de object
public record GetOrderNameCommand(Guid Id) : ICommand<string>; Esto retorna un string
public record GetOrderCountCommand() : ICommand<int>; Esto retorna un int

List<ICommand<object>> comandos = new()
{
    new GetOrderNameCommand(Guid.NewGuid()), // string
    new GetOrderCountCommand()               // int
};*/
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}
