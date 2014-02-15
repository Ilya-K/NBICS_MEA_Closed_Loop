namespace MEAClosedLoop
{
  partial class StatForm
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
      this.panel_Data = new System.Windows.Forms.Panel();
      this.cb_Channel = new System.Windows.Forms.ComboBox();
      this.panel_Stat1 = new System.Windows.Forms.Panel();
      this.bt_Start = new System.Windows.Forms.Button();
      this.bt_Stop = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // panel_Data
      // 
      this.panel_Data.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel_Data.BackColor = System.Drawing.SystemColors.Window;
      this.panel_Data.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel_Data.Location = new System.Drawing.Point(12, 41);
      this.panel_Data.Name = "panel_Data";
      this.panel_Data.Size = new System.Drawing.Size(1171, 145);
      this.panel_Data.TabIndex = 14;
      this.panel_Data.Paint += new System.Windows.Forms.PaintEventHandler(this.panel_Data_Paint);
      this.panel_Data.Resize += new System.EventHandler(this.panel_Data_Resize);
      // 
      // cb_Channel
      // 
      this.cb_Channel.FormattingEnabled = true;
      this.cb_Channel.Location = new System.Drawing.Point(12, 14);
      this.cb_Channel.Name = "cb_Channel";
      this.cb_Channel.Size = new System.Drawing.Size(113, 21);
      this.cb_Channel.TabIndex = 18;
      // 
      // panel_Stat1
      // 
      this.panel_Stat1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.panel_Stat1.BackColor = System.Drawing.SystemColors.Window;
      this.panel_Stat1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.panel_Stat1.Location = new System.Drawing.Point(12, 205);
      this.panel_Stat1.Name = "panel_Stat1";
      this.panel_Stat1.Size = new System.Drawing.Size(1171, 145);
      this.panel_Stat1.TabIndex = 19;
      // 
      // bt_Start
      // 
      this.bt_Start.Location = new System.Drawing.Point(151, 14);
      this.bt_Start.Name = "bt_Start";
      this.bt_Start.Size = new System.Drawing.Size(75, 23);
      this.bt_Start.TabIndex = 20;
      this.bt_Start.Text = "Start";
      this.bt_Start.UseVisualStyleBackColor = true;
      this.bt_Start.Click += new System.EventHandler(this.bt_Start_Click);
      // 
      // bt_Stop
      // 
      this.bt_Stop.Location = new System.Drawing.Point(247, 14);
      this.bt_Stop.Name = "bt_Stop";
      this.bt_Stop.Size = new System.Drawing.Size(75, 23);
      this.bt_Stop.TabIndex = 20;
      this.bt_Stop.Text = "Stop";
      this.bt_Stop.UseVisualStyleBackColor = true;
      this.bt_Stop.Click += new System.EventHandler(this.bt_Stop_Click);
      // 
      // StatForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1195, 734);
      this.Controls.Add(this.bt_Stop);
      this.Controls.Add(this.bt_Start);
      this.Controls.Add(this.panel_Stat1);
      this.Controls.Add(this.cb_Channel);
      this.Controls.Add(this.panel_Data);
      this.Name = "StatForm";
      this.Text = "StatForm";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StatForm_FormClosing);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel_Data;
    private System.Windows.Forms.ComboBox cb_Channel;
    private System.Windows.Forms.Panel panel_Stat1;
    private System.Windows.Forms.Button bt_Start;
    private System.Windows.Forms.Button bt_Stop;
  }
}