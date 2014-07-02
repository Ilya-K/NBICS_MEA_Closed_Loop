namespace MEAClosedLoop.Common
{
  partial class FCreateMeasure
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
      this.CreateButton = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.AboutTextBox = new System.Windows.Forms.TextBox();
      this.CancellButton = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // CreateButton
      // 
      this.CreateButton.Location = new System.Drawing.Point(54, 101);
      this.CreateButton.Name = "CreateButton";
      this.CreateButton.Size = new System.Drawing.Size(75, 23);
      this.CreateButton.TabIndex = 2;
      this.CreateButton.Text = "Create";
      this.CreateButton.UseVisualStyleBackColor = true;
      this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "About";
      // 
      // AboutTextBox
      // 
      this.AboutTextBox.Location = new System.Drawing.Point(54, 21);
      this.AboutTextBox.Multiline = true;
      this.AboutTextBox.Name = "AboutTextBox";
      this.AboutTextBox.Size = new System.Drawing.Size(316, 57);
      this.AboutTextBox.TabIndex = 1;
      // 
      // CancellButton
      // 
      this.CancellButton.Location = new System.Drawing.Point(213, 101);
      this.CancellButton.Name = "CancellButton";
      this.CancellButton.Size = new System.Drawing.Size(75, 23);
      this.CancellButton.TabIndex = 3;
      this.CancellButton.Text = "Cancel";
      this.CancellButton.UseVisualStyleBackColor = true;
      this.CancellButton.Click += new System.EventHandler(this.CancellButton_Click);
      // 
      // FCreateMeasure
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(376, 136);
      this.Controls.Add(this.CancellButton);
      this.Controls.Add(this.AboutTextBox);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.CreateButton);
      this.Name = "FCreateMeasure";
      this.Text = "CreateMeasureForm";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button CreateButton;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox AboutTextBox;
    private System.Windows.Forms.Button CancellButton;
  }
}