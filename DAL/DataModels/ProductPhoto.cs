using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataModels;

public partial class ProductPhoto
{
    [Key]
    public int ProductPhotosId { get; set; }

    public int ProductId { get; set; }

    [Column(TypeName = "character varying")]
    public string PhotoName { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductPhotos")]
    public virtual Product Product { get; set; } = null!;
}
