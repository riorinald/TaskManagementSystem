using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TaskManagementSystem.Infrastructure.Data
{
    public class DatabaseContextFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public DatabaseContextFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        public IDatabaseContext CreateDatabaseContext()
        {
            return new SqliteDatabaseContext(_configuration, _loggerFactory.CreateLogger<SqliteDatabaseContext>());
        }
    }
} 