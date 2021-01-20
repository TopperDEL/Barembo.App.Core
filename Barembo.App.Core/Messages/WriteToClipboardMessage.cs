using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public class WriteToClipboardMessage : PubSubEvent<string>
    {
    }
}
