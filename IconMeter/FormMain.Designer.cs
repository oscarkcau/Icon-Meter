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
			this.label2 = new System.Windows.Forms.Label();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
			this.contextMenuStripMain.Name = "contextMenuStripMain";
			this.contextMenuStripMain.Size = new System.Drawing.Size(161, 122);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStripMenuItem.Image")));
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
			this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(160, 30);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(29, 16);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "CPU";
			// 
			// buttonCpuColor
			// 
			this.buttonCpuColor.Location = new System.Drawing.Point(90, 10);
			this.buttonCpuColor.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCpuColor.Name = "buttonCpuColor";
			this.buttonCpuColor.Size = new System.Drawing.Size(66, 24);
			this.buttonCpuColor.TabIndex = 2;
			this.buttonCpuColor.UseVisualStyleBackColor = true;
			this.buttonCpuColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxMemory
			// 
			this.checkBoxMemory.AutoSize = true;
			this.checkBoxMemory.Location = new System.Drawing.Point(11, 43);
			this.checkBoxMemory.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxMemory.Name = "checkBoxMemory";
			this.checkBoxMemory.Size = new System.Drawing.Size(63, 17);
			this.checkBoxMemory.TabIndex = 3;
			this.checkBoxMemory.Text = "Memory";
			this.checkBoxMemory.UseVisualStyleBackColor = true;
			// 
			// buttonMemoryColor
			// 
			this.buttonMemoryColor.Location = new System.Drawing.Point(90, 38);
			this.buttonMemoryColor.Margin = new System.Windows.Forms.Padding(2);
			this.buttonMemoryColor.Name = "buttonMemoryColor";
			this.buttonMemoryColor.Size = new System.Drawing.Size(66, 24);
			this.buttonMemoryColor.TabIndex = 4;
			this.buttonMemoryColor.UseVisualStyleBackColor = true;
			this.buttonMemoryColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxDisk
			// 
			this.checkBoxDisk.AutoSize = true;
			this.checkBoxDisk.Location = new System.Drawing.Point(11, 71);
			this.checkBoxDisk.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxDisk.Name = "checkBoxDisk";
			this.checkBoxDisk.Size = new System.Drawing.Size(47, 17);
			this.checkBoxDisk.TabIndex = 5;
			this.checkBoxDisk.Text = "Disk";
			this.checkBoxDisk.UseVisualStyleBackColor = true;
			// 
			// checkBoxNetwork
			// 
			this.checkBoxNetwork.AutoSize = true;
			this.checkBoxNetwork.Location = new System.Drawing.Point(11, 99);
			this.checkBoxNetwork.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxNetwork.Name = "checkBoxNetwork";
			this.checkBoxNetwork.Size = new System.Drawing.Size(66, 17);
			this.checkBoxNetwork.TabIndex = 6;
			this.checkBoxNetwork.Text = "Network";
			this.checkBoxNetwork.UseVisualStyleBackColor = true;
			// 
			// buttonDiskColor
			// 
			this.buttonDiskColor.Location = new System.Drawing.Point(90, 66);
			this.buttonDiskColor.Margin = new System.Windows.Forms.Padding(2);
			this.buttonDiskColor.Name = "buttonDiskColor";
			this.buttonDiskColor.Size = new System.Drawing.Size(66, 24);
			this.buttonDiskColor.TabIndex = 7;
			this.buttonDiskColor.UseVisualStyleBackColor = true;
			this.buttonDiskColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonReceiveColor
			// 
			this.buttonReceiveColor.Location = new System.Drawing.Point(90, 94);
			this.buttonReceiveColor.Margin = new System.Windows.Forms.Padding(2);
			this.buttonReceiveColor.Name = "buttonReceiveColor";
			this.buttonReceiveColor.Size = new System.Drawing.Size(66, 24);
			this.buttonReceiveColor.TabIndex = 8;
			this.buttonReceiveColor.Text = "Receive";
			this.buttonReceiveColor.UseVisualStyleBackColor = true;
			this.buttonReceiveColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonSendColor
			// 
			this.buttonSendColor.Location = new System.Drawing.Point(160, 94);
			this.buttonSendColor.Margin = new System.Windows.Forms.Padding(2);
			this.buttonSendColor.Name = "buttonSendColor";
			this.buttonSendColor.Size = new System.Drawing.Size(66, 24);
			this.buttonSendColor.TabIndex = 9;
			this.buttonSendColor.Text = "Send";
			this.buttonSendColor.UseVisualStyleBackColor = true;
			this.buttonSendColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(93, 199);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(2);
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
			this.checkBoxUseVerticalBar.Location = new System.Drawing.Point(11, 133);
			this.checkBoxUseVerticalBar.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxUseVerticalBar.Name = "checkBoxUseVerticalBar";
			this.checkBoxUseVerticalBar.Size = new System.Drawing.Size(101, 17);
			this.checkBoxUseVerticalBar.TabIndex = 11;
			this.checkBoxUseVerticalBar.Text = "Use Vertical bar";
			this.checkBoxUseVerticalBar.UseVisualStyleBackColor = true;
			// 
			// checkBoxRunAtStartup
			// 
			this.checkBoxRunAtStartup.AutoSize = true;
			this.checkBoxRunAtStartup.Location = new System.Drawing.Point(11, 154);
			this.checkBoxRunAtStartup.Margin = new System.Windows.Forms.Padding(2);
			this.checkBoxRunAtStartup.Name = "checkBoxRunAtStartup";
			this.checkBoxRunAtStartup.Size = new System.Drawing.Size(93, 17);
			this.checkBoxRunAtStartup.TabIndex = 12;
			this.checkBoxRunAtStartup.Text = "Run at startup";
			this.checkBoxRunAtStartup.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(172, 199);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(2);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label2.Location = new System.Drawing.Point(9, 189);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(239, 2);
			this.label2.TabIndex = 14;
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(160, 30);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(258, 233);
			this.Controls.Add(this.label2);
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
			this.Margin = new System.Windows.Forms.Padding(2);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Icon Meter";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
			this.Load += new System.EventHandler(this.FormMain_Load);
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
	}
}

