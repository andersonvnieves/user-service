# User Service

Serviço de usuários e autenticação. Registra usuários, realiza login, emite JWTs e permite a administradores consultar usuários ou alterar suas roles. Os dados são persistidos no SQL Server e novos usuários geram o evento `user.created` no RabbitMQ.

## Componentes

| Projeto | Responsabilidade |
| --- | --- |
| `br.com.fiap.cloudgames.Users.Domain` | Regras de negócio, agregados e contratos. |
| `br.com.fiap.cloudgames.Users.Application` | Casos de uso, serviços e contratos de integração. |
| `br.com.fiap.cloudgames.Users.Infrastructure` | ASP.NET Identity, SQL Server/EF Core, JWT e RabbitMQ. |
| `br.com.fiap.cloudgames.Users.WebAPI` | API REST, Swagger, seed de roles e usuário inicial. |

## Pré-requisitos

- .NET SDK 10;
- SQL Server na porta `1433`;
- RabbitMQ na porta `5672` (painel: `15672`).

Para subir todos os serviços e suas dependências com Docker, veja o [README da orquestração](https://github.com/andersonvnieves/orchestration/blob/main/README.md).

## Configuração local

O perfil `Development` usa `br.com.fiap.cloudgames.Users.WebAPI/appsettings.Development.json`. Ele também define o usuário inicial. Para manter segredos fora do arquivo, use variáveis de ambiente:

```powershell
$env:ConnectionStrings__Default = "Server=localhost,1433;Database=FGC_User;User Id=sa;Password=<SENHA>;TrustServerCertificate=True;"
$env:Jwt__Issuer = "fgcapi"
$env:Jwt__Audience = "fgcapi"
$env:Jwt__Key = "<CHAVE_JWT>"
$env:RootUser__FirstName = "Admin"
$env:RootUser__LastName = "Cloud Games"
$env:RootUser__Email = "admin@cloudgames.local"
$env:RootUser__Password = "<SENHA_FORTE>"
$env:RabbitMQ__URI = "amqp://<USUARIO>:<SENHA>@localhost:5672/"
$env:RabbitMQ__UserCreatedEvent__Exchange = "fgc"
$env:RabbitMQ__UserCreatedEvent__RoutingKey = "user.created"
```

## Executar localmente

Na pasta `user-service`:

```powershell
dotnet restore .\br.com.fiap.cloudgames.Users.sln
dotnet run --project .\br.com.fiap.cloudgames.Users.WebAPI\br.com.fiap.cloudgames.Users.WebAPI.csproj --launch-profile http
```

A API atende em `http://localhost:5149` e o Swagger em `http://localhost:5149/swagger`. As migrations, roles e o usuário inicial são aplicados/criados na inicialização.

## Endpoints principais

| Método | Rota | Acesso |
| --- | --- | --- |
| `POST` | `/api/User` | Público |
| `POST` | `/api/Auth/login` | Público |
| `GET` | `/api/User?Id={id}` | JWT com role `admin` |
| `PATCH` | `/api/User/role` | JWT com role `admin` |

## Testes

```powershell
dotnet test .\br.com.fiap.cloudgames.Users.sln
```

## Contêiner

```powershell
docker build -t fgc-user-service:latest .
docker run --rm -p 8080:8080 fgc-user-service:latest
```

Ao executar a imagem isoladamente, informe banco, JWT, usuário inicial e RabbitMQ por variáveis de ambiente.
