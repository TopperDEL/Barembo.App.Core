using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Messages
{
    /// <summary>
    /// This message gets sent if a new BookShelf got created
    /// </summary>
    public class BookShelfCreatedMessage : PubSubEvent<StoreAccess>
    {
    }
}
