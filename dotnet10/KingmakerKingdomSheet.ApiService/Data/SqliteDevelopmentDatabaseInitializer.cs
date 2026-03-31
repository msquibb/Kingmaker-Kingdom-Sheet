using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Options;

namespace KingmakerKingdomSheet.ApiService.Data;

public sealed class SqliteDevelopmentDatabaseInitializer(
    IOptions<KingmakerDatabaseOptions> databaseOptions,
    IWebHostEnvironment environment,
    ILogger<SqliteDevelopmentDatabaseInitializer> logger)
{
    private readonly string _connectionString = databaseOptions.Value.ConnectionString;
    private readonly string _schemaPath = Path.Combine(environment.ContentRootPath, "Data", "Sqlite", "SqliteDevelopmentSchema.sql");
    private readonly ILogger<SqliteDevelopmentDatabaseInitializer> _logger = logger;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_schemaPath))
        {
            throw new FileNotFoundException("The SQLite development schema script was not found.", _schemaPath);
        }

        var connectionStringBuilder = new SqliteConnectionStringBuilder(_connectionString);

        if (!string.IsNullOrWhiteSpace(connectionStringBuilder.DataSource)
            && !string.Equals(connectionStringBuilder.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase))
        {
            var dataDirectory = Path.GetDirectoryName(connectionStringBuilder.DataSource);

            if (!string.IsNullOrWhiteSpace(dataDirectory))
            {
                Directory.CreateDirectory(dataDirectory);
            }
        }

        var schemaScript = await File.ReadAllTextAsync(_schemaPath, cancellationToken);

        await using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);

        await using var command = connection.CreateCommand();
        command.CommandText = schemaScript;
        await command.ExecuteNonQueryAsync(cancellationToken);

        _logger.LogInformation("Initialized SQLite development database at {DataSource}.", connection.DataSource);
    }
}
