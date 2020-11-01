using Barembo.App.Core.Interfaces;
using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    /// <summary>
    /// The ViewModel for the LoginView
    /// </summary>
    public class LoginViewModel : BindableBase
    {
        readonly IStoreAccessService _storeAccessService;
        readonly ILoginService _loginService;
        readonly IEventAggregator _eventAggregator;

        private string _satelliteAddress;
        public string SatelliteAddress
        {
            get { return _satelliteAddress; }
            set
            {
                SetProperty(ref _satelliteAddress, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _apiKey;
        public string ApiKey
        {
            get { return _apiKey; }
            set
            {
                SetProperty(ref _apiKey, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _secret;
        public string Secret
        {
            get { return _secret; }
            set
            {
                SetProperty(ref _secret, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _secretVerify;
        public string SecretVerify
        {
            get { return _secretVerify; }
            set
            {
                SetProperty(ref _secretVerify, value);
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private bool _loginFailed;
        public bool LoginFailed
        {
            get { return _loginFailed; }
            set { SetProperty(ref _loginFailed, value); }
        }

        private bool _secretsDoNotMatch;
        public bool SecretsDoNotMatch
        {
            get { return _secretsDoNotMatch; }
            set { SetProperty(ref _secretsDoNotMatch, value); }
        }

        private DelegateCommand _loginCommand;
        public DelegateCommand LoginCommand =>
            _loginCommand ?? (_loginCommand = new DelegateCommand(ExecuteLoginCommand, CanExecuteLoginCommand));

        void ExecuteLoginCommand()
        {
            LoginData loginData = new LoginData();
            loginData.ApiKey = ApiKey;
            loginData.SatelliteAddress = SatelliteAddress;
            loginData.Secret = Secret;

            var storeAccess = _storeAccessService.GenerateAccessFromLogin(loginData);
            var loggedIn = _loginService.Login(storeAccess);
            if (loggedIn)
                _eventAggregator.GetEvent<SuccessfullyLoggedInMessage>().Publish(storeAccess);
            else
                LoginFailed = true;
        }

        bool CanExecuteLoginCommand()
        {
            SecretsDoNotMatch = false;

            if (string.IsNullOrEmpty(SatelliteAddress))
                return false;
            if (string.IsNullOrEmpty(ApiKey))
                return false;
            if (string.IsNullOrEmpty(Secret))
                return false;
            if (string.IsNullOrEmpty(SecretVerify))
                return false;
            if (Secret != SecretVerify)
            {
                SecretsDoNotMatch = true;
                return false;
            }

            return true;
        }

        public LoginViewModel(IStoreAccessService storeAccessService, ILoginService loginService, IEventAggregator eventAggregator)
        {
            _storeAccessService = storeAccessService;
            _loginService = loginService;
            _eventAggregator = eventAggregator;
        }
    }
}
