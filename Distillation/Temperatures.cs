using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distillation
{
    public class Temperatures : Errorneous
    {
        private double refluxDrum;
        private double reboiler;
        private double cooler;
        private double heater;

        public double RefluxDrum
        {
            get { return this.refluxDrum; }
        }

        public double Reboiler
        {
            get { return this.reboiler; }
        }

        public double Cooler
        {
            get { return this.cooler; }
        }

        public double Heater
        {
            get { return this.heater; }
        }

        private void checkValue(double value, string error)
        {
            if (value <= 0.0)
            {
                this.addError(error);
            }
        }

        public Temperatures(double refluxDrum, double reboiler,
            double cooler, double heater)
        {
            this.prepareForErrors();
            this.checkValue(refluxDrum, "Температура в дефлегматоре" +
                    " отрицательна");
            this.checkValue(reboiler, "Температура в кубе" +
                    " отрицательна");
            this.checkValue(cooler, "Температура холодильника" +
                    " отрицательна");
            this.checkValue(heater, "Температура нагревателя" +
                    " отрицательна");
            this.refluxDrum = refluxDrum;
            this.reboiler = reboiler;
            this.cooler = cooler;
            this.heater = heater;
        }
    }
}
