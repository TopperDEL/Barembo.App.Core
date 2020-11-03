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
        Moq.Mock<IBookService> _bookServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("use this access");
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _bookServiceMock = new Moq.Mock<IBookService>();
            _viewModel = new BookViewModel(_bookServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public async Task CreateAsync_Loads_TheBook()
        {
            BookReference bookReference = new BookReference();
            Book book = new Book();

            _bookServiceMock.Setup(s => s.LoadBookAsync(bookReference)).Returns(Task.FromResult(book)).Verifiable();

            await BookViewModel.CreateAsync(_bookServiceMock.Object, _eventAggregator.Object, bookReference);

            _bookServiceMock.Verify();
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
