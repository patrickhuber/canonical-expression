using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Updates
{
    public class IntUpdate : UpdateBase<int?>
    {
        public IntUpdate() : base() { }
        public IntUpdate(int? value) : base(value) { }
        public IntUpdate(int? value, bool isSet) : base(value, isSet) { }
    }
}
