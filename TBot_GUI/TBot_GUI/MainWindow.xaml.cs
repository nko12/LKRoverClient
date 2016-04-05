using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LKXbobxController;

namespace TBot_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Global TBot SerialPort can be accessed by all methods
        private SerialPort TBot = new SerialPort();
        private LKXbobxController.XboxControllerListener _xlistener;

        public MainWindow()
        {
            InitializeComponent();
        }


        ///////////////////////////// SETUP ////////////////////////////////////////////////////////////////////////////
        private void buttonConnect_Click(object sender, RoutedEventArgs e)
        {
            //No COM Port Selected
            if (textBoxCOM.Text == null)
            {
                textBoxStatus.AppendText("-No COM Port Selected\n");
            }

            //Invalid COM port provided
            else if (!Regex.IsMatch(textBoxCOM.Text, @"^COM\d{1,256}$"))
            {
                textBoxStatus.AppendText("-Invalid Format. Enter as: COM#\n");
            }

            else
            {
                try
                {
                    textBoxStatus.AppendText("-Connecting to " + textBoxCOM.Text + "...\n");

                    //Define the properties for the Serial Port
                    TBot.PortName = textBoxCOM.Text;
                    TBot.BaudRate = 9600;
                    TBot.DataBits = 8;
                    TBot.StopBits = StopBits.One;
                    TBot.Parity = Parity.None;
                    TBot.ReadTimeout = 500;
                    TBot.ReceivedBytesThreshold = 1;

                    //Open the Connection
                    TBot.Open();
                    textBoxStatus.AppendText("-Connection Success\n");

                    //Basic Control Flow
                    buttonConnect.IsEnabled = false;
                    buttonDisconnect.IsEnabled = true;
                    enableControls(true);
                }

                catch (Exception)
                {
                    textBoxStatus.AppendText("-Connection FAIL\n");
                }
            }
        }

        private void buttonDisconnect_Click(object sender, RoutedEventArgs e)
        {
            if (TBot.IsOpen)
            {
                TBot.Close();
            }
            buttonDisconnect.IsEnabled = false;
            buttonConnect.IsEnabled = true;
            enableControls(false);
        }


        ////////////////////// CONTROLS /////////////////////////////////////////////////////////////////////////
        private void buttonForward_Click(object sender, RoutedEventArgs e)
        {
            textBoxStatus.AppendText("-Forward\n");
            TBot.Write("F");
        }

        private void buttonLeft_Click(object sender, RoutedEventArgs e)
        {
            textBoxStatus.AppendText("-Left\n");
            TBot.Write("P");
        }

        private void buttonReverse_Click(object sender, RoutedEventArgs e)
        {
            textBoxStatus.AppendText("-Reverse\n");
            TBot.Write("R");
        }

        private void buttonRight_Click(object sender, RoutedEventArgs e)
        {
            textBoxStatus.AppendText("-Right\n");
            TBot.Write("S");
        }

        private void buttonStop_Click(object sender, RoutedEventArgs e)
        {
            textBoxStatus.AppendText("-Stop\n");
            TBot.Write("E");
        }


        /////////////// HELPERS & FORMATTERS //////////////////////////////////////////////////////////////////////////

        private void enableControls(bool x)
        {
            buttonForward.IsEnabled = x;
            buttonReverse.IsEnabled = x;
            buttonLeft.IsEnabled = x;
            buttonRight.IsEnabled = x;
            buttonStop.IsEnabled = x;
        }

        private void textBoxCOM_GotFocus(object sender, RoutedEventArgs e)
        {
            textBoxCOM.Text = "";
        }

        private void TBot_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            //Does not seem to function at the moment. Expected 
            // functionality would be for this to autoexecute
            // when serial data is received from TBot

        }

        private void windowMain_Loaded(object sender, RoutedEventArgs e)
        {
            _xlistener = new XboxControllerListener(Dispatcher);
            _xlistener.ListeningSleepDelay = 30;
            _xlistener.ButtonPressed += _xlistener_ButtonPressed;
            _xlistener.ButtonReleased += _xlistener_ButtonReleased;
            _xlistener.StartControllerThread();
        }

        private void _xlistener_ButtonReleased(object sender, ButtonEventArgs e)
        {
            //If any button is released, this means the T-bot should start.
            textBoxStatus.AppendText("-Stop\n");
            TBot.Write("E");
        }

        private void _xlistener_ButtonPressed(object sender, ButtonEventArgs e)
        {
            //Check which button has been pressed.
            switch (e.AffectedButton)
            {
                case ControllerButtons.DPadUp:
                    textBoxStatus.AppendText("-Forward\n");
                    TBot.Write("F");
                    break;
                case ControllerButtons.DPadDown:
                    textBoxStatus.AppendText("-Reverse\n");
                    TBot.Write("R");
                    break;
                case ControllerButtons.DPadLeft:
                    textBoxStatus.AppendText("-Left\n");
                    TBot.Write("P");
                    break;

                case ControllerButtons.DPadRight:
                    textBoxStatus.AppendText("-Right\n");
                    TBot.Write("S");
                    break;
            }
        }
    }
}
