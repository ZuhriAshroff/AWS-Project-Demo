resource "aws_cloudwatch_log_group" "ecs" {
  name              = "/ecs/image-gallery"
  retention_in_days = 0

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}
