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
    public class Column : Errorneous
    {
        private const double eps = 1e-15;
        private const double R   = 8.31;

        private Concentrations concentrations;
        private Temperatures   temperatures;
        private Coefficients   coefficients;

        private double reversibleEfficiency = 0.0;
        private double irreversibility = 0.0;


        public Concentrations Concentrations
        {
            get { return this.concentrations; }
        }

        public Temperatures Temperatures
        {
            get { return this.temperatures; }
        }

        public Coefficients Coefficients
        {
            get { return this.coefficients; }
        }

        public double ReversibleEfficiency
        {
            get { return this.reversibleEfficiency; }
        }

        public double Irreversibility
        {
            get { return this.irreversibility; }
        }

        private void checkValue(Errorneous value)
        {
            if (!value.Correct)
            {
                this.addErrors(value.Errors);
            }
        }

        public Column(Concentrations c, Temperatures t, Coefficients cf)
        {
            this.prepareForErrors();
            this.checkValue(c);
            this.checkValue(t);
            this.checkValue(cf);
            this.concentrations = c;
            this.temperatures   = t;
            this.coefficients   = cf;
            if (this.Correct)
            {
                this.calculate();
            }
        }

        private Maybe<double> calculateShare()
        {
            double e;
            if (Math.Abs(this.Concentrations.RefluxDrum -
                    this.Concentrations.Reboiler) < eps)
            {
                this.addError("xD = xB");
                return Maybe<double>.Nothing;
            }
            else
            {
                e = (this.Concentrations.Feed -
                    this.Concentrations.Reboiler) /
                    (this.Concentrations.RefluxDrum -
                    this.Concentrations.Reboiler);
                return new Maybe<double>(e);
            }
        }

        private double partialGibbs(double c)
        {
            return - R * this.Temperatures.RefluxDrum *
                (c * Math.Log(c) + (1 - c) * Math.Log(1 - c));
        }

        private Maybe<double> calculateGibbs(double e)
        {
            double AF, AD, AB, AG;
            AF = this.partialGibbs(this.Concentrations.Feed);
            System.Console.WriteLine(this.Concentrations.Reboiler);
            AD = this.partialGibbs(this.Concentrations.RefluxDrum);
            AB = this.partialGibbs(this.Concentrations.Reboiler);
            AG = (AF - e * AD - (1 - e) * AB);
            if (Math.Abs(AG) < eps)
            {
                this.addError("Энергия разделения равна нулю");
                return Maybe<double>.Nothing;
            }
            else
            {
                return new Maybe<double>(AG);
            }
        }

        private double calculateReversibleEfficiency(double AG)
        {
            return (this.Temperatures.Reboiler -
                this.Temperatures.RefluxDrum) / 
                (this.Temperatures.Reboiler * AG);
        }

        private double calculateIrreversibility(double AG)
        {
            return (1 / (this.Coefficients.HeatReboiler * 
                this.Temperatures.Reboiler * 
                this.Temperatures.Heater) +
                1 / (this.Coefficients.HeatRefluxDrum *
                this.Temperatures.RefluxDrum *
                this.Temperatures.Cooler) +
                2 * (this.Concentrations.RefluxDrum - 
                this.Concentrations.Reboiler) / 
                (this.Coefficients.Mass * 
                Math.Pow(this.Coefficients.EvaporationHeat, 2))) * 
                this.Temperatures.RefluxDrum / AG;
        }

        private void calculate()
        {
            Maybe<double> val;

            this.prepareForErrors();
            val = this.calculateShare();
            if (!val.None)
            {
                val = this.calculateGibbs(val.Value);
                if (!val.None)
                {
                    this.reversibleEfficiency =
                        this.calculateReversibleEfficiency(val.Value);
                    this.irreversibility =
                        this.calculateIrreversibility(val.Value);
                }
            }
        }

        private XDocument generateXml()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement("column",
                    new XElement("xF", this.Concentrations.Feed),
                    new XElement("xD", this.Concentrations.RefluxDrum),
                    new XElement("xB", this.Concentrations.Reboiler),
                    new XElement("TD", this.Temperatures.RefluxDrum),
                    new XElement("TB", this.Temperatures.Reboiler),
                    new XElement("Tm", this.Temperatures.Cooler),
                    new XElement("Tp", this.Temperatures.Cooler),
                    new XElement("bD", this.Coefficients.HeatRefluxDrum),
                    new XElement("bB", this.Coefficients.HeatReboiler),
                    new XElement("k", this.Coefficients.Mass),
                    new XElement("r", this.Coefficients.EvaporationHeat)));
        }

        public bool Save(string fileName)
        {
            XDocument doc;

            this.prepareForErrors();

            if (!this.Correct)
            {
                return false;
            }
            else
            {
                doc = this.generateXml();

                try
                {
                    doc.Save(fileName);
                    return true;
                }
                catch (Exception e)
                {
                    this.addError(e.Message);
                    return false;
                }
            }
        }

        static public Maybe<Column> Load(string fileName)
        {
            XDocument    doc;
            XmlSchemaSet scm = new XmlSchemaSet();
            XElement     colElem;

            try
            {
                scm.Add(null, "Column.xsd");
                doc = XDocument.Load(fileName);
                doc.Validate(scm, (o, e) => { });

                colElem = doc.Element("column");
                return new Maybe<Column>(new Column(
                    new Concentrations(
                        Double.Parse(colElem.Element("xF").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("xD").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("xB").Value,
                            CultureInfo.InvariantCulture)),
                    new Temperatures(
                        Double.Parse(colElem.Element("TD").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("TB").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("Tm").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("Tp").Value,
                            CultureInfo.InvariantCulture)),
                    new Coefficients(
                        Double.Parse(colElem.Element("bD").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("bB").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("k").Value,
                            CultureInfo.InvariantCulture),
                        Double.Parse(colElem.Element("r").Value,
                            CultureInfo.InvariantCulture))));
            }
            catch (Exception)
            {
                return Maybe<Column>.Nothing;
            }
        }
    }
}
