using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Repositories
{
    // ====================================================================================
    // INTERFACE: I DEPARTAMENTO REPOSITORY
    // ====================================================================================
    // Interface específica para operações de acesso a dados relacionadas a departamentos.
    // Herda operações CRUD básicas de IGenericRepository e adiciona métodos customizados
    // para consultas específicas de departamentos (por nome, ativos, etc.).
    // ====================================================================================
    public interface IDepartamentoRepository : IGenericRepository<Departamento>
    {
        /// <summary>
        /// Busca um departamento pelo nome (nome é único no sistema)
        /// </summary>
        /// <param name="nome">Nome do departamento</param>
        /// <returns>Departamento encontrado ou null se não existir</returns>
        Task<Departamento?> GetByNomeAsync(string nome);

        /// <summary>
        /// Busca apenas departamentos ativos no sistema
        /// </summary>
        /// <returns>Lista de departamentos com status ativo</returns>
        Task<List<Departamento>> GetAtivosAsync();
    }
}
