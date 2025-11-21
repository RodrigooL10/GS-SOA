using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Services;
using FuturoDoTrabalho.Api.Dtos;

namespace FuturoDoTrabalho.Api.Controllers.v1
{
    // ====================================================================================
    // CONTROLLER: DEPARTAMENTO CONTROLLER V1
    // ====================================================================================
    // Controller da versão 1 da API para gerenciamento de departamentos.
    // Versão básica: oferece operações CRUD completas (GET, POST, PUT, DELETE).
    // Não possui paginação nem suporte a PATCH (atualização parcial).
    // Requer autenticação e autorização baseada em roles.
    // ====================================================================================
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class DepartamentoController : ControllerBase
    {
        private readonly IDepartamentoService _service;
        private readonly ILogger<DepartamentoController> _logger;

        public DepartamentoController(IDepartamentoService service, ILogger<DepartamentoController> logger)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Obtém todos os departamentos
        /// </summary>
        /// <returns>Lista de departamentos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<DepartamentoReadDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<IEnumerable<DepartamentoReadDto>>>> GetAll()
        {
            try
            {
                _logger.LogInformation("Listando todos os departamentos (v1)");
                var departamentos = await _service.GetAllAsync();
                return Ok(ApiResponse<IEnumerable<DepartamentoReadDto>>.SuccessResponse(departamentos, "Departamentos listados com sucesso"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter departamentos");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao obter departamentos"));
            }
        }

        /// <summary>
        /// Obtém um departamento por ID
        /// </summary>
        /// <param name="id">ID do departamento</param>
        /// <returns>Dados do departamento</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<DepartamentoReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<DepartamentoReadDto>>> GetById(int id)
        {
            try
            {
                _logger.LogInformation("Buscando departamento ID {DepartamentoId} (v1)", id);
                
                if (id <= 0)
                    return BadRequest(ApiResponse.ErrorResponse("ID deve ser maior que zero"));

                var departamento = await _service.GetByIdAsync(id);
                if (departamento == null)
                    return NotFound(ApiResponse.ErrorResponse("Departamento não encontrado"));

                return Ok(ApiResponse<DepartamentoReadDto>.SuccessResponse(departamento));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter departamento ID {DepartamentoId}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao obter departamento"));
            }
        }

        /// <summary>
        /// Cria um novo departamento (requer perfil Admin)
        /// </summary>
        /// <param name="dto">Dados do departamento</param>
        /// <returns>Departamento criado</returns>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<DepartamentoReadDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<DepartamentoReadDto>>> Create([FromBody] DepartamentoCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Criando novo departamento: {Nome}", dto.Nome);
                
                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResponse("Dados inválidos"));

                var departamento = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = departamento.Id }, 
                    ApiResponse<DepartamentoReadDto>.SuccessResponse(departamento, "Departamento criado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao criar departamento");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar departamento");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao criar departamento"));
            }
        }

        /// <summary>
        /// Atualiza um departamento existente (requer perfil Admin ou Gerente)
        /// </summary>
        /// <param name="id">ID do departamento</param>
        /// <param name="dto">Dados a serem atualizados</param>
        /// <returns>Departamento atualizado</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<DepartamentoReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<DepartamentoReadDto>>> Update(int id, [FromBody] DepartamentoUpdateDto dto)
        {
            try
            {
                _logger.LogInformation("Atualizando departamento ID {DepartamentoId} (v1)", id);
                
                if (id <= 0)
                    return BadRequest(ApiResponse.ErrorResponse("ID deve ser maior que zero"));

                if (!ModelState.IsValid)
                    return BadRequest(ApiResponse.ErrorResponse("Dados inválidos"));

                var departamento = await _service.UpdateAsync(id, dto);
                if (departamento == null)
                    return NotFound(ApiResponse.ErrorResponse("Departamento não encontrado"));

                return Ok(ApiResponse<DepartamentoReadDto>.SuccessResponse(departamento, "Departamento atualizado com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao atualizar departamento");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar departamento ID {DepartamentoId}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao atualizar departamento"));
            }
        }

        /// <summary>
        /// Deleta um departamento (requer perfil Admin)
        /// </summary>
        /// <param name="id">ID do departamento</param>
        /// <returns>Confirmação de deleção</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("Deletando departamento ID {DepartamentoId} (v1)", id);
                
                if (id <= 0)
                    return BadRequest(ApiResponse.ErrorResponse("ID deve ser maior que zero"));

                var resultado = await _service.DeleteAsync(id);
                if (!resultado)
                    return NotFound(ApiResponse.ErrorResponse("Departamento não encontrado"));

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao deletar departamento");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar departamento ID {DepartamentoId}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao deletar departamento"));
            }
        }
    }
}
