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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LKRoverApp
{
    /// <summary>
    /// The main window to connect and control the mining robot.
    /// </summary>
    public partial class WindowMain : Window
    {
        public WindowMain()
        {
            InitializeComponent();
        }

        private void MenuItemXboxDiagram_OnClick(object sender, RoutedEventArgs e)
        {
            WindowXboxDiagram win = new WindowXboxDiagram {Owner = this};
            win.ShowDialog();
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
