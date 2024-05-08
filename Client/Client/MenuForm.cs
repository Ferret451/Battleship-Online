using System;
using System.Net.WebSockets;
using System.Windows.Forms;

namespace Client {
    public partial class MenuForm : Form {
        private ClientWebSocket _clientWebSocket;
        
        public MenuForm() {
            InitializeComponent();
        }

        private async void butCreateRoom_Click(object sender, EventArgs e) {
            if (!ValidateNickname(tbNickname.Text))
                return;
            
            _clientWebSocket = new ClientWebSocket();
            Uri serverUri = new Uri("ws://localhost:8080/createRoom");
            await _clientWebSocket.ConnectAsync(serverUri, default);

            // Handle room code received from server
            byte[] buffer = new byte[1024];
            await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
            Message message = MessageManager.HandleMessage(buffer);
            
            if (message.Type != "code") {
                MessageBox.Show("Error: Expected room code message from server.");
            } else {
                string code = message.Info;
                GameForm gameForm = new GameForm(_clientWebSocket, code, tbNickname.Text);
                gameForm.ShowDialog();  
                
                buffer = MessageManager.CreateMessage("close", "client", code);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, default);
            }
        }

        private async void butConnect_Click(object sender, EventArgs e) {
            if (!ValidateNickname(tbNickname.Text))
                return;
            
            using (JoinForm joinRoomForm = new JoinForm()) {
                if (joinRoomForm.ShowDialog() == DialogResult.OK) {
                    string code = joinRoomForm.RoomCode;
                    
                    _clientWebSocket = new ClientWebSocket();
                    Uri serverUri = new Uri($"ws://localhost:8080/joinRoom?code={code}");
                    await _clientWebSocket.ConnectAsync(serverUri, default);
                    
                    byte[] buffer = new byte[1024];
                    await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
                    Message message = MessageManager.HandleMessage(buffer);

                    if (message.Type == "error") {
                        MessageBox.Show($"Error: {message.Info}");
                    } else if (message.Type == "code") {
                        GameForm gameForm = new GameForm(_clientWebSocket, code, tbNickname.Text);
                        gameForm.ShowDialog();
                        
                        buffer = MessageManager.CreateMessage("close", "client", code);
                        await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, default);
                    } else {
                        MessageBox.Show("Error: Expected joined message from server.");
                    }
                }
            }
        }

        private bool ValidateNickname(string nickname) {
            if (nickname.Length > 10) {
                MessageBox.Show("Nickname is too long!");
                return false;
            }

            if (string.IsNullOrEmpty(nickname)) {
                MessageBox.Show("Nickname cannot be empty!");
                return false;
            }

            return true;
        }
        
        private void butQuit_Click(object sender, EventArgs e) {
            Close();
        }

        private async void MenuForm_FormClosing(object sender, FormClosingEventArgs e) {
            await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by client", default);
        }
    }
}
