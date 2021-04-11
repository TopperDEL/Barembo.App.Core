using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using uplink.NET.Interfaces;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class BackgroundUploadInfoViewModelTest
    {
        BackgroundUploadInfoViewModel _viewModel;
        Moq.Mock<IUploadQueueService> _uploadQueueService;

        [TestInitialize]
        public void Init()
        {
            _uploadQueueService = new Moq.Mock<IUploadQueueService>();
            _viewModel = new BackgroundUploadInfoViewModel(_uploadQueueService.Object);
        }

        [TestMethod]
        public async Task CurrentUploaCount_IsEmpty_UntilRefreshed()
        {
            Assert.AreEqual(0, _viewModel.CurrentQueueCount);
        }

        [TestMethod]
        public async Task Refresh_Refreshes_CurrentUploadCount()
        {
            _uploadQueueService.Setup(s => s.GetOpenUploadCountAsync()).Returns(Task.FromResult(2));

            await _viewModel.RefreshAsync();

            Assert.AreEqual(2, _viewModel.CurrentQueueCount);
        }
    }
}
