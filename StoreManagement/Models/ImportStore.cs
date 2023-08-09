using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ImportStore")]
public partial class ImportStore
{
    [DisplayName("Mã nhập")]
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }

    [DisplayName("Người nhập")]
    [StringLength(100)]
    public string ImporterName { get; set; } = null!;


    [DisplayName("Nhà cung cấp")]
    [StringLength(100)]
    public string Supplier { get; set; } = null!;


    [DisplayName("Ngày nhập")]
    [Column(TypeName = "datetime")]
    public DateTime ImportDate { get; set; }


    [DisplayName("Tổng tiền")]
    [Column(TypeName = "money")]
    public float Total { get; set; }


    [DisplayName("Ngày tạo phiếu")]
    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("ImportStore")]
    public virtual ICollection<ImportStoreDetail> ImportStoreDetails { get; set; } = new List<ImportStoreDetail>();
}
