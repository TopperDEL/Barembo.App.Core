using Barembo.App.Core.ViewModels;
using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Messages
{
    public class CreateBookEntryMessage : PubSubEvent<Tuple<BookReference, BookShelfViewModel>>
    {
    }
}
