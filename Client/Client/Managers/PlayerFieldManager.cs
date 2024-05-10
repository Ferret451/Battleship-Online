using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client {
    public class PlayerFieldManager : FieldManager{
        private readonly MessageManager _messageManager;
        private readonly ComboBox _cbShips;
        private readonly string _nickname;
        
        public bool IsReady { get; private set; }

        private readonly string[] _ships =
            { "4 deck", "3 deck", "3 deck", "2 deck", "2 deck", "2 deck", "1 deck", "1 deck", "1 deck", "1 deck" };

        public PlayerFieldManager(DataGridView dgv, ComboBox cbShips, string nickname, MessageManager messageManager) : 
            base(dgv) {
            _messageManager = messageManager;
            _cbShips = cbShips;
            
            _nickname = nickname;
        }

        public void CellMouseEnter(int rowIndex, int columIndex) {
            if (_cbShips.SelectedIndex >= 0 &&
                Int32.TryParse(_cbShips.SelectedItem.ToString().Split(' ')[0], out int shipSize)) {
                base.CellMouseEnter(shipSize, rowIndex, columIndex);
            }
        }

        public void CellMouseLeave(int rowIndex, int columIndex) {
            if (_cbShips.SelectedIndex >= 0 &&
                Int32.TryParse(_cbShips.SelectedItem.ToString().Split(' ')[0], out int shipSize)) {
                base.CellMouseLeave(shipSize, rowIndex, columIndex);
            }
        }

        public void CellMouseDown(DataGridViewCellMouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                CellMouseLeave(e.RowIndex, e.ColumnIndex);
                IsRight = !IsRight;
                CellMouseEnter(e.RowIndex, e.ColumnIndex);
            }
            else if (e.Button == MouseButtons.Left) {
                if (_cbShips.SelectedIndex >= 0 && CanToStand && _cbShips.Items.Count > 0 &&
                    Int32.TryParse(_cbShips.SelectedItem.ToString().Split(' ')[0], out int shipSize)) {
                    if (IsRight) {
                        for (int i = 0; i < shipSize; i++) {
                            Dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + i].Style.BackColor = ShipColor;
                            Dgv.Rows[e.RowIndex].Cells[e.ColumnIndex + i].Style.SelectionBackColor = ShipColor;
                            PlaceAroundShip(e.RowIndex, e.ColumnIndex + i);
                        }
                    }
                    else {
                        for (int i = 0; i < shipSize; i++) {
                            Dgv.Rows[e.RowIndex + i].Cells[e.ColumnIndex].Style.BackColor = ShipColor;
                            Dgv.Rows[e.RowIndex + i].Cells[e.ColumnIndex].Style.SelectionBackColor = ShipColor;
                            PlaceAroundShip(e.RowIndex + i, e.ColumnIndex);
                        }
                    }

                    _cbShips.Items.RemoveAt(_cbShips.SelectedIndex);
                    if (_cbShips.Items.Count > 0) {
                        _cbShips.SelectedItem = _cbShips.Items[0];
                    }
                    else {
                        _cbShips.Text = "";
                    }
                }
            }
        }

        private void PlaceAroundShip(int rowIndex, int colIndex) {
            for (int i = -1; i < 2; i++) {
                for (int j = -1; j < 2; j++) {
                    int currRowIndex = rowIndex + i;
                    int currColIndex = colIndex + j;
                    if (currRowIndex >= 0 && currRowIndex < Dgv.RowCount &&
                        currColIndex >= 0 && currColIndex < Dgv.ColumnCount &&
                        Dgv.Rows[currRowIndex].Cells[currColIndex].Style.BackColor != ShipColor) {
                        Dgv.Rows[currRowIndex].Cells[currColIndex].Style.BackColor = AroundShipColor;
                        Dgv.Rows[currRowIndex].Cells[currColIndex].Style.SelectionBackColor = AroundShipColor;
                    }
                }
            }
        }

        public void ClearField() {
            _cbShips.Items.Clear();
            for (int i = 0; i < _ships.Length; i++) {
                _cbShips.Items.Add(_ships[i]);
            }

            _cbShips.SelectedIndex = 0;

            for (int i = 0; i < Dgv.RowCount; i++) {
                for (int j = 0; j < Dgv.ColumnCount; j++) {
                    Dgv.Rows[i].Cells[j].Style.BackColor = DefaultFieldColor;
                    Dgv.Rows[i].Cells[j].Style.SelectionBackColor = DefaultFieldColor;
                }
            }
        }

        public async Task ReadyClick() {
            IsReady = !IsReady;

            if (IsReady) {
                await _messageManager.SendMessageAsync("ready", _nickname, WriteField());
            }
            else {
                await _messageManager.SendMessageAsync("notReady", _nickname);
            }
        }
    }
}