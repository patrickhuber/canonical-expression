using System;

namespace AdventureWorks.Logical.PersonRead
{ 
    public class PersonReadRequest
    {
        public PersonReadAnyOf[] Where{get;set;}
        public PersonReadExpand Expand{get;set;}
        public PersonReadProjection Select { get; set; }
        public PersonReadRequest() { }
        public PersonReadRequest(params PersonReadAnyOf[] where)=> Where = where;
        public PersonReadRequest(PersonReadExpand expand, params PersonReadAnyOf[] where) 
        { 
            Where = where; 
            Expand = expand; 
        }
    }

    public class PersonReadAnyOf
    {
        public PersonReadAllOf[] AnyOf{ get; set; }
        public PersonReadAnyOf() { }
        public PersonReadAnyOf(params PersonReadAllOf[] anyOf)=> AnyOf = anyOf;
    }

    public class PersonReadAllOf
    {
        public PersonReadPredicate[] AllOf{get;set;}
        public PersonReadAllOf() { }
        public PersonReadAllOf(params PersonReadPredicate[] allOf)=> AllOf = allOf;
    }

    public abstract class PersonReadPredicate
    {
    }

    public class PersonReadGuidPredicate : PersonReadPredicate
    {
        public PersonReadGuidProperty Property {get;set;}
        public GuidOperator Operator { get; set; }
        public Guid Value { get; set; }

        public PersonReadGuidPredicate() { }
        public PersonReadGuidPredicate(PersonReadGuidProperty property, GuidOperator @operator, Guid value)
        {
            Property = property;
            Operator = @operator;
            Value = value;
        }
    }

    public enum GuidOperator
    {
        Equal
    }

    public enum StringOperator
    {
        Equal
    }

    public class PersonReadStringPredicate : PersonReadPredicate
    {
        public PersonReadStringPredicate(PersonReadStringProperty property, StringOperator @operator, string value)
        {
            Property = property;
            Operator = @operator;
            Value = value;
        }

        public PersonReadStringPredicate() { }

        public PersonReadStringProperty Property { get; set; }
        public StringOperator Operator { get; set; }
        public string Value { get; set; }        
    }

    public enum PersonReadGuidProperty
    { 
        Id,
    }

    public enum PersonReadStringProperty 
    {
        FirstName,
        LastName,
    }

    public class PersonReadExpand
    {
        public PersonReadEmailsExpand Emails{get;set;}
        public PersonReadPhonesExpand Phones{get;set;}
    }

    public class PersonReadEmailsExpand{}

    public class PersonReadPhonesExpand{}

    public class PersonReadProjection 
    {
        /// <summary>
        /// This is the default
        /// </summary>
        public bool All { get; set; }

        /// <summary>
        /// Returns the total count
        /// </summary>
        public bool TotalCount { get; set; }
    }
}