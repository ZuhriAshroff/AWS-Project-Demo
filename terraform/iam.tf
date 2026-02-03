# Task Execution Role (for ECS agent to pull images, write logs)
resource "aws_iam_role" "ecs_task_execution" {
  name        = "image-gallery-ecs-task-exec"
  description = "Allows ECS to create and manage AWS resources on your behalf."

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect = "Allow"
      Principal = {
        Service = "ecs-tasks.amazonaws.com"
      }
      Action = "sts:AssumeRole"
    }]
  })

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}

resource "aws_iam_role_policy_attachment" "ecs_task_execution" {
  role       = aws_iam_role.ecs_task_execution.name
  policy_arn = "arn:aws:iam::aws:policy/service-role/AmazonECSTaskExecutionRolePolicy"
}

# Task Role (for application to access AWS services)
resource "aws_iam_role" "ecs_task" {
  name        = "image-gallery-ecs-task-role"
  description = "Allows ECS to create and manage AWS resources on your behalf."

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect = "Allow"
      Principal = {
        Service = "ecs-tasks.amazonaws.com"
      }
      Action = "sts:AssumeRole"
    }]
  })

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}

# Inline policy for task role (S3, DynamoDB access) - matches actual AWS policy
resource "aws_iam_role_policy" "ecs_task_policy" {
  name = "image-gallery-ecs-task-role-policy"
  role = aws_iam_role.ecs_task.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        Effect = "Allow"
        Action = [
          "s3:GetObject",
          "s3:PutObject",
          "s3:AbortMultipartUpload",
          "s3:ListBucket"
        ]
        Resource = [
          "arn:aws:s3:::zuhri-image-gallery-1769082605",
          "arn:aws:s3:::zuhri-image-gallery-1769082605/*"
        ]
      },
      {
        Effect = "Allow"
        Action = [
          "dynamodb:GetItem",
          "dynamodb:PutItem",
          "dynamodb:Scan",
          "dynamodb:Query",
          "dynamodb:UpdateItem"
        ]
        Resource = "arn:aws:dynamodb:us-east-1:372405506987:table/image-gallery-metadata-dev"
      }
    ]
  })
}
