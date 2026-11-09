# Studium-Infra — Project State & Agent Context (`AGENTS.md`)

> **MANDATORY AGENT DIRECTIVE:**
> After **every user prompt**, always evaluate whether we reached a useful architectural conclusion, resolved a design question, or made code/configuration changes. If so, **update both the relevant sections and Section 3 (`Recent Changes History`) in this `./AGENTS.md` file before finishing your turn** so any future chat session immediately has full context of the project state.
> If using this repository as a cross-project reference, always verify that `AGENTS.md` exists and is updated in the referencing project as well.

---

## 1. Architectural Decisions & Infrastructure Log

### 1.1 Centralized Infrastructure Repository
- **Decision:** All shared AWS CDK cloud resources (`NetworkConstruct`, `CognitoConstruct`, `DatabaseConstruct`, `MessagingConstruct`, `EcrConstruct`, `EksConstruct`, `ParametersConstruct`) for the `Studium-PUCPR` platform are centralized in this repository (`Studium-Infra`).
- **Motivation:** Avoids CloudFormation resource collisions, duplicate EKS control planes ($0.10/hr each), and duplicate RDS databases across microservice repositories. Each microservice repository retains its own `Dockerfile`, `k8s/` manifests, and `.github/workflows/ci-cd.yml` pipeline.

### 1.2 Target Cloud Platform & Region
- **Stack:** `.NET 10.0` (`net10.0`) C# AWS CDK (`Amazon.CDK.Lib` `2.275.0`).
- **Region:** `us-east-2` (São Paulo) by default.
- **Entrypoint:** `Program.cs` synthesizes `StudiumStack` via `app.Synth()`.

### 1.3 Resource Architecture & Modular Constructs
- **`NetworkConstruct`:** Resolves the shared Default VPC (`Vpc.FromLookup(..., IsDefault = true)`), avoiding dedicated NAT Gateway costs while providing public subnets for RDS and EKS nodegroups.
- **`CognitoConstruct`:** Institutional Amazon Cognito User Pool (`studium-user-pool`) with `SelfSignUpEnabled = false` for SSO/pre-provisioned institutional users, custom attribute `custom:registration_number`, OAuth 2.0 / OIDC hosted UI domain (`studium-auth`), and Cognito groups (`Student`, `Professor`, `Administrator`).
- **`DatabaseConstruct`:** Standard Amazon RDS for PostgreSQL 16 (`db.t4g.micro`, 20 GB GP2 storage, Single-AZ, Free Tier eligible). Uses `Credentials.FromGeneratedSecret("postgres", ...)` targeting Secrets Manager secret `studium/database/postgres` to automatically generate and securely store database credentials.
- **`MessagingConstruct`:** Amazon SQS queue (`studium-users-events-queue`, `studium-booking-events-queue`, `studium-notifications-queue`) with 20-second long-polling, 30-second visibility timeout, 4-day retention, and DLQ. *(Preserved on branch `origin/sqs`; active platform messaging runs on RabbitMQ)*.
- **`RabbitMQ In-Cluster Messaging` (`k8s/rabbitmq.yaml`):** In-cluster RabbitMQ deployment (`rabbitmq:3-management-alpine`) on EKS namespace `studium` with AMQP port `5672` and Management UI on port `15672`. Microservices connect via `amqp://studium:StudiumRabbitMQ2026!@studium-rabbitmq-service.studium.svc.cluster.local:5672`.
- **`EcrConstruct`:** Container repositories `studium-users`, `studium-catalog`, `studium-booking`, `studium-notifications`, `studium-front` with `MaxImageCount = 5` lifecycle rule and `EmptyOnDelete = true`.
- **`EksConstruct`:** Amazon EKS 1.31 cluster (`studium-eks-cluster`) and managed EC2 nodegroup (`studium-eks-nodes`, `t3.small`, min 1 / desired 1-2 / max 3) with IAM roles for ECR and SQS access.
- **`ParametersConstruct`:** AWS Systems Manager (SSM) Parameter Store entries:
  - `/studium/users/cognito/authority`
  - `/studium/users/cognito/client-id`
  - `/studium/users/database/endpoint`
  - `/studium/users/database/secret-name`

---

