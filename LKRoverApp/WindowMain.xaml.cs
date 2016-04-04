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
    public partial class WindowMain
    {
        private XboxControllerListener _xListener;

        public WindowMain()
        {
            InitializeComponent();
        }

        private void MenuItemXboxDiagram_OnClick(object sender, RoutedEventArgs e)
        {
            WindowXboxDiagram win = new WindowXboxDiagram { Owner = this };
            win.ShowDialog();
        }

        private void menuItemExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void WindowMain_OnLoaded(object sender, RoutedEventArgs e)
        {
            _xListener = new XboxControllerListener(Dispatcher);
            _xListener.ButtonPressed += _xListener_ButtonPressed;
            _xListener.ButtonReleased += _xListener_ButtonReleased;
            _xListener.TriggerMoved += _xListener_TriggerMoved;
            _xListener.ThumbstickMoved += _xListener_ThumbstickMoved;
            _xListener.StartControllerThread();
        }

        private void _xListener_ThumbstickMoved(object sender, ThumbAxisEventArgs e)
        {
            if(e.Side == ControllerSide.Left)
                Console.WriteLine(@"Left Thumbstick Moved: ({0},{1})", e.XValue, e.YValue);
            else if (e.Side == ControllerSide.Right)
                Console.WriteLine(@"Right Thumbstick Moved: ({0},{1})", e.XValue, e.YValue);
        }

        private void _xListener_TriggerMoved(object sender, TriggerAxisEventArgs e)
        {
            if(e.Side == ControllerSide.Left)
                Console.WriteLine(@"Left Trigger Moved: " + e.ZValue);
            else if(e.Side == ControllerSide.Right)
                Console.WriteLine(@"Right Trigger Moved: " + e.ZValue);
        }

        private void _xListener_ButtonReleased(object sender, ButtonEventArgs e)
        {
            switch (e.AffectedButton)
            {
                case ControllerButtons.A:
                    gridMain.Background = new SolidColorBrush(Colors.Orange);
                    break;
            }
        }

        private void _xListener_ButtonPressed(object sender, ButtonEventArgs e)
        {
            switch (e.AffectedButton)
            {
                case ControllerButtons.A:
                    gridMain.Background = new SolidColorBrush(Colors.Aquamarine);
                    break;
            }
        }

        private void WindowMain_OnClosed(object sender, EventArgs e)
        {
            _xListener.StopControllerThread();
        }
    }
}
