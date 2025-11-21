using AutoMapper;
using FuturoDoTrabalho.Api.DTOs;
using FuturoDoTrabalho.Api.Models;
using FuturoDoTrabalho.Api.Repositories;

namespace FuturoDoTrabalho.Api.Services
{
    // ====================================================================================
    // SERVICE: FUNCIONARIO SERVICE
    // ====================================================================================
    // Camada de lógica de negócio para operações relacionadas a funcionários.
    // Responsável por validações, transformações e orquestração entre repositories.
    // Atua como intermediário entre Controllers e Repositories, aplicando regras de negócio.
    // ====================================================================================
    public class FuncionarioService : IFuncionarioService
    {
        private readonly IFuncionarioRepository _funcionarioRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly IMapper _mapper;

        public FuncionarioService(
            IFuncionarioRepository funcionarioRepository,
            IDepartamentoRepository departamentoRepository,
            IMapper mapper)
        {
            _funcionarioRepository = funcionarioRepository;
            _departamentoRepository = departamentoRepository;
            _mapper = mapper;
        }

        // Buscar funcionário por ID e converter para DTO
        public async Task<FuncionarioReadDto?> GetByIdAsync(int id)
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionario == null)
                return null;

            // Converter Model para DTO usando AutoMapper
            var dto = _mapper.Map<FuncionarioReadDto>(funcionario);
            
            // Preencher nome do departamento se existir relacionamento
            if (funcionario.Departamento != null)
                dto.DepartamentoNome = funcionario.Departamento.Nome;

