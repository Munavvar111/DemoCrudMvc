using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("Payment")]
public partial class Payment
{
    [Key]
    public int PaymentId { get; set; }

    public int PaymentType { get; set; }

    [Column(TypeName = "character varying")]
    public string? PaymentOrderId { get; set; }

    [Column(TypeName = "character varying")]
    public string? RazorPaymentId { get; set; }

    [Column(TypeName = "character varying")]
    public string? RazorSignature { get; set; }

    [InverseProperty("Payment")]
    public virtual ICollection<OrderProduct> OrderProducts { get; set; } = new List<OrderProduct>();

    [ForeignKey("PaymentType")]
    [InverseProperty("Payments")]
    public virtual PaymentType PaymentTypeNavigation { get; set; } = null!;
}
