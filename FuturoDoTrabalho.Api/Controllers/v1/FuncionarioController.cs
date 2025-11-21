using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Services;
using FuturoDoTrabalho.Api.Dtos;

namespace FuturoDoTrabalho.Api.Controllers.v1
{
    // ====================================================================================
    // CONTROLLER: FUNCIONARIO CONTROLLER V1
    // ====================================================================================
    // Controller da versão 1 da API para gerenciamento de funcionários.
    // Versão básica: oferece operações CRUD completas (GET, POST, PUT, DELETE).
    // Não possui paginação nem suporte a PATCH (atualização parcial).
    // Requer autenticação e autorização baseada em roles.
    // ====================================================================================
    [ApiController]
    [Route("api/v{version:apiVersion}/funcionario")]
    [ApiVersion("1.0")]
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
        /// Listar todos os funcionários
        /// </summary>
        /// <returns>Array de funcionários</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<FuncionarioReadDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<List<FuncionarioReadDto>>>> GetAll()
        {
            _logger.LogInformation("Listando todos os funcionários (v1)");
            var funcionarios = await _funcionarioService.GetAllAsync();
            return Ok(ApiResponse<List<FuncionarioReadDto>>.SuccessResponse(funcionarios, "Funcionários listados com sucesso"));
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
            _logger.LogInformation("Buscando funcionário ID {Id}", id);
            var funcionario = await _funcionarioService.GetByIdAsync(id);
            if (funcionario == null)
                return NotFound(ApiResponse<FuncionarioReadDto>.ErrorResponse("Funcionário não encontrado"));

            return Ok(ApiResponse<FuncionarioReadDto>.SuccessResponse(funcionario));
        }

        /// <summary>
        /// Criar novo funcionário (requer perfil Admin ou Gerente)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<FuncionarioReadDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<FuncionarioReadDto>>> Create([FromBody] FuncionarioCreateDto dto)
        {
            _logger.LogInformation("Criando novo funcionário: {Nome}", dto.Nome);

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
        /// Atualizar funcionário (completo, requer perfil Admin ou Gerente)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<FuncionarioReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<FuncionarioReadDto>>> Update(int id, [FromBody] FuncionarioUpdateDto dto)
        {
            _logger.LogInformation("Atualizando funcionário ID {Id}", id);

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
        /// Deletar funcionário (requer perfil Admin)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Deletando funcionário ID {Id}", id);
            var resultado = await _funcionarioService.DeleteAsync(id);
            if (!resultado)
                return NotFound(ApiResponse.ErrorResponse("Funcionário não encontrado"));

            return NoContent();
        }
    }
}
