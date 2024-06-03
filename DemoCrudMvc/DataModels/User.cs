using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DemoCrudMvc.DataModels;

[Table("User")]
public partial class User
{
    [Key]
    [Column("userID", TypeName = "character varying")]
    public string UserId { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string? FirstName { get; set; }

    [Column(TypeName = "character varying")]
    public string? Lastname { get; set; }

    [Column(TypeName = "character varying")]
    public string Email { get; set; } = null!;

    [Column(TypeName = "character varying")]
    public string Password { get; set; } = null!;
}
