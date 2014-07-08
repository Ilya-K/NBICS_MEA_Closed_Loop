namespace MEAClosedLoop
{
  partial class FLearnCycle
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
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.label5 = new System.Windows.Forms.Label();
      this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.button1 = new System.Windows.Forms.Button();
      this.button2 = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.RSPacks = new System.Windows.Forms.Panel();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.pictureBox1.Location = new System.Drawing.Point(21, 343);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(398, 159);
      this.pictureBox1.TabIndex = 0;
      this.pictureBox1.TabStop = false;
      this.pictureBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(6, 22);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(198, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Длительность стимуляции (max) - сек";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(146, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Длительность отдыха - сек";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 74);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(294, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Максимальная продолжительность эксперимента - мин";
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label5);
      this.groupBox1.Controls.Add(this.numericUpDown4);
      this.groupBox1.Controls.Add(this.numericUpDown3);
      this.groupBox1.Controls.Add(this.numericUpDown2);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.label4);
      this.groupBox1.Controls.Add(this.label3);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(398, 244);
      this.groupBox1.TabIndex = 4;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Параметры цикла";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(365, 100);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(24, 13);
      this.label5.TabIndex = 5;
      this.label5.Text = "/10";
      // 
      // numericUpDown4
      // 
      this.numericUpDown4.Location = new System.Drawing.Point(331, 98);
      this.numericUpDown4.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericUpDown4.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown4.Name = "numericUpDown4";
      this.numericUpDown4.Size = new System.Drawing.Size(28, 20);
      this.numericUpDown4.TabIndex = 4;
      this.numericUpDown4.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
      // 
      // numericUpDown3
      // 
      this.numericUpDown3.Location = new System.Drawing.Point(331, 72);
      this.numericUpDown3.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
      this.numericUpDown3.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown3.Name = "numericUpDown3";
      this.numericUpDown3.Size = new System.Drawing.Size(62, 20);
      this.numericUpDown3.TabIndex = 4;
      this.numericUpDown3.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
      // 
      // numericUpDown2
      // 
      this.numericUpDown2.Location = new System.Drawing.Point(331, 46);
      this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.numericUpDown2.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new System.Drawing.Size(62, 20);
      this.numericUpDown2.TabIndex = 4;
      this.numericUpDown2.Value = new decimal(new int[] {
            300,
            0,
            0,
            0});
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(331, 20);
      this.numericUpDown1.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(62, 20);
      this.numericUpDown1.TabIndex = 4;
      this.numericUpDown1.Value = new decimal(new int[] {
            600,
            0,
            0,
            0});
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(6, 100);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(159, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "критерий правильного ответа";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(21, 262);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(98, 23);
      this.button1.TabIndex = 5;
      this.button1.Text = "Начать цикл";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(21, 291);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(98, 23);
      this.button2.TabIndex = 5;
      this.button2.Text = "Завершить цикл";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.RSPacks);
      this.groupBox2.Location = new System.Drawing.Point(445, 17);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(403, 511);
      this.groupBox2.TabIndex = 8;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Ответ культуры";
      // 
      // RSPacks
      // 
      this.RSPacks.AutoScroll = true;
      this.RSPacks.Location = new System.Drawing.Point(7, 17);
      this.RSPacks.Name = "RSPacks";
      this.RSPacks.Size = new System.Drawing.Size(390, 488);
      this.RSPacks.TabIndex = 0;
      // 
      // FLearnCycle
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(860, 540);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.button2);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.pictureBox1);
      this.Name = "FLearnCycle";
      this.Text = "FLearnCycle";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.groupBox2.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.NumericUpDown numericUpDown3;
    private System.Windows.Forms.NumericUpDown numericUpDown2;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown numericUpDown4;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Panel RSPacks;
  }
}