using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Theatre.Domain.Aggregates.Actors;
using Theatre.Domain.Aggregates.Shows;
using Theatre.Domain.Configurations;

namespace Theatre.Domain;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Actor> Actors { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Show> Shows { get; set; }
    public DbSet<Contract> Contracts { get; set; }

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ActorConfiguration());
        modelBuilder.ApplyConfiguration(new ContractConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new ShowConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        
        base.OnModelCreating(modelBuilder);
    }
}