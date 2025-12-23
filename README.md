# Sistema de Pedidos AWS/.NET

Este projeto será um sistema de pedidos baseado em uma arquitetura moderna de microserviços, utilizando AWS e comunicação assíncrona. O foco é estudo e prática de boas práticas em cloud, SQS, API Gateway, DynamoDB e processamento assíncrono.

# Componentes principais:

- API Gateway — Porta de entrada da aplicação e camada de autenticação (JWT).
- Order Service — Microserviço responsável por criar e gerenciar pedidos.
- DynamoDB — Banco NoSQL utilizado para armazenar pedidos.
- SQS (Order Queue) — Fila para processamento assíncrono das orders.
- Order Processor (Worker) — Serviço que consome mensagens da fila e atualiza o status dos pedidos.

![alt text](Sistema-Pedidos-1.png)