using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GUI
{
    public partial class Plotter : UserControl
    {
        private bool ready = false;
        private double a, b;

        public Plotter()
        {
            InitializeComponent();
        }

        private int x(int x) { return x; }
        private int y(int y)
        {
            return this.Height - y;
        }

        private void Clean()
        {
            Graphics gr   = this.CreateGraphics();
            SolidBrush br = new SolidBrush(System.Drawing.Color.White);
            gr.FillRectangle(br, 0, 0, this.Width, this.Height);
            gr.Dispose();
        }

        private void drawAxes()
        {
            int w  = this.Width;
            int h  = this.Height;
            int ox = w / 10;
            int oy = h / 10;

            Graphics gr   = this.CreateGraphics();
            Pen p         = new Pen(System.Drawing.Color.Black, 1);
            SolidBrush br = new SolidBrush(System.Drawing.Color.Black);
            Font fnt      = new Font("Arial", 10);
            StringFormat sfmt = new StringFormat();
            Rectangle    rect;
            p.EndCap = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            gr.DrawLine(p, x(ox), y(oy), 
                x(w - ox), y(oy));
            gr.DrawLine(p, x(ox), y(oy),
                x(ox), y(h - oy));
            gr.DrawString("q", fnt, br,
                x(w - ox), y(oy));
            sfmt.Alignment = StringAlignment.Far;
            rect = new Rectangle(x(0), y(h), x(ox), y(h - oy));
            gr.DrawString("g", fnt, br,
                rect, sfmt);
            fnt.Dispose();
            br.Dispose();
            p.Dispose();
            gr.Dispose();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            int w = this.Width;
            int h = this.Height;
            int ox = w / 10;
            int oy = h / 10;
            Graphics gr;
            Pen p;
            Brush br;

            base.OnPaint(e);

            this.Clean();
            this.drawAxes();
        }

        public void Plot(double a, double b)
        {
            this.ready = true;
            this.a = a;
            this.b = b;
            this.Invalidate();
        }
    }
}
