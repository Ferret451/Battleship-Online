using System;
using System.Net.WebSockets;
using System.Text;
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
            WebSocketReceiveResult result = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), default);
            string roomCode = Encoding.UTF8.GetString(buffer, 0, result.Count);
            
            MessageBox.Show($"Room created with code: {roomCode}");
        }

        private async void butConnect_Click(object sender, EventArgs e) {
            using (JoinForm joinRoomForm = new JoinForm())
            {
                if (joinRoomForm.ShowDialog() == DialogResult.OK)
                {
                    string roomCode = joinRoomForm.RoomCode;
                    if (!string.IsNullOrEmpty(roomCode)) {
                        _clientWebSocket = new ClientWebSocket();
                        Uri serverUri = new Uri($"ws://localhost:8080/joinRoom?code={roomCode}");
                        await _clientWebSocket.ConnectAsync(serverUri, default);

                        MessageBox.Show($"Joined room with code: {roomCode}");
                    }
                    else {
                        MessageBox.Show("Room code cannot be empty.");
                    }
                }
            }
        }
        
        private void butQuit_Click(object sender, EventArgs e) {
            Close();
        }
    }
}