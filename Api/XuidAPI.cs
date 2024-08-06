using bedrock.NetHub.Utils;
using bedrock.NetHub.Service;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace bedrock.NetHub.Api
{
    public abstract class XuidAPI
    {
        public static void GetXuidByName(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("name"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string xuid = Program.GetXuidManager().GetXuidByName(ReqJSON["name"].Value<string>());
                    Http.WriteRequest(context, 200, new { xuid });
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

        public static void GetNameByXuid(HttpListenerContext context)
        {
            try
            {
                JObject ReqJSON = Http.ReadRequest(context);
                if (ReqJSON == null || !ReqJSON.ContainsKey("xuid"))
                {
                    Http.WriteRequest(context, 400, "{}");
                }
                else
                {
                    string name = Program.GetXuidManager().GetNameByXuid(ReqJSON["xuid"].Value<string>());
                    Http.WriteRequest(context, 200, new { name });
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
