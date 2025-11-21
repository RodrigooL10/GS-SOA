using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Repositories
{
    // ====================================================================================
    // INTERFACE: I FUNCIONARIO REPOSITORY
    // ====================================================================================
    // Interface específica para operações de acesso a dados relacionadas a funcionários.
    // Herda operações CRUD básicas de IGenericRepository e adiciona métodos customizados
    // para consultas específicas de funcionários (por CPF, departamento, nível, etc.).
    // ====================================================================================
    public interface IFuncionarioRepository : IGenericRepository<Funcionario>
    {
        /// <summary>
        /// Busca um funcionário pelo CPF
        /// </summary>
        /// <param name="cpf">CPF do funcionário (sem formatação)</param>
        /// <returns>Funcionário encontrado ou null se não existir</returns>
        Task<Funcionario?> GetByCpfAsync(string cpf);

        /// <summary>
        /// Busca todos os funcionários de um departamento específico
        /// </summary>
        /// <param name="departamentoId">ID do departamento</param>
        /// <returns>Lista de funcionários do departamento</returns>
        Task<List<Funcionario>> GetByDepartamentoAsync(int departamentoId);

        /// <summary>
        /// Busca funcionários por nível de senioridade
        /// </summary>
        /// <param name="nivel">Nível de senioridade (1=Júnior, 2=Pleno, 3=Sênior, 4=Especialista, 5=Arquiteto)</param>
        /// <returns>Lista de funcionários com o nível especificado</returns>
        Task<List<Funcionario>> GetByNivelSenioridadeAsync(int nivel);

        /// <summary>
        /// Busca apenas funcionários ativos no sistema
        /// </summary>
        /// <returns>Lista de funcionários com status ativo</returns>
        Task<List<Funcionario>> GetAtivosAsync();
    }
}
