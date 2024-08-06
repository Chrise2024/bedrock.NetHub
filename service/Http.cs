using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using bedrock.NetHub.Api;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace bedrock.NetHub.Service
{
    public abstract class Http
    {
        public static JObject ReadRequest(HttpListenerContext context)
        {
            StreamReader sr = new(context.Request.InputStream);
            return JObject.Parse(sr.ReadToEnd());
        }

        public static T ReadRequest<T>(HttpListenerContext context)
        {
            StreamReader sr = new(context.Request.InputStream);
            return JObject.Parse(sr.ReadToEnd()).ToObject<T>();
        }

        public static string ReadRequestAsString(HttpListenerContext context)
        {
            StreamReader sr = new(context.Request.InputStream);
            return sr.ReadToEnd();
        }

        public static void WriteRequest(HttpListenerContext context, int statusCode, string content)
        {
            StreamWriter sw = new(context.Response.OutputStream);
            sw.Write(JsonConvert.SerializeObject(new { data = content }));
            context.Response.StatusCode = statusCode;
            sw.Close();
            context.Response.Close();
        }

        public static void WriteRequest(HttpListenerContext context, int statusCode, object content)
        {
            StreamWriter sw = new(context.Response.OutputStream);
            sw.Write(JsonConvert.SerializeObject(new { data = content }));
            context.Response.StatusCode = statusCode;
            sw.Close();
            context.Response.Close();
        }

        public static void WriteRequest<T>(HttpListenerContext context, int statusCode, T content)
        {
            StreamWriter sw = new(context.Response.OutputStream);
            sw.Write(JsonConvert.SerializeObject(new { data = content }));
            context.Response.StatusCode = statusCode;
            sw.Close();
            context.Response.Close();
        }
    }
    public partial class HttpManager
    {
        private string RoorUrl;

        private HttpListener listener = new();
        public HttpManager(string RootUrl)
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

        public async void Start()
        {
            listener.Start();
            while (listener.IsListening)
            {
                try
                {
                    HttpListenerContext context = listener.GetContext();
                    HandleRequest(context);
                }
                catch (Exception ex)
                {
                    Program.stdhubLOGGER.Info(ex.Message);
                }
            }
            listener.Stop();
        }

        private void HandleRequest(HttpListenerContext context)
        {
            string RawUrl = context.Request.RawUrl[2..];
            string[] Options = RawUrl.Split('/');
            context.Response.ContentType = "text/plain;charset=UTF-8";
            context.Response.AddHeader("Content-type", "text/plain");
            context.Response.ContentEncoding = Encoding.UTF8;
            if (Options.Length < 1 || Options.Length > 2)
            {
                Program.stdhubLOGGER.Info("Bad Request Path");
                Http.WriteRequest(context, 400, "{}");
            }
            else if (Options.Length == 1)
            {
                switch (Options[0])
                {
                    case "config":
                        ConfigAPI.Read(context);
                        break;
                    case "log":
                        LogAPI.Log(context);
                        break;
                    default:
                        Http.WriteRequest(context, 404, "{}");
                        break;
                }
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
                            Http.WriteRequest(context, 404, "{}");
                        }
                        break;
                    case "data":
                        if (Options[1].Equals("read"))
                        {
                            DataAPI.Read(context);
                        }
                        else if (Options[1].Equals("write"))
                        {
                            DataAPI.Write(context);
                        }
                        else if (Options[1].Equals("delete"))
                        {
                            DataAPI.Delete(context);
                        }
                        else
                        {
                            Http.WriteRequest(context, 404, "{}");
                        }
                        break;
                    case "command":
                        if (Options[1].Equals("register"))
                        {
                            CommandAPI.Register(context);
                        }
                        else if (Options[1].Equals("submit"))
                        {
                            CommandAPI.Submit(context);
                        }
                        else
                        {
                            Http.WriteRequest(context, 404, "{}");
                        }
                        break;
                    case "xuid":
                        if (Options[1].Equals("get-xuid-by-name"))
                        {
                            XuidAPI.GetXuidByName(context);
                        }
                        else if (Options[1].Equals("get-name-by-xuid"))
                        {
                            XuidAPI.GetNameByXuid(context);
                        }
                        else
                        {
                            Http.WriteRequest(context, 404, "{}");
                        }
                        break;
                    case "perm":
                        switch (Options[1])
                        {
                            case "create-group":
                                PermissionAPI.CreateGroup(context);
                                break;
                            case "delete-group":
                                PermissionAPI.DeleteGroup(context);
                                break;
                            case "grant-permission":
                                PermissionAPI.GrantPermission(context);
                                break;
                            case "revoke-permission":
                                PermissionAPI.RevokePermission(context);
                                break;
                            case "test-permission":
                                PermissionAPI.TestPermission(context);
                                break;
                            case "add-player-to-group":
                                PermissionAPI.AddPlayerToGroup(context);
                                break;
                            case "remove-player-from-group":
                                PermissionAPI.RemovePlayerFromGroup(context);
                                break;
                            case "list-groups-of-player":
                                PermissionAPI.ListGroupsOfPlayer(context);
                                break;
                            case "list-players-in-group":
                                PermissionAPI.ListPlayersInGroup(context);
                                break;
                            case "list-explicit-permissions":
                                PermissionAPI.ListExplicitPermissions(context);
                                break;
                            case "list-all-permissions":
                                PermissionAPI.ListAllPermissions(context);
                                break;
                            case "list-all-groups":
                                PermissionAPI.ListAllGroups(context);
                                break;
                            default:
                                Http.WriteRequest(context, 404, "{}");
                                break;
                        }
                        break;
                    default:
                        Http.WriteRequest(context, 404, "{}");
                        break;
                }
            }
        }

        [GeneratedRegex(@"^http://127.0.0.1:\d+/")]
        private static partial Regex RootUrlRegex();
    }
}
