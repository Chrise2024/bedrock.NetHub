using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bedrock.NetHub.Event
{
    public class CommandDispatchEvent(string commandString, string playerId = null) : ScriptEvent()
    {
        public new readonly string eventName = "CommandDispatchEvent";

        public readonly string commandString = commandString;

        public readonly string playerId = playerId;
    }
}
