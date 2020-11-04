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
    public class CreateBookEntryViewModelTest
    {
        CreateBookEntryViewModel _viewModel;
        Moq.Mock<IEntryService> _entryServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;
        StoreAccess _storeAccess;

        [TestInitialize]
        public void Init()
        {
            _storeAccess = new StoreAccess("use this access");
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _entryServiceMock = new Moq.Mock<IEntryService>();
            _viewModel = new CreateBookEntryViewModel(_entryServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public void ExecuteSaveEntry_Publishes_Message()
        {
            BookReference bookReference = new BookReference();
            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteSaveEntry_Saves_Entry()
        {
            BookReference bookReference = new BookReference();
            EntryReference entryRef = new EntryReference();
            Entry entry = new Entry();

            _viewModel.Header = "header";
            _viewModel.Body = "body";

            _eventAggregator.Setup(s => s.GetEvent<BookEntrySavedMessage>()).Returns(new BookEntrySavedMessage()).Verifiable();
            _entryServiceMock.Setup(s => s.CreateEntry(_viewModel.Header, _viewModel.Body)).Returns(entry).Verifiable();
            _entryServiceMock.Setup(s => s.AddEntryToBookAsync(bookReference, entry)).Returns(Task.FromResult(entryRef)).Verifiable();

            _viewModel.Init(bookReference);
            _viewModel.SaveEntryCommand.Execute();

            _eventAggregator.Verify();
            _entryServiceMock.Verify();
        }
    }
}
