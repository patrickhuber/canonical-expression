namespace AdventureWorks.Logical
{
    public class Phone
    {        
        public int Id { get; set; }
        public int CountryCode {get; set; }

        public int AreaCode{get;set;}

        public int CentralOffice{get;set;}

        public int LineNumber{get;set;}

        public PhoneType PhoneType{get;set;}
    }
}