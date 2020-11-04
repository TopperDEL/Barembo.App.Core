using Barembo.App.Core.Messages;
using Barembo.Interfaces;
using Barembo.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
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

        private DelegateCommand _saveEntryCommand;
        public DelegateCommand SaveEntryCommand =>
            _saveEntryCommand ?? (_saveEntryCommand = new DelegateCommand(async () => await ExecuteSaveEntryCommand().ConfigureAwait(false), CanExecuteSaveEntryCommand));

        async Task ExecuteSaveEntryCommand()
        {
            Entry entry = _entryService.CreateEntry(Header, Body);

            var entryReference = await _entryService.AddEntryToBookAsync(_bookReference, entry);

            _eventAggregator.GetEvent<BookEntrySavedMessage>().Publish(new Tuple<BookReference, EntryReference>(_bookReference, entryReference));
        }

        bool CanExecuteSaveEntryCommand()
        {
            return true;
        }

        public CreateBookEntryViewModel(IEntryService entryService, IEventAggregator eventAggregator)
        {
            _entryService = entryService;
            _eventAggregator = eventAggregator;
        }

        public void Init(BookReference bookReference)
        {
            _bookReference = bookReference;
        }
    }
}
