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
        readonly IThumbnailGeneratorService _thumbnailGeneratorService;

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

        private bool _saveInProgress;
        public bool SaveInProgress
        {
            get { return _saveInProgress; }
            set { SetProperty(ref _saveInProgress, value); }
        }

        private ObservableCollection<MediaDataViewModel> _attachments;
        public ObservableCollection<MediaDataViewModel> Attachments
        {
            get { return _attachments; }
            private set { SetProperty(ref _attachments, value); }
        }

        private DelegateCommand _saveEntryCommand;
        public DelegateCommand SaveEntryCommand =>
            _saveEntryCommand ?? (_saveEntryCommand = new DelegateCommand(async () => await ExecuteSaveEntryCommand().ConfigureAwait(false), CanExecuteSaveEntryCommand));

        private DelegateCommand _addImageCommand;
        public DelegateCommand AddImageCommand =>
            _addImageCommand ?? (_addImageCommand = new DelegateCommand(() => ExecuteAddMediaCommand(AttachmentType.Image), CanExecuteAddMediaCommand));

        private DelegateCommand _addVideoCommand;
        public DelegateCommand AddVideoCommand =>
            _addVideoCommand ?? (_addVideoCommand = new DelegateCommand(() => ExecuteAddMediaCommand(AttachmentType.Video), CanExecuteAddMediaCommand));

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        async Task ExecuteSaveEntryCommand()
        {
            SaveInProgress = true;
            try
            {
                var entry = _entryService.CreateEntry(Header, Body);
                if (entry == null)
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.EntryCouldNotBeCreated, ""));
                    return;
                }

                try
                {
                    var entryReference = await _entryService.AddEntryToBookAsync(_bookReference, entry);

                    _eventAggregator.GetEvent<InAppInfoMessage>().Publish(new Tuple<InAppInfoMessageType, Dictionary<string, string>>(InAppInfoMessageType.EntrySaved, new Dictionary<string, string>() { { "EntryID", entry.Id } }));

                    bool setAsThumbnail = true;
                    foreach (var attachment in Attachments)
                    {
                        _eventAggregator.GetEvent<InAppInfoMessage>().Publish(new Tuple<InAppInfoMessageType, Dictionary<string, string>>(InAppInfoMessageType.SavingAttachment, new Dictionary<string, string>() { { "AttachmentName", attachment.MediaData.Attachment.FileName } }));

                        var attachmentAdded = await _entryService.AddAttachmentAsync(entryReference, entry, attachment.MediaData.Attachment, attachment.MediaData.Stream, attachment.MediaData.FilePath);
                        if (!attachmentAdded)
                        {
                            _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.AttachmentCouldNotBeSaved, attachment.MediaData.Attachment.FileName));
                            return;
                        }
                        if (setAsThumbnail)
                        {
                            var thumbnailSet = await _entryService.SetThumbnailAsync(entryReference, entry, attachment.MediaData.Attachment, attachment.MediaData.Stream, attachment.MediaData.FilePath);
                            if (!thumbnailSet)
                            {
                                _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.ThumbnailCouldNotBeSet, attachment.MediaData.Attachment.FileName));
                                return;
                            }
                        }
                        setAsThumbnail = false;

                        _eventAggregator.GetEvent<InAppInfoMessage>().Publish(new Tuple<InAppInfoMessageType, Dictionary<string, string>>(InAppInfoMessageType.AttachmentSaved, new Dictionary<string, string>() { { "AttachmentName", attachment.MediaData.Attachment.FileName } }));
                    }

                    _eventAggregator.GetEvent<BookEntrySavedMessage>().Publish(new Tuple<EntryReference, Entry>(entryReference, entry));
                }
                catch (EntryCouldNotBeSavedException ex)
                {
                    _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.EntryCouldNotBeSavedException, ex.Message));
                }
            }
            finally
            {
                SaveInProgress = false;
            }
        }

        bool CanExecuteSaveEntryCommand()
        {
            return !SaveInProgress;
        }

        void ExecuteAddMediaCommand(AttachmentType attachmentType)
        {
            _eventAggregator.GetEvent<MediaRequestedMessage>().Publish(attachmentType);
        }

        bool CanExecuteAddMediaCommand()
        {
            return true;
        }

        public CreateBookEntryViewModel(IEntryService entryService, IEventAggregator eventAggregator, IThumbnailGeneratorService thumbnailGeneratorService)
        {
            _entryService = entryService;
            _eventAggregator = eventAggregator;
            _thumbnailGeneratorService = thumbnailGeneratorService;
            _eventAggregator.GetEvent<MediaReceivedMessage>().Subscribe(HandleMediaReceived);

            Attachments = new ObservableCollection<MediaDataViewModel>();
        }

        public void Init(BookReference bookReference)
        {
            _bookReference = bookReference;
        }

        internal void HandleMediaReceived(MediaData mediaData)
        {
            _attachments.Add(new MediaDataViewModel(mediaData, _thumbnailGeneratorService));
        }
    }
}
