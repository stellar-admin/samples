using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Customer
    {
        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public int Id { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<Order> Orders { get; set; }

        public string Phone { get; set; }

        public string PostalCode { get; set; }
    }
}