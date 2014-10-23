namespace MEAClosedLoop
{
  partial class FDataSourceControl
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
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.RunEvBusrtButton = new System.Windows.Forms.Button();
      this.StatusMea = new System.Windows.Forms.TextBox();
      this.StatusMeaFlt = new System.Windows.Forms.TextBox();
      this.StatusStim = new System.Windows.Forms.TextBox();
      this.StatusBurst = new System.Windows.Forms.TextBox();
      this.StatusEvBurst = new System.Windows.Forms.TextBox();
      this.label6 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(2, 28);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Mea Data";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(2, 54);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(68, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Mea Flt Data";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(2, 80);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(53, 13);
      this.label3.TabIndex = 2;
      this.label3.Text = "Stim Data";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(2, 106);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(57, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Burst Data";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(2, 132);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(97, 13);
      this.label5.TabIndex = 3;
      this.label5.Text = "Evoked Burst Data";
      // 
      // RunEvBusrtButton
      // 
      this.RunEvBusrtButton.Location = new System.Drawing.Point(178, 129);
      this.RunEvBusrtButton.Name = "RunEvBusrtButton";
      this.RunEvBusrtButton.Size = new System.Drawing.Size(51, 21);
      this.RunEvBusrtButton.TabIndex = 4;
      this.RunEvBusrtButton.Text = "Run";
      this.RunEvBusrtButton.UseVisualStyleBackColor = true;
      this.RunEvBusrtButton.Click += new System.EventHandler(this.RunEvBusrtButton_Click);
      // 
      // StatusMea
      // 
      this.StatusMea.Enabled = false;
      this.StatusMea.Location = new System.Drawing.Point(99, 25);
      this.StatusMea.Name = "StatusMea";
      this.StatusMea.Size = new System.Drawing.Size(72, 20);
      this.StatusMea.TabIndex = 5;
      this.StatusMea.Text = "off";
      this.StatusMea.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // StatusMeaFlt
      // 
      this.StatusMeaFlt.Enabled = false;
      this.StatusMeaFlt.Location = new System.Drawing.Point(99, 51);
      this.StatusMeaFlt.Name = "StatusMeaFlt";
      this.StatusMeaFlt.Size = new System.Drawing.Size(72, 20);
      this.StatusMeaFlt.TabIndex = 5;
      this.StatusMeaFlt.Text = "off";
      this.StatusMeaFlt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // StatusStim
      // 
      this.StatusStim.Enabled = false;
      this.StatusStim.Location = new System.Drawing.Point(99, 77);
      this.StatusStim.Name = "StatusStim";
      this.StatusStim.Size = new System.Drawing.Size(72, 20);
      this.StatusStim.TabIndex = 5;
      this.StatusStim.Text = "off";
      this.StatusStim.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // StatusBurst
      // 
      this.StatusBurst.Enabled = false;
      this.StatusBurst.Location = new System.Drawing.Point(99, 103);
      this.StatusBurst.Name = "StatusBurst";
      this.StatusBurst.Size = new System.Drawing.Size(72, 20);
      this.StatusBurst.TabIndex = 5;
      this.StatusBurst.Text = "off";
      this.StatusBurst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // StatusEvBurst
      // 
      this.StatusEvBurst.Enabled = false;
      this.StatusEvBurst.Location = new System.Drawing.Point(99, 129);
      this.StatusEvBurst.Name = "StatusEvBurst";
      this.StatusEvBurst.Size = new System.Drawing.Size(72, 20);
      this.StatusEvBurst.TabIndex = 5;
      this.StatusEvBurst.Text = "off";
      this.StatusEvBurst.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(115, 9);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(37, 13);
      this.label6.TabIndex = 0;
      this.label6.Text = "Status";
      // 
      // FDataSourceControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(232, 153);
      this.Controls.Add(this.StatusEvBurst);
      this.Controls.Add(this.StatusBurst);
      this.Controls.Add(this.StatusStim);
      this.Controls.Add(this.StatusMeaFlt);
      this.Controls.Add(this.StatusMea);
      this.Controls.Add(this.RunEvBusrtButton);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FDataSourceControl";
      this.ShowIcon = false;
      this.ShowInTaskbar = false;
      this.Text = "FDataSourceControl";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Button RunEvBusrtButton;
    private System.Windows.Forms.TextBox StatusMea;
    private System.Windows.Forms.TextBox StatusMeaFlt;
    private System.Windows.Forms.TextBox StatusStim;
    private System.Windows.Forms.TextBox StatusBurst;
    private System.Windows.Forms.TextBox StatusEvBurst;
    private System.Windows.Forms.Label label6;

  }
}