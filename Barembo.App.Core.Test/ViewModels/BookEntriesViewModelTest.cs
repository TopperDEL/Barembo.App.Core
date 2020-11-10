using Barembo.App.Core.Messages;
using Barembo.App.Core.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.Test.ViewModels
{
    [TestClass]
    public class BookEntriesViewModelTest
    {
        BookEntriesViewModel _viewModel;
        Moq.Mock<IEventAggregator> _eventAggregator;

        [TestInitialize]
        public void Init()
        {
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _viewModel = new BookEntriesViewModel(_eventAggregator.Object);
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
