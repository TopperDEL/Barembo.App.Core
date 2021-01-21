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
    public class ShareBookViewModel : BindableBase
    {
        readonly IBookShelfService _bookShelfService;
        readonly IEventAggregator _eventAggregator;

        private StoreAccess _storeAccess;
        private BookReference _bookReference;

        private string _contributorName;
        public string ContributorName
        {
            get { return _contributorName; }
            set { SetProperty(ref _contributorName, value); }
        }

        private string _bookName;
        public string BookName
        {
            get { return _bookName; }
            set { SetProperty(ref _bookName, value); }
        }

        private AccessRights _accessRights;
        public AccessRights AccessRights
        {
            get { return _accessRights; }
            set { SetProperty(ref _accessRights, value); }
        }

        private bool _saveInProgress;
        public bool SaveInProgress
        {
            get { return _saveInProgress; }
            set { SetProperty(ref _saveInProgress, value); }
        }

        private DelegateCommand _saveBookShareCommand;
        public DelegateCommand SaveBookShareCommand =>
            _saveBookShareCommand ?? (_saveBookShareCommand = new DelegateCommand(async () => await ExecuteSaveBookShareCommand().ConfigureAwait(false), CanExecuteSaveBookShareCommand));

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        async Task ExecuteSaveBookShareCommand()
        {
            SaveInProgress = true;
            try
            {
                try
                {
                    var bookShareReference = await _bookShelfService.ShareBookAsync(_storeAccess, _bookReference, _contributorName, _accessRights, BookName);

                    _eventAggregator.GetEvent<BookShareSavedMessage>().Publish(bookShareReference);
                }
                catch (BookShareCouldNotBeSavedException ex)
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.BookShareCouldNotBeSavedException, ex.Message));
                }
            }
            finally
            {
                SaveInProgress = false;
            }
        }

        bool CanExecuteSaveBookShareCommand()
        {
            return !SaveInProgress;
        }

        public ShareBookViewModel(IBookShelfService bookShelfService, IEventAggregator eventAggregator)
        {
            _bookShelfService = bookShelfService;
            _eventAggregator = eventAggregator;

            _accessRights = new AccessRights();
        }

        public void Init(StoreAccess storeAccess, BookReference bookReference)
        {
            _storeAccess = storeAccess;
            _bookReference = bookReference;
        }
    }
}