            return dto;
        }

        // Buscar todos os funcionários e converter para DTOs
        public async Task<List<FuncionarioReadDto>> GetAllAsync()
        {
            var funcionarios = await _funcionarioRepository.GetAllAsync();
            var dtos = _mapper.Map<List<FuncionarioReadDto>>(funcionarios);

            // Preencher nome do departamento para cada funcionário
            foreach (var dto in dtos)
            {
                var funcionario = funcionarios.First(f => f.Id == dto.Id);
                if (funcionario.Departamento != null)
                    dto.DepartamentoNome = funcionario.Departamento.Nome;
            }

            return dtos;
        }

        // Buscar funcionários com paginação (usado na v2 da API)
        public async Task<(List<FuncionarioReadDto> data, int totalCount, int pageCount)> GetPagedAsync(int pageNumber, int pageSize)
        {
            var funcionarios = await _funcionarioRepository.GetPagedAsync(pageNumber, pageSize);
            var totalCount = await _funcionarioRepository.GetCountAsync();
            
            // Calcular total de páginas (arredondamento para cima)
            var pageCount = (totalCount + pageSize - 1) / pageSize;

            var dtos = _mapper.Map<List<FuncionarioReadDto>>(funcionarios);

            // Preencher nome do departamento para cada funcionário
            foreach (var dto in dtos)
            {
                var funcionario = funcionarios.First(f => f.Id == dto.Id);
                if (funcionario.Departamento != null)
                    dto.DepartamentoNome = funcionario.Departamento.Nome;
            }

            return (dtos, totalCount, pageCount);
        }

        // Buscar funcionário por CPF
        public async Task<FuncionarioReadDto?> GetByCpfAsync(string cpf)
        {
            var funcionario = await _funcionarioRepository.GetByCpfAsync(cpf);
            if (funcionario == null)
                return null;

            var dto = _mapper.Map<FuncionarioReadDto>(funcionario);
            if (funcionario.Departamento != null)
                dto.DepartamentoNome = funcionario.Departamento.Nome;

            return dto;
        }

        // Buscar funcionários por departamento
        public async Task<List<FuncionarioReadDto>> GetByDepartamentoAsync(int departamentoId)
        {
            var funcionarios = await _funcionarioRepository.GetByDepartamentoAsync(departamentoId);
            var dtos = _mapper.Map<List<FuncionarioReadDto>>(funcionarios);

            foreach (var dto in dtos)
            {
                var funcionario = funcionarios.First(f => f.Id == dto.Id);
                if (funcionario.Departamento != null)
                    dto.DepartamentoNome = funcionario.Departamento.Nome;
            }

            return dtos;
        }

        // Buscar apenas funcionários ativos
        public async Task<List<FuncionarioReadDto>> GetAtivosAsync()
        {
            var funcionarios = await _funcionarioRepository.GetAtivosAsync();
            var dtos = _mapper.Map<List<FuncionarioReadDto>>(funcionarios);

            foreach (var dto in dtos)
            {
                var funcionario = funcionarios.First(f => f.Id == dto.Id);
                if (funcionario.Departamento != null)
                    dto.DepartamentoNome = funcionario.Departamento.Nome;
            }

            return dtos;
        }

        // Criar novo funcionário com validações de negócio
        public async Task<FuncionarioReadDto> CreateAsync(FuncionarioCreateDto dto)
        {
            // Validar se o departamento informado existe
            var departamento = await _departamentoRepository.GetByIdAsync(dto.DepartamentoId);
            if (departamento == null)
                throw new InvalidOperationException("Departamento não encontrado");

            // Validar se CPF é único (se fornecido)
            if (!string.IsNullOrWhiteSpace(dto.CPF))
            {
                var existente = await _funcionarioRepository.GetByCpfAsync(dto.CPF);
                if (existente != null)
                    throw new InvalidOperationException("CPF já existe no sistema");
            }

            // Converter DTO para Model
            var funcionario = _mapper.Map<Funcionario>(dto);
            funcionario.DataCriacao = DateTime.UtcNow;

            // Salvar no banco de dados
            var criado = await _funcionarioRepository.CreateAsync(funcionario);
            criado.Departamento = departamento;

            // Converter de volta para DTO de resposta
            var resultado = _mapper.Map<FuncionarioReadDto>(criado);
            resultado.DepartamentoNome = departamento.Nome;

            return resultado;
        }

        // Atualizar funcionário completamente (PUT)
        public async Task<FuncionarioReadDto?> UpdateAsync(int id, FuncionarioUpdateDto dto)
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionario == null)
                return null;

            // Validar se o departamento informado existe
            var departamento = await _departamentoRepository.GetByIdAsync(dto.DepartamentoId);
            if (departamento == null)
                throw new InvalidOperationException("Departamento não encontrado");

            // Aplicar atualizações do DTO no Model
            _mapper.Map(dto, funcionario);
            funcionario.DataAtualizacao = DateTime.UtcNow;

            // Salvar alterações
            await _funcionarioRepository.UpdateAsync(funcionario);
            funcionario.Departamento = departamento;

            // Converter para DTO de resposta
            var resultado = _mapper.Map<FuncionarioReadDto>(funcionario);
            resultado.DepartamentoNome = departamento.Nome;

            return resultado;
        }

        // Atualizar funcionário parcialmente (PATCH) - apenas v2
        // Apenas campos fornecidos no DTO serão atualizados
        public async Task<FuncionarioReadDto?> PatchAsync(int id, FuncionarioPatchDto dto)
        {
            var funcionario = await _funcionarioRepository.GetByIdAsync(id);
            if (funcionario == null)
                return null;

            // Atualizar apenas campos que foram fornecidos (não nulos)
            if (!string.IsNullOrWhiteSpace(dto.Nome))
                funcionario.Nome = dto.Nome;

            if (!string.IsNullOrWhiteSpace(dto.Cargo))
                funcionario.Cargo = dto.Cargo;

            if (!string.IsNullOrWhiteSpace(dto.Email))
                funcionario.Email = dto.Email;

            if (!string.IsNullOrWhiteSpace(dto.Telefone))
                funcionario.Telefone = dto.Telefone;

            if (dto.Salario.HasValue)
                funcionario.Salario = dto.Salario.Value;

            if (!string.IsNullOrWhiteSpace(dto.Endereco))
                funcionario.Endereco = dto.Endereco;

            if (dto.NivelSenioridade.HasValue)
                funcionario.NivelSenioridade = dto.NivelSenioridade.Value;

            if (dto.Ativo.HasValue)
                funcionario.Ativo = dto.Ativo.Value;

            funcionario.DataAtualizacao = DateTime.UtcNow;

            // Salvar alterações
            await _funcionarioRepository.UpdateAsync(funcionario);

            // Converter para DTO de resposta
            var resultado = _mapper.Map<FuncionarioReadDto>(funcionario);
            if (funcionario.Departamento != null)
                resultado.DepartamentoNome = funcionario.Departamento.Nome;

            return resultado;
        }

        // Deletar funcionário
        public async Task<bool> DeleteAsync(int id)
        {
            return await _funcionarioRepository.DeleteAsync(id);
        }
    }
}
