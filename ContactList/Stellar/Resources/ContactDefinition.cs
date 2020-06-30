namespace ContactList.Stellar.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Data;
    using StellarAdmin.Fields;
    using StellarAdmin.Models;
    using StellarAdmin.Resources;
    using StellarAdmin.Validation;

    public class ContactDefinition : ResourceDefinition<Contact>
    {
        private readonly ContactDatabase _contactDatabase;

        public ContactDefinition(ContactDatabase contactDatabase)
        {
            _contactDatabase = contactDatabase;

            ConfigureOptions = options => { options.Search.Allow = true; };
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

            IEnumerable<Contact> contacts = _contactDatabase.Contacts;

            // Apply search criteria
            if (!string.IsNullOrEmpty(query.Search))
                contacts = contacts.Where(c =>
                    c.FirstName.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase)
                    || c.LastName.Contains(query.Search, StringComparison.CurrentCultureIgnoreCase));
            var count = contacts.Count();

            // Apply sort criteria
            contacts = (query.OrderByField, query.OrderByDirection) switch
            {
                ("FirstName", SortDirection.Ascending) => contacts.OrderBy(contact => contact.FirstName),
                ("FirstName", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.FirstName),
                ("LastName", SortDirection.Ascending) => contacts.OrderBy(contact => contact.LastName),
                ("LastName", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.LastName),
                ("EmailAddress", SortDirection.Ascending) => contacts.OrderBy(contact => contact.EmailAddress),
                ("EmailAddress", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.EmailAddress),
                ("PhoneNumber", SortDirection.Ascending) => contacts.OrderBy(contact => contact.PhoneNumber),
                ("PhoneNumber", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.PhoneNumber),
                ("Type", SortDirection.Ascending) => contacts.OrderBy(contact => contact.Type),
                ("Type", SortDirection.Descending) => contacts.OrderByDescending(contact => contact.Type),
                _ => contacts
            };

            // Apply paging
            contacts = contacts.Skip(skip)
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

        protected override IEnumerable<IField> CreateFields()
        {
            return new[]
            {
                CreateField(contact => contact.Id),
                CreateField(contact => contact.FirstName, f => f.Sort.Allow = true),
                CreateField(contact => contact.LastName, f => f.Sort.Allow = true),
                CreateField(contact => contact.EmailAddress, f => f.Sort.Allow = true),
                CreateField(contact => contact.PhoneNumber, f => f.Sort.Allow = true),
                CreateField(contact => contact.Type, f => f.Sort.Allow = true)
            };
        }

        private Contact FindContact(object key)
        {
            return _contactDatabase.Contacts.FirstOrDefault(c => c.Id == new Guid(key.ToString()));
        }
    }
}