﻿using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Attachments;
using NServiceBus.Installation;
using NServiceBus.Settings;

class NeedToInstallSomething : INeedToInstallSomething
{
    AttachmentSettings settings;

    public NeedToInstallSomething(ReadOnlySettings settings)
    {
        this.settings = settings.GetOrDefault<AttachmentSettings>();
    }

    public async Task Install(string identity)
    {
        if (settings == null || settings.InstallerDisabled)
        {
            return;
        }

        using (var connection = await settings.ConnectionFactory().ConfigureAwait(false))
        {
            await Installer.CreateTable(connection, settings.Schema, settings.TableName)
                .ConfigureAwait(false);
        }
    }
}