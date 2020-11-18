using Barembo.App.Core.Messages;
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

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        async Task ExecuteSaveEntryCommand()
        {
            Entry entry = _entryService.CreateEntry(Header, Body);

            var entryReference = await _entryService.AddEntryToBookAsync(_bookReference, entry);

            bool setAsThumbnail = true;
            foreach(var attachment in Attachments)
            {
                await _entryService.AddAttachmentAsync(entryReference, entry, attachment.Item1, attachment.Item2, setAsThumbnail);
                setAsThumbnail = false;
            }

            _eventAggregator.GetEvent<BookEntrySavedMessage>().Publish(entryReference);
        }

        bool CanExecuteSaveEntryCommand()
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
