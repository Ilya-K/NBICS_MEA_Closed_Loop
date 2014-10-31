namespace MEAClosedLoop.UI_Forms
{
  partial class FBurstFormDisplay
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
      this.MainPanel = new System.Windows.Forms.Panel();
      this.PicBox = new System.Windows.Forms.PictureBox();
      this.MainPanel.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PicBox)).BeginInit();
      this.SuspendLayout();
      // 
      // MainPanel
      // 
      this.MainPanel.AutoScroll = true;
      this.MainPanel.Controls.Add(this.PicBox);
      this.MainPanel.Location = new System.Drawing.Point(12, 12);
      this.MainPanel.Name = "MainPanel";
      this.MainPanel.Size = new System.Drawing.Size(964, 327);
      this.MainPanel.TabIndex = 0;
      // 
      // PicBox
      // 
      this.PicBox.BackColor = System.Drawing.SystemColors.ButtonHighlight;
      this.PicBox.Location = new System.Drawing.Point(3, 3);
      this.PicBox.Name = "PicBox";
      this.PicBox.Size = new System.Drawing.Size(100, 50);
      this.PicBox.TabIndex = 1;
      this.PicBox.TabStop = false;
      // 
      // FBurstFormDisplay
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(978, 351);
      this.Controls.Add(this.MainPanel);
      this.Name = "FBurstFormDisplay";
      this.Text = "FBurstFormDisplay";
      this.Load += new System.EventHandler(this.FBurstFormDisplay_Load);
      this.MainPanel.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.PicBox)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel MainPanel;
    private System.Windows.Forms.PictureBox PicBox;
  }
}