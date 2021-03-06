﻿using System;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments.FileShare;
using NServiceBus.Features;
using NServiceBus.ObjectBuilder;

class AttachmentFeature : Feature
{
    protected override void Setup(FeatureConfigurationContext context)
    {
        var readOnlySettings = context.Settings;
        var settings = readOnlySettings.Get<AttachmentSettings>();

        var pipeline = context.Pipeline;
        var persister = new Persister(settings.FileShare);
        pipeline.Register(new ReceiveRegistration(persister));
        pipeline.Register(new SendRegistration(persister, settings.TimeToKeep));
        if (context.Settings.PurgeOnStartup())
        {
            context.RegisterStartupTask(builder => new PurgeTask(persister));
        }
        if (settings.RunCleanTask)
        {
            context.RegisterStartupTask(builder => CreateCleaner(persister, builder));
        }
    }

    static Cleaner CreateCleaner(IPersister persister, IBuilder builder)
    {
        return new Cleaner(token =>
            {
                persister.CleanupItemsOlderThan(DateTime.UtcNow, token);
                return Task.CompletedTask;
            },
            criticalError: builder.Build<CriticalError>().Raise,
            frequencyToRunCleanup: TimeSpan.FromHours(1),
            timer: new AsyncTimer());
    }
}