using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class GameForm : Form {
        private readonly ClientWebSocket _clientWebSocket;
        private string _code;
        private string _nickname;
        private readonly int _baseFieldLeft = 12;
        private readonly int _baseFieldTop = 87;

        private readonly Color _defaultReadyColor;
        private readonly Color _readyColor = Color.Green;
        
        private readonly Color _yourTurnColor = Color.Green;
        private readonly Color _enemyTurnColor = Color.Red;
        
        private readonly PlayerFieldManager _playerFieldManager;
        private readonly EnemyFieldManager _enemyFieldManager;
        private readonly MessageManager _messageManager;

        private bool _isGameStarted = false;
        private bool _isYourTurn = false;

        public GameForm(ClientWebSocket clientWebSocket, string code, string nickname) {
            InitializeComponent();

            MakeField(dgvPlayerField, _baseFieldLeft, _baseFieldTop);
            MakeField(
                dgvEnemyField, 
                dgvPlayerField.Left + dgvPlayerField.Width + (int)(0.1 * dgvEnemyField.Parent.Width), 
                _baseFieldTop
                );
            dgvPlayerField.SendToBack();
            dgvEnemyField.SendToBack();
            _defaultReadyColor = butReady.BackColor;
            
            tbChat.BringToFront();
            tbMessage.BringToFront();
            butSend.BringToFront();
            panChat.Visible = false;
            
            labTurn.Visible = false;
            labTurn.SendToBack();
            labYou.SendToBack();
            labEnemy.SendToBack();
            
            ResizeControls();
            
            _clientWebSocket = clientWebSocket;
            _code = code;
            _nickname = nickname;
            
            _messageManager = new MessageManager(clientWebSocket);
            _playerFieldManager = new PlayerFieldManager(dgvPlayerField, cbShips, nickname, _messageManager);
            _playerFieldManager.ClearField();
            _enemyFieldManager = new EnemyFieldManager(dgvEnemyField, nickname, _messageManager);
            
            tbChat.AppendText($"Room created with code {code}\r\n");
        }

        private void MakeField(DataGridView dgv, int left, int top) {
            dgv.Left = left;
            dgv.Top = top;
            ResizeDgv(dgv);
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeColumns = false;
            dgv.AllowUserToResizeRows = false;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgv.ColumnHeadersVisible = false;
            dgv.RowHeadersVisible = false;
            dgv.RowCount = 10;
            dgv.ColumnCount = 10;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            dgv.DefaultCellStyle.SelectionBackColor = dgv.DefaultCellStyle.BackColor;
            dgv.DefaultCellStyle.SelectionForeColor = dgv.DefaultCellStyle.ForeColor;
            dgv.ScrollBars = ScrollBars.None;
        }
        
        private async void GameForm_Load(object sender, EventArgs e) {
            await ReceiveMessages();
        }
        
        private void GameForm_SizeChanged(object sender, EventArgs e) {
            ResizeControls();
        }
        
        private void ResizeControls() {
            ResizeDgv(dgvPlayerField);
            ResizeDgv(dgvEnemyField);
            dgvEnemyField.Left = dgvPlayerField.Left + dgvPlayerField.Width + (int)(0.1 * dgvEnemyField.Parent.Width);
            labEnemy.Left = dgvEnemyField.Left;
            labTurn.Left = dgvEnemyField.Left;
            
            tbChat.Height = (int)(tbChat.Parent.Height * 0.8);
            panSend.Height = (int)(panSend.Parent.Height * 0.2);
        }

        private void ResizeDgv(DataGridView dgv) {
            dgv.Width = (int)(dgv.Parent.Width * 0.4);
            dgv.Height = dgv.Width;
            
            dgv.AutoResizeRows();
            
            for (int i = 0; i < dgv.ColumnCount; i++) {
                dgv.Columns[i].MinimumWidth = dgv.Width / dgv.ColumnCount;
            }
            
            for (int i = 0; i < dgv.RowCount; i++) {
                dgv.Rows[i].MinimumHeight = dgv.Height / dgv.RowCount;
            }
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
            await SendChatMessage();
        }
        
        private async void tbMessage_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter && !e.Shift) {
                await SendChatMessage();
            }
        }

        private async Task SendChatMessage() {
            var message = tbMessage.Text;
            
            if (string.IsNullOrEmpty(message))
                return;
            
            message = message.Trim();

            butSend.Focus();
            tbMessage.Focus();
            tbMessage.Text = "";
            tbMessage.Select(0, 0);
            
            await _messageManager.SendMessageAsync("chat", _nickname, message);
        }
        
        private void tbMessage_TextChanged(object sender, EventArgs e) {
            if (tbMessage.Text.Length > 1024) {
                tbMessage.Text = tbMessage.Text.Substring(0, 1024);
            }
        }
        
        private async Task ReceiveMessages() {
            var receiveBuffer = new byte[1024];

            try {
                var receiveResult = await _clientWebSocket.ReceiveAsync(
                    new ArraySegment<byte>(receiveBuffer), CancellationToken.None);

                while (!receiveResult.CloseStatus.HasValue) {
                    Message message = MessageManager.HandleMessage(receiveBuffer);

                    if (message.Type == "chat") {
                        tbChat.AppendText($"{message.SentFrom}: {message.Info}\r\n");
                    }
                    else if (message.Type == "start") {
                        _isGameStarted = true;

                        butReady.Visible = false;
                        butClearField.Enabled = false;
                        cbShips.Enabled = false;
                        labTurn.Visible = true;
                    }
                    else if (message.Type == "goShoot") {
                        _isYourTurn = true;
                        
                        labTurn.Text = "Your turn";
                        labTurn.ForeColor = _yourTurnColor;
                    }
                    else if (message.Type == "wait") {
                        _isYourTurn = false;
                        
                        labTurn.Text = "Enemy turn";
                        labTurn.ForeColor = _enemyTurnColor;
                    }
                    else if (message.Type == "shotToYou") {
                        _playerFieldManager.ReadField(message.Info);   
                    }
                    else if (message.Type == "shotFromYou") {
                        _enemyFieldManager.ReadField(message.Info);
                    }
                    else if (message.Type == "win" || message.Type == "lose") {
                        MessageBox.Show(message.Info);
                        
                        Close();
                    }
                    else {
                        Console.WriteLine($"Error: unknown message type: {message.Type}.");
                    }

                    if (_clientWebSocket.State == WebSocketState.Open) {
                        receiveBuffer = new byte[1024];

                        receiveResult = await _clientWebSocket.ReceiveAsync(
                            new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                    }
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            finally {
                MessageBox.Show("Connection closed");
                
                CloseConnection(WebSocketCloseStatus.NormalClosure, "Connection closed");
                
                Close();
            }
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e) {
            if (_clientWebSocket.State != WebSocketState.Closed) {
                CloseConnection(WebSocketCloseStatus.NormalClosure, "Connection closed");
            }
        }

        private async void CloseConnection(WebSocketCloseStatus closeStatus, string statusDescription) {
            await _clientWebSocket.CloseAsync(closeStatus, statusDescription, default);
        }

        private void dgvPlayerField_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (!_playerFieldManager.IsReady && !_isGameStarted) {
                _playerFieldManager.CellMouseEnter(e.RowIndex, e.ColumnIndex);
            }
        }

        private void dgvPlayerField_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (!_playerFieldManager.IsReady && !_isGameStarted) {
                _playerFieldManager.CellMouseLeave(e.RowIndex, e.ColumnIndex);
            }
        }
        
        private void dgvPlayerField_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
            if (!_playerFieldManager.IsReady && !_isGameStarted) {
                _playerFieldManager.CellMouseDown(e);
            }
        }
        
        private void butClearField_Click(object sender, EventArgs e) {
            _playerFieldManager.ClearField();
        }

        private async void butReady_Click(object sender, EventArgs e) {
            if (cbShips.Items.Count == 0) {
                await _playerFieldManager.ReadyClick();

                if (_playerFieldManager.IsReady) {
                    butReady.BackColor = _readyColor;

                    butClearField.Enabled = false;
                    cbShips.Enabled = false;
                } else {
                    butReady.BackColor = _defaultReadyColor;
                    
                    butClearField.Enabled = true;
                    cbShips.Enabled = true;
                }
            }
        }

        private void dgvEnemyField_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            if (_isGameStarted && _isYourTurn) {
                _enemyFieldManager.CellMouseEnter(e.RowIndex, e.ColumnIndex);
            }
        }

        private void dgvEnemyField_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            if (_isGameStarted && _isYourTurn) {
                _enemyFieldManager.CellMouseLeave(e.RowIndex, e.ColumnIndex);
            }
        }

        private void dgvEnemyField_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
            if (_isGameStarted && _isYourTurn) {
                _enemyFieldManager.CellMouseDown(e.RowIndex, e.ColumnIndex);
            }
        }
    }
}