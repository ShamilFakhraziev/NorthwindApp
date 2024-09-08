using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Xml.Serialization;

namespace Packt.Shared
{
    [Index(nameof(City), Name = "City")]
    [Index(nameof(CompanyName), Name = "CompanyName")]
    [Index(nameof(PostalCode), Name = "PostalCode")]
    [Index(nameof(Region), Name = "Region")]
    public partial class Customer
    {
        public Customer()
        {
            Orders = new HashSet<Order>();
            CustomerTypes = new HashSet<CustomerDemographic>();
        }

        [Key]
        [Required]
        [StringLength(5)]
        [RegularExpression("[A-Z]{5}")]
        public string CustomerId { get; set; } = null!;
        [Required]
        [StringLength(40)]
        public string CompanyName { get; set; } = null!;
        [StringLength(30)]
        public string? ContactName { get; set; }
        [StringLength(30)]
        public string? ContactTitle { get; set; }
        [StringLength(60)]
        public string? Address { get; set; }
        [StringLength(15)]
        public string? City { get; set; }
        [StringLength(15)]
        public string? Region { get; set; }
        [StringLength(10)]
        public string? PostalCode { get; set; }
        [StringLength(15)]
        public string? Country { get; set; }
        [StringLength(24)]
        public string? Phone { get; set; }
        [StringLength(24)]
        public string? Fax { get; set; }

        [InverseProperty(nameof(Order.Customer))]
        [XmlIgnore]
        public virtual ICollection<Order> Orders { get; set; }

        [ForeignKey("CustomerId")]
        [InverseProperty(nameof(CustomerDemographic.Customers))]
        [XmlIgnore]
        public virtual ICollection<CustomerDemographic> CustomerTypes { get; set; }
    }
}
