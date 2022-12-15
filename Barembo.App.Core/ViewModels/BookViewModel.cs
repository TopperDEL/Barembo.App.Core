using Barembo.App.Core.Messages;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public class BookViewModel : AsyncLoadingBindableBase
    {
        readonly IBookService _bookService;
        readonly IEntryService _entryService;
        readonly IEventAggregator _eventAggregator;
        BookShelfViewModel _bookShelfViewModel;

        private BookReference _bookReference;
        public BookReference BookReference
        {
            get { return _bookReference; }
            set { SetProperty(ref _bookReference, value); }
        }
        
        private Book book;
        public Book Book
        {
            get { return book; }
            set { SetProperty(ref book, value); }
        }

        public byte[] Thumbnail
        {
            get
            {
                if (book == null)
                {
                    return null;
                }
                else if (string.IsNullOrEmpty(book.CoverImageBase64))
                    return null;

                return Convert.FromBase64String(book.CoverImageBase64);
            }
        }

        public bool HasThumbnail { get; set; } = false;

        public int EntryCount { get; set; } = 0;

        private DelegateCommand _createEntryCommand;
        public DelegateCommand CreateEntryCommand =>
            _createEntryCommand ?? (_createEntryCommand = new DelegateCommand(ExecuteCreateEntryCommand));

        void ExecuteCreateEntryCommand()
        {
            _eventAggregator.GetEvent<CreateBookEntryMessage>().Publish(new Tuple<BookReference, BookShelfViewModel>(BookReference, _bookShelfViewModel));
        }

        private DelegateCommand _shareBookCommand;
        public DelegateCommand ShareBookCommand =>
            _shareBookCommand ?? (_shareBookCommand = new DelegateCommand(ExecuteShareBookCommand));

        void ExecuteShareBookCommand()
        {
            _eventAggregator.GetEvent<ShareBookMessage>().Publish(BookReference);
        }

        private DelegateCommand _showEntriesCommand;
        public DelegateCommand ShowEntriesCommand =>
            _showEntriesCommand ?? (_showEntriesCommand = new DelegateCommand(ExecuteShowEntriesCommand));

        void ExecuteShowEntriesCommand()
        {
            _eventAggregator.GetEvent<ShowBookEntriesMessage>().Publish(BookReference);
        }

        internal BookViewModel(IBookService bookService, IEntryService entryService, BookShelfViewModel bookShelfViewModel, IEventAggregator eventAggregator)
        {
            _bookService = bookService;
            _entryService = entryService;
            _bookShelfViewModel = bookShelfViewModel;
            _eventAggregator = eventAggregator;
        }

        public static async Task<BookViewModel> CreateAsync(IBookService bookService, IEntryService entryService, BookShelfViewModel bookShelfViewModel, IEventAggregator eventAggregator, BookReference bookReference)
        {
            var bookVM = new BookViewModel(bookService, entryService, bookShelfViewModel, eventAggregator);
            await bookVM.InitAsync(bookReference).ConfigureAwait(false);

            return bookVM;
        }

        public async Task InitAsync(BookReference bookReference)
        {
            IsLoading = true;

            try
            {
                BookReference = bookReference;
                Book = await _bookService.LoadBookAsync(BookReference);
                if(string.IsNullOrEmpty(Book.CoverImageBase64))
                {
                    try
                    {
                        var entryReferences = await _entryService.ListEntriesAsync(bookReference);
                        EntryCount = entryReferences.Count();
                        RaisePropertyChanged(nameof(EntryCount));

                        foreach (var entryReference in entryReferences)
                        {
                            var entry = await _entryService.LoadEntryAsync(entryReference);
                            if (!string.IsNullOrEmpty(entry.ThumbnailBase64))
                            {
                                Book.CoverImageBase64 = entry.ThumbnailBase64;
                                RaisePropertyChanged(nameof(Thumbnail));
                                RaisePropertyChanged(nameof(HasThumbnail));
                                break;
                            }
                        }
                    }
                    catch(ActionNotAllowedException)
                    {
                        //Ignore - then we have no Entries to get a thumbnail from.
                    }
                }
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
