using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distillation
{
    public class Coefficients : Errorneous
    {
        private double heatRefluxDrum;
        private double heatReboiler;
        private double mass;
        private double evaporationHeat;

        public double HeatRefluxDrum
        {
            get { return this.heatRefluxDrum; }
        }

        public double HeatReboiler
        {
            get { return this.heatReboiler; }
        }

        public double Mass
        {
            get { return this.mass; }
        }

        public double EvaporationHeat
        {
            get { return this.evaporationHeat; }
        }

        private void checkValue(double value, string error)
        {
            if (value <= 0.0)
            {
                this.addError(error);
            }
        }

        public Coefficients(double heatRefluxDrum, double heatReboiler,
            double mass, double evaporationHeat)
        {
            this.prepareForErrors();
            this.checkValue(heatRefluxDrum, "Коэффициент теплопереноса" +
                    " в дефлегматоре отрицателен");
            this.checkValue(heatReboiler, "Коэффициент теплопереноса" +
                    " в кубе отрицателен");
            this.checkValue(mass, "Коэффициент массопереноса" +
                    " отрицателен");
            this.checkValue(evaporationHeat, "Мольная теплота парообразования" +
                    " отрицательна");
            this.heatRefluxDrum  = heatRefluxDrum;
            this.heatReboiler    = heatReboiler;
            this.mass            = mass;
            this.evaporationHeat = evaporationHeat;
        }
    }
}
