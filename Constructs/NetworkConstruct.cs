using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Constructs;

namespace Studium_Infra.Constructs;

public class NetworkConstruct : Construct
{
    public IVpc Vpc { get; }

    public NetworkConstruct(Construct scope, string id) : base(scope, id)
    {
        Vpc = Amazon.CDK.AWS.EC2.Vpc.FromLookup(this, "StudiumDefaultVpc", new VpcLookupOptions
        {
            IsDefault = true
        });

        _ = new CfnOutput(this, "StudiumVpcId", new CfnOutputProps
        {
            Value = Vpc.VpcId,
            Description = "Shared Default VPC ID for Studium resources"
        });
    }
}
