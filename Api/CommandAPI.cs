using bedrock.NetHub.Service;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace bedrock.NetHub.Api
{
    public abstract class CommandAPI
    {
        public static void Register(HttpListenerContext context)
        {
            try
            {
                
                JObject ReqJSON = Http.ReadRequest(context);
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("commandName"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    bool status = Program.GetCommandManager().RegisterCommand(
                        ReqJSON["namespace"].Value<string>(),
                        ReqJSON["commandName"].Value<string>(),
                        ReqJSON.ContainsKey("permission") ? ReqJSON["permission"].Value<string>() : ""
                        );
                    Http.WriteRequest(context, status ? 200 : 400, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void Submit(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("playerId") || !ReqJSON.ContainsKey("playerName") || !ReqJSON.ContainsKey("commandString"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    Program.stdhubLOGGER.Info(string.Format("Player {0} attempts to call plugin command {1}", ReqJSON["playerName"].Value<string>(), ReqJSON["commandString"].Value<string>()));
                    string Xuid = Program.GetXuidManager().GetXuidByName(ReqJSON["playerName"].Value<string>());
                    int status = Program.GetCommandManager().TriggerCommand(
                        ReqJSON["commandString"].Value<string>(),
                        ReqJSON.ContainsKey("playerIsOp") && ReqJSON["playerIsOp"].Value<bool>(),
                        ReqJSON["playerId"].Value<string>(),
                        Xuid
                        );
                    Http.WriteRequest(context, status, "{}");
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }
    }
}
