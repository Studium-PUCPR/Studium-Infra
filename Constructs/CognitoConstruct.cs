using Amazon.CDK;
using Amazon.CDK.AWS.Cognito;
using Constructs;

namespace Studium_Infra.Constructs;

public class CognitoConstruct : Construct
{
    public UserPool UserPool { get; }
    public UserPoolClient WebClient { get; }

    public CognitoConstruct(Construct scope, string id) : base(scope, id)
    {
        UserPool = new UserPool(this, "StudiumUserPool", new UserPoolProps
        {
            UserPoolName = "studium-user-pool",
            SelfSignUpEnabled = false,
            SignInAliases = new SignInAliases
            {
                Email = true
            },
            StandardAttributes = new StandardAttributes
            {
                Email = new StandardAttribute { Required = true, Mutable = true },
                Fullname = new StandardAttribute { Required = true, Mutable = true }
            },
            CustomAttributes = new Dictionary<string, ICustomAttribute>
            {
                ["registration_number"] = new StringAttribute(new StringAttributeProps
                {
                    Mutable = true
                })
            },
            RemovalPolicy = RemovalPolicy.DESTROY
        });

        UserPool.AddDomain("StudiumCognitoDomain", new UserPoolDomainOptions
        {
            CognitoDomain = new CognitoDomainOptions
            {
                DomainPrefix = "studium-auth"
            }
        });

        WebClient = UserPool.AddClient("StudiumWebClient", new UserPoolClientOptions
        {
            UserPoolClientName = "studium-web-client",
            GenerateSecret = false,
            AuthFlows = new AuthFlow
            {
                UserSrp = true,
                UserPassword = true,
                AdminUserPassword = true
            },
            OAuth = new OAuthSettings
            {
                Flows = new OAuthFlows
                {
                    AuthorizationCodeGrant = true
                },
                Scopes =
                [
                    OAuthScope.OPENID,
                    OAuthScope.EMAIL,
                    OAuthScope.PROFILE
                ],
                CallbackUrls = ["http://localhost:3000/auth/callback"],
                LogoutUrls = ["http://localhost:3000"]
            },
            SupportedIdentityProviders =
            [
                UserPoolClientIdentityProvider.COGNITO
            ]
        });

        string[] roles = ["Student", "Professor", "Administrator"];
        foreach (var role in roles)
        {
            UserPool.AddGroup($"Studium{role}Group", new UserPoolGroupOptions
            {
                GroupName = role
            });
        }

        _ = new CfnOutput(this, "CognitoAuthority", new CfnOutputProps
        {
            Value = UserPool.UserPoolProviderUrl,
            Description = "AWS:Cognito:Authority URL for Studium microservices"
        });

        _ = new CfnOutput(this, "CognitoClientId", new CfnOutputProps
        {
            Value = WebClient.UserPoolClientId,
            Description = "AWS:Cognito:ClientId for Studium frontend and microservices"
        });
    }
}
