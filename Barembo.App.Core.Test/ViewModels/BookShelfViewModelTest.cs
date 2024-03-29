﻿using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class BookShelfViewModelTest
    {
        BookShelfViewModel _viewModel;
        Moq.Mock<IBookShelfService> _bookShelfServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        Moq.Mock<IBookService> _bookServiceMock;
        Moq.Mock<IEntryService> _entryServiceMock;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("use this access");
            _bookShelfServiceMock = new Moq.Mock<IBookShelfService>();
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _bookServiceMock = new Moq.Mock<IBookService>();
            _entryServiceMock = new Moq.Mock<IEntryService>();
            _viewModel = new BookShelfViewModel(_bookShelfServiceMock.Object, _eventAggregator.Object, _bookServiceMock.Object, _entryServiceMock.Object);
        }

        [TestMethod]
        public async Task Init_RaisesNoBookShelfExistsMessage_IfNoBookShelfExists()
        {
            NoBookShelfExistsMessage msg = new NoBookShelfExistsMessage();

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Throws(new NoBookShelfExistsException("","","")).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<NoBookShelfExistsMessage>()).Returns(msg).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_SetsNoBookShelfExists_IfNoBookShelfExists()
        {
            NoBookShelfExistsMessage msg = new NoBookShelfExistsMessage();

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Throws(new NoBookShelfExistsException("", "", "")).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<NoBookShelfExistsMessage>()).Returns(msg).Verifiable();

            Assert.IsFalse(_viewModel.NoBookShelfExists);
            
            await _viewModel.InitAsync(_storeAccess);

            Assert.IsTrue(_viewModel.NoBookShelfExists);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_AddsBookViewModel_ForEachBookReference()
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.Content.Add(new BookReference());

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            Assert.AreEqual(_viewModel.Books.Count, bookShelf.Content.Count);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_SetsNoBooksToFalse_IfBooksExist()
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.Content.Add(new BookReference());

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            Assert.AreEqual(_viewModel.Books.Count, bookShelf.Content.Count);
            Assert.IsFalse(_viewModel.NoBooks);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_SetsHasBooksToTrue_IfBooksExist()
        {
            BookShelf bookShelf = new BookShelf();
            bookShelf.Content.Add(new BookReference());

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            Assert.AreEqual(_viewModel.Books.Count, bookShelf.Content.Count);
            Assert.IsTrue(_viewModel.HasBooks);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_SetsNoBooksToTrue_IfNoBooksExists()
        {
            BookShelf bookShelf = new BookShelf();

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            Assert.AreEqual(_viewModel.Books.Count, bookShelf.Content.Count);
            Assert.IsTrue(_viewModel.NoBooks);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_SetsBookShelf_OnViewModel()
        {
            BookShelf bookShelf = new BookShelf();

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            Assert.AreEqual(_viewModel.BookShelf, bookShelf);
            Assert.IsFalse(_viewModel.NoBookShelfExists);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public async Task Init_SetsStoreAccess_OnViewModel()
        {
            BookShelf bookShelf = new BookShelf();

            _bookShelfServiceMock.Setup(s => s.LoadBookShelfAsync(_storeAccess)).Returns(Task.FromResult(bookShelf)).Verifiable();

            await _viewModel.InitAsync(_storeAccess);

            Assert.AreEqual(_viewModel._storeAccess, _storeAccess);

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void AddOwnBookCommand_Raises_AddOwnBookMessage()
        {
            AddOwnBookMessage msg = new AddOwnBookMessage();
            _eventAggregator.Setup(s => s.GetEvent<AddOwnBookMessage>()).Returns(msg).Verifiable();

            _viewModel.AddOwnBookCommand.Execute();

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void AddForeignBookCommand_Raises_AddForeignBookMessage()
        {
            AddForeignBookMessage msg = new AddForeignBookMessage();
            _eventAggregator.Setup(s => s.GetEvent<AddForeignBookMessage>()).Returns(msg).Verifiable();

            _viewModel.AddForeignBookCommand.Execute();

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void CreateBookShelfCommand_Raises_NoBookShelfExistsMessage()
        {
            NoBookShelfExistsMessage msg = new NoBookShelfExistsMessage();
            _eventAggregator.Setup(s => s.GetEvent<NoBookShelfExistsMessage>()).Returns(msg).Verifiable();

            _viewModel.CreateBookShelfCommand.Execute();

            _bookShelfServiceMock.Verify();
            _eventAggregator.Verify();
        }
    }
}
