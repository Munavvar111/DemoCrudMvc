using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("PaymentType")]
public partial class PaymentType
{
    [Key]
    public int PaymentTypeId { get; set; }

    [Column("PaymentType", TypeName = "character varying")]
    public string PaymentType1 { get; set; } = null!;

    [InverseProperty("PaymentTypeNavigation")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
