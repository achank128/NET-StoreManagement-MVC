using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace StoreManagement.Models.Request
{
    public class ImportStoreRequest
    {
        public Guid? Id { get; set; }
        public Guid ImporterId { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime ImportDate { get; set; }
        public decimal Total { get; set; }
        public string Note { get; set; }
        public List<ProductItem> ListProducts { get; set; }
    }
}
