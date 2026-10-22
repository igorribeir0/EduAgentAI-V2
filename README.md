# 🎓 EduAgentAI-V2

Plataforma educacional **Full Stack com Inteligência Artificial**, desenvolvida com **ASP.NET Core (.NET 10)**, **React**, **TypeScript** e **SQL Server**.

O projeto oferece um assistente virtual para ajudar estudantes no aprendizado de idiomas. É possível criar uma conta, conversar com o Google Gemini e retomar conversas anteriores com histórico persistido no banco de dados.

## Tecnologias

| Área | Tecnologias |
| --- | --- |
| Backend | C#, .NET 10, ASP.NET Core Web API |
| Frontend | React, TypeScript, Vite, HTML e CSS |
| Banco de dados | SQL Server, Entity Framework Core |
| Autenticação | ASP.NET Core Identity, JWT |
| IA | Google Gemini API |
| Testes | xUnit |
| Desenvolvimento | Git, GitHub, OpenAPI, .NET User Secrets |

## Arquitetura

A solução separa as responsabilidades em camadas:

```text
EduAgentAI-V2/
├── src/
│   ├── EduAgentAI.Domain/          # Entidades e regras de domínio
│   ├── EduAgentAI.Application/     # Serviços, interfaces e DTOs
│   ├── EduAgentAI.Infrastructure/  # EF Core, SQL Server, Identity e Gemini
│   └── EduAgentAI.Api/             # Controllers, JWT e configuração da API
├── tests/
│   ├── EduAgentAI.Domain.Tests/
│   └── EduAgentAI.Application.Tests/
├── frontend/                       # Aplicação React + TypeScript
└── EduAgentAI.slnx
```

## Funcionalidades implementadas

- Cadastro e login de estudantes com ASP.NET Core Identity.
- Autenticação JWT e endpoints protegidos por autorização.
- Interface React com login, cadastro e opção de sair da conta.
- Chat educacional integrado à API do Google Gemini.
- Continuidade de conversas com contexto das mensagens anteriores.
- Histórico de conversas e mensagens persistido no SQL Server.
- Listagem e recuperação de conversas anteriores no frontend.
- Isolamento de conversas pelo identificador do usuário autenticado.
- Tratamento de erros na comunicação com a IA.
- Testes automatizados com xUnit.

## Como executar localmente

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js e npm](https://nodejs.org/)
- SQL Server ou SQL Server LocalDB
- Ferramenta `dotnet-ef` compatível com EF Core 10
- Chave de API do Google Gemini

### 1. Clonar o projeto

```powershell
git clone https://github.com/igorribeir0/EduAgentAI-V2.git
cd EduAgentAI-V2
```

### 2. Configurar o backend

Configure os segredos de desenvolvimento no projeto da API:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=(localdb)\MSSQLLocalDB;Database=EduAgentAI;Trusted_Connection=True;TrustServerCertificate=True;" --project .\src\EduAgentAI.Api

dotnet user-secrets set "Gemini:ApiKey" "SUA_CHAVE_GEMINI" --project .\src\EduAgentAI.Api

dotnet user-secrets set "Gemini:Model" "MODELO_GEMINI_DISPONIVEL" --project .\src\EduAgentAI.Api
```

Gere uma chave JWT aleatória e armazene-a nos User Secrets:

```powershell
$bytes = New-Object byte[] 64
$rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
try {
    $rng.GetBytes($bytes)
    $jwtKey = [Convert]::ToBase64String($bytes)
    dotnet user-secrets set "Jwt:Key" "$jwtKey" --project .\src\EduAgentAI.Api
}
finally {
    $rng.Dispose()
    Remove-Variable jwtKey, bytes, rng -ErrorAction SilentlyContinue
}

dotnet user-secrets set "Jwt:Issuer" "EduAgentAI" --project .\src\EduAgentAI.Api
dotnet user-secrets set "Jwt:Audience" "EduAgentAI.Frontend" --project .\src\EduAgentAI.Api
```

> **Segurança:** não inclua chaves, senhas ou arquivos de segredos no Git. O User Secrets é destinado ao desenvolvimento local.

### 3. Criar/atualizar o banco de dados

```powershell
dotnet restore EduAgentAI.slnx
dotnet ef database update --project .\src\EduAgentAI.Infrastructure --startup-project .\src\EduAgentAI.Api
```

### 4. Iniciar a API

Na raiz do repositório:

```powershell
dotnet run --project .\src\EduAgentAI.Api
```

Na configuração local utilizada no projeto, a API fica em `http://localhost:5186`.

### 5. Iniciar o React

Em **outro terminal**:

```powershell
cd frontend
npm install
npm run dev
```

Abra o endereço informado pelo Vite, normalmente `http://localhost:5173`.

## Endpoints principais

| Método | Endpoint | Descrição |
| --- | --- | --- |
| `POST` | `/api/auth/register` | Cadastrar usuário |
| `POST` | `/api/auth/login` | Autenticar e emitir JWT |
| `GET` | `/api/auth/me` | Dados do usuário autenticado |
| `POST` | `/api/chat/message` | Enviar mensagem ao Gemini |
| `GET` | `/api/conversations` | Listar conversas do usuário |
| `GET` | `/api/conversations/{id}/messages` | Recuperar mensagens de uma conversa |

Os endpoints de chat e histórico exigem JWT válido no cabeçalho `Authorization: Bearer <token>`.

## Testes e build

Na raiz do projeto, com a API parada:

```powershell
dotnet build EduAgentAI.slnx
dotnet test EduAgentAI.slnx
```

Para compilar o frontend:

```powershell
cd frontend
npm run build
```

## Segurança e limitações atuais

- O Identity gerencia o armazenamento de hashes de senha.
- O backend identifica o aluno a partir do JWT validado, sem confiar no `studentId` enviado pelo navegador.
- O token é mantido em memória no React: ao recarregar a página, é necessário fazer login novamente.
- O projeto é um **MVP funcional em ambiente local** e ainda não está preparado para exposição pública sem melhorias adicionais de segurança e operação.

## Próximas melhorias

- Ampliar testes automatizados, especialmente de autenticação e autorização.
- Implementar proteção contra tentativas excessivas de login e gerenciamento de sessões.
- Criar exercícios de idiomas com correção automática por IA.
- Desenvolver indicadores de evolução dos estudantes.
- Adicionar capturas de tela, containerização e deploy.

## Status

**MVP Full Stack funcional localmente**, com autenticação, chat com IA e histórico persistente por usuário.

## Autor

**Igor Ribeiro** — [GitHub](https://github.com/igorribeir0)
