using IdentityToken.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityToken.API.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<IdentityAuthWallet>? IdentityAuthWallets { get; set; }
    public DbSet<AuthenticatedIdentity>? AuthenticatedIdentities { get; set; }
    public DbSet<ChatUser>? ChatUsers { get; set; }
    public DbSet<ChatMessage>? ChatMessages { get; set; }
    public DbSet<Profile>? Profiles { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IdentityAuthWallet>().ToTable("IdentityAuthWallets");
        modelBuilder.Entity<AuthenticatedIdentity>().ToTable("AuthenticatedIdentities");
        modelBuilder.Entity<ChatUser>().ToTable("ChatUsers");
        modelBuilder.Entity<ChatMessage>().ToTable("ChatMessages");
        modelBuilder.Entity<Profile>().ToTable("Profiles");
    }
}