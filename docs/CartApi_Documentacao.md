

## 2. Arquitetura da Solução
- **Backend:** .NET 8 (Web API)  
- **Banco de Dados:** SQL Server 2022 (container Docker)  
- **Cache Distribuído:** Redis (container Docker)  
- **Padrões Utilizados:**
  - Repository Pattern
  - Service Layer
  - Cache-aside (write-through simplificado)
- **Infraestrutura:** Docker Compose para orquestração dos serviços  

---

## 3. Estrutura de Pastas
```plaintext
Ecommerce.CartApi/
│── CartApi/
│   ├── Controllers/        # Endpoints da API
│   ├── Models/             # Entidades e DTOs
│   ├── Repositories/       # Interfaces e implementação de acesso a dados
│   ├── Services/           # Regras de negócio (ex: CartService)
│   ├── Program.cs          # Configuração principal
│   ├── appsettings.json    # Configurações (DB, Redis, Logging)
│
└── docker-compose.yml      # Orquestração dos containers
```

---

## 4. Configuração de Dependências

### Banco de Dados (SQL Server)
- **Imagem:** `mcr.microsoft.com/mssql/server:2022-latest`
- **Porta exposta:** `1433`
- **Variáveis de ambiente:**
  - `SA_PASSWORD`: Senha forte (mín. 8 caracteres, maiúscula, minúscula, número, caractere especial)
  - `ACCEPT_EULA=Y`

### Redis
- **Imagem:** `redis:7`
- **Porta exposta:** `6379`

---

## 5. Arquivo `appsettings.json`
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",

  "ConnectionStrings": {
    "DefaultConnection": "Server=sqlserver;Database=EcommerceDb;User=sa;Password=Your_strong_Password123;TrustServerCertificate=True;"
  },

  "Redis": {
    "Connection": "redis:6379",
    "CacheExpirationMinutes": 10
  }
}
```

---

## 6. Serviço de Carrinho (`CartService`)
- **Cache Key:** `cart_total_{cartId}`
- **Fluxo:**
  1. Consulta Redis para verificar se o total já está em cache.  
  2. Se não estiver, calcula a partir do banco, salva no Redis e retorna.  
  3. Ao adicionar/remover itens ou aplicar descontos → **invalida o cache** com `RemoveAsync`.  

Esse padrão reduz leituras repetitivas do banco e melhora a performance em cenários de alta carga.  

---

## 7. Comandos Úteis

### Subir ambiente
```bash
docker-compose up -d
```

### Verificar saúde dos containers
```bash
docker ps
docker inspect --format='{{json .State.Health}}' cart-sql | jq
```

### Acessar container SQL Server
```bash
docker exec -it cart-sql /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "YourPassword"
```

### Acessar container Redis
```bash
docker exec -it cart-redis redis-cli
```

---

## 8. Testes
- **Redis:** mockado em testes unitários para não depender de infraestrutura externa.  
- **Banco de dados:** pode ser mockado ou substituído por `InMemoryDatabase` no Entity Framework.  

---

## 9. Próximos Passos / Melhorias
- Usar `ConnectionMultiplexer` (StackExchange.Redis) para cache avançado (ex.: listas de carrinhos, lock distribuído).  
- Criar testes de integração com containers reais (usando Testcontainers).  
- Implementar logs estruturados com **Winston** ou **Serilog**.  
- Configurar CI/CD para build e deploy automatizados.  
