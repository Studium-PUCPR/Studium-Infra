using Amazon.CDK;
using Constructs;
using Studium_Infra.Constructs;

namespace Studium_Infra.Stacks;

public class StudiumStack : Stack
{
    public StudiumStack(Construct scope, string id, IStackProps? props = null)
        : base(scope, id, props)
    {
        var network = new NetworkConstruct(this, "Network");
        _ = new CognitoConstruct(this, "Cognito");
        _ = new DatabaseConstruct(this, "Database", network.Vpc);
        _ = new MessagingConstruct(this, "Messaging");
    }
}
