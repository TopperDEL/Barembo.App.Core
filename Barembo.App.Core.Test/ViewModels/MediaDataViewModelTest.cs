using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class MediaDataViewModelTest
    {
        MediaDataViewModel _viewModel;
        Moq.Mock<IThumbnailGeneratorService> _thumbnailGeneratorService;
        MediaData _mediaData;

        [TestInitialize]
        public void Init()
        {
            _thumbnailGeneratorService = new Moq.Mock<IThumbnailGeneratorService>();

            _mediaData = new MediaData();
            _mediaData.Attachment = new Models.Attachment();
            _mediaData.Attachment.Type = Models.AttachmentType.Image;

            _viewModel = new MediaDataViewModel(_mediaData, _thumbnailGeneratorService.Object);
        }

        [TestMethod]
        public void MediaData_Gets_Set()
        {
            Assert.AreEqual(_mediaData, _viewModel.MediaData);
        }

        [TestMethod]
        public void AccessingProperties_GeneratesThumbnail_OnlyOnce()
        {
            MemoryStream mstream = new MemoryStream();

            _thumbnailGeneratorService.Setup(s => s.GenerateThumbnailBase64FromImageAsync(mstream)).Returns(Task.FromResult("thumbnailBase64"));
            var thumbnail = _viewModel.Thumbnail; //Access property

            _thumbnailGeneratorService.Verify();
        }
    }
}
