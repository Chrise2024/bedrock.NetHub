using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using bedrock.NetHub.Api;

namespace bedrock.NetHub.Service
{
    public partial class Http
    {
        private string RoorUrl;

        private HttpListener listener = new();
        public Http(string RootUrl)
        {
            if (!RootUrlRegex().IsMatch(RootUrl))
            {
                throw new Exception("Invalid Url");
            }
            try
            {
                listener.Prefixes.Add(RootUrl);
            }
            catch (Exception e)
            {
                Program.stdhubLOGGER.Info(e.Message);
            }
        }

        public void Start()
        {
            Task.Run(() =>
            {
                listener.Start();
                while (listener.IsListening)
                {
                    try
                    {
                        HttpListenerContext context = listener.GetContext();
                    }
                    catch { }
                }
                listener.Stop();
            });
        }

        private void HandleRequest(HttpListenerContext context)
        {
            string RawUrl = context.Request.RawUrl;
            string[] Options = RawUrl.Split('/');
            if (Options.Length != 2)
            {
                Program.stdhubLOGGER.Info("Bad Request Path");
            }
            else
            {
                switch (Options[0])
                {
                    case "file":
                        if (Options[1].Equals("read"))
                        {
                            FileAPI.Read(context);
                        }
                        else
                        {
                            context.Response.StatusCode = 404;
                        }
                        break;
                    case "config":
                        break;
                }
            }
        }

        [GeneratedRegex(@"^http://127.0.0.1:\d/\w+")]
        private static partial Regex RootUrlRegex();
    }
}
