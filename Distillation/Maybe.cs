using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Distillation
{
    public class Maybe<T>
    {
        private T value = default(T);
        private bool none = true;

        static public Maybe<T> Nothing
        {
            get 
            {
                Maybe<T> m = new Maybe<T>();
                return m;
            }
        }
        
        public bool None
        {
            get { return this.none; }
        }

        public T Value
        {
            get { return this.value; }
        }

        private Maybe() {  }

        public Maybe(T value)
        {
            this.none  = false;
            this.value = value;
        }
    }
}
