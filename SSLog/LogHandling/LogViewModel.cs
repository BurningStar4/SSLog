using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSLog.LogHandling;
using System.Windows.Input;
using System.Windows;
using SSLog.IO;
using System.ComponentModel;
using System.Timers;
using System.Windows.Data;

namespace SSLog
{
    // This View Model holds a collection of LogEntries. This ViewModel essentially represents a Log in the program.
    // Consider turning this into its own model IF metadata is required. System file metadata may be enough though.
    // Cross compatible serialization between C# and android may be problematic.
    // This is the primary view model.
    class LogViewModel : INotifyPropertyChanged
    {
        #region Private Variables

        Boolean CollectionIsDirty = false;
        Boolean LogOnDisk = false;
        int StatusBarTextDuration = 4000;

        #endregion

        #region Properties

        private int _listViewSelectedIndex;
        public int ListViewSelectedIndex
        {
            get { return _listViewSelectedIndex; }
            set
            {
                _listViewSelectedIndex = value;
                this.RaisePropertyChanged("ListViewSelectedIndex");
            }
        }

        private string version = "0.3";
        private string logName = "New Log";

        private string _statusBarText;
        public string StatusBarText
        {
            get { return _statusBarText; }
            set
            {
                _statusBarText = value;
                this.RaisePropertyChanged("StatusBarText");
            }
        }

