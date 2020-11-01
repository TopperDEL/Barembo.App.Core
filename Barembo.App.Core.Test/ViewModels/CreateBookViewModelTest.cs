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
    public class CreateBookViewModelTest
    {
        CreateBookViewModel _viewModel;
        Moq.Mock<IBookService> _bookServiceMock;
        Moq.Mock<IBookShelfService> _bookShelfServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        StoreAccess _storeAccess;
        BookShelf _bookShelf;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("use this access");
            _bookShelf = new BookShelf { OwnerName = "Mine" };
            _bookServiceMock = new Moq.Mock<IBookService>();
            _bookShelfServiceMock = new Moq.Mock<IBookShelfService>();
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _viewModel = new CreateBookViewModel(_bookServiceMock.Object, _bookShelfServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public void Init_SetsStoreAccess_OnViewModel()
        {
            _viewModel.Init(_storeAccess, _bookShelf);

            Assert.AreEqual(_viewModel._storeAccess, _storeAccess);
            Assert.AreEqual(_viewModel._bookShelf, _bookShelf);

            _bookServiceMock.Verify();
            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void Create_CanOnlyExecute_IfBookNameIsNotNullOrEmpty()
        {
            _viewModel.BookName = null;

            Assert.IsFalse(_viewModel.CreateBookCommand.CanExecute());

            _viewModel.BookName = "";

            Assert.IsFalse(_viewModel.CreateBookCommand.CanExecute());
        }

        [TestMethod]
        public void Create_Creates_NewBook()
        {
            Book book = new Book();
            book.Name = "Tim's Book";
            book.Description = "Description";
            Contributor contributor = new Contributor();
            contributor.Name = "contributor name";

            _viewModel.BookName = book.Name;
            _viewModel.BookDescription = book.Description;

            _bookServiceMock.Setup(s => s.CreateBook(_viewModel.BookName, _viewModel.BookDescription)).Returns(book).Verifiable();
            _bookShelfServiceMock.Setup(s => s.AddOwnBookToBookShelfAndSaveAsync(_storeAccess, book, Moq.It.Is<Contributor>(c => c.Name == _bookShelf.OwnerName))).Returns(Task.FromResult(true)).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<BookCreatedMessage>().Publish(_storeAccess)).Verifiable();

            _viewModel.Init(_storeAccess, _bookShelf);
            _viewModel.CreateBookCommand.Execute();

            _bookServiceMock.Verify();
            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void Create_Creates_PublishesCreatedMessage()
        {
            //Book Book = new Book();
            //Book.Name = "Tim";

            //_viewModel.BookName = Book.Name;

            //_bookServiceMock.Setup(s => s.CreateAndSaveBookAsync(_storeAccess, _viewModel.BookName)).Returns(Task.FromResult(Book)).Verifiable();
            //_eventAggregator.Setup(s => s.GetEvent<BookCreatedMessage>().Publish(_storeAccess)).Verifiable();

            //_viewModel.Init(_storeAccess, _bookShelf);
            //_viewModel.CreateBookCommand.Execute();

            //_bookServiceMock.Verify();
            //_bookShelfServiceMock.Verify();
            //_eventAggregator.Verify();
        }

        [TestMethod]
        public void Create_SetsError_IfCreationFailed()
        {
            //Book Book = new Book();
            //Book.Name = "Tim";

            //_viewModel.BookName = Book.Name;

            //_bookServiceMock.Setup(s => s.CreateAndSaveBookAsync(_storeAccess, _viewModel.BookName)).Throws(new BookCouldNotBeSavedException()).Verifiable();

            //_viewModel.Init(_storeAccess, _bookShelf);

            //_viewModel.CreateBookCommand.Execute();

            //Assert.IsTrue(_viewModel.CreationFailed);

            //_bookShelfServiceMock.Verify();
            //_bookServiceMock.Verify();
            //_eventAggregator.Verify();
        }
    }
}
