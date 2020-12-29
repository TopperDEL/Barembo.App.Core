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
        AttachmentCouldNotBeSaved,
        ThumbnailCouldNotBeSet
    }
    public class ErrorMessage : PubSubEvent<Tuple<ErrorType, string>>
    {
    }
}
