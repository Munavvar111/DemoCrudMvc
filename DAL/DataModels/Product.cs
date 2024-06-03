using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

[Table("product")]
public partial class Product
{
    [Key]
    [Column("productId")]
    public int ProductId { get; set; }

    [Column(TypeName = "character varying")]
    public string ProductName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public long? Price { get; set; }

    [Column(TypeName = "character varying")]
    public string? UniqueNo { get; set; }

    public bool? Isdeleted { get; set; }

    public int? Quantity { get; set; }

    [Column(TypeName = "timestamp without time zone")]
    public DateTime? DateTimePicker { get; set; }

    [Column(TypeName = "character varying")]
    public string? FeaturePhoto { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ProductPhoto> ProductPhotos { get; set; } = new List<ProductPhoto>();
}
