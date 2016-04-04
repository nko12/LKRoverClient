using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Threading;
using XInputDotNetPure;

namespace LKRoverApp
{
    /// <summary>
    /// Represents the buttons on a controller.
    /// </summary>
    public enum ControllerButtons
    {
        A,
        B,
        X,
        Y,
        LeftShoulder,
        RightShoulder,
        LeftStick,
        RightStick,
        Start,
        Back,
        Guide,
        DPadUp,
        DPadDown,
        DPadLeft,
        DPadRight
    }

    /// <summary>
    /// Represents the left or right side of the controller.
    /// </summary>
    public enum ControllerSide
    {
        /// <summary>
        /// Left side of the controller.
        /// </summary>
        Left,
        /// <summary>
        /// Right side of the controller.
        /// </summary>
        Right
    }

    /// <summary>
    /// EventArgs associated with the controller's buttons.
    /// </summary>
    public class ButtonEventArgs : EventArgs
    {
        /// <summary>
        /// The button that was affected in the event.
        /// </summary>
        public ControllerButtons AffectedButton { get; set; }
    }

    /// <summary>
    /// EventArgs associated with the controller's left and right thumbsticks.
    /// </summary>
    public class ThumbAxisEventArgs
    {
        /// <summary>
        /// Specifies whether this is the left or right thumbstick.
        /// </summary>
        public ControllerSide Side { get; set; }

        /// <summary>
        /// The value of the X axis. Note that this value has been constrained to a -100 to 100 range.
        /// </summary>
        public float XValue { get; set; }

        /// <summary>
        /// The value of the Y axis. Note that this value has been constrained to a -100 to 100 range.
        /// </summary>
        public float YValue { get; set; }
    }
    
    /// <summary>
    /// EventArgs associated with the controller's left and right triggers.
    /// </summary>
    public class TriggerAxisEventArgs
    {
        /// <summary>
        /// Specifies whether this is the left or right thumbstick.
        /// </summary>
        public ControllerSide Side { get; set; }

        /// <summary>
        /// The value of the trigger axis (sometimes considered Z axis). Note that this value has been constrained
        /// to a 0-100 range.
        /// </summary>
        public float ZValue { get; set; }
    }

    /// <summary>
    /// Listens to changes in attatched Xbox Controllers on a separate thread, and invokes events on the UI thread.
    /// </summary>
    public class XboxControllerListener
    {
        #region Constants
        //The minimum value of the Xbox 360 Controller's thumbstick range.
        private const float ThumbstickStandardRangeMin = -1;
        //The minimum value of the more convient "100" range.
        private const float Thumbstick100RangeMin = -100;
        //The full range size of the Xbox 360 Controller thumbsticks.
        private const float ThumbstickStandardRange = 1 - (ThumbstickStandardRangeMin);
        //The full range size of the convient "100" range.
        private const float Thumbstick100Range = 100 - (Thumbstick100RangeMin);

        //The minimum value of the Xbox 360 Controller's thumbstick range.
        private const float TriggerStandardRangeMin = 0;
        //The minimum value of the more convient "100" range.
        private const float Trigger100RangeMin = 0;
        //The full range size of the Xbox 360 Controller thumbsticks.
        private const float TriggerStandardRange = 1 - (TriggerStandardRangeMin);
        //The full range size of the convient "100" range.
        private const float Trigger100Range = 100 - (Trigger100RangeMin);

        #endregion

        #region Variables

        //The dispatcher to invoke events on.
        private Dispatcher _appDispatcher;
        //The thread that runs the listening code.
        private Thread _listenerThread;
        //A variable that stores the delay the listening thread will use between each game pad state check.
        private int _listenerSleepDelay;

        #endregion

        #region Public Delegates and Events

