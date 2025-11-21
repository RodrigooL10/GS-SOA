using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuturoDoTrabalho.Api.Models
{
    // ====================================================================================
    // MODEL: DEPARTAMENTO
    // ====================================================================================
    // Representa a entidade Departamento no banco de dados.
    // Esta classe mapeia para a tabela "departamentos" no MySQL.
    // Um departamento pode ter múltiplos funcionários (relacionamento um-para-muitos).
    // ====================================================================================
    [Table("departamentos")]
    public class Departamento
    {
        // Identificador único do departamento (chave primária, auto-incremento)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Nome do departamento (obrigatório, máximo 100 caracteres, único no sistema)
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        // Descrição detalhada do departamento (opcional, máximo 500 caracteres)
        [StringLength(500)]
        public string? Descricao { get; set; }

        // Nome do líder/gerente do departamento (obrigatório, máximo 150 caracteres)
        [Required]
        [StringLength(150)]
        public string Lider { get; set; }

        // Indica se o departamento está ativo no sistema (padrão: true)
        public bool Ativo { get; set; } = true;

        // Data de criação do registro (preenchida automaticamente)
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        // Data da última atualização do registro (null se nunca foi atualizado)
        public DateTime? DataAtualizacao { get; set; }

        // Relacionamento com Funcionários (um departamento tem muitos funcionários)
        // Esta é a propriedade de navegação para acessar os funcionários do departamento
        public ICollection<Funcionario>? Funcionarios { get; set; }
    }
}
