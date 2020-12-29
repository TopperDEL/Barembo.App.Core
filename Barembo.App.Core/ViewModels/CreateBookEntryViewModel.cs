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

        private ObservableCollection<Tuple<Attachment, Stream>> _attachments;
        public ObservableCollection<Tuple<Attachment, Stream>> Attachments
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

        private Entry _entry;
        async Task ExecuteSaveEntryCommand()
        {
            _entry = _entryService.CreateEntry(Header, Body);
            if(_entry == null)
            {
                _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.EntryCouldNotBeCreated, ""));
                return;
            }

            try
            {
                var entryReference = await _entryService.AddEntryToBookAsync(_bookReference, _entry);

                bool setAsThumbnail = true;
                foreach (var attachment in Attachments)
                {
                    var attachmentAdded = await _entryService.AddAttachmentAsync(entryReference, _entry, attachment.Item1, attachment.Item2);
                    if(!attachmentAdded)
                    {
                        _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.AttachmentCouldNotBeSaved, attachment.Item1.FileName));
                        return;
                    }
                    if (setAsThumbnail)
                    {
                        var thumbnailSet = await _entryService.SetThumbnailAsync(entryReference, _entry, attachment.Item2);
                        if (!thumbnailSet)
                        {
                            _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.ThumbnailCouldNotBeSet, attachment.Item1.FileName));
                            return;
                        }
                    }
                    setAsThumbnail = false;
                }

                _eventAggregator.GetEvent<BookEntrySavedMessage>().Publish(new Tuple<EntryReference, Entry>(entryReference, _entry));
            }
            catch(EntryCouldNotBeSavedException ex)
            {
                _eventAggregator.GetEvent<ErrorMessage>().Publish(new Tuple<ErrorType, string>(ErrorType.EntryCouldNotBeSavedException, ex.Message));
                return;
            }
        }

        bool CanExecuteSaveEntryCommand()
        {
            return true;
        }

        void ExecuteAddMediaCommand()
        {
            MediaResult result = new MediaResult();
            _eventAggregator.GetEvent<MediaRequestedMessage>().Publish(result);
            if(result.MediaSelected)
            {
                _attachments.Add(new Tuple<Attachment, Stream>(result.Attachment, result.Stream));
            }
        }

        bool CanExecuteAddMediaCommand()
        {
            return true;
        }

        public CreateBookEntryViewModel(IEntryService entryService, IEventAggregator eventAggregator)
        {
            _entryService = entryService;
            _eventAggregator = eventAggregator;

            Attachments = new ObservableCollection<Tuple<Attachment, Stream>>();
        }

        public void Init(BookReference bookReference)
        {
            _bookReference = bookReference;
        }
    }
}
