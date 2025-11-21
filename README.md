# GD Solutions API - Futuro do Trabalho

A GD Solutions desenvolve soluções voltadas para modernizar a gestão de pessoas e apoiar empresas na transição para o Futuro do Trabalho, um cenário marcado por digitalização, trabalho híbrido e uso intensivo de dados para tomada de decisão.

Esta API oferece uma base estruturada e segura para o gerenciamento de funcionários, departamentos e autenticação de usuários, permitindo que sistemas corporativos realizem operações de forma organizada, segura e escalável.

## Arquitetura e Tecnologias

A API foi projetada seguindo princípios de:

- **Segurança STATELESS** com autenticação JWT
- **Autorização baseada em Roles/Perfis** de usuário
- **Tratamento centralizado de exceções**
- **Respostas padronizadas** com Response Entity
- **Organização modular** baseada em serviços mínimos e independentes
- **API Versionamento** (v1 e v2) para evolução sem quebra de compatibilidade

### Stack Tecnológico

| Tecnologia | Versão | Propósito |
|-----------|--------|----------|
| .NET | 9.0 | Runtime |
| ASP.NET Core | 9.0 | Framework Web |
| Entity Framework Core | 9.0 | ORM e Data Access |
| MySQL | 8.0+ | Banco de Dados |
| AutoMapper | 12.0.1 | Mapeamento de objetos |
| JWT Bearer | 9.0.0 | Autenticação |
| Swagger/OpenAPI | 6.6.2 | Documentação interativa |

### Padrões de Arquitetura

- **Repository Pattern** - Abstrair acesso aos dados
- **Service Pattern** - Encapsular lógica de negócio
- **DTO Pattern** - Transferência de dados segura
- **Dependency Injection** - IoC container nativo do ASP.NET
- **Value Objects** - Encapsular validações complexas (CPF, Email)
- **Middleware Pattern** - Tratamento centralizado de exceções

---

## Guia de Início Rápido

### Pré-requisitos

