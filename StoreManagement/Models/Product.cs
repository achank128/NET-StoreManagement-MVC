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
    [StringLength(20)]
    [Unicode(false)]
    public string ProductCode { get; set; } = null!;


    [DisplayName("Tên sản phẩm")]
    [StringLength(100)]
    public string ProductName { get; set; } = null!;


    [DisplayName("Nhà xản xuất")]
    [StringLength(100)]
    public string Manufacturer { get; set; } = null!;


    [DisplayName("Danh mục")]
    public Guid CategoryId { get; set; }


    [DisplayName("Mô tả")]
    [StringLength(255)]
    public string? Description { get; set; }


    [DisplayName("Đơn vị tính")]
    [StringLength(50)]
    public string? Unit { get; set; }


    [DisplayName("Đơn giá")]
    [Column(TypeName = "money")]
    public float? Price { get; set; }


    [DisplayName("Số lượng tồn")]
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
