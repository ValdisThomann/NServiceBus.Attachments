﻿using System.Data.SqlClient;
using System.Reflection;
using NServiceBus.Persistence;

class StorageAccessor
{
    bool? hasConnectionProperty;
    PropertyInfo connectionProperty;
    bool? hasTransactionProperty;
    PropertyInfo transactionProperty;

    public bool TryGetConnection(SynchronizedStorageSession storageSession, out SqlConnection connection)
    {
        if (!hasConnectionProperty.HasValue)
        {
            connectionProperty = GetProperty(storageSession, "Connection");
            hasConnectionProperty = connectionProperty != null;
        }

        if (!hasConnectionProperty.Value)
        {
            connection = null;
            return false;
        }

        connection = (SqlConnection) connectionProperty.GetValue(storageSession);
        return connection != null;
    }

    public bool TryGetTransaction(SynchronizedStorageSession storageSession, out SqlTransaction transaction)
    {
        if (!hasTransactionProperty.HasValue)
        {
            transactionProperty = GetProperty(storageSession, "Transaction");
            hasTransactionProperty = transactionProperty != null;
        }

        if (!hasTransactionProperty.Value)
        {
            transaction = null;
            return false;
        }

        transaction = (SqlTransaction) transactionProperty.GetValue(storageSession);
        return transaction != null;
    }

    static PropertyInfo GetProperty(SynchronizedStorageSession storageSession, string name)
    {
        return storageSession
            .GetType()
            .GetProperty(name, BindingFlags.NonPublic |BindingFlags.Public | BindingFlags.Instance);
    }
}