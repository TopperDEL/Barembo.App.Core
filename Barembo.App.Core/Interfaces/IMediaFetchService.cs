using Barembo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Interfaces
{
    public interface IMediaFetchService
    {
        Task<Tuple<Attachment, Stream>> FetchMediaAsync();
    }
}
