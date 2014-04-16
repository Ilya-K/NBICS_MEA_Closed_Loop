namespace MEAClosedLoop
{
  partial class PackGraphForm
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
      this.RunStatButton = new System.Windows.Forms.Button();
      this.StatTypeListBox = new System.Windows.Forms.ListBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.timelabel = new System.Windows.Forms.Label();
      this.MinCountBox = new System.Windows.Forms.NumericUpDown();
      this.StatProgressBar = new System.Windows.Forms.ProgressBar();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.MinCountBox)).BeginInit();
      this.SuspendLayout();
      // 
      // RunStatButton
      // 
      this.RunStatButton.Location = new System.Drawing.Point(376, 19);
      this.RunStatButton.Name = "RunStatButton";
      this.RunStatButton.Size = new System.Drawing.Size(125, 26);
      this.RunStatButton.TabIndex = 0;
      this.RunStatButton.Text = "собрать статистику";
      this.RunStatButton.UseVisualStyleBackColor = true;
      this.RunStatButton.Click += new System.EventHandler(this.RunStatButton_Click);
      // 
      // StatTypeListBox
      // 
      this.StatTypeListBox.FormattingEnabled = true;
      this.StatTypeListBox.Items.AddRange(new object[] {
            "По амплитуде",
            "По частоте спайков"});
      this.StatTypeListBox.Location = new System.Drawing.Point(6, 19);
      this.StatTypeListBox.Name = "StatTypeListBox";
      this.StatTypeListBox.Size = new System.Drawing.Size(120, 43);
      this.StatTypeListBox.TabIndex = 3;
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.timelabel);
      this.groupBox1.Controls.Add(this.MinCountBox);
      this.groupBox1.Controls.Add(this.StatTypeListBox);
      this.groupBox1.Controls.Add(this.StatProgressBar);
      this.groupBox1.Controls.Add(this.RunStatButton);
      this.groupBox1.Location = new System.Drawing.Point(12, 0);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(756, 86);
      this.groupBox1.TabIndex = 4;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Анализ";
      // 
      // timelabel
      // 
      this.timelabel.Location = new System.Drawing.Point(140, 27);
      this.timelabel.Name = "timelabel";
      this.timelabel.Size = new System.Drawing.Size(78, 23);
      this.timelabel.TabIndex = 5;
      this.timelabel.Text = "время (мин.)";
      // 
      // MinCountBox
      // 
      this.MinCountBox.Location = new System.Drawing.Point(224, 24);
      this.MinCountBox.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.MinCountBox.Name = "MinCountBox";
      this.MinCountBox.Size = new System.Drawing.Size(43, 20);
      this.MinCountBox.TabIndex = 4;
      // 
      // StatProgressBar
      // 
      this.StatProgressBar.Location = new System.Drawing.Point(544, 19);
      this.StatProgressBar.Name = "StatProgressBar";
      this.StatProgressBar.Size = new System.Drawing.Size(125, 23);
      this.StatProgressBar.TabIndex = 1;
      this.StatProgressBar.UseWaitCursor = true;
      // 
      // PackGraphForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(780, 964);
      this.ControlBox = false;
      this.Controls.Add(this.groupBox1);
      this.Name = "PackGraphForm";
      this.Text = "PackGraphForm";
      this.Load += new System.EventHandler(this.PackGraphForm_Load);
      this.groupBox1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.MinCountBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ListBox StatTypeListBox;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.NumericUpDown MinCountBox;
    private System.Windows.Forms.Button RunStatButton;
    private System.Windows.Forms.Label timelabel;
    private System.Windows.Forms.ProgressBar StatProgressBar;
  }
}