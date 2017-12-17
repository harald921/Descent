using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Descent_Server
{
    class Application
    {
        const string _appName = "Descent_Server";
        static bool isRunning = true;

        public static World _world;

        // Constructor
        public Application()
        {
            Network _network = new Network(_appName);
            _world = new World();

            while (isRunning)
            {
                _network.ProcessMessages();     // Check for Incoming message

                // Logic();

                // Send State
            }
        }

        public static void Quit()
        {
            isRunning = false;
        }
    }
}