using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SSLog
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml. This is the View class in our MVVM.
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            // Is all thats needed to change to new log a change of datacontext?
            // If so will wpf refresh the listview?
            this.DataContext = new LogViewModel();

            InitializeComponent();
            startup();
        }

        public MainWindow(String filepath)
        {
            this.DataContext = new LogViewModel(filepath);
            InitializeComponent();
            startup();
            logListView.SelectedIndex = 0;
        }

        #region Startup
        // TODO: Load blank log
        // TODO: Initialize options

        private void startup(String filepath)
        {
            startup();
        }

        private void startup()
        {
            sortListViewByDate();
            // Create a new, blank log.
            // Load it, update UI. Would WPF update UI by itself?
                    FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            if (Properties.Settings.Default.startMaximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            // If an SSLog filename argument was passed in, then we open it.
        }

        #endregion

        // Is this cheating? Did i break MVVM or what?
        void MainWindowClosing(object sender, CancelEventArgs e)
        {
            LogViewModel lg = DataContext as LogViewModel;
            lg.MainWindowClosing(sender, e);
        }

        #region ListView

        // FOr now just sort by date. Custom sorting can be added later.
        private void sortListViewByDate()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(logListView.ItemsSource);
            view.SortDescriptions.Add(new SortDescription("Date", ListSortDirection.Descending));
        }

        #endregion

        #region Scaling
        public static readonly DependencyProperty ScaleValueProperty = DependencyProperty.Register("ScaleValue", typeof(double), typeof(MainWindow), 
            new UIPropertyMetadata(1.0, new PropertyChangedCallback(OnScaleValueChanged), new CoerceValueCallback(OnCoerceScaleValue)));

        private static void OnScaleValueChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.OnScaleValueChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        protected virtual void OnScaleValueChanged(double oldValue, double newValue)
        {

        }

        private static object OnCoerceScaleValue(DependencyObject o, object value)
        {
            MainWindow mainWindow = o as MainWindow;
            if (mainWindow != null)
            {
                return mainWindow.OnCoerceScaleValue((double)value);
            }
            else
            {
                return value;
            }
        }

        protected virtual double OnCoerceScaleValue(double value)
        {
            if (double.IsNaN(value))
                return 1.0f;

            value = Math.Max(0.1, value);
            return value;
        }

        public double ScaleValue
        {
            get
            {
                return (double)GetValue(ScaleValueProperty);
            }
            set
            {
                SetValue(ScaleValueProperty, value);
            }
        }

        private void MainGrid_SizeChanged(object sender, EventArgs e)
        {
            CalculateScale();
        }

        private void CalculateScale()
        {
            double yScale = ActualHeight / 488f;
            double xScale = ActualWidth / 1024f;
            double value = Math.Min(xScale, yScale);
            ScaleValue = (double)OnCoerceScaleValue(myMainWindow, value);
        }

        #endregion
    }
}
