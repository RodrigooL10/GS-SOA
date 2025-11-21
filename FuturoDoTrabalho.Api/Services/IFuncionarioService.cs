using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Services
{
    // ====================================================================================
    // INTERFACE: I FUNCIONARIO SERVICE
    // ====================================================================================
    // Interface que define o contrato para a camada de lógica de negócio de funcionários.
    // Responsável por orquestrar operações entre repositories, aplicar validações de negócio,
    // e converter entre Models (entidades) e DTOs (objetos de transferência).
    // Atua como intermediário entre Controllers e Repositories.
    // ====================================================================================
    public interface IFuncionarioService
    {
        /// <summary>
        /// Busca um funcionário por ID e retorna como DTO
        /// </summary>
        /// <param name="id">Identificador único do funcionário</param>
        /// <returns>DTO do funcionário ou null se não encontrado</returns>
        Task<FuncionarioReadDto?> GetByIdAsync(int id);

        /// <summary>
        /// Busca todos os funcionários e retorna como lista de DTOs
        /// </summary>
        /// <returns>Lista de DTOs de funcionários</returns>
        Task<List<FuncionarioReadDto>> GetAllAsync();

        /// <summary>
        /// Busca funcionários com paginação (usado na v2 da API)
        /// </summary>
        /// <param name="pageNumber">Número da página (começa em 1)</param>
        /// <param name="pageSize">Quantidade de itens por página</param>
        /// <returns>Tupla contendo: lista de DTOs, total de registros, total de páginas</returns>
        Task<(List<FuncionarioReadDto> data, int totalCount, int pageCount)> GetPagedAsync(int pageNumber, int pageSize);

        /// <summary>
        /// Busca um funcionário pelo CPF
        /// </summary>
        /// <param name="cpf">CPF do funcionário (sem formatação)</param>
        /// <returns>DTO do funcionário ou null se não encontrado</returns>
        Task<FuncionarioReadDto?> GetByCpfAsync(string cpf);

        /// <summary>
        /// Busca funcionários de um departamento específico
        /// </summary>
        /// <param name="departamentoId">ID do departamento</param>
        /// <returns>Lista de DTOs de funcionários do departamento</returns>
        Task<List<FuncionarioReadDto>> GetByDepartamentoAsync(int departamentoId);

        /// <summary>
        /// Busca apenas funcionários ativos no sistema
        /// </summary>
        /// <returns>Lista de DTOs de funcionários ativos</returns>
        Task<List<FuncionarioReadDto>> GetAtivosAsync();

        /// <summary>
        /// Cria um novo funcionário com validações de negócio
        /// Valida: existência do departamento, unicidade do CPF
        /// </summary>
        /// <param name="dto">DTO com dados do funcionário a ser criado</param>
        /// <returns>DTO do funcionário criado</returns>
        /// <exception cref="InvalidOperationException">Lançada se departamento não existir ou CPF já estiver em uso</exception>
        Task<FuncionarioReadDto> CreateAsync(FuncionarioCreateDto dto);

        /// <summary>
        /// Atualiza completamente um funcionário existente (PUT)
        /// Todos os campos devem ser fornecidos
        /// </summary>
        /// <param name="id">Identificador único do funcionário</param>
        /// <param name="dto">DTO com dados atualizados</param>
        /// <returns>DTO do funcionário atualizado ou null se não encontrado</returns>
        /// <exception cref="InvalidOperationException">Lançada se departamento não existir</exception>
        Task<FuncionarioReadDto?> UpdateAsync(int id, FuncionarioUpdateDto dto);

        /// <summary>
        /// Atualiza parcialmente um funcionário (PATCH) - apenas v2
        /// Apenas campos fornecidos no DTO serão atualizados
        /// </summary>
        /// <param name="id">Identificador único do funcionário</param>
        /// <param name="dto">DTO com campos a atualizar (todos opcionais)</param>
        /// <returns>DTO do funcionário atualizado ou null se não encontrado</returns>
        Task<FuncionarioReadDto?> PatchAsync(int id, FuncionarioPatchDto dto);

        /// <summary>
        /// Remove um funcionário do sistema
        /// </summary>
        /// <param name="id">Identificador único do funcionário</param>
        /// <returns>True se removido com sucesso, False se não encontrado</returns>
        Task<bool> DeleteAsync(int id);
    }
}
