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
                if (_attachmentPreview.PreviewPartsBase64?.Count == 1)
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
                if (_videoPreviewBytes?.Count > _currentVideoPreviewImageNumber)
                {
                    return _videoPreviewBytes[_currentVideoPreviewImageNumber];
                }
                else
                {
                    return null;
                }
            }
        }

        public byte[] VideoPreview1 { get { if (_videoPreviewBytes?.Count > 0) return _videoPreviewBytes[0]; else { return null; } } }
        public byte[] VideoPreview2 { get { if (_videoPreviewBytes?.Count > 0) return _videoPreviewBytes[1]; else { return null; } } }
        public byte[] VideoPreview3 { get { if (_videoPreviewBytes?.Count > 0) return _videoPreviewBytes[2]; else { return null; } } }
        public byte[] VideoPreview4 { get { if (_videoPreviewBytes?.Count > 0) return _videoPreviewBytes[3]; else { return null; } } }
        public byte[] VideoPreview5 { get { if (_videoPreviewBytes?.Count > 0) return _videoPreviewBytes[4]; else { return null; } } }
        public byte[] VideoPreview6 { get { if (_videoPreviewBytes?.Count > 0) return _videoPreviewBytes[5]; else { return null; } } }
        public bool ShowVideoPreview1 { get { return _currentVideoPreviewImageNumber == 0; } }
        public bool ShowVideoPreview2 { get { return _currentVideoPreviewImageNumber == 1; } }
        public bool ShowVideoPreview3 { get { return _currentVideoPreviewImageNumber == 2; } }
        public bool ShowVideoPreview4 { get { return _currentVideoPreviewImageNumber == 3; } }
        public bool ShowVideoPreview5 { get { return _currentVideoPreviewImageNumber == 4; } }
        public bool ShowVideoPreview6 { get { return _currentVideoPreviewImageNumber == 5; } }

        private List<byte[]> _videoPreviewBytes;
        private int _currentVideoPreviewImageNumber;

        public AttachmentPreviewViewModel(AttachmentPreview attachmentPreview)
        {
            _attachmentPreview = attachmentPreview;
            InitVideoPreviewBytes();
        }

        public virtual void ShowNextVideoImage()
        {
            _currentVideoPreviewImageNumber++;
            if (_currentVideoPreviewImageNumber > 5)
                _currentVideoPreviewImageNumber = 0;

            RaisePropertyChanged(nameof(VideoPreview));
            RaisePropertyChanged(nameof(ShowVideoPreview1));
            RaisePropertyChanged(nameof(ShowVideoPreview2));
            RaisePropertyChanged(nameof(ShowVideoPreview3));
            RaisePropertyChanged(nameof(ShowVideoPreview4));
            RaisePropertyChanged(nameof(ShowVideoPreview5));
            RaisePropertyChanged(nameof(ShowVideoPreview6));
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

            RaisePropertyChanged(nameof(VideoPreview1));
            RaisePropertyChanged(nameof(VideoPreview2));
            RaisePropertyChanged(nameof(VideoPreview3));
            RaisePropertyChanged(nameof(VideoPreview4));
            RaisePropertyChanged(nameof(VideoPreview5));
            RaisePropertyChanged(nameof(VideoPreview6));
        }
    }
}
