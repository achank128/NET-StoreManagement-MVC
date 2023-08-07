using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ImportStore")]
public partial class ImportStore
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }


    [Display(Name = "Người nhập")]
    [Required(ErrorMessage = "Tên người nhập là bắt buộc")]
    [StringLength(100)]
    public string? ImporterName { get; set; }


    [Display(Name = "Ngày nhập")]
    [Required(ErrorMessage = "Ngày nhập là bắt buộc")]
    [Column(TypeName = "datetime")]
    public DateTime ImportDate { get; set; }


    [Display(Name = "Sản phẩm")]
    [Required(ErrorMessage = "Sản phẩm là bắt buộc")]
    public Guid ProductId { get; set; }


    [Display(Name = "Số lượng")]
    [Required(ErrorMessage = "Số lượng là bắt buộc")]
    public int? Quantity { get; set; }


    [Display(Name = "Tổng tiền")]
    [Column(TypeName = "money")]
    public float? Total { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ImportStores")]
    public virtual Product? Product { get; set; } = null!;
}
