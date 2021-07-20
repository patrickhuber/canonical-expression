using AdventureWorks.Logical.Tests;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.Cosmos.Tests
{
    [TestClass]
    public class CosmosPersonServiceTests
    {
        private const string DatabaseId = "AdventureWorksDb";
        private const string ContainerId = "People";
        private PersonServiceTester tester;
        private static CosmosClient  _cosmosClient;


        private static readonly Person BillBrasky = new() { FirstName = "Bill", LastName = "Brasky", Id = new ("b929faa9-c2a2-461a-afc8-652217ef2676") };
        private static readonly Person TedRosevelt = new() { FirstName = "Ted", LastName = "Rosevelt", Id = new ("b950d84b-3671-4ee9-a565-e9109b1d9be8") };
        private static readonly Person JamesKing = new() { FirstName = "James", LastName = "King", Id = new ("f0492aa2-80e8-432e-9a94-52ae2e0491da") };
        private static readonly Person JamesDean = new() { FirstName = "James", LastName = "Dean", Id = new ("3a018b9f-5517-4fe5-94f4-fc47dae3eae5") };
        private static readonly Person[] People = new Person[] { BillBrasky, TedRosevelt, JamesKing, JamesDean };

        [ClassInitialize]
#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task ClassInitializeAsync(TestContext testContext)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            var builder = new ConfigurationBuilder()
                   .AddUserSecrets<CosmosPersonServiceTests>();

            var configuration = builder.Build();
            var connectionString = configuration["Cosmos:ConnectionString"];
            _cosmosClient = new CosmosClient(connectionString);
            await SetupDatabaseAsync();
        }

        private static async Task SetupDatabaseAsync()
        {
            var databaseId = DatabaseId;
            var databaseResponse = await _cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
            var database = databaseResponse.Database;
            var containerResponse = await database.CreateContainerIfNotExistsAsync(ContainerId, "/id");
            var container = containerResponse.Container;

            var tasks = new List<Task>();
            foreach (var person in People)
            {
                var task = container.UpsertItemAsync(person);
                tasks.Add(task);
            }
            await Task.WhenAll(tasks);
        }

        [TestInitialize]
        public Task TestInitializeAsync() 
        {            
            var personService = new CosmosPersonService(_cosmosClient, DatabaseId, ContainerId);
            tester = new PersonServiceTester(personService, personService, personService);
            return Task.CompletedTask;
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
            await tester.ReadReturnsAllPeopleWhenNoCriteriaProvided(People.Length);
        }

        [TestMethod]
        public async Task ReadTotalCountReturnsTotalCountWhenNoCriteriaProvided()
        {
            await tester.ReadTotalCountReturnsTotalCountWhenNoCriteriaProvided(People.Length);
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
