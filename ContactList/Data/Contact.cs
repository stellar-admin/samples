using System;

namespace ContactList.Data
{
    public class Contact
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        public Guid Id { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public ContactType Type { get; set; }
    }
}