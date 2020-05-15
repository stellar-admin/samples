using System;
using System.Collections.Generic;
using Bogus;
using Bogus.Extensions;

namespace ContactList.Data
{
    public class ContactDatabase
    {
        public List<Contact> Contacts { get; }

        private ContactDatabase(IEnumerable<Contact> contacts)
        {
            Contacts = new List<Contact>(contacts);
        }

        public static ContactDatabase Create()
        {
            var contacts = new Faker<Contact>()
                .StrictMode(true)
                .RuleFor(c => c.Id, Guid.NewGuid)
                .RuleFor(c => c.FirstName, faker => faker.Person.FirstName)
                .RuleFor(c => c.LastName, faker => faker.Person.LastName)
                .RuleFor(c => c.EmailAddress, faker => faker.Person.Email)
                .RuleFor(c => c.PhoneNumber, faker => faker.Person.Phone.OrNull(faker, 0.15f))
                .RuleFor(c => c.Type, faker => faker.PickRandom<ContactType>())
                .Generate(100);

            return new ContactDatabase(contacts);
        }
    }
}