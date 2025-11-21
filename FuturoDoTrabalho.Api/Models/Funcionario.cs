using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuturoDoTrabalho.Api.Models
{
    // ====================================================================================
    // MODEL: FUNCIONARIO
    // ====================================================================================
    // Representa a entidade Funcionário no banco de dados.
    // Esta classe mapeia para a tabela "funcionarios" no MySQL.
    // Contém todas as propriedades e validações relacionadas a um funcionário.
    // ====================================================================================
    [Table("funcionarios")]
    public class Funcionario
    {
        // Identificador único do funcionário (chave primária, auto-incremento)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Nome completo do funcionário (obrigatório, máximo 150 caracteres)
        [Required]
        [StringLength(150)]
        public string Nome { get; set; }

        // Cargo/função do funcionário na empresa (obrigatório, máximo 100 caracteres)
        [Required]
        [StringLength(100)]
        public string Cargo { get; set; }

        // CPF do funcionário (opcional, máximo 11 caracteres, único no sistema)
        [StringLength(11)]
        public string? CPF { get; set; }

        // Email do funcionário (opcional, validado como formato de email, máximo 150 caracteres)
        [EmailAddress]
        [StringLength(150)]
        public string? Email { get; set; }

        // Telefone de contato (opcional, máximo 20 caracteres)
        [StringLength(20)]
        public string? Telefone { get; set; }

        // Data de admissão do funcionário na empresa (obrigatório)
        [Required]
        public DateTime DataAdmissao { get; set; }

        // ID do departamento ao qual o funcionário pertence (chave estrangeira, obrigatório)
        [Required]
        public int DepartamentoId { get; set; }

        // Navegação para o objeto Departamento (relacionamento muitos-para-um)
        [ForeignKey("DepartamentoId")]
        public Departamento? Departamento { get; set; }

        // Salário do funcionário (decimal com precisão 10,2, valor mínimo 0.01)
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.01, 9999999999.99)]
        public decimal Salario { get; set; }

        // Endereço residencial (opcional, máximo 500 caracteres)
        [StringLength(500)]
        public string? Endereco { get; set; }

        // Nível de senioridade do funcionário (1-5)
        // 1: Júnior, 2: Pleno, 3: Sênior, 4: Especialista, 5: Arquiteto
        [Range(1, 5)]
        public int NivelSenioridade { get; set; } = 1;

        // Indica se o funcionário está ativo no sistema (padrão: true)
        public bool Ativo { get; set; } = true;

        // Data de criação do registro (preenchida automaticamente)
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Data da última atualização do registro (null se nunca foi atualizado)
        public DateTime? DataAtualizacao { get; set; }
    }
}
