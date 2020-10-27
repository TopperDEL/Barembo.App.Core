using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Interfaces
{
    public interface ILoginService
    {
        bool GetIsLoggedIn();
        bool Login(StoreAccess storeAccess);
        void Logout();
    }
}
