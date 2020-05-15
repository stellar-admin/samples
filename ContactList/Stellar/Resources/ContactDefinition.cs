using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactList.Data;
using StellarAdmin;
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

            HasField(c => c.Id,
                field =>
                {
                    field.IsKey = true;
                    field.HideOnList();
                    field.HideOnForms();
                    field.HideOnDetail();
                });
            HasField(c => c.FirstName);
            HasField(c => c.LastName);
            HasField(c => c.EmailAddress);
            HasField(c => c.PhoneNumber);
            HasField(c => c.Type);
        }

        public override Task<ValidationResult> CreateAsync(IDictionary<string, string> values, StellarRequest request)
        {
            var contact = new Contact { Id = Guid.NewGuid()};
            
            // Apply form values
            SetResourceValues(contact, values);
            
            // Validate the resource
            var validationResult = ValidateResource(contact, request);

            // Add to the list if it is valid
            if (validationResult.IsValid)
            {
                _contactDatabase.Contacts.Add(contact);
            }

            return Task.FromResult(validationResult);
        }

        public override Task DeleteAsync(object key, IStellarRequest request)
        {
            var contact = FindContact(key);
            if (contact != null)
            {
                _contactDatabase.Contacts.Remove(contact);
            }
            
            return Task.CompletedTask;
        }

        public override Task<object> GetByKeyAsync(object key, IStellarRequest request)
        {
            var contact = FindContact(key);

            return Task.FromResult((object) contact);
        }

        public override Task<PagedResourceList> GetListAsync(ResourceQuery query, IStellarRequest request)
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

        public override Task<IEnumerable<SelectItem>> GetLookupListAsync(ResourceLookupQuery query,
            IStellarRequest request)
        {
            throw new NotImplementedException();
        }

        public override Task<ValidationResult> UpdateAsync(object key, IDictionary<string, string> values,
            IStellarRequest request)
        {
            var contact = FindContact(key);
            if (contact != null)
            {
                // Update the values
                SetResourceValues(contact, values);
                
                // Perform validation
                return Task.FromResult(ValidateResource(contact, request));
            }

            return Task.FromResult(new ValidationResult());
        }

        private Contact FindContact(object key)
        {
            return _contactDatabase.Contacts.FirstOrDefault(c => c.Id == new Guid(key.ToString()));
        }
    }
}