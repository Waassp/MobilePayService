using MobilePayService.Methods;
using MobilePayService.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace MobilePayService.RestAPI
{
    public delegate void delReceiveWebRequest(HttpListenerContext Context);

    /// <summary>
    /// Wrapper class for the HTTPListener to allow easier access to the
    /// server, for start and stop management and event routing of the actual
    /// inbound requests.
    /// </summary>
    public class HttpServer
    {
        protected HttpListener Listener;
        protected bool IsStarted = false;
        public event delReceiveWebRequest ReceiveWebRequest;

        public HttpServer()
        {
        }

        /// <summary>
        /// Starts the Web Service
        /// </summary>
        /// <param name="UrlBase">
        /// A Uri that acts as the base that the server is listening on.
        /// Format should be: http://127.0.0.1:8080/ or http://127.0.0.1:8080/somevirtual/
        /// Note: the trailing backslash is required! For more info see the
        /// HttpListener.Prefixes property on MSDN.
        /// </param>
        public void Start(string UrlBase)
        {
            if (!HttpListener.IsSupported)
            {
                Console.WriteLine("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            // *** Already running - just leave it in place
            if (IsStarted)
                return;

            if (Listener == null)
            {
                Listener = new HttpListener();
            }

            Listener.Prefixes.Add(UrlBase);
            IsStarted = true;
            Listener.Start();

            IAsyncResult result = Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
        }

        /// <summary>
        /// Shut down the Web Service
        /// </summary>
        public void Stop()
        {
            if (Listener != null)
            {
                this.Listener.Close();
                this.Listener = null;
                this.IsStarted = false;
            }
        }

        protected void WebRequestCallback(IAsyncResult result)
        {
            if (Listener == null)
                return;

            // Get out the context object
            HttpListenerContext context = Listener.EndGetContext(result);
            
            // *** Immediately set up the next context
            Listener.BeginGetContext(new AsyncCallback(WebRequestCallback), Listener);
            ReceiveWebRequest?.Invoke(context);
            ProcessRequest(context);
        }

        /// <summary>
        /// Overridable method that can be used to implement a custom hnandler
        /// </summary>
        /// <param name="Context"></param>
        protected virtual void ProcessRequest(HttpListenerContext Context)
        {            
        }
    }
}