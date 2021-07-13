using AdventureWorks.Logical.PersonRead;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AdventureWorks.Logical.Json
{
    public class PersonReadPredicateConverter : JsonConverter<PersonReadPredicate>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(PersonReadPredicate).IsAssignableFrom(typeToConvert);

        public override PersonReadPredicate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected token type to be JsonTokenType.StartObject");
            
            var doc = JsonDocument.ParseValue(ref reader);
            
            var element = doc.RootElement;
            var prop = element.GetProperty("property");
            
            if (Enum.TryParse(prop.GetString(), out PersonReadGuidProperty personReadGuidProperty))
                return new PersonReadGuidPredicate 
                {
                    Operator = Enum.Parse<GuidOperator>(element.GetProperty("operator").GetString()),
                    Property = personReadGuidProperty,
                    Value = new Guid(element.GetProperty("value").GetString()),
                };

            if (Enum.TryParse(prop.GetString(), out PersonReadStringProperty personReadStringProperty))
                return new PersonReadStringPredicate
                {
                    Operator = Enum.Parse<StringOperator>(element.GetProperty("operator").GetString()),
                    Property = personReadStringProperty,
                    Value = element.GetProperty("value").GetString(),
                };

            throw new InvalidOperationException($"Unable to read PersonReadPredicate, unrecognized property {prop.GetString()}");
        }

        public override void Write(Utf8JsonWriter writer, PersonReadPredicate value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value is PersonReadGuidPredicate guidPredicate)
                Write(writer, guidPredicate, options);
            
            else if (value is PersonReadStringPredicate stringPredicate)
                Write(writer, stringPredicate, options);

            writer.WriteEndObject();
        }

        public void Write(Utf8JsonWriter writer, PersonReadGuidPredicate predicate, JsonSerializerOptions options)
        {
            writer.WriteString("property", Enum.GetName(predicate.Property));
            writer.WriteString("operator", Enum.GetName(predicate.Operator));
            writer.WriteString("value", predicate.Value);
        }

        public void Write(Utf8JsonWriter writer, PersonReadStringPredicate predicate, JsonSerializerOptions options)
        {
            writer.WriteString("property", Enum.GetName(predicate.Property));
            writer.WriteString("operator", Enum.GetName(predicate.Operator));
            writer.WriteString("value", predicate.Value);
        }
    }
}
