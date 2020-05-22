using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Store.Models
{
    public class Order
    {
        [Required]
        public Customer Customer { get; set; }

        public int CustomerId { get; set; }

        public DateTime? DeliveryDate { get; set; }

        public int Id { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public string ShipCity { get; set; }

        public string ShipCountry { get; set; }

        public string ShipPostalCode { get; set; }

        public string ShipStreetAddress { get; set; }
    }
}