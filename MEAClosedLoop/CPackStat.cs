using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace MEAClosedLoop
{
   public partial class CPackStat : Form
  {
    private Button button1;
    private ProgressBar progressBar1;
    private NumericUpDown numericUpDown1;
    private GroupBox groupBox1;
    private GroupBox groupBox2;
    private Label label8;
    private Label label7;
    private Label label6;
    private Label label5;
    private PictureBox pictureBox2;
    private GroupBox groupBox3;
    private Label label4;
    private Label label3;
    private TextBox textBox2;
    private TextBox textBox1;
    private PictureBox pictureBox1;
    private Label label1;
    private GroupBox groupBox4;
    private Button button2;
    private Label label2;


     private void InitializeComponent()
     {
      this.button1 = new System.Windows.Forms.Button();
      this.progressBar1 = new System.Windows.Forms.ProgressBar();
      this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
      this.label2 = new System.Windows.Forms.Label();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.groupBox2 = new System.Windows.Forms.GroupBox();
      this.groupBox3 = new System.Windows.Forms.GroupBox();
      this.label1 = new System.Windows.Forms.Label();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.groupBox4 = new System.Windows.Forms.GroupBox();
      this.pictureBox2 = new System.Windows.Forms.PictureBox();
      this.button2 = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label8 = new System.Windows.Forms.Label();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
      this.groupBox1.SuspendLayout();
      this.groupBox2.SuspendLayout();
      this.groupBox3.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.groupBox4.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(328, 13);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(126, 23);
      this.button1.TabIndex = 0;
      this.button1.Text = "CollectStat";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // progressBar1
      // 
      this.progressBar1.Location = new System.Drawing.Point(216, 13);
      this.progressBar1.Name = "progressBar1";
      this.progressBar1.Size = new System.Drawing.Size(100, 23);
      this.progressBar1.TabIndex = 1;
      // 
      // numericUpDown1
      // 
      this.numericUpDown1.Location = new System.Drawing.Point(90, 16);
      this.numericUpDown1.Name = "numericUpDown1";
      this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
      this.numericUpDown1.TabIndex = 3;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(6, 16);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(78, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "номер канала";
      this.label2.Click += new System.EventHandler(this.label2_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.button1);
      this.groupBox1.Controls.Add(this.label2);
      this.groupBox1.Controls.Add(this.progressBar1);
      this.groupBox1.Controls.Add(this.numericUpDown1);
      this.groupBox1.Location = new System.Drawing.Point(22, 21);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(460, 76);
      this.groupBox1.TabIndex = 5;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Подготовка";
      // 
      // groupBox2
      // 
      this.groupBox2.Controls.Add(this.label8);
      this.groupBox2.Controls.Add(this.label7);
      this.groupBox2.Controls.Add(this.label6);
      this.groupBox2.Controls.Add(this.label5);
      this.groupBox2.Controls.Add(this.pictureBox2);
      this.groupBox2.Location = new System.Drawing.Point(22, 474);
      this.groupBox2.Name = "groupBox2";
      this.groupBox2.Size = new System.Drawing.Size(460, 128);
      this.groupBox2.TabIndex = 6;
      this.groupBox2.TabStop = false;
      this.groupBox2.Text = "Вероятность пачки";
      this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
      // 
      // groupBox3
      // 
      this.groupBox3.Controls.Add(this.label4);
      this.groupBox3.Controls.Add(this.label3);
      this.groupBox3.Controls.Add(this.textBox2);
      this.groupBox3.Controls.Add(this.textBox1);
      this.groupBox3.Controls.Add(this.pictureBox1);
      this.groupBox3.Controls.Add(this.label1);
      this.groupBox3.Location = new System.Drawing.Point(22, 104);
      this.groupBox3.Name = "groupBox3";
      this.groupBox3.Size = new System.Drawing.Size(460, 209);
      this.groupBox3.TabIndex = 7;
      this.groupBox3.TabStop = false;
      this.groupBox3.Text = "Результат сбора статистики";
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
      // pictureBox1
      // 
      this.pictureBox1.BackColor = System.Drawing.SystemColors.Window;
      this.pictureBox1.Location = new System.Drawing.Point(10, 37);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(444, 101);
      this.pictureBox1.TabIndex = 1;
      this.pictureBox1.TabStop = false;
      // 
      // groupBox4
      // 
      this.groupBox4.Controls.Add(this.button2);
      this.groupBox4.Location = new System.Drawing.Point(23, 324);
      this.groupBox4.Name = "groupBox4";
      this.groupBox4.Size = new System.Drawing.Size(459, 100);
      this.groupBox4.TabIndex = 8;
      this.groupBox4.TabStop = false;
      this.groupBox4.Text = "Параметры стимуляции";
      // 
      // pictureBox2
      // 
      this.pictureBox2.BackColor = System.Drawing.SystemColors.Window;
      this.pictureBox2.Location = new System.Drawing.Point(1, 19);
      this.pictureBox2.Name = "pictureBox2";
      this.pictureBox2.Size = new System.Drawing.Size(453, 75);
      this.pictureBox2.TabIndex = 2;
      this.pictureBox2.TabStop = false;
      // 
      // button2
      // 
      this.button2.Location = new System.Drawing.Point(327, 71);
      this.button2.Name = "button2";
      this.button2.Size = new System.Drawing.Size(132, 23);
      this.button2.TabIndex = 0;
      this.button2.Text = "Начать стимуляцию";
      this.button2.UseVisualStyleBackColor = true;
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(150, 156);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(100, 20);
      this.textBox1.TabIndex = 2;
      // 
      // textBox2
      // 
      this.textBox2.Location = new System.Drawing.Point(150, 182);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(100, 20);
      this.textBox2.TabIndex = 2;
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
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(10, 182);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(38, 13);
      this.label4.TabIndex = 3;
      this.label4.Text = "сигма\r\n";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(154, 97);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(96, 13);
      this.label5.TabIndex = 3;
      this.label5.Text = "Времени прошло:";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(272, 97);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(36, 13);
      this.label6.TabIndex = 4;
      this.label6.Text = "часов";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(325, 97);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(37, 13);
      this.label7.TabIndex = 4;
      this.label7.Text = "минут";
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(384, 97);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(42, 13);
      this.label8.TabIndex = 4;
      this.label8.Text = "секунд";
      // 
      // CPackStat
      // 
      this.ClientSize = new System.Drawing.Size(498, 640);
      this.Controls.Add(this.groupBox4);
      this.Controls.Add(this.groupBox3);
      this.Controls.Add(this.groupBox2);
      this.Controls.Add(this.groupBox1);
      this.Name = "CPackStat";
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.groupBox2.ResumeLayout(false);
      this.groupBox2.PerformLayout();
      this.groupBox3.ResumeLayout(false);
      this.groupBox3.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.groupBox4.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
      this.ResumeLayout(false);

     }

     private void label2_Click(object sender, EventArgs e)
     {

     }

     private void groupBox2_Enter(object sender, EventArgs e)
     {

     }
   }
}
