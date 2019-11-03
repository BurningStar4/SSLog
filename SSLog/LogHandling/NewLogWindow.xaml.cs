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
    public partial class NewLogWindow : Window
    {
        #region Constructor
        
        // DO NOT USE!
        public NewLogWindow()
        {
            InitializeComponent();
        }

        // Use this one please
        public NewLogWindow(double width, double height)
        {
            // Set window size
            // Initialise
            InitializeComponent();

            this.Width = width;
            this.Height = height;

            // Check last used type
            if (Properties.Settings.Default.rememberLastUsedType && !String.IsNullOrWhiteSpace(Properties.Settings.Default.lastUsedType))
            {
                cbxType.Text = Properties.Settings.Default.lastUsedType;
            }
            txtTitle.Focus();
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
            cancelled = false;
            if (Properties.Settings.Default.rememberLastUsedType)
            {
                Properties.Settings.Default.lastUsedType = Type;
                Properties.Settings.Default.Save();
            }
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
