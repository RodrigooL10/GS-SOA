using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Services;
using FuturoDoTrabalho.Api.Dtos;

namespace FuturoDoTrabalho.Api.Controllers.v2
{
    // ====================================================================================
    // CONTROLLER: DEPARTAMENTO CONTROLLER V2
    // ====================================================================================
    // Controller da versão 2 da API para gerenciamento de departamentos.
    // Versão avançada: inclui todas as funcionalidades da v1 mais:
    // - Paginação nas listagens (GET com pageNumber e pageSize)
    // - Atualização parcial via PATCH
    // Requer autenticação e autorização baseada em roles.
    // ====================================================================================
    [ApiController]
    [ApiVersion("2.0")]
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
        /// Obtém departamentos com paginação
        /// </summary>
        /// <param name="pageNumber">Número da página (padrão: 1)</param>
        /// <param name="pageSize">Itens por página (padrão: 10, máximo: 100)</param>
        /// <returns>Lista paginada de departamentos</returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPaged(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                _logger.LogInformation("Listando departamentos com paginação - Página {PageNumber}, Tamanho {PageSize}", pageNumber, pageSize);
                
                if (pageSize > 100)
                    pageSize = 100;

                var (data, totalCount, pageCount) = await _service.GetPagedAsync(pageNumber, pageSize);

                var response = new
                {
                    data = data,
                    pageNumber = pageNumber,
                    pageSize = pageSize,
                    totalCount = totalCount,
                    totalPages = pageCount
                };

                return Ok(ApiResponse<object>.SuccessResponse(response, "Departamentos listados com paginação"));
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Erro de validação ao listar departamentos");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar departamentos com paginação");
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao listar departamentos"));
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
                _logger.LogInformation("Buscando departamento ID {DepartamentoId} (v2)", id);
                
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
        /// Cria um novo departamento (requer Admin)
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
        /// Atualiza um departamento existente (requer Admin ou Gerente)
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
                _logger.LogInformation("Atualizando departamento ID {DepartamentoId} (v2)", id);
                
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
        /// Atualiza parcialmente um departamento (PATCH, requer Admin ou Gerente)
        /// </summary>
        /// <param name="id">ID do departamento</param>
        /// <param name="dto">Campos a atualizar</param>
        /// <returns>Departamento atualizado</returns>
        [HttpPatch("{id}")]
        [Authorize(Roles = "Admin,Gerente")]
        [ProducesResponseType(typeof(ApiResponse<DepartamentoReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ApiResponse<DepartamentoReadDto>>> Patch(int id, [FromBody] DepartamentoPatchDto dto)
        {
            try
            {
                _logger.LogInformation("Atualizando parcialmente departamento ID {DepartamentoId} (PATCH v2)", id);
                
                if (id <= 0)
                    return BadRequest(ApiResponse.ErrorResponse("ID deve ser maior que zero"));

                var departamento = await _service.PatchAsync(id, dto);
                if (departamento == null)
                    return NotFound(ApiResponse.ErrorResponse("Departamento não encontrado"));

                return Ok(ApiResponse<DepartamentoReadDto>.SuccessResponse(departamento, "Departamento atualizado parcialmente com sucesso"));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro ao fazer patch em departamento");
                return BadRequest(ApiResponse.ErrorResponse(ex.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer patch em departamento ID {DepartamentoId}", id);
                return StatusCode(500, ApiResponse.ErrorResponse("Erro ao atualizar departamento"));
            }
        }

        /// <summary>
        /// Deleta um departamento (requer Admin)
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
                _logger.LogInformation("Deletando departamento ID {DepartamentoId} (v2)", id);
                
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
