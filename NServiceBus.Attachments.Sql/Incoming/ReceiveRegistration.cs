﻿using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

class ReceiveRegistration :
    RegisterStep
{
    public ReceiveRegistration(Func<Task<SqlConnection>> connectionBuilder, StreamPersister streamPersister)
        : base(
            stepId: $"{AssemblyHelper.Name}Receive",
            behavior: typeof(ReceiveBehavior),
            description: "Copies the shared data back to the logical messages",
            factoryMethod: builder => new ReceiveBehavior(connectionBuilder, streamPersister))
    {
    }
}