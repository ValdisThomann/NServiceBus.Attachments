using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    /// <summary>
    /// Used to take control over the storage table creation.
    /// </summary>
    public static class Installer
    {
        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static Task CreateTable(string connection, CancellationToken cancellation = default)
        {
            return CreateTable(connection, "MessageAttachments", cancellation);
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static async Task CreateTable(string connection, Table table, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(connection, nameof(connection));
            using (var dbConnection = new SqlConnection(connection))
            {
                await dbConnection.OpenAsync(cancellation).ConfigureAwait(false);
                await CreateTable(dbConnection, table, cancellation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static Task CreateTable(DbConnection dbConnection, CancellationToken cancellation = default)
        {
            return CreateTable(dbConnection, "MessageAttachments", cancellation);
        }

        /// <summary>
        /// Create the attachments storage table.
        /// </summary>
        public static async Task CreateTable(DbConnection dbConnection, Table table, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(dbConnection, nameof(dbConnection));
            Guard.AgainstNull(table, nameof(table));
            using (var command = dbConnection.CreateCommand())
            {
                command.CommandText = GetTableSql();
                command.AddParameter("schema", table.Schema);
                command.AddParameter("table", table.TableName);
                await command.ExecuteNonQueryAsync(cancellation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Get the sql used to create the attachments storage table.
        /// </summary>
        public static string GetTableSql()
        {
            using (var stream = AssemblyHelper.Current.GetManifestResourceStream("Table.sql"))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}