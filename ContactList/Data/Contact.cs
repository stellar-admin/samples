using System;
using System.ComponentModel.DataAnnotations;

namespace ContactList.Data
{
    public class Contact
    {
        public string EmailAddress { get; set; }

        public string FirstName { get; set; }

        [Key]
        public Guid Id { get; set; }

        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        public ContactType Type { get; set; }
    }
}