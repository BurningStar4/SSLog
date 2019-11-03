using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SSLog
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_Startup(object sender, StartupEventArgs e)
        {
            MainWindow mainWindow;
            if (e.Args.Length == 1)
            {
                mainWindow = new MainWindow(e.Args[0]);

            }
            else
            {
                mainWindow = new MainWindow();
            }
            mainWindow.Show();
        }
    }
}
