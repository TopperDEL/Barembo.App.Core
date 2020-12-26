using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public class MediaResult
    {
        public bool MediaSelected { get; set; }
        public Stream Stream { get; set; }
        public Attachment Attachment { get; set; }
    }
    public class MediaRequestedMessage : PubSubEvent<MediaResult>
    {
    }
}
