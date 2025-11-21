using Microsoft.EntityFrameworkCore;
using FuturoDoTrabalho.Api.Models;

namespace FuturoDoTrabalho.Api.Data
{
    // ====================================================================================
    // DBCONTEXT: APPDBCONTEXT
    // ====================================================================================
    // Representa a sessão com o banco de dados e permite realizar operações CRUD.
    // Herda de DbContext do Entity Framework Core e configura o mapeamento entre
    // as entidades C# e as tabelas do banco de dados MySQL.
    // ====================================================================================
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // ====================================================================================
        // DBSETS - Representam as tabelas do banco de dados
        // ====================================================================================
        // Cada DbSet representa uma tabela e permite realizar consultas e operações CRUD.
        // ====================================================================================

        // Tabela de funcionários
        public DbSet<Funcionario> Funcionarios { get; set; }

        // Tabela de departamentos
        public DbSet<Departamento> Departamentos { get; set; }

        // Tabela de usuários
        public DbSet<Usuario> Usuarios { get; set; }

        // ====================================================================================
        // CONFIGURAÇÃO DO MODELO (FLUENT API)
        // ====================================================================================
        // Método chamado pelo EF Core para configurar o modelo de dados.
        // Aqui definimos índices, relacionamentos, valores padrão e outras configurações.
        // ====================================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ====================================================================================
            // CONFIGURAÇÕES DA ENTIDADE FUNCIONARIO
            // ====================================================================================

            // Configurar precisão decimal para o campo Salário (10 dígitos, 2 decimais)
            modelBuilder.Entity<Funcionario>()
                .Property(f => f.Salario)
                .HasPrecision(10, 2);

            // Definir valor padrão para DataCriacao usando função SQL do MySQL
            modelBuilder.Entity<Funcionario>()
                .Property(f => f.DataCriacao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Criar índice único para CPF (garante que não haverá CPFs duplicados)
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.CPF)
                .IsUnique();

            // Criar índice para Email (melhora performance em buscas por email)
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Email);

            // Criar índice para DepartamentoId (melhora performance em joins e filtros)
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.DepartamentoId);

            // Criar índice para Ativo (melhora performance em filtros por status)
            modelBuilder.Entity<Funcionario>()
                .HasIndex(f => f.Ativo);

            // ====================================================================================
            // RELACIONAMENTO FUNCIONARIO -> DEPARTAMENTO
            // ====================================================================================
            // Configura o relacionamento muitos-para-um: um funcionário pertence a um departamento,
            // e um departamento pode ter muitos funcionários.
            // DeleteBehavior.Restrict impede deletar um departamento que tenha funcionários.
            // ====================================================================================
            modelBuilder.Entity<Funcionario>()
                .HasOne(f => f.Departamento)
                .WithMany(d => d.Funcionarios)
                .HasForeignKey(f => f.DepartamentoId)
                .OnDelete(DeleteBehavior.Restrict);

            // ====================================================================================
            // CONFIGURAÇÕES DA ENTIDADE DEPARTAMENTO
            // ====================================================================================

            // Definir valor padrão para DataCriacao usando DateTime.UtcNow
            modelBuilder.Entity<Departamento>()
                .Property(d => d.DataCriacao)
                .HasDefaultValue(DateTime.UtcNow);

            // Criar índice único para Nome (garante que não haverá departamentos com mesmo nome)
            modelBuilder.Entity<Departamento>()
                .HasIndex(d => d.Nome)
                .IsUnique();

            // ====================================================================================
            // CONFIGURAÇÕES DA ENTIDADE USUARIO
            // ====================================================================================

            // Criar índice único para NomeUsuario
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.NomeUsuario)
                .IsUnique();

            // Criar índice único para Email
            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Definir valor padrão para DataCriacao
            modelBuilder.Entity<Usuario>()
                .Property(u => u.DataCriacao)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Configurar o tipo de dado para Perfil (enum)
            modelBuilder.Entity<Usuario>()
                .Property(u => u.Perfil)
                .HasConversion<string>();

            // ====================================================================================
            // RELACIONAMENTO USUARIO -> FUNCIONARIO
            // ====================================================================================
            // Um usuário pode estar vinculado a um funcionário (opcional)
            // Um funcionário pode ter no máximo um usuário
            // ====================================================================================
            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Funcionario)
                .WithOne()
                .HasForeignKey<Usuario>(u => u.FuncionarioId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
