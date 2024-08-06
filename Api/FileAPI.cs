using bedrock.NetHub.Utils;
using bedrock.NetHub.Service;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json.Linq;
using bedrock.NetHub.service;

namespace bedrock.NetHub.Api
{
    public abstract class FileAPI
    {
        public static void Read(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("path") || !ReqJSON.ContainsKey("response"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string tPath = ReqJSON["path"].Value<string>();
                    string respType = ReqJSON["response"].Value<string>();
                    if (!File.Exists(tPath))
                    {
                        Http.WriteRequest(context, 404, "{}");
                    }
                    else
                    {
                        if (respType.Equals("text"))
                        {
                            Http.WriteRequest(context, 200, new { result = FileIO.ReadFile(tPath) });
                        }
                        else if (respType.Equals("bytes"))
                        {
                            Http.WriteRequest(context, 200, new { result = FileIO.ReadAsBytes(tPath) });
                        }
                        else
                        {
                            Http.WriteRequest(context, 404, "{}");
                        }
                    }
                }
                return;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Method: " + ex.TargetSite.Name + " run error.");
                Http.WriteRequest(context, 400, "{}");
                return;
            }
        }
    }
}
