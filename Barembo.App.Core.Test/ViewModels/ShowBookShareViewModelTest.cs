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
    public class ShowBookShareViewModelTest
    {
        ShowBookShareViewModel _viewModel;
        Moq.Mock<IEventAggregator> _eventAggregatorMock;
        Moq.Mock<IQRCodeGeneratorService> _qrCodeGeneratorService;
        Moq.Mock<IMagicLinkGeneratorService> _magicLinkGeneratorService;
        BookShareReference _bookShareReference;

        [TestInitialize]
        public void Init()
        {
            _bookShareReference = new BookShareReference();
            _eventAggregatorMock = new Moq.Mock<IEventAggregator>();
            _qrCodeGeneratorService = new Moq.Mock<IQRCodeGeneratorService>();
            _magicLinkGeneratorService = new Moq.Mock<IMagicLinkGeneratorService>();
            _viewModel = new ShowBookShareViewModel(_eventAggregatorMock.Object, _qrCodeGeneratorService.Object, _magicLinkGeneratorService.Object);
        }

        [TestMethod]
        public void Init_Generates_QRCodeAndMagicLink()
        {
            byte[] qrCodePNG = new byte[10];

            _qrCodeGeneratorService.Setup(s => s.GetQRCodePNGFor(_bookShareReference)).Returns(qrCodePNG).Verifiable();
            _magicLinkGeneratorService.Setup(s => s.GetMagicLinkFor(_bookShareReference)).Returns("myMagicLink").Verifiable();

            _viewModel.Init(_bookShareReference);

            Assert.AreEqual(qrCodePNG, _viewModel.QRCodePNG);
            Assert.AreEqual("myMagicLink", _viewModel.MagicLink);

            _qrCodeGeneratorService.Verify();
            _magicLinkGeneratorService.Verify();
        }

        [TestMethod]
        public void ExecuteWriteMagicLinkToClipboard_Raises_WriteToClipboardMessage()
        {
            _viewModel.MagicLink = "thisShouldBeWritten";

            _eventAggregatorMock.Setup(s => s.GetEvent<WriteToClipboardMessage>()).Returns(new WriteToClipboardMessage()).Verifiable();

            _viewModel.WriteMagicLinkToClipboardCommand.Execute();

            _eventAggregatorMock.Verify();
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
