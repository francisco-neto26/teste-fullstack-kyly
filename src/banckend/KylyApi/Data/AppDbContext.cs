using KylyApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace KylyApi.Data;
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // Declaração única das coleções/tabelas acessíveis pelo EF Core
    public DbSet<Produto> Produtos => Set<Produto>();
    public DbSet<ListaRelevancia> ListasRelevancia => Set<ListaRelevancia>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Varre a pasta Configurations e aplica todas as regras de mapeamento automaticamente
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}