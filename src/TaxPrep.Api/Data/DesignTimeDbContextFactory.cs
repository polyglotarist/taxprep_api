using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TaxPrep.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaxPrepDbContext>
{
    public TaxPrepDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("Default")
                 ?? "Server=localhost,1433;Database=TaxPrepDb;User Id=sa;Password=Password123;TrustServerCertificate=True";

        var opts = new DbContextOptionsBuilder<TaxPrepDbContext>()
            .UseSqlServer(cs)
            .Options;

        return new TaxPrepDbContext(opts);
    }
}
