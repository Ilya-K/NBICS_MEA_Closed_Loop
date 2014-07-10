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
      this.ParamBox = new System.Windows.Forms.GroupBox();
      this.label5 = new System.Windows.Forms.Label();
      this.numericUpDown4 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.StartCycle = new System.Windows.Forms.Button();
      this.FinishCycle = new System.Windows.Forms.Button();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.RSPacks = new System.Windows.Forms.Panel();
      this.label6 = new System.Windows.Forms.Label();
      this.DelayTime = new System.Windows.Forms.NumericUpDown();
      this.SearchDelta = new System.Windows.Forms.NumericUpDown();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.SelectIndex = new System.Windows.Forms.NumericUpDown();
      this.label9 = new System.Windows.Forms.Label();
      this.TimeStamp = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.SelectName = new System.Windows.Forms.NumericUpDown();
      this.label11 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.ParamBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.groupBox2.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DelayTime)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SearchDelta)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SelectIndex)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.SelectName)).BeginInit();
      this.SuspendLayout();
      // 
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.pictureBox1.Location = new System.Drawing.Point(12, 363);
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
      // ParamBox
      // 
      this.ParamBox.Controls.Add(this.SelectName);
      this.ParamBox.Controls.Add(this.SelectIndex);
      this.ParamBox.Controls.Add(this.SearchDelta);
      this.ParamBox.Controls.Add(this.DelayTime);
      this.ParamBox.Controls.Add(this.label11);
      this.ParamBox.Controls.Add(this.label10);
      this.ParamBox.Controls.Add(this.label8);
      this.ParamBox.Controls.Add(this.label7);
      this.ParamBox.Controls.Add(this.label6);
      this.ParamBox.Controls.Add(this.label5);
      this.ParamBox.Controls.Add(this.numericUpDown4);
      this.ParamBox.Controls.Add(this.numericUpDown3);
      this.ParamBox.Controls.Add(this.numericUpDown2);
      this.ParamBox.Controls.Add(this.numericUpDown1);
      this.ParamBox.Controls.Add(this.label1);
      this.ParamBox.Controls.Add(this.label4);
      this.ParamBox.Controls.Add(this.label3);
      this.ParamBox.Controls.Add(this.label2);
      this.ParamBox.Location = new System.Drawing.Point(12, 12);
      this.ParamBox.Name = "ParamBox";
      this.ParamBox.Size = new System.Drawing.Size(398, 259);
      this.ParamBox.TabIndex = 4;
      this.ParamBox.TabStop = false;
      this.ParamBox.Text = "Параметры цикла";
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
      this.label4.Size = new System.Drawing.Size(188, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "критерий правильного ответа (R/S)";
      // 
      // StartCycle
      // 
      this.StartCycle.Location = new System.Drawing.Point(12, 277);
      this.StartCycle.Name = "StartCycle";
      this.StartCycle.Size = new System.Drawing.Size(98, 23);
      this.StartCycle.TabIndex = 5;
      this.StartCycle.Text = "Начать цикл";
      this.StartCycle.UseVisualStyleBackColor = true;
      this.StartCycle.Click += new System.EventHandler(this.StartCycle_Click);
      // 
      // FinishCycle
      // 
      this.FinishCycle.Location = new System.Drawing.Point(12, 306);
      this.FinishCycle.Name = "FinishCycle";
      this.FinishCycle.Size = new System.Drawing.Size(98, 23);
      this.FinishCycle.TabIndex = 5;
      this.FinishCycle.Text = "Завершить цикл";
      this.FinishCycle.UseVisualStyleBackColor = true;
      this.FinishCycle.Click += new System.EventHandler(this.FinishCycle_Click);
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
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 140);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(262, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "Задержка ожидаемой активности от стимула - мс";
      // 
      // DelayTime
      // 
      this.DelayTime.Location = new System.Drawing.Point(331, 138);
      this.DelayTime.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.DelayTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.DelayTime.Name = "DelayTime";
      this.DelayTime.Size = new System.Drawing.Size(58, 20);
      this.DelayTime.TabIndex = 7;
      this.DelayTime.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
      // 
      // SearchDelta
      // 
      this.SearchDelta.Location = new System.Drawing.Point(331, 164);
      this.SearchDelta.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
      this.SearchDelta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.SearchDelta.Name = "SearchDelta";
      this.SearchDelta.Size = new System.Drawing.Size(58, 20);
      this.SearchDelta.TabIndex = 7;
      this.SearchDelta.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 166);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(173, 13);
      this.label7.TabIndex = 6;
      this.label7.Text = "Разброс поиска активности - мс";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(6, 204);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(122, 13);
      this.label8.TabIndex = 6;
      this.label8.Text = "Номер канала поиска ";
      // 
      // SelectIndex
      // 
      this.SelectIndex.Location = new System.Drawing.Point(331, 202);
      this.SelectIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.SelectIndex.Name = "SelectIndex";
      this.SelectIndex.Size = new System.Drawing.Size(58, 20);
      this.SelectIndex.TabIndex = 7;
      this.SelectIndex.Value = new decimal(new int[] {
            55,
            0,
            0,
            0});
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(122, 282);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(158, 13);
      this.label9.TabIndex = 9;
      this.label9.Text = "Всего прошло времени (сек): ";
      // 
      // TimeStamp
      // 
      this.TimeStamp.Location = new System.Drawing.Point(286, 280);
      this.TimeStamp.Name = "TimeStamp";
      this.TimeStamp.Size = new System.Drawing.Size(73, 20);
      this.TimeStamp.TabIndex = 10;
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(203, 204);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(110, 13);
      this.label10.TabIndex = 6;
      this.label10.Text = "Порядковый индекс";
      // 
      // SelectName
      // 
      this.SelectName.Location = new System.Drawing.Point(331, 228);
      this.SelectName.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.SelectName.Name = "SelectName";
      this.SelectName.Size = new System.Drawing.Size(58, 20);
      this.SelectName.TabIndex = 7;
      this.SelectName.Value = new decimal(new int[] {
            55,
            0,
            0,
            0});
      this.SelectName.ValueChanged += new System.EventHandler(this.SelectName_ValueChanged);
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(203, 230);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(103, 13);
      this.label11.TabIndex = 6;
      this.label11.Text = "Матричный индекс";
      // 
      // FLearnCycle
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(860, 540);
      this.Controls.Add(this.TimeStamp);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.FinishCycle);
      this.Controls.Add(this.StartCycle);
      this.Controls.Add(this.ParamBox);
      this.Controls.Add(this.pictureBox1);
      this.Name = "FLearnCycle";
      this.Text = "FLearnCycle";
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.ParamBox.ResumeLayout(false);
      this.ParamBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown4)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.groupBox2.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.DelayTime)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.SearchDelta)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.SelectIndex)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.SelectName)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox ParamBox;
    private System.Windows.Forms.NumericUpDown numericUpDown3;
    private System.Windows.Forms.NumericUpDown numericUpDown2;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.Button StartCycle;
    private System.Windows.Forms.Button FinishCycle;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown numericUpDown4;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Panel RSPacks;
    private System.Windows.Forms.NumericUpDown SearchDelta;
    private System.Windows.Forms.NumericUpDown DelayTime;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown SelectIndex;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox TimeStamp;
    private System.Windows.Forms.NumericUpDown SelectName;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label10;
  }
}