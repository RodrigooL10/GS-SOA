using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FuturoDoTrabalho.Api.Data;
using FuturoDoTrabalho.Api.Repositories;
using FuturoDoTrabalho.Api.Services;
using FuturoDoTrabalho.Api.Filters;
using FuturoDoTrabalho.Api.Middlewares;

// ====================================================================================
// CONFIGURAÇÃO PRINCIPAL DA APLICAÇÃO
// ====================================================================================
// Este arquivo é responsável por configurar todos os serviços, middlewares e
// dependências da API. É o ponto de entrada da aplicação ASP.NET Core.
// ====================================================================================

var builder = WebApplication.CreateBuilder(args);

// ====================================================================================
// REGISTRO DE SERVICES
// ====================================================================================

// Adicionar serviços de controllers para processar requisições HTTP
builder.Services.AddControllers();

// Configurar AutoMapper para conversão automática entre Models e DTOs
// O AutoMapper facilita a transformação de objetos, evitando código repetitivo
builder.Services.AddAutoMapper(typeof(Program));

// ====================================================================================
// CONFIGURAÇÃO JWT AUTHENTICATION
// ====================================================================================
// Configura a autenticação baseada em JWT para validar tokens nas requisições protegidas.
// A política STATELESS garante que cada requisição é independente.
// ====================================================================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"] ?? "";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Permitir HTTP para desenvolvimento
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ====================================================================================
// CONFIGURAÇÃO DE VERSIONAMENTO DA API
// ====================================================================================
// Permite que a API tenha múltiplas versões (v1, v2) coexistindo simultaneamente.
// Isso permite evoluir a API sem quebrar integrações existentes.
// ====================================================================================
builder.Services.AddApiVersioning(options =>
{
    // Versão padrão quando nenhuma versão é especificada na requisição
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    
    // Assumir versão padrão quando não especificada
    options.AssumeDefaultVersionWhenUnspecified = true;
    
    // Reportar versões disponíveis nos headers da resposta
    options.ReportApiVersions = true;
    
    // Configurar como a versão pode ser especificada:
    // - Via header HTTP: X-API-Version
    // - Via query string: ?api-version=2.0
    options.ApiVersionReader = ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.Versioning.HeaderApiVersionReader("X-API-Version"),
        new Microsoft.AspNetCore.Mvc.Versioning.QueryStringApiVersionReader("api-version")
    );
});

// ====================================================================================
// CONFIGURAÇÃO DO SWAGGER/OPENAPI
// ====================================================================================
// Swagger fornece documentação interativa da API, permitindo testar endpoints
// diretamente no navegador. Configurado com suporte a múltiplas versões.
// ====================================================================================
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    // Documentação da versão 1 (Básica - sem PATCH e sem paginação)
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GD Solutions - API de Gestão de Funcionários",
        Version = "v1",
        Description = "API REST para gerenciamento de funcionários e departamentos - Versão 1 (Básica)",
    });
    
    // Documentação da versão 2 (Avançada - com PATCH e paginação)
    options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "GD Solutions - API de Gestão de Funcionários",
        Version = "v2",
        Description = "API REST para gerenciamento de funcionários e departamentos - Versão 2 (Avançada com paginação e PATCH)",
    });
    
    // Filtrar endpoints por versão - v1 exclui métodos PATCH
    // Garante que cada versão mostre apenas seus endpoints correspondentes
    options.DocInclusionPredicate((docName, apiDesc) =>
    {
        // Verificar versão pelo caminho da ação (namespace do controller)
        var actionDescriptor = apiDesc.ActionDescriptor;
        if (actionDescriptor == null)
            return true;

        var controllerName = actionDescriptor.DisplayName;
        
        if (docName == "v1")
        {
            // v1 não deve mostrar métodos PATCH (apenas disponível na v2)
            var isPatchMethod = apiDesc.HttpMethod?.Equals("PATCH", StringComparison.OrdinalIgnoreCase) == true;
            if (isPatchMethod)
                return false;
            
            // Mostrar apenas endpoints de controllers no namespace v1
            return controllerName?.Contains("FuturoDoTrabalho.Api.Controllers.v1") == true;
        }

        if (docName == "v2")
        {
            // Mostrar apenas endpoints de controllers no namespace v2
            return controllerName?.Contains("FuturoDoTrabalho.Api.Controllers.v2") == true;
        }

        return false;
    });
    
    // Aplicar filtro customizado para pré-preencher e bloquear parâmetro de versão
    // Isso melhora a experiência no Swagger UI
    options.OperationFilter<SetVersionParameter>();

    // Aplicar filtro customizado para adicionar exemplos reais de respostas
    // Isso faz o Swagger UI mostrar dados concretos (não vazios) em cada endpoint
    options.OperationFilter<SwaggerExampleFilter>();

    // ====================================================================================
    // CONFIGURAÇÃO DE SEGURANÇA JWT NO SWAGGER
    // ====================================================================================
    // Permite testar endpoints protegidos diretamente no Swagger UI
    // ====================================================================================
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Insira o token JWT obtido no endpoint de login",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

