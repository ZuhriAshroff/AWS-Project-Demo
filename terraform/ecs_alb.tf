resource "aws_lb" "main" {
  name               = "image-gallery-alb"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.alb.id]
  subnets            = data.aws_subnets.default.ids

  enable_deletion_protection = false

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}

resource "aws_lb_target_group" "app" {
  name        = "image-gallery-tg"
  port        = 8080
  protocol    = "HTTP"
  vpc_id      = data.aws_vpc.default.id
  target_type = "ip"

  health_check {
    enabled             = true
    healthy_threshold   = 5
    interval            = 30
    matcher             = "200"
    path                = "/api/health"
    port                = "traffic-port"
    timeout             = 5
    unhealthy_threshold = 2
  }

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}

resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.main.arn
  port              = 80
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.app.arn
  }

  lifecycle {
    ignore_changes = [tags, tags_all]
  }
}

