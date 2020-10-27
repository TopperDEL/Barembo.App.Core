using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Interfaces
{
    /// <summary>
    /// A LoginService is responsible for Login and Logout of a user via a StoreAccess.
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Returns if the current user is logged in
        /// </summary>
        /// <returns>True, if the user is logged in; false if not</returns>
        bool GetIsLoggedIn();

        /// <summary>
        /// Returns the StoreAccess for the current user if logged in
        /// </summary>
        /// <returns>The StoreAccess of the user</returns>
        StoreAccess GetLogin();

        /// <summary>
        /// Logs a user in using the given StoreAccess
        /// </summary>
        /// <param name="storeAccess">The StoreAccess to use</param>
        /// <returns>True, if the Login was successfull; false if not</returns>
        bool Login(StoreAccess storeAccess);

        /// <summary>
        /// Logs the current user out
        /// </summary>
        void Logout();
    }
}
