# GD Solutions API - Futuro do Trabalho

> **Observação:** Este projeto é uma **adaptação da entrega da matéria de C#**, reutilizada e expandida para atender aos requisitos da disciplina de SOA.

## 📌 Sobre o Projeto
A GD Solutions desenvolve soluções voltadas para modernizar a gestão de pessoas e apoiar empresas na transição para o Futuro do Trabalho, um cenário marcado por digitalização, trabalho híbrido e uso intensivo de dados para tomada de decisão.

Esta API oferece uma base estruturada e segura para o gerenciamento de funcionários, departamentos e autenticação de usuários, permitindo que sistemas corporativos realizem operações de forma organizada, segura e escalável.



## 🚀 Início Rápido

### Pré-requisitos

- **.NET 9.0+** - [Baixar](https://dotnet.microsoft.com/download/dotnet/9.0)
- **MySQL 8.0+** - [Baixar](https://dev.mysql.com/downloads/mysql/)
- **Git** - [Baixar](https://git-scm.com/)

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
mysql -u root -padmin12@ -e "CREATE DATABASE futuro_trabalho CHARACTER SET utf8mb4;"
```

**Nota:** Se sua senha MySQL é diferente, substitua `admin12@` pela sua senha.

#### Atualizar ConnectionString em `appsettings.Development.json` e `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=futuro_trabalho;User=root;Password=SUA_SENHA_AQUI;"
  }
}
```

### 4. Criar e aplicar Migrations (Criar Tabelas)

```bash
dotnet ef migrations add Initial 

dotnet ef database update
```

Isso criará automaticamente todas as tabelas necessárias no MySQL.

### 5. Executar a Aplicação

```bash
dotnet run
```

A API será iniciada em: `https://localhost:5000` (ou a porta exibida no console)

### 6. Acessar API

Abra no navegador:

```
http://localhost:5000/auth.html
```

Você verá uma tela de cadastro e login.

### 7. Crie seu usuário e faça login para obter o token JWT necessário para acessar os endpoints protegidos.

Realize o cadastro e login para obter o token JWT necessário para acessar os endpoints protegidos da API. Copie o token retornado na resposta de login, pois ele será necessário para autenticar suas requisições.

### 8. Acesse o Swagger UI

Abra no navegador:

```
http://localhost:5000/swagger/index.html
```

ou apenas clique no botão abaixo da tela de login "Ver Swagger UI".

### 9. Coloque seu token JWT no Swagger

Clique no botão **"Authorize"** (cadeado 🔓 no topo direito) e cole o token JWT.

### 10. Teste os Endpoints

Agora você pode testar todos os endpoints protegidos da API diretamente pelo Swagger UI!

---


## 🧩 Perfis/Roles e Autorização

Cada usuário tem um perfil que define suas permissões:

| Perfil | GET | POST | PUT | PATCH | DELETE |
|--------|-----|------|-----|-------|--------|
| **Admin** | ✅ | ✅ | ✅ | ✅ | ✅ |
| **Gerente** | ✅ | ✅ | ✅ | ✅ | ❌ |
| **Funcionario** | ✅ | ❌ | ❌ | ❌ | ❌ |

### Como Alterar o Perfil de um Usuário

Você precisa acessar o banco de dados diretamente e alterar a coluna `Perfil` na tabela `usuarios`:

```sql
UPDATE usuarios SET Perfil = 'Admin' WHERE NomeUsuario = 'joao.silva';
```

**Valores válidos para Perfil:**
- `Admin` - Acesso completo
- `Gerente` - Pode criar, ler e atualizar
- `Funcionario` - Apenas leitura

---

## 🔗 Endpoints da API

### 🔓 Autenticação (SEM AUTENTICAÇÃO)

| Método | Rota | Descrição |
|--------|------|-----------|
| **POST** | `/api/autenticacao/registrar` | Registrar novo usuário |
| **POST** | `/api/autenticacao/login` | Login e obter token JWT |
| **GET** | `/api/autenticacao/verificar-token` | Verificar se token é válido |

### v1 - Básica
Endpoints: `/api/v1/funcionario` e `/api/v1/departamento`


| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| **GET** | `/` | Todos | Listar funcionários |
| **GET** | `/{id}` | Todos | Obter funcionário específico |
| **POST** | `/` | Admin, Gerente | Criar novo funcionário |
| **PUT** | `/{id}` | Admin, Gerente | Atualizar funcionário |
| **DELETE** | `/{id}` | Admin | Deletar funcionário |

### v2 - Avançada

Inclui tudo da v1 mais:

| Método | Rota | Roles | Descrição |
|--------|------|-------|-----------|
| **GET** | `/?pageNumber=1&pageSize=10&ativo=true` | Todos | Listar com paginação |
| **PATCH** | `/{id}` | Admin, Gerente | Atualizar parcialmente |

---

## 📚 Exemplos de Uso

### Exemplo 1: Fluxo Completo no Swagger UI

1. Após fazer cadastro/login em `http://localhost:5000/auth.html`, abra `http://localhost:5000/swagger/index.html`
2. Procure por `Authentication` e insira seu token JWT
3. Agora teste endpoints como GET `/api/v1/funcionario`

### Exemplo 2: Criar um Funcionário (Command Line)

```bash
# 1. Login para obter token
TOKEN=$(curl -s -X POST http://localhost:5000/api/autenticacao/login \
  -H "Content-Type: application/json" \
  -d '{"nomeUsuario":"admin","senha":"Admin@123"}' \
  | grep -o '"token":"[^"]*' | cut -d'"' -f4)

# 2. Criar funcionário
curl -X POST http://localhost:5000/api/v1/funcionario \
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
curl -X GET "http://localhost:5000/api/v2/funcionario?pageNumber=1&pageSize=10&ativo=true" \
  -H "Authorization: Bearer SEU_TOKEN"
```

### Exemplo 4: Atualizar Parcialmente (PATCH)

```bash
curl -X PATCH http://localhost:5000/api/v2/funcionario/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer SEU_TOKEN" \
  -d '{"salario": 6000}'
```

---

## 📤 Formato de Respostas

Todas as respostas da API seguem um padrão consistente:

### ✅ Resposta de Sucesso

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

### ❌ Resposta de Erro

```json
{
  "success": false,
  "message": "Descrição do erro",
  "timestamp": "2025-11-20T11:35:00Z"
}
```

### 📡 Status Codes HTTP

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

## 🛠️ Configurações Importantes

### 🔑 JWT (em `appsettings.json`)

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


### 🗄️ Banco de Dados

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=futuro_trabalho_dev;User=root;Password=sua_senha;"
  }
}
```

---

## 🧰 Comandos Úteis

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

## 📁 Estrutura do Projeto

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

## 🧱 Arquitetura e Tecnologias

A API foi projetada seguindo princípios de:

- **Segurança STATELESS** com autenticação JWT
- **Autorização baseada em Roles/Perfis** de usuário
- **Tratamento centralizado de exceções**
- **Respostas padronizadas** com Response Entity
- **Organização modular** baseada em serviços mínimos e independentes
- **API Versionamento** (v1 e v2) para evolução sem quebra de compatibilidade

---

### Tecnologias Utilizadas

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


## 🧑‍💻 Integrantes

- Adriano Lopes - RM98574
- Henrique de Brito - RM98831
- Rodrigo Lima - RM98326
