namespace FuturoDoTrabalho.Api.Repositories
{
    // ====================================================================================
    // INTERFACE: I GENERIC REPOSITORY
    // ====================================================================================
    // Interface genérica que define o contrato para operações CRUD básicas em qualquer entidade.
    // Implementa o padrão Repository, abstraindo o acesso aos dados e facilitando testes.
    // Outras interfaces específicas herdam desta para adicionar métodos customizados.
    // ====================================================================================
    /// <typeparam name="T">Tipo da entidade (deve ser uma classe)</typeparam>
    public interface IGenericRepository<T> where T : class
    {
        /// <summary>
        /// Busca uma entidade por seu ID
        /// </summary>
        /// <param name="id">Identificador único da entidade</param>
        /// <returns>Entidade encontrada ou null se não existir</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Busca todas as entidades do tipo T
        /// </summary>
        /// <returns>Lista com todas as entidades</returns>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// Busca entidades com paginação (útil para grandes volumes de dados)
        /// </summary>
        /// <param name="pageNumber">Número da página (começa em 1)</param>
        /// <param name="pageSize">Quantidade de itens por página</param>
        /// <returns>Lista de entidades da página solicitada</returns>
        Task<List<T>> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Conta o total de entidades no banco de dados
        /// </summary>
        /// <returns>Total de registros</returns>
        Task<int> GetCountAsync();

        /// <summary>
        /// Cria uma nova entidade no banco de dados
        /// </summary>
        /// <param name="entity">Entidade a ser criada</param>
        /// <returns>Entidade criada com ID gerado</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Atualiza uma entidade existente no banco de dados
        /// </summary>
        /// <param name="entity">Entidade com dados atualizados</param>
        /// <returns>Entidade atualizada ou null em caso de erro</returns>
        Task<T?> UpdateAsync(T entity);

        /// <summary>
        /// Remove uma entidade do banco de dados por ID
        /// </summary>
        /// <param name="id">Identificador único da entidade a ser removida</param>
        /// <returns>True se removido com sucesso, False se não encontrado</returns>
        Task<bool> DeleteAsync(int id);

        /// <summary>
        /// Salva alterações pendentes no banco de dados
        /// Útil quando múltiplas operações são realizadas antes de salvar
        /// </summary>
        /// <returns>True se salvo com sucesso</returns>
        Task<bool> SaveChangesAsync();
    }
}
