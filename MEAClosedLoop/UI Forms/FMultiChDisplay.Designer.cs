namespace MEAClosedLoop.UI_Forms
{
  partial class FMultiChDisplay
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
      this.zedGraphPlot = new ZedGraph.ZedGraphControl();
      this.label7 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.DurationChecker = new System.Windows.Forms.NumericUpDown();
      this.AmplitudeChecker = new System.Windows.Forms.NumericUpDown();
      this.label5 = new System.Windows.Forms.Label();
      this.StartButton = new System.Windows.Forms.Button();
      this.stopButton = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.UpdateTimeLabel = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.DurationChecker)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.AmplitudeChecker)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // zedGraphPlot
      // 
      this.zedGraphPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.zedGraphPlot.EditModifierKeys = System.Windows.Forms.Keys.None;
      this.zedGraphPlot.IsAntiAlias = true;
      this.zedGraphPlot.Location = new System.Drawing.Point(12, 64);
      this.zedGraphPlot.Name = "zedGraphPlot";
      this.zedGraphPlot.ScrollGrace = 0D;
      this.zedGraphPlot.ScrollMaxX = 0D;
      this.zedGraphPlot.ScrollMaxY = 0D;
      this.zedGraphPlot.ScrollMaxY2 = 0D;
      this.zedGraphPlot.ScrollMinX = 0D;
      this.zedGraphPlot.ScrollMinY = 0D;
      this.zedGraphPlot.ScrollMinY2 = 0D;
      this.zedGraphPlot.Size = new System.Drawing.Size(917, 343);
      this.zedGraphPlot.TabIndex = 1;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 18);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(105, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "Продолжителность";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(266, 18);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(62, 13);
      this.label6.TabIndex = 3;
      this.label6.Text = "Амплитуда";
      // 
      // DurationChecker
      // 
      this.DurationChecker.Location = new System.Drawing.Point(117, 16);
      this.DurationChecker.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.DurationChecker.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.DurationChecker.Name = "DurationChecker";
      this.DurationChecker.Size = new System.Drawing.Size(49, 20);
      this.DurationChecker.TabIndex = 4;
      this.DurationChecker.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.DurationChecker.ValueChanged += new System.EventHandler(this.DurationChecker_ValueChanged);
      // 
      // AmplitudeChecker
      // 
      this.AmplitudeChecker.Location = new System.Drawing.Point(334, 16);
      this.AmplitudeChecker.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.AmplitudeChecker.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.AmplitudeChecker.Name = "AmplitudeChecker";
      this.AmplitudeChecker.Size = new System.Drawing.Size(120, 20);
      this.AmplitudeChecker.TabIndex = 5;
      this.AmplitudeChecker.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
      this.AmplitudeChecker.ValueChanged += new System.EventHandler(this.AmplitudeChecker_ValueChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(182, 18);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(43, 13);
      this.label5.TabIndex = 6;
      this.label5.Text = "Секунд";
      // 
      // StartButton
      // 
      this.StartButton.Location = new System.Drawing.Point(570, 13);
      this.StartButton.Name = "StartButton";
      this.StartButton.Size = new System.Drawing.Size(75, 23);
      this.StartButton.TabIndex = 7;
      this.StartButton.Text = "старт";
      this.StartButton.UseVisualStyleBackColor = true;
      this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
      // 
      // stopButton
      // 
      this.stopButton.Location = new System.Drawing.Point(663, 13);
      this.stopButton.Name = "stopButton";
      this.stopButton.Size = new System.Drawing.Size(75, 23);
      this.stopButton.TabIndex = 7;
      this.stopButton.Text = "стоп";
      this.stopButton.UseVisualStyleBackColor = true;
      this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.UpdateTimeLabel);
      this.groupBox2.Controls.Add(this.label1);
      this.groupBox2.Controls.Add(this.stopButton);
      this.groupBox2.Controls.Add(this.StartButton);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.AmplitudeChecker);
      this.groupBox2.Controls.Add(this.DurationChecker);
      this.groupBox2.Controls.Add(this.label6);
      this.groupBox2.Controls.Add(this.label7);
      this.groupBox2.Location = new System.Drawing.Point(12, 12);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(917, 46);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Настройки";
      // 
      // UpdateTimeLabel
      // 
      this.UpdateTimeLabel.AutoSize = true;
      this.UpdateTimeLabel.Location = new System.Drawing.Point(836, 18);
      this.UpdateTimeLabel.Name = "UpdateTimeLabel";
      this.UpdateTimeLabel.Size = new System.Drawing.Size(0, 13);
      this.UpdateTimeLabel.TabIndex = 8;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(784, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(46, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "update: ";
      // 
      // FMultiChDisplay
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(941, 419);
      this.Controls.Add(this.zedGraphPlot);
      this.Controls.Add(this.groupBox2);
      this.Name = "FMultiChDisplay";
      this.Text = "FDataDisplay";
      this.Load += new System.EventHandler(this.FMultiChDisplay_Load);
      ((System.ComponentModel.ISupportInitialize)(this.DurationChecker)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.AmplitudeChecker)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private ZedGraph.ZedGraphControl zedGraphPlot;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown DurationChecker;
    private System.Windows.Forms.NumericUpDown AmplitudeChecker;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button StartButton;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label UpdateTimeLabel;
    private System.Windows.Forms.Label label1;
  }
}