using IdentityToken.API.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityToken.API.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<IdentityAuthWallet>? IdentityAuthWallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityAuthWallet>().ToTable("IdentityAuthWallet");
    }
}