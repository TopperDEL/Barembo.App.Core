﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public enum InAppInfoMessageType
    {
        EntrySaved,
        SavingAttachment,
        AttachmentSaved
    }
    public class InAppInfoMessage:PubSubEvent<Tuple<InAppInfoMessageType, Dictionary<string,string>>>
    {
    }
}
