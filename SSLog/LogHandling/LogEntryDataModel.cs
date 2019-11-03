using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Will need other fields for Rich Text settings
// Link this to Log Entries.
namespace SSLog.LogHandling
{
    [Serializable]
    public class LogEntryDataModel : INotifyPropertyChanged
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

        private string _type;
        public String Type
        {
            get { return _type; }
            set
            {
                _type = value;
                this.RaisePropertyChanged("Type");
            }
        }

        private string _entryTitle;
        public String EntryTitle
        {
            get {return _entryTitle; }
            set
            {
                _entryTitle = value;
                this.RaisePropertyChanged("EntryTitle");
            }
        }

        // Change this to a date type lol
        private DateTime _date;
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                this.RaisePropertyChanged("Date");
            }
        }

        [field:NonSerialized]
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
