using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class Product
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }


    [Display(Name = "Tên sản phẩm")]
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;


    [Display(Name = "Hãng sản xuất")]
    [Required(ErrorMessage = "Hãng sản xuất là bắt buộc")]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;


    [Display(Name = "Mô tả")]
    [StringLength(255)]
    public string? Description { get; set; }


    [Display(Name = "Đơn giá")]
    [Required(ErrorMessage = "Đơn giá là bắt buộc")]
    [Column(TypeName = "money")]
    public float? Price { get; set; }


    [Display(Name = "Số lượng tồn")]
    [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
    public int? Number { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ExportStore> ExportStores { get; set; } = new List<ExportStore>();

    [InverseProperty("Product")]
    public virtual ICollection<ImportStore> ImportStores { get; set; } = new List<ImportStore>();
}
