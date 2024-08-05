using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Net;
using System.Collections;
using bedrock.NetHub.Api;

namespace bedrock.NetHub.Service
{
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
            if (Options.Length < 1 || Options.Length > 2)
            {
                Program.stdhubLOGGER.Info("Bad Request Path");
                context.Response.StatusCode = 400;
                context.Response.Close();
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
                        context.Response.StatusCode = 404;
                        context.Response.Close();
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
                            context.Response.StatusCode = 404;
                            context.Response.Close();
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
                            context.Response.StatusCode = 404;
                            context.Response.Close();
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
                            context.Response.StatusCode = 404;
                            context.Response.Close();
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
                            context.Response.StatusCode = 404;
                            context.Response.Close();
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
                                context.Response.StatusCode = 404;
                                context.Response.Close();
                                break;
                        }
                        break;
                    default:
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        break;
                }
            }
        }

        [GeneratedRegex(@"^http://127.0.0.1:\d+/")]
        private static partial Regex RootUrlRegex();
    }
}
