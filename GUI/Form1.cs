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
        private Approximation apx;

        public Form1()
        {
            InitializeComponent();
            apx = new Approximation();
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
                out value)) { }
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

        private void addPoint(double q, double g)
        {
            ListViewItem lvi;

            lvi = new ListViewItem(q.ToString("G5"));
            lvi.SubItems.Add(g.ToString("G5"));
            this.listView1.Items.Insert(this.apx.Find(q), lvi);
        }

        private void loadApproximation(OpenFileDialog dlg)
        {
            if (!this.apx.Load(dlg.FileName))
            {
                ErrorBox.Error("Ошибка при загрузке файла");
            }
            else
            {
                this.listView1.Items.Clear();
                foreach (Distillation.Point p in this.apx.Points)
                {
                    this.addPoint(p.Q, p.G);
                }
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
                    this.loadApproximation(dlg1);
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
                plotter1.Plot(col.Irreversibility, col.ReversibleEfficiency);
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

        private void saveApproximation(SaveFileDialog dlg)
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (!this.apx.Correct)
                {
                    ErrorBox.Error("Необходимы минимум две точки");
                }
                else
                {
                    if (!this.apx.Save(dlg.FileName))
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
                this.saveApproximation(dlg1);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.listView1.Items.Clear();
            this.apx = new Approximation();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ListView.SelectedIndexCollection idcs =
                this.listView1.SelectedIndices;
            foreach (int idx in idcs)
            {
                this.apx.RemovePoint(idx);
                this.listView1.Items.RemoveAt(idx);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PointInputDialog pd = new PointInputDialog();
            pd.ShowDialog();

            if (pd.DialogResult == DialogResult.OK)
            {
                this.apx.AddPoint(pd.Q, pd.G);
                this.addPoint(pd.Q, pd.G);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (this.apx.Calculate())
            {
                System.Console.WriteLine("{0} {1}", this.apx.Irreversibility, this.apx.ReversibleEfficiency);
                this.plotter2.Plot(this.apx.Irreversibility,
                    this.apx.ReversibleEfficiency);
                this.tbA2.Text = this.apx.Irreversibility
                    .ToString("G5", CultureInfo.InvariantCulture);
                this.tbB2.Text = this.apx.ReversibleEfficiency
                    .ToString("G5", CultureInfo.InvariantCulture);
            }
            else
            {
                ErrorBox.Error("Произошла ошибка при вычислениях");
            }
        }
    }
}
