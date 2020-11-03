using Barembo.Interfaces;
using Barembo.Models;
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

        private Book book;
        public Book Book
        {
            get { return book; }
            set { SetProperty(ref book, value); }
        }

        private BookViewModel(IBookService bookService, IEventAggregator eventAggregator)
        {
            _bookService = bookService;
            _eventAggregator = eventAggregator;
        }

        public static async Task<BookViewModel> CreateAsync(IBookService bookService, IEventAggregator eventAggregator, BookReference bookReference)
        {
            var bookVM = new BookViewModel(bookService, eventAggregator);
            await bookVM.InitAsync(bookReference);

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
