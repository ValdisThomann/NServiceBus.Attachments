﻿using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Attachments.FileShare
#if Raw
    .Raw
#endif
{
    public partial class Persister
    {
        /// <summary>
        /// Copies an attachment to <paramref name="target"/>.
        /// </summary>
        public virtual Task CopyTo(string messageId, string name, Stream target, CancellationToken cancellation = default)
        {
            Guard.AgainstNullOrEmpty(messageId, nameof(messageId));
            Guard.AgainstNullOrEmpty(name, nameof(name));
            Guard.AgainstNull(target, nameof(target));
            var attachmentDirectory = GetAttachmentDirectory(messageId, name);
            var dataFile = GetDataFile(attachmentDirectory);
            ThrowIfFileNotFound(dataFile, messageId, name);
            return FileHelpers.CopyTo(target, cancellation, dataFile);
        }
    }
}