using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public class MediaData
    {
        public Stream Stream { get; set; }
        public Attachment Attachment { get; set; }
        public string FilePath { get; set; }
    }

    public class MediaReceivedMessage : PubSubEvent<MediaData>
    {
    }
}
