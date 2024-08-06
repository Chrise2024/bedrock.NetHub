using bedrock.NetHub.Utils;
using bedrock.NetHub.Service;
using bedrock.NetHub.service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace bedrock.NetHub.Api
{
    public abstract class LogAPI
    {
        public static void Log(HttpListenerContext context)
        {
            try
            {
                
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("content"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    Logger.GeneralLog(ReqJSON["namespace"].Value<string>(), ReqJSON["content"].Value<string>());
                    Http.WriteRequest(context, 200, "{}");
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
