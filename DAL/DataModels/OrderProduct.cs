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

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? OrderDate { get; set; }

    public bool NotificationBool { get; set; }

    public int PaymentId { get; set; }

    [ForeignKey("CustomerId")]
    [InverseProperty("OrderProducts")]
    public virtual Customer Customer { get; set; } = null!;

    [ForeignKey("PaymentId")]
    [InverseProperty("OrderProducts")]
    public virtual Payment Payment { get; set; } = null!;
}
