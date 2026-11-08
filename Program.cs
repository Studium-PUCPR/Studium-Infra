using Amazon.CDK;
using Studium_Infra.Stacks;

var app = new App();

new StudiumStack(app, "StudiumStack", new StackProps
{
    Env = new Amazon.CDK.Environment
    {
        Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT"),
        Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION") ?? "us-east-1"
    }
});

app.Synth();
