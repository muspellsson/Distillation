using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distillation
{
    public class Errorneous
    {
        protected List<string> errors;
        protected bool correct;

        public bool Correct
        {
            get { return this.correct; }
        }

        public List<string> Errors
        {
            get { return this.errors; }
        }

        protected void prepareForErrors()
        {
            this.errors  = new List<string>();
            this.correct = true;
        }

        protected void addError(string error)
        {
            this.correct = false;
            this.errors.Add(error);
        }

        protected void addErrors(List<string> errors)
        {
            this.correct = false;
            this.errors.AddRange(errors);
        }
    }
}
