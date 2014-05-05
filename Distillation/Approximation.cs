using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Globalization;

namespace Distillation
{
    public struct Point 
    {
        private double q, g;

        public double Q
        {
            get { return this.q; }
        }

        public double G
        {
            get { return this.g; }
        }

        public Point(double q, double g)
        {
            this.q = q;
            this.g = g;
        }
    }

    public class Approximation
    {
        private const double eps = 1e-15;

        private List<Point> points;
        private double reversibleEfficiency = 0.0;
        private double irreversibility = 0.0;

        public double ReversibleEfficiency
        {
            get { return this.reversibleEfficiency; }
        }

        public double Irreversibility
        {
            get { return this.irreversibility; }
        }

        public bool Correct
        {
            get { return this.points.Count >= 2; }
        }

        public List<Point> Points
        {
            get { return this.points; }
        }

        private XDocument generateXml()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("approximation",
                    this.points.Select((s, idx) =>
                        new XElement("QG",
                            new XElement("Q", s.Q),
                            new XElement("G", s.G)))));
        }

        public Approximation()
        {
            this.points = new List<Point>();
        }

        public int Find(double q)
        {
            return this.points.Where(p => p.Q < q).Count();
        }

        public bool AddPoint(double q, double g)
        {
            if ((q >= 0.0) && (g >= 0.0))
            {
                this.points.Insert(this.Find(q), new Point(q, g));
                return true;
            }
            else
            {
                return false;
            }
        }

        public void RemovePoint(int idx)
        {
            this.points.RemoveAt(idx);
        }

        public bool solveLES()
        {
            double q1 = this.points[0].Q;
            double q2 = this.points[1].Q;
            double g1 = this.points[0].G;
            double g2 = this.points[1].G;
            double num, den;
            num = (g2 - g1 * q2 / q1);
            den = (q1 * q2 - Math.Pow(q2, 2));

            if (Math.Abs(den) < eps)
            {
                return false;
            }
            else
            {
                this.irreversibility = num / den;
                this.reversibleEfficiency = g1 / q1 +
                    this.irreversibility * q1;
                return ((this.irreversibility > 0.0) &&
                        (this.reversibleEfficiency > 0.0));
            }
        }

        private bool leastSquares()
        {
            int len = this.points.Count;
            double sumgq  = 0.0; 
            double sumgq2 = 0.0; 
            double sumq2  = 0.0;
            double sumq3  = 0.0;
            double sumq4  = 0.0;
            double left, right;

            foreach (Point p in this.points)
            {
                double q2 = Math.Pow(p.Q, 2);
                sumgq     += p.G * p.Q;
                sumgq2    += p.G * q2;
                sumq2     += q2;
                sumq3     += q2 * p.Q;
                sumq4     += q2 * q2;
            }

            if (Math.Abs(sumq2) < eps)
            {
                return false;
            }
            else
            {
                left = sumgq2 - (sumgq * sumq3) / sumq2;
                right = sumq3 * sumq3 / sumq2 - sumq4;
                if (Math.Abs(left) < eps)
                {
                    return false;
                }
                else
                {
                    this.irreversibility = right / left;
                    this.reversibleEfficiency = (sumgq + 
                        this.irreversibility * sumq3) / sumq2;
                    return ((this.irreversibility > 0.0) &&
                        (this.reversibleEfficiency > 0.0));
                }
            }
        }

        public bool Calculate()
        {
            if (!this.Correct)
            {
                return false;
            }
            else if (this.points.Count == 2)
            {
                return this.solveLES();
            }
            else
            {
                return this.leastSquares();
            }
        }

        public bool Save(string fileName)
        {
            XDocument doc;

            doc = this.generateXml();

            try
            {
                doc.Save(fileName);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Load(string fileName)
        {
            XDocument    doc;
            XmlSchemaSet scm = new XmlSchemaSet();
            XElement     apxElem;
            List<XElement> qgs;
            List<Point>  opoints = this.points;

            this.points = new List<Point>();

            try
            {
                scm.Add(null, "Approximation.xsd");
                doc = XDocument.Load(fileName);
                doc.Validate(scm, (o, e) => { });

                apxElem = doc.Element("approximation");
                qgs     = apxElem.Elements().ToList();
                foreach (XElement p in qgs)
                {
                    if (!this.AddPoint(Double.Parse(p.Element("Q").Value,
                        CultureInfo.InvariantCulture),
                        Double.Parse(p.Element("G").Value,
                        CultureInfo.InvariantCulture)))
                    {
                        this.points = opoints;
                        return false;
                    }
                }
                if (this.Correct)
                {
                    return true;
                }
                else
                {
                    this.points = opoints;
                    return false;
                }
            }
            catch (Exception)
            {
                this.points = opoints;
                return false;
            }
        }
    }
}
