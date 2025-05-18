namespace backend.model
{
    public class ProductsModel
    {
        public int Code { get; set; }
        public string CodeProducts { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Presentation { get; set; }
        public string Category { get; set; }
        public bool IsService { get; set; }
        public bool IsLote { get; set; }
        public bool IsActive { get; set; }
        public string State { get; set; }
        public int User { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public ICollection<ProductsPriceModel>? Prices { get; set; }
    }
}
