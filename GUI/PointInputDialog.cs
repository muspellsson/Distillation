using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace GUI
{
    public partial class PointInputDialog : Form
    {
        public PointInputDialog()
        {
            InitializeComponent();
        }

        public double Q
        {
            get
            {
                return Double.Parse(this.textBox1.Text,
                    CultureInfo.InvariantCulture);
            }
        }

        public double G
        {
            get
            {
                return Double.Parse(this.textBox2.Text,
                    CultureInfo.InvariantCulture);
            }
        }

        private void validateForDouble(object sender, KeyPressEventArgs e)
        {

            string s;
            double value;
            TextBox tb = (TextBox)sender;

            s = tb.Text + e.KeyChar.ToString();

            if (e.KeyChar == '-') { e.Handled = true; }
            else if (e.KeyChar == ' ') { e.Handled = true; }
            else if (Double.TryParse(s,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value)) { }
            else if (e.KeyChar == '\b') { }
            else { e.Handled = true; }
        }
    }
}
