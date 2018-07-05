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
			this.resetIconMeterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
			this.labelSeparationBar3 = new System.Windows.Forms.Label();
			this.notifyIconLogicalProcessor = new System.Windows.Forms.NotifyIcon(this.components);
			this.labelSeparationBar1 = new System.Windows.Forms.Label();
			this.checkBoxLogicalProcessors = new System.Windows.Forms.CheckBox();
			this.labelSeparationBar2 = new System.Windows.Forms.Label();
			this.buttonLogicalProcessorsColor = new System.Windows.Forms.Button();
			this.checkBoxCpu = new System.Windows.Forms.CheckBox();
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
			this.notifyIconMain.ContextMenuStrip = this.contextMenuStripMain;
			this.notifyIconMain.Visible = true;
			this.notifyIconMain.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconMain_MouseDoubleClick);
			// 
			// contextMenuStripMain
			// 
			this.contextMenuStripMain.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.contextMenuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.settingsToolStripMenuItem,
            this.resetIconMeterToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.toolStripMenuItem1,
            this.closeToolStripMenuItem});
			this.contextMenuStripMain.Name = "contextMenuStripMain";
			this.contextMenuStripMain.Size = new System.Drawing.Size(279, 154);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("settingsToolStripMenuItem.Image")));
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(278, 36);
			this.settingsToolStripMenuItem.Text = "Settings...";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// resetIconMeterToolStripMenuItem
			// 
			this.resetIconMeterToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("resetIconMeterToolStripMenuItem.Image")));
			this.resetIconMeterToolStripMenuItem.Name = "resetIconMeterToolStripMenuItem";
			this.resetIconMeterToolStripMenuItem.Size = new System.Drawing.Size(278, 36);
			this.resetIconMeterToolStripMenuItem.Text = "Reset Icon Meter";
			this.resetIconMeterToolStripMenuItem.Click += new System.EventHandler(this.resetIconMeterToolStripMenuItem_Click);
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("aboutToolStripMenuItem.Image")));
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(278, 36);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			this.toolStripMenuItem1.Name = "toolStripMenuItem1";
			this.toolStripMenuItem1.Size = new System.Drawing.Size(275, 6);
			// 
			// closeToolStripMenuItem
			// 
			this.closeToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("closeToolStripMenuItem.Image")));
			this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
			this.closeToolStripMenuItem.Size = new System.Drawing.Size(278, 36);
			this.closeToolStripMenuItem.Text = "Close";
			this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
			// 
			// buttonCpuColor
			// 
			this.buttonCpuColor.Location = new System.Drawing.Point(310, 19);
			this.buttonCpuColor.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCpuColor.Name = "buttonCpuColor";
			this.buttonCpuColor.Size = new System.Drawing.Size(132, 46);
			this.buttonCpuColor.TabIndex = 2;
			this.buttonCpuColor.UseVisualStyleBackColor = true;
			this.buttonCpuColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxMemory
			// 
			this.checkBoxMemory.AutoSize = true;
			this.checkBoxMemory.Location = new System.Drawing.Point(22, 83);
			this.checkBoxMemory.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxMemory.Name = "checkBoxMemory";
			this.checkBoxMemory.Size = new System.Drawing.Size(121, 29);
			this.checkBoxMemory.TabIndex = 3;
			this.checkBoxMemory.Text = "Memory";
			this.checkBoxMemory.UseVisualStyleBackColor = true;
			// 
			// buttonMemoryColor
			// 
			this.buttonMemoryColor.Location = new System.Drawing.Point(310, 73);
			this.buttonMemoryColor.Margin = new System.Windows.Forms.Padding(4);
			this.buttonMemoryColor.Name = "buttonMemoryColor";
			this.buttonMemoryColor.Size = new System.Drawing.Size(132, 46);
			this.buttonMemoryColor.TabIndex = 4;
			this.buttonMemoryColor.UseVisualStyleBackColor = true;
			this.buttonMemoryColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxDisk
			// 
			this.checkBoxDisk.AutoSize = true;
			this.checkBoxDisk.Location = new System.Drawing.Point(22, 137);
			this.checkBoxDisk.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxDisk.Name = "checkBoxDisk";
			this.checkBoxDisk.Size = new System.Drawing.Size(86, 29);
			this.checkBoxDisk.TabIndex = 5;
			this.checkBoxDisk.Text = "Disk";
			this.checkBoxDisk.UseVisualStyleBackColor = true;
			// 
			// checkBoxNetwork
			// 
			this.checkBoxNetwork.AutoSize = true;
			this.checkBoxNetwork.Location = new System.Drawing.Point(22, 190);
			this.checkBoxNetwork.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxNetwork.Name = "checkBoxNetwork";
			this.checkBoxNetwork.Size = new System.Drawing.Size(122, 29);
			this.checkBoxNetwork.TabIndex = 6;
			this.checkBoxNetwork.Text = "Network";
			this.checkBoxNetwork.UseVisualStyleBackColor = true;
			// 
			// buttonDiskColor
			// 
			this.buttonDiskColor.Location = new System.Drawing.Point(310, 127);
			this.buttonDiskColor.Margin = new System.Windows.Forms.Padding(4);
			this.buttonDiskColor.Name = "buttonDiskColor";
			this.buttonDiskColor.Size = new System.Drawing.Size(132, 46);
			this.buttonDiskColor.TabIndex = 7;
			this.buttonDiskColor.UseVisualStyleBackColor = true;
			this.buttonDiskColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonReceiveColor
			// 
			this.buttonReceiveColor.Location = new System.Drawing.Point(170, 181);
			this.buttonReceiveColor.Margin = new System.Windows.Forms.Padding(4);
			this.buttonReceiveColor.Name = "buttonReceiveColor";
			this.buttonReceiveColor.Size = new System.Drawing.Size(132, 46);
			this.buttonReceiveColor.TabIndex = 8;
			this.buttonReceiveColor.Text = "Receive";
			this.buttonReceiveColor.UseVisualStyleBackColor = true;
			this.buttonReceiveColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonSendColor
			// 
			this.buttonSendColor.Location = new System.Drawing.Point(310, 181);
			this.buttonSendColor.Margin = new System.Windows.Forms.Padding(4);
			this.buttonSendColor.Name = "buttonSendColor";
			this.buttonSendColor.Size = new System.Drawing.Size(132, 46);
			this.buttonSendColor.TabIndex = 9;
			this.buttonSendColor.Text = "Send";
			this.buttonSendColor.UseVisualStyleBackColor = true;
			this.buttonSendColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(186, 441);
			this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(150, 44);
			this.buttonOK.TabIndex = 10;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// checkBoxUseVerticalBar
			// 
			this.checkBoxUseVerticalBar.AutoSize = true;
			this.checkBoxUseVerticalBar.Location = new System.Drawing.Point(22, 332);
			this.checkBoxUseVerticalBar.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxUseVerticalBar.Name = "checkBoxUseVerticalBar";
			this.checkBoxUseVerticalBar.Size = new System.Drawing.Size(197, 29);
			this.checkBoxUseVerticalBar.TabIndex = 11;
			this.checkBoxUseVerticalBar.Text = "Use Vertical bar";
			this.checkBoxUseVerticalBar.UseVisualStyleBackColor = true;
			// 
			// checkBoxRunAtStartup
			// 
			this.checkBoxRunAtStartup.AutoSize = true;
			this.checkBoxRunAtStartup.Location = new System.Drawing.Point(22, 372);
			this.checkBoxRunAtStartup.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxRunAtStartup.Name = "checkBoxRunAtStartup";
			this.checkBoxRunAtStartup.Size = new System.Drawing.Size(179, 29);
			this.checkBoxRunAtStartup.TabIndex = 12;
			this.checkBoxRunAtStartup.Text = "Run at startup";
			this.checkBoxRunAtStartup.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.Location = new System.Drawing.Point(344, 441);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(4);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(150, 44);
			this.buttonCancel.TabIndex = 13;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelSeparationBar3
			// 
			this.labelSeparationBar3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSeparationBar3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelSeparationBar3.Location = new System.Drawing.Point(18, 421);
			this.labelSeparationBar3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.labelSeparationBar3.Name = "labelSeparationBar3";
			this.labelSeparationBar3.Size = new System.Drawing.Size(478, 4);
			this.labelSeparationBar3.TabIndex = 14;
			// 
			// notifyIconLogicalProcessor
			// 
			this.notifyIconLogicalProcessor.ContextMenuStrip = this.contextMenuStripMain;
			this.notifyIconLogicalProcessor.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIconLogicalProcessor.Icon")));
			this.notifyIconLogicalProcessor.Text = "notifyIconLogicalProcessor";
			this.notifyIconLogicalProcessor.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIconMain_MouseDoubleClick);
			// 
			// labelSeparationBar1
			// 
			this.labelSeparationBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSeparationBar1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelSeparationBar1.Location = new System.Drawing.Point(19, 241);
			this.labelSeparationBar1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.labelSeparationBar1.Name = "labelSeparationBar1";
			this.labelSeparationBar1.Size = new System.Drawing.Size(478, 4);
			this.labelSeparationBar1.TabIndex = 15;
			// 
			// checkBoxLogicalProcessors
			// 
			this.checkBoxLogicalProcessors.AutoSize = true;
			this.checkBoxLogicalProcessors.Location = new System.Drawing.Point(22, 265);
			this.checkBoxLogicalProcessors.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxLogicalProcessors.Name = "checkBoxLogicalProcessors";
			this.checkBoxLogicalProcessors.Size = new System.Drawing.Size(227, 29);
			this.checkBoxLogicalProcessors.TabIndex = 16;
			this.checkBoxLogicalProcessors.Text = "Logical Processors";
			this.checkBoxLogicalProcessors.UseVisualStyleBackColor = true;
			// 
			// labelSeparationBar2
			// 
			this.labelSeparationBar2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSeparationBar2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelSeparationBar2.Location = new System.Drawing.Point(19, 316);
			this.labelSeparationBar2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
			this.labelSeparationBar2.Name = "labelSeparationBar2";
			this.labelSeparationBar2.Size = new System.Drawing.Size(478, 4);
			this.labelSeparationBar2.TabIndex = 17;
			// 
			// buttonLogicalProcessorsColor
			// 
			this.buttonLogicalProcessorsColor.Location = new System.Drawing.Point(310, 255);
			this.buttonLogicalProcessorsColor.Margin = new System.Windows.Forms.Padding(4);
			this.buttonLogicalProcessorsColor.Name = "buttonLogicalProcessorsColor";
			this.buttonLogicalProcessorsColor.Size = new System.Drawing.Size(132, 46);
			this.buttonLogicalProcessorsColor.TabIndex = 18;
			this.buttonLogicalProcessorsColor.UseVisualStyleBackColor = true;
			this.buttonLogicalProcessorsColor.Click += new System.EventHandler(this.buttonColor_Click);
			// 
			// checkBoxCpu
			// 
			this.checkBoxCpu.AutoSize = true;
			this.checkBoxCpu.Location = new System.Drawing.Point(22, 29);
			this.checkBoxCpu.Margin = new System.Windows.Forms.Padding(4);
			this.checkBoxCpu.Name = "checkBoxCpu";
			this.checkBoxCpu.Size = new System.Drawing.Size(88, 29);
			this.checkBoxCpu.TabIndex = 19;
			this.checkBoxCpu.Text = "CPU";
			this.checkBoxCpu.UseVisualStyleBackColor = true;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(516, 506);
			this.Controls.Add(this.checkBoxCpu);
			this.Controls.Add(this.buttonLogicalProcessorsColor);
			this.Controls.Add(this.labelSeparationBar2);
			this.Controls.Add(this.checkBoxLogicalProcessors);
			this.Controls.Add(this.labelSeparationBar1);
			this.Controls.Add(this.labelSeparationBar3);
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
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Margin = new System.Windows.Forms.Padding(4);
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
		private System.Windows.Forms.Label labelSeparationBar3;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetIconMeterToolStripMenuItem;
		private System.Windows.Forms.NotifyIcon notifyIconLogicalProcessor;
		private System.Windows.Forms.Label labelSeparationBar1;
		private System.Windows.Forms.CheckBox checkBoxLogicalProcessors;
		private System.Windows.Forms.Label labelSeparationBar2;
		private System.Windows.Forms.Button buttonLogicalProcessorsColor;
		private System.Windows.Forms.CheckBox checkBoxCpu;
	}
}

