using System;
using System.Collections.Generic;
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

namespace SSLog
{
    /// <summary>
    /// Interaction logic for OptionsWindow.xaml
    /// </summary>
    public partial class OptionsWindow : Window
    {
        Properties.Settings settings = Properties.Settings.Default;
        public OptionsWindow()
        {
            InitializeComponent();
            loadSettings();
        }

        public OptionsWindow(double width, double height)
        {

            InitializeComponent();
            // Set settings based on saved settings.
            loadSettings();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            // Set all settings
            settings.saveOnExit = chkSaveOnExit.IsChecked.Value;
            settings.startMaximized = chkStartMaximized.IsChecked.Value;
            settings.confirmOnDelete = chkConfirmOnDelete.IsChecked.Value;
            settings.rememberLastUsedType = chkRememberType.IsChecked.Value;
            // Wipe the last used type if this is set to false when pressing OK.
            if (chkRememberType.IsChecked.Value == false)
            {
                settings.lastUsedType = String.Empty;
            }
            settings.Save();
            Close();
        }

        private void loadSettings()
        {
            chkSaveOnExit.IsChecked = settings.saveOnExit;
            chkStartMaximized.IsChecked = settings.startMaximized;
            chkConfirmOnDelete.IsChecked = settings.confirmOnDelete;
            chkRememberType.IsChecked = settings.rememberLastUsedType;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e)
        {
            settings.Reset();
            loadSettings();
        }
    }
}
