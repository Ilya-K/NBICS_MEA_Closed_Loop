namespace MEAClosedLoop
{
  partial class FChSorter
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
      System.Windows.Forms.GroupBox groupBox1;
      this.SigmaUpDown = new System.Windows.Forms.NumericUpDown();
      this.label5 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.T3StartValue = new System.Windows.Forms.TextBox();
      this.T1EndValue = new System.Windows.Forms.TextBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.StopButton = new System.Windows.Forms.Button();
      this.StartButton = new System.Windows.Forms.Button();
      this.StatTable = new System.Windows.Forms.DataGridView();
      this.Midx = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.idx = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.T_0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.T_1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.T_2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Koef = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      groupBox1 = new System.Windows.Forms.GroupBox();
      groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.SigmaUpDown)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.StatTable)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      groupBox1.Controls.Add(this.SigmaUpDown);
      groupBox1.Controls.Add(this.label5);
      groupBox1.Controls.Add(this.label4);
      groupBox1.Controls.Add(this.label3);
      groupBox1.Controls.Add(this.T3StartValue);
      groupBox1.Controls.Add(this.T1EndValue);
      groupBox1.Controls.Add(this.label2);
      groupBox1.Controls.Add(this.label1);
      groupBox1.Controls.Add(this.StopButton);
      groupBox1.Controls.Add(this.StartButton);
      groupBox1.Location = new System.Drawing.Point(553, 7);
      groupBox1.Name = "groupBox1";
      groupBox1.Size = new System.Drawing.Size(391, 174);
      groupBox1.TabIndex = 3;
      groupBox1.TabStop = false;
      groupBox1.Text = "Анализ";
      // 
      // SigmaUpDown
      // 
      this.SigmaUpDown.Location = new System.Drawing.Point(88, 76);
      this.SigmaUpDown.Name = "SigmaUpDown";
      this.SigmaUpDown.Size = new System.Drawing.Size(60, 20);
      this.SigmaUpDown.TabIndex = 8;
      this.SigmaUpDown.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
      this.SigmaUpDown.ValueChanged += new System.EventHandler(this.SigmaUpDown_ValueChanged);
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(6, 78);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(39, 13);
      this.label5.TabIndex = 7;
      this.label5.Text = "Сигма";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(154, 53);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(21, 13);
      this.label4.TabIndex = 6;
      this.label4.Text = "мс";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(154, 24);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(21, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "мс";
      // 
      // T3StartValue
      // 
      this.T3StartValue.Location = new System.Drawing.Point(88, 50);
      this.T3StartValue.Name = "T3StartValue";
      this.T3StartValue.Size = new System.Drawing.Size(60, 20);
      this.T3StartValue.TabIndex = 5;
      this.T3StartValue.Text = "60";
      // 
      // T1EndValue
      // 
      this.T1EndValue.Location = new System.Drawing.Point(88, 21);
      this.T1EndValue.Name = "T1EndValue";
      this.T1EndValue.Size = new System.Drawing.Size(60, 20);
      this.T1EndValue.TabIndex = 5;
      this.T1EndValue.Text = "40";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 53);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(26, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "T_2";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 24);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(26, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "T_1";
      // 
      // StopButton
      // 
      this.StopButton.Location = new System.Drawing.Point(310, 48);
      this.StopButton.Name = "StopButton";
      this.StopButton.Size = new System.Drawing.Size(75, 23);
      this.StopButton.TabIndex = 2;
      this.StopButton.Text = "Закончить";
      this.StopButton.UseVisualStyleBackColor = true;
      // 
      // StartButton
      // 
      this.StartButton.Location = new System.Drawing.Point(310, 19);
      this.StartButton.Name = "StartButton";
      this.StartButton.Size = new System.Drawing.Size(75, 23);
      this.StartButton.TabIndex = 1;
      this.StartButton.Text = "Начать";
      this.StartButton.UseVisualStyleBackColor = true;
      this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
      // 
      // StatTable
      // 
      this.StatTable.AllowUserToAddRows = false;
      this.StatTable.AllowUserToDeleteRows = false;
      this.StatTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.StatTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Midx,
            this.idx,
            this.T_0,
            this.T_1,
            this.T_2,
            this.Koef});
      this.StatTable.Location = new System.Drawing.Point(3, 7);
      this.StatTable.Name = "StatTable";
      this.StatTable.ReadOnly = true;
      this.StatTable.Size = new System.Drawing.Size(544, 344);
      this.StatTable.TabIndex = 0;
      // 
      // Midx
      // 
      this.Midx.HeaderText = "Midx";
      this.Midx.Name = "Midx";
      this.Midx.ReadOnly = true;
      this.Midx.Width = 50;
      // 
      // idx
      // 
      this.idx.HeaderText = "idx";
      this.idx.Name = "idx";
      this.idx.ReadOnly = true;
      this.idx.Width = 50;
      // 
      // T_0
      // 
      this.T_0.HeaderText = "%T_0";
      this.T_0.Name = "T_0";
      this.T_0.ReadOnly = true;
      // 
      // T_1
      // 
      this.T_1.HeaderText = "%T_1";
      this.T_1.Name = "T_1";
      this.T_1.ReadOnly = true;
      // 
      // T_2
      // 
      this.T_2.HeaderText = "%T_2";
      this.T_2.Name = "T_2";
      this.T_2.ReadOnly = true;
      // 
      // Koef
      // 
      this.Koef.HeaderText = "K";
      this.Koef.Name = "Koef";
      this.Koef.ReadOnly = true;
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
      this.pictureBox1.Location = new System.Drawing.Point(6, 19);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(376, 136);
      this.pictureBox1.TabIndex = 4;
      this.pictureBox1.TabStop = false;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.pictureBox1);
      this.groupBox2.Location = new System.Drawing.Point(554, 187);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(390, 164);
      this.groupBox2.TabIndex = 5;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Усреднение для выбранного";
      // 
      // FChSorter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(951, 354);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(groupBox1);
      this.Controls.Add(this.StatTable);
      this.Name = "FChSorter";
      this.Text = "FChSorter";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FChSorter_FormClosing);
      this.Load += new System.EventHandler(this.FChSorter_Load);
      groupBox1.ResumeLayout(false);
      groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.SigmaUpDown)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.StatTable)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView StatTable;
    private System.Windows.Forms.Button StartButton;
    private System.Windows.Forms.Button StopButton;
    private System.Windows.Forms.DataGridViewTextBoxColumn Midx;
    private System.Windows.Forms.DataGridViewTextBoxColumn idx;
    private System.Windows.Forms.DataGridViewTextBoxColumn T_0;
    private System.Windows.Forms.DataGridViewTextBoxColumn T_1;
    private System.Windows.Forms.DataGridViewTextBoxColumn T_2;
    private System.Windows.Forms.DataGridViewTextBoxColumn Koef;
    private System.Windows.Forms.NumericUpDown SigmaUpDown;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox T3StartValue;
    private System.Windows.Forms.TextBox T1EndValue;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.GroupBox groupBox2;
  }
}