using AdventureWorks.Logical.PersonRead;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Json.Tests
{
    [TestClass]
    public class PersonReadPredicateConverterTests
    {
        [TestMethod]
        public async Task CanSerializePersonReadStringPredicate()
        {
            var personReadRequest = new PersonReadRequest(
                new PersonReadAnyOf(
                    new PersonReadAllOf(
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.FirstName,
                            StringOperator.Equal,
                            "Hello"))));

            var expected = @"
            {
	            ""where"": [{
                    ""anyOf"": [{
                        ""allOf"": [{
                            ""property"" : ""FirstName"",
					        ""operator"" : ""Equal"",
					        ""value"": ""Hello""
                        }]
		            }]
	            }]
            }";
            await CanSerializePersonReadRequestAsync(personReadRequest, expected);
        }

        [TestMethod]
        public async Task CanSerializePersonReadGuidPredicate()
        {
            var guid = Guid.NewGuid();
            var personReadRequest = new PersonReadRequest(
                new PersonReadAnyOf(
                    new PersonReadAllOf(
                        new PersonReadGuidPredicate(
                            PersonReadGuidProperty.Id,
                            GuidOperator.Equal,
                            guid))));

            var expected = @"
            {
	            ""where"": [{
                    ""anyOf"": [{
                        ""allOf"": [{
                            ""property"" : ""Id"",
					        ""operator"" : ""Equal"",
					        ""value"": ""GUID_VALUE""
                        }]
		            }]
	            }]
            }".Replace("GUID_VALUE", guid.ToString());
            await CanSerializePersonReadRequestAsync(personReadRequest, expected);
        }

        private async Task CanSerializePersonReadRequestAsync(PersonReadRequest personReadRequest, string expected)
        {
            var memoryStream = new MemoryStream();

            var options = new JsonSerializerOptions();
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new PersonReadPredicateConverter());
            options.WriteIndented = true;

            await JsonSerializer.SerializeAsync(
                memoryStream,
                personReadRequest,
                options);
            memoryStream.Position = 0;
            var actualToken = JToken.Parse(await new StreamReader(memoryStream).ReadToEndAsync());
            var expectedToken = JToken.Parse(expected);
            Assert.IsTrue(JToken.DeepEquals(expectedToken, actualToken), "Expected {0} Found {1}", expectedToken, actualToken);
        }

        [TestMethod]
        public async Task CanDeserializePersonReadStringPredicate()
        {
            var input = @"
            {
	            ""where"": [{
                    ""anyOf"": [{
                        ""allOf"": [{
                            ""property"" : ""FirstName"",
					        ""operator"" : ""Equal"",
					        ""value"": ""Hello""
                        }]
		            }]
	            }]
            }";
            var expected = new PersonReadRequest(
                new PersonReadAnyOf(
                    new PersonReadAllOf(
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.FirstName,
                            StringOperator.Equal,
                            "Hello"))));

            await CanDeserializePersonReadRequestAsync(input, expected);
        }

        [TestMethod]
        public async Task CanDeserializePersonReadGuidPredicate()
        {
            var guid = Guid.NewGuid();

            var input = @"
            {
	            ""where"": [{
                    ""anyOf"": [{
                        ""allOf"": [{
                            ""property"" : ""Id"",
					        ""operator"" : ""Equal"",
					        ""value"": ""GUID_VALUE""
                        }]
		            }]
	            }]
            }".Replace("GUID_VALUE", guid.ToString());

            var expected = new PersonReadRequest(
                new PersonReadAnyOf(
                    new PersonReadAllOf(
                        new PersonReadGuidPredicate(
                            PersonReadGuidProperty.Id,
                            GuidOperator.Equal,
                            guid))));

            await CanDeserializePersonReadRequestAsync(input, expected);
        }

        private async Task CanDeserializePersonReadRequestAsync(string input, PersonReadRequest expected)
        {
            var memoryStream = new MemoryStream();
            var writer = new StreamWriter(memoryStream);
            await writer.WriteAsync(input);
            await writer.FlushAsync();         
            
            memoryStream.Seek(0, SeekOrigin.Begin);

            var options = new JsonSerializerOptions();
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.Converters.Add(new PersonReadPredicateConverter());
            options.WriteIndented = true;

            var actual = await JsonSerializer.DeserializeAsync<PersonReadRequest>(memoryStream, options);
            Assert.IsTrue(Equal(expected, actual));
        }

        private bool Equal(PersonReadRequest first, PersonReadRequest second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            if (!Equal(first.Where, second.Where))
                return false;

            return true;
        }

        private bool Equal(PersonReadAnyOf[] first, PersonReadAnyOf[] second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            if (first.Length != second.Length)
                return false;
            for (var i = 0; i < first.Length; i++)
            {
                var firstAnyOf = first[i];
                var secondAnyOf = second[i];
                if (!Equal(firstAnyOf, secondAnyOf))
                    return false;
            }
            return true;
        }

        private bool Equal(PersonReadAnyOf first, PersonReadAnyOf second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            if (!Equal(first.AnyOf, second.AnyOf))
                return false;

            return true;
        }

        private bool Equal(PersonReadAllOf[] first, PersonReadAllOf[] second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            if (first.Length != second.Length)
                return false;
            for (var i = 0; i < first.Length; i++)
            {
                var firstAllOf = first[i];
                var secondAllOf = second[i];
                if (!Equal(firstAllOf, secondAllOf))
                    return false;
            }
            return true;
        }

        private bool Equal(PersonReadAllOf first, PersonReadAllOf second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            if (!Equal(first.AllOf, second.AllOf))
                return false;

            return true;
        }

        private bool Equal(PersonReadPredicate[] first, PersonReadPredicate[] second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            if (first.Length != second.Length)
                return false;
            for (var i = 0; i < first.Length; i++)
            {
                var firstPredicate = first[i];
                var secondPredicate = second[i];
                if (!Equal(firstPredicate, secondPredicate))
                    return false;
            }
            return true;
        }

        private bool Equal(PersonReadPredicate first, PersonReadPredicate second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;

            if (!first.GetType().Equals(second.GetType()))
                return false;

            if (first is PersonReadStringPredicate firstStringPredicate)
                return Equal(firstStringPredicate, second as PersonReadStringPredicate);
            return Equal(first as PersonReadGuidPredicate, second as PersonReadGuidPredicate);
        }

        private bool Equal(PersonReadStringPredicate first, PersonReadStringPredicate second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            return first.Operator == second.Operator
                && first.Property == second.Property
                && first.Value == second.Value;
        }

        private bool Equal(PersonReadGuidPredicate first, PersonReadGuidPredicate second)
        {
            if (first is null && second is not null)
                return false;
            if (first is not null && second is null)
                return false;
            if (first is null && second is null)
                return true;
            return first.Operator == second.Operator
                && first.Property == second.Property
                && first.Value == second.Value;
        }
    }
}
