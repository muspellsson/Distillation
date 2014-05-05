using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace GUI
{
    public partial class Plotter : UserControl
    {
        private bool ready = false;
        private double a, b;
        private double mq, mg;
        private int w, h, ox, oy;

        public Plotter()
        {
            InitializeComponent();
        }

        private int x(int x) { return x; }
        private int y(int y)
        {
            return this.Height - y;
        }

        private int gx(int x)
        {
            return this.x(x + this.ox);
        }

        private int gy(int y)
        {
            return this.y(y + this.oy);
        }

        private int fx(double x)
        {
            return this.gx(Convert.ToInt32(Convert.ToDouble(x) / 
                this.mq * (w - 3 * ox)));
        }

        private int fy(double y)
        {
            return this.gy(Convert.ToInt32(Convert.ToDouble(y) /
                this.mg * (h - 3 * oy)));
        }

        private void Clean()
        {
            Graphics   gr = this.CreateGraphics();
            SolidBrush br = new SolidBrush(System.Drawing.Color.White);
            gr.FillRectangle(br, 0, 0, this.Width, this.Height);
            gr.Dispose();
        }

        private void calculateMaxima()
        {
            this.mg = Math.Pow(this.b, 2) / (4 * this.a);
            this.mq = this.b / (2 * this.a);
        }

        private void drawAxes()
        {
            Graphics   gr  = this.CreateGraphics();
            Pen        p   = new Pen(System.Drawing.Color.Black, 1);
            SolidBrush br  = new SolidBrush(System.Drawing.Color.Black);
            Font       fnt = new Font("Arial", 10);
            StringFormat sfmt = new StringFormat();
            Rectangle    rect = new Rectangle(x(0), y(h), x(ox), y(h - oy));
            p.EndCap       = System.Drawing.Drawing2D.LineCap.ArrowAnchor;
            sfmt.Alignment = StringAlignment.Far;
            gr.DrawLine(p, x(ox), y(oy), 
                x(w - ox), y(oy));
            gr.DrawLine(p, x(ox), y(oy),
                x(ox), y(h - oy));
            gr.DrawString("q", fnt, br,
                x(w - ox), y(oy));
            gr.DrawString("g", fnt, br,
                rect, sfmt);
            fnt.Dispose();
            br.Dispose();
            p.Dispose();
            gr.Dispose();
        }

        private void drawDashed()
        {
            Graphics gr = this.CreateGraphics();
            Pen      p  = new Pen(System.Drawing.Color.Black, 1);
            
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            gr.DrawLine(p, x(ox), y(h - 2 * oy),
                x(w - 2 * ox), y(h - 2 * oy));
            gr.DrawLine(p, x(w - 2 * ox), y(oy),
                x(w - 2 * ox), y(h - 2 * oy));
            p.Dispose();
            gr.Dispose();
        }

        private void drawText()
        {
            Graphics gr = this.CreateGraphics();
            //Pen      p  = new Pen(System.Drawing.Color.Black, 1);
            SolidBrush br = new SolidBrush(System.Drawing.Color.Black);
            Font fnt = new Font("Arial", 10);
            StringFormat sfmt = new StringFormat();
            Rectangle rect;

            rect           = new Rectangle(ox, oy, w - 2 * ox, 3 * oy);
            sfmt.Alignment = StringAlignment.Center;
            gr.DrawString(this.mg.ToString("G5", 
                CultureInfo.InvariantCulture) + " моль/с", 
                fnt, br,
                rect, sfmt);
            sfmt.FormatFlags = StringFormatFlags.DirectionVertical;
            rect             = new Rectangle(w - 2 * ox, oy, w - ox, h - oy);
            gr.DrawString(this.mq.ToString("G5",
                CultureInfo.InvariantCulture) + " Вт",
                fnt, br,
                rect, sfmt);

            fnt.Dispose();
            br.Dispose();
            gr.Dispose();
        }

        private void drawParabola()
        {
            Graphics gr = this.CreateGraphics();
            Pen p = new Pen(System.Drawing.Color.Black, 1);

            int px, py;
            int oldx, oldy;
            double g, q;

            oldx = 0;
            oldy = gy(0);

            for (int i = 0; i < w - 3 * ox; i += 5)
            {
                px = i;
                q  = Convert.ToDouble(px) / (w - 3 * ox) * mq;
                g  = b * q - a * Math.Pow(q, 2);
                py = fy(g);
                gr.DrawLine(p, gx(oldx), oldy, gx(px), py);
                oldx = px;
                oldy = py;
            }
        }

        private void draw()
        {
            this.drawDashed();
            this.drawParabola();
            this.drawText();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.w  = this.Width;
            this.h  = this.Height;
            this.ox = w / 10;
            this.oy = h / 10;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            this.Clean();
            this.drawAxes();
            if (this.ready)
            {
                this.draw();
            }
        }

        public void Plot(double a, double b)
        {
            this.ready = true;
            this.a = a;
            this.b = b;
            this.calculateMaxima();
            this.Invalidate();
        }
    }
}
