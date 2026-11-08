using Amazon.CDK;
using Amazon.CDK.AWS.SSM;
using Constructs;

namespace Studium_Infra.Constructs;

public class ParametersConstruct : Construct
{
    public StringParameter DbEndpointParam { get; }
    public StringParameter DbSecretNameParam { get; }
    public StringParameter CognitoAuthorityParam { get; }
    public StringParameter CognitoClientIdParam { get; }

    public ParametersConstruct(
        Construct scope,
        string id,
        CognitoConstruct cognito,
        DatabaseConstruct database) : base(scope, id)
    {
        CognitoAuthorityParam = new StringParameter(this, "ParamCognitoAuthority", new StringParameterProps
        {
            ParameterName = "/studium/users/cognito/authority",
            StringValue = cognito.UserPool.UserPoolProviderUrl,
            Description = "AWS:Cognito:Authority URL for Studium-Users"
        });

        CognitoClientIdParam = new StringParameter(this, "ParamCognitoClientId", new StringParameterProps
        {
            ParameterName = "/studium/users/cognito/client-id",
            StringValue = cognito.WebClient.UserPoolClientId,
            Description = "AWS:Cognito:ClientId for Studium-Users"
        });

        DbEndpointParam = new StringParameter(this, "ParamDatabaseEndpoint", new StringParameterProps
        {
            ParameterName = "/studium/users/database/endpoint",
            StringValue = database.Instance.DbInstanceEndpointAddress,
            Description = "PostgreSQL Host endpoint address for Studium-Users"
        });

        DbSecretNameParam = new StringParameter(this, "ParamDatabaseSecretName", new StringParameterProps
        {
            ParameterName = "/studium/users/database/secret-name",
            StringValue = "studium/database/postgres",
            Description = "AWS Secrets Manager secret name containing RDS credentials"
        });

        _ = new CfnOutput(this, "ParamCognitoAuthorityName", new CfnOutputProps
        {
            Value = CognitoAuthorityParam.ParameterName,
            Description = "SSM Parameter Name for Cognito Authority"
        });

        _ = new CfnOutput(this, "ParamCognitoClientIdName", new CfnOutputProps
        {
            Value = CognitoClientIdParam.ParameterName,
            Description = "SSM Parameter Name for Cognito Client ID"
        });

        _ = new CfnOutput(this, "ParamDbEndpointName", new CfnOutputProps
        {
            Value = DbEndpointParam.ParameterName,
            Description = "SSM Parameter Name for Database Endpoint"
        });
    }
}
