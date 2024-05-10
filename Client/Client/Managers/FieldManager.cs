using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Client {
    public abstract class FieldManager {
        protected readonly DataGridView Dgv;
        public Color ShipColor { get; } = Color.Black;
        public  Color AroundShipColor { get; } = Color.Gray;
        public  Color MissColor { get; } = Color.CadetBlue;
        public  Color HitColor { get; } = Color.Yellow;
        public  Color DestroyColor { get; } = Color.DarkRed;
        public  Color DefaultFieldColor { get; private set;  }

        protected bool IsRight = true;
        protected bool CanToStand = true;
        
        private readonly Color _canToStandColor = Color.LawnGreen;
        private readonly Color _cannotToStandColor = Color.Red;

        protected FieldManager(DataGridView dgv) {
            Dgv = dgv;
            
            DefaultFieldColor = dgv.DefaultCellStyle.BackColor;
        }
        
        protected void CellMouseEnter(int shipSize, int rowIndex, int columIndex) {
            Color color = _canToStandColor;
            CanToStand = true;
            if (IsRight) {
                if (columIndex + shipSize > Dgv.ColumnCount || IsShipOrAround(rowIndex, columIndex, shipSize)) {
                    color = _cannotToStandColor;
                    CanToStand = false;
                }

                for (int i = 0; i < shipSize; i++) {
                    if (columIndex + i < Dgv.ColumnCount) {
                        Color cellColor = Dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor;
                        if (cellColor == DefaultFieldColor) {
                            Dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor = color;
                            Dgv.Rows[rowIndex].Cells[columIndex + i].Style.SelectionBackColor = color;
                        }
                    }
                }
            }
            else {
                if (rowIndex + shipSize > Dgv.RowCount || IsShipOrAround(rowIndex, columIndex, shipSize)) {
                    color = _cannotToStandColor;
                    CanToStand = false;
                }

                for (int i = 0; i < shipSize; i++) {
                    if (rowIndex + i < Dgv.RowCount) {
                        Color cellColor = Dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor;
                        if (cellColor == DefaultFieldColor) {
                            Dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor = color;
                            Dgv.Rows[rowIndex + i].Cells[columIndex].Style.SelectionBackColor = color;
                        }
                    }
                }
            }
        }
        
        private bool IsShipOrAround(int rowIndex, int colIndex, int shipSize) {
            if (IsRight) {
                for (int i = 0; i < shipSize; i++) {
                    Color color = Dgv.Rows[rowIndex].Cells[colIndex + i].Style.BackColor;
                    if (color == ShipColor || color == AroundShipColor) {
                        return true;
                    }
                }
            }
            else {
                for (int i = 0; i < shipSize; i++) {
                    Color color = Dgv.Rows[rowIndex + i].Cells[colIndex].Style.BackColor;
                    if (color == ShipColor || color == AroundShipColor) {
                        return true;
                    }
                }
            }

            return false;
        }

        protected void CellMouseLeave(int shipSize, int rowIndex, int columIndex) {
            if (IsRight) {
                for (int i = 0; i < shipSize; i++) {
                    if (columIndex + i < Dgv.ColumnCount) {
                        Color cellColor = Dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor;
                        if (cellColor == _canToStandColor || cellColor == _cannotToStandColor) {
                            Dgv.Rows[rowIndex].Cells[columIndex + i].Style.BackColor = DefaultFieldColor;
                            Dgv.Rows[rowIndex].Cells[columIndex + i].Style.SelectionBackColor = DefaultFieldColor;
                        }
                    }
                }
            }
            else {

                for (int i = 0; i < shipSize; i++) {
                    if (rowIndex + i < Dgv.RowCount) {
                        Color cellColor = Dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor;
                        if (cellColor == _canToStandColor || cellColor == _cannotToStandColor) {
                            Dgv.Rows[rowIndex + i].Cells[columIndex].Style.BackColor = DefaultFieldColor;
                            Dgv.Rows[rowIndex + i].Cells[columIndex].Style.SelectionBackColor = DefaultFieldColor;
                        }
                    }
                }
            }
        }
        
        public string WriteField() {
            StringBuilder str = new StringBuilder("");

            for (int i = 0; i < Dgv.RowCount; i++) {
                for (int j = 0; j < Dgv.ColumnCount; j++) {
                    Color color = Dgv.Rows[i].Cells[j].Style.BackColor;
                    if (color == DefaultFieldColor) {
                        str.Append(0);
                    } else if (color == ShipColor) {
                        str.Append(1);
                    } else if (color == AroundShipColor) {
                        str.Append(2);
                    }
                }
            }

            return str.ToString();
        }
        
        public void ReadField(string fieldStr) {
            var strIndex = 0;

            for (int i = 0; i < Dgv.RowCount; i++) {
                for (int j = 0; j < Dgv.ColumnCount; j++) {
                    if (fieldStr[strIndex] == '0') {
                        Dgv.Rows[i].Cells[j].Style.BackColor = DefaultFieldColor;
                        Dgv.Rows[i].Cells[j].Style.SelectionBackColor = DefaultFieldColor;
                    }
                    else if (fieldStr[strIndex] == '1') {
                        Dgv.Rows[i].Cells[j].Style.BackColor = ShipColor;
                        Dgv.Rows[i].Cells[j].Style.SelectionBackColor = ShipColor;
                    }
                    else if (fieldStr[strIndex] == '2') {
                        Dgv.Rows[i].Cells[j].Style.BackColor = AroundShipColor;
                        Dgv.Rows[i].Cells[j].Style.SelectionBackColor = AroundShipColor;
                    }
                    else if (fieldStr[strIndex] == '3') {
                        Dgv.Rows[i].Cells[j].Style.BackColor = MissColor;
                        Dgv.Rows[i].Cells[j].Style.SelectionBackColor = MissColor;
                    }
                    else if (fieldStr[strIndex] == '4') {
                        Dgv.Rows[i].Cells[j].Style.BackColor = HitColor;
                        Dgv.Rows[i].Cells[j].Style.SelectionBackColor = HitColor;
                    }
                    else if (fieldStr[strIndex] == '5') {
                        Dgv.Rows[i].Cells[j].Style.BackColor = DestroyColor;
                        Dgv.Rows[i].Cells[j].Style.SelectionBackColor = DestroyColor;
                    }
                    
                    strIndex++;
                }
            }
        }
    }
}