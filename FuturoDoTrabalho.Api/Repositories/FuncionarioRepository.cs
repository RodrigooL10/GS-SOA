using Microsoft.EntityFrameworkCore;
using FuturoDoTrabalho.Api.Data;
using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Repositories
{
    // ====================================================================================
    // REPOSITORY: FUNCIONARIO REPOSITORY
    // ====================================================================================
    // Repository específico para a entidade Funcionario.
    // Herda operações CRUD básicas do GenericRepository e adiciona métodos customizados
    // para consultas específicas de funcionários (por CPF, departamento, nível, etc.).
    // ====================================================================================
    public class FuncionarioRepository : GenericRepository<Funcionario>, IFuncionarioRepository
    {
        public FuncionarioRepository(AppDbContext context) : base(context)
        {
        }

        // Buscar funcionário por CPF (incluindo dados do departamento)
        // Include carrega o relacionamento Departamento junto com o funcionário
        public async Task<Funcionario?> GetByCpfAsync(string cpf)
        {
            return await _context.Funcionarios
                .Include(f => f.Departamento) // Carregar departamento relacionado
                .FirstOrDefaultAsync(f => f.CPF == cpf);
        }

        // Buscar todos os funcionários de um departamento específico
        public async Task<List<Funcionario>> GetByDepartamentoAsync(int departamentoId)
        {
            return await _context.Funcionarios
                .Where(f => f.DepartamentoId == departamentoId)
                .Include(f => f.Departamento) // Carregar departamento relacionado
                .ToListAsync();
        }

        // Buscar funcionários por nível de senioridade
        // Níveis: 1=Júnior, 2=Pleno, 3=Sênior, 4=Especialista, 5=Arquiteto
        public async Task<List<Funcionario>> GetByNivelSenioridadeAsync(int nivel)
        {
            return await _context.Funcionarios
                .Where(f => f.NivelSenioridade == nivel)
                .Include(f => f.Departamento) // Carregar departamento relacionado
                .ToListAsync();
        }

        // Buscar apenas funcionários ativos no sistema
        public async Task<List<Funcionario>> GetAtivosAsync()
        {
            return await _context.Funcionarios
                .Where(f => f.Ativo)
                .Include(f => f.Departamento) // Carregar departamento relacionado
                .ToListAsync();
        }
    }
}
