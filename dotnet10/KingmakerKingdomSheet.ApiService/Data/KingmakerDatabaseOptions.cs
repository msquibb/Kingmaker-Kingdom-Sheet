using Microsoft.Data.Sqlite;

namespace KingmakerKingdomSheet.ApiService.Data;

public sealed class KingmakerDatabaseOptions
{
    public string ConnectionString { get; set; } = string.Empty;
}

internal static class SqliteConnectionStringNormalizer
{
    public static string Normalize(string connectionString, string contentRootPath)
    {
        var builder = new SqliteConnectionStringBuilder(connectionString)
        {
            ForeignKeys = true
        };

        if (!string.IsNullOrWhiteSpace(builder.DataSource)
            && !string.Equals(builder.DataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
            && !Path.IsPathRooted(builder.DataSource))
        {
            builder.DataSource = Path.GetFullPath(Path.Combine(contentRootPath, builder.DataSource));
        }

        return builder.ToString();
    }
}
