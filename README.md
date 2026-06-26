# 🎮 fiap-12nett-tc-01

## 🏗️ Arquitetura

### 🔹 Domain
- Entidades de negócio
- Regras de domínio
- Interfaces de repositórios
- Value Objects
- Enums

### 🔹 Application
- Casos de uso (Use Cases)
- Serviços de aplicação
- Interfaces (contratos)

### 🔹 Infrastructure
- Persistência com Entity Framework Core
- Implementação de repositórios
- Configuração do banco de dados
- Serviços (JWT, Identity)

### 🔹 WebAPI
- Controllers REST
- Configuração do pipeline HTTP
- Middlewares customizados
- GraphQL (HotChocolate)

---

## 🧪 Tecnologias Utilizadas

- .NET 8 / ASP.NET Core
- Entity Framework Core
- SQL Server
- ASP.NET Identity
- JWT Bearer Authentication
- GraphQL (HotChocolate)
- Swagger / OpenAPI

---

## ⚙️ Configuração da Aplicação

### 🔐 SQL Connection String
```json
"ConnectionStrings": {
    "Default": ""
  },
```
### 🔐 JWT Settings
A aplicação utiliza autenticação via JWT configurada no `appsettings.json`:
```json
"Jwt": {
  "Key": "SUA_CHAVE_SECRETA",
  "Issuer": "issuer",
  "Audience": "audience"
}
```
---

## ⚙️ Rodar o projeto
### 📥 Clonar o repositório
```bash
git clone https://github.com/seu-repo/fiap-cloud-games.git
cd fiap-cloud-games
```
### 🧱 Rodar migrations
```bash
dotnet ef database update --project br.com.fiap.cloudgames.Users.Infrastructure --startup-project br.com.fiap.cloudgames.Users.WebAPI 
```
### ▶️ Executar a aplicação
```bash
dotnet run 
```


PS C:\REPOS\fiap-12nett-tc-02-userapi> dotnet ef migrations add InitialDB `
>>     --project br.com.fiap.cloudgames.Users.Infrastructure `
>>     --startup-project br.com.fiap.cloudgames.Users.WebAPI `
>>     --output-dir Persistence/Migrations