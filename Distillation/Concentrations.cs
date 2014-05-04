using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distillation
{
    public class Concentrations : Errorneous
    {
        private double feed;
        private double refluxDrum;
        private double reboiler;

        public double Feed
        {
            get { return this.feed; }
        }

        public double RefluxDrum
        {
            get { return this.refluxDrum; }
        }

        public double Reboiler
        {
            get { return this.reboiler; }
        }

        private void checkValue(double value, string error)
        {
            if ((value <= 0.0) || (value >= 1.0))
            {
                this.addError(error);
            }
        }

        public Concentrations(double feed, double refluxDrum, double reboiler)
        {
            this.prepareForErrors();
            this.checkValue(feed, "Концентрация отделяемого компонента" +
                    " в потоке питания не в интервале (0, 1)");
            this.checkValue(refluxDrum, "Концентрация отделяемого компонента" +
                    " в дефлегматоре не в интервале (0, 1)");
            this.checkValue(reboiler, "Концентрация отделяемого компонента" +
                    " в кубе не в интервале (0, 1)");
            this.feed = feed;
            this.refluxDrum = refluxDrum;
            this.reboiler = reboiler;
        }
    }
}
