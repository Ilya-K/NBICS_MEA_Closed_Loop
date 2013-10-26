namespace MEAClosedLoop
{
  partial class FormCalibrate
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.buttonStartStop = new System.Windows.Forms.Button();
      this.buttonSave = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.lStimStartLatency = new System.Windows.Forms.Label();
      this.lStimStopLatency = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.Location = new System.Drawing.Point(69, 43);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(78, 24);
      this.label1.TabIndex = 0;
      this.label1.Text = "label1";
      // 
      // label2
      // 
      this.label2.Location = new System.Drawing.Point(69, 67);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(78, 23);
      this.label2.TabIndex = 0;
      this.label2.Text = "label2";
      // 
      // buttonStartStop
      // 
      this.buttonStartStop.Location = new System.Drawing.Point(331, 25);
      this.buttonStartStop.Name = "buttonStartStop";
      this.buttonStartStop.Size = new System.Drawing.Size(75, 23);
      this.buttonStartStop.TabIndex = 1;
      this.buttonStartStop.Text = "Stop";
      this.buttonStartStop.UseVisualStyleBackColor = true;
      this.buttonStartStop.Click += new System.EventHandler(this.buttonStartStop_Click);
      // 
      // buttonSave
      // 
      this.buttonSave.Location = new System.Drawing.Point(331, 54);
      this.buttonSave.Name = "buttonSave";
      this.buttonSave.Size = new System.Drawing.Size(75, 23);
      this.buttonSave.TabIndex = 1;
      this.buttonSave.Text = "Save";
      this.buttonSave.UseVisualStyleBackColor = true;
      // 
      // label3
      // 
      this.label3.Location = new System.Drawing.Point(69, 99);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(78, 23);
      this.label3.TabIndex = 0;
      this.label3.Text = "label3";
      this.label3.Paint += new System.Windows.Forms.PaintEventHandler(this.label3_Paint);
      // 
      // lStimStartLatency
      // 
      this.lStimStartLatency.Location = new System.Drawing.Point(197, 43);
      this.lStimStartLatency.Name = "lStimStartLatency";
      this.lStimStartLatency.Size = new System.Drawing.Size(78, 23);
      this.lStimStartLatency.TabIndex = 0;
      this.lStimStartLatency.Text = "label4";
      this.lStimStartLatency.Paint += new System.Windows.Forms.PaintEventHandler(this.label3_Paint);
      // 
      // lStimStopLatency
      // 
      this.lStimStopLatency.Location = new System.Drawing.Point(197, 83);
      this.lStimStopLatency.Name = "lStimStopLatency";
      this.lStimStopLatency.Size = new System.Drawing.Size(78, 23);
      this.lStimStopLatency.TabIndex = 0;
      this.lStimStopLatency.Text = "label5";
      this.lStimStopLatency.Paint += new System.Windows.Forms.PaintEventHandler(this.label3_Paint);
      // 
      // FormCalibrate
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(424, 272);
      this.Controls.Add(this.buttonSave);
      this.Controls.Add(this.buttonStartStop);
      this.Controls.Add(this.lStimStopLatency);
      this.Controls.Add(this.lStimStartLatency);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FormCalibrate";
      this.ShowInTaskbar = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Calibration";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCalibrate_FormClosing);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button buttonStartStop;
    private System.Windows.Forms.Button buttonSave;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lStimStartLatency;
    private System.Windows.Forms.Label lStimStopLatency;

  }
}