        private string _windowTitle;
        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                this.RaisePropertyChanged("WindowTitle");
            }
        }

        public ObservableCollection<LogEntryDataModel> LogEntries
        {
            get;
            private set;
        }

        private CollectionViewSource CvsLogEntries { get; set; }
        public ICollectionView LogEntriesViewable
        {
            get { return CvsLogEntries.View; }
        }

        #endregion

        #region Setup

        public LogViewModel()
        {
            setupLogViewModel();
        }

        public LogViewModel(string filepath)
        {
            setupLogViewModel();

            loadLogEntries(filepath);
        }

        private void setupLogViewModel()
        {
            this.LogEntries = new ObservableCollection<LogEntryDataModel>();
            LogEntries.CollectionChanged += LogEntriesChanged;
            setWindowTitle();
            StatusBarText = "Ready to Log!";

            CvsLogEntries = new CollectionViewSource();
            CvsLogEntries.Source = LogEntries;
            CvsLogEntries.Filter += ApplyFilter;

            timer = new Timer(StatusBarTextDuration);
            timer.Elapsed += new ElapsedEventHandler(ClearStatusBarText);
        }

        private void ApplyOptions()
        {
            // Timer
            timer.Enabled = false;
            timer.Interval = StatusBarTextDuration;
        }

        #endregion

        #region Prompts

        private MessageBoxResult PromptToSave()
        {
            if (Properties.Settings.Default.saveOnExit == false)
            {
                return MessageBoxResult.No;
            }

            MessageBoxResult result = MessageBox.Show("Do you want to save first?", "SSLog", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                // Save or Save As depending
                if (LogOnDisk)
                {
                    // Just do normal save
                    SaveLog(false);
                }
                else
                {
                    // Call Save As
                    SaveLog(true);
                }
            }
            return result;
        }

        private MessageBoxResult ConfirmDelete()
        {
            if (Properties.Settings.Default.confirmOnDelete)
            {
               return MessageBox.Show("Do you really want to delete this entry?", "SSLog", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            }
            else
            {
                return MessageBoxResult.Yes;
            }
        }

        #endregion

        #region Commands

        private ICommand _newLogCommand;
        public ICommand NewLogCommand
        {
            get
            {
                if (_newLogCommand == null)
                {
                    _newLogCommand = new DelegateCommand<object>(
                        param => MakeNewLog(),
                        param => true
                    );
                }
                return _newLogCommand;
            }
        }

        private void MakeNewLog()
        {
            // Prompt user if they haven't saved.
            if (CollectionIsDirty)
            {
                if (PromptToSave() != MessageBoxResult.Cancel)
                {
                    setupNewLog();
                }
            }
            else
            {
                setupNewLog();
            }
            CollectionIsDirty = false;
            UpdateStatusAsync("Ready to Log!");
        }

        private void setupNewLog()
        {
            LogEntries.Clear();
            CollectionIsDirty = true;
            LogOnDisk = false;

            logName = "New Log";
            setWindowTitle();
        }

        private ICommand _addLogEntryCommand;
        public ICommand AddLogEntryCommand
        {
            get
            {
                if (_addLogEntryCommand == null)
                {
                    _addLogEntryCommand = new DelegateCommand<string>(
                    param => AddLogEntry(),
                    param => true
                    );
                }
                return _addLogEntryCommand;
            }
        }

        private void AddLogEntry()
        {
           // LogEntryTitle to be taken from a custom input field.
            NewLogWindow dlg = new NewLogWindow((Application.Current.MainWindow.ActualWidth / 3.3), (Application.Current.MainWindow.ActualHeight / 2));
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();

            if (!dlg.cancelled)
            {
                LogEntries.Add(new LogEntryDataModel { content = "", Type = dlg.Type, Date = DateTime.Now, EntryTitle = dlg.EntryTitle });
                UpdateStatusAsync("New Entry added!");
                CollectionIsDirty = true;
            }
        }

        private ICommand _deleteLogEntryCommand;
        public ICommand DeleteLogEntryCommand
        {
            get
            {
                if (_deleteLogEntryCommand == null)
                {
                    _deleteLogEntryCommand = new DelegateCommand<LogEntryDataModel>(
                        param => DeleteLogEntry(param),
                        param => true
                     );
                }
                return _deleteLogEntryCommand;
            }
        }

        private void DeleteLogEntry(LogEntryDataModel aLogEntry)
        {
            if(ConfirmDelete() == MessageBoxResult.Yes)
            {
                LogEntries.Remove(aLogEntry);
                CollectionIsDirty = true;
            }
        }

        private ICommand _exitCommand;
        public ICommand ExitCommand
        {
            get
            {
                if (_exitCommand == null)
                {
                    _exitCommand = new DelegateCommand<object>(
                        param => Application.Current.MainWindow.Close(),
                        param => true
                        );
                }
                return _exitCommand;
            }
        }

        private void Exit()
        {
            if (!CollectionIsDirty)
            {
                Application.Current.Shutdown();
            }
            else
            {
                if (PromptToSave() != MessageBoxResult.Cancel)
                {
                    Application.Current.Shutdown();
                }
            }
        }

        private ICommand _openCommand;
        public ICommand OpenCommand
        {
            get
            {
                if (_openCommand == null)
                {
                    _openCommand = new DelegateCommand<object>(
                        param => loadLogEntries(),
                        param => true
                    );
                }
                return _openCommand;
            }
        }

        // Loads LogEntries by copying the list.

        private void loadLogEntries()
        {
            loadLogEntries("");
        }

        private void loadLogEntries(String filename)
        {
            Tuple<ObservableCollection<LogEntryDataModel>, String> loadResultTuple;
            if (!CollectionIsDirty)
            {
                loadResultTuple = FileIO.LoadLog(filename);

                ObservableCollection<LogEntryDataModel> logColl = loadResultTuple.Item1;
                if (logColl != null)
                {                                       
                    LogEntries.Clear();

                    foreach (var logEntry in logColl)
                    {
                        LogEntries.Add(logEntry);
                    }
                    LogOnDisk = true;
                    CollectionIsDirty = false;
                    ListViewSelectedIndex = 0;
                    logName = loadResultTuple.Item2;
                    setWindowTitle();
                    UpdateStatusAsync("Successfully opened " + logName);
                }
                else if(loadResultTuple.Item2.Equals("BADFILE"))
                {
                    MessageBox.Show("The file is either from an older version or invalid and cannot be opened.","SSLog");
                }
            }
            else
            {
                // Prompt for save
                if (PromptToSave() != MessageBoxResult.Cancel)
                {
                    CollectionIsDirty = false;
                    loadLogEntries();
                }
            }
        }

        private ICommand _saveAsCommand;
        public ICommand SaveAsCommand
        {
            get
            {
                if (_saveAsCommand == null)
                {
                    _saveAsCommand = new DelegateCommand<object>(
                    param => SaveLog(true),
                    param => true //Add error checking here you peasant
                    );
                }
                return _saveAsCommand;
            }
        }
        
        private ICommand _saveCommand;
        public ICommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new DelegateCommand<object>(
                    param => SaveLog(false),
                    param => true //Add error checking here you peasant
                    );
                }
                return _saveCommand;
            }
        }

        private ICommand _saveOrSaveAsCommand;
        public ICommand SaveOrSaveAsCommand
        {
            get
            {
                if (_saveOrSaveAsCommand == null)
                {
                    _saveOrSaveAsCommand = new DelegateCommand<object>(
                    param => SaveLog(!LogOnDisk),
                    param => true //Add error checking here you peasant
                    );
                }
                return _saveOrSaveAsCommand;
            }
        }

        private void SaveLog(Boolean saveAs)
        {
            Tuple<Boolean, String> saveResultTuple = FileIO.SaveLog(LogEntries, saveAs);

            if (saveResultTuple.Item1)
            {
                CollectionIsDirty = false;
                LogOnDisk = true;
                logName = saveResultTuple.Item2;
                setWindowTitle();
                UpdateStatusAsync("Save Successful!");
            }
        }

        private ICommand _showAboutCommand;
        public ICommand ShowAboutCommand
        {
            get
            {
                if (_showAboutCommand == null)
                {
                    _showAboutCommand = new DelegateCommand<object>(
                    param => ShowAbout(),
                    param => true //Add error checking here you peasant
                    );
                }
                return _showAboutCommand;
            }
        }

        private void ShowAbout()
        {
            AboutWindow dlg = new AboutWindow((Application.Current.MainWindow.ActualWidth / 3.3), (Application.Current.MainWindow.ActualHeight / 2));
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
        }

        private ICommand _showPreferencesCommand;
        public ICommand ShowPreferencesCommand
        {
            get
            {
                if (_showPreferencesCommand == null)
                {
                    _showPreferencesCommand = new DelegateCommand<object>(
                        param => ShowPreferences(),
                        param => true // Error checking
                    );
                }
                return _showPreferencesCommand;
            }
        }

        private void ShowPreferences()
        {
            OptionsWindow dlg = new OptionsWindow((Application.Current.MainWindow.ActualWidth / 3.3), (Application.Current.MainWindow.ActualHeight / 2));
            dlg.Owner = Application.Current.MainWindow;
            dlg.ShowDialog();
        }

        private ICommand _showLogEntryPropertiesCommand;
        public ICommand ShowLogEntryPropertiesCommand
        {
            get
            {
                if (_showLogEntryPropertiesCommand == null)
                {
                    _showLogEntryPropertiesCommand = new DelegateCommand<LogEntryDataModel>(
                        param => ShowLogEntryProperties(param),
                        param => true
                    );
                }
                return _showLogEntryPropertiesCommand;
            }
        }

        private void ShowLogEntryProperties(LogEntryDataModel aLogEntry)
        {
            if (aLogEntry != null)
            {
                // Display LogEntry Properties Window
                LogEntryPropertiesWindow dlg = new LogEntryPropertiesWindow((Application.Current.MainWindow.ActualWidth / 3.3), (Application.Current.MainWindow.ActualHeight / 2), aLogEntry);
                dlg.Owner = Application.Current.MainWindow;
                dlg.ShowDialog();

                if (!dlg.cancelled)
                {
                    aLogEntry.Date = dlg.Date;
                    aLogEntry.EntryTitle = dlg.EntryTitle;
                    aLogEntry.Type = dlg.Type;
                    CvsLogEntries.View.Refresh();
                }
            }
        }

        #endregion

        #region Helpers

        private void setWindowTitle()
        {
            WindowTitle = "SSLog " + version + " (" + logName + ")";
        }

        #endregion

        #region EventHandling

        private void ApplyFilter(object sender, FilterEventArgs e)
        {
            LogEntryDataModel ldm = (LogEntryDataModel)e.Item;

            if (string.IsNullOrWhiteSpace(Filter) || Filter.Length == 0)
            {
                e.Accepted = true;
            }
            else
            {
                e.Accepted = ldm.Date.ToString().Contains(Filter) || ldm.Type.Contains(Filter) || ldm.EntryTitle.Contains(Filter);
            }
        }

        private void LogEntriesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
                foreach (LogEntryDataModel oldItem in args.OldItems)
                    oldItem.PropertyChanged -= ItemChanged;

            if (args.NewItems != null)
                foreach (LogEntryDataModel newItem in args.NewItems)
                    newItem.PropertyChanged += ItemChanged;
        }

        private void ItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs args)
        {
            CollectionIsDirty = true;
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

        private void ClearStatusBarText(object source, ElapsedEventArgs e)
        {
            timer.Enabled = false;
            StatusBarText = "";
        }

        public void MainWindowClosing(object sender, CancelEventArgs e)
        {
            
            if (CollectionIsDirty && PromptToSave() == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
        }

        #endregion

        #region Asynchronous UI Stuff

        private Timer timer;

        private void UpdateStatusAsync(String statusText)
        {
            timer.Enabled = false;
            StatusBarText = statusText;
            timer.Enabled = true;
        }

        #endregion

        #region ListViewFilter

        private string _filter;
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                OnFilterChanged();
            }
        }

        private void OnFilterChanged()
        {
            CvsLogEntries.View.Refresh();
        }

        #endregion
    }
}
