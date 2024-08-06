using bedrock.NetHub.Utils;
using bedrock.NetHub.Service;
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
                
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("defaults"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string pluginRoot = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>());
                    string configFilePath = ReqJSON.ContainsKey("subConfigName") ? Path.Join(pluginRoot, ReqJSON["subConfigName"].Value<string>() + ".json") : Path.Join(pluginRoot, "config.json");
                    if (!File.Exists(configFilePath))
                    {
                        FileIO.EnsureFile(configFilePath, ReqJSON["defaults"].Value<string>());
                        Http.WriteRequest(context,200, new { data = JsonConvert.SerializeObject(ReqJSON["defaults"]) });
                    }
                    else
                    {
                        JObject defaultConfig = ReqJSON["defaults"].Value<JObject>();
                        JObject readConfig = FileIO.ReadAsJSON(configFilePath);
                        defaultConfig.Merge(readConfig);
                        Http.WriteRequest(context, 200, new { data = JsonConvert.SerializeObject(defaultConfig) });
                    }
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
