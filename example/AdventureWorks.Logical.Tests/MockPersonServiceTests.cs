using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using AdventureWorks.Logical.Updates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Tests
{
    [TestClass]
    public class MockPersonServiceTests
    {
        public IPersonRead PersonRead { get; private set; }
        public IPersonWrite PersonWrite { get; private set; }

        public List<Person> People { get; private set; }

        private static Person BillBrasky = new() { FirstName = "Bill", LastName = "Brasky", Id = Guid.NewGuid() };
        private static Person TedRosevelt = new() { FirstName = "Ted", LastName = "Rosevelt", Id = Guid.NewGuid() };
        private static Person JamesKing = new() { FirstName = "James", LastName = "King", Id = Guid.NewGuid() };
        private static Person JamesDean = new() { FirstName = "James", LastName = "Dean", Id = Guid.NewGuid() };

        [TestInitialize]
        public void Initialize()
        {
            People = new List<Person>
            {
                BillBrasky,
                TedRosevelt,
                JamesKing,
                JamesDean
            };
            var personService = new MockPersonService(People);
            PersonRead = personService;
            PersonWrite = personService;
        }

        /// <summary>
        /// <code>
        /// SELECT * FROM PERSON
        /// </code>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ReadReturnsAllPeopleWhenNoCriteriaProvided()
        {
            var request = new PersonReadRequest { };
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(People.Count, response.People.Length);
        }

        [TestMethod]
        public async Task ReadTotalCountReturnsTotalCountWhenNoCriteriaProvided()
        {
            var request = new PersonReadRequest() 
            {
                Select = new () 
                { 
                    TotalCount = true 
                }
            };
            var response = await PersonRead.ReadAsync(request);
            Assert.IsNotNull(response.TotalCount);
            Assert.AreEqual(People.Count, response.TotalCount.Value);
        }

        /// <summary>
        /// <code>
        /// SELECT * 
        /// FROM PERSON
        /// WHERE FirstName = 'Bill'
        /// </code>
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task ReadReturnsQualfiedPeopleWhenFirstNameFilterSet()
        {
            var request = new PersonReadRequest(
                where: new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        allOf: new PersonReadStringPredicate
                        {
                            Property = PersonReadStringProperty.FirstName,
                            Value = "Bill"
                        })));
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(1, response.People.Length);
        }

        [TestMethod]
        public async Task ReadReturnsQualifiedPeopleWhenFirstNameAndLastNameFilterSetInDifferentCriteria()
        {
            var request = new PersonReadRequest(
                where: new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        new PersonReadStringPredicate
                        {
                            Property = PersonReadStringProperty.FirstName,
                            Value = "Bill",
                        },
                        new PersonReadStringPredicate
                        {
                            Property = PersonReadStringProperty.LastName,
                            Value = "Brasky",
                        })));
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(1, response.People.Length);
        }


        [TestMethod]
        public async Task ReadReturnsQualifiedPeopleWhenAnyOfFilterSet()
        {
            var request = new PersonReadRequest(
                new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.FirstName,
                            StringOperator.Equal,
                            BillBrasky.FirstName),
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.LastName,
                            StringOperator.Equal,
                            BillBrasky.LastName)
                        )),
                new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.FirstName,
                            StringOperator.Equal,
                            BillBrasky.FirstName.Reverse().ToString()),
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.LastName,
                            StringOperator.Equal,
                            BillBrasky.LastName.Reverse().ToString()))));
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(1, response.People.Length);
        }

        [TestMethod]
        public async Task WriteAddsPersonWhenPersonDoesNotExist()
        {
            var request = new PersonWriteRequest
            {
                Set = new[]
                {
                    new PersonInput
                    {
                        Title = "Mr",
                        LastName = "Sinatra",
                        FirstName = "Frank",
                        EmailAddresses = new EmailUpdate
                        {
                            Add = new []
                            {
                                new EmailUpdateAdd{}
                            }
                        }
                    }
                }
            };

            var expectedCount = await TotalCountAsync() + 1;
            var response = await PersonWrite.WriteAsync(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.People);
            // one person is added
            Assert.AreEqual(1, response.People.Length);
            // count is incremented by 1
            Assert.AreEqual(expectedCount, await TotalCountAsync());
        }

        private async Task<int> TotalCountAsync() 
        {
            var response = await PersonRead.ReadAsync(
                new PersonReadRequest 
                { 
                    Select = new PersonReadProjection 
                    { 
                        TotalCount = true 
                    } 
                });
            return response.TotalCount.Value;
        }

        [TestMethod]
        public async Task WriteUpdatesPersonWhenPersonExists()
        {
            var title = "Mr";
            var request = new PersonWriteRequest(
                new PersonInput
                {
                    Id = JamesDean.Id,
                    Title = title,
                });

            var expectedCount = await TotalCountAsync();
            var response = await PersonWrite.WriteAsync(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.People);
            // one person is added
            Assert.AreEqual(1, response.People.Length);
            // count is incremented by 1
            Assert.AreEqual(expectedCount, await TotalCountAsync());

            // returned title matches
            var person = response.People[0];
            Assert.AreEqual(title, person.Title);

            // fetch by id and check title
            var personRead = await PersonRead.ReadAsync(
                new PersonReadRequest(
                    new PersonReadAnyOf(
                        new PersonReadAllOf(
                            new PersonReadGuidPredicate(
                                PersonReadGuidProperty.Id, 
                                GuidOperator.Equal, 
                                JamesDean.Id)))));

            Assert.AreEqual(1, personRead.People.Length);
            Assert.AreEqual(title, personRead.People[0].Title);
        }
    }
}
