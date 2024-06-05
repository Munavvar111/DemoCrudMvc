using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("OrderStatus")]
public partial class OrderStatus
{
    [Key]
    public int StatusId { get; set; }

    [Column(TypeName = "character varying")]
    public string StatusName { get; set; } = null!;

    [InverseProperty("StatusNavigation")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
