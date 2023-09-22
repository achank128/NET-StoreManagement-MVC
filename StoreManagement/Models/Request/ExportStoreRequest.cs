using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Request
{
    public class ExportStoreRequest
    {
        public Guid? Id { get; set; }
        public Guid ExporterId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime ExportDate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Discount { get; set; }
        public string Note { get; set; }
        public decimal Total { get; set; }
        public List<ProductItem> ListProducts { get; set; }
    }
}
