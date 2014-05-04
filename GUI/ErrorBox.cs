using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI
{
    public partial class ErrorBox : Form
    {
        private List<string> errors = null;

        public ErrorBox()
        {
            InitializeComponent();
            this.errors = new List<string>();
        }

        private void addError(string error)
        {
            this.textBox1.Text += "Ошибка 1:" +
                    error + Environment.NewLine + Environment.NewLine;
        }

        private void addErrors(List<string> errors)
        {
            int errCount = 0;

            foreach (string error in errors)
            {
                errCount++;
                this.textBox1.Text += "Ошибка " + errCount.ToString() + ": " +
                    error + Environment.NewLine + Environment.NewLine;
            }
        }

        static public void Error(string error)
        {
            ErrorBox erb = new ErrorBox();
            erb.addError(error);
            erb.ShowDialog();
        }

        static public void Errors(List<string> errors)
        {
            ErrorBox erb = new ErrorBox();
            erb.addErrors(errors);
            erb.ShowDialog();
        }
    }
}
