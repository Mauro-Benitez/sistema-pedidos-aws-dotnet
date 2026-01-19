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

# ----- Autenticação Cognito -----

# 1. Banco de usuários
resource "aws_cognito_user_pool" "users" {
    name = "OrderSystemUsers"

    password_policy {
        minimum_length    = 10
        require_lowercase = true
        require_numbers   = true
        require_symbols   = true
        require_uppercase = true

        temporary_password_validity_days = 7
    }

    auto_verified_attributes = ["email"]
}
    
# 2. O cliente da aplicação
resource "aws_cognito_user_pool_client" "client" {
    name         = "order-system-client"
    user_pool_id = aws_cognito_user_pool.users.id

    # Fluxo ADMIN_NO_SRP_AUTH 
   explicit_auth_flows = [
    "ALLOW_USER_PASSWORD_AUTH", 
    "ALLOW_REFRESH_TOKEN_AUTH",
    "ALLOW_ADMIN_USER_PASSWORD_AUTH"
  ]
}

# 3. Dominio
resource "aws_cognito_user_pool_domain" "main" {
  domain       = "sistema-pedidos-${random_id.suffix.hex}" 
  user_pool_id = aws_cognito_user_pool.users.id
}

resource "random_id" "suffix" {
  byte_length = 4
}


# ----- API GATEWAY -----

# 1. API Gateway do tipo HTTP 
resource "aws_apigatewayv2_api" "gateway" {
  name          = "OrderSystemGateway"
  protocol_type = "HTTP"
}

# 2. Configura o "Authorizer" (Conecta o Gateway ao Cognito)
resource "aws_apigatewayv2_authorizer" "auth" {
  api_id           = aws_apigatewayv2_api.gateway.id
  authorizer_type  = "JWT"
  identity_sources = ["$request.header.Authorization"]
  name             = "CognitoAuth"

  jwt_configuration {
    audience = [aws_cognito_user_pool_client.client.id]
    issuer   = "https://${aws_cognito_user_pool.users.endpoint}"
  }
}

# 3. Cria uma Rota Protegida (POST /orders)
resource "aws_apigatewayv2_route" "create_order" {
  api_id    = aws_apigatewayv2_api.gateway.id
  route_key = "POST /orders"
  
  # AQUI ESTÁ A MÁGICA: Só passa se tiver token válido
  authorization_type = "JWT"
  authorizer_id      = aws_apigatewayv2_authorizer.auth.id

  target = "integrations/${aws_apigatewayv2_integration.backend.id}"
}

# 4. Integração (Para onde o Gateway manda o pedido?)
# NOTA: Como não temos IP público ainda, vamos apontar para um mock
resource "aws_apigatewayv2_integration" "backend" {
  api_id           = aws_apigatewayv2_api.gateway.id
  integration_type = "HTTP_PROXY"
  integration_uri  = "https://httpbin.org/post" # Mock para teste
  integration_method = "POST"
}

# 5. Deploy automático (Stage)
resource "aws_apigatewayv2_stage" "default" {
  api_id      = aws_apigatewayv2_api.gateway.id
  name        = "$default"
  auto_deploy = true
}

# --- OUTPUTS ---
output "cognito_user_pool_id" {
  value = aws_cognito_user_pool.users.id
  sensitive   = true
}

output "api_gateway_url" {
  value = aws_apigatewayv2_stage.default.invoke_url
}


