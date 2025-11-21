using FuturoDoTrabalho.Api.Dtos;
using FuturoDoTrabalho.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FuturoDoTrabalho.Api.Controllers
{
    // ====================================================================================
    // CONTROLLER: AUTENTICAÇÃO
    // ====================================================================================
    // Responsável por processar requisições relacionadas a autenticação e autorização:
    // - Registrar novos usuários (POST /api/autenticacao/registrar)
    // - Realizar login de usuários existentes (POST /api/autenticacao/login)
    // - Validar tokens JWT (GET /api/autenticacao/verificar-token)
    // Endpoints registrar e login não requerem autenticação
    // Endpoint verificar-token requer token JWT válido
    // ====================================================================================
    [ApiController]
    [Route("api/[controller]")]
    public class AutenticacaoController : ControllerBase
    {
        // ==================== DEPENDÊNCIAS ====================
        private readonly IAutenticacaoService _autenticacaoService;
        private readonly ILogger<AutenticacaoController> _logger;

        public AutenticacaoController(IAutenticacaoService autenticacaoService, ILogger<AutenticacaoController> logger)
        {
            _autenticacaoService = autenticacaoService;
            _logger = logger;
        }

        // ==================== ENDPOINT: REGISTRAR ====================
        // POST /api/autenticacao/registrar
        // Cria uma nova conta de usuário no sistema
        // Retorna dados do usuário e token JWT para acesso imediato
        // ============================================================
        [HttpPost("registrar")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Registrar([FromBody] RegistroRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.ErrorResponse($"Validação falhou: {errors}"));
            }

            var (success, message, data) = await _autenticacaoService.RegistrarAsync(request);

            if (success)
            {
                _logger.LogInformation("Novo usuário registrado: {NomeUsuario}", request.NomeUsuario);
                return Ok(ApiResponse<AutenticacaoResponse>.SuccessResponse(data, message));
            }

            return BadRequest(ApiResponse.ErrorResponse(message));
        }

        // ==================== ENDPOINT: LOGIN ====================
        // POST /api/autenticacao/login
        // Autentica um usuário existente com suas credenciais
        // Retorna dados do usuário e token JWT para acesso aos endpoints protegidos
        // ==========================================================
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ApiResponse<AutenticacaoResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("; ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse.ErrorResponse($"Validação falhou: {errors}"));
            }

            var (success, message, data) = await _autenticacaoService.LoginAsync(request);

            if (success)
            {
                _logger.LogInformation("Usuário logado: {NomeUsuario}", request.NomeUsuario);
                return Ok(ApiResponse<AutenticacaoResponse>.SuccessResponse(data, message));
            }

            return BadRequest(ApiResponse.ErrorResponse(message));
        }

        // ==================== ENDPOINT: VERIFICAR TOKEN ====================
        // GET /api/autenticacao/verificar-token
        // Valida se o token JWT no header Authorization é válido
        // Requer autenticação (token JWT no header Authorization: Bearer <token>)
        // =====================================================================
        [HttpGet("verificar-token")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> VerificarToken()
        {
            var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

            var (success, message) = await _autenticacaoService.VerificarTokenAsync(token);

            if (success)
            {
                return Ok(ApiResponse.SuccessResponse(message));
            }

            return Unauthorized(ApiResponse.ErrorResponse(message));
        }
    }
}
