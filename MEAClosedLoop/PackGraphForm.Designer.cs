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
      this.StatProgressBar = new System.Windows.Forms.ProgressBar();
      this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
      this.StatTypeListBox = new System.Windows.Forms.ListBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox1.SuspendLayout();
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
      // 
      // StatProgressBar
      // 
      this.StatProgressBar.Location = new System.Drawing.Point(544, 19);
      this.StatProgressBar.Name = "StatProgressBar";
      this.StatProgressBar.Size = new System.Drawing.Size(125, 23);
      this.StatProgressBar.TabIndex = 1;
      this.StatProgressBar.UseWaitCursor = true;
      // 
      // domainUpDown1
      // 
      this.domainUpDown1.Location = new System.Drawing.Point(166, 25);
      this.domainUpDown1.Name = "domainUpDown1";
      this.domainUpDown1.Size = new System.Drawing.Size(120, 20);
      this.domainUpDown1.TabIndex = 2;
      this.domainUpDown1.Text = "время подсчёта";
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
      this.groupBox1.Controls.Add(this.StatTypeListBox);
      this.groupBox1.Controls.Add(this.StatProgressBar);
      this.groupBox1.Controls.Add(this.domainUpDown1);
      this.groupBox1.Controls.Add(this.RunStatButton);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(675, 86);
      this.groupBox1.TabIndex = 4;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "параметры подсчёта";
      // 
      // PackGraphForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(780, 771);
      this.Controls.Add(this.groupBox1);
      this.Name = "PackGraphForm";
      this.Text = "PackGraphForm";
      this.Load += new System.EventHandler(this.PackGraphForm_Load);
      this.groupBox1.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button RunStatButton;
    private System.Windows.Forms.ProgressBar StatProgressBar;
    private System.Windows.Forms.DomainUpDown domainUpDown1;
    private System.Windows.Forms.ListBox StatTypeListBox;
    private System.Windows.Forms.GroupBox groupBox1;
  }
}