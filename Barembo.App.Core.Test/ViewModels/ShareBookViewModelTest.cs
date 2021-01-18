using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class ShareBookViewModelTest
    {
        ShareBookViewModel _viewModel;
        Moq.Mock<IBookShelfService> _bookShelfServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;

        [TestInitialize]
        public void Init()
        {
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _bookShelfServiceMock = new Moq.Mock<IBookShelfService>();
            _viewModel = new ShareBookViewModel(_bookShelfServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public void ExecuteSaveEntry_Saves_EntryPublishes_Message()
        {
            BookReference bookReference = new BookReference();
            StoreAccess storeAccess = new StoreAccess();
            BookShareReference bookShareReference = new BookShareReference();
            _viewModel.ContributorName = "Tim";
            _viewModel.AccessRights.CanAddEntries = true;

            _bookShelfServiceMock.Setup(s => s.ShareBookAsync(storeAccess, bookReference, _viewModel.ContributorName, _viewModel.AccessRights)).Returns(Task.FromResult(bookShareReference)).Verifiable();

            _viewModel.Init(storeAccess, bookReference);
            _viewModel.SaveBookShareCommand.Execute();

            _bookShelfServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_Publishes_Message()
        {
            BookReference bookReference = new BookReference();
            StoreAccess storeAccess = new StoreAccess();
            BookShareReference bookShareReference = new BookShareReference();
            _viewModel.ContributorName = "Tim";
            _viewModel.AccessRights.CanAddEntries = true;

            _bookShelfServiceMock.Setup(s => s.ShareBookAsync(storeAccess, bookReference, _viewModel.ContributorName, _viewModel.AccessRights)).Returns(Task.FromResult(bookShareReference)).Verifiable();
            _eventAggregator.Setup(s => s.GetEvent<BookShareSavedMessage>()).Returns(new BookShareSavedMessage()).Verifiable();

            _viewModel.Init(storeAccess, bookReference);
            _viewModel.SaveBookShareCommand.Execute();

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_SetsAndClears_SaveInProgress()
        {
            BookReference bookReference = new BookReference();
            StoreAccess storeAccess = new StoreAccess();
            BookShareReference bookShareReference = new BookShareReference();

            _viewModel.ContributorName = "Tim";
            _viewModel.AccessRights.CanAddEntries = true;

            _eventAggregator.Setup(s => s.GetEvent<BookShareSavedMessage>()).Returns(new BookShareSavedMessage()).Verifiable();
            _bookShelfServiceMock.Setup(s => s.ShareBookAsync(storeAccess, bookReference, _viewModel.ContributorName, _viewModel.AccessRights)).Returns(Task.FromResult(bookShareReference)).Verifiable();

            _viewModel.Init(storeAccess, bookReference);
            Assert.IsFalse(_viewModel.SaveInProgress);
            _viewModel.SaveBookShareCommand.Execute();
            Assert.IsFalse(_viewModel.SaveInProgress);

            _eventAggregator.Verify();
            _bookShelfServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_RaisesError_IfEntryCouldNotBeCreated()
        {
            BookReference bookReference = new BookReference();
            StoreAccess storeAccess = new StoreAccess();
            BookShareReference bookShareReference = new BookShareReference();

            _viewModel.ContributorName = "Tim";
            _viewModel.AccessRights.CanAddEntries = true;

            _eventAggregator.Setup(s => s.GetEvent<ErrorMessage>()).Returns(new ErrorMessage()).Verifiable();
            _bookShelfServiceMock.Setup(s => s.ShareBookAsync(storeAccess, bookReference, _viewModel.ContributorName, _viewModel.AccessRights)).Throws(new BookShareCouldNotBeSavedException()).Verifiable();

            _viewModel.Init(storeAccess, bookReference);
            _viewModel.SaveBookShareCommand.Execute();

            _eventAggregator.Verify();
            _bookShelfServiceMock.Verify();
        }

        [TestMethod]
        public void GoBack_Goes_Back()
        {
            _eventAggregator.Setup(s => s.GetEvent<GoBackMessage>()).Returns(new GoBackMessage()).Verifiable();
            _viewModel.GoBackCommand.Execute();

            _eventAggregator.Verify();
        }
    }
}
