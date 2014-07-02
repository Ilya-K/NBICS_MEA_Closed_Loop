using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MEAClosedLoop
{
  public partial class FCreateExpOpt : Form
  {
    public bool IsValidated = false;
    public FCreateExpOpt()
    {
      InitializeComponent();
    }

    private void Create_Click(object sender, EventArgs e)
    {
      if (ValidateInput())
      {
        IsValidated = true;
        this.Close();
      }
      else
      {
        DialogResult result = MessageBox.Show("Вы не ввели полные данные", "Ошибка", MessageBoxButtons.OKCancel);
        switch (result)
        {
          case System.Windows.Forms.DialogResult.OK:
            
            break;
          case System.Windows.Forms.DialogResult.Cancel:
            this.IsValidated = false;
            this.Close();
            break;
        }
      }
    }
    private bool ValidateInput()
    {
      if (AuthorName.Text.Length > 2 && ExpName.Text.Length > 2 && ExpTarget.Text.Length > 2)
        return true;
      return false;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      this.IsValidated = false;
      this.Close();
    }
  }
}
