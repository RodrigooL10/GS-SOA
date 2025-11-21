using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Any;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FuturoDoTrabalho.Api.Filters
{
    // ====================================================================================
    // FILTER: SwaggerExampleFilter
    // ====================================================================================
    // Provides realistic example data in Swagger UI response documentation
    // Replaces empty arrays with actual sample data for better API testing experience
    // Implements IOperationFilter to modify OpenAPI operation metadata
    // ====================================================================================
    public class SwaggerExampleFilter : IOperationFilter
    {
        // ====================
        // FIELDS
        // ====================
        // JSON serializer options configured for camelCase property naming
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // ====================
        // METHOD: Apply
        // Main filter method that adds example data to operation responses
        // Detects endpoint type and creates appropriate sample data
        // ====================
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var returnType = context.ApiDescription.ActionDescriptor.GetType();
            var methodName = context.MethodInfo.Name;
            var controllerName = context.MethodInfo.DeclaringType?.Name;

            // Exemplos para GET todos os Departamentos (v1 e v2)
            if ((methodName == "GetAll" || methodName == "GetPaged") && controllerName?.Contains("Departamento") == true)
            {
                if (operation.Responses.ContainsKey("200"))
                {
                    var exemploDepartamentos = new
                    {
                        success = true,
                        message = methodName == "GetPaged" ? "Departamentos listados com paginação" : "Departamentos listados com sucesso",
                        data = methodName == "GetPaged" ? new
                        {
                            data = new object[]
                            {
                                new
                                {
                                    id = 1,
                                    nome = "Tecnologia",
                                    descricao = "Departamento de TI e Desenvolvimento",
                                    lider = "João Silva",
                                    ativo = true,
                                    dataCriacao = "2024-10-21T00:00:00Z",
                                    dataAtualizacao = "2024-11-15T00:00:00Z"
                                },
                                new
                                {
                                    id = 2,
                                    nome = "Recursos Humanos",
                                    descricao = "Departamento de RH",
                                    lider = "Maria Santos",
                                    ativo = true,
                                    dataCriacao = "2024-09-10T00:00:00Z",
                                    dataAtualizacao = (string?)null
                                }
                            },
                            pageNumber = 1,
                            pageSize = 10,
                            totalCount = 2,
                            totalPages = 1
                        } as object : new object[]
                        {
                            new
                            {
                                id = 1,
                                nome = "Tecnologia",
                                descricao = "Departamento de TI e Desenvolvimento",
                                lider = "João Silva",
                                ativo = true,
                                dataCriacao = "2024-10-21T00:00:00Z",
                                dataAtualizacao = "2024-11-15T00:00:00Z"
                            },
                            new
                            {
                                id = 2,
                                nome = "Recursos Humanos",
                                descricao = "Departamento de RH",
                                lider = "Maria Santos",
                                ativo = true,
                                dataCriacao = "2024-09-10T00:00:00Z",
                                dataAtualizacao = (string?)null
                            }
                        },
                        timestamp = DateTime.UtcNow.ToString("O")
                    };

                    AdicionarExemplo(operation, "200", exemploDepartamentos);
                }
            }

            // Exemplos para GET um Departamento por ID
            if (methodName == "GetById" && controllerName?.Contains("Departamento") == true)
            {
                if (operation.Responses.ContainsKey("200"))
                {
                    var exemploDepartamento = new
                    {
                        success = true,
                        message = "Departamento retornado com sucesso",
                        data = new
                        {
                            id = 1,
                            nome = "Tecnologia",
                            descricao = "Departamento de TI e Desenvolvimento",
                            lider = "João Silva",
                            ativo = true,
                            dataCriacao = "2024-10-21T00:00:00Z",
                            dataAtualizacao = "2024-11-15T00:00:00Z"
                        },
                        timestamp = DateTime.UtcNow.ToString("O")
                    };

                    AdicionarExemplo(operation, "200", exemploDepartamento);
                }
            }

            // Exemplos para GET todos os Funcionários
            if ((methodName == "GetAll" || methodName == "GetPaged") && controllerName?.Contains("Funcionario") == true)
            {
                if (operation.Responses.ContainsKey("200"))
                {
                    var exemploFuncionarios = new
                    {
                        success = true,
                        message = methodName == "GetPaged" ? "Funcionários listados com paginação" : "Funcionários listados com sucesso",
                        data = methodName == "GetPaged" ? new
                        {
                            data = new object[]
                            {
                                new
                                {
                                    id = 1,
                                    nome = "Carlos Silva",
                                    cargo = "Desenvolvedor Senior",
                                    cpf = "123.456.789-00",
                                    email = "carlos.silva@gdsolutions.com",
                                    telefone = "(11) 99999-9999",
                                    dataAdmissao = "2021-11-20T00:00:00Z",
                                    departamentoId = 1,
                                    departamentoNome = "Tecnologia",
                                    salario = 8500.00,
                                    endereco = "Rua das Flores, 123, São Paulo",
                                    nivelSenioridade = 4,
                                    ativo = true,
                                    dataCriacao = "2024-10-21T00:00:00Z",
                                    dataAtualizacao = (string?)null
                                },
                                new
                                {
                                    id = 2,
                                    nome = "Ana Costa",
                                    cargo = "Desenvolvedora Pleno",
                                    cpf = "987.654.321-00",
                                    email = "ana.costa@gdsolutions.com",
                                    telefone = "(11) 98888-8888",
                                    dataAdmissao = "2023-06-15T00:00:00Z",
                                    departamentoId = 1,
                                    departamentoNome = "Tecnologia",
                                    salario = 6500.00,
                                    endereco = "Av. Paulista, 456, São Paulo",
                                    nivelSenioridade = 3,
                                    ativo = true,
                                    dataCriacao = "2024-10-21T00:00:00Z",
                                    dataAtualizacao = (string?)null
                                }
                            },
                            pageNumber = 1,
                            pageSize = 10,
                            totalCount = 2,
                            totalPages = 1
                        } as object : new object[]
                        {
                            new
                            {
                                id = 1,
                                nome = "Carlos Silva",
                                cargo = "Desenvolvedor Senior",
                                cpf = "123.456.789-00",
                                email = "carlos.silva@gdsolutions.com",
                                telefone = "(11) 99999-9999",
                                dataAdmissao = "2021-11-20T00:00:00Z",
                                departamentoId = 1,
                                departamentoNome = "Tecnologia",
                                salario = 8500.00,
                                endereco = "Rua das Flores, 123, São Paulo",
                                nivelSenioridade = 4,
                                ativo = true,
                                dataCriacao = "2024-10-21T00:00:00Z",
                                dataAtualizacao = (string?)null
                            },
                            new
                            {
                                id = 2,
                                nome = "Ana Costa",
                                cargo = "Desenvolvedora Pleno",
                                cpf = "987.654.321-00",
                                email = "ana.costa@gdsolutions.com",
                                telefone = "(11) 98888-8888",
                                dataAdmissao = "2023-06-15T00:00:00Z",
                                departamentoId = 1,
                                departamentoNome = "Tecnologia",
                                salario = 6500.00,
                                endereco = "Av. Paulista, 456, São Paulo",
                                nivelSenioridade = 3,
                                ativo = true,
                                dataCriacao = "2024-10-21T00:00:00Z",
                                dataAtualizacao = (string?)null
                            }
                        },
                        timestamp = DateTime.UtcNow.ToString("O")
                    };

                    AdicionarExemplo(operation, "200", exemploFuncionarios);
                }
            }

            // Exemplos para GET um Funcionário por ID
            if (methodName == "GetById" && controllerName?.Contains("Funcionario") == true)
            {
                if (operation.Responses.ContainsKey("200"))
                {
                    var exemploFuncionario = new
                    {
                        success = true,
                        message = "Funcionário retornado com sucesso",
                        data = new
                        {
                            id = 1,
                            nome = "Carlos Silva",
                            cargo = "Desenvolvedor Senior",
                            cpf = "123.456.789-00",
                            email = "carlos.silva@gdsolutions.com",
                            telefone = "(11) 99999-9999",
                            dataAdmissao = "2021-11-20T00:00:00Z",
                            departamentoId = 1,
                            departamentoNome = "Tecnologia",
                            salario = 8500.00,
                            endereco = "Rua das Flores, 123, São Paulo",
                            nivelSenioridade = 4,
                            ativo = true,
                            dataCriacao = "2024-10-21T00:00:00Z",
                            dataAtualizacao = (string?)null
                        },
                        timestamp = DateTime.UtcNow.ToString("O")
                    };

                    AdicionarExemplo(operation, "200", exemploFuncionario);
                }
            }
        }

        private void AdicionarExemplo(OpenApiOperation operation, string statusCode, object exemplo)
        {
            try
            {
                var json = JsonSerializer.Serialize(exemplo, _jsonOptions);
                var openApiObject = ParseJsonToOpenApiObject(json);
                
                var mediaType = operation.Responses[statusCode].Content.FirstOrDefault().Value;
                if (mediaType != null)
                {
                    mediaType.Example = openApiObject;
                }
            }
            catch
            {
                // Se houver erro na serialização, apenas ignora
            }
        }

        // ====================
        // METHOD: ParseJsonToOpenApiObject
        // Converts JSON string to OpenApiObject for use in Swagger examples
        // ====================
        private IOpenApiAny ParseJsonToOpenApiObject(string json)
        {
            using (var doc = JsonDocument.Parse(json))
            {
                return ConvertJsonElementToOpenApi(doc.RootElement);
            }
        }

        // ====================
        // METHOD: ConvertJsonElementToOpenApi
        // Recursively converts JsonElement to appropriate OpenAPI type
        // Handles objects, arrays, strings, numbers, booleans, and null values
        // ====================
        private IOpenApiAny ConvertJsonElementToOpenApi(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.Object => ConvertObject(element),
                JsonValueKind.Array => ConvertArray(element),
                JsonValueKind.String => new OpenApiString(element.GetString()),
                JsonValueKind.Number => element.TryGetInt32(out var intValue) 
                    ? new OpenApiInteger(intValue)
                    : new OpenApiDouble(element.GetDouble()),
                JsonValueKind.True => new OpenApiBoolean(true),
                JsonValueKind.False => new OpenApiBoolean(false),
                JsonValueKind.Null => new OpenApiNull(),
                _ => new OpenApiNull()
            };
        }

        // ====================
        // METHOD: ConvertObject
        // Converts a JSON object to OpenApiObject, processing all properties
        // ====================
        private OpenApiObject ConvertObject(JsonElement element)
        {
            var obj = new OpenApiObject();
            foreach (var property in element.EnumerateObject())
            {
                obj[property.Name] = ConvertJsonElementToOpenApi(property.Value);
            }
            return obj;
        }

        // ====================
        // METHOD: ConvertArray
        // Converts a JSON array to OpenApiArray, processing all items
        // ====================
        private OpenApiArray ConvertArray(JsonElement element)
        {
            var array = new OpenApiArray();
            foreach (var item in element.EnumerateArray())
            {
                array.Add(ConvertJsonElementToOpenApi(item));
            }
            return array;
        }
    }
}
