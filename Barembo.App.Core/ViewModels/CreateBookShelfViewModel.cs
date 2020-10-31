using Barembo.App.Core.Messages;
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
    public class CreateBookShelfViewModel : BindableBase
    {
        readonly IBookShelfService _bookShelfService;
        readonly IEventAggregator _eventAggregator;
        internal StoreAccess _storeAccess;

        private string _ownerName;
        public string OwnerName
        {
            get { return _ownerName; }
            set { SetProperty(ref _ownerName, value); }
        }

        private bool _creationFailed;
        public bool CreationFailed
        {
            get { return _creationFailed; }
            set { SetProperty(ref _creationFailed, value); }
        }

        private DelegateCommand _createBookShelfCommand;
        public DelegateCommand CreateBookShelfCommand =>
            _createBookShelfCommand ?? (_createBookShelfCommand = new DelegateCommand(async () => await ExecuteCreateBookShelfCommand(), CanExecuteCreateBookShelfCommand));

        async Task ExecuteCreateBookShelfCommand()
        {
            CreationFailed = false;

            try
            {
                await _bookShelfService.CreateAndSaveBookShelfAsync(_storeAccess, OwnerName);

                _eventAggregator.GetEvent<BookShelfCreatedMessage>().Publish(_storeAccess);
            }
            catch(Barembo.Exceptions.BookShelfCouldNotBeSavedException)
            {
                CreationFailed = true;
            }
        }

        bool CanExecuteCreateBookShelfCommand()
        {
            return !string.IsNullOrEmpty(OwnerName);
        }

        public CreateBookShelfViewModel(IBookShelfService bookShelfService, IEventAggregator eventAggregator)
        {
            _bookShelfService = bookShelfService;
            _eventAggregator = eventAggregator;
        }

        public void Init(StoreAccess storeAccess)
        {
            _storeAccess = storeAccess;
        }
    }
}
