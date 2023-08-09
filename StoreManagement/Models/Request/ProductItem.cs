namespace StoreManagement.Models.Request
{
    public class ProductItem
    {
        public Guid? Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public float Price { get; set; }
    }
}
