﻿using Barembo.App.Core.Messages;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
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
        readonly IBookService _bookService;
        readonly IEntryService _entryService;
        readonly IEventAggregator _eventAggregator;
        internal StoreAccess _storeAccess;

        private ObservableCollection<BookViewModel> _books;
        public ObservableCollection<BookViewModel> Books
        {
            get { return _books; }
            set { SetProperty(ref _books, value); }
        }

        private BookShelf _bookShelf;
        public BookShelf BookShelf
        {
            get { return _bookShelf; }
            set { SetProperty(ref _bookShelf, value); }
        }

        private DelegateCommand _addOwnBookCommand;
        public DelegateCommand AddOwnBookCommand =>
            _addOwnBookCommand ?? (_addOwnBookCommand = new DelegateCommand(ExecuteAddOwnBookCommand));

        void ExecuteAddOwnBookCommand()
        {
            _eventAggregator.GetEvent<AddOwnBookMessage>().Publish(new Tuple<StoreAccess, BookShelf>(_storeAccess, _bookShelf));
        }

        private DelegateCommand _addForeignBookCommand;
        public DelegateCommand AddForeignBookCommand =>
            _addForeignBookCommand ?? (_addForeignBookCommand = new DelegateCommand(ExecuteAddForeignBookCommand));

        void ExecuteAddForeignBookCommand()
        {
            _eventAggregator.GetEvent<AddForeignBookMessage>().Publish(new Tuple<StoreAccess, BookShelf>(_storeAccess, _bookShelf));
        }

        public BookShelfViewModel(IBookShelfService bookShelfService, IEventAggregator eventAggregator, IBookService bookService, IEntryService entryService)
        {
            _bookShelfService = bookShelfService;
            _eventAggregator = eventAggregator;
            _bookService = bookService;
            _entryService = entryService;

            Books = new ObservableCollection<BookViewModel>();
        }

        public async Task InitAsync(StoreAccess storeAccess)
        {
            int tryCount = 0;
            Books.Clear();
            _storeAccess = storeAccess;
            try
            {
                try
                {
                    tryCount++;
                    BookShelf = await _bookShelfService.LoadBookShelfAsync(storeAccess);//Crashes as BookShelf is created on the UI-thread .ConfigureAwait(false);
                }catch(Exception ex)
                {
                    tryCount++;
                    await Task.Delay(1000).ConfigureAwait(false); //ToDo: Check if this works in production - it might use another thread and crash
                    BookShelf = await _bookShelfService.LoadBookShelfAsync(storeAccess);//Crashes as BookShelf is created on the UI-thread .ConfigureAwait(false);
                }

                foreach (var bookReference in BookShelf.Content)
                {
                    var bookVM = await BookViewModel.CreateAsync(_bookService, _entryService, this, _eventAggregator, bookReference);
                    Books.Add(bookVM);
                }
            }
            catch (NoBookShelfExistsException ex)
            {
                _eventAggregator.GetEvent<NoBookShelfExistsMessage>().Publish(new Tuple<StoreAccess, string>(storeAccess, ex.Message + " - " + ex.AccessGrant + " - " + ex.AdditionalError + " - " + ex.StoreKey + " - " + tryCount.ToString() + " - " + ex.StackTrace));
            }
        }
    }
}
