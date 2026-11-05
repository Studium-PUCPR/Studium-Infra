# Studium-Infra (DevOps & Cloud Configuration)

Stack: Terraform + Docker Compose (with LocalStack).

Responsibilities:

Provisions Amazon Cognito (User Pool + Estudante, Professor, Administrador groups), AWS API Gateway (HTTP API with Cognito JWT Authorizer), SNS Topics, SQS Queues (plus SNS→SQS fan-out subscriptions), and Amazon SES.

Provides a unified docker-compose.yml with LocalStack and PostgreSQL/SQL Server instances so all 5 team members can run the entire cloud stack locally for free.
