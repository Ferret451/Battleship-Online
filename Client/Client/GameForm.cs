using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class GameForm : Form {
        private ClientWebSocket _clientWebSocket;
        private string _code;
        private string _nickname;

        public GameForm(ClientWebSocket clientWebSocket, string code, string nickname) {
            InitializeComponent();
            
            _clientWebSocket = clientWebSocket;
            _code = code;
            _nickname = nickname;

            tbChat.BringToFront();
            tbMessage.BringToFront();
            butSend.BringToFront();

            panChat.Visible = false;
            
            tbChat.AppendText($"Room created with code {code}\r\n");
        }
        
        private async void GameForm_Load(object sender, EventArgs e) {
            await SendReceiveMessages();
        }
        
        private void GameForm_SizeChanged(object sender, EventArgs e) {
            ResizeControls();
        }
        
        private void ResizeControls() {
            tbChat.Height = (int)(tbChat.Parent.Height * 0.8);
            panSend.Height = (int)(panSend.Parent.Height * 0.2);
        }
        
        private void butChat_Click(object sender, EventArgs e) {
            panChat.Visible = !panChat.Visible;
            ResizeControls();
        }

        private void butFullScreen_Click(object sender, EventArgs e) {
            if (FormBorderStyle != FormBorderStyle.None) {
                FormBorderStyle = FormBorderStyle.None;
                WindowState = FormWindowState.Maximized;
            } else {
                FormBorderStyle = FormBorderStyle.Sizable;
                WindowState = FormWindowState.Normal;
            }
        }

        private async void SendMessageButton_Click(object sender, EventArgs e) {
            await SendMessageAsync();
        }
        
        private async void tbMessage_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && !e.Shift) {
                await SendMessageAsync();

                e.Handled = true;
            }
        }
        
        private void tbMessage_TextChanged(object sender, EventArgs e) {
            if (tbMessage.Text.Length > 1024) {
                tbMessage.Text = tbMessage.Text.Substring(0, 1024);
            }
        }

        private async Task SendMessageAsync() {
            string message = tbMessage.Text;

            message = message.Trim();

            butSend.Focus();
            tbMessage.Focus();
            tbMessage.Text = "";
            tbMessage.Select(0, 0);
            
            if (string.IsNullOrEmpty(message))
                return;
            
            if (_clientWebSocket.State == WebSocketState.Open) {
                byte[] buffer = MessageManager.CreateMessage("chat", _nickname, message);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);

                LogMessage("Sent message: " + message); 
            } else {
                LogMessage("Error: WebSocket connection is not open.");
            }
        }
        
        private async Task SendReceiveMessages() {
            var receiveBuffer = new byte[1024];
            
            var receiveResult = await _clientWebSocket.ReceiveAsync(
                new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

            do {
                Message message = MessageManager.HandleMessage(receiveBuffer);

                if (message.Type == "chat") {
                    tbChat.AppendText($"{message.SentFrom}: {message.Info}\r\n");
                }
                else {
                    LogMessage("Error: Expected chat message from server.");
                }

                if (_clientWebSocket.State == WebSocketState.Open) {
                    receiveBuffer = new byte[1024];
                    receiveResult = await _clientWebSocket.ReceiveAsync(
                        new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                }
            } while (!receiveResult.CloseStatus.HasValue);
            
            // await _clientWebSocket.CloseAsync(
            //     receiveResult.CloseStatus.Value,
            //     receiveResult.CloseStatusDescription,
            //     CancellationToken.None);

            MessageBox.Show("Connection closed by server");
            
            CloseConnection(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription);
            
            Close();
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (_clientWebSocket.State != WebSocketState.Closed) {
                CloseConnection(WebSocketCloseStatus.NormalClosure, "Closed by client");
            }
        }

        private async void CloseConnection(WebSocketCloseStatus closeStatus, string statusDescription) {
            await _clientWebSocket.CloseAsync(closeStatus, statusDescription, default);
        }
        
        private void LogMessage(string message) {
            Console.WriteLine(message);
        }
    }
}