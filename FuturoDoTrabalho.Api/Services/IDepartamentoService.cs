using FuturoDoTrabalho.Api.DTOs;

namespace FuturoDoTrabalho.Api.Services
{
    // ====================================================================================
    // INTERFACE: I DEPARTAMENTO SERVICE
    // ====================================================================================
    // Interface que define o contrato para a camada de lógica de negócio de departamentos.
    // Responsável por orquestrar operações entre repositories, aplicar validações de negócio,
    // e converter entre Models (entidades) e DTOs (objetos de transferência).
    // Atua como intermediário entre Controllers e Repositories.
    // ====================================================================================
    public interface IDepartamentoService
    {
        /// <summary>
        /// Busca um departamento por ID e retorna como DTO
        /// </summary>
        /// <param name="id">Identificador único do departamento</param>
        /// <returns>DTO do departamento ou null se não encontrado</returns>
        Task<DepartamentoReadDto?> GetByIdAsync(int id);

        /// <summary>
        /// Busca todos os departamentos e retorna como lista de DTOs
        /// </summary>
        /// <returns>Lista de DTOs de departamentos</returns>
        Task<IEnumerable<DepartamentoReadDto>> GetAllAsync();

        /// <summary>
        /// Busca departamentos com paginação (usado na v2 da API)
        /// </summary>
        /// <param name="pageNumber">Número da página (começa em 1)</param>
        /// <param name="pageSize">Quantidade de itens por página</param>
        /// <returns>Tupla contendo: lista de DTOs, total de registros, total de páginas</returns>
        Task<(IEnumerable<DepartamentoReadDto> data, int totalCount, int pageCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Busca um departamento pelo nome (nome é único no sistema)
        /// </summary>
        /// <param name="nome">Nome do departamento</param>
        /// <returns>DTO do departamento ou null se não encontrado</returns>
        Task<DepartamentoReadDto?> GetByNomeAsync(string nome);

        /// <summary>
        /// Busca apenas departamentos ativos no sistema
        /// </summary>
        /// <returns>Lista de DTOs de departamentos ativos</returns>
        Task<IEnumerable<DepartamentoReadDto>> GetAtivosAsync();

        /// <summary>
        /// Cria um novo departamento com validações de negócio
        /// Valida: unicidade do nome
        /// </summary>
        /// <param name="dto">DTO com dados do departamento a ser criado</param>
        /// <returns>DTO do departamento criado</returns>
        /// <exception cref="InvalidOperationException">Lançada se já existir departamento com o mesmo nome</exception>
        Task<DepartamentoReadDto> CreateAsync(DepartamentoCreateDto dto);

        /// <summary>
        /// Atualiza completamente um departamento existente (PUT)
        /// Todos os campos devem ser fornecidos
        /// </summary>
        /// <param name="id">Identificador único do departamento</param>
        /// <param name="dto">DTO com dados atualizados</param>
        /// <returns>DTO do departamento atualizado ou null se não encontrado</returns>
        /// <exception cref="InvalidOperationException">Lançada se já existir outro departamento com o mesmo nome</exception>
        Task<DepartamentoReadDto?> UpdateAsync(int id, DepartamentoUpdateDto dto);

        /// <summary>
        /// Atualiza parcialmente um departamento (PATCH) - apenas v2
        /// Apenas campos fornecidos no DTO serão atualizados
        /// </summary>
        /// <param name="id">Identificador único do departamento</param>
        /// <param name="dto">DTO com campos a atualizar (todos opcionais)</param>
        /// <returns>DTO do departamento atualizado ou null se não encontrado</returns>
        /// <exception cref="InvalidOperationException">Lançada se já existir outro departamento com o mesmo nome</exception>
        Task<DepartamentoReadDto?> PatchAsync(int id, DepartamentoPatchDto dto);

        /// <summary>
        /// Remove um departamento do sistema
        /// Valida: não pode deletar departamento que tenha funcionários associados
        /// </summary>
        /// <param name="id">Identificador único do departamento</param>
        /// <returns>True se removido com sucesso, False se não encontrado</returns>
        /// <exception cref="InvalidOperationException">Lançada se o departamento tiver funcionários associados</exception>
        Task<bool> DeleteAsync(int id);
    }
}
