using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public class ShowBookShareViewModel : BindableBase
    {
        readonly IEventAggregator _eventAggregator;
        readonly IQRCodeGeneratorService _qrCodeGeneratorService;
        readonly IMagicLinkGeneratorService _magicLinkGeneratorService;

        private string _magicLink;
        public string MagicLink
        {
            get { return _magicLink; }
            set { SetProperty(ref _magicLink, value); }
        }

        private byte[] _qrCodePNG;
        public byte[] QRCodePNG
        {
            get { return _qrCodePNG; }
            set { SetProperty(ref _qrCodePNG, value); }
        }

        private DelegateCommand _writeMagicLinkToClipboardCommand;
        public DelegateCommand WriteMagicLinkToClipboardCommand =>
            _writeMagicLinkToClipboardCommand ?? (_writeMagicLinkToClipboardCommand = new DelegateCommand(ExecuteWriteToClipboardCommand));

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        void ExecuteWriteToClipboardCommand()
        {
            _eventAggregator.GetEvent<WriteToClipboardMessage>().Publish(MagicLink);
        }

        public ShowBookShareViewModel(IEventAggregator eventAggregator, IQRCodeGeneratorService qrCodeGeneratorService, IMagicLinkGeneratorService magicLinkGeneratorService)
        {
            _eventAggregator = eventAggregator;
            _qrCodeGeneratorService = qrCodeGeneratorService;
            _magicLinkGeneratorService = magicLinkGeneratorService;
        }

        public void Init(BookShareReference bookShareReference)
        {
            QRCodePNG = _qrCodeGeneratorService.GetQRCodePNGFor(bookShareReference);
            MagicLink = _magicLinkGeneratorService.GetMagicLinkFor(bookShareReference);
        }
    }
}
