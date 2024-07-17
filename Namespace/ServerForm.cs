using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Windows.Forms;

namespace InfoServerApp
{
    public partial class ServerForm : Form
    {
        private static List<Info> infos = new List<Info>
        {
            new Info
            {
                Id = 1,
                Title = "Info 1",
                Description = "Description for info 1",
                Status = InfoStatus.Draft,
                ImagesBase64 = new List<string> { Convert.ToBase64String(File.ReadAllBytes("images/info1.jpg")) }
            },
            new Info
            {
                Id = 2,
                Title = "Info 2",
                Description = "Description for info 2",
                Status = InfoStatus.UnderReview,
                ImagesBase64 = new List<string> { Convert.ToBase64String(File.ReadAllBytes("images/info2.jpg")) }
            }
            // Add more info objects here
        };

        private static ConcurrentDictionary<string, ClientRequestInfo> clientRequests = new ConcurrentDictionary<string, ClientRequestInfo>();
        private static readonly int requestLimit = 10;
        private static readonly TimeSpan timeSpan = TimeSpan.FromHours(1);
        private static readonly int maxClients = 100;
        private static readonly TimeSpan inactivityLimit = TimeSpan.FromMinutes(10);

        private Thread serverThread;
        private UdpClient udpServer;
        private bool isRunning = false;

        public ServerForm()
        {
            InitializeComponent();
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (!isRunning)
            {
                serverThread = new Thread(StartServer);
                serverThread.IsBackground = true;
                serverThread.Start();
                isRunning = true;
                Log("Server started.");
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if (isRunning)
            {
                isRunning = false;
                udpServer.Close();
                serverThread.Join();
                Log("Server stopped.");
            }
        }

        private void StartServer()
        {
            udpServer = new UdpClient(11000);
            IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, 11000);

            Thread cleanupThread = new Thread(CleanupInactiveClients);
            cleanupThread.IsBackground = true;
            cleanupThread.Start();

            while (isRunning)
            {
                try
                {
                    var receivedBytes = udpServer.Receive(ref remoteEP);
                    var clientKey = remoteEP.ToString();

                    if (!clientRequests.ContainsKey(clientKey))
                    {
                        if (clientRequests.Count >= maxClients)
                        {
                            var responseString = "Server is full. Try again later.";
                            var responseBytes = Encoding.UTF8.GetBytes(responseString);
                            udpServer.Send(responseBytes, responseBytes.Length, remoteEP);
                            Log($"Connection attempt from {clientKey} failed: server is full.");
                            continue;
                        }

                        clientRequests[clientKey] = new ClientRequestInfo
                        {
                            RequestCount = 0,
                            LastRequestTime = DateTime.Now,
                            LastActiveTime = DateTime.Now
                        };
                        Log($"Client connected: {clientKey}");
                    }

                    var clientInfo = clientRequests[clientKey];
                    if (clientInfo.RequestCount >= requestLimit && DateTime.Now - clientInfo.LastRequestTime < timeSpan)
                    {
                        var responseString = "Request limit exceeded. Please try again later.";
                        var responseBytes = Encoding.UTF8.GetBytes(responseString);
                        udpServer.Send(responseBytes, responseBytes.Length, remoteEP);
                        Log($"Request limit exceeded for client {clientKey}.");
                        continue;
                    }

                    if (DateTime.Now - clientInfo.LastRequestTime >= timeSpan)
                    {
                        clientInfo.RequestCount = 0;
                        clientInfo.LastRequestTime = DateTime.Now;
                    }

                    clientInfo.RequestCount++;
                    clientInfo.LastActiveTime = DateTime.Now;

                    var receivedString = Encoding.UTF8.GetString(receivedBytes);
                    var requestedInfo = JsonSerializer.Deserialize<Info>(receivedString);

                    var matchingInfos = infos.Where(i => i.Title.Contains(requestedInfo.Title) || i.Description.Contains(requestedInfo.Description)).ToList();
                    var responseString = JsonSerializer.Serialize(matchingInfos);

                    var responseBytes = Encoding.UTF8.GetBytes(responseString);
                    udpServer.Send(responseBytes, responseBytes.Length, remoteEP);

                    Log($"Processed request from {clientKey}: {requestedInfo.Title} - {requestedInfo.Description}");
                }
                catch (SocketException ex)
                {
                    Log($"Socket exception: {ex.Message}");
                }
            }
        }

        private void CleanupInactiveClients()
        {
            while (isRunning)
            {
                foreach (var clientKey in clientRequests.Keys)
                {
                    if (DateTime.Now - clientRequests[clientKey].LastActiveTime >= inactivityLimit)
                    {
                        clientRequests.TryRemove(clientKey, out _);
                        Log($"Client disconnected due to inactivity: {clientKey}");
                    }
                }

                Thread.Sleep(60000); // Check every 60 seconds
            }
        }

        private void Log(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(Log), message);
            }
            else
            {
                txtLog.AppendText($"{DateTime.Now}: {message}{Environment.NewLine}");
                Logger.Log(message);
            }
        }
    }

    public class ClientRequestInfo
    {
        public int RequestCount { get; set; }
        public DateTime LastRequestTime { get; set; }
        public DateTime LastActiveTime { get; set; }
    }
}
