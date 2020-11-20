using Barembo.App.Core.Interfaces;
using Barembo.App.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.Services
{
    [TestClass]
    public class ThumbnailGeneratorServiceTest
    {
        private IThumbnailGeneratorService _service;

        [TestInitialize]
        public void Init()
        {
            _service = new ThumbnailGeneratorService();
        }

        [TestMethod]
        public async Task Thumbnail_Gets_Generated()
        {
            try
            {
                File.Delete("TestImageThumbnail.jpg");
            }
            catch { }

            FileStream image = new FileStream("TestImage.jpg", FileMode.Open);
            var result = await _service.GenerateThumbnailFromImageAsync(image);
            var bytes = new byte[result.Length];
            await result.ReadAsync(bytes, 0, bytes.Length);
            await File.WriteAllBytesAsync("TestImageThumbnail.jpg", bytes);

            Assert.IsTrue(File.Exists("TestImageThumbnail.jpg"));
        }
    }
}
