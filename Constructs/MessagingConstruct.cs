using Amazon.CDK;
using Amazon.CDK.AWS.SQS;
using Constructs;

namespace Studium_Infra.Constructs;

public class MessagingConstruct : Construct
{
    public Queue UsersEventsQueue { get; }

    public MessagingConstruct(Construct scope, string id) : base(scope, id)
    {
        UsersEventsQueue = new Queue(this, "StudiumUsersEventsQueue", new QueueProps
        {
            QueueName = "studium-users-events-queue",
            ReceiveMessageWaitTime = Duration.Seconds(20),
            VisibilityTimeout = Duration.Seconds(30),
            RetentionPeriod = Duration.Days(4),
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        _ = new CfnOutput(this, "UsersEventsQueueUrl", new CfnOutputProps
        {
            Value = UsersEventsQueue.QueueUrl,
            Description = "SQS Queue URL for Studium-Users inbound booking events"
        });
    }
}
