﻿using Barembo.App.Core.Interfaces;
using Barembo.Interfaces;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public class VersionInfoViewModel : BindableBase
    {
        private string _storjVersion;
        public string StorjVersion
        {
            get
            {
                return _storjVersion;
            }
            set
            {
                SetProperty(ref _storjVersion, value);
            }
        }

        private string _baremboVersion;
        public string BaremboVersion
        {
            get { return _baremboVersion; }
            set
            {
                SetProperty(ref _baremboVersion, value);
            }
        }

        public VersionInfoViewModel(IStoreAccessService storeAccessService, IBuildVersionInfoService buildVersionInfo)
        {
            try
            {
                StorjVersion = storeAccessService.GetVersionInfo();
            }
            catch (Exception ex)
            {
                StorjVersion = ex.Message;
            }

            try
            {
                BaremboVersion = buildVersionInfo.GetBaremboVersion();
            }
            catch (Exception ex)
            {
                BaremboVersion = ex.Message;
            }
        }
    }
}
