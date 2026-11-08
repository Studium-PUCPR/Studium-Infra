using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace Studium_Infra.Constructs;

public class MessagingConstruct : Construct
{
    public Queue DeadLetterQueue { get; }
    public Queue UsersEventsQueue { get; }
    public Queue BookingEventsQueue { get; }
    public Queue NotificationsQueue { get; }

    public MessagingConstruct(Construct scope, string id) : base(scope, id)
    {
        DeadLetterQueue = new Queue(this, "WolverineDeadLetterQueue", new QueueProps
        {
            QueueName = "wolverine-dead-letter-queue",
            RetentionPeriod = Duration.Days(14),
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        UsersEventsQueue = CreateQueue("StudiumUsersEventsQueue", "studium-users-events-queue", "Users");
        BookingEventsQueue = CreateQueue("StudiumBookingEventsQueue", "studium-booking-events-queue", "Booking");
        NotificationsQueue = CreateQueue("StudiumNotificationsQueue", "studium-notifications-queue", "Notifications");

        _ = new CfnOutput(this, "DeadLetterQueueUrl", new CfnOutputProps
        {
            Value = DeadLetterQueue.QueueUrl,
            Description = "SQS Queue URL for Wolverine Dead Letter Queue"
        });
    }

    private Queue CreateQueue(string id, string queueName, string serviceName)
    {
        var queue = new Queue(this, id, new QueueProps
        {
            QueueName = queueName,
            ReceiveMessageWaitTime = Duration.Seconds(20),
            VisibilityTimeout = Duration.Seconds(30),
            RetentionPeriod = Duration.Days(4),
            DeadLetterQueue = new DeadLetterQueue
            {
                MaxReceiveCount = 5,
                Queue = DeadLetterQueue
            },
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        _ = new CfnOutput(this, $"{serviceName}EventsQueueUrl", new CfnOutputProps
        {
            Value = queue.QueueUrl,
            Description = $"SQS Queue URL for Studium-{serviceName} events"
        });

        return queue;
    }
}
