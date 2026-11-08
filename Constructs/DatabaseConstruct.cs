using Amazon.CDK;
using Amazon.CDK.AWS.EC2;
using Amazon.CDK.AWS.RDS;
using Constructs;

namespace Studium_Infra.Constructs;

public class DatabaseConstruct : Construct
{
    public DatabaseInstance Instance { get; }

    public DatabaseConstruct(Construct scope, string id, IVpc vpc) : base(scope, id)
    {
        Instance = new DatabaseInstance(this, "StudiumPostgresInstance", new DatabaseInstanceProps
        {
            InstanceIdentifier = "studium-postgres-db",
            DatabaseName = "studium_users",
            Engine = DatabaseInstanceEngine.Postgres(new PostgresInstanceEngineProps
            {
                Version = PostgresEngineVersion.VER_16
            }),
            InstanceType = Amazon.CDK.AWS.EC2.InstanceType.Of(InstanceClass.T4G, InstanceSize.MICRO),
            Vpc = vpc,
            VpcSubnets = new SubnetSelection
            {
                SubnetType = SubnetType.PUBLIC
            },
            PubliclyAccessible = true,
            AllocatedStorage = 20,
            MaxAllocatedStorage = 20,
            StorageType = StorageType.GP2,
            MultiAz = false,
            BackupRetention = Duration.Days(0),
            DeleteAutomatedBackups = true,
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        _ = new CfnOutput(this, "RdsEndpointAddress", new CfnOutputProps
        {
            Value = Instance.DbInstanceEndpointAddress,
            Description = "RDS PostgreSQL Host endpoint address"
        });

        _ = new CfnOutput(this, "RdsSecretArn", new CfnOutputProps
        {
            Value = Instance.Secret?.SecretArn ?? string.Empty,
            Description = "Secrets Manager ARN containing the generated RDS credentials"
        });
    }
}
