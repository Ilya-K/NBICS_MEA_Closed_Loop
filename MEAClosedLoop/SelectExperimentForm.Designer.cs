namespace MEAClosedLoop
{
  partial class SelectExperimentForm
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
      this.ExpTable = new System.Windows.Forms.DataGridView();
      this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Name = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Target = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Author = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.testDBDataSetBindingSource = new System.Windows.Forms.BindingSource(this.components);
      this.testDBDataSet = new MEAClosedLoop.TestDBDataSet();
      this.SelectButton = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.ExpTable)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.testDBDataSetBindingSource)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.testDBDataSet)).BeginInit();
      this.SuspendLayout();
      // 
      // ExpTable
      // 
      this.ExpTable.AllowUserToAddRows = false;
      this.ExpTable.AllowUserToDeleteRows = false;
      this.ExpTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.ExpTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.id,
            this.Name,
            this.Target,
            this.Author});
      this.ExpTable.Location = new System.Drawing.Point(13, 13);
      this.ExpTable.Name = "ExpTable";
      this.ExpTable.ReadOnly = true;
      this.ExpTable.Size = new System.Drawing.Size(773, 225);
      this.ExpTable.TabIndex = 0;
      // 
      // id
      // 
      this.id.FillWeight = 30F;
      this.id.HeaderText = "id";
      this.id.MinimumWidth = 20;
      this.id.Name = "id";
      this.id.ReadOnly = true;
      this.id.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.id.Width = 30;
      // 
      // Name
      // 
      this.Name.FillWeight = 250F;
      this.Name.HeaderText = "Name";
      this.Name.Name = "Name";
      this.Name.ReadOnly = true;
      this.Name.Width = 250;
      // 
      // Target
      // 
      this.Target.FillWeight = 350F;
      this.Target.HeaderText = "Target";
      this.Target.Name = "Target";
      this.Target.ReadOnly = true;
      this.Target.Width = 350;
      // 
      // Author
      // 
      this.Author.HeaderText = "Author";
      this.Author.Name = "Author";
      this.Author.ReadOnly = true;
      // 
      // testDBDataSetBindingSource
      // 
      this.testDBDataSetBindingSource.DataSource = this.testDBDataSet;
      this.testDBDataSetBindingSource.Position = 0;
      // 
      // testDBDataSet
      // 
      this.testDBDataSet.DataSetName = "TestDBDataSet";
      this.testDBDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
      // 
      // SelectButton
      // 
      this.SelectButton.Location = new System.Drawing.Point(13, 249);
      this.SelectButton.Name = "SelectButton";
      this.SelectButton.Size = new System.Drawing.Size(75, 23);
      this.SelectButton.TabIndex = 1;
      this.SelectButton.Text = "Select";
      this.SelectButton.UseVisualStyleBackColor = true;
      this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
      // 
      // SelectExperimentForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(798, 284);
      this.Controls.Add(this.SelectButton);
      this.Controls.Add(this.ExpTable);
      //this.Name = "SelectExperimentForm";
      this.Text = "SelectExperimentForm";
      ((System.ComponentModel.ISupportInitialize)(this.ExpTable)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.testDBDataSetBindingSource)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.testDBDataSet)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView ExpTable;
    private System.Windows.Forms.BindingSource testDBDataSetBindingSource;
    private TestDBDataSet testDBDataSet;
    private System.Windows.Forms.DataGridViewTextBoxColumn id;
    private System.Windows.Forms.DataGridViewTextBoxColumn Name;
    private System.Windows.Forms.DataGridViewTextBoxColumn Target;
    private System.Windows.Forms.DataGridViewTextBoxColumn Author;
    private System.Windows.Forms.Button SelectButton;
  }
}