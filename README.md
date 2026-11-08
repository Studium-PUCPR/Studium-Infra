# Studium-Infra

Centralized `.NET 10` C# AWS CDK (`Amazon.CDK.Lib`) infrastructure repository for the **Studium** platform (`Studium-PUCPR`).

## Provisioned Resources (`StudiumStack`)
- **`NetworkConstruct`:** Shared Default VPC (`StudiumDefaultVpc`).
- **`CognitoConstruct`:** Institutional Amazon Cognito User Pool (`studium-user-pool`), OAuth 2.0 / OIDC SSO domain (`studium-auth`), SPA client (`studium-web-client`), and role groups (`Student`, `Professor`, `Administrator`).
- **`DatabaseConstruct`:** Free Tier Amazon RDS for PostgreSQL 16 (`db.t4g.micro`, 20 GB GP2, Single-AZ).
- **`MessagingConstruct`:** Amazon SQS queue (`studium-users-events-queue`) with 20s long-polling.

## Commands
```bash
dotnet build
cdk synth
cdk deploy
```
