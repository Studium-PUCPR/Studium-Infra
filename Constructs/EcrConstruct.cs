using Amazon.CDK;
using Amazon.CDK.AWS.ECR;
using Constructs;

namespace Studium_Infra.Constructs;

public class EcrConstruct : Construct
{
    public Repository UsersRepository { get; }

    public EcrConstruct(Construct scope, string id) : base(scope, id)
    {
        UsersRepository = new Repository(this, "StudiumUsersEcrRepo", new RepositoryProps
        {
            RepositoryName = "studium-users",
            EmptyOnDelete = true,
            RemovalPolicy = RemovalPolicy.DESTROY,
            LifecycleRules =
            [
                new LifecycleRule
                {
                    MaxImageCount = 5
                }
            ]
        });

        _ = new CfnOutput(this, "UsersEcrRepositoryUri", new CfnOutputProps
        {
            Value = UsersRepository.RepositoryUri,
            Description = "ECR Repository URI for Studium-Users Docker images"
        });
    }
}
