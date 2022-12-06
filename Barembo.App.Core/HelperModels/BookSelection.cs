using Barembo.App.Core.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Barembo.App.Core.HelperModels
{
    public class BookSelection
    {
        public bool IsSelected { get; set; }
        public BookViewModel BookViewModel { get; set; }
    }
}
