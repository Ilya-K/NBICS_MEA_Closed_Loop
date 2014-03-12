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
      this.label11 = new System.Windows.Forms.Label();
      this.CollectStatButton = new System.Windows.Forms.Button();
      this.StatProgressBar = new System.Windows.Forms.ProgressBar();
      this.GraphChannelSelectButton = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.StimParams = new System.Windows.Forms.GroupBox();
      this.label10 = new System.Windows.Forms.Label();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.button1 = new System.Windows.Forms.Button();
      this.label9 = new System.Windows.Forms.Label();
      this.StimType = new System.Windows.Forms.ComboBox();
      this.StartStimButton = new System.Windows.Forms.Button();
      this.StatResult = new System.Windows.Forms.GroupBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.SelectedSigmaBox = new System.Windows.Forms.TextBox();
      this.SelectedAverageBox = new System.Windows.Forms.TextBox();
      this.DistribGrath = new System.Windows.Forms.PictureBox();
      this.label1 = new System.Windows.Forms.Label();
      this.PackProbability = new System.Windows.Forms.GroupBox();
      this.label14 = new System.Windows.Forms.Label();
      this.SecondsWindow = new System.Windows.Forms.NumericUpDown();
      this.label13 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.MinutesWindow = new System.Windows.Forms.NumericUpDown();
      this.SecondCount = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      this.MinuteCount = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.HourCount = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.PackCountGraph = new System.Windows.Forms.PictureBox();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.Prepearing.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.StimParams.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
      this.StatResult.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DistribGrath)).BeginInit();
      this.PackProbability.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.SecondsWindow)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.MinutesWindow)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.PackCountGraph)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.SuspendLayout();
      // 
      // Prepearing
      // 
      this.Prepearing.Controls.Add(this.label11);
      this.Prepearing.Controls.Add(this.CollectStatButton);
      this.Prepearing.Controls.Add(this.StatProgressBar);
      this.Prepearing.Location = new System.Drawing.Point(12, 12);
      this.Prepearing.Name = "Prepearing";
      this.Prepearing.Size = new System.Drawing.Size(461, 44);
      this.Prepearing.TabIndex = 6;
      this.Prepearing.TabStop = false;
      this.Prepearing.Text = "Подготовка";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Location = new System.Drawing.Point(2, 18);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(167, 13);
      this.label11.TabIndex = 6;
      this.label11.Text = "Анализ спонтанной активности";
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
      // StatProgressBar
      // 
      this.StatProgressBar.Location = new System.Drawing.Point(176, 13);
      this.StatProgressBar.Name = "StatProgressBar";
      this.StatProgressBar.Size = new System.Drawing.Size(139, 23);
      this.StatProgressBar.TabIndex = 1;
      // 
      // GraphChannelSelectButton
      // 
      this.GraphChannelSelectButton.Location = new System.Drawing.Point(6, 19);
      this.GraphChannelSelectButton.Name = "GraphChannelSelectButton";
      this.GraphChannelSelectButton.Size = new System.Drawing.Size(133, 25);
      this.GraphChannelSelectButton.TabIndex = 5;
      this.GraphChannelSelectButton.Text = "Анализировать";
      this.GraphChannelSelectButton.UseVisualStyleBackColor = true;
      this.GraphChannelSelectButton.Click += new System.EventHandler(this.GraphChannelSelectButton_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(154, 26);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(78, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "номер канала";
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(251, 24);
      this.numericUpDown1.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(43, 20);
      this.numericUpDown1.TabIndex = 3;
      this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
      // 
      // StimParams
      // 
      this.StimParams.Controls.Add(this.label10);
      this.StimParams.Controls.Add(this.trackBar1);
      this.StimParams.Location = new System.Drawing.Point(12, 277);
      this.StimParams.Name = "StimParams";
      this.StimParams.Size = new System.Drawing.Size(461, 107);
      this.StimParams.TabIndex = 11;
      this.StimParams.TabStop = false;
      this.StimParams.Text = "Выбор параметров стимуляции";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Location = new System.Drawing.Point(2, 26);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(205, 13);
      this.label10.TabIndex = 4;
      this.label10.Text = "отступ импулься от предыдущей пачки";
      // 
      // trackBar1
      // 
      this.trackBar1.Location = new System.Drawing.Point(5, 51);
      this.trackBar1.Maximum = 2000;
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(448, 45);
      this.trackBar1.SmallChange = 20;
      this.trackBar1.TabIndex = 3;
      this.trackBar1.TickFrequency = 15;
      this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(320, 19);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(133, 23);
      this.button1.TabIndex = 5;
      this.button1.Text = "Дополнительо";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Location = new System.Drawing.Point(34, 19);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(89, 13);
      this.label9.TabIndex = 2;
      this.label9.Text = "Тип стимуляции";
      // 
      // StimType
      // 
      this.StimType.DisplayMember = "1";
      this.StimType.FormattingEnabled = true;
      this.StimType.Items.AddRange(new object[] {
            "эмуляция"});
      this.StimType.Location = new System.Drawing.Point(152, 16);
      this.StimType.Name = "StimType";
      this.StimType.Size = new System.Drawing.Size(121, 21);
      this.StimType.TabIndex = 1;
      // 
      // StartStimButton
      // 
      this.StartStimButton.Location = new System.Drawing.Point(320, 14);
      this.StartStimButton.Name = "StartStimButton";
      this.StartStimButton.Size = new System.Drawing.Size(133, 23);
      this.StartStimButton.TabIndex = 0;
      this.StartStimButton.Text = "Начать стимуляцию";
      this.StartStimButton.UseVisualStyleBackColor = true;
      this.StartStimButton.Click += new System.EventHandler(this.StartStimButton_Click);
      // 
      // StatResult
      // 
      this.StatResult.Controls.Add(this.label4);
      this.StatResult.Controls.Add(this.label3);
      this.StatResult.Controls.Add(this.SelectedSigmaBox);
      this.StatResult.Controls.Add(this.SelectedAverageBox);
      this.StatResult.Controls.Add(this.DistribGrath);
      this.StatResult.Controls.Add(this.label1);
      this.StatResult.Location = new System.Drawing.Point(12, 62);
      this.StatResult.Name = "StatResult";
      this.StatResult.Size = new System.Drawing.Size(461, 209);
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
      this.PackProbability.Controls.Add(this.label14);
      this.PackProbability.Controls.Add(this.SecondsWindow);
      this.PackProbability.Controls.Add(this.label13);
      this.PackProbability.Controls.Add(this.label12);
      this.PackProbability.Controls.Add(this.MinutesWindow);
      this.PackProbability.Controls.Add(this.SecondCount);
      this.PackProbability.Controls.Add(this.label8);
      this.PackProbability.Controls.Add(this.MinuteCount);
      this.PackProbability.Controls.Add(this.label7);
      this.PackProbability.Controls.Add(this.HourCount);
      this.PackProbability.Controls.Add(this.label6);
      this.PackProbability.Controls.Add(this.label5);
      this.PackProbability.Controls.Add(this.PackCountGraph);
      this.PackProbability.Location = new System.Drawing.Point(12, 498);
      this.PackProbability.Name = "PackProbability";
      this.PackProbability.Size = new System.Drawing.Size(461, 222);
      this.PackProbability.TabIndex = 9;
      this.PackProbability.TabStop = false;
      this.PackProbability.Text = "Вероятность пачки";
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Location = new System.Drawing.Point(232, 25);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(42, 13);
      this.label14.TabIndex = 10;
      this.label14.Text = "секунд";
      // 
      // SecondsWindow
      // 
      this.SecondsWindow.Location = new System.Drawing.Point(188, 23);
      this.SecondsWindow.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
      this.SecondsWindow.Name = "SecondsWindow";
      this.SecondsWindow.Size = new System.Drawing.Size(35, 20);
      this.SecondsWindow.TabIndex = 9;
      this.SecondsWindow.ValueChanged += new System.EventHandler(this.SecondsWindow_ValueChanged);
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Location = new System.Drawing.Point(141, 25);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(37, 13);
      this.label13.TabIndex = 8;
      this.label13.Text = "минут";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Location = new System.Drawing.Point(13, 25);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(82, 13);
      this.label12.TabIndex = 7;
      this.label12.Text = "Окно подсчета";
      // 
      // MinutesWindow
      // 
      this.MinutesWindow.Location = new System.Drawing.Point(101, 23);
      this.MinutesWindow.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
      this.MinutesWindow.Name = "MinutesWindow";
      this.MinutesWindow.Size = new System.Drawing.Size(34, 20);
      this.MinutesWindow.TabIndex = 6;
      this.MinutesWindow.ValueChanged += new System.EventHandler(this.MinutesWindow_ValueChanged);
      // 
      // SecondCount
      // 
      this.SecondCount.AutoSize = true;
      this.SecondCount.Location = new System.Drawing.Point(326, 193);
      this.SecondCount.Name = "SecondCount";
      this.SecondCount.Size = new System.Drawing.Size(19, 13);
      this.SecondCount.TabIndex = 5;
      this.SecondCount.Text = "00";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(278, 193);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(42, 13);
      this.label8.TabIndex = 4;
      this.label8.Text = "секунд";
      // 
      // MinuteCount
      // 
      this.MinuteCount.AutoSize = true;
      this.MinuteCount.Location = new System.Drawing.Point(253, 193);
      this.MinuteCount.Name = "MinuteCount";
      this.MinuteCount.Size = new System.Drawing.Size(19, 13);
      this.MinuteCount.TabIndex = 5;
      this.MinuteCount.Text = "00";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(210, 193);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(37, 13);
      this.label7.TabIndex = 4;
      this.label7.Text = "минут";
      // 
      // HourCount
      // 
      this.HourCount.AutoSize = true;
      this.HourCount.Location = new System.Drawing.Point(185, 193);
      this.HourCount.Name = "HourCount";
      this.HourCount.Size = new System.Drawing.Size(19, 13);
      this.HourCount.TabIndex = 5;
      this.HourCount.Text = "00";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(143, 193);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(36, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "часов";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(33, 193);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(96, 13);
      this.label5.TabIndex = 3;
      this.label5.Text = "Времени прошло:";
      // 
      // PackCountGraph
      // 
      this.PackCountGraph.BackColor = System.Drawing.SystemColors.Window;
      this.PackCountGraph.Location = new System.Drawing.Point(5, 49);
      this.PackCountGraph.Name = "PackCountGraph";
      this.PackCountGraph.Size = new System.Drawing.Size(449, 75);
      this.PackCountGraph.TabIndex = 2;
      this.PackCountGraph.TabStop = false;
      this.PackCountGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.PackCountGraph_Paint);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.GraphChannelSelectButton);
      this.groupBox1.Location = new System.Drawing.Point(12, 390);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(460, 52);
      this.groupBox1.TabIndex = 12;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Выбор канала";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label9);
      this.groupBox2.Controls.Add(this.StimType);
      this.groupBox2.Controls.Add(this.StartStimButton);
      this.groupBox2.Location = new System.Drawing.Point(12, 448);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(461, 44);
      this.groupBox2.TabIndex = 13;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Стимуляция";
      // 
      // CPackStat
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(516, 749);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.StimParams);
      this.Controls.Add(this.StatResult);
      this.Controls.Add(this.PackProbability);
      this.Controls.Add(this.Prepearing);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Name = "CPackStat";
      this.Text = "CPackStat";
      this.Load += new System.EventHandler(this.CPackStat_Load);
      this.Prepearing.ResumeLayout(false);
      this.Prepearing.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.StimParams.ResumeLayout(false);
      this.StimParams.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
      this.StatResult.ResumeLayout(false);
      this.StatResult.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.DistribGrath)).EndInit();
      this.PackProbability.ResumeLayout(false);
      this.PackProbability.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.SecondsWindow)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.MinutesWindow)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.PackCountGraph)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
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
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.ComboBox StimType;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button GraphChannelSelectButton;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.NumericUpDown SecondsWindow;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.NumericUpDown MinutesWindow;
  }
}