namespace MEAClosedLoop
{
  partial class FBurstDescription
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
      this.components = new System.ComponentModel.Container();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.button1 = new System.Windows.Forms.Button();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
      this.label3 = new System.Windows.Forms.Label();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.comboBox1 = new System.Windows.Forms.ComboBox();
      this.PlotGraph = new ZedGraph.ZedGraphControl();
      this.groupBox1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
      this.SuspendLayout();
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Controls.Add(this.groupBox4);
      this.groupBox1.Controls.Add(this.groupBox3);
      this.groupBox1.Controls.Add(this.groupBox2);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.label1);
      this.groupBox1.Controls.Add(this.comboBox1);
      this.groupBox1.Location = new System.Drawing.Point(12, 12);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(897, 113);
      this.groupBox1.TabIndex = 1;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Параметры";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(215, 83);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 4;
      this.button1.Text = "Обновить";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(226, 50);
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(65, 20);
      this.numericUpDown1.TabIndex = 3;
      this.numericUpDown1.Value = new decimal(new int[] {
            26,
            0,
            0,
            0});
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.numericUpDown2);
      this.groupBox4.Controls.Add(this.label3);
      this.groupBox4.Location = new System.Drawing.Point(720, 14);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(171, 99);
      this.groupBox4.TabIndex = 2;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Интегральная с затуханием";
      // 
      // numericUpDown2
      // 
      this.numericUpDown2.Location = new System.Drawing.Point(94, 16);
      this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
      this.numericUpDown2.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
      this.numericUpDown2.Name = "numericUpDown2";
      this.numericUpDown2.Size = new System.Drawing.Size(69, 20);
      this.numericUpDown2.TabIndex = 1;
      this.numericUpDown2.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
      this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(6, 18);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(82, 13);
      this.label3.TabIndex = 0;
      this.label3.Text = "Амплитуда, мв";
      // 
      // groupBox3
      // 
      this.groupBox3.Location = new System.Drawing.Point(514, 11);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(200, 99);
      this.groupBox3.TabIndex = 2;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Интегральная с затуханием";
      // 
      // groupBox2
      // 
      this.groupBox2.Location = new System.Drawing.Point(297, 11);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(211, 96);
      this.groupBox2.TabIndex = 2;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Скользящее среднее";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(7, 52);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(80, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Номер канала";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(7, 26);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(72, 13);
      this.label1.TabIndex = 1;
      this.label1.Text = "Тип функции";
      // 
      // comboBox1
      // 
      this.comboBox1.FormattingEnabled = true;
      this.comboBox1.Items.AddRange(new object[] {
            "Скользящее среднее",
            "Итегральная с затуханием"});
      this.comboBox1.Location = new System.Drawing.Point(99, 23);
      this.comboBox1.Name = "comboBox1";
      this.comboBox1.Size = new System.Drawing.Size(192, 21);
      this.comboBox1.TabIndex = 0;
      // 
      // PlotGraph
      // 
      this.PlotGraph.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.PlotGraph.IsAntiAlias = true;
      this.PlotGraph.Location = new System.Drawing.Point(22, 131);
      this.PlotGraph.Name = "PlotGraph";
      this.PlotGraph.ScrollGrace = 0D;
      this.PlotGraph.ScrollMaxX = 0D;
      this.PlotGraph.ScrollMaxY = 0D;
      this.PlotGraph.ScrollMaxY2 = 0D;
      this.PlotGraph.ScrollMinX = 0D;
      this.PlotGraph.ScrollMinY = 0D;
      this.PlotGraph.ScrollMinY2 = 0D;
      this.PlotGraph.Size = new System.Drawing.Size(887, 237);
      this.PlotGraph.TabIndex = 2;
      // 
      // FBurstDescription
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(921, 380);
      this.Controls.Add(this.PlotGraph);
      this.Controls.Add(this.groupBox1);
      this.Name = "FBurstDescription";
      this.Text = "FBurstDescription";
      this.Load += new System.EventHandler(this.FBurstDescription_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.groupBox4.ResumeLayout(false);
      this.groupBox4.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.GroupBox groupBox3;
    private System.Windows.Forms.GroupBox groupBox2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.GroupBox groupBox4;
    private System.Windows.Forms.NumericUpDown numericUpDown2;
    private System.Windows.Forms.Label label3;
    private ZedGraph.ZedGraphControl PlotGraph;
  }
}