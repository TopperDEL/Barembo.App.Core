using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public enum ErrorType
    {
        EntryCouldNotBeCreated,
        EntryCouldNotBeSavedException,
        NoTargetBookSelected,
        AttachmentCouldNotBeSaved,
        ThumbnailCouldNotBeSet,
        BookShareCouldNotBeSavedException,
        CouldNotImportBook,
        NoBarcodeScanned,
        NoBookShareReferenceMagicLink
    }
    public class ErrorMessage : PubSubEvent<Tuple<ErrorType, string>>
    {
    }
}
