using Barembo.App.Core.Messages;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public class ShowBookToImportInfoViewModel : BindableBase
    {
        readonly IBookShelfService _bookShelfService;
        readonly IEventAggregator _eventAggregator;
        readonly IBookShareStoreService _bookShareStoreService;
        StoreAccess _storeAccess;
        BookShareReference _bookShareReference;

        private bool _saveInProgress;
        public bool SaveInProgress
        {
            get { return _saveInProgress; }
            set { SetProperty(ref _saveInProgress, value); }
        }

        private BookShare _bookShare;
        public BookShare BookShare
        {
            get { return _bookShare; }
            set { SetProperty(ref _bookShare, value); }
        }

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        private DelegateCommand _importBookCommand;
        public DelegateCommand ImportBookCommand =>
            _importBookCommand ?? (_importBookCommand = new DelegateCommand(async () => await ExecuteImportBookCommand().ConfigureAwait(false), CanExecuteImportBookCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        async Task ExecuteImportBookCommand()
        {
            SaveInProgress = true;
            try
            {
                var imported = await _bookShelfService.AddSharedBookToBookShelfAndSaveAsync(_storeAccess, _bookShareReference);

                if (imported)
                {
                    _eventAggregator.GetEvent<BookImportedMessage>().Publish();
                }
                else
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.CouldNotImportBook, ""));
                }
            }
            finally
            {
                SaveInProgress = false;
            }
        }

        bool CanExecuteImportBookCommand()
        {
            return !SaveInProgress;
        }

        public ShowBookToImportInfoViewModel(IEventAggregator eventAggregator, IBookShelfService bookShelfService, IBookShareStoreService bookShareStoreService)
        {
            _eventAggregator = eventAggregator;
            _bookShelfService = bookShelfService;
            _bookShareStoreService = bookShareStoreService;
        }

        public async Task InitAsync(StoreAccess storeAccess, BookShareReference bookShareReference)
        {
            _storeAccess = storeAccess;
            _bookShareReference = bookShareReference;

            BookShare = await _bookShareStoreService.LoadBookShareAsync(_bookShareReference);
        }
    }
}
