using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Interfaces;

namespace Barembo.App.Core.ViewModels
{
    public class BackgroundUploadInfoViewModel : BindableBase
    {
        readonly IUploadQueueService _uploadQueueService;

        private int _currentQueueCount;
        public int CurrentQueueCount
        {
            get { return _currentQueueCount; }
            set { SetProperty(ref _currentQueueCount, value); }
        }

        public BackgroundUploadInfoViewModel(IUploadQueueService uploadQueueService)
        {
            _uploadQueueService = uploadQueueService;
        }

        public async Task RefreshAsync()
        {
            _currentQueueCount = await _uploadQueueService.GetOpenUploadCountAsync();
        }
    }
}
