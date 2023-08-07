using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ExportStore")]
public partial class ExportStore
{
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }


    [Display(Name = "Người xuất")]
    [Required(ErrorMessage = "Tên người xuất là bắt buộc")]
    [StringLength(100)]
    public string? ExporterName { get; set; }


    [Display(Name = "Ngày xuất")]
    [Required(ErrorMessage = "Ngày xuất là bắt buộc")]
    [Column(TypeName = "datetime")]
    public DateTime ExporterDate { get; set; }


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
    [InverseProperty("ExportStores")]
    public virtual Product? Product { get; set; } = null!;
}
