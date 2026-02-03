resource "aws_ecs_service" "app" {
  name                   = "image-gallery-task-service"
  cluster                = aws_ecs_cluster.main.id
  task_definition        = aws_ecs_task_definition.app.arn
  desired_count          = 1
  launch_type            = "FARGATE"
  enable_ecs_managed_tags = true

  deployment_circuit_breaker {
    enable   = true
    rollback = true
  }

  network_configuration {
    subnets          = data.aws_subnets.default.ids
    security_groups  = [aws_security_group.ecs_tasks.id]
    assign_public_ip = true
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.app.arn
    container_name   = "app"
    container_port   = 8080
  }

  # CRITICAL: Ignore task_definition and desired_count to preserve CI/CD deployments
  lifecycle {
    ignore_changes = [
      task_definition,
      desired_count,
      tags,
      tags_all,
      availability_zone_rebalancing
    ]
  }

  depends_on = [aws_lb_listener.http]
}
