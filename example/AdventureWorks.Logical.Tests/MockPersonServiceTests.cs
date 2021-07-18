using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using AdventureWorks.Logical.PersonDelete;
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
        public IPersonDelete PersonDelete { get; private set; }

        public List<Person> People { get; private set; }

        private static Person BillBrasky = new() { FirstName = "Bill", LastName = "Brasky", Id = Guid.NewGuid() };
        private static Person TedRosevelt = new() { FirstName = "Ted", LastName = "Rosevelt", Id = Guid.NewGuid() };
        private static Person JamesKing = new() { FirstName = "James", LastName = "King", Id = Guid.NewGuid() };
        private static Person JamesDean = new() { FirstName = "James", LastName = "Dean", Id = Guid.NewGuid() };


        private PersonServiceTester tester;

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
            PersonDelete = personService;
            tester = new PersonServiceTester(PersonRead, PersonWrite, PersonDelete);
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
            await tester.ReadReturnsAllPeopleWhenNoCriteriaProvided(People.Count);
        }

        [TestMethod]
        public async Task ReadTotalCountReturnsTotalCountWhenNoCriteriaProvided()
        {
            await tester.ReadTotalCountReturnsTotalCountWhenNoCriteriaProvided(People.Count);
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
            await tester.ReadReturnsQualfiedPeopleWhenFirstNameFilterSet("Bill");
        }

        [TestMethod]
        public async Task ReadReturnsQualifiedPeopleWhenFirstNameAndLastNameFilterSetInDifferentCriteria()
        {
            await tester.ReadReturnsQualifiedPeopleWhenFirstNameAndLastNameFilterSetInDifferentCriteria("Bill", "Brasky", 1);
        }


        [TestMethod]
        public async Task ReadReturnsQualifiedPeopleWhenAnyOfFilterSet()
        {
            await tester.ReadReturnsQualifiedPeopleWhenAnyOfFilterSet(
                new[] 
                { 
                    (BillBrasky.FirstName, BillBrasky.LastName),
                    (BillBrasky.FirstName.Reverse().ToString(), BillBrasky.LastName.Reverse().ToString())
                }, 1);
        }

        [TestMethod]
        public async Task WriteAddsPersonWhenPersonDoesNotExist()
        {
            await tester.WriteAddsPersonWhenPersonDoesNotExist("Mr", "Frank", "Sinatra");
        }

        [TestMethod]
        public async Task WriteUpdatesPersonWhenPersonExists()
        {
            await tester.WriteUpdatesPersonWhenPersonExists(JamesDean.Id, "Mr");
        }

        [TestMethod]
        public async Task WriteReturnsZeroResultsWhenSetIsEmpty()
        {
            await tester.WriteReturnsZeroResultsWhenSetIsEmpty();
        }

        [TestMethod]
        public async Task WriteAddsPersonWhenPersonDoesNotExistAndClientSetId()
        {
            await tester.WriteAddsPersonWhenPersonDoesNotExistAndClientSetId();
        }
    }
}
