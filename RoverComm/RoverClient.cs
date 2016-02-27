using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Sockets;

namespace RoverComm
{
    /// <summary>
    /// The primary RoverClient class. This is used to start and stop communication with the robot, send and receive
    /// data to and from the robot, and send commands to the robot.
    /// </summary>
    public class RoverClient
    {
        #region Class Variables
        /// <summary>
        /// The primary TCP client to use for communication with robot server.
        /// </summary>
        private TcpClient client;

        #endregion

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RoverClient()
        {
         //Test Change by Reid h
        }
    }
}
