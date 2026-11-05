Studium-Infra

Stack: Terraform + Kubernetes YAML manifests (k8s/).

Role: Provisions the Amazon EKS cluster, NGINX Ingress Controller, SNS Topics + SQS Queues (with IAM IRSA / Pod Identity permissions so EKS pods can poll SQS), and Amazon Cognito.
