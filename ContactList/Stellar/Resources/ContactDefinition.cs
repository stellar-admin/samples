using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactList.Data;
using StellarAdmin.Fields;
using StellarAdmin.Models;
using StellarAdmin.Resources;
using StellarAdmin.Validation;

namespace ContactList.Stellar.Resources
{
    public class ContactDefinition : ResourceDefinition<Contact>
    {
        private readonly ContactDatabase _contactDatabase;

        public ContactDefinition(ContactDatabase contactDatabase)
        {
            _contactDatabase = contactDatabase;
        }

        public override Task<ValidationResult> CreateAsync(IDictionary<string, string> values)
        {
            var contact = new Contact {Id = Guid.NewGuid()};

            // Apply form values
            SetResourceValues(contact, values);

            // Validate the resource
            var validationResult = ValidateResource(contact);

            // Add to the list if it is valid
            if (validationResult.IsValid) _contactDatabase.Contacts.Add(contact);

            return Task.FromResult(validationResult);
        }

        public override Task DeleteAsync(object key)
        {
            var contact = FindContact(key);
            if (contact != null) _contactDatabase.Contacts.Remove(contact);

            return Task.CompletedTask;
        }

        public override Task<object> GetByKeyAsync(object key)
        {
            var contact = FindContact(key);

            return Task.FromResult((object) contact);
        }

        public override Task<PagedResourceList> GetListAsync(ResourceQuery query)
        {
            var skip = (query.PageNo - 1) * query.PageSize;
            var take = query.PageSize;

            var count = _contactDatabase.Contacts.Count;
            var contacts = _contactDatabase.Contacts
                .OrderBy(c => c.LastName)
                .Skip(skip)
                .Take(take);

            var pagedContactList = new PagedResourceList(contacts, query.PageNo, query.PageSize, count);

            return Task.FromResult(pagedContactList);
        }

        public override Task<IEnumerable<SelectItem>> GetLookupListAsync(ResourceLookupQuery query)
        {
            throw new NotImplementedException();
        }

        public override Task<ValidationResult> UpdateAsync(object key, IDictionary<string, string> values)
        {
            var contact = FindContact(key);
            if (contact != null)
            {
                // Update the values
                SetResourceValues(contact, values);

                // Perform validation
                return Task.FromResult(ValidateResource(contact));
            }

            return Task.FromResult(new ValidationResult());
        }

        private Contact FindContact(object key)
        {
            return _contactDatabase.Contacts.FirstOrDefault(c => c.Id == new Guid(key.ToString()));
        }
    }
}