namespace backend.model
{
    public class ProductsPriceModel
    {
        public int Id { get; set; }
        public int CodeProducts { get; set; }
        public float Stock { get; set; }
        public string? Lote { get; set; }
        public DateTime? DateLote { get; set; }
        public decimal Price { get; set; }

        public ProductsModel Products { get; set; }
    }
}
