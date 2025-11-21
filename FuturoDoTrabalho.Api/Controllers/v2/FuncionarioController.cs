using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Services;
using FuturoDoTrabalho.Api.Dtos;

namespace FuturoDoTrabalho.Api.Controllers.v2
{
    // ====================================================================================
    // CONTROLLER: FUNCIONARIO CONTROLLER V2
    // ====================================================================================
    // Controller da versão 2 da API para gerenciamento de funcionários.
    // Versão avançada: inclui todas as funcionalidades da v1 mais:
    // - Paginação nas listagens (GET com pageNumber e pageSize)
    // - Atualização parcial via PATCH
    // - Filtros avançados (por status ativo/inativo)
    // Requer autenticação e autorização baseada em roles.
    // ====================================================================================
    [ApiController]
    [Route("api/v{version:apiVersion}/funcionario")]
    [ApiVersion("2.0")]
    [Authorize]
    public class FuncionarioController : ControllerBase
    {
        private readonly IFuncionarioService _funcionarioService;
        private readonly ILogger<FuncionarioController> _logger;

        public FuncionarioController(
            IFuncionarioService funcionarioService,
            ILogger<FuncionarioController> logger)
        {
            _funcionarioService = funcionarioService;
            _logger = logger;
        }

        /// <summary>
        /// Listar funcionários com paginação (v2)
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<object>>> GetAll(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool? ativo = null)
        {
            _logger.LogInformation("Listando funcionários com paginação - Página {PageNumber}, Tamanho {PageSize}", pageNumber, pageSize);

            // Validar e corrigir parâmetros de paginação
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10; // Limitar máximo de 100 itens por página

            // Buscar dados paginados do service
            var (data, totalCount, pageCount) = await _funcionarioService.GetPagedAsync(pageNumber, pageSize);

            // Aplicar filtro por status ativo/inativo se especificado
            if (ativo.HasValue)
            {
                data = data.Where(f => f.Ativo == ativo.Value).ToList();
            }

            // Retornar resposta paginada com metadados
            var response = new
            {
                data,
                pageNumber,
                pageSize,
                totalCount,
                totalPages = pageCount
            };

            return Ok(ApiResponse<object>.SuccessResponse(response, "Funcionários listados com paginação"));
        }

        /// <summary>
        /// Obter funcionário por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<FuncionarioReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<FuncionarioReadDto>>> GetById(int id)
        {
            _logger.LogInformation("Buscando funcionário ID {Id} (v2)", id);
            var funcionario = await _funcionarioService.GetByIdAsync(id);
            if (funcionario == null)
                return NotFound(ApiResponse.ErrorResponse("Funcionário não encontrado"));

            return Ok(ApiResponse<FuncionarioReadDto>.SuccessResponse(funcionario));
        }

        /// <summary>
        /// Criar novo funcionário (v2, requer Admin ou Gerente)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<FuncionarioReadDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<FuncionarioReadDto>>> Create([FromBody] FuncionarioCreateDto dto)
        {
            _logger.LogInformation("Criando novo funcionário: {Nome} (v2)", dto.Nome);

            try
            {
                var funcionario = await _funcionarioService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = funcionario.Id }, 
                    ApiResponse<FuncionarioReadDto>.SuccessResponse(funcionario, "Funcionário criado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Erro ao criar funcionário: {Mensagem}", ex.Message);
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Atualizar funcionário completo (v2, requer Admin ou Gerente)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<FuncionarioReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<FuncionarioReadDto>>> Update(int id, [FromBody] FuncionarioUpdateDto dto)
        {
            _logger.LogInformation("Atualizando funcionário ID {Id} (v2)", id);

            try
            {
                var funcionario = await _funcionarioService.UpdateAsync(id, dto);
                if (funcionario == null)
                    return NotFound(ApiResponse.ErrorResponse("Funcionário não encontrado"));

                return Ok(ApiResponse<FuncionarioReadDto>.SuccessResponse(funcionario, "Funcionário atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Erro ao atualizar funcionário: {Mensagem}", ex.Message);
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Atualizar parcialmente funcionário (PATCH) - APENAS v2, requer Admin ou Gerente
        /// </summary>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<FuncionarioReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<FuncionarioReadDto>>> Patch(int id, [FromBody] FuncionarioPatchDto dto)
        {
            _logger.LogInformation("Atualizando parcialmente funcionário ID {Id} (PATCH v2)", id);

            try
            {
                var funcionario = await _funcionarioService.PatchAsync(id, dto);
                if (funcionario == null)
                    return NotFound(ApiResponse.ErrorResponse("Funcionário não encontrado"));

                return Ok(ApiResponse<FuncionarioReadDto>.SuccessResponse(funcionario, "Funcionário atualizado parcialmente com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Erro ao fazer PATCH: {Mensagem}", ex.Message);
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Deletar funcionário (v2, requer Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deletando funcionário ID {Id} (v2)", id);
            var resultado = await _funcionarioService.DeleteAsync(id);
            if (!resultado)
                return NotFound(ApiResponse.ErrorResponse("Funcionário não encontrado"));

            return NoContent();
        }
    }
}
