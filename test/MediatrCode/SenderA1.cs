using MediatR;

namespace MediatrCode;

public class SenderA1(IMediator mediator)
{
    public virtual void Send()
    {
        mediator.Publish(new NotificationA());
    }
}

public class DerivedSenderA1(IMediator mediator) : SenderA1(mediator) { }

public class DerivedSenderA1ThatOverrides(IMediator mediator) : SenderA1(mediator)
{
    public override void Send()
    {
        mediator.Publish(new NotificationB());
    }
}