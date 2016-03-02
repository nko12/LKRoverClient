using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
            this.Loaded += WindowMain_Loaded;
        }

        private void WindowMain_Loaded(object sender, RoutedEventArgs e)
        {
            Thread controllerThread = new Thread(XboxControllerThread);
            controllerThread.Name = "Xbox Controller Thread";
            controllerThread.IsBackground = true;
            controllerThread.SetApartmentState(ApartmentState.STA);
            controllerThread.Start();

            
        }

        private void MenuItemXboxDiagram_OnClick(object sender, RoutedEventArgs e)
        {
            WindowXboxDiagram win = new WindowXboxDiagram { Owner = this };
            win.ShowDialog();
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
        
        private void XboxControllerThread()
        {
            GamePadState lastState = GamePad.GetState(PlayerIndex.One);
            while (true)
            {
                Thread.Sleep(20);

                GamePadState currentState = GamePad.GetState(PlayerIndex.One);
                //if (currentState.PacketNumber == lastState.PacketNumber)
                //    continue;

                //if statements to read the xbox buttons and preform an action
                if (currentState.Buttons.A == ButtonState.Pressed && lastState.Buttons.A == ButtonState.Released)
                {
                    //The button has been pressed.
                    InvokeSetControlBackColor(textBox1, Colors.Green);
                }
                else if(currentState.Buttons.A == ButtonState.Released && lastState.Buttons.A == ButtonState.Pressed)
                {
                    //The button has been released.
                    InvokeSetControlBackColor(textBox1, Colors.Red);
                }


                if(currentState.Buttons.B == ButtonState.Pressed && lastState.Buttons.B == ButtonState.Released)
                {
                    //The button has been pressed.
                    InvokeSetControlBackColor(textBox, Colors.Green);
                }
                else if(currentState.Buttons.B == ButtonState.Released && lastState.Buttons.B == ButtonState.Pressed)
                {
                    //The button has been released.
                    InvokeSetControlBackColor(textBox, Colors.Red);
                }


                if(currentState.Buttons.X == ButtonState.Pressed && lastState.Buttons.X == ButtonState.Released)
                {
                    //The button has been pressed.
                    InvokeSetControlBackColor(textBox2, Colors.Green);
                }
                else if(currentState.Buttons.X == ButtonState.Released && lastState.Buttons.X == ButtonState.Pressed)
                {
                    //The button has been released.
                    InvokeSetControlBackColor(textBox2, Colors.Red);
                }


                if(currentState.Buttons.Y == ButtonState.Pressed && lastState.Buttons.Y == ButtonState.Released)
                {
                    //The button has been pressed.
                    InvokeSetControlBackColor(textBox3, Colors.Green);
                }
                else if (currentState.Buttons.Y == ButtonState.Released && lastState.Buttons.Y == ButtonState.Pressed)
                {
                    //The button has been released.
                    InvokeSetControlBackColor(textBox3, Colors.Red);
                }


                if (currentState.Buttons.Start == ButtonState.Pressed && lastState.Buttons.Start == ButtonState.Released)
                {
                    //The button has been pressed.
                    
                }
                else if (currentState.Buttons.Start == ButtonState.Released && lastState.Buttons.Start == ButtonState.Pressed)
                {
                    //The button has been released.
                    
                }


                if (currentState.Buttons.Back == ButtonState.Pressed && lastState.Buttons.Back == ButtonState.Released)
                {
                    //The button has been pressed.

                }
                else if (currentState.Buttons.Back == ButtonState.Released && lastState.Buttons.Back == ButtonState.Pressed)
                {
                    //The button has been released.

                }


                if (currentState.Buttons.RightShoulder == ButtonState.Pressed && lastState.Buttons.RightShoulder == ButtonState.Released)
                {
                    //The button has been pressed.

                }
                else if (currentState.Buttons.RightShoulder == ButtonState.Released && lastState.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    //The button has been released.

                }


                if (currentState.Buttons.LeftShoulder == ButtonState.Pressed && lastState.Buttons.LeftShoulder == ButtonState.Released)
                {
                    //The button has been pressed.

                }
                else if (currentState.Buttons.LeftShoulder == ButtonState.Released && lastState.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    //The button has been released.

                }


                if (currentState.Buttons.Guide == ButtonState.Pressed && lastState.Buttons.Guide == ButtonState.Released)
                {
                    //The button has been pressed.

                }
                else if (currentState.Buttons.Guide == ButtonState.Released && lastState.Buttons.Guide == ButtonState.Pressed)
                {
                    //The button has been released.

                }

                //if joystick is pressed in, not movement of joystick
                if (currentState.Buttons.RightStick == ButtonState.Pressed && lastState.Buttons.RightStick == ButtonState.Released)
                {
                    //The button has been pressed.

                }
                else if (currentState.Buttons.RightStick == ButtonState.Released && lastState.Buttons.RightStick == ButtonState.Pressed)
                {
                    //The button has been released.

                }

                //if joystick is pressed in, not movement of joystick
                if (currentState.Buttons.LeftStick == ButtonState.Pressed && lastState.Buttons.LeftStick == ButtonState.Released)
                {
                    //The button has been pressed.

                }
                else if (currentState.Buttons.LeftStick == ButtonState.Released && lastState.Buttons.LeftStick == ButtonState.Pressed)
                {
                    //The button has been released.

                }

                //Dpad inputs to return Dpad press values of up, down, left and right
                if (currentState.DPad.Up == ButtonState.Pressed )
                {
                    InvokeSetTextboxText(textBox4, "up");
                }else if(currentState.DPad.Down == ButtonState.Pressed)
                {
                    InvokeSetTextboxText(textBox4, "down");
                }else if (currentState.DPad.Left == ButtonState.Pressed)
                {
                    InvokeSetTextboxText(textBox4, "left");
                }else if(currentState.DPad.Right == ButtonState.Pressed)
                {
                    InvokeSetTextboxText(textBox4, "right");
                }
                else
                {
                    InvokeSetTextboxText(textBox4, "Dpad");
                }

                //left joystick values
                if(currentState.ThumbSticks.Left.X != 0 || currentState.ThumbSticks.Left.Y != 0)
                {
                    float left_x;
                    float left_y;
                    left_x = currentState.ThumbSticks.Left.X;
                    left_y = currentState.ThumbSticks.Left.Y;
                    InvokeSetTextboxText(textBox6, "X: " + left_x);
                    InvokeSetTextboxText(textBox7, "Y: " + left_y);
                }
                else
                {
                    InvokeSetTextboxText(textBox6, "X: 0");
                    InvokeSetTextboxText(textBox7, "Y: 0");
                }
                
                //right joystick values
                if(currentState.ThumbSticks.Right.X != 0 || currentState.ThumbSticks.Right.Y != 0)
                {
                    float right_x;
                    float right_y;
                    right_x = currentState.ThumbSticks.Right.X;
                    right_y = currentState.ThumbSticks.Right.Y;
                    InvokeSetTextboxText(textBox9, "X: " + right_x);
                    InvokeSetTextboxText(textBox10, "Y: " + right_y);

                }
                else
                {
                    InvokeSetTextboxText(textBox9, "X: 0");
                    InvokeSetTextboxText(textBox10, "Y: 0");
                }

                //trigger values
                if(currentState.Triggers.Left != 0 || currentState.Triggers.Right != 0)
                {
                    float right_trigger;
                    float left_trigger;
                    right_trigger = currentState.Triggers.Right;
                    left_trigger = currentState.Triggers.Left;
                    InvokeSetTextboxText(textBox11, "Right Trigger: " + right_trigger);
                    InvokeSetTextboxText(textBox12, "Left Trigger: " + left_trigger);
                }
                else
                {
                    InvokeSetTextboxText(textBox11, "Right Trigger: 0");
                    InvokeSetTextboxText(textBox12, "Left Trigger: 0");
                }


                lastState = currentState;
            }
        }

        private void InvokeSetControlBackColor(Control c, Color color)
        {
            c.Dispatcher.Invoke(delegate()
            {
                c.Background = new SolidColorBrush(color);
            });
        }

        private void InvokeSetTextboxText(TextBox c, string message)
        {

            try {
                c.Dispatcher.Invoke(delegate ()
                {
                    c.Text = message;
                });
            }
            catch
            {
              //prevents error message when main window is closed as it will not run code when main window is closed  
            }
        }

        private void textBox8_TextChanged(object sender, TextChangedEventArgs e)
        {
        
        }
    }

}

public class xbox_controller
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
        var gamePadState = GamePad.GetState(player);
        
        xbox_a = gamePadState.Buttons.A;
        xbox_b = gamePadState.Buttons.B;
        xbox_x = gamePadState.Buttons.X;
        xbox_y = gamePadState.Buttons.Y;
        xbox_start = gamePadState.Buttons.Start;
        xbox_back = gamePadState.Buttons.Back;
        xbox_home = gamePadState.Buttons.Guide;
        xbox_rightbumper = gamePadState.Buttons.RightShoulder;
        xbox_leftbumper = gamePadState.Buttons.LeftShoulder;
        var left_joystick_press = gamePadState.Buttons.LeftStick;  //not values of axis (only pressed value)
        var right_joystick_press = gamePadState.Buttons.RightStick; // not values of axis (only pressed value)
    }

}
