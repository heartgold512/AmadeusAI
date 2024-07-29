using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace AmadeusAI.Behaviours
{
    internal class InstanceListener : IDisposable
    {
        private readonly Action<string> listenerAction;

        public InstanceListener(Action<string> actionToPerform)
        {

           // listenerAction = actionToPerform ?? throw new ArgumentNullException(nameof(actionToPerform));
        }

        public void Listen(string message)
        {
            // Perform the desired action when an event occurs
            listenerAction.Invoke(message);
        }

        public void Dispose()
        {
            // Clean up resources if needed
        }
    }
}

