using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Request
{
    public class ImportStoreRequest
    {
        public Guid? Id { get; set; }
        public string ImporterName { get; set; } = null!;
        public string Supplier { get; set; } = null!;
        public DateTime ImportDate { get; set; }
        public decimal Total { get; set; }
        public List<ProductItem> ListProducts { get; set; }
    }
}
