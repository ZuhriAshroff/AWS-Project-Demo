resource "aws_ecr_repository" "app" {
  name                 = "image-gallery"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = false
  }

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}
