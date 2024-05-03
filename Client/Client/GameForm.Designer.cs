namespace Client {
    partial class GameForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.tbMessages = new System.Windows.Forms.TextBox();
            this.lbMessages = new System.Windows.Forms.ListBox();
            this.butSend = new System.Windows.Forms.Button();
            this.butRecieve = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tbMessages
            // 
            this.tbMessages.Location = new System.Drawing.Point(40, 27);
            this.tbMessages.Multiline = true;
            this.tbMessages.Name = "tbMessages";
            this.tbMessages.Size = new System.Drawing.Size(292, 300);
            this.tbMessages.TabIndex = 0;
            // 
            // lbMessages
            // 
            this.lbMessages.FormattingEnabled = true;
            this.lbMessages.Location = new System.Drawing.Point(395, 27);
            this.lbMessages.Name = "lbMessages";
            this.lbMessages.Size = new System.Drawing.Size(341, 303);
            this.lbMessages.TabIndex = 1;
            // 
            // butSend
            // 
            this.butSend.Location = new System.Drawing.Point(132, 380);
            this.butSend.Name = "butSend";
            this.butSend.Size = new System.Drawing.Size(104, 23);
            this.butSend.TabIndex = 2;
            this.butSend.Text = "Send";
            this.butSend.UseVisualStyleBackColor = true;
            this.butSend.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // butRecieve
            // 
            this.butRecieve.Location = new System.Drawing.Point(523, 380);
            this.butRecieve.Name = "butRecieve";
            this.butRecieve.Size = new System.Drawing.Size(75, 23);
            this.butRecieve.TabIndex = 3;
            this.butRecieve.Text = "Recieve";
            this.butRecieve.UseVisualStyleBackColor = true;
            this.butRecieve.Click += new System.EventHandler(this.ReceiveMessageButton_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.butRecieve);
            this.Controls.Add(this.butSend);
            this.Controls.Add(this.lbMessages);
            this.Controls.Add(this.tbMessages);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "GameForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button butSend;
        private System.Windows.Forms.Button butRecieve;

        private System.Windows.Forms.ListBox lbMessages;

        private System.Windows.Forms.TextBox tbMessages;

        #endregion
    }
}