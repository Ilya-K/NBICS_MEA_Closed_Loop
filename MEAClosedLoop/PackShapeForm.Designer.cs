namespace MEAClosedLoop
{
  partial class PackShapeForm
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
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.ShapeSelectListBox = new System.Windows.Forms.ListBox();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(197, 222);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "Принять";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(197, 251);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(75, 23);
      this.button2.TabIndex = 1;
      this.button2.Text = "Отклонить";
      this.button2.UseVisualStyleBackColor = true;
      this.button2.Click += new System.EventHandler(this.button2_Click);
      // 
      // ShapeSelectListBox
      // 
      this.ShapeSelectListBox.FormattingEnabled = true;
      this.ShapeSelectListBox.Items.AddRange(new object[] {
            "по амплитуде",
            "по частоте"});
      this.ShapeSelectListBox.Location = new System.Drawing.Point(12, 222);
      this.ShapeSelectListBox.Name = "ShapeSelectListBox";
      this.ShapeSelectListBox.Size = new System.Drawing.Size(142, 43);
      this.ShapeSelectListBox.TabIndex = 2;
      this.ShapeSelectListBox.SelectedIndexChanged += new System.EventHandler(this.ShapeSelectListBox_SelectedIndexChanged);
      // 
      // PackShapeForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 286);
      this.Controls.Add(this.ShapeSelectListBox);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Name = "PackShapeForm";
      this.Text = "PackShapeForm";
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.ListBox ShapeSelectListBox;
  }
}