using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace IdentityToken.API.Data;

public class IdentityDbContextFactory : IDesignTimeDbContextFactory<IdentityDbContext>
{
    private readonly DbContextOptions<IdentityDbContext>? _options;

    public IdentityDbContextFactory()
    {

    }

    public IdentityDbContextFactory(DbContextOptions<IdentityDbContext> options)
    {
        _options = options;
    }

    public IdentityDbContext CreateDbContext(string[]? args = null)
    {
        if(_options == null && args != null && args.Length > 0)
        {
            var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();
            optionsBuilder.UseNpgsql(args[0]);
            return new IdentityDbContext(optionsBuilder.Options);
        }
        else if(_options != null)
        {
            return new IdentityDbContext(_options);
        }
        throw new Exception("No DbContextOptions");
    }
}

public static class IdentityDbContextFactoryServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityDbContextFactory(this IServiceCollection services, Action<DbContextOptionsBuilder<IdentityDbContext>> optionsAction)
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>();
        optionsAction(options);
        services.AddSingleton<IdentityDbContextFactory>(provider => new IdentityDbContextFactory(options.Options));
        return services;
    }
}