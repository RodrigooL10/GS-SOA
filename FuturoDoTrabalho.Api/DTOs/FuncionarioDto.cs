using System.ComponentModel.DataAnnotations;

namespace FuturoDoTrabalho.Api.DTOs
{
    // ====================================================================================
    // DTOs: FUNCIONARIO
    // ====================================================================================
    // DTOs (Data Transfer Objects) são objetos usados para transferir dados entre camadas.
    // Separam a estrutura de dados exposta pela API da estrutura interna do banco.
    // Isso permite evoluir o modelo interno sem quebrar contratos da API.
    // ====================================================================================

    // DTO para criação de funcionário
    // Usado no endpoint POST para receber dados do cliente
    public class FuncionarioCreateDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        public string Cargo { get; set; }

        [StringLength(11)]
        public string? CPF { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [Required]
        public DateTime DataAdmissao { get; set; }

        [Required]
        public int DepartamentoId { get; set; }

        [Range(0.01, 9999999999.99)]
        [Required]
        public decimal Salario { get; set; }

        [StringLength(500)]
        public string? Endereco { get; set; }

        [Range(1, 5)]
        public int NivelSenioridade { get; set; } = 1;

        public bool Ativo { get; set; } = true;
    }

    // DTO para atualização completa de funcionário (PUT)
    // Usado no endpoint PUT - todos os campos devem ser fornecidos
    public class FuncionarioUpdateDto
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string Nome { get; set; }

        [Required]
        [StringLength(100)]
        public string Cargo { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [Required]
        public int DepartamentoId { get; set; }

        [Range(0.01, 9999999999.99)]
        [Required]
        public decimal Salario { get; set; }

        [StringLength(500)]
        public string? Endereco { get; set; }

        [Range(1, 5)]
        public int NivelSenioridade { get; set; }

        public bool Ativo { get; set; }
    }

    // DTO para atualização parcial de funcionário (PATCH) - apenas v2
    // Usado no endpoint PATCH - apenas campos fornecidos serão atualizados
    // Todos os campos são opcionais (nullable)
    public class FuncionarioPatchDto
    {
        [StringLength(150, MinimumLength = 3)]
        public string? Nome { get; set; }

        [StringLength(100)]
        public string? Cargo { get; set; }

        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        [StringLength(20)]
        public string? Telefone { get; set; }

        [Range(0.01, 9999999999.99)]
        public decimal? Salario { get; set; }

        [StringLength(500)]
        public string? Endereco { get; set; }

        [Range(1, 5)]
        public int? NivelSenioridade { get; set; }

        public bool? Ativo { get; set; }
    }

    // DTO para leitura/consulta de funcionário
    // Usado nas respostas GET - inclui dados calculados como nome do departamento
    public class FuncionarioReadDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cargo { get; set; }
        public string? CPF { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }
        public DateTime DataAdmissao { get; set; }
        public int DepartamentoId { get; set; }
        public string? DepartamentoNome { get; set; }
        public decimal Salario { get; set; }
        public string? Endereco { get; set; }
        public int NivelSenioridade { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
