using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bedrock.NetHub.Event
{
    public class CommandDispatchEvent :ScriptEvent
    {
        public readonly string eventName = "CommandDispatchEvent";

        public CommandDispatchEvent(string commandString,string playerId = null) : base() {}

    }
}
