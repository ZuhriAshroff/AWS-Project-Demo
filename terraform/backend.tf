terraform {
  backend "s3" {
    bucket         = "zuhri-image-gallery-1769082605"
    key            = "terraform/image-gallery.tfstate"
    region         = "us-east-1"
    dynamodb_table = "terraform-state-locks"
    encrypt        = true
  }
}