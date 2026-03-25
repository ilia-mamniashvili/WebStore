using Microsoft.EntityFrameworkCore;
using WebStore.Application.DTOs;
using WebStore.Application.Interfaces.Repositories;

namespace Webstore.Infrastructure;         

public sealed class StoreDbContext : DbContext
{
    public StoreDbContext() { }
    public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options) { }
    public DbSet<Product>? Products { get; set; }
    public DbSet<Category>? Categories { get; set; }
    public DbSet<Cart>? Carts { get; set; }
    public DbSet<CartItem>? CartItems { get; set; }
    public DbSet<Order>? Orders { get; set; }
    public DbSet<OrderItem>? OrderItems { get; set; }
    public DbSet<User>? Users { get; set; }
    public DbSet<Admin>? Admins { get; set; }
    public DbSet<Customer>? Customers { get; set; }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Deleted || entry.Entity is not IDisable disableEntity) continue;

            entry.State = EntityState.Modified;
            disableEntity.Activity.IsActive = false;
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Deleted || entry.Entity is not IDisable disableEntity) continue;

            entry.State = EntityState.Modified;
            disableEntity.Activity.IsActive = false;
        }

        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(StoreDbContext).Assembly,
            type => type.Namespace == "Webstore.Infrastructure.EntityConfigurations"
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionString);
        }
    }
}