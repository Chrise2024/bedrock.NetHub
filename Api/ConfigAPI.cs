﻿using bedrock.NetHub.Utils;
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
                StreamWriter writer = new(context.Response.OutputStream);
                if (ReqJSON == null || !ReqJSON.ContainsKey("namespace") || !ReqJSON.ContainsKey("defaults"))
                {
                    writer.Write("{}");
                    context.Response.StatusCode = 400;
                }
                else
                {
                    string pluginRoot = Path.Join(Program.pluginsRoot, ReqJSON["namespace"].Value<string>());
                    string configFilePath = ReqJSON.ContainsKey("subConfigName") ? Path.Join(pluginRoot, ReqJSON["subConfigName"].Value<string>() + ".json") : Path.Join(pluginRoot, "config.json");
                    if (!File.Exists(configFilePath))
                    {
                        writer.Write(ReqJSON["defaults"].Value<string>());
                        context.Response.StatusCode = 404;
                    }
                    else
                    {
                        writer.Write(FileIO.ReadFile(configFilePath));
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
