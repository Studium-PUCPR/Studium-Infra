using Amazon.CDK;
using Studium_Infra.Stacks;

var app = new App();

var env = new Amazon.CDK.Environment
{
    Account = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_ACCOUNT") ?? "315584099393",
    Region = System.Environment.GetEnvironmentVariable("CDK_DEFAULT_REGION") ?? "us-east-2"
};

new StudiumStack(app, "StudiumStack", new StackProps { Env = env });

app.Synth();
