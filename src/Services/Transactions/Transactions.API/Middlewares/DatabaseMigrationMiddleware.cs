using Microsoft.EntityFrameworkCore;
using Transactions.Infrastructure.Persistence;
namespace Transactions.API.Middlewares
{
    public class DatabaseMigrationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseMigrationMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;
        private static bool _migrationChecked = false;
        private static readonly object _lock = new object();

        public DatabaseMigrationMiddleware(
            RequestDelegate next,
            ILogger<DatabaseMigrationMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context, TransactionsDbContext dbContext)
        {
            if (!_migrationChecked)
            {
                lock (_lock)
                {
                    if (!_migrationChecked)
                    {
                        // Only apply migrations in Staging or Production

                        try
                        {
                            _logger.LogInformation(
                                "Checking for pending database migrations in {Environment} environment...",
                                _environment.EnvironmentName);

                            // Get pending migrations
                            var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();

                            if (pendingMigrations.Any())
                            {
                                _logger.LogWarning("Found {Count} pending migration(s). Applying migrations...", pendingMigrations.Count);

                                foreach (var migration in pendingMigrations)
                                {
                                    _logger.LogInformation("Pending migration: {MigrationName}", migration);
                                }

                                // Apply pending migrations
                                dbContext.Database.Migrate();

                                _logger.LogInformation("Successfully applied all pending migrations in {Environment}.", _environment.EnvironmentName);
                            }
                            else
                            {
                                _logger.LogInformation("Database is up to date. No pending migrations found.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to apply database migrations in {Environment}. Application startup aborted.", _environment.EnvironmentName);
                            throw; // Fail fast - don't start the app if migrations fail
                        }



                        _migrationChecked = true;
                    }
                }
            }

            await _next(context);
        }
    }
}

