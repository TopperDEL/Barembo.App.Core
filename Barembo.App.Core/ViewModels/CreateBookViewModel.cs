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
    public class CreateBookViewModel : BindableBase
    {
        readonly IBookService _bookService;
        readonly IBookShelfService _bookShelfService;
        readonly IEventAggregator _eventAggregator;
        internal StoreAccess _storeAccess;
        internal BookShelf _bookShelf;

        private string _bookName;
        public string BookName
        {
            get { return _bookName; }
            set
            {
                SetProperty(ref _bookName, value);
                CreateBookCommand.RaiseCanExecuteChanged();
            }
        }

        private string _bookDescription;
        public string BookDescription
        {
            get { return _bookDescription; }
            set
            {
                SetProperty(ref _bookDescription, value);
                CreateBookCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _creationFailed;
        public bool CreationFailed
        {
            get { return _creationFailed; }
            set { SetProperty(ref _creationFailed, value); }
        }

        private DelegateCommand _createBookCommand;
        public DelegateCommand CreateBookCommand =>
            _createBookCommand ?? (_createBookCommand = new DelegateCommand(async () => await ExecuteCreateBookCommand().ConfigureAwait(false), CanExecuteCreateBookCommand));

        async Task ExecuteCreateBookCommand()
        {
            CreationFailed = false;

            var book = _bookService.CreateBook(BookName, BookDescription);
            var contributor = new Contributor { Name = _bookShelf.OwnerName };

            var added = await _bookShelfService.AddOwnBookToBookShelfAndSaveAsync(_storeAccess, book, contributor);

            if (added)
            {
                _eventAggregator.GetEvent<BookCreatedMessage>().Publish(_storeAccess);
            }
            else
            {
                CreationFailed = true;
            }
        }

        bool CanExecuteCreateBookCommand()
        {
            return !string.IsNullOrEmpty(BookName);
        }

        public CreateBookViewModel(IBookService bookService, IBookShelfService bookShelfService, IEventAggregator eventAggregator)
        {
            _bookService = bookService;
            _bookShelfService = bookShelfService;
            _eventAggregator = eventAggregator;
        }

        public void Init(StoreAccess storeAccess, BookShelf bookShelf)
        {
            _storeAccess = storeAccess;
            _bookShelf = bookShelf;
        }
    }
}
