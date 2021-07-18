using AdventureWorks.Logical.PersonDelete;
using AdventureWorks.Logical.PersonRead;
using AdventureWorks.Logical.PersonWrite;
using AdventureWorks.Logical.Updates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureWorks.Logical.Tests
{
    public class MockPersonService : IPersonRead, IPersonWrite, IPersonDelete
    {
        private readonly Dictionary<Guid, Person> _people;

        public MockPersonService() { }

        public MockPersonService(List<Person> people)
        {
            // create a copy so this class owns the list
            _people = new Dictionary<Guid, Person>();
            foreach (var person in people)
                _people[person.Id] = person;
        }

        public Task<PersonReadResponse> ReadAsync(PersonReadRequest request)
        {
            return Task.FromResult(Read(request));
        }

        private PersonReadResponse Read(PersonReadRequest request)
        {
            var people = ReadCore(request);

            if (request.Select?.TotalCount ?? false)
                return new PersonReadResponse 
                { 
                    TotalCount = people.Length
                };

            return new PersonReadResponse
            {
                People = people
            };
        }

        private Person[] ReadCore(PersonReadRequest request)
        {
            var noCriteriaProvided = request.Where is null || request.Where.Length == 0;
            if (noCriteriaProvided)
                return _people.Values.ToArray();

            var people = new Dictionary<Guid, Person>();

            // iterate over people because we want to only traverse the list one time (think table scan)
            foreach (var person in _people.Values)
            {
                foreach (var anyOf in request.Where)
                {
                    foreach (var allOf in anyOf.AnyOf)
                    {
                        var isMatch = true;

                        // looking for all criteria to match
                        foreach (var criteria in allOf.AllOf)
                        {
                            isMatch = IsMatch(criteria, person);

                            // all of the criteria need to match the given object
                            if (!isMatch)
                                break;
                        }

                        if (isMatch)
                            people[person.Id] = person;
                    }
                }
            }
            return people.Values.ToArray();
        }
    

        /// <summary>
        /// Checks to see if the criteria matches the person object.
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        private static bool IsMatch(PersonReadPredicate predicate, Person person)
        {
            if (predicate is PersonReadGuidPredicate guidPredicate)
                return IsMatch(guidPredicate, person);
            if (predicate is PersonReadStringPredicate stringPredicate)
                return IsMatch(stringPredicate, person);

            // the default case when all criteria present match their respective fields
            return true;
        }

        private static bool IsMatch(PersonReadGuidPredicate predicate, Person person)
        {
            var propertyValue = predicate.Property switch
            {
                PersonReadGuidProperty.Id => person.Id,
                _ => default,
            };
            return predicate.Operator switch
            {
                GuidOperator.Equal => propertyValue.Equals(predicate.Value),
                _ => false,
            };
        }

        private static bool IsMatch(PersonReadStringPredicate predicate, Person person)
        {
            var propertyValue = predicate.Property switch
            {
                PersonReadStringProperty.FirstName => person.FirstName,
                PersonReadStringProperty.LastName => person.LastName,
                _ => default,
            };
            return predicate.Operator switch
            {
                StringOperator.Equal => propertyValue.Equals(predicate.Value),
                _ => false,
            };
        }

        public Task<PersonWriteResponse> WriteAsync(PersonWriteRequest request)
        {
            return Task.FromResult(Write(request));
        }

        private PersonWriteResponse Write(PersonWriteRequest request)
        {
            var response = new PersonWriteResponse();
            var people = new List<Person>();

            if (request?.Set == null || request.Set.Length == 0)
            {
                response.People = Array.Empty<Person>();
                return response;
            }

            // match people to inputs
            var matches =
                from i in request.Set
                join p in _people
                    on i.Id ?? Guid.Empty equals p.Key into match
                from m in match.DefaultIfEmpty()
                select new { Person = m.Value, PersonInput = i };

            var upserts = new List<Person>();
            foreach (var update in matches)
            {
                var person = update.Person is null
                    ? Create(update.PersonInput)
                    : Update(update.Person, update.PersonInput);
                _people[person.Id] = person;
                upserts.Add(person);
            }
            return new PersonWriteResponse
            { 
                People = upserts.ToArray(),
            };
        }

        private Person Update(Person person, PersonInput personInput)
        {
            Map(personInput, person);
            return person;
        }

        private static Person Create(PersonInput personInput)
        {
            var person = new Person
            {
                Id = personInput.Id ?? Guid.NewGuid()
            };
            Map(personInput, person);
            return person;
        }

        private static void Map(PersonInput personInput, Person person)
        {
            if (IsSetAndNotNull(personInput.FirstName))
                person.FirstName = personInput.FirstName.Value;
            if (IsSetAndNotNull(personInput.LastName))
                person.LastName = personInput.LastName.Value;
            if (IsSetAndNotNull(personInput.AdditionalContactInfo))
                person.AdditionalContactInfo = personInput.AdditionalContactInfo.Value;
            if (IsSetAndNotNull(personInput.Title))
                person.Title = personInput.Title.Value;
        }

        private static bool IsSetAndNotNull<T>(UpdateBase<T> value)
        {
            return value != null && value.IsSet;
        }

        public Task<PersonDeleteResponse> DeleteAsync(PersonDeleteRequest request)
        {
            return Task.FromResult(Delete(request));
        }

        public PersonDeleteResponse Delete(PersonDeleteRequest request)
        {
            if (request?.Where is null)
                return new PersonDeleteResponse
                {
                    People = Array.Empty<PersonDeleteResult>()
                };

            var removed = new List<PersonDeleteResult>();
            foreach (var criteria in request.Where)
                if (_people.Remove(criteria.Id))
                    removed.Add(new PersonDeleteResult 
                    { 
                        Id = criteria.Id
                    });
            return new PersonDeleteResponse 
            {
                People = removed.ToArray()
            };
        }
    }

}

