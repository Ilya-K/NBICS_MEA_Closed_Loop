namespace MEAClosedLoop.Common
{
  partial class FStimParams
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
      this.WaitPackTime = new System.Windows.Forms.TextBox();
      this.ok = new System.Windows.Forms.Button();
      this.cancel = new System.Windows.Forms.Button();
      this.accept = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(13, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(218, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Дополнительные параметры стимуляции";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 42);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(199, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Временное окно ожидания пачки (мс)";
      // 
      // WaitPackTime
      // 
      this.WaitPackTime.Location = new System.Drawing.Point(217, 42);
      this.WaitPackTime.Name = "WaitPackTime";
      this.WaitPackTime.Size = new System.Drawing.Size(100, 20);
      this.WaitPackTime.TabIndex = 2;
      // 
      // ok
      // 
      this.ok.Location = new System.Drawing.Point(12, 86);
      this.ok.Name = "ok";
      this.ok.Size = new System.Drawing.Size(75, 23);
      this.ok.TabIndex = 3;
      this.ok.Text = "Ок";
      this.ok.UseVisualStyleBackColor = true;
      this.ok.Click += new System.EventHandler(this.ok_Click);
      // 
      // cancel
      // 
      this.cancel.Location = new System.Drawing.Point(124, 86);
      this.cancel.Name = "cancel";
      this.cancel.Size = new System.Drawing.Size(75, 23);
      this.cancel.TabIndex = 4;
      this.cancel.Text = "отмена";
      this.cancel.UseVisualStyleBackColor = true;
      this.cancel.Click += new System.EventHandler(this.cancel_Click);
      // 
      // accept
      // 
      this.accept.Location = new System.Drawing.Point(242, 86);
      this.accept.Name = "accept";
      this.accept.Size = new System.Drawing.Size(75, 23);
      this.accept.TabIndex = 5;
      this.accept.Text = "применить";
      this.accept.UseVisualStyleBackColor = true;
      this.accept.Click += new System.EventHandler(this.accept_Click);
      // 
      // StimParams
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(336, 118);
      this.Controls.Add(this.accept);
      this.Controls.Add(this.cancel);
      this.Controls.Add(this.ok);
      this.Controls.Add(this.WaitPackTime);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Name = "StimParams";
      this.Text = "StimParams";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox WaitPackTime;
    private System.Windows.Forms.Button ok;
    private System.Windows.Forms.Button cancel;
    private System.Windows.Forms.Button accept;
  }
}