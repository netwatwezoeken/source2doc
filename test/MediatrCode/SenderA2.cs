using MediatR;

namespace MediatrCode;

public class SenderA2(IMediator mediator)
{
    public void Send()
    {
        mediator.Publish(new NotificationA());
    }
}