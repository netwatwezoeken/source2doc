using MediatR;

namespace MediatrCode;

public class NotificationAHandler1 : INotificationHandler<NotificationA>
{
    public Task Handle(NotificationA notification, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}