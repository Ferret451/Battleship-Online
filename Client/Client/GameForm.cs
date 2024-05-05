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
        private string _code;

        public GameForm(ClientWebSocket clientWebSocket, string code) {
            InitializeComponent();
            
            _clientWebSocket = clientWebSocket;
            _code = code;
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
    }
}