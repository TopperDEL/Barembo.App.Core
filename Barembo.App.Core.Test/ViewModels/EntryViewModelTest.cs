using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Barembo.Interfaces;
using Barembo.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class EntryViewModelTest
    {
        EntryViewModel _viewModel;
        Moq.Mock<IEntryService> _entryServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        EntryReference _entryReference;
        Moq.Mock<System.Threading.SynchronizationContext> _syncConMock;

        [TestInitialize]
        public void Init()
        {
            _entryReference = new EntryReference();
            _entryServiceMock = new Moq.Mock<IEntryService>();
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _syncConMock = new Moq.Mock<System.Threading.SynchronizationContext>();
            _viewModel = new EntryViewModel(_entryReference, _entryServiceMock.Object, _eventAggregator.Object, _syncConMock.Object);
        }

        [TestMethod]
        public void IfNotLoaded_Shows_Placeholders()
        {
            Assert.AreEqual("...", _viewModel.Header);
            Assert.AreEqual("...", _viewModel.Body);
            Assert.IsNull(_viewModel.Thumbnail);
        }

        [TestMethod]
        public void IsLoading_IfPropertiesAreAccessed()
        {
            _entryServiceMock.Setup(s => s.LoadEntryAsSoonAsPossible(_entryReference, Moq.It.IsAny<ElementLoadedDelegate<Entry>>(), Moq.It.IsAny<ElementLoadingDequeuedDelegate>())).Verifiable();

            Assert.IsFalse(_viewModel.IsLoading);

            Assert.AreEqual("...", _viewModel.Header);

            Assert.IsTrue(_viewModel.IsLoading);

            _entryServiceMock.Verify();
        }

        [TestMethod]
        public void InitFromEntry_Refreshes_ViewModelValues()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";
            entry.ThumbnailBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("Barembo rockz"));

            _viewModel.InitFromEntry(entry);

            Assert.AreEqual(entry.Header, _viewModel.Header);
            Assert.AreEqual(entry.Body, _viewModel.Body);
            Assert.AreEqual("Barembo rockz", Encoding.UTF8.GetString(_viewModel.Thumbnail));

            Assert.IsFalse(_viewModel.IsLoading);
        }

        [TestMethod]
        public void AccessingProperties_LoadsEntry_OnlyOnce()
        {
            Entry entry = new Entry();
            entry.Header = "Header";
            entry.Body = "Body";

            _viewModel.InitFromEntry(entry);

            var header = _viewModel.Header; //Access property

            Assert.IsFalse(_viewModel.IsLoading);
            _entryServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void GoBack_Goes_Back()
        {
            _eventAggregator.Setup(s => s.GetEvent<GoBackMessage>()).Returns(new GoBackMessage()).Verifiable();
            _viewModel.GoBackCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.VerifyNoOtherCalls();
        }
    }
}
