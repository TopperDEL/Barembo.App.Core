using Barembo.App.Core.Interfaces;
using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public class ImportBookViewModel : BindableBase
    {
        readonly IEventAggregator _eventAggregator;
        readonly IQRCodeScannerService _qrCodeScannerService;
        readonly IMagicLinkResolverService _magicLinkResolver;

        private bool _scanInProgress;
        public bool ScanInProgress
        {
            get { return _scanInProgress; }
            set { SetProperty(ref _scanInProgress, value); }
        }

        private string _magicLink;
        public string MagicLink
        {
            get { return _magicLink; }
            set
            {
                SetProperty(ref _magicLink, value);
                ImportMagicLinkCommand.RaiseCanExecuteChanged();
            }
        }

        private DelegateCommand _scanQRCodeCommand;
        public DelegateCommand ScanQRCodeCommand =>
            _scanQRCodeCommand ?? (_scanQRCodeCommand = new DelegateCommand(async () => await ExecuteScanQRCodeCommand().ConfigureAwait(false), CanExecuteScanQRCodeCommand));

        private DelegateCommand _importMagicLinkCommand;
        public DelegateCommand ImportMagicLinkCommand =>
            _importMagicLinkCommand ?? (_importMagicLinkCommand = new DelegateCommand(ExecuteImportMagicLinkCommand, CanExecuteImportMagicLinkCommand));

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        async Task ExecuteScanQRCodeCommand()
        {
            ScanInProgress = true;
            try
            {
                MagicLink = await _qrCodeScannerService.ScanQRCodeAsync();
                if (string.IsNullOrEmpty(MagicLink))
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.NoBarcodeScanned, ""));
                    return;
                }

                try
                {
                    var bookShareReference = _magicLinkResolver.GetBookShareReferenceFrom(MagicLink);
                    _eventAggregator.GetEvent<BookToImportMessage>().Publish(bookShareReference);
                }
                catch
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.NoBookShareReferenceMagicLink, ""));
                    return;
                }
            }
            finally
            {
                ScanInProgress = false;
            }
        }

        bool CanExecuteScanQRCodeCommand()
        {
            return !ScanInProgress;
        }

        void ExecuteImportMagicLinkCommand()
        {
            ScanInProgress = true;
            try
            {
                try
                {
                    var bookShareReference = _magicLinkResolver.GetBookShareReferenceFrom(MagicLink);
                    _eventAggregator.GetEvent<BookToImportMessage>().Publish(bookShareReference);
                }
                catch
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.NoBookShareReferenceMagicLink, ""));
                    return;
                }
            }
            finally
            {
                ScanInProgress = false;
            }
        }

        bool CanExecuteImportMagicLinkCommand()
        {
            return !ScanInProgress && !string.IsNullOrEmpty(MagicLink);
        }

        public ImportBookViewModel(IEventAggregator eventAggregator, IQRCodeScannerService qrCodeScannerService, IMagicLinkResolverService magicLinkResolver)
        {
            _eventAggregator = eventAggregator;
            _qrCodeScannerService = qrCodeScannerService;
            _magicLinkResolver = magicLinkResolver;
        }
    }
}
