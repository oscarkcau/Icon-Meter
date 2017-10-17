namespace IconMeter
{
    partial class FormMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
			this.timerMain = new System.Windows.Forms.Timer(this.components);
			this.notifyIconMain = new System.Windows.Forms.NotifyIcon(this.components);
			this.contextMenuStripMain = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonCpuColor = new System.Windows.Forms.Button();
			this.checkBoxMemory = new System.Windows.Forms.CheckBox();
			this.buttonMemoryColor = new System.Windows.Forms.Button();
			this.checkBoxDisk = new System.Windows.Forms.CheckBox();
			this.checkBoxNetwork = new System.Windows.Forms.CheckBox();
			this.buttonDiskColor = new System.Windows.Forms.Button();
			this.buttonReceiveColor = new System.Windows.Forms.Button();
			this.buttonSendColor = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.colorDialogMain = new System.Windows.Forms.ColorDialog();
			this.checkBoxUseVerticalBar = new System.Windows.Forms.CheckBox();
			this.checkBoxRunAtStartup = new System.Windows.Forms.CheckBox();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.contextMenuStripMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerMain
			// 
			this.timerMain.Interval = 1000;
			this.timerMain.Tick += new System.EventHandler(this.timerMain_Tick);
			// 
			// notifyIconMain
			// 
			this.notifyIconMain.BalloonTipTitle = "HIHI";
			this.notifyIconMain.ContextMenuStrip = this.contextMenuStripMain;
			this.notifyIconMain.Text = "FUCK";
			this.notifyIconMain.Visible = true;
			this.notifyIconMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconMain_MouseDoubleClick);
			// 
			// contextMenuStripMain
			// 
			this.contextMenuStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
			this.contextMenuStripMain.Name = "contextMenuStripMain";
			this.contextMenuStripMain.Size = new System.Drawing.Size(161, 70);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(160, 30);
			this.settingsToolStripMenuItem.Text = "Settings...";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(157, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(160, 30);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(36, 15);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 20);
			this.label1.TabIndex = 1;
			this.label1.Text = "CPU";
			// 
			// buttonCpuColor
			// 
			this.buttonCpuColor.Location = new System.Drawing.Point(131, 11);
			this.buttonCpuColor.Name = "buttonCpuColor";
			this.buttonCpuColor.Size = new System.Drawing.Size(75, 23);
			this.buttonCpuColor.TabIndex = 2;
			this.buttonCpuColor.UseVisualStyleBackColor = true;
			this.buttonCpuColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxMemory
			// 
			this.checkBoxMemory.AutoSize = true;
			this.checkBoxMemory.Location = new System.Drawing.Point(12, 41);
			this.checkBoxMemory.Name = "checkBoxMemory";
			this.checkBoxMemory.Size = new System.Drawing.Size(91, 24);
			this.checkBoxMemory.TabIndex = 3;
			this.checkBoxMemory.Text = "Memory";
			this.checkBoxMemory.UseVisualStyleBackColor = true;
			// 
			// buttonMemoryColor
			// 
			this.buttonMemoryColor.Location = new System.Drawing.Point(131, 41);
			this.buttonMemoryColor.Name = "buttonMemoryColor";
			this.buttonMemoryColor.Size = new System.Drawing.Size(75, 23);
			this.buttonMemoryColor.TabIndex = 4;
			this.buttonMemoryColor.UseVisualStyleBackColor = true;
			this.buttonMemoryColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxDisk
			// 
			this.checkBoxDisk.AutoSize = true;
			this.checkBoxDisk.Location = new System.Drawing.Point(12, 71);
			this.checkBoxDisk.Name = "checkBoxDisk";
			this.checkBoxDisk.Size = new System.Drawing.Size(66, 24);
			this.checkBoxDisk.TabIndex = 5;
			this.checkBoxDisk.Text = "Disk";
			this.checkBoxDisk.UseVisualStyleBackColor = true;
			// 
			// checkBoxNetwork
			// 
			this.checkBoxNetwork.AutoSize = true;
			this.checkBoxNetwork.Location = new System.Drawing.Point(12, 101);
			this.checkBoxNetwork.Name = "checkBoxNetwork";
			this.checkBoxNetwork.Size = new System.Drawing.Size(93, 24);
			this.checkBoxNetwork.TabIndex = 6;
			this.checkBoxNetwork.Text = "Network";
			this.checkBoxNetwork.UseVisualStyleBackColor = true;
			// 
			// buttonDiskColor
			// 
			this.buttonDiskColor.Location = new System.Drawing.Point(131, 71);
			this.buttonDiskColor.Name = "buttonDiskColor";
			this.buttonDiskColor.Size = new System.Drawing.Size(75, 23);
			this.buttonDiskColor.TabIndex = 7;
			this.buttonDiskColor.UseVisualStyleBackColor = true;
			this.buttonDiskColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonReceiveColor
			// 
			this.buttonReceiveColor.Location = new System.Drawing.Point(131, 101);
			this.buttonReceiveColor.Name = "buttonReceiveColor";
			this.buttonReceiveColor.Size = new System.Drawing.Size(75, 23);
			this.buttonReceiveColor.TabIndex = 8;
			this.buttonReceiveColor.Text = "Receive";
			this.buttonReceiveColor.UseVisualStyleBackColor = true;
			this.buttonReceiveColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonSendColor
			// 
			this.buttonSendColor.Location = new System.Drawing.Point(212, 102);
			this.buttonSendColor.Name = "buttonSendColor";
			this.buttonSendColor.Size = new System.Drawing.Size(75, 23);
			this.buttonSendColor.TabIndex = 9;
			this.buttonSendColor.Text = "Send";
			this.buttonSendColor.UseVisualStyleBackColor = true;
			this.buttonSendColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(168, 257);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 10;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// checkBoxUseVerticalBar
			// 
			this.checkBoxUseVerticalBar.AutoSize = true;
			this.checkBoxUseVerticalBar.Location = new System.Drawing.Point(12, 131);
			this.checkBoxUseVerticalBar.Name = "checkBoxUseVerticalBar";
			this.checkBoxUseVerticalBar.Size = new System.Drawing.Size(148, 24);
			this.checkBoxUseVerticalBar.TabIndex = 11;
			this.checkBoxUseVerticalBar.Text = "Use Vertical bar";
			this.checkBoxUseVerticalBar.UseVisualStyleBackColor = true;
			// 
			// checkBoxRunAtStartup
			// 
			this.checkBoxRunAtStartup.AutoSize = true;
			this.checkBoxRunAtStartup.Location = new System.Drawing.Point(12, 161);
			this.checkBoxRunAtStartup.Name = "checkBoxRunAtStartup";
			this.checkBoxRunAtStartup.Size = new System.Drawing.Size(137, 24);
			this.checkBoxRunAtStartup.TabIndex = 12;
			this.checkBoxRunAtStartup.Text = "Run at startup";
			this.checkBoxRunAtStartup.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(249, 257);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(336, 292);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.checkBoxRunAtStartup);
			this.Controls.Add(this.checkBoxUseVerticalBar);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.buttonSendColor);
			this.Controls.Add(this.buttonReceiveColor);
			this.Controls.Add(this.buttonDiskColor);
			this.Controls.Add(this.checkBoxNetwork);
			this.Controls.Add(this.checkBoxDisk);
			this.Controls.Add(this.buttonMemoryColor);
			this.Controls.Add(this.checkBoxMemory);
			this.Controls.Add(this.buttonCpuColor);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Icon Meter";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Shown += new System.EventHandler(this.FormMain_Shown);
			this.contextMenuStripMain.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

		private System.Windows.Forms.Timer timerMain;
		private System.Windows.Forms.NotifyIcon notifyIconMain;
		private System.Windows.Forms.ContextMenuStrip contextMenuStripMain;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonCpuColor;
		private System.Windows.Forms.CheckBox checkBoxMemory;
		private System.Windows.Forms.Button buttonMemoryColor;
		private System.Windows.Forms.CheckBox checkBoxDisk;
		private System.Windows.Forms.CheckBox checkBoxNetwork;
		private System.Windows.Forms.Button buttonDiskColor;
		private System.Windows.Forms.Button buttonReceiveColor;
		private System.Windows.Forms.Button buttonSendColor;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ColorDialog colorDialogMain;
		private System.Windows.Forms.CheckBox checkBoxUseVerticalBar;
		private System.Windows.Forms.CheckBox checkBoxRunAtStartup;
		private System.Windows.Forms.Button buttonCancel;
	}
}

