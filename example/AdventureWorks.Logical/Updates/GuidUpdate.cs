using System;

namespace AdventureWorks.Logical.Updates
{
    public class GuidUpdate : UpdateBase<Guid?>
    {
        public GuidUpdate()
        {
        }

        public GuidUpdate(Guid? value) : base(value)
        {
        }

        public GuidUpdate(Guid? value, bool isSet) : base(value, isSet)
        {
        }
    }
}
