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
      this.label15 = new System.Windows.Forms.Label();
      this.StimPadding = new System.Windows.Forms.TextBox();
      this.label10 = new System.Windows.Forms.Label();
      this.trackBar1 = new System.Windows.Forms.TrackBar();
      this.button1 = new System.Windows.Forms.Button();
      this.label9 = new System.Windows.Forms.Label();
      this.StimType = new System.Windows.Forms.ComboBox();
      this.StartStimButton = new System.Windows.Forms.Button();
      this.StatResult = new System.Windows.Forms.GroupBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.CurrentAverage = new System.Windows.Forms.TextBox();
      this.label26 = new System.Windows.Forms.Label();
      this.CurrentSigma = new System.Windows.Forms.TextBox();
      this.label25 = new System.Windows.Forms.Label();
      this.label20 = new System.Windows.Forms.Label();
      this.groupBox6 = new System.Windows.Forms.GroupBox();
      this.StatWindowMinCount = new System.Windows.Forms.NumericUpDown();
      this.label23 = new System.Windows.Forms.Label();
      this.StatWindowSecCount = new System.Windows.Forms.NumericUpDown();
      this.label24 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label3 = new System.Windows.Forms.Label();
      this.SelectedAverageBox = new System.Windows.Forms.TextBox();
      this.SelectedSigmaBox = new System.Windows.Forms.TextBox();
      this.label4 = new System.Windows.Forms.Label();
      this.label19 = new System.Windows.Forms.Label();
      this.label18 = new System.Windows.Forms.Label();
      this.label17 = new System.Windows.Forms.Label();
      this.StatGraphYRange = new System.Windows.Forms.NumericUpDown();
      this.StatGraphXRange = new System.Windows.Forms.NumericUpDown();
      this.label16 = new System.Windows.Forms.Label();
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
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.StatWindowMinCount)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.StatWindowSecCount)).BeginInit();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.StatGraphYRange)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.StatGraphXRange)).BeginInit();
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
      this.Prepearing.Location = new System.Drawing.Point(11, 12);
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
      this.StimParams.Controls.Add(this.label15);
      this.StimParams.Controls.Add(this.StimPadding);
      this.StimParams.Controls.Add(this.label10);
      this.StimParams.Controls.Add(this.trackBar1);
      this.StimParams.Location = new System.Drawing.Point(11, 390);
      this.StimParams.Name = "StimParams";
      this.StimParams.Size = new System.Drawing.Size(461, 74);
      this.StimParams.TabIndex = 11;
      this.StimParams.TabStop = false;
      this.StimParams.Text = "Выбор параметров стимуляции";
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Location = new System.Drawing.Point(331, 26);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(100, 13);
      this.label15.TabIndex = 6;
      this.label15.Text = "(в миллисекундах)";
      // 
      // StimPadding
      // 
      this.StimPadding.Location = new System.Drawing.Point(213, 23);
      this.StimPadding.Name = "StimPadding";
      this.StimPadding.Size = new System.Drawing.Size(111, 20);
      this.StimPadding.TabIndex = 5;
      this.StimPadding.TextChanged += new System.EventHandler(this.StimPadding_TextChanged);
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
      this.trackBar1.AutoSize = false;
      this.trackBar1.Location = new System.Drawing.Point(5, 51);
      this.trackBar1.Maximum = 2000;
      this.trackBar1.Name = "trackBar1";
      this.trackBar1.Size = new System.Drawing.Size(448, 22);
      this.trackBar1.SmallChange = 20;
      this.trackBar1.TabIndex = 3;
      this.trackBar1.TickFrequency = 15;
      this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
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
      this.StatResult.Controls.Add(this.groupBox4);
      this.StatResult.Controls.Add(this.groupBox3);
      this.StatResult.Controls.Add(this.label19);
      this.StatResult.Controls.Add(this.label18);
      this.StatResult.Controls.Add(this.label17);
      this.StatResult.Controls.Add(this.StatGraphYRange);
      this.StatResult.Controls.Add(this.StatGraphXRange);
      this.StatResult.Controls.Add(this.label16);
      this.StatResult.Controls.Add(this.DistribGrath);
      this.StatResult.Controls.Add(this.label1);
      this.StatResult.Location = new System.Drawing.Point(11, 62);
      this.StatResult.Name = "StatResult";
      this.StatResult.Size = new System.Drawing.Size(461, 322);
      this.StatResult.TabIndex = 10;
      this.StatResult.TabStop = false;
      this.StatResult.Text = "Результат сбора статистики";
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.CurrentAverage);
      this.groupBox4.Controls.Add(this.label26);
      this.groupBox4.Controls.Add(this.CurrentSigma);
      this.groupBox4.Controls.Add(this.label25);
      this.groupBox4.Controls.Add(this.label20);
      this.groupBox4.Controls.Add(this.groupBox6);
      this.groupBox4.Controls.Add(this.StatWindowMinCount);
      this.groupBox4.Controls.Add(this.label23);
      this.groupBox4.Controls.Add(this.StatWindowSecCount);
      this.groupBox4.Controls.Add(this.label24);
      this.groupBox4.Location = new System.Drawing.Point(5, 227);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(350, 91);
      this.groupBox4.TabIndex = 10;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Текущаяя статистика";
      // 
      // CurrentAverage
      // 
      this.CurrentAverage.Location = new System.Drawing.Point(126, 42);
      this.CurrentAverage.Name = "CurrentAverage";
      this.CurrentAverage.Size = new System.Drawing.Size(66, 20);
      this.CurrentAverage.TabIndex = 18;
      // 
      // label26
      // 
      this.label26.AutoSize = true;
      this.label26.Location = new System.Drawing.Point(11, 45);
      this.label26.Name = "label26";
      this.label26.Size = new System.Drawing.Size(50, 13);
      this.label26.TabIndex = 17;
      this.label26.Text = "Среднее";
      // 
      // CurrentSigma
      // 
      this.CurrentSigma.Location = new System.Drawing.Point(126, 65);
      this.CurrentSigma.Name = "CurrentSigma";
      this.CurrentSigma.Size = new System.Drawing.Size(66, 20);
      this.CurrentSigma.TabIndex = 2;
      // 
      // label25
      // 
      this.label25.AutoSize = true;
      this.label25.Location = new System.Drawing.Point(226, 16);
      this.label25.Name = "label25";
      this.label25.Size = new System.Drawing.Size(42, 13);
      this.label25.TabIndex = 16;
      this.label25.Text = "секунд";
      // 
      // label20
      // 
      this.label20.AutoSize = true;
      this.label20.Location = new System.Drawing.Point(11, 68);
      this.label20.Name = "label20";
      this.label20.Size = new System.Drawing.Size(39, 13);
      this.label20.TabIndex = 3;
      this.label20.Text = "Сигма\r\n";
      // 
      // groupBox6
      // 
      this.groupBox6.Location = new System.Drawing.Point(102, 250);
      this.groupBox6.Name = "groupBox6";
      this.groupBox6.Size = new System.Drawing.Size(393, 88);
      this.groupBox6.TabIndex = 15;
      this.groupBox6.TabStop = false;
      this.groupBox6.Text = "Статистика текущая";
      // 
      // StatWindowMinCount
      // 
      this.StatWindowMinCount.Location = new System.Drawing.Point(102, 14);
      this.StatWindowMinCount.Maximum = new decimal(new int[] {
            60,
            0,
            0,
            0});
      this.StatWindowMinCount.Name = "StatWindowMinCount";
      this.StatWindowMinCount.Size = new System.Drawing.Size(34, 20);
      this.StatWindowMinCount.TabIndex = 11;
      this.StatWindowMinCount.ValueChanged += new System.EventHandler(this.StatWindowCount_ValueChanged);
      this.StatWindowMinCount.Click += new System.EventHandler(this.StatWindowCount_ValueChanged);
      this.StatWindowMinCount.DoubleClick += new System.EventHandler(this.StatWindowCount_ValueChanged);
      // 
      // label23
      // 
      this.label23.AutoSize = true;
      this.label23.Location = new System.Drawing.Point(11, 16);
      this.label23.Name = "label23";
      this.label23.Size = new System.Drawing.Size(82, 13);
      this.label23.TabIndex = 12;
      this.label23.Text = "Окно подсчета";
      // 
      // StatWindowSecCount
      // 
      this.StatWindowSecCount.Location = new System.Drawing.Point(185, 14);
      this.StatWindowSecCount.Maximum = new decimal(new int[] {
            59,
            0,
            0,
            0});
      this.StatWindowSecCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.StatWindowSecCount.Name = "StatWindowSecCount";
      this.StatWindowSecCount.Size = new System.Drawing.Size(35, 20);
      this.StatWindowSecCount.TabIndex = 14;
      this.StatWindowSecCount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.StatWindowSecCount.ValueChanged += new System.EventHandler(this.StatWindowCount_ValueChanged);
      this.StatWindowSecCount.Click += new System.EventHandler(this.StatWindowCount_ValueChanged);
      this.StatWindowSecCount.DoubleClick += new System.EventHandler(this.StatWindowCount_ValueChanged);
      // 
      // label24
      // 
      this.label24.AutoSize = true;
      this.label24.Location = new System.Drawing.Point(142, 16);
      this.label24.Name = "label24";
      this.label24.Size = new System.Drawing.Size(37, 13);
      this.label24.TabIndex = 13;
      this.label24.Text = "минут";
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.label3);
      this.groupBox3.Controls.Add(this.SelectedAverageBox);
      this.groupBox3.Controls.Add(this.SelectedSigmaBox);
      this.groupBox3.Controls.Add(this.label4);
      this.groupBox3.Location = new System.Drawing.Point(5, 158);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(350, 63);
      this.groupBox3.TabIndex = 9;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Статистика до стимуляции";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(11, 16);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(109, 13);
      this.label3.TabIndex = 3;
      this.label3.Text = "Выбранное среднее";
      // 
      // SelectedAverageBox
      // 
      this.SelectedAverageBox.Location = new System.Drawing.Point(126, 13);
      this.SelectedAverageBox.Name = "SelectedAverageBox";
      this.SelectedAverageBox.Size = new System.Drawing.Size(66, 20);
      this.SelectedAverageBox.TabIndex = 2;
      // 
      // SelectedSigmaBox
      // 
      this.SelectedSigmaBox.Location = new System.Drawing.Point(126, 37);
      this.SelectedSigmaBox.Name = "SelectedSigmaBox";
      this.SelectedSigmaBox.Size = new System.Drawing.Size(66, 20);
      this.SelectedSigmaBox.TabIndex = 2;
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(11, 40);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(39, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "Сигма\r\n";
      // 
      // label19
      // 
      this.label19.AutoSize = true;
      this.label19.Location = new System.Drawing.Point(413, 27);
      this.label19.Name = "label19";
      this.label19.Size = new System.Drawing.Size(36, 13);
      this.label19.TabIndex = 8;
      this.label19.Text = "пачек";
      // 
      // label18
      // 
      this.label18.AutoSize = true;
      this.label18.Location = new System.Drawing.Point(244, 27);
      this.label18.Name = "label18";
      this.label18.Size = new System.Drawing.Size(42, 13);
      this.label18.TabIndex = 7;
      this.label18.Text = "секунд";
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Location = new System.Drawing.Point(304, 27);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(51, 13);
      this.label17.TabIndex = 6;
      this.label17.Text = "по игрек";
      // 
      // StatGraphYRange
      // 
      this.StatGraphYRange.Location = new System.Drawing.Point(361, 25);
      this.StatGraphYRange.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
      this.StatGraphYRange.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
      this.StatGraphYRange.Name = "StatGraphYRange";
      this.StatGraphYRange.Size = new System.Drawing.Size(46, 20);
      this.StatGraphYRange.TabIndex = 5;
      this.StatGraphYRange.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
      this.StatGraphYRange.ValueChanged += new System.EventHandler(this.StatGraphYRange_ValueChanged);
      // 
      // StatGraphXRange
      // 
      this.StatGraphXRange.Location = new System.Drawing.Point(197, 25);
      this.StatGraphXRange.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.StatGraphXRange.Name = "StatGraphXRange";
      this.StatGraphXRange.Size = new System.Drawing.Size(35, 20);
      this.StatGraphXRange.TabIndex = 5;
      this.StatGraphXRange.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
      this.StatGraphXRange.ValueChanged += new System.EventHandler(this.StatGraphXRange_ValueChanged);
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Location = new System.Drawing.Point(157, 27);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(40, 13);
      this.label16.TabIndex = 4;
      this.label16.Text = "по икс";
      // 
      // DistribGrath
      // 
      this.DistribGrath.BackColor = System.Drawing.SystemColors.Window;
      this.DistribGrath.Location = new System.Drawing.Point(6, 51);
      this.DistribGrath.Name = "DistribGrath";
      this.DistribGrath.Size = new System.Drawing.Size(449, 101);
      this.DistribGrath.TabIndex = 1;
      this.DistribGrath.TabStop = false;
      this.DistribGrath.Click += new System.EventHandler(this.DistribGrath_Click);
      this.DistribGrath.Paint += new System.Windows.Forms.PaintEventHandler(this.DistribGrath_Paint);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(5, 27);
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
      this.PackProbability.Location = new System.Drawing.Point(11, 578);
      this.PackProbability.Name = "PackProbability";
      this.PackProbability.Size = new System.Drawing.Size(461, 146);
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
      this.SecondCount.Location = new System.Drawing.Point(434, 127);
      this.SecondCount.Name = "SecondCount";
      this.SecondCount.Size = new System.Drawing.Size(19, 13);
      this.SecondCount.TabIndex = 5;
      this.SecondCount.Text = "00";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(386, 127);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(42, 13);
      this.label8.TabIndex = 4;
      this.label8.Text = "секунд";
      // 
      // MinuteCount
      // 
      this.MinuteCount.AutoSize = true;
      this.MinuteCount.Location = new System.Drawing.Point(361, 127);
      this.MinuteCount.Name = "MinuteCount";
      this.MinuteCount.Size = new System.Drawing.Size(19, 13);
      this.MinuteCount.TabIndex = 5;
      this.MinuteCount.Text = "00";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(318, 127);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(37, 13);
      this.label7.TabIndex = 4;
      this.label7.Text = "минут";
      // 
      // HourCount
      // 
      this.HourCount.AutoSize = true;
      this.HourCount.Location = new System.Drawing.Point(293, 127);
      this.HourCount.Name = "HourCount";
      this.HourCount.Size = new System.Drawing.Size(19, 13);
      this.HourCount.TabIndex = 5;
      this.HourCount.Text = "00";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(251, 127);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(36, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "часов";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(141, 127);
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
      this.PackCountGraph.Click += new System.EventHandler(this.PackCountGraph_Click);
      this.PackCountGraph.Paint += new System.Windows.Forms.PaintEventHandler(this.PackCountGraph_Paint);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.GraphChannelSelectButton);
      this.groupBox1.Location = new System.Drawing.Point(11, 470);
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
      this.groupBox2.Location = new System.Drawing.Point(11, 528);
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
      this.ClientSize = new System.Drawing.Size(478, 726);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.StimParams);
      this.Controls.Add(this.StatResult);
      this.Controls.Add(this.PackProbability);
      this.Controls.Add(this.Prepearing);
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
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.StatWindowMinCount)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.StatWindowSecCount)).EndInit();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.StatGraphYRange)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.StatGraphXRange)).EndInit();
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
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.TextBox StimPadding;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.NumericUpDown StatGraphYRange;
    private System.Windows.Forms.NumericUpDown StatGraphXRange;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.Label label19;
    private System.Windows.Forms.Label label18;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.TextBox CurrentAverage;
    private System.Windows.Forms.Label label26;
    private System.Windows.Forms.Label label25;
    private System.Windows.Forms.GroupBox groupBox6;
    private System.Windows.Forms.NumericUpDown StatWindowMinCount;
    private System.Windows.Forms.Label label23;
    private System.Windows.Forms.NumericUpDown StatWindowSecCount;
    private System.Windows.Forms.Label label24;
    private System.Windows.Forms.TextBox CurrentSigma;
    private System.Windows.Forms.Label label20;
  }
}