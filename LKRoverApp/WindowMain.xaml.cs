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

        private void buttonUCF_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("UCF Lunar Knights!");
        }

        private void buttonSpace_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wow, space is cool!", "Space Is Awesome", MessageBoxButton.OK, MessageBoxImage.Exclamation);
        }

        private void buttonBackground_Click(object sender, RoutedEventArgs e)
        {
            this.windowMain.Background = new SolidColorBrush(Colors.Gold);
        }

        private void buttonShowName_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Hi " + this.textBoxName.Text + "!");
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            this.windowMain.Background = new SolidColorBrush(Colors.Green);
        }
    }
}
