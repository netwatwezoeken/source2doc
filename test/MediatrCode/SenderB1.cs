using MediatR;

namespace MediatrCode;

public class SenderB2(IMediator mediator)
{
    public void Send()
    {
        mediator.Publish(new CommandB());
    }
}