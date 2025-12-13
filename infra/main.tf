provider "aws" {
    region = "us-east-1"
    profile = "default"
}

# Tabela de Pedidos (DynamoDB)
resource "aws_dynamodb_table" "orders" {
    name         = "Orders"
    billing_mode = "PAY_PER_REQUEST"
    hash_key     = "Id"

    attribute {
        name = "Id"
        type = "S"
    }

    tags = {
        Environment = "dev"
        Name        = "SistemaPedidos"
    }
}

# Fila de Pedidos (SQS)
resource "aws_sqs_queue" "orders_queue" {
    name                      = "Orders_queue"
    delay_seconds             = 0
    max_message_size          = 2048
    message_retention_seconds = 86400 # 1 dia de rentenção
    receive_wait_time_seconds = 10   
  
}

output "queue_url" {
  value = aws_sqs_queue.orders_queue.id
}