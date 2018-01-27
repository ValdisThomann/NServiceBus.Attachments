﻿using System;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;

class AttachmentFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var settings = context.Settings.Get<AttachmentSettings>();
        var pipeline = context.Pipeline;
        var streamPersister = new StreamPersister(settings.Schema, settings.TableName);
        pipeline.Register(new ReceiveRegistration(settings.ConnectionBuilder, streamPersister));
        pipeline.Register(new SendRegistration(settings.ConnectionBuilder, streamPersister, settings.TimeToKeep));
        if (settings.RunCleanTask)
        {
            context.RegisterStartupTask(builder => CreateCleaner(settings, streamPersister, builder));
        }
    }

    static Cleaner CreateCleaner(AttachmentSettings settings, StreamPersister streamPersister, IBuilder builder)
    {
        return new Cleaner(async token =>
            {
                using (var connection = await settings.ConnectionBuilder().ConfigureAwait(false))
                {
                    streamPersister.CleanupItemsOlderThan(connection, DateTime.UtcNow);
                }
            },
            criticalError: builder.Build<CriticalError>().Raise,
            frequencyToRunCleanup: TimeSpan.FromHours(1),
            timer: new AsyncTimer());
    }
}