using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

public partial class Product
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }


    [DisplayName("Mã sản phẩm")]
    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
    [StringLength(20)]
    [Unicode(false)]
    public string ProductCode { get; set; } = null!;


    [DisplayName("Tên sản phẩm")]
    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;


    [DisplayName("Nhà xản xuất")]
    [Required(ErrorMessage = "Nhà xản xuất là bắt buộc")]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;


    [DisplayName("Danh mục")]
    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public Guid CategoryId { get; set; }


    [DisplayName("Mô tả")]
    [StringLength(255)]
    public string? Description { get; set; }


    [DisplayName("Đơn vị tính")]
    [Required(ErrorMessage = "Đơn vị tính là bắt buộc")]
    [StringLength(50)]
    public string? Unit { get; set; }


    [DisplayName("Đơn giá")]
    [Required(ErrorMessage = "Đơn giá là bắt buộc")]
    [Column(TypeName = "money")]
    public float? Price { get; set; }


    [DisplayName("Số lượng tồn")]
    [Required(ErrorMessage = "Số lượng tồn là bắt buộc")]
    public int? Number { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category? Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual ICollection<ExportStoreDetail>? ExportStoreDetails { get; set; } = new List<ExportStoreDetail>();

    [InverseProperty("Product")]
    public virtual ICollection<ImportStoreDetail>? ImportStoreDetails { get; set; } = new List<ImportStoreDetail>();
}
