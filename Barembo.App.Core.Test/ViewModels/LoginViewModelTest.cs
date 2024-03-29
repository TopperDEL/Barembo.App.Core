﻿using Barembo.App.Core.Interfaces;
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
    public class LoginViewModelTest
    {
        LoginViewModel _viewModel;
        Moq.Mock<IStoreAccessService> _storeAccessServiceMock;
        Moq.Mock<ILoginService> _loginServiceMock;
        Moq.Mock<IEventAggregator> _eventAggregator;

        [TestInitialize]
        public void Init()
        {
            _storeAccessServiceMock = new Moq.Mock<IStoreAccessService>();
            _loginServiceMock = new Moq.Mock<ILoginService>();
            _eventAggregator = new Moq.Mock<IEventAggregator>();
            _viewModel = new LoginViewModel(_storeAccessServiceMock.Object, _loginServiceMock.Object, _eventAggregator.Object);
        }

        [TestMethod]
        public void Login_IsPossible_IfEverythingIsProvided()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.ApiKey = "apiKey";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecret";

            Assert.IsTrue(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_IsPossible_IfAccessGrantIsProvided()
        {
            _viewModel.AccessGrant = "myAccessGrant";
            
            Assert.IsTrue(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_IsNotPossible_IfSatelliteAddressIsEmpty()
        {
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecret";
            _viewModel.ApiKey = "apiKey";

            Assert.IsFalse(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_IsNotPossible_IfApiKeyIsEmpty()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecret";

            Assert.IsFalse(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_IsNotPossible_IfSecretIsEmpty()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.SecretVerify = "mySecret";
            _viewModel.ApiKey = "apiKey";

            Assert.IsFalse(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_IsNotPossible_IfSecretVerifyIsEmpty()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.ApiKey = "apiKey";

            Assert.IsFalse(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_IsNotPossible_IfSecretAndSecretVerifyAreNotEqual()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";

            Assert.IsFalse(_viewModel.LoginCommand.CanExecute());
        }

        [TestMethod]
        public void Login_SetsSecretDoNotMatch_IfTheyAreNotEmptyAndDoNotMatch()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";

            _viewModel.LoginCommand.CanExecute();

            Assert.IsTrue(_viewModel.SecretsDoNotMatch);
        }

        [TestMethod]
        public void Login_DoesNotSetSecretDoNotMatch_IfTheyAreEmpty()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "";
            _viewModel.ApiKey = "apiKey";

            _viewModel.LoginCommand.CanExecute();

            Assert.IsFalse(_viewModel.SecretsDoNotMatch);

            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "";
            _viewModel.SecretVerify = "mySecretVerifiy";
            _viewModel.ApiKey = "apiKey";

            _viewModel.LoginCommand.CanExecute();

            Assert.IsFalse(_viewModel.SecretsDoNotMatch);
        }

        [TestMethod]
        public void Login_DoesNotSetSecretDoNotMatch_IfOneIsEmptryButHaventMatchedBefore()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";

            _viewModel.LoginCommand.CanExecute();

            Assert.IsTrue(_viewModel.SecretsDoNotMatch);

            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "";
            _viewModel.ApiKey = "apiKey";

            _viewModel.LoginCommand.CanExecute();

            Assert.IsFalse(_viewModel.SecretsDoNotMatch);
        }

        [TestMethod]
        public void Login_DoesNotSetSecretDoNotMatch_IfTheyMatch()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecret";
            _viewModel.ApiKey = "apiKey";

            _viewModel.LoginCommand.CanExecute();

            Assert.IsFalse(_viewModel.SecretsDoNotMatch);
        }

        [TestMethod]
        public void ExecuteLogin_LogsIn()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";
            StoreAccess access = new StoreAccess("myAccess");
            SuccessfullyLoggedInMessage loggedInMessage = new SuccessfullyLoggedInMessage();

            _storeAccessServiceMock.Setup(s => s.GenerateAccessFromLogin(Moq.It.Is<LoginData>(l => l.ApiKey == _viewModel.ApiKey &&
                                                                                                  l.SatelliteAddress == _viewModel.SatelliteAddress &&
                                                                                                  l.Secret == _viewModel.Secret))).Returns(access).Verifiable();

            _loginServiceMock.Setup(s => s.Login(access)).Returns(true).Verifiable();

            _eventAggregator.Setup(s => s.GetEvent<SuccessfullyLoggedInMessage>()).Returns(loggedInMessage).Verifiable();

            _viewModel.LoginCommand.Execute();

            Assert.IsFalse(_viewModel.LoginFailed);

            _storeAccessServiceMock.Verify();
            _loginServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteLogin_RaisesError_IfCouldNotGenerateAccess()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";

            _storeAccessServiceMock.Setup(s => s.GenerateAccessFromLogin(Moq.It.Is<LoginData>(l => l.ApiKey == _viewModel.ApiKey &&
                                                                                                  l.SatelliteAddress == _viewModel.SatelliteAddress &&
                                                                                                  l.Secret == _viewModel.Secret))).Throws(new Exception("Test")).Verifiable();

            _viewModel.LoginCommand.Execute();

            Assert.IsTrue(_viewModel.LoginFailed);
            Assert.AreEqual(_viewModel.LoginError, "Could not generate access - Test");

            _storeAccessServiceMock.Verify();
            _loginServiceMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public void ExecuteLogin_RaisesError_IfCouldNotLogIn()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";
            StoreAccess access = new StoreAccess("myAccess");

            _storeAccessServiceMock.Setup(s => s.GenerateAccessFromLogin(Moq.It.Is<LoginData>(l => l.ApiKey == _viewModel.ApiKey &&
                                                                                                  l.SatelliteAddress == _viewModel.SatelliteAddress &&
                                                                                                  l.Secret == _viewModel.Secret))).Returns(access).Verifiable();

            _loginServiceMock.Setup(s => s.Login(access)).Returns(false).Verifiable();

            _viewModel.LoginCommand.Execute();

            Assert.IsTrue(_viewModel.LoginFailed);
            Assert.AreEqual(_viewModel.LoginError, "Could not log in");

            _storeAccessServiceMock.Verify();
            _loginServiceMock.Verify();
        }

        [TestMethod]
        public void ExecuteLogin_LogsIn_AndRaisesLoggedInEvent()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";
            StoreAccess access = new StoreAccess("myAccess");
            SuccessfullyLoggedInMessage loggedInMessage = new SuccessfullyLoggedInMessage();

            _storeAccessServiceMock.Setup(s => s.GenerateAccessFromLogin(Moq.It.Is<LoginData>(l => l.ApiKey == _viewModel.ApiKey &&
                                                                                                  l.SatelliteAddress == _viewModel.SatelliteAddress &&
                                                                                                  l.Secret == _viewModel.Secret))).Returns(access).Verifiable();

            _loginServiceMock.Setup(s => s.Login(access)).Returns(true).Verifiable();

            _eventAggregator.Setup(s => s.GetEvent<SuccessfullyLoggedInMessage>()).Returns(loggedInMessage).Verifiable();

            _viewModel.LoginCommand.Execute();

            Assert.IsFalse(_viewModel.LoginFailed);

            _storeAccessServiceMock.Verify();
            _loginServiceMock.Verify();
            _eventAggregator.Verify();
        }

        [TestMethod]
        public void ExecuteLogin_SetsError_IfLoginFailed()
        {
            _viewModel.SatelliteAddress = "europe-west-1.tardigrade.io:7777";
            _viewModel.Secret = "mySecret";
            _viewModel.SecretVerify = "mySecretVerify";
            _viewModel.ApiKey = "apiKey";
            StoreAccess access = new StoreAccess("myAccess");

            _storeAccessServiceMock.Setup(s => s.GenerateAccessFromLogin(Moq.It.Is<LoginData>(l => l.ApiKey == _viewModel.ApiKey &&
                                                                                                  l.SatelliteAddress == _viewModel.SatelliteAddress &&
                                                                                                  l.Secret == _viewModel.Secret))).Returns(access).Verifiable();

            _loginServiceMock.Setup(s => s.Login(access)).Returns(false).Verifiable();

            _viewModel.LoginCommand.Execute();

            Assert.IsTrue(_viewModel.LoginFailed);

            _storeAccessServiceMock.Verify();
            _loginServiceMock.Verify();
            _eventAggregator.Verify();
        }
    }
}
