using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("OrderProduct")]
public partial class OrderProduct
{
    [Key]
    public int OrderProductId { get; set; }

    public int CustomerId { get; set; }

    [Column(TypeName = "character varying")]
    public string? OrderUniqId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("OrderProducts")]
    public virtual Customer Customer { get; set; } = null!;
}
