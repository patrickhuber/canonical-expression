using AdventureWorks.Logical.PersonDelete;
using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using LinqKit;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdventureWorks.Cosmos
{
    public class CosmosPersonService : IPersonRead, IPersonWrite, IPersonDelete
    {
        private readonly CosmosClient _cosmosClient;
        private readonly Container _container;

        public CosmosPersonService(CosmosClient cosmosClient, string databaseId = "AdventureWorksDB", string containerId = "People")
        {
            _cosmosClient = cosmosClient;
            _container = _cosmosClient.GetContainer(databaseId, containerId);
        }

        public async Task<PersonReadResponse> ReadAsync(PersonReadRequest request)
        {
            // For direct ID queries, some analysis can be done of the request. If it conforms to direct ID queryies, then change the Filter method
            var queryable = Filter(request);
            return await ProjectAsync(request, queryable);
        }

        private IQueryable<Person> Filter(PersonReadRequest request)
        {
            var noCriteriaProvided = request.Where is null || request.Where.Length == 0;

            IQueryable<Person> queryable = _container.GetItemLinqQueryable<Person>(true);
            if (noCriteriaProvided)
            {
                return queryable;
            }

            var anyOfPredicateBuilder = PredicateBuilder.New<Person>();            
            foreach (var anyOf in request.Where)
            {
                var allOfPredicate = PredicateBuilder.New<Person>();
                foreach (var allOf in anyOf.AnyOf)
                {
                    foreach (var predicate in allOf.AllOf)
                    {
                        if (predicate is PersonReadGuidPredicate guidPredicate)
                        {
                            if (guidPredicate.Operator == GuidOperator.Equal)
                                allOfPredicate = allOfPredicate.And(x => x.Id == guidPredicate.Value);
                        }
                        if (predicate is PersonReadStringPredicate stringPredicate)
                        {
                            if (stringPredicate.Operator == StringOperator.Equal)
                            {
                                if (stringPredicate.Property == PersonReadStringProperty.FirstName)
                                {
                                    allOfPredicate = allOfPredicate.And(x => x.FirstName == stringPredicate.Value);
                                }
                                else if (stringPredicate.Property == PersonReadStringProperty.LastName)
                                {
                                    allOfPredicate = allOfPredicate.And(x => x.LastName == stringPredicate.Value);
                                }
                            }
                        }
                    }
                }
                anyOfPredicateBuilder = anyOfPredicateBuilder.Or(allOfPredicate);
            }
            return queryable.Where(anyOfPredicateBuilder);
        }

        private static async Task<PersonReadResponse> ProjectAsync(PersonReadRequest request, IQueryable<Person> queryable)
        {            
            var selectCount = request.Select is not null && request.Select.TotalCount;
            var response = new PersonReadResponse();
            if (selectCount)
            {
                int count = await queryable.CountAsync();
                response.TotalCount = count;
            }
            else
            {
                var people = await MapAsync(queryable);
                response.People = people.ToArray();
            }
            return response;
        }
        
        private static async Task<List<Logical.Person>> MapAsync(IQueryable<Person> queryable)
        {            
            var iterator = queryable.ToFeedIterator();
            var people = new List<Logical.Person>();

            while (iterator.HasMoreResults)
            {
                var results = await iterator.ReadNextAsync();
                people.AddRange(results.Select(Map));
            }
            return people;
        }

        private static Logical.Person Map(Person result)
        {
            return new Logical.Person
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                Title = result.Title,
            };
        }

        public async Task<PersonWriteResponse> WriteAsync(PersonWriteRequest request)
        {            
            // return the empty set if the write is empty
            if (request?.Set == null || request.Set.Length == 0)
            {
                return new PersonWriteResponse
                {
                    People = Array.Empty<Logical.Person>()
                };
            }

            // fetch the documents using multi lookup            
            var criteria =
                from p in request.Set
                where p.Id.HasValue
                select (p.Id.ToString(), new PartitionKey(p.Id.ToString()));

            var people = await _container.ReadManyItemsAsync<Person>(criteria.ToList());

            // match each item to its input, loop over the results modifying the items
            // items that don't have an id are null matched to a person
            var matches =
                from i in request.Set
                join p in people.Resource
                    on i.Id ?? Guid.Empty equals p.Id into match
                from m in match.DefaultIfEmpty()
                select new { Person = m, PersonInput = i };

            var tasks = new List<Task>();
            var upserts = new List<Person>();

            // insert each item without a match
            foreach (var update in matches)
            {
                var person = update.Person is null
                    ? Create(update.PersonInput)
                    : Mutate(update.Person, update.PersonInput);

                upserts.Add(person);
                var task = _container.UpsertItemAsync(person);
                tasks.Add(task);
            }
            
            await Task.WhenAll(tasks);

            return new PersonWriteResponse 
            {
                People = upserts.Select(Map).ToArray()
            };
        }

        private static Person Mutate(Person person, PersonInput input)
        {
            if (input.FirstName is not null && input.FirstName.IsSet)
                person.FirstName = input.FirstName.Value;
            if (input.LastName is not null && input.LastName.IsSet)
                person.LastName = input.LastName.Value;
            if (input.Title is not null && input.Title.IsSet)
                person.Title = input.Title.Value;
            return person;
        }

        private static Person Create(PersonInput personInput)
        {
            var person = new Person
            {
                Id = personInput.Id ?? Guid.NewGuid()
            };
            Mutate(person, personInput);
            return person;
        }

        public async Task<PersonDeleteResponse> DeleteAsync(PersonDeleteRequest request)
        {
            if (request?.Where is null || request.Where.Length == 0)
                return new PersonDeleteResponse
                {
                    People = Array.Empty<PersonDeleteResult>()
                };

            // cosmos doesn't return the id of the item in a consistent way (uses internal diagnostics) so we need to keep the id matched with the task
            var tasks = new Dictionary<Guid, Task<ItemResponse<Person>>>();
            var removed = new List<PersonDeleteResult>();

            foreach (var criteria in request.Where)
            {
                var task = _container.DeleteItemAsync<Person>(criteria.Id.ToString(), new PartitionKey(criteria.Id.ToString()));
                tasks[criteria.Id] = task;
            }

            await Task.WhenAll(tasks.Values);
            
            foreach (var task in tasks)
            {
                var result = await task.Value;
                var id = task.Key;

                if (result.StatusCode == System.Net.HttpStatusCode.NotFound)
                    continue;
                removed.Add(
                    new PersonDeleteResult 
                    { 
                        Id = id
                    });
            }
            return new PersonDeleteResponse { People = removed.ToArray() };
        }
    }
}
