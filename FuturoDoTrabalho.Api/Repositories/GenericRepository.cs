using Microsoft.EntityFrameworkCore;
using FuturoDoTrabalho.Api.Data;

namespace FuturoDoTrabalho.Api.Repositories
{
    // ====================================================================================
    // REPOSITORY: GENERIC REPOSITORY
    // ====================================================================================
    // Implementação genérica do padrão Repository que fornece operações CRUD básicas
    // para qualquer entidade. Reduz código duplicado e centraliza lógica de acesso a dados.
    // Outros repositories específicos herdam desta classe e adicionam métodos customizados.
    // ====================================================================================
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        // ====================
        // FIELDS
        // ====================
        // Entity Framework Core database context for data access
        protected readonly AppDbContext _context;

        // ====================
        // CONSTRUCTOR
        // ====================
        // Initializes repository with the provided database context
        public GenericRepository(AppDbContext context)
        {
            _context = context;
        }

        // ====================
        // METHOD: GetByIdAsync
        // Retrieves a single entity by its primary key ID
        // ====================
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // ====================
        // METHOD: GetAllAsync
        // Retrieves all entities from the database
        // ====================
        public async Task<List<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // ====================
        // METHOD: GetPagedAsync
        // Retrieves a specific page of entities for pagination support
        // Skips records from previous pages and takes only the requested page size
        // ====================
        public async Task<List<T>> GetPagedAsync(int pageNumber, int pageSize)
        {
            return await _context.Set<T>()
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // ====================
        // METHOD: GetCountAsync
        // Counts the total number of entities in the database
        // Useful for calculating total pages in pagination
        // ====================
        public async Task<int> GetCountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        // ====================
        // METHOD: CreateAsync
        // Creates and persists a new entity in the database
        // ====================
        public async Task<T> CreateAsync(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // ====================
        // METHOD: UpdateAsync
        // Updates an existing entity in the database
        // ====================
        public async Task<T?> UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        // ====================
        // METHOD: DeleteAsync
        // Deletes an entity from the database by ID
        // ====================
        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return false;

            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // ====================
        // METHOD: SaveChangesAsync
        // Explicitly saves pending changes to the database
        // Useful when multiple operations are performed before saving
        // ====================
        public async Task<bool> SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
