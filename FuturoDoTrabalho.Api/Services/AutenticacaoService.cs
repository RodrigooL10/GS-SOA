using FuturoDoTrabalho.Api.Data;
using FuturoDoTrabalho.Api.Dtos;
using FuturoDoTrabalho.Api.Enums;
using FuturoDoTrabalho.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace FuturoDoTrabalho.Api.Services
{
    /// <summary>
    /// Interface do serviço de autenticação
    /// </summary>
    public interface IAutenticacaoService
    {
        Task<(bool Success, string Message, AutenticacaoResponse? Data)> RegistrarAsync(RegistroRequest request);
        Task<(bool Success, string Message, AutenticacaoResponse? Data)> LoginAsync(LoginRequest request);
        Task<(bool Success, string Message)> VerificarTokenAsync(string token);
    }

    /// <summary>
    /// Serviço de autenticação com JWT
    /// </summary>
    public class AutenticacaoService : IAutenticacaoService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AutenticacaoService> _logger;

        public AutenticacaoService(AppDbContext context, IConfiguration configuration, ILogger<AutenticacaoService> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Registra um novo usuário
        /// </summary>
        public async Task<(bool Success, string Message, AutenticacaoResponse? Data)> RegistrarAsync(RegistroRequest request)
        {
            try
            {
                // Validar se usuário já existe
                var usuarioExistente = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NomeUsuario == request.NomeUsuario || u.Email == request.Email);

                if (usuarioExistente != null)
                {
                    return (false, "Usuário ou email já cadastrado", null);
                }

                // Parse do perfil fornecido no request, caso inválido usa padrão
                var perfil = UserRole.Funcionario;
                if (Enum.TryParse<UserRole>(request.Perfil, ignoreCase: true, out var perfilParsed))
                {
                    perfil = perfilParsed;
                }

                // Criar novo usuário
                var usuario = new Usuario
                {
                    NomeUsuario = request.NomeUsuario,
                    Email = request.Email,
                    NomeCompleto = request.NomeCompleto,
                    SenhaHash = HasharSenha(request.Senha),
                    Perfil = perfil,
                    Ativo = true,
                    DataCriacao = DateTime.UtcNow
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário {NomeUsuario} registrado com sucesso com perfil {Perfil}", request.NomeUsuario, perfil);

                // Gerar token
                var token = GerarToken(usuario);
                var response = MapearParaResponse(usuario, token);

                return (true, "Usuário registrado com sucesso", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar usuário");
                return (false, "Erro ao registrar usuário", null);
            }
        }

        /// <summary>
        /// Realiza login do usuário
        /// </summary>
        public async Task<(bool Success, string Message, AutenticacaoResponse? Data)> LoginAsync(LoginRequest request)
        {
            try
            {
                // Buscar usuário
                var usuario = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.NomeUsuario == request.NomeUsuario);

                if (usuario == null || !VerificarSenha(request.Senha, usuario.SenhaHash))
                {
                    _logger.LogWarning("Tentativa de login falhada para usuário {NomeUsuario}", request.NomeUsuario);
                    return (false, "Nome de usuário ou senha inválidos", null);
                }

                if (!usuario.Ativo)
                {
                    return (false, "Usuário inativo", null);
                }

                // Atualizar data do último login
                usuario.DataUltimoLogin = DateTime.UtcNow;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Usuário {NomeUsuario} logado com sucesso", request.NomeUsuario);

                // Gerar token
                var token = GerarToken(usuario);
                var response = MapearParaResponse(usuario, token);

                return (true, "Login realizado com sucesso", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao realizar login");
                return (false, "Erro ao realizar login", null);
            }
        }

        /// <summary>
        /// Verifica se um token é válido
        /// </summary>
        public async Task<(bool Success, string Message)> VerificarTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "");

                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (true, "Token válido");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Token inválido: {Message}", ex.Message);
                return (false, "Token inválido ou expirado");
            }
        }

        /// <summary>
        /// Gera um token JWT para o usuário
        /// </summary>
        private string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"] ?? "");
            var expiracaoMinutos = int.Parse(_configuration["Jwt:ExpiracaoMinutos"] ?? "60");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new System.Security.Claims.ClaimsIdentity(new[]
                {
                    new System.Security.Claims.Claim("id", usuario.Id.ToString()),
                    new System.Security.Claims.Claim("nomeUsuario", usuario.NomeUsuario),
                    new System.Security.Claims.Claim("email", usuario.Email),
                    new System.Security.Claims.Claim("perfil", usuario.Perfil.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Role, usuario.Perfil.ToString()),
                    new System.Security.Claims.Claim("nomeCompleto", usuario.NomeCompleto)
                }),
                Expires = DateTime.UtcNow.AddMinutes(expiracaoMinutos),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Cria hash de uma senha usando PBKDF2
        /// </summary>
        private string HasharSenha(string senha)
        {
            using (var rng = new Rfc2898DeriveBytes(senha, 16, 10000, HashAlgorithmName.SHA256))
            {
                var salt = rng.Salt;
                var hash = rng.GetBytes(20);

                var hashWithSalt = new byte[36];
                Array.Copy(salt, 0, hashWithSalt, 0, 16);
                Array.Copy(hash, 0, hashWithSalt, 16, 20);

                return Convert.ToBase64String(hashWithSalt);
            }
        }

        /// <summary>
        /// Verifica se a senha fornecida corresponde ao hash armazenado
        /// </summary>
        private bool VerificarSenha(string senhaFornecida, string senhaHash)
        {
            var hashBytes = Convert.FromBase64String(senhaHash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            using (var rng = new Rfc2898DeriveBytes(senhaFornecida, salt, 10000, HashAlgorithmName.SHA256))
            {
                var hash = rng.GetBytes(20);
                for (int i = 0; i < 20; i++)
                {
                    if (hashBytes[i + 16] != hash[i])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Mapeia a entidade Usuário para o DTO de resposta
        /// </summary>
        private AutenticacaoResponse MapearParaResponse(Usuario usuario, string token)
        {
            var expiracaoMinutos = int.Parse(_configuration["Jwt:ExpiracaoMinutos"] ?? "60");

            return new AutenticacaoResponse
            {
                UsuarioId = usuario.Id,
                NomeUsuario = usuario.NomeUsuario,
                Email = usuario.Email,
                NomeCompleto = usuario.NomeCompleto,
                Perfil = usuario.Perfil.ToString(),
                Token = token,
                ExpiracaoToken = DateTime.UtcNow.AddMinutes(expiracaoMinutos)
            };
        }
    }
}
