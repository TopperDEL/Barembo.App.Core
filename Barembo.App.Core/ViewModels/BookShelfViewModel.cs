using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public class BookShelfViewModel : BindableBase
    {
        readonly IBookShelfService _bookShelfService;

        private ObservableCollection<BookViewModel> books;
        public ObservableCollection<BookViewModel> Books
        {
            get { return books; }
            set { SetProperty(ref books, value); }
        }

        public BookShelfViewModel(IBookShelfService bookShelfService)
        {
            Books = new ObservableCollection<BookViewModel>();

            _bookShelfService = bookShelfService;
        }

        public async Task InitAsync(StoreAccess storeAccess)
        {
            BookShelf bookShelf;
            try
            {
                bookShelf = await _bookShelfService.LoadBookShelfAsync(storeAccess);
            }
            catch(NoBookShelfExistsException)
            {
                //ToDo: Raise event for Navigation to "create Bookshelf" view
            }
        }
    }
}
