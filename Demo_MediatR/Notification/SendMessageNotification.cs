using MediatR;

namespace Demo_MediatR.Notification
{
    public class SendMessageNotification : INotification
    {
        public string Message { get; set; }
    }

    public class MessageLogHandler : INotificationHandler<SendMessageNotification>
    {
        public Task Handle(SendMessageNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"log : {notification.Message}");
            return Task.CompletedTask;
        }
    }

    public class MessageEmailHandler : INotificationHandler<SendMessageNotification>
    {
        public Task Handle(SendMessageNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"email : {notification.Message}");
            return Task.CompletedTask;
        }
    }

    public class MessageSmsHandler : INotificationHandler<SendMessageNotification>
    {
        public Task Handle(SendMessageNotification notification, CancellationToken cancellationToken)
        {
            Console.WriteLine($"sms : {notification.Message}");
            return Task.CompletedTask;
        }
    }
}
