namespace MEAClosedLoop.UI_Forms
{
  partial class FSingleChDisplay
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
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.button1 = new System.Windows.Forms.Button();
      this.label4 = new System.Windows.Forms.Label();
      this.Amplitude = new System.Windows.Forms.NumericUpDown();
      this.Duration = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.chNum = new System.Windows.Forms.NumericUpDown();
      this.label1 = new System.Windows.Forms.Label();
      this.zedGraphPlot = new ZedGraph.ZedGraphControl();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.stopButton = new System.Windows.Forms.Button();
      this.StartButton = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
      this.label8 = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.Amplitude)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.Duration)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.chNum)).BeginInit();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.Amplitude);
      this.groupBox1.Controls.Add(this.Duration);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.chNum);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(917, 46);
      this.groupBox1.TabIndex = 0;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Настройки";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(570, 13);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 7;
      this.button1.Text = "старт";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(301, 18);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(43, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "Секунд";
      // 
      // Amplitude
      // 
      this.Amplitude.Location = new System.Drawing.Point(418, 16);
      this.Amplitude.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.Amplitude.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.Amplitude.Name = "Amplitude";
      this.Amplitude.Size = new System.Drawing.Size(120, 20);
      this.Amplitude.TabIndex = 5;
      this.Amplitude.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
      // 
      // Duration
      // 
      this.Duration.Location = new System.Drawing.Point(246, 16);
      this.Duration.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.Duration.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.Duration.Name = "Duration";
      this.Duration.Size = new System.Drawing.Size(49, 20);
      this.Duration.TabIndex = 4;
      this.Duration.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(350, 18);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(62, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Амплитуда";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(123, 18);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(105, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Продолжителность";
      // 
      // chNum
      // 
      this.chNum.Location = new System.Drawing.Point(79, 16);
      this.chNum.Name = "chNum";
      this.chNum.Size = new System.Drawing.Size(38, 20);
      this.chNum.TabIndex = 1;
      this.chNum.Value = new decimal(new int[] {
            56,
            0,
            0,
            0});
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 18);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(55, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Канал  №";
      // 
      // zedGraphPlot
      // 
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
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.stopButton);
      this.groupBox2.Controls.Add(this.StartButton);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.numericUpDown1);
      this.groupBox2.Controls.Add(this.numericUpDown2);
      this.groupBox2.Controls.Add(this.label6);
      this.groupBox2.Controls.Add(this.label7);
      this.groupBox2.Controls.Add(this.numericUpDown3);
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Location = new System.Drawing.Point(12, 12);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(917, 46);
      this.groupBox2.TabIndex = 0;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Настройки";
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
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(301, 18);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(43, 13);
      this.label5.TabIndex = 6;
      this.label5.Text = "Секунд";
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(418, 16);
      this.numericUpDown1.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
      this.numericUpDown1.Minimum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
      this.numericUpDown1.TabIndex = 5;
      this.numericUpDown1.Value = new decimal(new int[] {
            250,
            0,
            0,
            0});
      // 
      // numericUpDown2
      // 
      this.numericUpDown2.Location = new System.Drawing.Point(246, 16);
      this.numericUpDown2.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new System.Drawing.Size(49, 20);
      this.numericUpDown2.TabIndex = 4;
      this.numericUpDown2.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(350, 18);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(62, 13);
      this.label6.TabIndex = 3;
      this.label6.Text = "Амплитуда";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(123, 18);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(105, 13);
      this.label7.TabIndex = 2;
      this.label7.Text = "Продолжителность";
      // 
      // numericUpDown3
      // 
      this.numericUpDown3.Location = new System.Drawing.Point(79, 16);
      this.numericUpDown3.Name = "numericUpDown3";
      this.numericUpDown3.Size = new System.Drawing.Size(38, 20);
      this.numericUpDown3.TabIndex = 1;
      this.numericUpDown3.Value = new decimal(new int[] {
            56,
            0,
            0,
            0});
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 18);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(55, 13);
      this.label8.TabIndex = 0;
      this.label8.Text = "Канал  №";
      // 
      // FSingleChDisplay
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(941, 419);
      this.Controls.Add(this.zedGraphPlot);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "FSingleChDisplay";
      this.Text = "FDataDisplay";
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.Amplitude)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.Duration)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.chNum)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.NumericUpDown chNum;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.NumericUpDown Amplitude;
    private System.Windows.Forms.NumericUpDown Duration;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private ZedGraph.ZedGraphControl zedGraphPlot;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button StartButton;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.NumericUpDown numericUpDown2;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.NumericUpDown numericUpDown3;
    private System.Windows.Forms.Label label8;
  }
}