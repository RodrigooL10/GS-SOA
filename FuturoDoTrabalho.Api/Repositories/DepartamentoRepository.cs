using Microsoft.EntityFrameworkCore;
using FuturoDoTrabalho.Api.Data;
using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Repositories
{
    // ====================================================================================
    // REPOSITORY: DEPARTAMENTO REPOSITORY
    // ====================================================================================
    // Repository específico para a entidade Departamento.
    // Herda operações CRUD básicas do GenericRepository e adiciona métodos customizados
    // para consultas específicas de departamentos (por nome, ativos, etc.).
    // ====================================================================================
    public class DepartamentoRepository : GenericRepository<Departamento>, IDepartamentoRepository
    {
        public DepartamentoRepository(AppDbContext context) : base(context)
        {
        }

        // Buscar departamento por nome (nome é único no sistema)
        public async Task<Departamento?> GetByNomeAsync(string nome)
        {
            return await _context.Departamentos
                .FirstOrDefaultAsync(d => d.Nome == nome);
        }

        // Buscar apenas departamentos ativos no sistema
        public async Task<List<Departamento>> GetAtivosAsync()
        {
            return await _context.Departamentos
                .Where(d => d.Ativo)
                .ToListAsync();
        }
    }
}
