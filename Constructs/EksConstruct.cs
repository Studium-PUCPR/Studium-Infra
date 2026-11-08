using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.EKS;
using Amazon.CDK.AWS.IAM;
using Constructs;

namespace Studium_Infra.Constructs;

public class EksConstruct : Construct
{
    public CfnCluster Cluster { get; }

    public EksConstruct(Construct scope, string id, IVpc vpc) : base(scope, id)
    {
        var clusterRole = new Role(this, "StudiumEksClusterRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("eks.amazonaws.com"),
            ManagedPolicies =
            [
                ManagedPolicy.FromAwsManagedPolicyName("AmazonEKSClusterPolicy")
            ]
        });

        var nodeRole = new Role(this, "StudiumEksNodeRole", new RoleProps
        {
            AssumedBy = new ServicePrincipal("ec2.amazonaws.com"),
            ManagedPolicies =
            [
                ManagedPolicy.FromAwsManagedPolicyName("AmazonEKSWorkerNodePolicy"),
                ManagedPolicy.FromAwsManagedPolicyName("AmazonEKS_CNI_Policy"),
                ManagedPolicy.FromAwsManagedPolicyName("AmazonEC2ContainerRegistryReadOnly"),
                ManagedPolicy.FromAwsManagedPolicyName("AmazonSQSFullAccess")
            ]
        });

        var publicSubnetIds = vpc.PublicSubnets.Select(s => s.SubnetId).ToArray();

        Cluster = new CfnCluster(this, "StudiumEksCluster", new CfnClusterProps
        {
            Name = "studium-eks-cluster",
            Version = "1.31",
            RoleArn = clusterRole.RoleArn,
            ResourcesVpcConfig = new CfnCluster.ResourcesVpcConfigProperty
            {
                SubnetIds = publicSubnetIds,
                EndpointPublicAccess = true
            },
            AccessConfig = new CfnCluster.AccessConfigProperty
            {
                AuthenticationMode = "API_AND_CONFIG_MAP",
                BootstrapClusterCreatorAdminPermissions = true
            }
        });

        _ = new CfnNodegroup(this, "StudiumEksNodegroup", new CfnNodegroupProps
        {
            ClusterName = Cluster.Ref,
            NodegroupName = "studium-eks-nodes",
            NodeRole = nodeRole.RoleArn,
            Subnets = publicSubnetIds,
            InstanceTypes = ["t3.small"],
            ScalingConfig = new CfnNodegroup.ScalingConfigProperty
            {
                MinSize = 1,
                DesiredSize = 2,
                MaxSize = 3
            }
        });

        _ = new CfnOutput(this, "EksClusterName", new CfnOutputProps
        {
            Value = Cluster.Ref,
            Description = "EKS Cluster Name (aws eks update-kubeconfig --name studium-eks-cluster)"
        });
    }
}
