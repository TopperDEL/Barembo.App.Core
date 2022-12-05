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
    public class BookViewModel : AsyncLoadingBindableBase
    {
        readonly IBookService _bookService;
        readonly IEventAggregator _eventAggregator;
        BookReference _bookReference;
        BookShelfViewModel _bookShelfViewModel;

        private Book book;
        public Book Book
        {
            get { return book; }
            set { SetProperty(ref book, value); }
        }

        private DelegateCommand _createEntryCommand;
        public DelegateCommand CreateEntryCommand =>
            _createEntryCommand ?? (_createEntryCommand = new DelegateCommand(ExecuteCreateEntryCommand));

        void ExecuteCreateEntryCommand()
        {
            _eventAggregator.GetEvent<CreateBookEntryMessage>().Publish(new Tuple<BookReference, BookShelfViewModel>(_bookReference, _bookShelfViewModel));
        }

        private DelegateCommand _shareBookCommand;
        public DelegateCommand ShareBookCommand =>
            _shareBookCommand ?? (_shareBookCommand = new DelegateCommand(ExecuteShareBookCommand));

        void ExecuteShareBookCommand()
        {
            _eventAggregator.GetEvent<ShareBookMessage>().Publish(_bookReference);
        }

        private DelegateCommand _showEntriesCommand;
        public DelegateCommand ShowEntriesCommand =>
            _showEntriesCommand ?? (_showEntriesCommand = new DelegateCommand(ExecuteShowEntriesCommand));

        void ExecuteShowEntriesCommand()
        {
            _eventAggregator.GetEvent<ShowBookEntriesMessage>().Publish(_bookReference);
        }

        internal BookViewModel(IBookService bookService, BookShelfViewModel bookShelfViewModel, IEventAggregator eventAggregator)
        {
            _bookService = bookService;
            _bookShelfViewModel = bookShelfViewModel;
            _eventAggregator = eventAggregator;
        }

        public static async Task<BookViewModel> CreateAsync(IBookService bookService, BookShelfViewModel bookShelfViewModel, IEventAggregator eventAggregator, BookReference bookReference)
        {
            var bookVM = new BookViewModel(bookService, bookShelfViewModel, eventAggregator);
            await bookVM.InitAsync(bookReference).ConfigureAwait(false);

            return bookVM;
        }

        public async Task InitAsync(BookReference bookReference)
        {
            IsLoading = true;

            try
            {
                _bookReference = bookReference;
                Book = await _bookService.LoadBookAsync(_bookReference);
            }
            catch(Exception ex)
            {
                LoadingFailed = true;
                LoadingError = ex.Message;
            }

            IsLoading = false;
        }
    }
}
