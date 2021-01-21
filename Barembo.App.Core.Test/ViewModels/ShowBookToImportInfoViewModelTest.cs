using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
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
    public class ShowBookToImportInfoViewModelTest
    {
        ShowBookToImportInfoViewModel _viewModel;
        Moq.Mock<IEventAggregator> _eventAggregatorMock;
        Moq.Mock<IBookShelfService> _bookShelfService;
        Moq.Mock<IBookShareStoreService> _bookShareStoreService;

        [TestInitialize]
        public void Init()
        {
            _eventAggregatorMock = new Moq.Mock<IEventAggregator>();
            _bookShelfService = new Moq.Mock<IBookShelfService>();
            _bookShareStoreService = new Moq.Mock<IBookShareStoreService>();
            _viewModel = new ShowBookToImportInfoViewModel(_eventAggregatorMock.Object, _bookShelfService.Object, _bookShareStoreService.Object);
        }

        [TestMethod]
        public async Task ImportBookCommand_Imports_Book()
        {
            StoreAccess storeAccess = new StoreAccess();
            BookShareReference bookShareReference = new BookShareReference();
            BookShare bookShare = new BookShare();

            _bookShelfService.Setup(s => s.AddSharedBookToBookShelfAndSaveAsync(storeAccess, bookShareReference)).Returns(Task.FromResult(true)).Verifiable();
            _bookShareStoreService.Setup(s => s.LoadBookShareAsync(bookShareReference)).Returns(Task.FromResult(bookShare)).Verifiable();

            await _viewModel.InitAsync(storeAccess, bookShareReference);

            _eventAggregatorMock.Setup(s => s.GetEvent<BookImportedMessage>()).Returns(new BookImportedMessage()).Verifiable();
            _viewModel.ImportBookCommand.Execute();

            _eventAggregatorMock.Verify();
            _bookShareStoreService.Verify();
        }

        [TestMethod]
        public async Task ImportBookCommand_RaisesError_OnImportFail()
        {
            StoreAccess storeAccess = new StoreAccess();
            BookShareReference bookShareReference = new BookShareReference();
            BookShare bookShare = new BookShare();

            _bookShelfService.Setup(s => s.AddSharedBookToBookShelfAndSaveAsync(storeAccess, bookShareReference)).Returns(Task.FromResult(false)).Verifiable();
            _bookShareStoreService.Setup(s => s.LoadBookShareAsync(bookShareReference)).Returns(Task.FromResult(bookShare)).Verifiable();

            await _viewModel.InitAsync(storeAccess, bookShareReference);

            _eventAggregatorMock.Setup(s => s.GetEvent<ErrorMessage>()).Returns(new ErrorMessage()).Verifiable();
            _viewModel.ImportBookCommand.Execute();

            _eventAggregatorMock.Verify();
            _bookShareStoreService.Verify();
        }

        [TestMethod]
        public void GoBack_Goes_Back()
        {
            _eventAggregatorMock.Setup(s => s.GetEvent<GoBackMessage>()).Returns(new GoBackMessage()).Verifiable();
            _viewModel.GoBackCommand.Execute();

            _eventAggregatorMock.Verify();
        }
    }
}
