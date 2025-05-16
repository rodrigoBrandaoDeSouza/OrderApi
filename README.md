# 🚀 API de Pedidos - Sistema de Gerenciamento

Sistema completo para gerenciamento de pedidos com autenticação JWT, filtros avançados e arquitetura em camadas.

---

## 🛠 Tecnologias Utilizadas

- ✅ **.NET 9 + C# 12**  
- ✅ **Entity Framework Core** (SQL Server)  
- ✅ **JWT Authentication**  
- ✅ **Swagger UI** (Documentação Interativa)  
- ✅ **Clean Architecture** (DDD + Camadas)

---

## 📌 Funcionalidades Principais

- 🔐 Autenticação com JWT Bearer Token  
- 📄 CRUD Completo de Pedidos  
- 🔍 Listagem de pedidos com retorno paginado
- 🗑 Exclusão Lógica e Física
- 
---

## ⚙️ Configuração Rápida

### ✔ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download)  
- SQL Server (ou LocalDB)  
- Visual Studio 2022 ou VS Code  

### 🚀 Passo a Passo

1. **Clone o repositório:**

```bash
git clone https://github.com/seu-usuario/pedidos-api.git
```

2. **Configure a string de conexão:**

Edite o arquivo `appsettings.json` com os dados do seu SQL Server.

3. **Execute as migrations:**

```bash
dotnet ef database update --project src/PedidosApi.Infra
```

4. **Inicie a API:**

```bash
dotnet run --project src/PedidosApi.API
```

5. **Acesse a documentação Swagger:**

[http://localhost:5000/swagger](http://localhost:5000/swagger)

---

## 🔐 Autenticação

Utilize o endpoint de login para obter um token JWT:

### Requisição

```json
POST /api/auth/login
{
  "email": "admin@pedidos.com",
  "password": "Admin@123"
}
```

> 💡 Dica: No Swagger, clique em "Authorize" e insira apenas o token (sem "Bearer").

---

## 🧭 Endpoints 

| Método | Rota                                  | Descrição                   |
|--------|---------------------------------------|-----------------------------|
| POST   | `/api/pedidos`                        | Cria um novo pedido         |
| PUT    | `/api/pedidos`                        | Atualiza um pedido          |
| GET    | `/api/pedidos?PageSize=1PageNumber=1` | Listagem pedidos paginada   |
| GET    | `/api/pedidos/{id}`                   | Retorna o pedido pelo Id    |
| DELETE | `/api/pedidos/{id}`                   | Exclusão física de pedido   |
| DELETE | `/api/pedidos/logical/{id}`           | Exclusão lógica de pedido   |

---

## 📂 Estrutura do Projeto

```
PedidosApi/
├── API/         # Controllers, Middlewares, Program.cs
├── Domain/      # Entidades, Interfaces, Regras de Negócio
├── Infra/       # EF Core, Repositórios, Migrations
└── Services/    # Casos de Uso e Lógica de Aplicação
```

---

## 📏 Regras de Negócio

- ❌ Pedidos com status **"Pago"** não podem ser excluídos  
- ✔ Valor do pedido deve ser **maior que 0**  
- ⏳ Exclusão lógica define `active = false`  

---

## 💡 Dicas para Desenvolvedores

- 👤 **Seed do banco** já inclui o usuário `admin@pedidos.com` com senha `Admin@123`  
- 🔐 **Chaves JWT** devem ser armazenadas com `dotnet user-secrets` em dev  
- 📊 **Swagger** contém exemplos prontos para testes rápidos  

---

**💻 Happy Coding! 🚀**
