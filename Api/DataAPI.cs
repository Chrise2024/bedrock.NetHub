using bedrock.NetHub.Utils;
using bedrock.NetHub.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class DataAPI
    {
        public static void Read(HttpListenerContext context)
        {
            try
            {
                
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    if (!File.Exists(dataFilePath))
                    {
                        Http.WriteRequest(context, 404, "{}");
                    }
                    else
                    {
                        Http.WriteRequest(context, 200, FileIO.ReadFile(dataFilePath));
                    }
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }

        public static void Write(HttpListenerContext context)
        {
            try
            {
                
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath") || !ReqJSON.ContainsKey("data"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    FileIO.EnsureFile(dataFilePath);
                    FileIO.WriteFile(dataFilePath,ReqJSON["data"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
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

        public static void Delete(HttpListenerContext context)
        {
            try
            {
                
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("subDataPath"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string dataFilePath = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>(), "data", ReqJSON["subDataPath"].Value<string>());
                    string dataFileDirname = Path.GetDirectoryName(dataFilePath);
                    if (!File.Exists(dataFilePath))
                    {
                        Http.WriteRequest(context, 404, "{}");
                    }
                    else
                    {
                        File.Delete(dataFilePath);
                        Http.WriteRequest(context, 200, "{}");
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
