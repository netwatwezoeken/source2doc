using MediatR;

namespace MediatrCode;

public class RequestAHandler1 : IRequestHandler<RequestA, ResponseA>
{
    public async Task<ResponseA> Handle(RequestA request, CancellationToken cancellationToken)
    {
        return new ResponseA();
    }
}