using Barembo.App.Core.Messages;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public class BookEntriesViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private ObservableCollection<EntryViewModel> _entries;
        public ObservableCollection<EntryViewModel> Entries
        {
            get { return _entries; }
            set { SetProperty(ref _entries, value); }
        }

        private DelegateCommand _goBackCommand;
        public DelegateCommand GoBackCommand =>
            _goBackCommand ?? (_goBackCommand = new DelegateCommand(ExecuteGoBackCommand));

        void ExecuteGoBackCommand()
        {
            _eventAggregator.GetEvent<GoBackMessage>().Publish();
        }

        public BookEntriesViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void InitEntries(ObservableCollection<EntryViewModel> entries)
        {
            _entries = entries;
        }
    }
}
