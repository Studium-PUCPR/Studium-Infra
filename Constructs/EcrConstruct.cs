using Amazon.CDK;
using Amazon.CDK.AWS.ECR;
using Constructs;

namespace Studium_Infra.Constructs;

public class EcrConstruct : Construct
{
    public Repository UsersRepository { get; }
    public Repository CatalogRepository { get; }
    public Repository BookingRepository { get; }
    public Repository NotificationsRepository { get; }
    public Repository FrontRepository { get; }
    public Repository AuditRepository { get; }

    public EcrConstruct(Construct scope, string id) : base(scope, id)
    {
        UsersRepository = CreateRepository("StudiumUsersEcrRepo", "studium-users", "Users");
        CatalogRepository = CreateRepository("StudiumCatalogEcrRepo", "studium-catalog", "Catalog");
        BookingRepository = CreateRepository("StudiumBookingEcrRepo", "studium-booking", "Booking");
        NotificationsRepository = CreateRepository("StudiumNotificationsEcrRepo", "studium-notifications", "Notifications");
        FrontRepository = CreateRepository("StudiumFrontEcrRepo", "studium-front", "Frontend");
        AuditRepository = CreateRepository("StudiumAuditEcrRepo", "studium-audit", "Audit");
    }

    private Repository CreateRepository(string id, string repositoryName, string serviceName)
    {
        var repo = new Repository(this, id, new RepositoryProps
        {
            RepositoryName = repositoryName,
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

        _ = new CfnOutput(this, $"{serviceName}EcrRepositoryUri", new CfnOutputProps
        {
            Value = repo.RepositoryUri,
            Description = $"ECR Repository URI for Studium-{serviceName} Docker images"
        });

        return repo;
    }
}
