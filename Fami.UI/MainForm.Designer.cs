
namespace Fami.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadROMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.changeSlotToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot5ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot6ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot7ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot8ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot9ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.slot10ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveStateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controllersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gamepadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowSizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(575, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadROMToolStripMenuItem,
            this.recentToolStripMenuItem,
            this.toolStripMenuItem1,
            this.changeSlotToolStripMenuItem,
            this.loadStateToolStripMenuItem,
            this.saveStateToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // loadROMToolStripMenuItem
            // 
            this.loadROMToolStripMenuItem.Name = "loadROMToolStripMenuItem";
            this.loadROMToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.loadROMToolStripMenuItem.Text = "&Load ROM";
            this.loadROMToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // recentToolStripMenuItem
            // 
            this.recentToolStripMenuItem.Name = "recentToolStripMenuItem";
            this.recentToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.recentToolStripMenuItem.Text = "Recent";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(135, 6);
            // 
            // changeSlotToolStripMenuItem
            // 
            this.changeSlotToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.slot1ToolStripMenuItem,
            this.slot2ToolStripMenuItem,
            this.slot3ToolStripMenuItem,
            this.slot4ToolStripMenuItem,
            this.slot5ToolStripMenuItem,
            this.slot6ToolStripMenuItem,
            this.slot7ToolStripMenuItem,
            this.slot8ToolStripMenuItem,
            this.slot9ToolStripMenuItem,
            this.slot10ToolStripMenuItem});
            this.changeSlotToolStripMenuItem.Enabled = false;
            this.changeSlotToolStripMenuItem.Name = "changeSlotToolStripMenuItem";
            this.changeSlotToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.changeSlotToolStripMenuItem.Text = "Change Slot";
            // 
            // slot1ToolStripMenuItem
            // 
            this.slot1ToolStripMenuItem.Name = "slot1ToolStripMenuItem";
            this.slot1ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot1ToolStripMenuItem.Text = "Slot #1";
            // 
            // slot2ToolStripMenuItem
            // 
            this.slot2ToolStripMenuItem.Name = "slot2ToolStripMenuItem";
            this.slot2ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot2ToolStripMenuItem.Text = "Slot #2";
            // 
            // slot3ToolStripMenuItem
            // 
            this.slot3ToolStripMenuItem.Name = "slot3ToolStripMenuItem";
            this.slot3ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot3ToolStripMenuItem.Text = "Slot #3";
            // 
            // slot4ToolStripMenuItem
            // 
            this.slot4ToolStripMenuItem.Name = "slot4ToolStripMenuItem";
            this.slot4ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot4ToolStripMenuItem.Text = "Slot #4";
            // 
            // slot5ToolStripMenuItem
            // 
            this.slot5ToolStripMenuItem.Name = "slot5ToolStripMenuItem";
            this.slot5ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot5ToolStripMenuItem.Text = "Slot #5";
            // 
            // slot6ToolStripMenuItem
            // 
            this.slot6ToolStripMenuItem.Name = "slot6ToolStripMenuItem";
            this.slot6ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot6ToolStripMenuItem.Text = "Slot #6";
            // 
            // slot7ToolStripMenuItem
            // 
            this.slot7ToolStripMenuItem.Name = "slot7ToolStripMenuItem";
            this.slot7ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot7ToolStripMenuItem.Text = "Slot #7";
            // 
            // slot8ToolStripMenuItem
            // 
            this.slot8ToolStripMenuItem.Name = "slot8ToolStripMenuItem";
            this.slot8ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot8ToolStripMenuItem.Text = "Slot #8";
            // 
            // slot9ToolStripMenuItem
            // 
            this.slot9ToolStripMenuItem.Name = "slot9ToolStripMenuItem";
            this.slot9ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot9ToolStripMenuItem.Text = "Slot #9";
            // 
            // slot10ToolStripMenuItem
            // 
            this.slot10ToolStripMenuItem.Name = "slot10ToolStripMenuItem";
            this.slot10ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.slot10ToolStripMenuItem.Text = "Slot #10";
            // 
            // loadStateToolStripMenuItem
            // 
            this.loadStateToolStripMenuItem.Enabled = false;
            this.loadStateToolStripMenuItem.Name = "loadStateToolStripMenuItem";
            this.loadStateToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.loadStateToolStripMenuItem.Text = "Load State";
            // 
            // saveStateToolStripMenuItem
            // 
            this.saveStateToolStripMenuItem.Enabled = false;
            this.saveStateToolStripMenuItem.Name = "saveStateToolStripMenuItem";
            this.saveStateToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.saveStateToolStripMenuItem.Text = "Save State";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(135, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(138, 22);
            this.exitToolStripMenuItem.Text = "E&xit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controllersToolStripMenuItem,
            this.windowSizeToolStripMenuItem});
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.optionsToolStripMenuItem.Text = "Options";
            // 
            // controllersToolStripMenuItem
            // 
            this.controllersToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.keyboardToolStripMenuItem,
            this.gamepadToolStripMenuItem});
            this.controllersToolStripMenuItem.Name = "controllersToolStripMenuItem";
            this.controllersToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.controllersToolStripMenuItem.Text = "Controllers";
            // 
            // keyboardToolStripMenuItem
            // 
            this.keyboardToolStripMenuItem.Name = "keyboardToolStripMenuItem";
            this.keyboardToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.keyboardToolStripMenuItem.Text = "Keyboard";
            this.keyboardToolStripMenuItem.Click += new System.EventHandler(this.keyboardToolStripMenuItem_Click);
            // 
            // gamepadToolStripMenuItem
            // 
            this.gamepadToolStripMenuItem.Name = "gamepadToolStripMenuItem";
            this.gamepadToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.gamepadToolStripMenuItem.Text = "Gamepad";
            // 
            // windowSizeToolStripMenuItem
            // 
            this.windowSizeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.x1ToolStripMenuItem,
            this.x2ToolStripMenuItem,
            this.x3ToolStripMenuItem,
            this.x4ToolStripMenuItem});
            this.windowSizeToolStripMenuItem.Name = "windowSizeToolStripMenuItem";
            this.windowSizeToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.windowSizeToolStripMenuItem.Text = "Window Size";
            // 
            // x1ToolStripMenuItem
            // 
            this.x1ToolStripMenuItem.Name = "x1ToolStripMenuItem";
            this.x1ToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.x1ToolStripMenuItem.Text = "x1";
            // 
            // x2ToolStripMenuItem
            // 
            this.x2ToolStripMenuItem.Name = "x2ToolStripMenuItem";
            this.x2ToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.x2ToolStripMenuItem.Text = "x2";
            // 
            // x3ToolStripMenuItem
            // 
            this.x3ToolStripMenuItem.Name = "x3ToolStripMenuItem";
            this.x3ToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.x3ToolStripMenuItem.Text = "x3";
            // 
            // x4ToolStripMenuItem
            // 
            this.x4ToolStripMenuItem.Name = "x4ToolStripMenuItem";
            this.x4ToolStripMenuItem.Size = new System.Drawing.Size(86, 22);
            this.x4ToolStripMenuItem.Text = "x4";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            this.ClientSize = new System.Drawing.Size(575, 535);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Fami";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadROMToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem changeSlotToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveStateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot5ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot6ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot7ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot8ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot9ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem slot10ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem recentToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controllersToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gamepadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowSizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x4ToolStripMenuItem;
    }
}

