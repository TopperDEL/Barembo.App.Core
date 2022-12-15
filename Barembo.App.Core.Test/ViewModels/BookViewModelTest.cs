using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class BookViewModelTest
    {
        BookViewModel _viewModel;
        BookShelfViewModel _bookShelfViewModel;
        Moq.Mock<IBookService> _bookServiceMock;
        Moq.Mock<IEntryService> _entryServiceMock;
        Moq.Mock<IBookShelfService> _bookShelfServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("use this access");
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _bookServiceMock = new Moq.Mock<IBookService>();
            _entryServiceMock = new Moq.Mock<IEntryService>();
            _bookShelfServiceMock = new Moq.Mock<IBookShelfService>();
            _bookShelfViewModel = new BookShelfViewModel(_bookShelfServiceMock.Object, _eventAggregator.Object, _bookServiceMock.Object, _entryServiceMock.Object);
            _viewModel = new BookViewModel(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object);
        }

        [TestMethod]
        public async Task CreateAsync_Loads_TheBook()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();

            await BookViewModel.CreateAsync(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object, bookReference);

            _bookServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task CreateAsync_SetsCover_OfFirstEntry()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();
            Entry entry = new Entry { Id = "First", ThumbnailBase64 = "MyThumb" };
            EntryReference entryref = new EntryReference { EntryId = "First" };
            List<EntryReference> entryRefs = new List<EntryReference>
            {
                entryref
            };

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();
            _entryServiceMock.Setup(s => s.ListEntriesAsync(bookReference)).Returns(Task.FromResult(entryRefs as IEnumerable<EntryReference>)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref)).Returns(Task.FromResult(entry)).Verifiable();

           var bookVm = await BookViewModel.CreateAsync(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object, bookReference);

            Assert.AreEqual(entry.ThumbnailBase64, bookVm.Book.CoverImageBase64);
            _bookServiceMock.Verify();
            _entryServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task CreateAsync_SetsCover_OfFirstEntryWithThumbnail()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();
            Entry entry1 = new Entry { Id = "First"};
            Entry entry2 = new Entry { Id = "Second" };
            Entry entry3 = new Entry { Id = "Third", ThumbnailBase64 = "MyThumbFromThird" };
            EntryReference entryref1 = new EntryReference { EntryId = "First" };
            EntryReference entryref2 = new EntryReference { EntryId = "Second" };
            EntryReference entryref3 = new EntryReference { EntryId = "Third" };
            List<EntryReference> entryRefs = new List<EntryReference>
            {
                entryref1,
                entryref2,
                entryref3
            };

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();
            _entryServiceMock.Setup(s => s.ListEntriesAsync(bookReference)).Returns(Task.FromResult(entryRefs as IEnumerable<EntryReference>)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref1)).Returns(Task.FromResult(entry1)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref2)).Returns(Task.FromResult(entry2)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref3)).Returns(Task.FromResult(entry3)).Verifiable();

            var bookVm = await BookViewModel.CreateAsync(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object, bookReference);

            Assert.AreEqual(entry3.ThumbnailBase64, bookVm.Book.CoverImageBase64);
            _bookServiceMock.Verify();
            _entryServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task CreateAsync_SetsNoCover_IfActionIsNotAllowed()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();
            _entryServiceMock.Setup(s => s.ListEntriesAsync(bookReference)).Throws(new ActionNotAllowedException()).Verifiable();

            var bookVm = await BookViewModel.CreateAsync(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object, bookReference);

            Assert.IsTrue(string.IsNullOrEmpty(bookVm.Book.CoverImageBase64));
            Assert.IsFalse(bookVm.LoadingFailed);
            _bookServiceMock.Verify();
            _entryServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task CreateAsync_SetsNoCover_IfActionIsNotAllowed2()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();
            EntryReference entryref = new EntryReference { EntryId = "First" };
            List<EntryReference> entryRefs = new List<EntryReference>
            {
                entryref
            };

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();
            _entryServiceMock.Setup(s => s.ListEntriesAsync(bookReference)).Returns(Task.FromResult(entryRefs as IEnumerable<EntryReference>)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref)).Throws(new ActionNotAllowedException()).Verifiable();

            var bookVm = await BookViewModel.CreateAsync(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object, bookReference);

            Assert.IsTrue(string.IsNullOrEmpty(bookVm.Book.CoverImageBase64));
            Assert.IsFalse(bookVm.LoadingFailed);
            _bookServiceMock.Verify();
            _entryServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task CreateAsync_SetsEntryCount()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();
            Entry entry1 = new Entry { Id = "First" };
            Entry entry2 = new Entry { Id = "Second" };
            Entry entry3 = new Entry { Id = "Third", ThumbnailBase64 = "MyThumbFromThird" };
            EntryReference entryref1 = new EntryReference { EntryId = "First" };
            EntryReference entryref2 = new EntryReference { EntryId = "Second" };
            EntryReference entryref3 = new EntryReference { EntryId = "Third" };
            List<EntryReference> entryRefs = new List<EntryReference>
            {
                entryref1,
                entryref2,
                entryref3
            };

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();
            _entryServiceMock.Setup(s => s.ListEntriesAsync(bookReference)).Returns(Task.FromResult(entryRefs as IEnumerable<EntryReference>)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref1)).Returns(Task.FromResult(entry1)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref2)).Returns(Task.FromResult(entry2)).Verifiable();
            _entryServiceMock.Setup(s => s.LoadEntryAsync(entryref3)).Returns(Task.FromResult(entry3)).Verifiable();

            var bookVm = await BookViewModel.CreateAsync(_bookServiceMock.Object, _entryServiceMock.Object, _bookShelfViewModel, _eventAggregator.Object, bookReference);

            Assert.AreEqual(3, bookVm.EntryCount);
            _bookServiceMock.Verify();
            _entryServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task InitAsync_DoesNotSetLoadingFailed_IfSuccessfull()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();

            await _viewModel.InitAsync(bookReference);

            Assert.IsFalse(_viewModel.LoadingFailed);

            _bookServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task InitAsync_DoesSetLoadingFailed_IfLoadingFailed()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Throws(new BookNotExistsException()).Verifiable();

            await _viewModel.InitAsync(bookReference);

            Assert.IsTrue(_viewModel.LoadingFailed);

            _bookServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteCreateEntry_Publishes_Message()
        {
            _eventAggregator.Setup(s => s.GetEvent<CreateBookEntryMessage>()).Returns(new CreateBookEntryMessage()).Verifiable();

            _viewModel.CreateEntryCommand.Execute();

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteShareBook_Publishes_Message()
        {
            _eventAggregator.Setup(s => s.GetEvent<ShareBookMessage>()).Returns(new ShareBookMessage()).Verifiable();

            _viewModel.ShareBookCommand.Execute();

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteShowEntries_Publishes_Message()
        {
            _eventAggregator.Setup(s => s.GetEvent<ShowBookEntriesMessage>()).Returns(new ShowBookEntriesMessage()).Verifiable();

            _viewModel.ShowEntriesCommand.Execute();

            _eventAggregator.Verify();
        }
    }
}
