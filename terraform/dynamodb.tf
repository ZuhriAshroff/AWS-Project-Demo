resource "aws_dynamodb_table" "metadata" {
  name         = "image-gallery-metadata-dev"
  billing_mode = "PAY_PER_REQUEST"
  hash_key     = "id"

  attribute {
    name = "id"
    type = "S"
  }

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}
