resource "aws_ecs_task_definition" "app" {
  family                   = "image-gallery-task"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = "1024"
  memory                   = "3072"
  execution_role_arn       = aws_iam_role.ecs_task_execution.arn
  task_role_arn            = aws_iam_role.ecs_task.arn

  runtime_platform {
    cpu_architecture        = "X86_64"
    operating_system_family = "LINUX"
  }

  container_definitions = jsonencode([
    {
      name      = "app"
      image     = "${aws_ecr_repository.app.repository_url}:latest"
      essential = true

      portMappings = [{
        containerPort = 8080
        hostPort      = 8080
        protocol      = "tcp"
      }]

      environment = [
        { name = "AWS__Region", value = var.aws_region },
        { name = "AWS__S3BucketName", value = aws_s3_bucket.images.bucket },
        { name = "AWS__DynamoDbTableName", value = aws_dynamodb_table.metadata.name }
      ]

      logConfiguration = {
        logDriver = "awslogs"
        options = {
          "awslogs-group"         = aws_cloudwatch_log_group.ecs.name
          "awslogs-region"        = var.aws_region
          "awslogs-stream-prefix" = "ecs"
        }
      }
    }
  ])

  # CRITICAL: Ignore container_definitions changes to preserve CI/CD deployed images
  lifecycle {
    ignore_changes = [
      container_definitions,
      tags,
      tags_all
    ]
  }
}
