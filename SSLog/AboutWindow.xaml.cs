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
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        public AboutWindow(double width, double height)
        {
            InitializeComponent();

            double xDiff = this.ActualWidth - controlGrid.ActualWidth;
            double yDiff = this.ActualHeight - controlGrid.ActualHeight;

            this.Width = width;
            this.Height = height;
        }
    }
}
