using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Rim.Server.Http;
using Rim.Server.Security;
using Rim.Server.WebSockets;

namespace Rim.Server
{
    public class HttpServer
    {

        #region Properties

        internal static readonly string WEBSOCKET_GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";

        public IHttpRequestHandler RequestHandler { get; set; }
        public IClientContainer Container { get; set; }
        public IFirewall Firewall { get; set; }
        public IClientFactory ClientFactory { get; set; }
        public IErrorLogger Logger { get; set; }

        public ServerOptions Options { get; private set; }

        private TcpListener Listener { get; set; }

        private Thread HandleThread { get; set; }

        public bool IsRunning { get; private set; }

        private X509Certificate Certificate { get; set; }

        #endregion

        #region Constructors

        public HttpServer(IClientFactory clientFactory, ServerOptions options) : this(null, clientFactory, null, null, null)
        {
            Firewall = new DefaultFirewall(this);
            Options = options;
        }

        public HttpServer(IHttpRequestHandler requestHandler, IClientFactory clientFactory)
            : this(requestHandler, clientFactory, null, null)
        {
            Firewall = new DefaultFirewall(this);
        }

        public HttpServer(IHttpRequestHandler requestHandler,
            IClientFactory clientFactory,
            IClientContainer clientContainer,
            IFirewall firewall) : this(requestHandler, clientFactory, clientContainer, firewall, null)
        {
            string serialized = System.IO.File.ReadAllText("rimserver.json");
            Options = JsonConvert.DeserializeObject<ServerOptions>(serialized);
        }

        public HttpServer(IHttpRequestHandler requestHandler,
            IClientFactory clientFactory,
            IClientContainer clientContainer,
            IFirewall firewall,
            ServerOptions options)
        {
            RequestHandler = requestHandler;
            ClientFactory = clientFactory;
            Container = Container;

            if (firewall != null)
                Firewall = firewall;

            if (options != null)
                Options = options;

            Logger = new ErrorLogger(this);
        }

        #endregion

        #region Start - Stop

        public void Start(bool async)
        {
            if (Listener != null)
                throw new InvalidOperationException("Stop the HttpServer before restarting");

            if (!string.IsNullOrEmpty(Options.CertificateFilename))
                Certificate = X509Certificate.CreateFromCertFile(Options.CertificateFilename);

            Listener = new TcpListener(IPAddress.Any, Options.Port);

            if (Options.MaximumPendingConnections == 0)
                Listener.Start();
            else
                Listener.Start(Options.MaximumPendingConnections);

            if (async)
            {
                HandleThread = new Thread(HandleConnections);
                HandleThread.Priority = ThreadPriority.AboveNormal;
                HandleThread.Start();
            }
            else
            {
                HandleThread = null;
                HandleConnections();
            }
        }

        public void Stop()
        {
            IsRunning = false;

            Listener.Stop();
            try
            {
                HandleThread.Interrupt();
            }
            catch { }

            HandleThread = null;
            Listener = null;
        }

        #endregion

        #region Handle Connections

        private void HandleConnections()
        {
            IsRunning = true;

            while (IsRunning)
            {
                if (Listener == null)
                    break;

                TcpClient tcp = Listener.AcceptTcpClient();

                Task.Factory.StartNew(() =>
                {
                    AcceptClient(tcp);
                });
            }

            Stop();
        }

        private void AcceptClient(TcpClient tcp)
        {
            string ip = tcp.Client.RemoteEndPoint.ToString().Split(':')[0];

            //check firewall
            if (Firewall != null && Firewall.Enabled && !Firewall.EntryAuthority(ip))
            {
                tcp.Close();
                return;
            }

            HttpResponse response = new HttpResponse();

            //ssl handshaking
            if (Options.UseSecureConnection)
            {
                SslStream sslStream = new SslStream(tcp.GetStream(), true);
                response.Stream = sslStream;
                try
                {
                    sslStream.AuthenticateAsServer(Certificate, false, true);
                }
                catch
                {
                    tcp.Close();
                    return;
                }
            }
            else
                response.Stream = tcp.GetStream();

            //create request
            string request_data = ReadRequest(response.Stream);
            if (request_data == null)
            {
                tcp.Close();
                return;
            }

            RequestReader reader = new RequestReader();
            HttpRequest request = reader.Read(request_data);
            request.IpAddress = ip;
            request.IsSecureConnection = Options.UseSecureConnection;
            if (!string.IsNullOrEmpty(request.AcceptEncoding) && request.AcceptEncoding.Contains("gzip"))
                response.ApplyGzip = true;

            //handle request
            if (request.IsWebSocket)
            {
                ServerSocket client = SocketRequestHandler.HandshakeClient(this, request, tcp);
                client.Start();
            }
            else
            {
                if (RequestHandler == null)
                {
                    tcp.Close();
                    return;
                }

                RequestHandler.Request(this, request, response);
                ResponseWriter writer = new ResponseWriter();
                writer.Write(response);
                tcp.Close();
            }

        }

        private string ReadRequest(Stream stream)
        {
            byte[] buffer = new byte[1024];

            int read = stream.Read(buffer, 0, buffer.Length);
            int total = 0;
            using (MemoryStream ms = new MemoryStream())
            {
                while (read > 0)
                {
                    total += read;
                    if (total > Options.MaximumRequestLength)
                        return null;

                    ms.Write(buffer, 0, read);

                    if (read < buffer.Length) //eof
                        break;

                    read = stream.Read(buffer, 0, buffer.Length);
                }

                string data = Encoding.UTF8.GetString(ms.ToArray());
                return data;
            }
        }

        #endregion

    }
}
