namespace MEAClosedLoop
{
    partial class Form1
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
      this.buttonStartDAQ = new System.Windows.Forms.Button();
      this.buttonStop = new System.Windows.Forms.Button();
      this.labelAmpl1 = new System.Windows.Forms.Label();
      this.labelAmpl2 = new System.Windows.Forms.Label();
      this.buttonOpen = new System.Windows.Forms.Button();
      this.checkBox_Manual = new System.Windows.Forms.CheckBox();
      this.button_Next = new System.Windows.Forms.Button();
      this.comboBox_DAQs = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.comboBox_Stimulators = new System.Windows.Forms.ComboBox();
      this.buttonCalibrate = new System.Windows.Forms.Button();
      this.buttonClosedLoop = new System.Windows.Forms.Button();
      this.label_time = new System.Windows.Forms.Label();
      this.button_integral0 = new System.Windows.Forms.Button();
      this.showChannelData = new System.Windows.Forms.Button();
      this.DisplayData = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.button1 = new System.Windows.Forms.Button();
      this.PackStatButton = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.buttonStatWindow = new System.Windows.Forms.Button();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label_refreshRate = new System.Windows.Forms.Label();
      this.textBox_DeviceInfo = new System.Windows.Forms.TextBox();
      this.DisplayData.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.SuspendLayout();
      // 
      // buttonStartDAQ
      // 
      this.buttonStartDAQ.Enabled = false;
      this.buttonStartDAQ.Location = new System.Drawing.Point(547, 10);
      this.buttonStartDAQ.Name = "buttonStartDAQ";
      this.buttonStartDAQ.Size = new System.Drawing.Size(75, 23);
      this.buttonStartDAQ.TabIndex = 11;
      this.buttonStartDAQ.Text = "Start DAQ";
      this.buttonStartDAQ.UseVisualStyleBackColor = true;
      this.buttonStartDAQ.Click += new System.EventHandler(this.buttonStartDAQ_Click);
      // 
      // buttonStop
      // 
      this.buttonStop.Enabled = false;
      this.buttonStop.Location = new System.Drawing.Point(628, 10);
      this.buttonStop.Name = "buttonStop";
      this.buttonStop.Size = new System.Drawing.Size(75, 23);
      this.buttonStop.TabIndex = 12;
      this.buttonStop.Text = "Stop";
      this.buttonStop.UseVisualStyleBackColor = true;
      this.buttonStop.Click += new System.EventHandler(this.buttonStop_Click);
      // 
      // labelAmpl1
      // 
      this.labelAmpl1.Location = new System.Drawing.Point(339, 44);
      this.labelAmpl1.Name = "labelAmpl1";
      this.labelAmpl1.Size = new System.Drawing.Size(38, 17);
      this.labelAmpl1.TabIndex = 19;
      this.labelAmpl1.Click += new System.EventHandler(this.labelAmpl1_Click);
      // 
      // labelAmpl2
      // 
      this.labelAmpl2.Location = new System.Drawing.Point(463, 47);
      this.labelAmpl2.Name = "labelAmpl2";
      this.labelAmpl2.Size = new System.Drawing.Size(38, 17);
      this.labelAmpl2.TabIndex = 20;
      this.labelAmpl2.Click += new System.EventHandler(this.labelAmpl2_Click);
      // 
      // buttonOpen
      // 
      this.buttonOpen.Location = new System.Drawing.Point(466, 10);
      this.buttonOpen.Name = "buttonOpen";
      this.buttonOpen.Size = new System.Drawing.Size(75, 23);
      this.buttonOpen.TabIndex = 11;
      this.buttonOpen.Text = "Open";
      this.buttonOpen.UseVisualStyleBackColor = true;
      this.buttonOpen.Click += new System.EventHandler(this.buttonOpen_Click);
      // 
      // checkBox_Manual
      // 
      this.checkBox_Manual.AutoSize = true;
      this.checkBox_Manual.Location = new System.Drawing.Point(393, 15);
      this.checkBox_Manual.Name = "checkBox_Manual";
      this.checkBox_Manual.Size = new System.Drawing.Size(15, 14);
      this.checkBox_Manual.TabIndex = 21;
      this.checkBox_Manual.UseVisualStyleBackColor = true;
      this.checkBox_Manual.CheckedChanged += new System.EventHandler(this.checkBox_Manual_CheckedChanged);
      // 
      // button_Next
      // 
      this.button_Next.Enabled = false;
      this.button_Next.Location = new System.Drawing.Point(414, 10);
      this.button_Next.Name = "button_Next";
      this.button_Next.Size = new System.Drawing.Size(40, 23);
      this.button_Next.TabIndex = 22;
      this.button_Next.Text = "Next";
      this.button_Next.UseVisualStyleBackColor = true;
      this.button_Next.Click += new System.EventHandler(this.button_Next_Click);
      // 
      // comboBox_DAQs
      // 
      this.comboBox_DAQs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox_DAQs.DropDownWidth = 250;
      this.comboBox_DAQs.FormattingEnabled = true;
      this.comboBox_DAQs.Location = new System.Drawing.Point(86, 12);
      this.comboBox_DAQs.Name = "comboBox_DAQs";
      this.comboBox_DAQs.Size = new System.Drawing.Size(151, 21);
      this.comboBox_DAQs.TabIndex = 25;
      this.comboBox_DAQs.SelectedIndexChanged += new System.EventHandler(this.comboBox_DAQs_SelectedIndexChanged);
      this.comboBox_DAQs.Click += new System.EventHandler(this.comboBox_DAQs_Click);
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(12, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(78, 19);
      this.label1.TabIndex = 23;
      this.label1.Text = "DAQ System:";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(12, 45);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(78, 19);
      this.label2.TabIndex = 23;
      this.label2.Text = "Stimulator:";
      // 
      // comboBox_Stimulators
      // 
      this.comboBox_Stimulators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.comboBox_Stimulators.FormattingEnabled = true;
      this.comboBox_Stimulators.Location = new System.Drawing.Point(86, 42);
      this.comboBox_Stimulators.Name = "comboBox_Stimulators";
      this.comboBox_Stimulators.Size = new System.Drawing.Size(151, 21);
      this.comboBox_Stimulators.TabIndex = 25;
      this.comboBox_Stimulators.SelectedIndexChanged += new System.EventHandler(this.comboBox_Stimulators_SelectedIndexChanged);
      this.comboBox_Stimulators.Click += new System.EventHandler(this.comboBox_Stimulators_Click);
      // 
      // buttonCalibrate
      // 
      this.buttonCalibrate.Location = new System.Drawing.Point(258, 11);
      this.buttonCalibrate.Name = "buttonCalibrate";
      this.buttonCalibrate.Size = new System.Drawing.Size(87, 23);
      this.buttonCalibrate.TabIndex = 22;
      this.buttonCalibrate.Text = "Calibrate";
      this.buttonCalibrate.UseVisualStyleBackColor = true;
      this.buttonCalibrate.Click += new System.EventHandler(this.buttonCalibrate_Click);
      // 
      // buttonClosedLoop
      // 
      this.buttonClosedLoop.Location = new System.Drawing.Point(547, 45);
      this.buttonClosedLoop.Name = "buttonClosedLoop";
      this.buttonClosedLoop.Size = new System.Drawing.Size(75, 23);
      this.buttonClosedLoop.TabIndex = 11;
      this.buttonClosedLoop.Text = "Start Loop";
      this.buttonClosedLoop.UseVisualStyleBackColor = true;
      this.buttonClosedLoop.Click += new System.EventHandler(this.buttonClosedLoop_Click);
      // 
      // label_time
      // 
      this.label_time.Location = new System.Drawing.Point(628, 50);
      this.label_time.Name = "label_time";
      this.label_time.Size = new System.Drawing.Size(72, 17);
      this.label_time.TabIndex = 23;
      this.label_time.Text = "Time";
      // 
      // button_integral0
      // 
      this.button_integral0.Location = new System.Drawing.Point(414, 39);
      this.button_integral0.Name = "button_integral0";
      this.button_integral0.Size = new System.Drawing.Size(40, 23);
      this.button_integral0.TabIndex = 22;
      this.button_integral0.Text = "0";
      this.button_integral0.UseVisualStyleBackColor = true;
      this.button_integral0.Click += new System.EventHandler(this.button_integral0_Click);
      // 
      // showChannelData
      // 
      this.showChannelData.Enabled = false;
      this.showChannelData.Location = new System.Drawing.Point(59, 19);
      this.showChannelData.Name = "showChannelData";
      this.showChannelData.Size = new System.Drawing.Size(87, 23);
      this.showChannelData.TabIndex = 29;
      this.showChannelData.Text = "Show";
      this.showChannelData.UseVisualStyleBackColor = true;
      this.showChannelData.Click += new System.EventHandler(this.showChannelData_Click);
      // 
      // DisplayData
      // 
      this.DisplayData.Controls.Add(this.showChannelData);
      this.DisplayData.Location = new System.Drawing.Point(569, 69);
      this.DisplayData.Name = "DisplayData";
      this.DisplayData.Size = new System.Drawing.Size(170, 48);
      this.DisplayData.TabIndex = 30;
      this.DisplayData.TabStop = false;
      this.DisplayData.Text = "Display MChannel data";
      this.DisplayData.Enter += new System.EventHandler(this.DisplayData_Enter);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.button1);
      this.groupBox2.Location = new System.Drawing.Point(191, 69);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(170, 48);
      this.groupBox2.TabIndex = 31;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Recorder";
      this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(67, 19);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "Open";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.OpenRecorder_Click);
      // 
      // PackStatButton
      // 
      this.PackStatButton.Enabled = false;
      this.PackStatButton.Location = new System.Drawing.Point(68, 19);
      this.PackStatButton.Name = "PackStatButton";
      this.PackStatButton.Size = new System.Drawing.Size(87, 23);
      this.PackStatButton.TabIndex = 27;
      this.PackStatButton.Text = "Pack Stat";
      this.PackStatButton.UseVisualStyleBackColor = true;
      this.PackStatButton.Click += new System.EventHandler(this.PackStatButton_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.PackStatButton);
      this.groupBox1.Location = new System.Drawing.Point(371, 69);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(170, 48);
      this.groupBox1.TabIndex = 31;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "LearnExp";
      this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
      // 
      // buttonStatWindow
      // 
      this.buttonStatWindow.Enabled = false;
      this.buttonStatWindow.Location = new System.Drawing.Point(69, 19);
      this.buttonStatWindow.Name = "buttonStatWindow";
      this.buttonStatWindow.Size = new System.Drawing.Size(88, 23);
      this.buttonStatWindow.TabIndex = 26;
      this.buttonStatWindow.Text = "Stat";
      this.buttonStatWindow.UseVisualStyleBackColor = true;
      this.buttonStatWindow.Click += new System.EventHandler(this.buttonStatWindow_Click);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.buttonStatWindow);
      this.groupBox3.Location = new System.Drawing.Point(15, 69);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(170, 48);
      this.groupBox3.TabIndex = 31;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Debug";
      this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
      // 
      // label_refreshRate
      // 
      this.label_refreshRate.Location = new System.Drawing.Point(261, 45);
      this.label_refreshRate.Name = "label_refreshRate";
      this.label_refreshRate.Size = new System.Drawing.Size(72, 17);
      this.label_refreshRate.TabIndex = 23;
      this.label_refreshRate.Text = "Rerfesh Rate";
      // 
      // textBox_DeviceInfo
      // 
      this.textBox_DeviceInfo.Location = new System.Drawing.Point(754, 1);
      this.textBox_DeviceInfo.Multiline = true;
      this.textBox_DeviceInfo.Name = "textBox_DeviceInfo";
      this.textBox_DeviceInfo.Size = new System.Drawing.Size(10, 10);
      this.textBox_DeviceInfo.TabIndex = 14;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(765, 125);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.DisplayData);
      this.Controls.Add(this.comboBox_Stimulators);
      this.Controls.Add(this.comboBox_DAQs);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label_time);
      this.Controls.Add(this.label_refreshRate);
      this.Controls.Add(this.buttonCalibrate);
      this.Controls.Add(this.button_integral0);
      this.Controls.Add(this.button_Next);
      this.Controls.Add(this.checkBox_Manual);
      this.Controls.Add(this.labelAmpl2);
      this.Controls.Add(this.labelAmpl1);
      this.Controls.Add(this.textBox_DeviceInfo);
      this.Controls.Add(this.buttonStop);
      this.Controls.Add(this.buttonOpen);
      this.Controls.Add(this.buttonClosedLoop);
      this.Controls.Add(this.buttonStartDAQ);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.Name = "Form1";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "Device && Loop";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.Load += new System.EventHandler(this.Form1_Load);
      this.DisplayData.ResumeLayout(false);
      this.groupBox2.ResumeLayout(false);
      this.groupBox1.ResumeLayout(false);
      this.groupBox3.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonStartDAQ;
        private System.Windows.Forms.Button buttonStop;
        private System.Windows.Forms.Label labelAmpl1;
        private System.Windows.Forms.Label labelAmpl2;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.CheckBox checkBox_Manual;
        private System.Windows.Forms.Button button_Next;
        private System.Windows.Forms.ComboBox comboBox_DAQs;
        private System.Windows.Forms.ComboBox comboBox_Stimulators;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonCalibrate;
        private System.Windows.Forms.Button buttonClosedLoop;
        private System.Windows.Forms.Label label_time;
        private System.Windows.Forms.Button button_integral0;
        private System.Windows.Forms.Button showChannelData;
        private System.Windows.Forms.GroupBox DisplayData;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button PackStatButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button buttonStatWindow;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label_refreshRate;
        private System.Windows.Forms.TextBox textBox_DeviceInfo;
    }
}

