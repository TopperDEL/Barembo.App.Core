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
    public class CreateBookShelfViewModelTest
    {
        CreateBookShelfViewModel _viewModel;
        Moq.Mock<IBookShelfService> _bookShelfServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("use this access");
            _bookShelfServiceMock = new Moq.Mock<IBookShelfService>();
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _viewModel = new CreateBookShelfViewModel(_bookShelfServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public void Init_SetsStoreAccess_OnViewModel()
        {
            _viewModel.Init(_storeAccess);

            Assert.AreEqual(_viewModel._storeAccess, _storeAccess);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void Create_CanOnlyExecute_IfOwnerNameIsNotNullOrEmpty()
        {
            _viewModel.OwnerName = null;

            Assert.IsFalse(_viewModel.CreateBookShelfCommand.CanExecute());

            _viewModel.OwnerName = "";

            Assert.IsFalse(_viewModel.CreateBookShelfCommand.CanExecute());
        }

        [TestMethod]
        public void Create_Creates_NewBookShelf()
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "Tim";

            _viewModel.OwnerName = bookShelf.OwnerName;

            _bookShelfServiceMock.Setup(s => s.CreateAndSaveBookShelfAsync(_storeAccess, _viewModel.OwnerName)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<BookShelfCreatedMessage>().Publish(_storeAccess)).Verifiable();

            _viewModel.Init(_storeAccess);
            _viewModel.CreateBookShelfCommand.Execute();

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void Create_Creates_PublishesCreatedMessage()
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "Tim";

            _viewModel.OwnerName = bookShelf.OwnerName;

            _bookShelfServiceMock.Setup(s => s.CreateAndSaveBookShelfAsync(_storeAccess, _viewModel.OwnerName)).Returns(Task.FromResult(bookShelf)).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<BookShelfCreatedMessage>().Publish(_storeAccess)).Verifiable();

            _viewModel.Init(_storeAccess);
            _viewModel.CreateBookShelfCommand.Execute();

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void Create_SetsError_IfCreationFailed()
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.OwnerName = "Tim";

            _viewModel.OwnerName = bookShelf.OwnerName;

            _bookShelfServiceMock.Setup(s => s.CreateAndSaveBookShelfAsync(_storeAccess, _viewModel.OwnerName)).Throws(new BookShelfCouldNotBeSavedException()).Verifiable();

            _viewModel.Init(_storeAccess);

            _viewModel.CreateBookShelfCommand.Execute();

            Assert.IsTrue(_viewModel.CreationFailed);

            _bookShelfServiceMock.Verify();
        }
    }
}
