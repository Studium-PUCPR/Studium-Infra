# Studium-Infra

Centralized `.NET 10` C# AWS CDK (`Amazon.CDK.Lib`) infrastructure repository for the **Studium** platform (`Studium-PUCPR`).

## Provisioned Resources (`StudiumStack`)
- **`NetworkConstruct`:** Shared Default VPC (`StudiumDefaultVpc`).
- **`CognitoConstruct`:** Institutional Amazon Cognito User Pool (`studium-user-pool`), OAuth 2.0 / OIDC SSO domain (`studium-auth`), SPA client (`studium-web-client`), and role groups (`Student`, `Professor`, `Administrator`).
- **`DatabaseConstruct`:** Free Tier Amazon RDS for PostgreSQL 16 (`db.t4g.micro`, 20 GB GP2, Single-AZ).
- **`MessagingConstruct`:** Amazon SQS queues (`studium-users-events-queue`, `studium-booking-events-queue`, `studium-notifications-queue`). *(Preserved for reference; active platform messaging runs via in-cluster **RabbitMQ** at `k8s/rabbitmq.yaml`)*.
- **`RabbitMQ Cluster Component` (`k8s/rabbitmq.yaml`):** In-cluster RabbitMQ with Management UI running on EKS in namespace `studium` (`studium-rabbitmq-service:5672`).

> **Note on Messaging Architecture:** Active platform messaging uses **RabbitMQ** (AMQP) across all microservices. The cloud-native AWS SQS configuration is safely preserved on git branch `origin/sqs`.

## Commands
```bash
dotnet build
cdk synth
cdk deploy
```
