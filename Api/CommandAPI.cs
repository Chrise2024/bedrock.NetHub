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
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("commandName"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    if (ReqJSON.ContainsKey("permission"))
                    {
                        writer.Write("{}");
                        bool status = Program.GetCommandManager().RegisterCommand(ReqJSON["namespace"].Value<string>(), ReqJSON["commandName"].Value<string>(), ReqJSON["permission"].Value<string>());
                        context.Response.StatusCode = status ? 200 : 400;
                    }
                    else
                    {
                        writer.Write("{}");
                        bool status = Program.GetCommandManager().RegisterCommand(ReqJSON["namespace"].Value<string>(), ReqJSON["commandName"].Value<string>());
                        context.Response.StatusCode = status ? 200 : 400;
                    }
                }
                writer.Close();
                context.Response.Close();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StreamWriter writer = new(context.Response.OutputStream);
                writer.Write("{}");
                context.Response.StatusCode = 400;
                writer.Close();
                context.Response.Close();
                return;
            }
        }

        public static void Submit(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("playerId") || !ReqJSON.ContainsKey("playerName") || !ReqJSON.ContainsKey("commandString"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    Program.stdhubLOGGER.Info(string.Format("Player {0} attempts to call plugin command {1}", ReqJSON["playerName"].Value<string>(), ReqJSON["commandString"].Value<string>()));
                    if (ReqJSON.ContainsKey("playerIsOp"))
                    {
                        string Xuid = Program.GetXuidManager().GetXuidByName(ReqJSON["playerName"].Value<string>());
                        int status = Program.GetCommandManager().TriggerCommand(
                            ReqJSON["commandString"].Value<string>(),
                            ReqJSON["playerIsOp"].Value<bool>(),
                            ReqJSON["playerId"].Value<string>(),
                            Xuid
                            );
                        writer.Write("{}");
                        context.Response.StatusCode = status;
                    }
                    else
                    {
                        string Xuid = Program.GetXuidManager().GetXuidByName(ReqJSON["playerName"].Value<string>());
                        int status = Program.GetCommandManager().TriggerCommand(
                            ReqJSON["commandString"].Value<string>(),
                            false,
                            ReqJSON["playerId"].Value<string>(),
                            Xuid
                            );
                        writer.Write("{}");
                        context.Response.StatusCode = status;
                    }
                }
                writer.Close();
                context.Response.Close();
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                StreamWriter writer = new(context.Response.OutputStream);
                writer.Write("{}");
                context.Response.StatusCode = 400;
                writer.Close();
                context.Response.Close();
                return;
            }
        }
    }
}