        /// <summary>
        /// Delegate defining the function prototype for controller button events.
        /// </summary>
        /// <param name="sender">The object instance that initiated the event.</param>
        /// <param name="e">The controller button event args associated with the event.</param>
        public delegate void OnControllerButtonEvent(object sender, ButtonEventArgs e);
        /// <summary>
        /// Delegate defining the function prototype for controller thumbstick axis events.
        /// </summary>
        /// <param name="sender">The object instance that initiated the event.</param>
        /// <param name="e">The controller thumbstick axis event args associated with the event.</param>
        public delegate void OnControllerAxisEvent(object sender, ThumbAxisEventArgs e);
        /// <summary>
        /// Delegate defining the function prototype for controller trigger axis events.
        /// </summary>
        /// <param name="sender">The object instance that initiated the event.</param>
        /// <param name="e">The controller trigger axis event args associated with the event.</param>
        public delegate void OnControllerTriggerEvent(object sender, TriggerAxisEventArgs e);

        /// <summary>
        /// Occurs when a controller button is pressed.
        /// </summary>
        public event OnControllerButtonEvent ButtonPressed;
        /// <summary>
        /// Occurs when a controller button is released.
        /// </summary>
        public event OnControllerButtonEvent ButtonReleased;

        /// <summary>
        /// Occurs when the left or right thumbstick is moved.
        /// </summary>
        public event OnControllerAxisEvent ThumbstickMoved;
        /// <summary>
        /// Occurs when the left or right trigger is moved.
        /// </summary>
        public event OnControllerTriggerEvent TriggerMoved;

        /// <summary>
        /// Occurs when the controller is disconnected, or the listening thread is started and never detects a
        /// controller.
        /// </summary>
        public event EventHandler ControllerDisconnected;
        /// <summary>
        /// Occurs when the controller is disconnected, but has been reconnected. Does not unless the controller was
        /// disconnected and then reconnected while the listening thread was running.
        /// </summary>
        public event EventHandler ControllerReconnected;

        #endregion

        #region Constructores and Destructors

        /// <summary>
        /// Creates a new XboxController using the specified dispatcher to invoke events on the UI thread.
        /// </summary>
        /// <param name="dispatcher">The WPF application dispatcher.</param>
        public XboxControllerListener(Dispatcher dispatcher)
        {
            _appDispatcher = dispatcher;
            IsListeningThreadRunning = false;
            _listenerSleepDelay = 50;
        }

