using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Link this to Log Entries.
namespace SSLog.LogHandling
{
    class LogEntryViewModel : INotifyPropertyChanged
    {
        private String internalContent;
        public String content
        {
            get { return this.internalContent; }
            set
            {
                this.internalContent = value;

                this.RaisePropertyChanged("content");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
