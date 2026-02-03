resource "aws_s3_bucket" "images" {
  bucket = "zuhri-image-gallery-1769082605"

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}
