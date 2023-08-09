using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StoreManagement.Models;

[Table("ExportStore")]
public partial class ExportStore
{
    [DisplayName("Mã xuất")]
    [Key]
    [Column("ID")]
    public Guid Id { get; set; }


    [DisplayName("Người xuất")]
    [StringLength(100)]
    public string ExporterName { get; set; } = null!;


    [DisplayName("Khách hàng")]
    [StringLength(100)]
    public string Customer { get; set; } = null!;


    [DisplayName("Ngày xuất")]
    [Column(TypeName = "datetime")]
    public DateTime ExportDate { get; set; }


    [DisplayName("Tổng tiền")]
    [Column(TypeName = "money")]
    public float Total { get; set; }


    [DisplayName("Ngày cập nhật")]
    [Column(TypeName = "datetime")]
    public DateTime CreatedDate { get; set; }

    [InverseProperty("ExportStore")]
    public virtual ICollection<ExportStoreDetail> ExportStoreDetails { get; set; } = new List<ExportStoreDetail>();
}
