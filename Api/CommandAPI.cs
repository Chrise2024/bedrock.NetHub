using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("commandName"))
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                    return;
                }
                else
                {
                    if (ReqJSON.ContainsKey("permission"))
                    {
                        context.Response.Close();
                        return;
                    }
                    else
                    {
                        context.Response.Close();
                        return;
                    }
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
