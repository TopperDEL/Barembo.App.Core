using Barembo.App.Core.Messages;
using Barembo.Exceptions;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Barembo.App.Core.ViewModels
{
    public class CreateBookEntryViewModel : BindableBase
    {
        readonly IEntryService _entryService;
        readonly IEventAggregator _eventAggregator;
        private BookReference _bookReference;

        private string _header;
        public string Header
        {
            get { return _header; }
            set { SetProperty(ref _header, value); }
        }

        private string _body;
        public string Body
        {
            get { return _body; }
            set { SetProperty(ref _body, value); }
        }

        private ObservableCollection<MediaData> _attachments;
        public ObservableCollection<MediaData> Attachments
        {
            get { return _attachments; }
            private set { SetProperty(ref _attachments, value); }
        }

        private DelegateCommand _saveEntryCommand;
        public DelegateCommand SaveEntryCommand =>
            _saveEntryCommand ?? (_saveEntryCommand = new DelegateCommand(async () => await ExecuteSaveEntryCommand().ConfigureAwait(false), CanExecuteSaveEntryCommand));

        private DelegateCommand _addMediaCommand;
        public DelegateCommand AddMediaCommand =>
            _addMediaCommand ?? (_addMediaCommand = new DelegateCommand(() => ExecuteAddMediaCommand(), CanExecuteAddMediaCommand));

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        async Task ExecuteSaveEntryCommand()
        {
            var entry = _entryService.CreateEntry(Header, Body);
            if(entry == null)
            {
                _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.EntryCouldNotBeCreated, ""));
                return;
            }

            try
            {
                var entryReference = await _entryService.AddEntryToBookAsync(_bookReference, entry);

                bool setAsThumbnail = true;
                foreach (var attachment in Attachments)
                {
                    var attachmentAdded = await _entryService.AddAttachmentAsync(entryReference, entry, attachment.Attachment, attachment.Stream);
                    if(!attachmentAdded)
                    {
                        _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.AttachmentCouldNotBeSaved, attachment.Attachment.FileName));
                        return;
                    }
                    if (setAsThumbnail)
                    {
                        var thumbnailSet = await _entryService.SetThumbnailAsync(entryReference, entry, attachment.Attachment, attachment.Stream, attachment.FilePath);
                        if (!thumbnailSet)
                        {
                            _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.ThumbnailCouldNotBeSet, attachment.Attachment.FileName));
                            return;
                        }
                    }
                    setAsThumbnail = false;
                }

                _eventAggregator.GetEvent<BookEntrySavedMessage>().Publish(new Tuple<EntryReference, Entry>(entryReference, entry));
            }
            catch(EntryCouldNotBeSavedException ex)
            {
                _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.EntryCouldNotBeSavedException, ex.Message));
            }
        }

        bool CanExecuteSaveEntryCommand()
        {
            return true;
        }

        void ExecuteAddMediaCommand()
        {
            _eventAggregator.GetEvent<MediaRequestedMessage>().Publish();
        }

        bool CanExecuteAddMediaCommand()
        {
            return true;
        }

        public CreateBookEntryViewModel(IEntryService entryService, IEventAggregator eventAggregator)
        {
            _entryService = entryService;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<MediaReceivedMessage>().Subscribe(HandleMediaReceived);

            Attachments = new ObservableCollection<MediaData>();
        }

        public void Init(BookReference bookReference)
        {
            _bookReference = bookReference;
        }

        internal void HandleMediaReceived(MediaData mediaData)
        {
            _attachments.Add(mediaData);
        }
    }
}
