using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Interfaces
{
    public interface IQRCodeScannerService
    {
        Task<string> ScanQRCodeAsync();
    }
}
