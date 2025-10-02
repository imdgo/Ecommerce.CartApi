# 🛒 Ecommerce.CartApi

API de gerenciamento de carrinho de compras, desenvolvida em **.NET 9**, com suporte a **SQL Server** e **Redis** para cache.

## 🚀 Tecnologias utilizadas
- .NET 9
- ASP.NET Core Web API
- SQL Server (Docker)
- Redis (Docker)
- xUnit + Moq (Testes Unitários)
- Docker Compose

## ▶️ Como executar

1. Clone o repositório:
   ```bash
   git clone https://github.com/imdgo/Ecommerce.CartApi.git
   cd Ecommerce.CartApi
   
2. Suba os contêineres (API, SQL Server, Redis):
    ```bash
    docker-compose up --build
  A API estará disponível em:
  <http://localhost:5000>

📖 Documentação completa
A documentação detalhada do projeto, incluindo arquitetura, decisões técnicas e exemplos de uso, está disponível em:

👉 docs/CartApi_Documentacao.md