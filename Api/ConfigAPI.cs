using bedrock.NetHub.Utils;
using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class ConfigAPI
    {
        public static void Read(HttpListenerContext context)
        {
            try
            {
                StreamReader sw = new(context.Request.InputStream);
                JObject ReqJSON = JObject.Parse(sw.ReadToEnd());
                context.Response.ContentType = "text/plain;charset=UTF-8";
                context.Response.AddHeader("Content-type", "text/plain");
                context.Response.ContentEncoding = Encoding.UTF8;
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("defaults"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    return;
                }
                else
                {
                    string pluginRoot = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>());
                    string configFilePath = ReqJSON.ContainsKey("subConfigName") ? Path.Join(pluginRoot, ReqJSON["subConfigName"].Value<string>() + ".yaml") : Path.Join(pluginRoot, "config.json");
                    if (!File.Exists(configFilePath))
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        return;
                    }
                    Stream stream = context.Response.OutputStream;
                    byte[] returnByteArr = Encoding.UTF8.GetBytes(FileIO.ReadFile(configFilePath));
                    stream.Write(returnByteArr, 0, returnByteArr.Length);
                    context.Response.StatusCode = 200;
                    context.Response.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                context.Response.StatusCode = 400;
                context.Response.Close();
                return;
            }
        }
    }
}
