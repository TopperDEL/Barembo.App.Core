using Barembo.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public class AttachmentPreviewViewModel : BindableBase
    {
        internal readonly AttachmentPreview _attachmentPreview;

        public bool IsImage
        {
            get { return _attachmentPreview.Type == AttachmentType.Image; }
        }

        public bool IsVideo
        {
            get { return _attachmentPreview.Type == AttachmentType.Video; }
        }

        public byte[] ImagePreview
        {
            get
            {
                if (_attachmentPreview.PreviewPartsBase64.Count == 1)
                {
                    return Convert.FromBase64String(_attachmentPreview.PreviewPartsBase64[0]);
                }
                else
                {
                    return null;
                }
            }
        }

        public byte[] VideoPreview
        {
            get
            {
                if (_videoPreviewBytes.Count > _currentVideoPreviewImageNumber)
                {
                    return _videoPreviewBytes[_currentVideoPreviewImageNumber];
                }
                else
                {
                    return null;
                }
            }
        }

        private List<byte[]> _videoPreviewBytes;
        private int _currentVideoPreviewImageNumber;

        public AttachmentPreviewViewModel(AttachmentPreview attachmentPreview)
        {
            _attachmentPreview = attachmentPreview;
            InitVideoPreviewBytes();
        }

        public void ShowNextVideoImage()
        {
            _currentVideoPreviewImageNumber++;
            if (_currentVideoPreviewImageNumber > 5)
                _currentVideoPreviewImageNumber = 0;

            RaisePropertyChanged(nameof(VideoPreview));
        }

        private void InitVideoPreviewBytes()
        {
            if (_attachmentPreview.Type == AttachmentType.Video)
            {
                _currentVideoPreviewImageNumber = 0;
                _videoPreviewBytes = new List<byte[]>();
                foreach (var previewBase64 in _attachmentPreview.PreviewPartsBase64)
                    _videoPreviewBytes.Add(Convert.FromBase64String(previewBase64));
            }
        }
    }
}
