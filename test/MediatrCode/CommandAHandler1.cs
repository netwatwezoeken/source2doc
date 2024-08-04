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

public class RequestAHandler1 : IRequestHandler<RequestA, ResponseA>
{
    public async Task<ResponseA> Handle(RequestA request, CancellationToken cancellationToken)
    {
        return new ResponseA();
    }
}

public class RequestAHandler2 : IRequestHandler<RequestA, ResponseA>
{
    public async Task<ResponseA> Handle(RequestA request, CancellationToken cancellationToken)
    {
        return new ResponseA();
    }
}

public class RequestA : IRequest<ResponseA>
{
}

public class ResponseA
{
}

public class CommandA : IRequest
{
}