// ====================================================================================
// CONFIGURAÇÃO DO BANCO DE DADOS
// ====================================================================================
// Configuração do Entity Framework Core para trabalhar com MySQL.
// O Pomelo.EntityFrameworkCore.MySql é o provider oficial para MySQL no EF Core.
// ====================================================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, 
        ServerVersion.AutoDetect(connectionString),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure()) // Retry automático em caso de falha de conexão
);

// ====================================================================================
// REGISTRO DE REPOSITORIES (Padrão Repository)
// ====================================================================================
// Repositories abstraem o acesso aos dados, facilitando testes e manutenção.
// Usando injeção de dependência com escopo por requisição (AddScoped).
// ====================================================================================
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IFuncionarioRepository, FuncionarioRepository>();
builder.Services.AddScoped<IDepartamentoRepository, DepartamentoRepository>();

// ====================================================================================
// REGISTRO DE SERVICES (Camada de Lógica de Negócio)
// ====================================================================================
// Services contêm a lógica de negócio e validações.
// Atuam como intermediários entre Controllers e Repositories.
// ====================================================================================
builder.Services.AddScoped<IFuncionarioService, FuncionarioService>();
builder.Services.AddScoped<IDepartamentoService, DepartamentoService>();
builder.Services.AddScoped<IAutenticacaoService, AutenticacaoService>();

// ====================================================================================
// CONFIGURAÇÃO DE CORS (Cross-Origin Resource Sharing)
// ====================================================================================
// Permite que a API seja acessada de diferentes origens (domínios).
// Configurado para permitir todas as origens (apenas para desenvolvimento).
// Em produção, deve ser restrito a origens específicas.
// ====================================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

// ====================================================================================
// CONFIGURAÇÃO DO PIPELINE HTTP
// ====================================================================================
// Define a ordem dos middlewares que processam as requisições HTTP.
// A ordem é importante: cada middleware processa a requisição na ordem definida.
// ====================================================================================

// Servir arquivos estáticos PRIMEIRO (para não bloquear rotas de API)
app.UseStaticFiles();

// Middleware global de tratamento de exceções (deve ser o primeiro do pipeline lógico)
app.UseGlobalExceptionHandler();

// Mapear endpoint OpenAPI para documentação
app.MapOpenApi();

// Habilitar Swagger UI para documentação interativa
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "GD Solutions v1 - Básica");
    options.SwaggerEndpoint("/swagger/v2/swagger.json", "GD Solutions v2 - Avançada (com PATCH)");
    options.RoutePrefix = "swagger"; // Swagger em /swagger
    options.DefaultModelsExpandDepth(-1); // Não expandir modelos por padrão
});

// Redirecionar requisições HTTP para HTTPS (desabilitado em desenvolvimento)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Aplicar política CORS configurada anteriormente
app.UseCors("AllowAll");

// ====================================================================================
// MIDDLEWARE DE AUTENTICAÇÃO E AUTORIZAÇÃO
// ====================================================================================
// Ordem importante: SEMPRE autenticar antes de autorizar
// ====================================================================================
app.UseAuthentication();
app.UseAuthorization();

// Mapear controllers para processar rotas
app.MapControllers();

// Iniciar a aplicação e aguardar requisições
app.Run();
