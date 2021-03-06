﻿using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.Sql
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <summary>
        /// Deletes all attachments.
        /// </summary>
        public virtual async Task DeleteAllAttachments(SqlConnection connection, SqlTransaction transaction, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $"delete from {table}";
                await command.ExecuteNonQueryAsync(cancellation).ConfigureAwait(false);
            }
        }
    }
}