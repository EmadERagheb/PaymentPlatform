namespace BuildingBlocks.CQRS;
public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse> where TCommand : ICommand<TResponse>;
public interface ICommandHandler<TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand<Unit>;
