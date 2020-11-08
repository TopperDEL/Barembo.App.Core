using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Barembo.App.Core.ViewModels
{
    public class BookEntriesViewModel : BindableBase
    {
        private ObservableCollection<EntryViewModel> _entries;
        public ObservableCollection<EntryViewModel> Entries
        {
            get { return _entries; }
            set { SetProperty(ref _entries, value); }
        }

        public BookEntriesViewModel()
        {

        }

        public void InitEntries(ObservableCollection<EntryViewModel> entries)
        {
            _entries = entries;
        }
    }
}
