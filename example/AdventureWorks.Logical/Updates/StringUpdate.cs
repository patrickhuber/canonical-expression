using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Updates
{    
    public class StringUpdate
        : UpdateBase<string>
    {
        public StringUpdate() : base() { }
        public StringUpdate(string value) : base(value) { }
        public StringUpdate(string value, bool isSet) : base(value, isSet) { }

        public static implicit operator StringUpdate(string value)
        {
            return new StringUpdate(value);
        }
    }
}
