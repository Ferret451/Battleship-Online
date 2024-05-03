using System;
using System.Windows.Forms;

namespace Client {
    public partial class JoinForm : Form {
        public string RoomCode { get; private set; }

        public JoinForm() {
            InitializeComponent();
        }

        private void butOK_Click(object sender, EventArgs e) {
            RoomCode = tbRoomCode.Text;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}