        /// <summary>
        /// Destructor.
        /// </summary>
        ~XboxControllerListener()
        {
            IsListeningThreadRunning = false;
            //Wait for the thread to finish before letting this class finish disposing.
            _listenerThread.Join();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating if the thread listening for controller input is running.
        /// </summary>
        public bool IsListeningThreadRunning { get; private set; }

        /// <summary>
        /// Gets or sets the dispatcher the class will use when invoking events.
        /// </summary>
        public Dispatcher ApplicationDispatcher
        {
            get { return _appDispatcher; }
            set
            {
                if (IsListeningThreadRunning)
                {
                    //Stop the controller thread.
                    StopControllerThread();
                    //Change the dispatcher.
                    _appDispatcher = value;
                    //Restart the thread.
                    StartControllerThread();
                }
                else
                    _appDispatcher = value;
            }
        }

        /// <summary>
        /// Gets or sets the sleep delay the listener thread uses. This value is in milliseconds.
        /// </summary>
        public int ListeningSleepDelay
        {
            get { return _listenerSleepDelay; }
            set
            {
                if (IsListeningThreadRunning)
                {
                    //Stop the controller thread.
                    StopControllerThread();
                    //Change the sleep delay value.
                    _listenerSleepDelay = value;
                    //Restart the controller thread.
                    StartControllerThread();
                }
                else
                    _listenerSleepDelay = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the XboxController input listening thread.
        /// </summary>
        public void StartControllerThread()
        {
            //Do nothing if the listening thread is already running.
            if (IsListeningThreadRunning)
                return;
            //Create a thread for running the Xbox Controller checking code.
            _listenerThread = new Thread(ControllerListenerThread)
            {
                Name = "Xbox Controller Thread",
                IsBackground = true
            };
            _listenerThread.SetApartmentState(ApartmentState.STA);
            _listenerThread.Start();
            IsListeningThreadRunning = true;
        }

        /// <summary>
        /// Stops the XboxController input listening thread.
        /// </summary>
        public void StopControllerThread()
        {
            IsListeningThreadRunning = false;
            _listenerThread.Join();
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Represents the connection state of a controller. For use by the listening thread.
        /// </summary>
        private enum ConnectionState
        {
            /// <summary>
            /// The controller is connected.
            /// </summary>
            Connected,
            /// <summary>
            /// The controller has just become disconnected.
            /// </summary>
            InitDisconnect,
            /// <summary>
            /// The controller has remained disconnected for the duration of the disconnect timer.
            /// </summary>
            FullDisconnect
        }

        //The entry point to the thread that listens for changes in Xbox controllers attached to the computer.
        private void ControllerListenerThread()
        {
            //Stores the previous state of the gamepad for comparison against the new state.
            GamePadState lastState = GamePad.GetState(PlayerIndex.One);
            //A dictionary of each controller button and whether or not it has changed to pressed or released.
            Dictionary<ControllerButtons, bool?> buttonStates = new Dictionary<ControllerButtons, bool?>();

            // ReSharper disable once NotAccessedVariable
            Timer disconnectTimer = null;
            ConnectionState connState;
            if(lastState.IsConnected)
                connState = ConnectionState.Connected;
            else
                connState = ConnectionState.InitDisconnect;

            //While this separate worker thread is running...
            while (IsListeningThreadRunning)
            {
                //Sleep for whatever listener sleep delay is so the UI thread isn't flooded with events.
                Thread.Sleep(_listenerSleepDelay);

                //Get the current gamepad state.
                GamePadState currentState = GamePad.GetState(PlayerIndex.One);

                //Detect if the controller has been disconnected or was never connected since the listening thread
                //was started. This can save some troubleshooting headaches.
                switch (connState)
                {
                    case ConnectionState.Connected:
                        //Controller is connected since the last call to get state. Make sure it is still connected.
                        if (!currentState.IsConnected)
                        {
                            //Controller seems to have been disconnected.
                            connState = ConnectionState.InitDisconnect;

                            //Instantiate the disconnect timer. If more than 5 seconds passes since the controller was
                            //disconnected, the ControllerDisconnected event will be fired.
                            disconnectTimer = new Timer(delegate
                            {
                                //Execution here means 5 seconds have passed. If connState is still set to disconnected,
                                //the controller wasn't found in 5 seconds.
                                // ReSharper disable once AccessToModifiedClosure
                                if (connState == ConnectionState.InitDisconnect && ControllerDisconnected != null)
                                {
                                    connState = ConnectionState.FullDisconnect;
                                    ApplicationDispatcher.Invoke(
                                        delegate { ControllerDisconnected(this, EventArgs.Empty); });
                                }
                                
                                //After the timer executes, set its value back to null.
                                disconnectTimer = null;
                            }, null, 5000, Timeout.Infinite);
                        }
                        break;
                    case ConnectionState.InitDisconnect:
                        //The controller was disconnected since the last call to get state, and a timer thread has been
                        //initiated. Check if the controller has been found since then.
                        if (currentState.IsConnected)
                        {
                            //The controller was found again. Change connection state to connected so when the timer
                            //finishes, it won't fire the ControllerDisconnected event. Also note that I won't fire
                            //the ControllerReconnected event because the 5 seconds of disconnection did not pass. This
                            //is so clients are not alerted of a disconnect that was really just a small "hiccup" in
                            //the connection.
                            connState = ConnectionState.Connected;
                        }
                        //Controller has still not been found. Sleep again.
                        continue;
                    case ConnectionState.FullDisconnect:
                        //The controller was fully disconnected, and the ControllerDisconnected event was fired. Just
                        //keep checking for reconnection infinitely.
                        if (currentState.IsConnected)
                        {
                            //Controller reconnected.
                            connState = ConnectionState.Connected;
                            //Fire the ControllerReconnected event.
                            if (ControllerReconnected != null)
                            {
                                ApplicationDispatcher.Invoke(
                                    delegate { ControllerReconnected(this, EventArgs.Empty);});
                            }
                        }
                        else
                            continue; //Sleep again.
                        break;
                }

                //Check if the actual state of the controller has changed since the last call to GetState. If it hasn't
                //changed, make the thread sleep again. Additionally, if the controller isn't connected, just sleep.
                if (currentState.PacketNumber == lastState.PacketNumber)
                    continue;
                
                //Store the current pressed, released or no change values of each button in the dictionary. This helps
                //reduce the amount of repetitive, copy pasted code.
                buttonStates[ControllerButtons.A] = CompareButtonStates(lastState.Buttons.A, currentState.Buttons.A);
                buttonStates[ControllerButtons.B] = CompareButtonStates(lastState.Buttons.B, currentState.Buttons.B);
                buttonStates[ControllerButtons.X] = CompareButtonStates(lastState.Buttons.X, currentState.Buttons.X);
                buttonStates[ControllerButtons.Y] = CompareButtonStates(lastState.Buttons.Y, currentState.Buttons.Y);

                buttonStates[ControllerButtons.Start] = CompareButtonStates(lastState.Buttons.Start, currentState.Buttons.Start);
                buttonStates[ControllerButtons.Back] = CompareButtonStates(lastState.Buttons.Back, currentState.Buttons.Back);
                buttonStates[ControllerButtons.RightShoulder] = CompareButtonStates(lastState.Buttons.RightShoulder, currentState.Buttons.RightShoulder);
                buttonStates[ControllerButtons.LeftShoulder] = CompareButtonStates(lastState.Buttons.LeftShoulder, currentState.Buttons.LeftShoulder);

                buttonStates[ControllerButtons.Guide] = CompareButtonStates(lastState.Buttons.Guide, currentState.Buttons.Guide);
                buttonStates[ControllerButtons.RightStick] = CompareButtonStates(lastState.Buttons.RightStick, currentState.Buttons.RightStick);
                buttonStates[ControllerButtons.LeftStick] = CompareButtonStates(lastState.Buttons.LeftStick, currentState.Buttons.LeftStick);

                buttonStates[ControllerButtons.DPadUp] = CompareButtonStates(lastState.DPad.Up, currentState.DPad.Up);
                buttonStates[ControllerButtons.DPadDown] = CompareButtonStates(lastState.DPad.Down, currentState.DPad.Down);
                buttonStates[ControllerButtons.DPadLeft] = CompareButtonStates(lastState.DPad.Left, currentState.DPad.Left);
                buttonStates[ControllerButtons.DPadRight] = CompareButtonStates(lastState.DPad.Right, currentState.DPad.Right);

                //Loop through the dictionary calling the button pressed or released events as necessary for buttons.
                foreach (var keyValPair in buttonStates)
                {
                    //A value of true for the keyValPair means the button is pressed, false means it has been released,
                    //and null means there has been no change.
                    if (keyValPair.Value != null)
                        ExecuteButtonChangedEvent(keyValPair.Key, (bool)keyValPair.Value);
                }

                //Note that all deadzone and threshold calculations have been taken care of by the the XInput library
                //being used by our project.
                var prevLeftThumb = lastState.ThumbSticks.Left;
                var prevRightThumb = lastState.ThumbSticks.Right;
                var curLeftThumb = currentState.ThumbSticks.Left;
                var curRightThumb = currentState.ThumbSticks.Right;

                //Range change calculations from http://stackoverflow.com/questions/929103/convert-a-number-range-to-another-range-maintaining-ratio

                //Is the new left thumbstick value outside of the deadzone?
                //Has the left thumbstick moved?
                if (Math.Abs(curLeftThumb.X - prevLeftThumb.X) > float.Epsilon
                    || Math.Abs(curLeftThumb.Y - prevLeftThumb.Y) > float.Epsilon)
                {
                    if (ThumbstickMoved != null)
                    {
                        ThumbAxisEventArgs args = new ThumbAxisEventArgs();
                        //Change the thumbstick ranges from [-1, 1] to [-100, 100]
                        args.XValue = (((curLeftThumb.X - ThumbstickStandardRangeMin) * Thumbstick100Range) /
                                       ThumbstickStandardRange) + Thumbstick100RangeMin;
                        args.YValue = (((curLeftThumb.Y - ThumbstickStandardRangeMin) * Thumbstick100Range) /
                                       ThumbstickStandardRange) + Thumbstick100RangeMin;
                        args.Side = ControllerSide.Left;
                        ApplicationDispatcher.Invoke(delegate { ThumbstickMoved(this, args); });
                    }
                }

                //Has the right thumbstick moved?
                if (Math.Abs(curRightThumb.X - prevRightThumb.X) > float.Epsilon
                    || Math.Abs(curRightThumb.Y - prevRightThumb.Y) > float.Epsilon)
                {
                    if (ThumbstickMoved != null)
                    {
                        ThumbAxisEventArgs args = new ThumbAxisEventArgs();
                        //Change the thumbstick ranges from [-1, 1] to [-100, 100]
                        args.XValue = (((curRightThumb.X - ThumbstickStandardRangeMin) * Thumbstick100Range) /
                                       ThumbstickStandardRange) + Thumbstick100RangeMin;
                        args.YValue = (((curRightThumb.Y - ThumbstickStandardRangeMin) * Thumbstick100Range) /
                                       ThumbstickStandardRange) + Thumbstick100RangeMin;
                        args.Side = ControllerSide.Right;
                        ApplicationDispatcher.Invoke(delegate { ThumbstickMoved(this, args); });
                    }
                }

                //Has the left trigger changed?
                if (Math.Abs(currentState.Triggers.Left - lastState.Triggers.Left) > float.Epsilon)
                {
                    if (TriggerMoved != null)
                    {
                        TriggerAxisEventArgs args = new TriggerAxisEventArgs();
                        //Change the trigger range from [0, 1] to [0, 100].
                        args.Side = ControllerSide.Left;
                        args.ZValue = (((currentState.Triggers.Left - TriggerStandardRangeMin) * Trigger100Range) /
                                           TriggerStandardRange) + Trigger100RangeMin;
                        ApplicationDispatcher.Invoke(delegate { TriggerMoved(this, args); });
                    }
                }
                //Has the right trigger changed?
                if (Math.Abs(currentState.Triggers.Right - lastState.Triggers.Right) > float.Epsilon)
                {
                    if (TriggerMoved != null)
                    {
                        TriggerAxisEventArgs args = new TriggerAxisEventArgs();
                        //Change the trigger range from [0, 1] to [0, 100].
                        args.Side = ControllerSide.Right;
                        args.ZValue = (((currentState.Triggers.Right - TriggerStandardRangeMin) * Trigger100Range) /
                                           TriggerStandardRange) + Trigger100RangeMin;
                        ApplicationDispatcher.Invoke(delegate { TriggerMoved(this, args); });
                    }
                }

                lastState = currentState;
            }
        }

        //Helper method that executes the Pressed or Released event for a button on the UI thread.
        private void ExecuteButtonChangedEvent(ControllerButtons button, bool isPressed)
        {
            ButtonEventArgs eventArgs = new ButtonEventArgs() { AffectedButton = button };
            if(isPressed && ButtonPressed != null)
                ApplicationDispatcher.Invoke(delegate { ButtonPressed(this, eventArgs); });
            else if(!isPressed && ButtonReleased != null)
                ApplicationDispatcher.Invoke(delegate { ButtonReleased(this, eventArgs); });
        }

        //Helper method that determines if a button has been pressed, released or hasn't changed since the previous
        //controller state.
        private bool? CompareButtonStates(ButtonState oldState, ButtonState newState)
        {
            if (newState == ButtonState.Pressed && oldState == ButtonState.Released)
                return true;
            if (newState == ButtonState.Released && oldState == ButtonState.Pressed)
                return false;
            return null;
        }

        #endregion
    }
}
