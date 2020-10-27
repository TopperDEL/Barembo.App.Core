using Barembo.Models;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Messages
{
    /// <summary>
    /// This message gets send whenever a Login was successfull
    /// </summary>
    public class SuccessfullyLoggedInMessage: PubSubEvent<StoreAccess>
    {
    }
}
