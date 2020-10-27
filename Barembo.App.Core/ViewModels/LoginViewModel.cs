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
    public class LoginViewModel : BindableBase
    {
        IStoreAccessService _storeAccessService;
        ILoginService _loginService;
        IEventAggregator _eventAggregator;

        private string satelliteAddress;
        public string SatelliteAddress
        {
            get { return satelliteAddress; }
            set { SetProperty(ref satelliteAddress, value); }
        }

        private string apiKey;
        public string ApiKey
        {
            get { return apiKey; }
            set { SetProperty(ref apiKey, value); }
        }

        private string secret;
        public string Secret
        {
            get { return secret; }
            set { SetProperty(ref secret, value); }
        }

        private string secretVerify;
        public string SecretVerify
        {
            get { return secretVerify; }
            set { SetProperty(ref secretVerify, value); }
        }

        private bool loginFailed;
        public bool LoginFailed
        {
            get { return loginFailed; }
            set { SetProperty(ref loginFailed, value); }
        }

        private DelegateCommand delegateCommand;
        public DelegateCommand LoginCommand =>
            delegateCommand ?? (delegateCommand = new DelegateCommand(ExecuteLoginCommand, CanExecuteLoginCommand));

        void ExecuteLoginCommand()
        {
            LoginData loginData = new LoginData();
            loginData.ApiKey = ApiKey;
            loginData.SatelliteAddress = SatelliteAddress;
            loginData.Secret = Secret;

            var storeAccess = _storeAccessService.GenerateAccesFromLogin(loginData);
            var loggedIn = _loginService.Login(storeAccess);
            if (loggedIn)
                _eventAggregator.GetEvent<SuccessfullyLoggedInMessage>().Publish(storeAccess);
            else
                LoginFailed = true;
        }

        bool CanExecuteLoginCommand()
        {
            if (string.IsNullOrEmpty(SatelliteAddress))
                return false;
            if (string.IsNullOrEmpty(ApiKey))
                return false;
            if (string.IsNullOrEmpty(Secret))
                return false;
            if (string.IsNullOrEmpty(SecretVerify))
                return false;
            if (Secret != SecretVerify)
                return false;

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
