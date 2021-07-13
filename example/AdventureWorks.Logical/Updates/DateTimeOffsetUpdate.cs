using System;

namespace AdventureWorks.Logical.Updates
{
    public class DateTimeOffsetUpdate : UpdateBase<DateTimeOffset?>
    {
        public DateTimeOffsetUpdate() : base() { }
        public DateTimeOffsetUpdate(DateTimeOffset? value) : base(value) { }
        public DateTimeOffsetUpdate(DateTimeOffset? value, bool isSet) : base(value, isSet) { }
    }
}