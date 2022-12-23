using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public delegate void EntryLoadedDelegate(EntryViewModel vm, Entry entry);
    public class EntryViewModel : AsyncLoadingBindableBase
    {
        private readonly EntryReference _entryReference;
        private readonly IEntryService _entryService;
        private readonly SynchronizationContext _synchronizationContext;
        private Entry _entry;
        public event EntryLoadedDelegate EntryLoaded;
        private readonly object _loadingLock = new object();

        public string Header
        {
            get
            {
                if (_entry == null)
                {
                    LoadEntryInBackground();
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
                    LoadEntryInBackground();
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
                    LoadEntryInBackground();
                    return null;
                }
                else if (string.IsNullOrEmpty(_entry.ThumbnailBase64))
                    return null;

                return Convert.FromBase64String(_entry.ThumbnailBase64);
            }
        }

        public bool HasThumbnail { get; set; }

        public DateTime CreationDate
        {
            get
            {
                return _entryReference.CreationDate;
            }
        }

        public ObservableCollection<AttachmentPreviewViewModel> AttachmentPreviews { get; set; }

        public EntryViewModel(EntryReference entryReference, IEntryService entryService, SynchronizationContext synchronizationContext)
        {
            _entryReference = entryReference;
            _entryService = entryService;
            _synchronizationContext = synchronizationContext;
            AttachmentPreviews = new ObservableCollection<AttachmentPreviewViewModel>();
        }

        public void LoadEntryInBackground()
        {

            lock (_loadingLock)
            {
                if (IsLoading)
                    return;

                IsLoading = true;

                _entryService.LoadEntryAsSoonAsPossible(
                    _entryReference, //The entry to load
                    (loadedEntry) =>
                        _synchronizationContext.Post((_) =>
                        {
                            InitFromEntry(loadedEntry);
                        }, null), //If the entry got loaded, Refresh the values on the UI-Thread
                    () => IsLoading = false); //If the entry failed to load, reset the IsLoading to start a new attempt
            }
        }

        public async Task LoadEntryAsync()
        {
            lock (_loadingLock)
            {
                if (IsLoading)
                    return;

                IsLoading = true;
            }

            var entry = await _entryService.LoadEntryAsync(_entryReference);
            _synchronizationContext.Post((_) =>
            {
                InitFromEntry(entry);
            }, null);
        }

        public async Task LoadAttachmentPreviewsAsync()
        {
            if (_entry != null)
            {
                foreach (var attachment in _entry.Attachments)
                {
                    try
                    {
                        var preview = await _entryService.LoadAttachmentPreviewAsync(_entryReference, attachment);
                        AttachmentPreviews.Add(new AttachmentPreviewViewModel(preview));
                    }
                    catch (Barembo.Exceptions.AttachmentPreviewNotExistsException)
                    {
                        //Ignore
                    }
                }
            }
        }

        internal void InitFromEntry(Entry entry)
        {
            _entry = entry;

            EntryLoaded?.Invoke(this, _entry);

            if(!string.IsNullOrEmpty(_entry.ThumbnailBase64))
            {
                HasThumbnail= true;
            }

            IsLoading = false;

            RaisePropertyChanged(nameof(Header));
            RaisePropertyChanged(nameof(Body));
            RaisePropertyChanged(nameof(Thumbnail));
            RaisePropertyChanged(nameof(HasThumbnail));
        }
    }
}
