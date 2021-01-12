using Barembo.App.Core.ViewModels;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class AttachmentPreviewViewModelTest
    {
        AttachmentPreviewViewModel _viewModel;

        [TestInitialize]
        public void Init()
        {
        }

        [TestMethod]
        public void Image_Returns_Image()
        {
            List<string> imageParts = new List<string>();
            imageParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("image")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Image, imageParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);

            Assert.IsTrue(_viewModel.IsImage);
            Assert.IsFalse(_viewModel.IsVideo);
            Assert.AreEqual("image", Encoding.UTF8.GetString(_viewModel.ImagePreview));
        }

        [TestMethod]
        public void Image_Sets_IsImage()
        {
            List<string> imageParts = new List<string>();
            imageParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("image")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Image, imageParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);

            Assert.IsTrue(_viewModel.IsImage);
            Assert.IsFalse(_viewModel.IsVideo);
        }

        [TestMethod]
        public void Video_Sets_IsVideo()
        {
            List<string> videoParts = new List<string>();
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video1")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video2")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video3")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video4")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video5")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video6")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Video, videoParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);

            Assert.IsFalse(_viewModel.IsImage);
            Assert.IsTrue(_viewModel.IsVideo);
        }

        [TestMethod]
        public void Video_Returns_FirstVideoImageAtStart()
        {
            List<string> videoParts = new List<string>();
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video1")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video2")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video3")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video4")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video5")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video6")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Video, videoParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);

            Assert.AreEqual("Video1", Encoding.UTF8.GetString(_viewModel.VideoPreview));
        }

        [TestMethod]
        public void Video_ReturnsSecondVideoImage_AfterShowNext()
        {
            List<string> videoParts = new List<string>();
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video1")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video2")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video3")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video4")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video5")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video6")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Video, videoParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);
            _viewModel.ShowNextVideoImage();

            Assert.AreEqual("Video2", Encoding.UTF8.GetString(_viewModel.VideoPreview));
        }

        [TestMethod]
        public void Video_ReturnsNextVideoImageAndShowsAll()
        {
            List<string> videoParts = new List<string>();
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video1")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video2")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video3")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video4")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video5")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video6")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Video, videoParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);

            Assert.AreEqual("Video1", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video2", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video3", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video4", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video5", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video6", Encoding.UTF8.GetString(_viewModel.VideoPreview));
        }

        [TestMethod]
        public void Video_ReturnsToFirstImageAfterLastOne()
        {
            List<string> videoParts = new List<string>();
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video1")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video2")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video3")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video4")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video5")));
            videoParts.Add(Convert.ToBase64String(Encoding.UTF8.GetBytes("Video6")));

            AttachmentPreview attachmentPreview = new AttachmentPreview(AttachmentType.Video, videoParts);
            _viewModel = new AttachmentPreviewViewModel(attachmentPreview);

            Assert.AreEqual("Video1", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video2", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video3", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video4", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video5", Encoding.UTF8.GetString(_viewModel.VideoPreview));
            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video6", Encoding.UTF8.GetString(_viewModel.VideoPreview));

            _viewModel.ShowNextVideoImage();
            Assert.AreEqual("Video1", Encoding.UTF8.GetString(_viewModel.VideoPreview));
        }
    }
}
