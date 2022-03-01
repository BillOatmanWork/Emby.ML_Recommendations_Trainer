using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ML_Recommendations_Trainer
{
    public delegate byte[] ProcessDataDelegate(string data);

    public class SubmissionService
    {
        private const int HandlerThread = 2;
        private readonly ProcessDataDelegate Handler;
        private readonly HttpListener Listener;

        public SubmissionService(HttpListener listener, string url, ProcessDataDelegate handler)
        {
            this.Listener = listener;
            this.Handler = handler;
            listener.Prefixes.Add(url);
        }

        public void Start()
        {
            if (Listener.IsListening)
                return;

            Listener.Start();

            for (int i = 0; i < HandlerThread; i++)
            {
                Listener.GetContextAsync().ContinueWith(ProcessRequestHandler);
            }
        }

        public void Stop()
        {
            if(Listener.IsListening)
                Listener.Stop();
        }

        private void ProcessRequestHandler(Task<HttpListenerContext> result)
        {
            var context = result.Result;

            if (!Listener.IsListening)
                return;

            // Start new listener which replace this
            Listener.GetContextAsync().ContinueWith(ProcessRequestHandler);

            // Read request
            string request = new StreamReader(context.Request.InputStream).ReadToEnd();

            // Prepare response
            var responseBytes = Handler.Invoke(request);
            context.Response.ContentLength64 = responseBytes.Length;

            var output = context.Response.OutputStream;
            output.WriteAsync(responseBytes, 0, responseBytes.Length);
            output.Close();
        }
    }
}
