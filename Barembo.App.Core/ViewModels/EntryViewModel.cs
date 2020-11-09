using Barembo.Interfaces;
using Barembo.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public class EntryViewModel : AsyncLoadingBindableBase
    {
        private readonly EntryReference _entryReference;
        private readonly IEntryService _entryService;
        private Entry _entry;

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }
        //public string Header
        //{
        //    get
        //    {
        //        if (_entry == null)
        //        {
        //            LoadEntry();
        //            return "...";
        //        }
        //        return _entry.Header;
        //    }
        //}

        public string Body
        {
            get
            {
                if (_entry == null)
                {
                    LoadEntry();
                    return "...";
                }
                return _entry.Body;
            }
        }

        public byte[] Thumbnail
        {
            get
            {
                if (_entry == null)
                {
                    LoadEntry();
                    return null; //ToDo: Return placeholder-Image
                }
                else if (string.IsNullOrEmpty(_entry.ThumbnailBase64))
                    return null;

                return Convert.FromBase64String(_entry.ThumbnailBase64);
            }
        }

        public EntryViewModel(EntryReference entryReference, IEntryService entryService)
        {
            _entryReference = entryReference;
            _entryService = entryService;

            _header = "...";
        }

        private void LoadEntry()
        {
            if (IsLoading)
                return;
            
            IsLoading = true;

            _entryService.LoadEntryAsSoonAsPossible(_entryReference, (loadedEntry) => InitFromEntry(loadedEntry), () => IsLoading = false);
        }

        private void InitFromEntry(Entry entry)
        {
            _entry = entry;
            Header = _entry.Header;

            IsLoading = false;

            //RaisePropertyChanged(nameof(Header));
            //RaisePropertyChanged(nameof(Body));
            //RaisePropertyChanged(nameof(Thumbnail));
        }
    }
}
