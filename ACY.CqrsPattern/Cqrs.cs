using System;
using System.Threading.Tasks;

namespace ACY.CqrsLibrary;

// Marker interfaces
public interface ICommand { }
public interface IQuery<TResult> { }

// Command handler interface
public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    Task HandleAsync(TCommand command);
}

// Query handler interface
public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
    Task<TResult> HandleAsync(TQuery query);
}

// Dispatcher
public class CqrsDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CqrsDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
    {
        var handler = (ICommandHandler<TCommand>)_serviceProvider.GetService(typeof(ICommandHandler<TCommand>));
        if (handler == null) throw new InvalidOperationException($"No handler found for {typeof(TCommand).Name}");
        await handler.HandleAsync(command);
    }

    public async Task<TResult> QueryAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        var handler = (IQueryHandler<TQuery, TResult>)_serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>));
        if (handler == null) throw new InvalidOperationException($"No handler found for {typeof(TQuery).Name}");
        return await handler.HandleAsync(query);
    }
}

