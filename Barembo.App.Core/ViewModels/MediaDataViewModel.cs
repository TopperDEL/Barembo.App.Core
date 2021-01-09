using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public class MediaDataViewModel : AsyncLoadingBindableBase
    {
        private readonly IThumbnailGeneratorService _thumbnailGeneratorService;

        private string ThumbnailBase64 = null;

        public MediaData MediaData { get; set; }

        public byte[] Thumbnail
        {
            get
            {
                if (ThumbnailBase64 == null)
                {
                    GenerateThumbnailAsync();
                    return null;
                }
                else if (string.IsNullOrEmpty(ThumbnailBase64))
                    return null;

                return Convert.FromBase64String(ThumbnailBase64);
            }
        }

        public MediaDataViewModel(MediaData mediaData, IThumbnailGeneratorService thumbnailGeneratorService)
        {
            _thumbnailGeneratorService = thumbnailGeneratorService;

            MediaData = mediaData;
        }

        internal async Task GenerateThumbnailAsync()
        {
            if (IsLoading)
                return;

            IsLoading = true;
            try
            {
                if (MediaData.Attachment.Type == AttachmentType.Image)
                    ThumbnailBase64 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromImageAsync(MediaData.Stream);
                else if (MediaData.Attachment.Type == AttachmentType.Video)
                    ThumbnailBase64 = await _thumbnailGeneratorService.GenerateThumbnailBase64FromVideoAsync(MediaData.Stream, 0, MediaData.FilePath);

                RaisePropertyChanged(nameof(Thumbnail));
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
