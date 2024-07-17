using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Windows.Forms;

namespace InfoClientApp
{
    public partial class ClientForm : Form
    {
        private UdpClient udpClient;
        private IPEndPoint serverEndpoint;

        public ClientForm()
        {
            InitializeComponent();
            udpClient = new UdpClient();
            serverEndpoint = new IPEndPoint(IPAddress.Loopback, 11000); // Assuming server runs on localhost
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var infoRequest = new Info
            {
                Title = txtTitle.Text,
                Description = txtDescription.Text
            };

            var requestString = JsonSerializer.Serialize(infoRequest);
            var requestBytes = Encoding.UTF8.GetBytes(requestString);
            udpClient.Send(requestBytes, requestBytes.Length, serverEndpoint);

            var responseBytes = udpClient.Receive(ref serverEndpoint);
            var responseString = Encoding.UTF8.GetString(responseBytes);

            var infos = JsonSerializer.Deserialize<List<Info>>(responseString);
            DisplayInfos(infos);
        }

        private void DisplayInfos(List<Info> infos)
        {
            infoPanel.Controls.Clear();

            foreach (var info in infos)
            {
                var infoCard = new InfoCard();
                infoCard.InfoTitle = info.Title;
                infoCard.Description = info.Description;
                infoCard.Status = info.Status.ToString();

                if (info.ImagesBase64 != null && info.ImagesBase64.Count > 0)
                {
                    foreach (var imageBase64 in info.ImagesBase64)
                    {
                        var imageBytes = Convert.FromBase64String(imageBase64);
                        using (var ms = new System.IO.MemoryStream(imageBytes))
                        {
                            infoCard.AddImage(System.Drawing.Image.FromStream(ms));
                        }
                    }
                }

                infoPanel.Controls.Add(infoCard);
            }
        }
    }

    public class Info
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public InfoStatus Status { get; set; } // New property for status
        public List<string> ImagesBase64 { get; set; } // List of base64 encoded images

        public Info()
        {
            ImagesBase64 = new List<string>();
        }
    }

    public enum InfoStatus
    {
        Draft,
        UnderReview,
        Rejected,
        Approved
    }
}