## 2. Project Structure (`./Studium-Infra`)
- [`Directory.Build.props`](./Directory.Build.props) & [`Directory.Packages.props`](./Directory.Packages.props) — CPM and shared build settings (`net10.0`, `Nullable`, `ImplicitUsings`).
- [`Studium-Infra.csproj`](./Studium-Infra.csproj) — CDK executable project.
- [`cdk.json`](./cdk.json) — CDK configuration (`dotnet run --project Studium-Infra.csproj`).
- [`Program.cs`](./Program.cs) — Composition root synthesizing `StudiumStack`.
- [`Stacks/StudiumStack.cs`](./Stacks/StudiumStack.cs) — Central CloudFormation stack composing all infrastructure constructs.
- [`k8s/rabbitmq.yaml`](./k8s/rabbitmq.yaml) — RabbitMQ deployment and ClusterIP service for EKS in namespace `studium`.
- [`.github/workflows/deploy.yml`](./.github/workflows/deploy.yml) — GitHub Actions CI/CD deploying CDK changes on push/dispatch using Node 24 and IAM credentials.
- `Constructs/`:
  - [`NetworkConstruct.cs`](./Constructs/NetworkConstruct.cs)
  - [`CognitoConstruct.cs`](./Constructs/CognitoConstruct.cs)
  - [`DatabaseConstruct.cs`](./Constructs/DatabaseConstruct.cs)
  - [`MessagingConstruct.cs`](./Constructs/MessagingConstruct.cs)
  - [`EcrConstruct.cs`](./Constructs/EcrConstruct.cs)
  - [`EksConstruct.cs`](./Constructs/EksConstruct.cs)
  - [`ParametersConstruct.cs`](./Constructs/ParametersConstruct.cs)

---

## 3. Recent Changes History
- **2026-11-08 (Initial Scaffold & Separation):**
  - Scaffolded `.NET 10` AWS CDK project with CPM (`Amazon.CDK.Lib` `2.275.0`).
  - Extracted infrastructure from `Studium-Users/Infra` into dedicated `Studium-Infra` repository.
  - Implemented modular constructs: `NetworkConstruct`, `CognitoConstruct`, `DatabaseConstruct`, `MessagingConstruct`, `EcrConstruct`, and `EksConstruct`.
- **2026-11-08 (Added ParametersConstruct & Secrets Manager Integration):**
  - Configured `Credentials.FromGeneratedSecret` in `DatabaseConstruct` with deterministic secret name `studium/database/postgres`.
  - Added `ParametersConstruct` creating SSM parameters for Cognito authority/client-id and RDS endpoint/secret-name.
  - Added `.agents/rules/maintain-agents-md.md` and repository tracking `AGENTS.md`.
- **2026-11-08 (Added Studium-Infra CI/CD & Node 24 Support):**
  - Added `.github/workflows/deploy.yml` with `workflow_dispatch` and push triggers running `cdk synth` on PRs and `cdk deploy` on main.
  - Enabled `ACTIONS_RUNNER_FORCE_ACTIONS_NODE_VERSION: node24` to address GitHub Actions Node 20 deprecation warnings.
- **2026-11-08 (Switched Default AWS Region to us-east-2):**
  - Switched default region from `sa-east-1` to `us-east-2` across `Program.cs` and `deploy.yml`.
- **2026-11-08 (Switched to Standard IAM Access Keys for CI/CD):**
  - Reverted experimental OIDC stack due to Free Tier/SCP restrictions on IAM identity providers.
  - Configured GitHub Actions to authenticate via `AWS_ACCESS_KEY_ID` and `AWS_SECRET_ACCESS_KEY`.
- **2026-11-08 (Allowed Ingress on PostgreSQL RDS):**
  - Added `Instance.Connections.AllowDefaultPortFromAnyIpv4()` to `DatabaseConstruct.cs` ensuring EKS pods can access PostgreSQL port 5432.
- **2026-11-08 (Added Wolverine DLQ to SQS Infrastructure):**
  - Added `wolverine-dead-letter-queue` and attached redrive policy to `studium-users-events-queue` in `Constructs/MessagingConstruct.cs`.
- **2026-11-08 (Added Catalog, Booking, Notifications & Front ECR and SQS Queues):**
  - Updated `EcrConstruct.cs` provisioning container repositories for `studium-catalog`, `studium-booking`, `studium-notifications`, and `studium-front` (in addition to `studium-users`).
  - Updated `MessagingConstruct.cs` provisioning `studium-booking-events-queue` and `studium-notifications-queue` (in addition to `studium-users-events-queue`), all bound with DLQ redrive policies to `wolverine-dead-letter-queue`.
- **2026-11-09 (RabbitMQ Deployment Manifest & SQS Storage Branches):**
  - Created `k8s/rabbitmq.yaml` containing the complete in-cluster RabbitMQ deployment and ClusterIP service for EKS (`studium-rabbitmq-service:5672`).
  - Preserved AWS SQS configurations on `origin/sqs` branches across all microservices and documented RabbitMQ migration for `main`.
