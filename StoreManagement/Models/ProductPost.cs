using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class ProductPost
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }

    public Guid AuthorId { get; set; }

    [StringLength(255)]
    public string? CoverImg { get; set; }

    public string Content { get; set; } = null!;

    public bool? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("AuthorId")]
    [InverseProperty("ProductPosts")]
    public virtual User Author { get; set; } = null!;

    [ForeignKey("ProductId")]
    [InverseProperty("ProductPosts")]
    public virtual Product Product { get; set; } = null!;
}
