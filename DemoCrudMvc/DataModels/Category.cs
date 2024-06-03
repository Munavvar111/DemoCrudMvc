using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DemoCrudMvc.DataModels;

[Table("Category")]
public partial class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Column(TypeName = "character varying")]
    public string CategoryName { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? CategoryDesc { get; set; }

    public bool? IsDeleted { get; set; }

    [InverseProperty("Category")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
