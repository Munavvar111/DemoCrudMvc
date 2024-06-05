using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("Customer")]
public partial class Customer
{
    [Key]
    public int CustomerId { get; set; }

    [Column(TypeName = "character varying")]
    public string? FirstName { get; set; }

    [Column(TypeName = "character varying")]
    public string? LastName { get; set; }

    [Column(TypeName = "character varying")]
    public string Email { get; set; } = null!;

    public long? ZipCode { get; set; }

    [Column(TypeName = "character varying")]
    public string? Address { get; set; }

    [Column(TypeName = "character varying")]
    public string? City { get; set; }

    [InverseProperty("Customer")]
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    [InverseProperty("Customer")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
