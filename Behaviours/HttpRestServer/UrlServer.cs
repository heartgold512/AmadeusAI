﻿// File: UrlServer.cs
// Created: 19.05.2018
// 
// See <summary> tags for more information.

using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using uhttpsharp;
using uhttpsharp.Listeners;
using uhttpsharp.RequestProviders;

namespace AmadeusAI.Behaviours.HttpRestServer
{
    internal class UrlServer
    {
        public static string URL { get; private set; }

        //may be offset this to a thread?
        public static void StartServer()
        {
            var httpServer = new HttpServer(new HttpRequestProvider());

            // listen to 127.0.0.1:9812
            httpServer.Use(new TcpListenerAdapter(new TcpListener(IPAddress.Loopback, 9812))); //if you want a different port you may choose so but update elsewhere where is stated of a port num

            // Request handling: 
            httpServer.Use((context, next) =>
            {
                return Task.Run(() =>
                {
                    try
                    {
                        UrlServer.URL = Encoding.UTF8.GetString(context.Request.Post.Raw); //pass string as the request
                    }
                    catch
                    {
                        Debugger.Break(); //speaks for itself
                    }
                });
            });

            // Handler request : 
            //httpServer.Use(new HttpRouter().With("AmadeusAI.Settingssskai", new RestHandler<string>(new RestController((url) => this.URL = url), JsonResponseProvider.Default)));

            httpServer.Start();
        }
    }
}