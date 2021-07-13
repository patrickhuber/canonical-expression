﻿namespace AdventureWorks.Cosmos
{
    public class CustomerAddress
    {
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string zipCode { get; set; }
        public Location location { get; set; }
    }
}
