namespace MEAClosedLoop
{
  partial class FMainWindow
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
      this.MainTopMenu = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.opitonsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.experimentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.aboutProgrammToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.versionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.reportABugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.newManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.MainTopMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // MainTopMenu
      // 
      this.MainTopMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.experimentsToolStripMenuItem,
            this.opitonsToolStripMenuItem,
            this.aboutToolStripMenuItem});
      this.MainTopMenu.Location = new System.Drawing.Point(0, 0);
      this.MainTopMenu.Name = "MainTopMenu";
      this.MainTopMenu.Size = new System.Drawing.Size(852, 24);
      this.MainTopMenu.TabIndex = 0;
      this.MainTopMenu.Text = "MainTopMenu";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newManagerToolStripMenuItem,
            this.exitToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "File";
      // 
      // opitonsToolStripMenuItem
      // 
      this.opitonsToolStripMenuItem.Name = "opitonsToolStripMenuItem";
      this.opitonsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
      this.opitonsToolStripMenuItem.Text = "Opitons";
      // 
      // aboutToolStripMenuItem
      // 
      this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutProgrammToolStripMenuItem,
            this.versionToolStripMenuItem,
            this.reportABugToolStripMenuItem});
      this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
      this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
      this.aboutToolStripMenuItem.Text = "About";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
      // 
      // experimentsToolStripMenuItem
      // 
      this.experimentsToolStripMenuItem.Name = "experimentsToolStripMenuItem";
      this.experimentsToolStripMenuItem.Size = new System.Drawing.Size(83, 20);
      this.experimentsToolStripMenuItem.Text = "Experiments";
      // 
      // aboutProgrammToolStripMenuItem
      // 
      this.aboutProgrammToolStripMenuItem.Name = "aboutProgrammToolStripMenuItem";
      this.aboutProgrammToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.aboutProgrammToolStripMenuItem.Text = "About Programm";
      // 
      // versionToolStripMenuItem
      // 
      this.versionToolStripMenuItem.Name = "versionToolStripMenuItem";
      this.versionToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.versionToolStripMenuItem.Text = "Version";
      // 
      // reportABugToolStripMenuItem
      // 
      this.reportABugToolStripMenuItem.Name = "reportABugToolStripMenuItem";
      this.reportABugToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
      this.reportABugToolStripMenuItem.Text = "Report a bug";
      // 
      // newManagerToolStripMenuItem
      // 
      this.newManagerToolStripMenuItem.Name = "newManagerToolStripMenuItem";
      this.newManagerToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.newManagerToolStripMenuItem.Text = "NewManager";
      this.newManagerToolStripMenuItem.Click += new System.EventHandler(this.newManagerToolStripMenuItem_Click);
      // 
      // FMainWindow
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.ClientSize = new System.Drawing.Size(852, 402);
      this.Controls.Add(this.MainTopMenu);
      this.IsMdiContainer = true;
      this.MainMenuStrip = this.MainTopMenu;
      this.Name = "FMainWindow";
      this.Text = "FMainWindow";
      this.MainTopMenu.ResumeLayout(false);
      this.MainTopMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip MainTopMenu;
    private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem experimentsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem opitonsToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem aboutProgrammToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem versionToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem reportABugToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem newManagerToolStripMenuItem;
  }
}