using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public abstract class AsyncLoadingBindableBase: BindableBase
    {
        private bool _isLoading;
        public bool IsLoading
        {
            get { return _isLoading; }
            set { SetProperty(ref _isLoading, value); }
        }

        private bool _loadingFailed;
        public bool LoadingFailed
        {
            get { return _loadingFailed; }
            set { SetProperty(ref _loadingFailed, value); }
        }

        private string _loadingError;
        public string LoadingError
        {
            get { return _loadingError; }
            set { SetProperty(ref _loadingError, value); }
        }
    }
}
