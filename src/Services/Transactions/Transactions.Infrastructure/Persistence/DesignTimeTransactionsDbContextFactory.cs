//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.Extensions.Configuration;

//namespace Transactions.Infrastructure.Persistence;

///// <summary>
///// Used by EF Core tools at design time (e.g. add-migration, update-database)
///// so that the DbContext can be created without the full application DI container
///// (no IMediator or other runtime services required).
///// </summary>
//public sealed class DesignTimeTransactionsDbContextFactory : IDesignTimeDbContextFactory<TransactionsDbContext>
//{
//    private const string DefaultConnectionString =
//        "Server=(localdb)\\mssqllocaldb;Database=TransactionsDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

//    public TransactionsDbContext CreateDbContext(string[] args)
//    {
//        var configuration = new ConfigurationBuilder()
//            .SetBasePath(Directory.GetCurrentDirectory())
//            .AddJsonFile("appsettings.json", optional: true)
//            .AddJsonFile("appsettings.Development.json", optional: true)
//            .AddEnvironmentVariables()
//            .Build();

//        var connectionString = configuration.GetConnectionString("DefaultConnection") ?? DefaultConnectionString;

//        var optionsBuilder = new DbContextOptionsBuilder<TransactionsDbContext>();
//        optionsBuilder.UseSqlServer(
//            connectionString,
//            b =>
//            {
//                b.CommandTimeout(30);
//                b.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null);
//            });

//        // No interceptors at design time (avoids IMediator and other runtime dependencies)
//        return new TransactionsDbContext(optionsBuilder.Options);
//    }
//}
