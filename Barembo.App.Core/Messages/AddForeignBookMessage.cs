using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public class AddForeignBookMessage : PubSubEvent<Tuple<StoreAccess, BookShelf>>
    {
    }
}
