using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace Studium_Infra.Constructs;

public class MessagingConstruct : Construct
{
    public Queue UsersEventsQueue { get; }
    public Queue DeadLetterQueue { get; }

    public MessagingConstruct(Construct scope, string id) : base(scope, id)
    {
        DeadLetterQueue = new Queue(this, "WolverineDeadLetterQueue", new QueueProps
        {
            QueueName = "wolverine-dead-letter-queue",
            RetentionPeriod = Duration.Days(14),
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        UsersEventsQueue = new Queue(this, "StudiumUsersEventsQueue", new QueueProps
        {
            QueueName = "studium-users-events-queue",
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

        _ = new CfnOutput(this, "UsersEventsQueueUrl", new CfnOutputProps
        {
            Value = UsersEventsQueue.QueueUrl,
            Description = "SQS Queue URL for Studium-Users inbound booking events"
        });

        _ = new CfnOutput(this, "DeadLetterQueueUrl", new CfnOutputProps
        {
            Value = DeadLetterQueue.QueueUrl,
            Description = "SQS Queue URL for Wolverine Dead Letter Queue"
        });
    }
}
