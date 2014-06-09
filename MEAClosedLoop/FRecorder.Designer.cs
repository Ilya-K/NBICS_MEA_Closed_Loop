namespace MEAClosedLoop
{
  partial class FRecorder
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
      this.StartButton = new System.Windows.Forms.Button();
      this.StopButton = new System.Windows.Forms.Button();
      this.CreateDB = new System.Windows.Forms.Button();
      this.button4 = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label9 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.DeselectAllRecs = new System.Windows.Forms.Button();
      this.SelectAllRecords = new System.Windows.Forms.Button();
      this.DoRecordStimData = new System.Windows.Forms.CheckBox();
      this.DoRecordPackData = new System.Windows.Forms.CheckBox();
      this.DoRecordCompressData = new System.Windows.Forms.CheckBox();
      this.DoRecordCmpData = new System.Windows.Forms.CheckBox();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.CreateMeasureButton = new System.Windows.Forms.Button();
      this.RecordTimeElapsed = new System.Windows.Forms.Label();
      this.button11 = new System.Windows.Forms.Button();
      this.ManageMeasure = new System.Windows.Forms.Button();
      this.OpenExp = new System.Windows.Forms.Button();
      this.OpenDB = new System.Windows.Forms.Button();
      this.CreateExp = new System.Windows.Forms.Button();
      this.ManageExp = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.PlayPosition = new System.Windows.Forms.TrackBar();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.label7 = new System.Windows.Forms.Label();
      this.button8 = new System.Windows.Forms.Button();
      this.prev = new System.Windows.Forms.Button();
      this.TimeElapsed = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.button2 = new System.Windows.Forms.Button();
      this.button6 = new System.Windows.Forms.Button();
      this.button5 = new System.Windows.Forms.Button();
      this.button7 = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.InfoBar = new System.Windows.Forms.StatusStrip();
      this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
      this.BDStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.ExpStateLabel = new System.Windows.Forms.ToolStripStatusLabel();
      this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
      this.groupBox1.SuspendLayout();
      this.groupBox3.SuspendLayout();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PlayPosition)).BeginInit();
      this.groupBox4.SuspendLayout();
      this.InfoBar.SuspendLayout();
      this.SuspendLayout();
      // 
      // StartButton
      // 
      this.StartButton.Location = new System.Drawing.Point(100, 19);
      this.StartButton.Name = "StartButton";
      this.StartButton.Size = new System.Drawing.Size(75, 23);
      this.StartButton.TabIndex = 0;
      this.StartButton.Text = "start";
      this.StartButton.UseVisualStyleBackColor = true;
      this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
      // 
      // StopButton
      // 
      this.StopButton.Location = new System.Drawing.Point(184, 19);
      this.StopButton.Name = "StopButton";
      this.StopButton.Size = new System.Drawing.Size(75, 23);
      this.StopButton.TabIndex = 1;
      this.StopButton.Text = "stop";
      this.StopButton.UseVisualStyleBackColor = true;
      this.StopButton.Click += new System.EventHandler(this.StopButton_Click);
      // 
      // CreateDB
      // 
      this.CreateDB.Location = new System.Drawing.Point(80, 29);
      this.CreateDB.Name = "CreateDB";
      this.CreateDB.Size = new System.Drawing.Size(59, 23);
      this.CreateDB.TabIndex = 2;
      this.CreateDB.Text = "Create";
      this.CreateDB.UseVisualStyleBackColor = true;
      this.CreateDB.Click += new System.EventHandler(this.CreateDB_Click);
      // 
      // button4
      // 
      this.button4.Location = new System.Drawing.Point(21, 19);
      this.button4.Name = "button4";
      this.button4.Size = new System.Drawing.Size(198, 23);
      this.button4.TabIndex = 3;
      this.button4.Text = "Load Experiment";
      this.button4.UseVisualStyleBackColor = true;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label9);
      this.groupBox1.Controls.Add(this.label8);
      this.groupBox1.Controls.Add(this.groupBox3);
      this.groupBox1.Controls.Add(this.button11);
      this.groupBox1.Controls.Add(this.ManageMeasure);
      this.groupBox1.Controls.Add(this.OpenExp);
      this.groupBox1.Controls.Add(this.OpenDB);
      this.groupBox1.Controls.Add(this.CreateExp);
      this.groupBox1.Controls.Add(this.CreateDB);
      this.groupBox1.Controls.Add(this.ManageExp);
      this.groupBox1.Location = new System.Drawing.Point(13, 13);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(515, 175);
      this.groupBox1.TabIndex = 4;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Record";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(21, 60);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(59, 13);
      this.label9.TabIndex = 4;
      this.label9.Text = "Experiment";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(21, 34);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(53, 13);
      this.label8.TabIndex = 4;
      this.label8.Text = "Database";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.DeselectAllRecs);
      this.groupBox3.Controls.Add(this.SelectAllRecords);
      this.groupBox3.Controls.Add(this.DoRecordStimData);
      this.groupBox3.Controls.Add(this.DoRecordPackData);
      this.groupBox3.Controls.Add(this.DoRecordCompressData);
      this.groupBox3.Controls.Add(this.DoRecordCmpData);
      this.groupBox3.Controls.Add(this.label6);
      this.groupBox3.Controls.Add(this.StopButton);
      this.groupBox3.Controls.Add(this.label5);
      this.groupBox3.Controls.Add(this.CreateMeasureButton);
      this.groupBox3.Controls.Add(this.StartButton);
      this.groupBox3.Controls.Add(this.RecordTimeElapsed);
      this.groupBox3.Location = new System.Drawing.Point(241, 9);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(268, 160);
      this.groupBox3.TabIndex = 3;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Measure";
      // 
      // DeselectAllRecs
      // 
      this.DeselectAllRecs.Location = new System.Drawing.Point(184, 125);
      this.DeselectAllRecs.Name = "DeselectAllRecs";
      this.DeselectAllRecs.Size = new System.Drawing.Size(75, 23);
      this.DeselectAllRecs.TabIndex = 10;
      this.DeselectAllRecs.Text = "Deselect All";
      this.DeselectAllRecs.UseVisualStyleBackColor = true;
      this.DeselectAllRecs.Click += new System.EventHandler(this.DeselectAllRecs_Click);
      // 
      // SelectAllRecords
      // 
      this.SelectAllRecords.Location = new System.Drawing.Point(184, 96);
      this.SelectAllRecords.Name = "SelectAllRecords";
      this.SelectAllRecords.Size = new System.Drawing.Size(75, 23);
      this.SelectAllRecords.TabIndex = 9;
      this.SelectAllRecords.Text = "Select All";
      this.SelectAllRecords.UseVisualStyleBackColor = true;
      this.SelectAllRecords.Click += new System.EventHandler(this.SelectAllRecords_Click);
      // 
      // DoRecordStimData
      // 
      this.DoRecordStimData.AutoSize = true;
      this.DoRecordStimData.Location = new System.Drawing.Point(6, 137);
      this.DoRecordStimData.Name = "DoRecordStimData";
      this.DoRecordStimData.Size = new System.Drawing.Size(110, 17);
      this.DoRecordStimData.TabIndex = 8;
      this.DoRecordStimData.Text = "Record Stim Data";
      this.DoRecordStimData.UseVisualStyleBackColor = true;
      // 
      // DoRecordPackData
      // 
      this.DoRecordPackData.AutoSize = true;
      this.DoRecordPackData.Location = new System.Drawing.Point(6, 114);
      this.DoRecordPackData.Name = "DoRecordPackData";
      this.DoRecordPackData.Size = new System.Drawing.Size(120, 17);
      this.DoRecordPackData.TabIndex = 8;
      this.DoRecordPackData.Text = "Record Packs Data";
      this.DoRecordPackData.UseVisualStyleBackColor = true;
      // 
      // DoRecordCompressData
      // 
      this.DoRecordCompressData.AutoSize = true;
      this.DoRecordCompressData.Location = new System.Drawing.Point(6, 91);
      this.DoRecordCompressData.Name = "DoRecordCompressData";
      this.DoRecordCompressData.Size = new System.Drawing.Size(112, 17);
      this.DoRecordCompressData.TabIndex = 8;
      this.DoRecordCompressData.Text = "Record Raw Data";
      this.DoRecordCompressData.UseVisualStyleBackColor = true;
      // 
      // DoRecordCmpData
      // 
      this.DoRecordCmpData.AutoSize = true;
      this.DoRecordCmpData.Location = new System.Drawing.Point(6, 68);
      this.DoRecordCmpData.Name = "DoRecordCmpData";
      this.DoRecordCmpData.Size = new System.Drawing.Size(148, 17);
      this.DoRecordCmpData.TabIndex = 7;
      this.DoRecordCmpData.Text = "Record Compressed Data";
      this.DoRecordCmpData.UseVisualStyleBackColor = true;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(79, 45);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(19, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "00";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(97, 45);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(24, 13);
      this.label5.TabIndex = 5;
      this.label5.Text = "sec";
      // 
      // CreateMeasureButton
      // 
      this.CreateMeasureButton.Location = new System.Drawing.Point(6, 19);
      this.CreateMeasureButton.Name = "CreateMeasureButton";
      this.CreateMeasureButton.Size = new System.Drawing.Size(75, 23);
      this.CreateMeasureButton.TabIndex = 0;
      this.CreateMeasureButton.Text = "create";
      this.CreateMeasureButton.UseVisualStyleBackColor = true;
      this.CreateMeasureButton.Click += new System.EventHandler(this.CreateMeasureButton_Click);
      // 
      // RecordTimeElapsed
      // 
      this.RecordTimeElapsed.AutoSize = true;
      this.RecordTimeElapsed.Location = new System.Drawing.Point(3, 45);
      this.RecordTimeElapsed.Name = "RecordTimeElapsed";
      this.RecordTimeElapsed.Size = new System.Drawing.Size(69, 13);
      this.RecordTimeElapsed.TabIndex = 5;
      this.RecordTimeElapsed.Text = "time elapsed:";
      // 
      // button11
      // 
      this.button11.Location = new System.Drawing.Point(24, 142);
      this.button11.Name = "button11";
      this.button11.Size = new System.Drawing.Size(180, 23);
      this.button11.TabIndex = 0;
      this.button11.Text = "Database Managment";
      this.button11.UseVisualStyleBackColor = true;
      // 
      // ManageMeasure
      // 
      this.ManageMeasure.Location = new System.Drawing.Point(24, 113);
      this.ManageMeasure.Name = "ManageMeasure";
      this.ManageMeasure.Size = new System.Drawing.Size(180, 23);
      this.ManageMeasure.TabIndex = 0;
      this.ManageMeasure.Text = "Measure Managment";
      this.ManageMeasure.UseVisualStyleBackColor = true;
      // 
      // OpenExp
      // 
      this.OpenExp.Location = new System.Drawing.Point(145, 55);
      this.OpenExp.Name = "OpenExp";
      this.OpenExp.Size = new System.Drawing.Size(59, 23);
      this.OpenExp.TabIndex = 2;
      this.OpenExp.Text = "Open";
      this.OpenExp.UseVisualStyleBackColor = true;
      this.OpenExp.Click += new System.EventHandler(this.OpenExp_Click);
      // 
      // OpenDB
      // 
      this.OpenDB.Location = new System.Drawing.Point(145, 29);
      this.OpenDB.Name = "OpenDB";
      this.OpenDB.Size = new System.Drawing.Size(59, 23);
      this.OpenDB.TabIndex = 2;
      this.OpenDB.Text = "Open";
      this.OpenDB.UseVisualStyleBackColor = true;
      this.OpenDB.Click += new System.EventHandler(this.OpenDB_Click);
      // 
      // CreateExp
      // 
      this.CreateExp.Location = new System.Drawing.Point(80, 55);
      this.CreateExp.Name = "CreateExp";
      this.CreateExp.Size = new System.Drawing.Size(59, 23);
      this.CreateExp.TabIndex = 2;
      this.CreateExp.Text = "Create";
      this.CreateExp.UseVisualStyleBackColor = true;
      this.CreateExp.Click += new System.EventHandler(this.CreateExp_Click);
      // 
      // ManageExp
      // 
      this.ManageExp.Location = new System.Drawing.Point(24, 84);
      this.ManageExp.Name = "ManageExp";
      this.ManageExp.Size = new System.Drawing.Size(180, 23);
      this.ManageExp.TabIndex = 0;
      this.ManageExp.Text = "Experiment Managment";
      this.ManageExp.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.PlayPosition);
      this.groupBox2.Controls.Add(this.groupBox4);
      this.groupBox2.Controls.Add(this.button7);
      this.groupBox2.Controls.Add(this.button1);
      this.groupBox2.Controls.Add(this.button4);
      this.groupBox2.Location = new System.Drawing.Point(7, 245);
      this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Padding = new System.Windows.Forms.Padding(0);
      this.groupBox2.Size = new System.Drawing.Size(515, 145);
      this.groupBox2.TabIndex = 5;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Play";
      // 
      // PlayPosition
      // 
      this.PlayPosition.AutoSize = false;
      this.PlayPosition.Cursor = System.Windows.Forms.Cursors.VSplit;
      this.PlayPosition.Location = new System.Drawing.Point(4, 113);
      this.PlayPosition.Maximum = 1000;
      this.PlayPosition.Name = "PlayPosition";
      this.PlayPosition.Size = new System.Drawing.Size(508, 29);
      this.PlayPosition.TabIndex = 6;
      // 
      // groupBox4
      // 
      this.groupBox4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.groupBox4.Controls.Add(this.label7);
      this.groupBox4.Controls.Add(this.button8);
      this.groupBox4.Controls.Add(this.prev);
      this.groupBox4.Controls.Add(this.TimeElapsed);
      this.groupBox4.Controls.Add(this.label3);
      this.groupBox4.Controls.Add(this.label2);
      this.groupBox4.Controls.Add(this.textBox2);
      this.groupBox4.Controls.Add(this.textBox1);
      this.groupBox4.Controls.Add(this.label1);
      this.groupBox4.Controls.Add(this.button2);
      this.groupBox4.Controls.Add(this.button6);
      this.groupBox4.Controls.Add(this.button5);
      this.groupBox4.Location = new System.Drawing.Point(241, 12);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(268, 95);
      this.groupBox4.TabIndex = 5;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Measure";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(126, 24);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(62, 13);
      this.label7.TabIndex = 8;
      this.label7.Text = "Measure ID";
      // 
      // button8
      // 
      this.button8.Location = new System.Drawing.Point(51, 19);
      this.button8.Name = "button8";
      this.button8.Size = new System.Drawing.Size(44, 23);
      this.button8.TabIndex = 7;
      this.button8.Text = ">>";
      this.button8.UseVisualStyleBackColor = true;
      // 
      // prev
      // 
      this.prev.Enabled = false;
      this.prev.Location = new System.Drawing.Point(6, 19);
      this.prev.Name = "prev";
      this.prev.Size = new System.Drawing.Size(39, 23);
      this.prev.TabIndex = 7;
      this.prev.Text = "<<";
      this.prev.UseVisualStyleBackColor = true;
      // 
      // TimeElapsed
      // 
      this.TimeElapsed.AutoSize = true;
      this.TimeElapsed.Location = new System.Drawing.Point(82, 75);
      this.TimeElapsed.Name = "TimeElapsed";
      this.TimeElapsed.Size = new System.Drawing.Size(19, 13);
      this.TimeElapsed.TabIndex = 6;
      this.TimeElapsed.Text = "00";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(100, 75);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(24, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "sec";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 75);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(69, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "time elapsed:";
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(194, 21);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(32, 20);
      this.textBox2.TabIndex = 3;
      this.textBox2.Text = "0";
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(194, 50);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(32, 20);
      this.textBox1.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(152, 53);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(36, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "speed";
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(103, 48);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(43, 23);
      this.button2.TabIndex = 1;
      this.button2.Text = "stop";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // button6
      // 
      this.button6.Location = new System.Drawing.Point(51, 48);
      this.button6.Name = "button6";
      this.button6.Size = new System.Drawing.Size(46, 23);
      this.button6.TabIndex = 0;
      this.button6.Text = "pause";
      this.button6.UseVisualStyleBackColor = true;
      // 
      // button5
      // 
      this.button5.Location = new System.Drawing.Point(6, 48);
      this.button5.Name = "button5";
      this.button5.Size = new System.Drawing.Size(39, 23);
      this.button5.TabIndex = 0;
      this.button5.Text = "play";
      this.button5.UseVisualStyleBackColor = true;
      // 
      // button7
      // 
      this.button7.Location = new System.Drawing.Point(21, 77);
      this.button7.Name = "button7";
      this.button7.Size = new System.Drawing.Size(198, 23);
      this.button7.TabIndex = 4;
      this.button7.Text = "Show MChannelData";
      this.button7.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(21, 48);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(198, 23);
      this.button1.TabIndex = 4;
      this.button1.Text = "Load Measure";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // InfoBar
      // 
      this.InfoBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.BDStatus,
            this.ExpStateLabel,
            this.toolStripStatusLabel2});
      this.InfoBar.Location = new System.Drawing.Point(0, 432);
      this.InfoBar.Name = "InfoBar";
      this.InfoBar.Size = new System.Drawing.Size(554, 22);
      this.InfoBar.TabIndex = 6;
      this.InfoBar.Text = "Info:";
      // 
      // toolStripStatusLabel1
      // 
      this.toolStripStatusLabel1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
      this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
      this.toolStripStatusLabel1.Size = new System.Drawing.Size(54, 17);
      this.toolStripStatusLabel1.Text = "BD State:";
      // 
      // BDStatus
      // 
      this.BDStatus.ForeColor = System.Drawing.Color.DarkRed;
      this.BDStatus.Name = "BDStatus";
      this.BDStatus.Size = new System.Drawing.Size(78, 17);
      this.BDStatus.Text = "disconnected";
      // 
      // ExpStateLabel
      // 
      this.ExpStateLabel.Name = "ExpStateLabel";
      this.ExpStateLabel.Size = new System.Drawing.Size(60, 17);
      this.ExpStateLabel.Text = "Exp State: ";
      // 
      // toolStripStatusLabel2
      // 
      this.toolStripStatusLabel2.ForeColor = System.Drawing.Color.DarkRed;
      this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
      this.toolStripStatusLabel2.Size = new System.Drawing.Size(84, 17);
      this.toolStripStatusLabel2.Text = "not connected";
      // 
      // FRecorder
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(554, 454);
      this.Controls.Add(this.InfoBar);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Cursor = System.Windows.Forms.Cursors.Default;
      this.Name = "FRecorder";
      this.Text = "Recorder";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.PlayPosition)).EndInit();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      this.InfoBar.ResumeLayout(false);
      this.InfoBar.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button StartButton;
    private System.Windows.Forms.Button StopButton;
    private System.Windows.Forms.Button CreateDB;
    private System.Windows.Forms.Button button4;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button button6;
    private System.Windows.Forms.Button button5;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Label TimeElapsed;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Button button7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label RecordTimeElapsed;
    private System.Windows.Forms.Button button8;
    private System.Windows.Forms.Button prev;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Button ManageMeasure;
    private System.Windows.Forms.Button CreateMeasureButton;
    private System.Windows.Forms.Button button11;
    private System.Windows.Forms.Button ManageExp;
    private System.Windows.Forms.TrackBar PlayPosition;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Button OpenExp;
    private System.Windows.Forms.Button OpenDB;
    private System.Windows.Forms.Button CreateExp;
    private System.Windows.Forms.StatusStrip InfoBar;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    private System.Windows.Forms.ToolStripStatusLabel BDStatus;
    private System.Windows.Forms.ToolStripStatusLabel ExpStateLabel;
    private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
    private System.Windows.Forms.CheckBox DoRecordCmpData;
    private System.Windows.Forms.Button DeselectAllRecs;
    private System.Windows.Forms.Button SelectAllRecords;
    private System.Windows.Forms.CheckBox DoRecordStimData;
    private System.Windows.Forms.CheckBox DoRecordPackData;
    private System.Windows.Forms.CheckBox DoRecordCompressData;
  }
}