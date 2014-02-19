namespace MEAClosedLoop
{
  partial class CPackStat
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
      this.Prepearing = new System.Windows.Forms.GroupBox();
      this.CollectStatButton = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.StatProgressBar = new System.Windows.Forms.ProgressBar();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.StimParams = new System.Windows.Forms.GroupBox();
      this.StartStimButton = new System.Windows.Forms.Button();
      this.StatResult = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.SelectedSigmaBox = new System.Windows.Forms.TextBox();
      this.SelectedAverageBox = new System.Windows.Forms.TextBox();
      this.DistribGrath = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.PackProbability = new System.Windows.Forms.GroupBox();
      this.SecondCount = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.MinuteCount = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.HourCount = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.PackCountGraph = new System.Windows.Forms.PictureBox();
      this.CalcStatButton = new System.Windows.Forms.Button();
      this.Prepearing.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.StimParams.SuspendLayout();
      this.StatResult.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DistribGrath)).BeginInit();
      this.PackProbability.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PackCountGraph)).BeginInit();
      this.SuspendLayout();
      // 
      // Prepearing
      // 
      this.Prepearing.Controls.Add(this.CalcStatButton);
      this.Prepearing.Controls.Add(this.CollectStatButton);
      this.Prepearing.Controls.Add(this.label2);
      this.Prepearing.Controls.Add(this.StatProgressBar);
      this.Prepearing.Controls.Add(this.numericUpDown1);
      this.Prepearing.Location = new System.Drawing.Point(12, 12);
      this.Prepearing.Name = "Prepearing";
      this.Prepearing.Size = new System.Drawing.Size(460, 76);
      this.Prepearing.TabIndex = 6;
      this.Prepearing.TabStop = false;
      this.Prepearing.Text = "Подготовка";
      // 
      // CollectStatButton
      // 
      this.CollectStatButton.Location = new System.Drawing.Point(321, 13);
      this.CollectStatButton.Name = "CollectStatButton";
      this.CollectStatButton.Size = new System.Drawing.Size(133, 23);
      this.CollectStatButton.TabIndex = 0;
      this.CollectStatButton.Text = "Собрать Статистику";
      this.CollectStatButton.UseVisualStyleBackColor = true;
      this.CollectStatButton.Click += new System.EventHandler(this.CollectStatButton_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(78, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "номер канала";
      // 
      // StatProgressBar
      // 
      this.StatProgressBar.Location = new System.Drawing.Point(150, 13);
      this.StatProgressBar.Name = "StatProgressBar";
      this.StatProgressBar.Size = new System.Drawing.Size(139, 23);
      this.StatProgressBar.TabIndex = 1;
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(90, 16);
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(43, 20);
      this.numericUpDown1.TabIndex = 3;
      // 
      // StimParams
      // 
      this.StimParams.Controls.Add(this.StartStimButton);
      this.StimParams.Location = new System.Drawing.Point(13, 314);
      this.StimParams.Name = "StimParams";
      this.StimParams.Size = new System.Drawing.Size(459, 100);
      this.StimParams.TabIndex = 11;
      this.StimParams.TabStop = false;
      this.StimParams.Text = "Параметры стимуляции";
      // 
      // StartStimButton
      // 
      this.StartStimButton.Location = new System.Drawing.Point(327, 71);
      this.StartStimButton.Name = "StartStimButton";
      this.StartStimButton.Size = new System.Drawing.Size(132, 23);
      this.StartStimButton.TabIndex = 0;
      this.StartStimButton.Text = "Начать стимуляцию";
      this.StartStimButton.UseVisualStyleBackColor = true;
      // 
      // StatResult
      // 
      this.StatResult.Controls.Add(this.label4);
      this.StatResult.Controls.Add(this.label3);
      this.StatResult.Controls.Add(this.SelectedSigmaBox);
      this.StatResult.Controls.Add(this.SelectedAverageBox);
      this.StatResult.Controls.Add(this.DistribGrath);
      this.StatResult.Controls.Add(this.label1);
      this.StatResult.Location = new System.Drawing.Point(12, 94);
      this.StatResult.Name = "StatResult";
      this.StatResult.Size = new System.Drawing.Size(460, 209);
      this.StatResult.TabIndex = 10;
      this.StatResult.TabStop = false;
      this.StatResult.Text = "Результат сбора статистики";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(10, 182);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(38, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "сигма\r\n";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(10, 156);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(109, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Выбранное среднее";
      // 
      // SelectedSigmaBox
      // 
      this.SelectedSigmaBox.Location = new System.Drawing.Point(150, 182);
      this.SelectedSigmaBox.Name = "SelectedSigmaBox";
      this.SelectedSigmaBox.Size = new System.Drawing.Size(174, 20);
      this.SelectedSigmaBox.TabIndex = 2;
      // 
      // SelectedAverageBox
      // 
      this.SelectedAverageBox.Location = new System.Drawing.Point(150, 156);
      this.SelectedAverageBox.Name = "SelectedAverageBox";
      this.SelectedAverageBox.Size = new System.Drawing.Size(174, 20);
      this.SelectedAverageBox.TabIndex = 2;
      // 
      // DistribGrath
      // 
      this.DistribGrath.BackColor = System.Drawing.SystemColors.Window;
      this.DistribGrath.Location = new System.Drawing.Point(10, 37);
      this.DistribGrath.Name = "DistribGrath";
      this.DistribGrath.Size = new System.Drawing.Size(444, 101);
      this.DistribGrath.TabIndex = 1;
      this.DistribGrath.TabStop = false;
      this.DistribGrath.Paint += new System.Windows.Forms.PaintEventHandler(this.DistribGrath_Paint);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 20);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(126, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "График распределения";
      // 
      // PackProbability
      // 
      this.PackProbability.Controls.Add(this.SecondCount);
      this.PackProbability.Controls.Add(this.label8);
      this.PackProbability.Controls.Add(this.MinuteCount);
      this.PackProbability.Controls.Add(this.label7);
      this.PackProbability.Controls.Add(this.HourCount);
      this.PackProbability.Controls.Add(this.label6);
      this.PackProbability.Controls.Add(this.label5);
      this.PackProbability.Controls.Add(this.PackCountGraph);
      this.PackProbability.Location = new System.Drawing.Point(12, 433);
      this.PackProbability.Name = "PackProbability";
      this.PackProbability.Size = new System.Drawing.Size(460, 128);
      this.PackProbability.TabIndex = 9;
      this.PackProbability.TabStop = false;
      this.PackProbability.Text = "Вероятность пачки";
      // 
      // SecondCount
      // 
      this.SecondCount.AutoSize = true;
      this.SecondCount.Location = new System.Drawing.Point(330, 97);
      this.SecondCount.Name = "SecondCount";
      this.SecondCount.Size = new System.Drawing.Size(19, 13);
      this.SecondCount.TabIndex = 5;
      this.SecondCount.Text = "00";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(282, 97);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(42, 13);
      this.label8.TabIndex = 4;
      this.label8.Text = "секунд";
      // 
      // MinuteCount
      // 
      this.MinuteCount.AutoSize = true;
      this.MinuteCount.Location = new System.Drawing.Point(257, 97);
      this.MinuteCount.Name = "MinuteCount";
      this.MinuteCount.Size = new System.Drawing.Size(19, 13);
      this.MinuteCount.TabIndex = 5;
      this.MinuteCount.Text = "00";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(214, 97);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(37, 13);
      this.label7.TabIndex = 4;
      this.label7.Text = "минут";
      // 
      // HourCount
      // 
      this.HourCount.AutoSize = true;
      this.HourCount.Location = new System.Drawing.Point(189, 97);
      this.HourCount.Name = "HourCount";
      this.HourCount.Size = new System.Drawing.Size(19, 13);
      this.HourCount.TabIndex = 5;
      this.HourCount.Text = "00";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(147, 97);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(36, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "часов";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(37, 97);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(96, 13);
      this.label5.TabIndex = 3;
      this.label5.Text = "Времени прошло:";
      // 
      // PackCountGraph
      // 
      this.PackCountGraph.BackColor = System.Drawing.SystemColors.Window;
      this.PackCountGraph.Location = new System.Drawing.Point(10, 19);
      this.PackCountGraph.Name = "PackCountGraph";
      this.PackCountGraph.Size = new System.Drawing.Size(444, 75);
      this.PackCountGraph.TabIndex = 2;
      this.PackCountGraph.TabStop = false;
      // 
      // CalcStatButton
      // 
      this.CalcStatButton.Location = new System.Drawing.Point(321, 42);
      this.CalcStatButton.Name = "CalcStatButton";
      this.CalcStatButton.Size = new System.Drawing.Size(133, 23);
      this.CalcStatButton.TabIndex = 4;
      this.CalcStatButton.Text = "Посчитать статистику";
      this.CalcStatButton.UseVisualStyleBackColor = true;
      this.CalcStatButton.Click += new System.EventHandler(this.CalcStatButton_Click);
      // 
      // CPackStat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(486, 629);
      this.Controls.Add(this.StimParams);
      this.Controls.Add(this.StatResult);
      this.Controls.Add(this.PackProbability);
      this.Controls.Add(this.Prepearing);
      this.Name = "CPackStat";
      this.Text = "CPackStat";
      this.Prepearing.ResumeLayout(false);
      this.Prepearing.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.StimParams.ResumeLayout(false);
      this.StatResult.ResumeLayout(false);
      this.StatResult.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DistribGrath)).EndInit();
      this.PackProbability.ResumeLayout(false);
      this.PackProbability.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.PackCountGraph)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox Prepearing;
    private System.Windows.Forms.Button CollectStatButton;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ProgressBar StatProgressBar;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.GroupBox StimParams;
    private System.Windows.Forms.Button StartStimButton;
    private System.Windows.Forms.GroupBox StatResult;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox SelectedSigmaBox;
    private System.Windows.Forms.TextBox SelectedAverageBox;
    private System.Windows.Forms.PictureBox DistribGrath;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.GroupBox PackProbability;
    private System.Windows.Forms.Label SecondCount;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label MinuteCount;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label HourCount;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.PictureBox PackCountGraph;
    private System.Windows.Forms.Button CalcStatButton;
  }
}