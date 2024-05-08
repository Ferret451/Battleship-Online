using System.ComponentModel;

namespace Client {
    partial class MenuForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.butCreateRoom = new System.Windows.Forms.Button();
            this.butConnect = new System.Windows.Forms.Button();
            this.butQuit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.tbNickname = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // butCreateRoom
            // 
            this.butCreateRoom.Font = new System.Drawing.Font("Ink Free", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butCreateRoom.Location = new System.Drawing.Point(300, 251);
            this.butCreateRoom.Name = "butCreateRoom";
            this.butCreateRoom.Size = new System.Drawing.Size(200, 40);
            this.butCreateRoom.TabIndex = 0;
            this.butCreateRoom.Text = "Create room";
            this.butCreateRoom.UseVisualStyleBackColor = true;
            this.butCreateRoom.Click += new System.EventHandler(this.butCreateRoom_Click);
            // 
            // butConnect
            // 
            this.butConnect.Font = new System.Drawing.Font("Ink Free", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butConnect.Location = new System.Drawing.Point(300, 324);
            this.butConnect.Name = "butConnect";
            this.butConnect.Size = new System.Drawing.Size(200, 40);
            this.butConnect.TabIndex = 1;
            this.butConnect.Text = "Connect";
            this.butConnect.UseVisualStyleBackColor = true;
            this.butConnect.Click += new System.EventHandler(this.butConnect_Click);
            // 
            // butQuit
            // 
            this.butQuit.Font = new System.Drawing.Font("Ink Free", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.butQuit.Location = new System.Drawing.Point(300, 398);
            this.butQuit.Name = "butQuit";
            this.butQuit.Size = new System.Drawing.Size(200, 40);
            this.butQuit.TabIndex = 2;
            this.butQuit.Text = "Quit";
            this.butQuit.UseVisualStyleBackColor = true;
            this.butQuit.Click += new System.EventHandler(this.butQuit_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Ink Free", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(300, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 34);
            this.label1.TabIndex = 3;
            this.label1.Text = "Nickname:";
            // 
            // tbNickname
            // 
            this.tbNickname.Font = new System.Drawing.Font("Ink Free", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tbNickname.Location = new System.Drawing.Point(300, 75);
            this.tbNickname.Name = "tbNickname";
            this.tbNickname.Size = new System.Drawing.Size(200, 41);
            this.tbNickname.TabIndex = 4;
            // 
            // MenuForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tbNickname);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.butQuit);
            this.Controls.Add(this.butConnect);
            this.Controls.Add(this.butCreateRoom);
            this.Name = "MenuForm";
            this.Text = "Menu";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MenuForm_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.TextBox tbNickname;

        private System.Windows.Forms.Label label1;

        private System.Windows.Forms.Button butCreateRoom;
        private System.Windows.Forms.Button butConnect;

        private System.Windows.Forms.Button butQuit;

        #endregion
    }
}