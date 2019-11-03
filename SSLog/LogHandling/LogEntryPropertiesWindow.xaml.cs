using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SSLog.LogHandling
{
    /// <summary>
    /// Interaction logic for NewLogWindow.xaml
    /// </summary>
    partial class LogEntryPropertiesWindow : Window
    {
        #region Constructor
        private LogEntryDataModel LogEntry;
       // private DateTime dateComponent;
        private TimeSpan timeComponent;
        // DO NOT USE!
        public LogEntryPropertiesWindow()
        {
            InitializeComponent();
        }

        // Use this one please
        public LogEntryPropertiesWindow(double width, double height, LogEntryDataModel aLogEntry)
        {
            // Set window size
            // Initialise
            InitializeComponent();

            this.Width = width;
            this.Height = height;
            LogEntry = aLogEntry;
            Type = aLogEntry.Type;
            EntryTitle = aLogEntry.EntryTitle;
            Date = aLogEntry.Date;
            timeComponent = Date.TimeOfDay;
          //  dateComponent = Date.Date;

            txtTitle.Text = EntryTitle;
            cbxType.Text = Type;
            dprDatePicker.SelectedDate = Date;
        }

        #endregion

        #region Properties

        public String Type
        {
            get;
            private set;
        }

        public String EntryTitle
        {
            get;
            private set;
        }

        public DateTime Date
        {
            get;
            private set;
        }

        #endregion

        #region Functions

        // Use this!
        // Just incase i ever decide to overload it.

        #endregion
        public Boolean cancelled = true;

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Type = cbxType.Text;
            EntryTitle = txtTitle.Text;
            // Combine dates, retaining the timespan.
            Date = dprDatePicker.SelectedDate.Value;
            Date = Date.Add(timeComponent);
            
            cancelled = false;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
