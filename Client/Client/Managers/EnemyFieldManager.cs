using System.Windows.Forms;

namespace Client {
    public class EnemyFieldManager : FieldManager {
        private readonly MessageManager _messageManager;
        private readonly string _nickname;

        public EnemyFieldManager(DataGridView dgv, string nickname, MessageManager messageManager) : base(dgv) {
            _messageManager = messageManager;
            _nickname = nickname;
        }
        
        public void CellMouseEnter(int rowIndex, int columIndex) {
            base.CellMouseEnter(1, rowIndex, columIndex);
        }

        public void CellMouseLeave(int rowIndex, int columIndex) {
            base.CellMouseLeave(1, rowIndex, columIndex);
        }

        public async void CellMouseDown(int rowIndex, int colIndex) {
            await _messageManager.SendMessageAsync("shot", _nickname, $"{rowIndex} {colIndex}");
        }
    }
}