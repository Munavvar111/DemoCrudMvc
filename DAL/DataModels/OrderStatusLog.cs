using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("OrderStatusLog")]
public partial class OrderStatusLog
{
    [Key]
    public int StatusLogId { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime UpDatedDate { get; set; }

    public int OrderStatus { get; set; }

    [Column(TypeName = "character varying")]
    public string UniqOrderId { get; set; } = null!;
}
