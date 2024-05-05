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
                GameForm gameForm = new GameForm(_clientWebSocket, code);
                gameForm.ShowDialog();  
                
                buffer = MessageManager.CreateMessage("close", "client", code);
                await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, default);
            }
        }

        private async void butConnect_Click(object sender, EventArgs e) {
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
                    } else if (message.Type == "joined") {
                        GameForm gameForm = new GameForm(_clientWebSocket, code);
                        gameForm.ShowDialog();
                        
                        buffer = MessageManager.CreateMessage("close", "client", code);
                        await _clientWebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Binary, true, default);
                    } else {
                        MessageBox.Show("Error: Expected joined message from server.");
                    }
                }
            }
        }
        
        private void butQuit_Click(object sender, EventArgs e) {
            Close();
        }
    }
}
