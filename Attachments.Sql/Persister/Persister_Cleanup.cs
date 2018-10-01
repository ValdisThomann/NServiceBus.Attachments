﻿using System;
using System.Data;
using System.Data.Common;
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
        /// Deletes attachments older than <paramref name="dateTime"/>.
        /// </summary>
        public virtual async Task CleanupItemsOlderThan(DbConnection connection, DbTransaction transaction, DateTime dateTime, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $"delete from {table} where expiry < @date";
                var dateParameter = command.CreateParameter();
                dateParameter.ParameterName = "date";
                dateParameter.Value = dateTime;
                dateParameter.DbType = DbType.DateTime;
                command.Parameters.Add(dateParameter);
                await command.ExecuteNonQueryAsync(cancellation).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Deletes all items.
        /// </summary>
        public virtual async Task PurgeItems(DbConnection connection, DbTransaction transaction, CancellationToken cancellation = default)
        {
            Guard.AgainstNull(connection, nameof(connection));
            using (var command = connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = $@"
if exists (
    select * from sys.objects
    where
        object_id = object_id('{table}')
        and type in ('U')
)
begin

delete from {table}

end";
                await command.ExecuteNonQueryAsync(cancellation).ConfigureAwait(false);
            }
        }
    }
}