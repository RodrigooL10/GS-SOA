using System.ComponentModel.DataAnnotations;

namespace FuturoDoTrabalho.Api.Dtos
{
    // ====================================================================================
    // DTOs: AUTENTICACAO
    // ====================================================================================
    // DTOs (Data Transfer Objects) são objetos usados para transferir dados entre camadas.
    // Separam a estrutura de dados exposta pela API da estrutura interna do banco.
    // Isso permite evoluir o modelo interno sem quebrar contratos da API.
    // ====================================================================================

    // ====================
    // CLASS: LoginRequest
    // DTO for user login request
    // Represents the credentials required for user authentication
    // ====================
    public class LoginRequest
    {
        // Username - required field, max 50 characters
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        // Password - required field, no length restriction at DTO level (validated at service)
        [Required(ErrorMessage = "Senha é obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }

    // ====================
    // CLASS: RegistroRequest
    // DTO for user registration request
    // Represents the information required to create a new user account
    // ====================
    public class RegistroRequest
    {
        // Username - required field, max 50 characters
        [Required(ErrorMessage = "Nome de usuário é obrigatório")]
        [StringLength(50)]
        public string NomeUsuario { get; set; } = string.Empty;

        // Email address - required field, must be valid email format
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        // Password - required field, must be between 6 and 100 characters
        [Required(ErrorMessage = "Senha é obrigatória")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter entre 6 e 100 caracteres")]
        public string Senha { get; set; } = string.Empty;

        // Full name - required field, max 150 characters
        [Required(ErrorMessage = "Nome completo é obrigatório")]
        [StringLength(150)]
        public string NomeCompleto { get; set; } = string.Empty;
    }

    // ====================
    // CLASS: AutenticacaoResponse
    // DTO for authentication response
    // Represents the result of successful user authentication including JWT token
    // ====================
    public class AutenticacaoResponse
    {
        // User ID - unique identifier from database
        public int UsuarioId { get; set; }

        // Username - the account username
        public string NomeUsuario { get; set; } = string.Empty;

        // Email address - the registered email
        public string Email { get; set; } = string.Empty;

        // Full name - the user's full name
        public string NomeCompleto { get; set; } = string.Empty;

        // User role/profile - e.g., "Admin", "User", "Manager"
        public string Perfil { get; set; } = string.Empty;

        // JWT Bearer token - used for subsequent authenticated requests
        public string Token { get; set; } = string.Empty;

        // Token expiration date/time - when the token becomes invalid
        public DateTime ExpiracaoToken { get; set; }
    }

    // ====================
    // CLASS: UsuarioLeituraDto
    // DTO for user read/query response
    // Represents user data returned from GET endpoints
    // ====================
    public class UsuarioLeituraDto
    {
        // User ID - unique identifier from database
        public int Id { get; set; }

        // Username - the account username
        public string NomeUsuario { get; set; } = string.Empty;

        // Email address - the registered email
        public string Email { get; set; } = string.Empty;

        // Full name - the user's full name
        public string NomeCompleto { get; set; } = string.Empty;

        // User role/profile - e.g., "Admin", "User", "Manager"
        public string Perfil { get; set; } = string.Empty;

        // Active status - indicates if the user account is active
        public bool Ativo { get; set; }

        // Creation date - UTC timestamp of account creation
        public DateTime DataCriacao { get; set; }

        // Last login date - UTC timestamp of the most recent login
        public DateTime? DataUltimoLogin { get; set; }
    }
}
