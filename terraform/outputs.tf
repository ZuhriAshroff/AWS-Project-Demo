output "alb_dns_name" {
  description = "DNS name of the load balancer"
  value       = aws_lb.main.dns_name
}

output "ecr_repository_url" {
  description = "ECR repository URL"
  value       = aws_ecr_repository.app.repository_url
}

output "ecs_cluster_name" {
  description = "ECS cluster name"
  value       = aws_ecs_cluster.main.name
}

output "ecs_service_name" {
  description = "ECS service name"
  value       = aws_ecs_service.app.name
}

output "s3_bucket_name" {
  description = "S3 bucket for images"
  value       = aws_s3_bucket.images.bucket
}

output "dynamodb_table_name" {
  description = "DynamoDB table for metadata"
  value       = aws_dynamodb_table.metadata.name
}