- **.NET 9.0+** - [Baixar](https://dotnet.microsoft.com/download/dotnet/9.0)
- **MySQL 8.0+** - [Baixar](https://dev.mysql.com/downloads/mysql/)
- **Git** - [Baixar](https://git-scm.com/)
- **Postman** (opcional) - [Baixar](https://www.postman.com/)

### 1. Clonar o Repositório

```bash
git clone https://github.com/RodrigooL10/GS-C-Sharp.git
cd GS-C-Sharp
cd FuturoDoTrabalho.Api
```

### 2. Restaurar Dependências

```bash
dotnet restore
```

### 3. Configurar Banco de Dados

#### Criar o banco MySQL (Windows CMD ou PowerShell):

```bash
mysql -u root -padmin12@ -e "CREATE DATABASE futuro_trabalho_dev CHARACTER SET utf8mb4;"
```

**Nota:** Se sua senha MySQL é diferente, substitua `admin12@` pela sua senha.

#### Atualizar ConnectionString em `appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=futuro_trabalho_dev;User=root;Password=SUA_SENHA_AQUI;"
  }
}
```

### 4. Aplicar Migrations (Criar Tabelas)

```bash
dotnet ef database update
```

Isso criará automaticamente todas as tabelas necessárias no MySQL.

### 5. Executar a Aplicação

```bash
dotnet run
```

A API será iniciada em: `https://localhost:5000` (ou a porta exibida no console)

### 6. Acessar Swagger UI

Abra no navegador:

```
http://localhost:5000
```

Você verá a documentação interativa com todas as rotas disponíveis.

---

## Autenticação e Autorização

### 1. Registrar um Novo Usuário

**POST** `/api/autenticacao/registrar`

```bash
curl -X POST http://localhost:5015/api/autenticacao/registrar \
  -H "Content-Type: application/json" \
  -d '{
    "nomeUsuario": "joao.silva",
    "email": "joao@empresa.com",
    "senha": "Senha@123",
    "nomeCompleto": "João Silva"
  }'
```

**Resposta de Sucesso (200 OK):**

```json
{
  "success": true,
  "message": "Usuário registrado com sucesso",
  "data": {
    "usuarioId": 1,
    "nomeUsuario": "joao.silva",
    "email": "joao@empresa.com",
    "nomeCompleto": "João Silva",
    "perfil": "Funcionario",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiracaoToken": "2025-11-20T12:30:00Z"
  },
  "timestamp": "2025-11-20T11:30:00Z"
}
```

### 2. Fazer Login

**POST** `/api/autenticacao/login`

```bash
curl -X POST http://localhost:5015/api/autenticacao/login \
  -H "Content-Type: application/json" \
  -d '{
    "nomeUsuario": "joao.silva",
    "senha": "Senha@123"
  }'
```

**Resposta:**

```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "data": {
    "usuarioId": 1,
    "nomeUsuario": "joao.silva",
    "email": "joao@empresa.com",
    "nomeCompleto": "João Silva",
    "perfil": "Funcionario",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expiracaoToken": "2025-11-20T12:30:00Z"
  },
  "timestamp": "2025-11-20T11:30:00Z"
}
```

**⚠️ IMPORTANTE:** Copie o valor do campo `token`. Você precisará desse token em todas as requisições protegidas.

### 3. Usar o Token em Requisições

Todas as rotas (exceto `/api/autenticacao/registrar` e `/api/autenticacao/login`) requerem autenticação.

Adicione o token no header `Authorization`:

```bash
curl -X GET http://localhost:5015/api/v1/funcionario \
  -H "Authorization: Bearer SEU_TOKEN_AQUI"
```

### 4. Testes via Swagger UI (Recomendado)

**Passo 1:** Abra `http://localhost:5015`

**Passo 2:** Clique em "Try it out" no endpoint `/api/autenticacao/registrar`:
```
POST /api/autenticacao/registrar
```

**Passo 3:** Preencha o Request Body:
```json
{
  "nomeUsuario": "usuario_teste",
  "email": "usuario@teste.com",
  "senha": "Teste@123",
  "nomeCompleto": "Usuário Teste"
}
```

**Passo 4:** Clique em "Execute" e copie o `token` da resposta

**Passo 5:** Clique no botão **"Authorize"** (cadeado 🔓 no topo direito)

**Passo 6:** Cole o token (sem a palavra "Bearer"):
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

**Passo 7:** Clique em "Authorize" e depois "Close"

**Passo 8:** Agora você pode testar os endpoints protegidos! Tente:
- **GET** `/api/v1/funcionario` - Listar funcionários
- **POST** `/api/v1/departamento` - Criar departamento (se for Admin)
- **DELETE** `/api/v1/funcionario/{id}` - Deletar (se for Admin)

---

## Perfis/Roles e Autorização

Cada usuário tem um perfil que define suas permissões:

| Perfil | GET | POST | PUT | PATCH | DELETE |
|--------|-----|------|-----|-------|--------|
| **Admin** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Gerente** | ✅ | ✅ | ✅ | ✅ | ❌ |
| **Funcionario** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Viewer** | ✅ | ❌ | ❌ | ❌ | ❌ |

### Como Alterar o Perfil de um Usuário

Você precisa acessar o banco de dados diretamente e alterar a coluna `Perfil` na tabela `usuarios`:

```sql
UPDATE usuarios SET Perfil = 'Admin' WHERE NomeUsuario = 'joao.silva';
```

**Valores válidos para Perfil:**
- `Admin` - Acesso completo
- `Gerente` - Pode criar, ler e atualizar
- `Funcionario` - Apenas leitura
- `Viewer` - Apenas leitura (alias para Funcionario)

---

## Endpoints da API

### Autenticação (SEM AUTENTICAÇÃO)

| Método | Rota | Descrição |
|--------|------|-----------|
| **POST** | `/api/autenticacao/registrar` | Registrar novo usuário |
| **POST** | `/api/autenticacao/login` | Login e obter token JWT |
| **GET** | `/api/autenticacao/verificar-token` | Verificar se token é válido |

### Funcionários v1 (Básica)

**Base:** `/api/v1/funcionario` (Requer autenticação)

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| **GET** | `/` | Todos | Listar funcionários |
| **GET** | `/{id}` | Todos | Obter funcionário específico |
| **POST** | `/` | Admin, Gerente | Criar novo funcionário |
| **PUT** | `/{id}` | Admin, Gerente | Atualizar funcionário |
| **DELETE** | `/{id}` | Admin | Deletar funcionário |

### Funcionários v2 (Avançada)

**Base:** `/api/v2/funcionario` (Requer autenticação)

Inclui tudo da v1 mais:

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| **GET** | `/?pageNumber=1&pageSize=10&ativo=true` | Todos | Listar com paginação |
| **PATCH** | `/{id}` | Admin, Gerente | Atualizar parcialmente |

### Departamentos v1 (Básica)

**Base:** `/api/v1/departamento` (Requer autenticação)

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| **GET** | `/` | Todos | Listar departamentos |
| **GET** | `/{id}` | Todos | Obter departamento específico |
| **POST** | `/` | Admin | Criar novo |
| **PUT** | `/{id}` | Admin, Gerente | Atualizar |
| **DELETE** | `/{id}` | Admin | Deletar |

### Departamentos v2 (Avançada)

**Base:** `/api/v2/departamento` (Requer autenticação)

Inclui tudo da v1 mais:

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| **GET** | `/?pageNumber=1&pageSize=10` | Todos | Listar com paginação |
| **PATCH** | `/{id}` | Admin, Gerente | Atualizar parcialmente |

---

## Exemplos de Uso

### Exemplo 1: Fluxo Completo no Swagger UI

1. Abra `http://localhost:5015`
2. Procure por `/api/autenticacao/registrar`
3. Clique "Try it out" e preencha os dados:
   ```json
   {
     "nomeUsuario": "admin",
     "email": "admin@empresa.com",
     "senha": "Admin@123",
     "nomeCompleto": "Administrador"
   }
   ```
4. Execute e copie o token
5. Clique em Authorize (cadeado) e cole o token
6. Agora teste endpoints como GET `/api/v1/funcionario`

### Exemplo 2: Criar um Funcionário (Command Line)

```bash
# 1. Login para obter token
TOKEN=$(curl -s -X POST http://localhost:5015/api/autenticacao/login \
  -H "Content-Type: application/json" \
  -d '{"nomeUsuario":"admin","senha":"Admin@123"}' \
  | grep -o '"token":"[^"]*' | cut -d'"' -f4)

# 2. Criar funcionário
curl -X POST http://localhost:5015/api/v1/funcionario \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d '{
    "nome": "João Silva",
    "cargo": "Desenvolvedor",
    "email": "joao@empresa.com",
    "cpf": "12345678901",
    "departamentoId": 1,
    "salario": 5000,
    "dataAdmissao": "2024-01-15"
  }'
```

### Exemplo 3: Listar com Paginação (v2)

```bash
curl -X GET "http://localhost:5015/api/v2/funcionario?pageNumber=1&pageSize=10&ativo=true" \
  -H "Authorization: Bearer SEU_TOKEN"
```

### Exemplo 4: Atualizar Parcialmente (PATCH)

```bash
curl -X PATCH http://localhost:5015/api/v2/funcionario/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -d '{"salario": 6000}'
```

---

## Formato de Respostas

Todas as respostas da API seguem um padrão consistente:

### Resposta de Sucesso

```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": {
    // Dados da resposta
  },
  "timestamp": "2025-11-20T11:35:00Z"
}
```

### Resposta de Erro

```json
{
  "success": false,
  "message": "Descrição do erro",
  "timestamp": "2025-11-20T11:35:00Z"
}
```

### Status Codes HTTP

| Código | Significado |
|--------|------------|
| **200** | OK - Requisição bem-sucedida |
| **201** | Created - Recurso criado |
| **204** | No Content - Deletado com sucesso |
| **400** | Bad Request - Dados inválidos |
| **401** | Unauthorized - Token ausente/inválido |
| **403** | Forbidden - Acesso negado (sem permissão) |
| **404** | Not Found - Recurso não encontrado |
| **500** | Internal Server Error - Erro no servidor |

---

## Configurações Importantes

### JWT (em `appsettings.json`)

```json
{
  "Jwt": {
    "SecretKey": "sua_chave_secreta_muito_segura_com_mais_de_32_caracteres_aqui",
    "Issuer": "FuturoDoTrabalho.Api",
    "Audience": "FuturoDoTrabalho.Client",
    "ExpiracaoMinutos": 60
  }
}
```

**⚠️ IMPORTANTE:** 
- Em produção, use uma chave secreta muito forte
- Armazene em variáveis de ambiente
- Nunca faça commit da chave secreta

### Banco de Dados

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=futuro_trabalho_dev;User=root;Password=sua_senha;"
  }
}
```

---

## Estrutura do Projeto

```
FuturoDoTrabalho.Api/
├── Controllers/               # Endpoints HTTP
│   ├── AutenticacaoController.cs
│   ├── v1/
│   │   ├── FuncionarioController.cs
│   │   └── DepartamentoController.cs
│   └── v2/
│       ├── FuncionarioController.cs
│       └── DepartamentoController.cs
├── Services/                  # Lógica de Negócio
├── Repositories/              # Acesso a Dados
├── Models/                    # Entidades
├── DTOs/                      # Transferência de Dados
├── ValueObjects/              # Validações complexas
├── Enums/                     # Enumerações
├── Middlewares/               # Middlewares
├── Data/                      # DbContext
├── Program.cs                 # Configuração
└── appsettings.json
```

---

## Comandos Úteis

```bash
# Compilar
dotnet build

# Executar
dotnet run

# Criar migration
dotnet ef migrations add NomeMigracao

# Aplicar migrations
dotnet ef database update

# Desfazer
dotnet ef migrations remove

# Recriar banco
dotnet ef database drop --force
dotnet ef database update
```

---

## Integrantes

- Adriano Lopes - RM98574
- Henrique de Brito - RM98831
- Rodrigo Lima - RM98326