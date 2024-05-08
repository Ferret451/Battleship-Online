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
            this.panChat = new System.Windows.Forms.Panel();
            this.panSend = new System.Windows.Forms.Panel();
            this.butSend = new System.Windows.Forms.Button();
            this.tbMessage = new System.Windows.Forms.TextBox();
            this.tbChat = new System.Windows.Forms.TextBox();
            this.butFullScreen = new System.Windows.Forms.Button();
            this.butChat = new System.Windows.Forms.Button();
            this.panChat.SuspendLayout();
            this.panSend.SuspendLayout();
            this.SuspendLayout();
            // 
            // panChat
            // 
            this.panChat.Controls.Add(this.panSend);
            this.panChat.Controls.Add(this.tbChat);
            this.panChat.Dock = System.Windows.Forms.DockStyle.Right;
            this.panChat.Location = new System.Drawing.Point(572, 0);
            this.panChat.Name = "panChat";
            this.panChat.Size = new System.Drawing.Size(228, 450);
            this.panChat.TabIndex = 5;
            // 
            // panSend
            // 
            this.panSend.Controls.Add(this.butSend);
            this.panSend.Controls.Add(this.tbMessage);
            this.panSend.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panSend.Location = new System.Drawing.Point(0, 382);
            this.panSend.Name = "panSend";
            this.panSend.Size = new System.Drawing.Size(228, 68);
            this.panSend.TabIndex = 7;
            // 
            // butSend
            // 
            this.butSend.Dock = System.Windows.Forms.DockStyle.Right;
            this.butSend.Image = global::Client.Properties.Resources.send;
            this.butSend.Location = new System.Drawing.Point(183, 0);
            this.butSend.Name = "butSend";
            this.butSend.Size = new System.Drawing.Size(45, 68);
            this.butSend.TabIndex = 5;
            this.butSend.UseVisualStyleBackColor = true;
            this.butSend.Click += new System.EventHandler(this.SendMessageButton_Click);
            // 
            // tbMessage
            // 
            this.tbMessage.Dock = System.Windows.Forms.DockStyle.Left;
            this.tbMessage.Font = new System.Drawing.Font("Ink Free", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbMessage.Location = new System.Drawing.Point(0, 0);
            this.tbMessage.Multiline = true;
            this.tbMessage.Name = "tbMessage";
            this.tbMessage.Size = new System.Drawing.Size(183, 68);
            this.tbMessage.TabIndex = 6;
            this.tbMessage.TextChanged += new System.EventHandler(this.tbMessage_TextChanged);
            this.tbMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbMessage_KeyDown);
            // 
            // tbChat
            // 
            this.tbChat.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbChat.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbChat.Font = new System.Drawing.Font("Ink Free", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbChat.Location = new System.Drawing.Point(0, 0);
            this.tbChat.Multiline = true;
            this.tbChat.Name = "tbChat";
            this.tbChat.ReadOnly = true;
            this.tbChat.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbChat.Size = new System.Drawing.Size(228, 382);
            this.tbChat.TabIndex = 4;
            // 
            // butFullScreen
            // 
            this.butFullScreen.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butFullScreen.Image = global::Client.Properties.Resources.fullscreen;
            this.butFullScreen.Location = new System.Drawing.Point(12, 406);
            this.butFullScreen.Name = "butFullScreen";
            this.butFullScreen.Size = new System.Drawing.Size(32, 32);
            this.butFullScreen.TabIndex = 6;
            this.butFullScreen.UseVisualStyleBackColor = true;
            this.butFullScreen.Click += new System.EventHandler(this.butFullScreen_Click);
            // 
            // butChat
            // 
            this.butChat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.butChat.Image = global::Client.Properties.Resources.chat;
            this.butChat.Location = new System.Drawing.Point(50, 406);
            this.butChat.Name = "butChat";
            this.butChat.Size = new System.Drawing.Size(32, 32);
            this.butChat.TabIndex = 7;
            this.butChat.UseVisualStyleBackColor = true;
            this.butChat.Click += new System.EventHandler(this.butChat_Click);
            // 
            // GameForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.butChat);
            this.Controls.Add(this.butFullScreen);
            this.Controls.Add(this.panChat);
            this.Location = new System.Drawing.Point(15, 15);
            this.Name = "GameForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Load += new System.EventHandler(this.GameForm_Load);
            this.SizeChanged += new System.EventHandler(this.GameForm_SizeChanged);
            this.panChat.ResumeLayout(false);
            this.panChat.PerformLayout();
            this.panSend.ResumeLayout(false);
            this.panSend.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Button butFullScreen;
        private System.Windows.Forms.Button butChat;

        private System.Windows.Forms.Panel panChat;

        private System.Windows.Forms.Panel panSend;

        private System.Windows.Forms.TextBox tbMessage;

        private System.Windows.Forms.Button butSend;

        private System.Windows.Forms.TextBox tbChat;

        #endregion
    }
}