using MediatR;
using MediatR.Pipeline;

namespace MediatrCode;

public class CommandAHandler1 : IRequestHandler<CommandA>
{
    public Task Handle(CommandA request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}