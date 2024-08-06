using MediatR;

namespace MediatrCode;

public class CommandAHandler2 : IRequestHandler<CommandA>, IRequestHandler<CommandB>
{
    public Task Handle(CommandA request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task Handle(CommandB request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}