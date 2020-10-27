using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Messages
{
    public class SuccessfullyLoggedInMessage: PubSubEvent<StoreAccess>
    {
    }
}
