using System;
using System.Drawing;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public partial class GameForm : Form {
        private ClientWebSocket _clientWebSocket;
        private string _code;
        private string _nickname;
        private readonly int _baseFieldLeft = 12;
        private readonly int _baseFieldTop = 87;
        private readonly Color _defaultColor;
        private bool _isRight = true;
        private bool _canToStand = true;

        private readonly Color _canToStandColor = Color.LawnGreen;
        private readonly Color _cannotToStandColor = Color.Red;
        private readonly Color _shipColor = Color.Black;
        private readonly Color _aroundShipColor = Color.Gray;

        private readonly string[] _ships =
            { "4 deck", "3 deck", "3 deck", "2 deck", "2 deck", "2 deck", "1 deck", "1 deck", "1 deck", "1 deck" };

        public GameForm(ClientWebSocket clientWebSocket, string code, string nickname) {
            InitializeComponent();
            
            _clientWebSocket = clientWebSocket;
            _code = code;
            _nickname = nickname;

            ClearField();

            MakeField(dgvPlayerField, _baseFieldLeft, _baseFieldTop);
            MakeField(
                dgvEnemyField, 
                dgvPlayerField.Left + dgvPlayerField.Width + (int)(0.1 * dgvEnemyField.Parent.Width), 
                _baseFieldTop
                );
            dgvPlayerField.SendToBack();
            dgvEnemyField.SendToBack();
            _defaultColor = dgvPlayerField.DefaultCellStyle.BackColor;
            
            tbChat.BringToFront();
            tbMessage.BringToFront();
            butSend.BringToFront();
            
            panChat.Visible = false;
            
            ResizeControls();
            
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
            await SendReceiveMessages();
        }
        
        private void GameForm_SizeChanged(object sender, EventArgs e) {
            ResizeControls();
        }
        
        private void ResizeControls() {
            ResizeDgv(dgvPlayerField);
            ResizeDgv(dgvEnemyField);
            dgvEnemyField.Left = dgvPlayerField.Left + dgvPlayerField.Width + (int)(0.1 * dgvEnemyField.Parent.Width);
            labEnemy.Left = dgvEnemyField.Left;
            
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

        private void dgvPlayerField_CellMouseEnter(object sender, DataGridViewCellEventArgs e) {
            CellMouseEnter((DataGridView) sender, e.RowIndex, e.ColumnIndex);
        }

        private void CellMouseEnter(DataGridView dgv, int rowIndex, int columIndex) {
            if (cbShips.SelectedIndex >= 0 && Int32.TryParse(cbShips.SelectedItem.ToString().Split(' ')[0], out int shipSize)) {
                Color color = _canToStandColor;
                _canToStand = true;
                if (_isRight) {
                    if (columIndex + shipSize > dgv.ColumnCount || IsShipAround(dgv, rowIndex, columIndex, shipSize)) {
                        color = _cannotToStandColor;
                        _canToStand = false;
                    }
                    
                    for (int i = 0; i < shipSize; i++) {
                        if (columIndex + i < dgv.ColumnCount) {
                            Color cellColor = dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor;
                            if (cellColor != _shipColor && cellColor != _aroundShipColor) {
                                dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor = color;
                                dgv.Rows[rowIndex].Cells[columIndex + i].Style.SelectionBackColor = color;
                            }
                        }
                    }
                }
                else {
                    if (rowIndex + shipSize > dgv.RowCount || IsShipAround(dgv, rowIndex, columIndex, shipSize)) {
                        color = _cannotToStandColor;
                        _canToStand = false;
                    }
                    
                    for (int i = 0; i < shipSize; i++) {
                        if (rowIndex + i < dgv.RowCount) {
                            Color cellColor = dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor;
                            if (cellColor != _shipColor &&
                                cellColor != _aroundShipColor) {
                                dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor = color;
                                dgv.Rows[rowIndex + i].Cells[columIndex].Style.SelectionBackColor = color;
                            }
                        }
                    }
                }
            }
        }

        private void dgvPlayerField_CellMouseLeave(object sender, DataGridViewCellEventArgs e) {
            CellMouseLeave((DataGridView) sender, e.RowIndex, e.ColumnIndex);
        }
        
        private void CellMouseLeave(DataGridView dgv, int rowIndex, int columIndex) {
            if (cbShips.SelectedIndex >= 0 && Int32.TryParse(cbShips.SelectedItem.ToString().Split(' ')[0], out int shipSize)) {
                if (_isRight) {
                    for (int i = 0; i < shipSize; i++) {
                        if (columIndex + i < dgv.ColumnCount) {
                            Color cellColor = dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor;
                            if (cellColor != _shipColor &&
                                cellColor != _aroundShipColor) {
                                dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor = _defaultColor;
                                dgv.Rows[rowIndex].Cells[columIndex + i].Style.SelectionBackColor = _defaultColor;
                            }
                        }
                    }
                }
                else {
                    
                    for (int i = 0; i < shipSize; i++) {
                        if (rowIndex + i < dgv.RowCount) {
                            Color cellColor = dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor;
                            if (cellColor != _shipColor &&
                                cellColor != _aroundShipColor) {
                                dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor = _defaultColor;
                                dgv.Rows[rowIndex + i].Cells[columIndex].Style.SelectionBackColor = _defaultColor;
                            }
                        }
                    }
                }
            }
        }

        private bool IsShipAround(DataGridView dgv, int rowIndex, int colIndex, int shipSize) {
            if (_isRight) {
                for (int i = 0; i < shipSize; i++) {
                    Color color = dgv.Rows[rowIndex].Cells[colIndex + i].Style.BackColor;
                    if (color == _shipColor || color == _aroundShipColor) {
                        return true;
                    }
                }
            } else {
                for (int i = 0; i < shipSize; i++) {
                    Color color = dgv.Rows[rowIndex + i].Cells[colIndex].Style.BackColor;
                    if (color == _shipColor || color == _aroundShipColor) {
                        return true;
                    }
                }
            }

            return false;
        }

        private void dgvPlayerField_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e) {
            DataGridView dgv = (DataGridView) sender;
            
            if (e.Button == MouseButtons.Right) {
                CellMouseLeave(dgv, e.RowIndex, e.ColumnIndex);
                _isRight = !_isRight;
                CellMouseEnter(dgv, e.RowIndex, e.ColumnIndex);
            } else if (e.Button == MouseButtons.Left) {
                if (cbShips.SelectedIndex >= 0 && _canToStand && cbShips.Items.Count > 0 && Int32.TryParse(cbShips.SelectedItem.ToString().Split(' ')[0], out int shipSize)) {
                    if (_isRight) {
                        for (int i = 0; i < shipSize; i++) {
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + i].Style.BackColor = _shipColor;
                            dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + i].Style.SelectionBackColor = _shipColor;
                            PlaceAroundShip(dgv, e.RowIndex, e.ColumnIndex + i);
                        }
                    }
                    else {
                        for (int i = 0; i < shipSize; i++) {
                            dgv.Rows[e.RowIndex + i].Cells[e.ColumnIndex].Style.BackColor = _shipColor;
                            dgv.Rows[e.RowIndex + i].Cells[e.ColumnIndex].Style.SelectionBackColor = _shipColor;
                            PlaceAroundShip(dgv, e.RowIndex + i, e.ColumnIndex);
                        }
                    }
                    
                    cbShips.Items.RemoveAt(cbShips.SelectedIndex);
                }
            }
        }
        
        private void PlaceAroundShip(DataGridView dgv, int rowIndex, int colIndex) {
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int currRowIndex = rowIndex + i;
                    int currColIndex = colIndex + j;
                    if (currRowIndex >= 0 && currRowIndex < dgv.RowCount && 
                        currColIndex >= 0 && currColIndex < dgv.ColumnCount &&
                        dgv.Rows[currRowIndex].Cells[currColIndex].Style.BackColor != _shipColor) {
                        dgv.Rows[currRowIndex].Cells[currColIndex].Style.BackColor = _aroundShipColor;
                        dgv.Rows[currRowIndex].Cells[currColIndex].Style.SelectionBackColor = _aroundShipColor;
                    }
                }
            }
        }
        
        private void butClearField_Click(object sender, EventArgs e) {
            ClearField();
        }

        private void ClearField() {
            cbShips.Items.Clear();
            for (int i = 0; i < _ships.Length; i++) {
                cbShips.Items.Add(_ships[i]);
            }

            cbShips.SelectedIndex = 0;
            
            for (int i = 0; i < dgvPlayerField.RowCount; i++) {
                for (int j = 0; j < dgvPlayerField.ColumnCount; j++) {
                    dgvPlayerField.Rows[i].Cells[j].Style.BackColor = _defaultColor;
                    dgvPlayerField.Rows[i].Cells[j].Style.SelectionBackColor = _defaultColor;
                }
            }
        }
    }
}