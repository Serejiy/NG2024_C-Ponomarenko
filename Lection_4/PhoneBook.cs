using System;
using System.Collections.Generic;
using System.Linq;

namespace PhoneBookApp
{
    public class PhoneBook
    {
        private List<Person> persons = new List<Person>();

        public void AddPerson(Person person)
        {
            if (persons.Any(p => p.PhoneNumber == person.PhoneNumber))
            {
                throw new Exception("Phone number already exists.");
            }
            persons.Add(person);
        }

        public Person GetPersonByPhone(string phoneNumber)
        {
            var person = persons.SingleOrDefault(p => p.PhoneNumber == phoneNumber);
            if (person == null)
            {
                throw new Exception("Person with this phone number not found.");
            }
            return person;
        }

        public List<Person> GetPersonsByName(string fullName)
        {
            var filteredPersons = persons.Where(p => p.FullName.Contains(fullName, StringComparison.OrdinalIgnoreCase)).ToList();
            if (!filteredPersons.Any())
            {
                throw new Exception("No persons found with this name.");
            }
            return filteredPersons;
        }

        public List<Person> GetAllPersons()
        {
            return persons.OrderBy(p => p.FullName).ToList();
        }

        public void DeletePersonByPhone(string phoneNumber)
        {
            var person = persons.SingleOrDefault(p => p.PhoneNumber == phoneNumber);
            if (person == null)
            {
                throw new Exception("Person with this phone number not found.");
            }
            persons.Remove(person);
        }
    }
}
