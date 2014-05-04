using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;
using Distillation;

namespace GUI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void validateForDouble(object sender, KeyPressEventArgs e)
        {

            string  s;
            double  value;
            TextBox tb = (TextBox)sender;

            s = tb.Text + e.KeyChar.ToString();

            if (Double.TryParse(s, 
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value))
            {

            }
            else if (e.KeyChar == '\b') { }
            else { e.Handled = true; }
        }

        private void fillTextBoxes(Column col)
        {
            this.tbXF.Text = col.Concentrations.Feed
                        .ToString("G5", CultureInfo.InvariantCulture);
            this.tbXD.Text = col.Concentrations.RefluxDrum
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbXB.Text = col.Concentrations.Reboiler
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbTD.Text = col.Temperatures.RefluxDrum
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbTB.Text = col.Temperatures.Reboiler
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbTm.Text = col.Temperatures.Cooler
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbTp.Text = col.Temperatures.Heater
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbBD.Text = col.Coefficients.HeatRefluxDrum
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbBB.Text = col.Coefficients.HeatReboiler
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbK.Text = col.Coefficients.Mass
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbR.Text = col.Coefficients.EvaporationHeat
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbB1.Text = col.ReversibleEfficiency
                .ToString("G5", CultureInfo.InvariantCulture);
            this.tbA1.Text = col.Irreversibility
                .ToString("G5", CultureInfo.InvariantCulture);
        }

        private Column storeTextBoxes()
        {
            return new Column(
                new Concentrations(
                    Double.Parse(this.tbXF.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbXD.Text,
                         CultureInfo.InvariantCulture),
                    Double.Parse(this.tbXB.Text,
                        CultureInfo.InvariantCulture)),
                new Temperatures(
                    Double.Parse(this.tbTD.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbTB.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbTm.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbTp.Text,
                        CultureInfo.InvariantCulture)),
                new Coefficients(
                    Double.Parse(this.tbBD.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbBB.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbK.Text,
                        CultureInfo.InvariantCulture),
                    Double.Parse(this.tbR.Text,
                        CultureInfo.InvariantCulture)));
        }

        private void loadColumn(OpenFileDialog dlg)
        {
            Maybe<Column> val;

            val = Column.Load(dlg.FileName);
            if (val.None)
            {
                ErrorBox.Error("Файл ошибочен");
            }
            else if (!val.Value.Correct)
            {
                ErrorBox.Errors(val.Value.Errors);
            }
            else
            {
                this.fillTextBoxes(val.Value);
            }
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg1 = new OpenFileDialog();

            dlg1.InitialDirectory = Environment.CurrentDirectory;
            dlg1.Filter = "XML-документы (*.xml)|*.xml";
            dlg1.RestoreDirectory = true;

            if (dlg1.ShowDialog() == DialogResult.OK)
            {
                if (this.tabControl1.SelectedIndex == 0)
                {
                    this.loadColumn(dlg1);
                }
                else if (this.tabControl1.SelectedIndex == 1)
                {
                }
            }
        }

        private void bCalc_Click(object sender, EventArgs e)
        {
            Column col = this.storeTextBoxes();
            if (!col.Correct)
            {
                ErrorBox.Errors(col.Errors);
            }
            else
            {
                this.fillTextBoxes(col);
            }
        }

        private void saveColumn(SaveFileDialog dlg)
        {
            Column col;

            col = this.storeTextBoxes();
            if (!col.Correct)
            {
                ErrorBox.Errors(col.Errors);
            }
            else
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    if (!col.Save(dlg.FileName))
                    {
                        ErrorBox.Error("Ошибка при сохранении файла");
                    }
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg1 = new SaveFileDialog();

            dlg1.InitialDirectory = Environment.CurrentDirectory;
            dlg1.Filter = "XML-документы (*.xml)|*.xml";
            dlg1.RestoreDirectory = true;

            if (tabControl1.SelectedIndex == 0)
            {
                this.saveColumn(dlg1);
            }
            else if (tabControl1.SelectedIndex == 1)
            {
            }
        }
    }
}
