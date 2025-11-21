using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FuturoDoTrabalho.Api.Filters
{
    // ====================================================================================
    // FILTRO: SET VERSION PARAMETER
    // ====================================================================================
    // Filtro customizado para o Swagger que pré-preenche e bloqueia o parâmetro "version"
    // de acordo com a versão da API selecionada no Swagger UI.
    // Isso melhora a experiência do desenvolvedor ao testar a API, evitando erros de versão.
    // ====================================================================================
    /// <summary>
    /// Filtro que pré-preenche e bloqueia o parâmetro "version" de acordo com a versão selecionada
    /// </summary>
    public class SetVersionParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Se não houver parâmetros na operação, não há nada a fazer
            if (operation.Parameters == null || operation.Parameters.Count == 0)
                return;

            // Encontrar o parâmetro "version" na lista de parâmetros da operação
            var versionParameter = operation.Parameters
                .FirstOrDefault(p => p.Name.Equals("version", StringComparison.OrdinalIgnoreCase));

            if (versionParameter != null)
            {
                // ====================================================================================
                // DETECÇÃO DA VERSÃO DA API
                // ====================================================================================
                // Tenta identificar a versão da API através do namespace do controller ou do caminho.
                // ====================================================================================

                // Versão padrão (v1)
                string apiVersion = "1";
                
                // Tentar obter a versão pelo caminho relativo (ex: /api/v{version}/Departamento)
                var relativePath = context.ApiDescription.RelativePath ?? string.Empty;
                
                // Verificar se o controller está no namespace v2
                var actionDescriptor = context.ApiDescription.ActionDescriptor;
                if (actionDescriptor?.DisplayName?.Contains(".v2.") == true)
                {
                    apiVersion = "2";
                }
                // Fallback: verificar se o caminho contém "v2"
                else if (relativePath.Contains("/v2/") || relativePath.Contains("v2"))
                {
                    apiVersion = "2";
                }
                
                // ====================================================================================
                // CONFIGURAÇÃO DO PARÂMETRO NO SWAGGER
                // ====================================================================================
                // Pré-preenche o valor da versão e o marca como somente leitura,
                // evitando que o usuário altere manualmente no Swagger UI.
                // ====================================================================================

                // Pré-preencher com o valor correto da versão
                versionParameter.Example = new Microsoft.OpenApi.Any.OpenApiString(apiVersion);
                versionParameter.Schema.Default = new Microsoft.OpenApi.Any.OpenApiString(apiVersion);
                
                // Marcar como ReadOnly (bloqueado para edição no Swagger UI)
                versionParameter.Schema.ReadOnly = true;
                
                // Adicionar descrição útil para o desenvolvedor
                versionParameter.Description = $"Versão da API (pré-preenchida como {apiVersion})";
            }
        }
    }
}
