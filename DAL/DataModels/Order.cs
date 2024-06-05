using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("Order")]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int OrderPrice { get; set; }

    public int OrderQuantity { get; set; }

    public int CustomerId { get; set; }

    public int Status { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime OrderDate { get; set; }

    [Column(TypeName = "character varying")]
    public string UniqOrderId { get; set; } = null!;

    [ForeignKey("CustomerId")]
    [InverseProperty("Orders")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("Orders")]
    public virtual Product Product { get; set; } = null!;

    [ForeignKey("Status")]
    [InverseProperty("Orders")]
    public virtual OrderStatus StatusNavigation { get; set; } = null!;
}
