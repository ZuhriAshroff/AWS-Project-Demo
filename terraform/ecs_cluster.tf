resource "aws_ecs_cluster" "main" {
  name = "image-gallery-cluster"

  setting {
    name  = "containerInsights"
    value = "disabled"
  }

  configuration {
    execute_command_configuration {
      logging = "DEFAULT"
    }
  }

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}
