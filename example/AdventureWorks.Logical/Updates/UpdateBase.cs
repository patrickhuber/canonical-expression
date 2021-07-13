using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Updates
{
    public abstract class UpdateBase<T>
    {
        public T Value { get; set; }

        /// <summary>        
        /// IsSet = true, Value = null : set the value to null
        /// IsSet = true, Value != null : set the value to the value
        /// IsSet = false : do nothing        
        /// </summary>
        public bool IsSet { get; set; }

        public UpdateBase() : this(default, false){ }

        public UpdateBase(T value) : this(value, true)
        {
            Value = value;
        }

        public UpdateBase(T value, bool isSet) 
        {
            Value = value;
            IsSet = isSet;
        }
    }
}
