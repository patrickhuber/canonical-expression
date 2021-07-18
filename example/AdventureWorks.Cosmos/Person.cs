using Newtonsoft.Json;
using System;

namespace AdventureWorks.Cosmos
{
    public class Person
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }

        [JsonProperty(PropertyName = "firstName")]
        public string FirstName { get; set; }

        [JsonProperty(PropertyName = "lastName")]
        public string LastName { get; set; }

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
    }
}
