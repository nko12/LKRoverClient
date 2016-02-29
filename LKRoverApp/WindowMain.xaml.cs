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
using XInputDotNetPure;

namespace LKRoverApp
{
    /// <summary>
    /// The main window to connect and control the mining robot.
    /// </summary>
    public partial class WindowMain : Window
    {
        xbox_controller xbox = new xbox_controller();
               

        public WindowMain()
        {
            
            InitializeComponent();
            
        }

        private void MenuItemXboxDiagram_OnClick(object sender, RoutedEventArgs e)
        {
            WindowXboxDiagram win = new WindowXboxDiagram {Owner = this};
            win.ShowDialog();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {    
        
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            
           
        }
        
        private void test_controller()
        {
            xbox.buttons();
            if(xbox.xbox_a != ButtonState.Pressed)
            {

                textBox1.Background = Brushes.Red;
            }
            else
            {
                textBox1.Background = Brushes.Green;
            }
        }

    }

}

    public partial class xbox_controller
    {
        public void rumble_on()
        {
            GamePad.SetVibration(PlayerIndex.One, 1, 1);
        }
        
        public void rumble_off()
        {
            GamePad.SetVibration(PlayerIndex.One, 0, 0);
        }
        //variables for the xbox buttons (are either buttonstate.pressed  or buttonstate.released)
        public ButtonState xbox_a;
        public ButtonState xbox_b;
        public ButtonState xbox_x;
        public ButtonState xbox_y;
        public ButtonState xbox_start;
        public ButtonState xbox_back;
        public ButtonState xbox_home;
        public ButtonState xbox_rightbumper;
        public ButtonState xbox_leftbumper;
        public ButtonState left_joystick_press;
        public ButtonState right_joystick_press;
    
        public void buttons()
        {
        //retrieves the buttonstate of all buttons
        PlayerIndex player = PlayerIndex.One;

        xbox_a = GamePad.GetState(player).Buttons.A;
        xbox_b = GamePad.GetState(player).Buttons.B;
        xbox_x = GamePad.GetState(player).Buttons.X;
        xbox_y = GamePad.GetState(player).Buttons.Y;
        xbox_start = GamePad.GetState(player).Buttons.Start;
        xbox_back = GamePad.GetState(player).Buttons.Back;
        xbox_home = GamePad.GetState(player).Buttons.Guide;
        xbox_rightbumper = GamePad.GetState(player).Buttons.RightShoulder;
        xbox_leftbumper = GamePad.GetState(player).Buttons.LeftShoulder;
        var left_joystick_press = GamePad.GetState(player).Buttons.LeftStick;  //not values of axis (only pressed value)
        var right_joystick_press = GamePad.GetState(player).Buttons.RightStick; // not values of axis (only pressed value)
        }

}
