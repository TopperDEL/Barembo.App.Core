﻿using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Barembo.App.Core.ViewModels
{
    public class EntryViewModel : AsyncLoadingBindableBase
    {
        private readonly EntryReference _entryReference;
        private readonly IEntryService _entryService;
        private readonly IEventAggregator _eventAggregator;
        private readonly SynchronizationContext _synchronizationContext;
        private Entry _entry;

        public string Header
        {
            get
            {
                if (_entry == null)
                {
                    LoadEntry();
                    return "...";
                }
                return _entry.Header;
            }
        }

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

        private DelegateCommand delegateCommand;
        public DelegateCommand GoBackCommand =>
            delegateCommand ?? (delegateCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        public EntryViewModel(EntryReference entryReference, IEntryService entryService, IEventAggregator eventAggregator, SynchronizationContext synchronizationContext)
        {
            _entryReference = entryReference;
            _entryService = entryService;
            _eventAggregator = eventAggregator;
            _synchronizationContext = synchronizationContext;
        }

        private void LoadEntry()
        {
            if (IsLoading)
                return;

            IsLoading = true;

            _entryService.LoadEntryAsSoonAsPossible(
                _entryReference, //The entry to load
                (loadedEntry) =>
                    _synchronizationContext.Post((o) =>
                    {
                        InitFromEntry(loadedEntry);
                    }, null), //If the entry got loaded, Refresh the values on the UI-Thread
                () => IsLoading = false); //If the entry failed to load, reset the IsLoading to start a new attempt
        }

        internal void InitFromEntry(Entry entry)
        {
            _entry = entry;

            IsLoading = false;

            RaisePropertyChanged(nameof(Header));
            RaisePropertyChanged(nameof(Body));
            RaisePropertyChanged(nameof(Thumbnail));
        }
    }
}
