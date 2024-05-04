using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class GameForm : Form {
        private ClientWebSocket _clientWebSocket;

        public GameForm() {
            InitializeComponent();
        }

        private async void MainForm_Load(object sender, EventArgs e) {
            _clientWebSocket = new ClientWebSocket();
            Uri serverUri = new Uri("ws://localhost:8080/echo");
            await _clientWebSocket.ConnectAsync(serverUri, CancellationToken.None);

            LogMessage("Connected to server.");
        }

        private async void SendMessageButton_Click(object sender, EventArgs e) {
            string message = tbMessages.Text;
            await SendMessageAsync(message);
        }

        private async Task SendMessageAsync(string message) {
            if (_clientWebSocket.State == WebSocketState.Open) {
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                LogMessage("Sent message: " + message); 
            } else {
                LogMessage("Error: WebSocket connection is not open.");
            }
        }

        private async void ReceiveMessageButton_Click(object sender, EventArgs e) {
            await ReceiveMessageAsync();
        }

        private async Task ReceiveMessageAsync() {
            if (_clientWebSocket.State == WebSocketState.Open) {
                byte[] buffer = new byte[1024];
                WebSocketReceiveResult result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

                LogMessage("Received message: " + receivedMessage);
            } else {
                LogMessage("Error: WebSocket connection is not open.");
            }
        }

        private void LogMessage(string message) {
            lbMessages.Items.Add(message);
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (_clientWebSocket != null) {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                _clientWebSocket.Dispose();
            }
        }
    }
}