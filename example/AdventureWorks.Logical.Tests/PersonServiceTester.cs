using AdventureWorks.Logical.PersonDelete;
using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Tests
{
    public class PersonServiceTester
    {
        public IPersonRead PersonRead { get; }
        public IPersonWrite PersonWrite { get; }
        public IPersonDelete PersonDelete { get; }

        public PersonServiceTester(IPersonRead personRead, IPersonWrite personWrite, IPersonDelete personDelete)
        {
            PersonRead = personRead;
            PersonWrite = personWrite;
            PersonDelete = personDelete;
        }

        public async Task ReadReturnsAllPeopleWhenNoCriteriaProvided(int expectedCount)
        {
            var request = new PersonReadRequest { };
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(expectedCount, response.People.Length);
        }
                        
        public async Task ReadTotalCountReturnsTotalCountWhenNoCriteriaProvided(int expectedCount)
        {
            var request = new PersonReadRequest()
            {
                Select = new()
                {
                    TotalCount = true
                }
            };
            var response = await PersonRead.ReadAsync(request);
            Assert.IsNotNull(response.TotalCount);
            Assert.AreEqual(expectedCount, response.TotalCount.Value);
        }

        /// <summary>
        /// <code>
        /// SELECT * 
        /// FROM PERSON
        /// WHERE FirstName = 'Bill'
        /// </code>
        /// </summary>
        /// <returns></returns>        
        public async Task ReadReturnsQualfiedPeopleWhenFirstNameFilterSet(string firstName)
        {
            var request = new PersonReadRequest(
                where: new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        allOf: new PersonReadStringPredicate
                        {
                            Property = PersonReadStringProperty.FirstName,
                            Value = firstName
                        })));
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(1, response.People.Length);
        }
                
        public async Task ReadReturnsQualifiedPeopleWhenFirstNameAndLastNameFilterSetInDifferentCriteria(string firstName, string lastName, int expectedCount)
        {
            var request = new PersonReadRequest(
                where: new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        new PersonReadStringPredicate
                        {
                            Property = PersonReadStringProperty.FirstName,
                            Value = firstName,
                        },
                        new PersonReadStringPredicate
                        {
                            Property = PersonReadStringProperty.LastName,
                            Value = lastName,
                        })));
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(expectedCount, response.People.Length);
        }

                
        public async Task ReadReturnsQualifiedPeopleWhenAnyOfFilterSet((string firstName, string lastName)[] names, int expectedCount)
        {
            var request = new PersonReadRequest(
                names.Select(x => new PersonReadAnyOf(
                    anyOf: new PersonReadAllOf(
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.FirstName,
                            StringOperator.Equal,
                            x.firstName),
                        new PersonReadStringPredicate(
                            PersonReadStringProperty.LastName,
                            StringOperator.Equal,
                            x.lastName)))).ToArray());
            var response = await PersonRead.ReadAsync(request);
            Assert.AreEqual(expectedCount, response.People.Length);
        }

        public async Task WriteAddsPersonWhenPersonDoesNotExistAndClientSetId()
        {
            var id = Guid.NewGuid();
            var writeRequest = new PersonWriteRequest(
                new PersonInput
                { 
                    Title = "test",
                    FirstName = "test",
                    LastName = "test",
                    Id = id,
                });
            
            var writeResponse = await PersonWrite.WriteAsync(writeRequest);

            Assert.IsNotNull(writeResponse?.People);
            Assert.AreEqual(1, writeResponse.People.Length);

            var deleteRequest = new PersonDeleteRequest(
                new PersonDeleteCriteria
                {
                    Id = id
                });

            var deleteResponse = await PersonDelete.DeleteAsync(
                deleteRequest);

            Assert.IsNotNull(deleteResponse?.People);
            Assert.AreEqual(1, deleteResponse.People.Length);
            Assert.AreEqual(id, deleteResponse.People[0].Id);
        }

        public async Task WriteAddsPersonWhenPersonDoesNotExist(string title, string firstName, string lastName)
        {
            var request = new PersonWriteRequest(
                new PersonInput
                {
                    Title = title,
                    LastName = firstName,
                    FirstName = lastName,
                    EmailAddresses = new EmailUpdate
                    {
                        Add = new[]
                        {
                            new EmailUpdateAdd{}
                        }
                    }
                }
            );

            var expectedCount = await TotalCountAsync() + 1;
            var response = await PersonWrite.WriteAsync(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.People);
            // one person is added
            Assert.AreEqual(1, response.People.Length);
            // count is incremented by 1
            Assert.AreEqual(expectedCount, await TotalCountAsync());
            var deleteResponse = await PersonDelete.DeleteAsync(
                new PersonDeleteRequest(
                    new PersonDeleteCriteria
                    { 
                        Id = response.People[0].Id,
                    }));
            Assert.IsNotNull(deleteResponse);
            Assert.IsNotNull(deleteResponse.People);
            Assert.AreEqual(1, deleteResponse.People.Length);
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

        public async Task WriteReturnsZeroResultsWhenSetIsEmpty()
        {
            var request = new PersonWriteRequest();
            var expectedCount = 0;
            var response = await PersonWrite.WriteAsync(request);
            Assert.IsNotNull(response);
            Assert.IsNotNull(response.People);            
            Assert.AreEqual(expectedCount, response.People.Length);
        }

        public async Task WriteUpdatesPersonWhenPersonExists(Guid id, string title)
        {            
            var request = new PersonWriteRequest(
                new PersonInput
                {
                    Id = id,
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
                                id)))));

            Assert.AreEqual(1, personRead.People.Length);
            Assert.AreEqual(title, personRead.People[0].Title);
        }
    }
}
