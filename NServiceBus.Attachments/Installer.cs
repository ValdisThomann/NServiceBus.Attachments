﻿using System.Data.SqlClient;
using System.IO;

namespace NServiceBus.Attachments
{
    public static class Installer
    {
        public static void CreateTable(SqlConnection connection, string schema = "dbo", string tableName = "NServiceBusAttachments")
        {
            Guard.AgainstNullOrEmpty(schema, nameof(schema));
            Guard.AgainstNullOrEmpty(tableName, nameof(tableName));
            using (var command = connection.CreateCommand())
            {
                command.CommandText = GetTableSql();
                command.Parameters.AddWithValue("schema", schema);
                command.Parameters.AddWithValue("tableName", tableName);
                command.ExecuteNonQuery();
            }
        }

        public static string GetTableSql()
        {
            using (var stream = typeof(Installer).Assembly.GetManifestResourceStream("NServiceBus.Attachments.Table.sql"))
            using (var streamReader = new StreamReader(stream))
            {
                return streamReader.ReadToEnd();
            }
        }
    }
}