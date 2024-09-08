using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data.SqlTypes;

namespace Packt.Shared;

public static class NorthwindContextExtensions
{
    ///<summary>
    /// Adds NorthwindContext to the specified IserviceCollection. Uses the SqlServer database provider.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="connectionString">Set to override the default of ".."</param>
    /// <returns>An IServiceCollection that can be used to add more services.</returns>
    /// 
    public static IServiceCollection AddNorthwindContext(this IServiceCollection services, string connectionString = @"Data Source=DESKTOP-A8O78SR;Initial Catalog=Northwind;Integrated Security=True;Encrypt=True")
    {
        services.AddDbContext<NorthwindContext>(options =>
        options.UseSqlServer(connectionString)
        .UseLoggerFactory(new ConsoleLoggerFactory())
        );
        return services;
    }
}