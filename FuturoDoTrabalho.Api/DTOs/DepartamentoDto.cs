using System.ComponentModel.DataAnnotations;

namespace FuturoDoTrabalho.Api.DTOs
{
    // ====================================================================================
    // DTOs: DEPARTAMENTO
    // ====================================================================================
    // DTOs (Data Transfer Objects) são objetos usados para transferir dados entre camadas.
    // Separam a estrutura de dados exposta pela API da estrutura interna do banco.
    // Isso permite evoluir o modelo interno sem quebrar contratos da API.
    // ====================================================================================

    // DTO para criação de departamento
    // Usado no endpoint POST para receber dados do cliente
    public class DepartamentoCreateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [StringLength(150)]
        public string Lider { get; set; }

        public bool Ativo { get; set; } = true;
    }

    // DTO para atualização completa de departamento (PUT)
    // Usado no endpoint PUT - todos os campos devem ser fornecidos
    public class DepartamentoUpdateDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Nome { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [Required]
        [StringLength(150)]
        public string Lider { get; set; }

        public bool Ativo { get; set; }
    }

    // DTO para atualização parcial de departamento (PATCH) - apenas v2
    // Usado no endpoint PATCH - apenas campos fornecidos serão atualizados
    // Todos os campos são opcionais (nullable)
    public class DepartamentoPatchDto
    {
        [StringLength(100, MinimumLength = 3)]
        public string? Nome { get; set; }

        [StringLength(500)]
        public string? Descricao { get; set; }

        [StringLength(150)]
        public string? Lider { get; set; }

        public bool? Ativo { get; set; }
    }

    // DTO para leitura/consulta de departamento
    // Usado nas respostas GET - retorna dados do departamento
    public class DepartamentoReadDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public string Lider { get; set; }
        public bool Ativo { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}
