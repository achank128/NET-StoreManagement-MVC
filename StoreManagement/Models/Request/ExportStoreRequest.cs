using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Request
{
    public class ExportStoreRequest
    {
        public Guid? Id { get; set; }
        public string ExporterName { get; set; } = null!;
        public string Customer { get; set; } = null!;
        public DateTime ExportDate { get; set; }
        public decimal Total { get; set; }
        public List<ProductItem> ListProducts { get; set; }
    }
}
