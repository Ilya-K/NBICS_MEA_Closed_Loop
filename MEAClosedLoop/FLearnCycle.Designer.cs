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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.ParamBox = new System.Windows.Forms.GroupBox();
      this.StimBreakCheckBox = new System.Windows.Forms.CheckBox();
      this.PSelectName = new System.Windows.Forms.NumericUpDown();
      this.PSelectIndex = new System.Windows.Forms.NumericUpDown();
      this.PSearchDelta = new System.Windows.Forms.NumericUpDown();
      this.PDelayTime = new System.Windows.Forms.NumericUpDown();
      this.label11 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.PRSCount = new System.Windows.Forms.NumericUpDown();
      this.PExpMaxLength = new System.Windows.Forms.NumericUpDown();
      this.PCoolDownLength = new System.Windows.Forms.NumericUpDown();
      this.PStimLength = new System.Windows.Forms.NumericUpDown();
      this.label4 = new System.Windows.Forms.Label();
      this.StartCycle = new System.Windows.Forms.Button();
      this.FinishCycle = new System.Windows.Forms.Button();
      this.RSBurstBox = new System.Windows.Forms.GroupBox();
      this.RSPacks = new System.Windows.Forms.Panel();
      this.label9 = new System.Windows.Forms.Label();
      this.TimeStamp = new System.Windows.Forms.TextBox();
      this.LernLogTextBox = new System.Windows.Forms.TextBox();
      this.EvPacksBox = new System.Windows.Forms.GroupBox();
      this.evBurstPanel = new System.Windows.Forms.Panel();
      this.TrainEvolutionGraph = new System.Windows.Forms.PictureBox();
      this.label12 = new System.Windows.Forms.Label();
      this.RSManualButton = new System.Windows.Forms.Button();
      this.ParamBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PSelectName)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PSelectIndex)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PSearchDelta)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PDelayTime)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PRSCount)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PExpMaxLength)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PCoolDownLength)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PStimLength)).BeginInit();
      this.RSBurstBox.SuspendLayout();
      this.EvPacksBox.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.TrainEvolutionGraph)).BeginInit();
      this.SuspendLayout();
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
      this.ParamBox.Controls.Add(this.StimBreakCheckBox);
      this.ParamBox.Controls.Add(this.PSelectName);
      this.ParamBox.Controls.Add(this.PSelectIndex);
      this.ParamBox.Controls.Add(this.PSearchDelta);
      this.ParamBox.Controls.Add(this.PDelayTime);
      this.ParamBox.Controls.Add(this.label11);
      this.ParamBox.Controls.Add(this.label10);
      this.ParamBox.Controls.Add(this.label8);
      this.ParamBox.Controls.Add(this.label7);
      this.ParamBox.Controls.Add(this.label6);
      this.ParamBox.Controls.Add(this.label5);
      this.ParamBox.Controls.Add(this.PRSCount);
      this.ParamBox.Controls.Add(this.PExpMaxLength);
      this.ParamBox.Controls.Add(this.PCoolDownLength);
      this.ParamBox.Controls.Add(this.PStimLength);
      this.ParamBox.Controls.Add(this.label1);
      this.ParamBox.Controls.Add(this.label4);
      this.ParamBox.Controls.Add(this.label3);
      this.ParamBox.Controls.Add(this.label2);
      this.ParamBox.Location = new System.Drawing.Point(12, 11);
      this.ParamBox.Name = "ParamBox";
      this.ParamBox.Size = new System.Drawing.Size(398, 260);
      this.ParamBox.TabIndex = 4;
      this.ParamBox.TabStop = false;
      this.ParamBox.Text = "Параметры цикла";
      // 
      // StimBreakCheckBox
      // 
      this.StimBreakCheckBox.AutoSize = true;
      this.StimBreakCheckBox.CheckAlign = System.Drawing.ContentAlignment.BottomRight;
      this.StimBreakCheckBox.Checked = true;
      this.StimBreakCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
      this.StimBreakCheckBox.ImageAlign = System.Drawing.ContentAlignment.BottomRight;
      this.StimBreakCheckBox.Location = new System.Drawing.Point(6, 229);
      this.StimBreakCheckBox.Name = "StimBreakCheckBox";
      this.StimBreakCheckBox.Size = new System.Drawing.Size(147, 17);
      this.StimBreakCheckBox.TabIndex = 8;
      this.StimBreakCheckBox.Text = "выключать стимуляцию";
      this.StimBreakCheckBox.UseVisualStyleBackColor = true;
      // 
      // PSelectName
      // 
      this.PSelectName.Location = new System.Drawing.Point(331, 228);
      this.PSelectName.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PSelectName.Name = "PSelectName";
      this.PSelectName.Size = new System.Drawing.Size(58, 20);
      this.PSelectName.TabIndex = 7;
      this.PSelectName.Value = new decimal(new int[] {
            55,
            0,
            0,
            0});
      this.PSelectName.ValueChanged += new System.EventHandler(this.SelectName_ValueChanged);
      // 
      // PSelectIndex
      // 
      this.PSelectIndex.Location = new System.Drawing.Point(331, 202);
      this.PSelectIndex.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PSelectIndex.Name = "PSelectIndex";
      this.PSelectIndex.Size = new System.Drawing.Size(58, 20);
      this.PSelectIndex.TabIndex = 7;
      this.PSelectIndex.Value = new decimal(new int[] {
            55,
            0,
            0,
            0});
      this.PSelectIndex.ValueChanged += new System.EventHandler(this.SelectIndex_ValueChanged);
      // 
      // PSearchDelta
      // 
      this.PSearchDelta.Location = new System.Drawing.Point(331, 161);
      this.PSearchDelta.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
      this.PSearchDelta.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PSearchDelta.Name = "PSearchDelta";
      this.PSearchDelta.Size = new System.Drawing.Size(58, 20);
      this.PSearchDelta.TabIndex = 7;
      this.PSearchDelta.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      // 
      // PDelayTime
      // 
      this.PDelayTime.Location = new System.Drawing.Point(331, 135);
      this.PDelayTime.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.PDelayTime.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PDelayTime.Name = "PDelayTime";
      this.PDelayTime.Size = new System.Drawing.Size(58, 20);
      this.PDelayTime.TabIndex = 7;
      this.PDelayTime.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
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
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(203, 204);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(110, 13);
      this.label10.TabIndex = 6;
      this.label10.Text = "Порядковый индекс";
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
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(6, 163);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(173, 13);
      this.label7.TabIndex = 6;
      this.label7.Text = "Разброс поиска активности - мс";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(6, 137);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(262, 13);
      this.label6.TabIndex = 6;
      this.label6.Text = "Задержка ожидаемой активности от стимула - мс";
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
      // PRSCount
      // 
      this.PRSCount.Location = new System.Drawing.Point(331, 98);
      this.PRSCount.Maximum = new decimal(new int[] {
            3,
            0,
            0,
            0});
      this.PRSCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PRSCount.Name = "PRSCount";
      this.PRSCount.Size = new System.Drawing.Size(28, 20);
      this.PRSCount.TabIndex = 4;
      this.PRSCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PRSCount.ValueChanged += new System.EventHandler(this.PRSCount_ValueChanged);
      // 
      // PExpMaxLength
      // 
      this.PExpMaxLength.Location = new System.Drawing.Point(331, 72);
      this.PExpMaxLength.Maximum = new decimal(new int[] {
            600,
            0,
            0,
            0});
      this.PExpMaxLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PExpMaxLength.Name = "PExpMaxLength";
      this.PExpMaxLength.Size = new System.Drawing.Size(62, 20);
      this.PExpMaxLength.TabIndex = 4;
      this.PExpMaxLength.Value = new decimal(new int[] {
            120,
            0,
            0,
            0});
      // 
      // PCoolDownLength
      // 
      this.PCoolDownLength.Location = new System.Drawing.Point(331, 46);
      this.PCoolDownLength.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.PCoolDownLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PCoolDownLength.Name = "PCoolDownLength";
      this.PCoolDownLength.Size = new System.Drawing.Size(62, 20);
      this.PCoolDownLength.TabIndex = 4;
      this.PCoolDownLength.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
      // 
      // PStimLength
      // 
      this.PStimLength.Location = new System.Drawing.Point(331, 20);
      this.PStimLength.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.PStimLength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.PStimLength.Name = "PStimLength";
      this.PStimLength.Size = new System.Drawing.Size(62, 20);
      this.PStimLength.TabIndex = 4;
      this.PStimLength.Value = new decimal(new int[] {
            50,
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
      // RSBurstBox
      // 
      this.RSBurstBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RSBurstBox.Controls.Add(this.RSPacks);
      this.RSBurstBox.Location = new System.Drawing.Point(416, 11);
      this.RSBurstBox.Name = "RSBurstBox";
      this.RSBurstBox.Size = new System.Drawing.Size(423, 473);
      this.RSBurstBox.TabIndex = 8;
      this.RSBurstBox.TabStop = false;
      this.RSBurstBox.Text = "Ответ культуры";
      // 
      // RSPacks
      // 
      this.RSPacks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.RSPacks.AutoScroll = true;
      this.RSPacks.Location = new System.Drawing.Point(6, 19);
      this.RSPacks.Name = "RSPacks";
      this.RSPacks.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
      this.RSPacks.Size = new System.Drawing.Size(411, 448);
      this.RSPacks.TabIndex = 0;
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
      this.TimeStamp.ReadOnly = true;
      this.TimeStamp.Size = new System.Drawing.Size(73, 20);
      this.TimeStamp.TabIndex = 10;
      // 
      // LernLogTextBox
      // 
      this.LernLogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.LernLogTextBox.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.LernLogTextBox.Location = new System.Drawing.Point(7, 335);
      this.LernLogTextBox.Multiline = true;
      this.LernLogTextBox.Name = "LernLogTextBox";
      this.LernLogTextBox.ReadOnly = true;
      this.LernLogTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
      this.LernLogTextBox.Size = new System.Drawing.Size(398, 317);
      this.LernLogTextBox.TabIndex = 11;
      this.LernLogTextBox.TextChanged += new System.EventHandler(this.LernLogTextBox_TextChanged);
      // 
      // EvPacksBox
      // 
      this.EvPacksBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.EvPacksBox.Controls.Add(this.evBurstPanel);
      this.EvPacksBox.Location = new System.Drawing.Point(845, 11);
      this.EvPacksBox.Name = "EvPacksBox";
      this.EvPacksBox.Size = new System.Drawing.Size(393, 647);
      this.EvPacksBox.TabIndex = 12;
      this.EvPacksBox.TabStop = false;
      this.EvPacksBox.Text = "Вызванные пачки";
      // 
      // evBurstPanel
      // 
      this.evBurstPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.evBurstPanel.Location = new System.Drawing.Point(6, 19);
      this.evBurstPanel.Name = "evBurstPanel";
      this.evBurstPanel.Size = new System.Drawing.Size(381, 622);
      this.evBurstPanel.TabIndex = 0;
      // 
      // TrainEvolutionGraph
      // 
      this.TrainEvolutionGraph.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.TrainEvolutionGraph.BackColor = System.Drawing.SystemColors.ControlLightLight;
      this.TrainEvolutionGraph.Location = new System.Drawing.Point(416, 498);
      this.TrainEvolutionGraph.Name = "TrainEvolutionGraph";
      this.TrainEvolutionGraph.Size = new System.Drawing.Size(417, 154);
      this.TrainEvolutionGraph.TabIndex = 0;
      this.TrainEvolutionGraph.TabStop = false;
      this.TrainEvolutionGraph.Click += new System.EventHandler(this.TrainEvolutionGraph_Click);
      this.TrainEvolutionGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox1_Paint);
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(122, 311);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(107, 13);
      this.label12.TabIndex = 6;
      this.label12.Text = "Ручное прерывание";
      // 
      // RSManualButton
      // 
      this.RSManualButton.Enabled = false;
      this.RSManualButton.Location = new System.Drawing.Point(235, 306);
      this.RSManualButton.Name = "RSManualButton";
      this.RSManualButton.Size = new System.Drawing.Size(98, 23);
      this.RSManualButton.TabIndex = 5;
      this.RSManualButton.Text = "R/S выполнен";
      this.RSManualButton.UseVisualStyleBackColor = true;
      this.RSManualButton.Click += new System.EventHandler(this.RSManualButton_Click);
      // 
      // FLearnCycle
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1251, 663);
      this.Controls.Add(this.EvPacksBox);
      this.Controls.Add(this.LernLogTextBox);
      this.Controls.Add(this.TimeStamp);
      this.Controls.Add(this.label9);
      this.Controls.Add(this.RSBurstBox);
      this.Controls.Add(this.FinishCycle);
      this.Controls.Add(this.RSManualButton);
      this.Controls.Add(this.StartCycle);
      this.Controls.Add(this.label12);
      this.Controls.Add(this.ParamBox);
      this.Controls.Add(this.TrainEvolutionGraph);
      this.Name = "FLearnCycle";
      this.Text = "FLearnCycle";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FLearnCycle_FormClosing);
      this.Load += new System.EventHandler(this.FLearnCycle_Load);
      this.ResizeEnd += new System.EventHandler(this.FLearnCycle_ResizeEnd);
      this.ParamBox.ResumeLayout(false);
      this.ParamBox.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PSelectName)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PSelectIndex)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PSearchDelta)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PDelayTime)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PRSCount)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PExpMaxLength)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PCoolDownLength)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PStimLength)).EndInit();
      this.RSBurstBox.ResumeLayout(false);
      this.EvPacksBox.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.TrainEvolutionGraph)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.GroupBox ParamBox;
    private System.Windows.Forms.NumericUpDown PExpMaxLength;
    private System.Windows.Forms.NumericUpDown PCoolDownLength;
    private System.Windows.Forms.NumericUpDown PStimLength;
    private System.Windows.Forms.Button StartCycle;
    private System.Windows.Forms.Button FinishCycle;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.NumericUpDown PRSCount;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox RSBurstBox;
    private System.Windows.Forms.Panel RSPacks;
    private System.Windows.Forms.NumericUpDown PSearchDelta;
    private System.Windows.Forms.NumericUpDown PDelayTime;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.NumericUpDown PSelectIndex;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.TextBox TimeStamp;
    private System.Windows.Forms.NumericUpDown PSelectName;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label10;
    private volatile System.Windows.Forms.TextBox LernLogTextBox;
    private System.Windows.Forms.GroupBox EvPacksBox;
    private System.Windows.Forms.Panel evBurstPanel;
    private System.Windows.Forms.PictureBox TrainEvolutionGraph;
    private System.Windows.Forms.CheckBox StimBreakCheckBox;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.Button RSManualButton;
  }
}