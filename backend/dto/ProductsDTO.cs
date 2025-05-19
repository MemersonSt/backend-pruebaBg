namespace backend.dto
{
    public class ProductsDTO
    {
        public int Code { get; set; }
        public string CodeProducts { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Presentation { get; set; }
        public string Category { get; set; }
        public bool IsService { get; set; }
        public int Stock { get; set; }
        public decimal Price { get; set; }
        public bool IsLote { get; set; }
        public string? Lote { get; set; }
        public DateTime? DateLote { get; set; }
        public bool IsActive { get; set; }
        public string? State { get; set; }
        public DateTime Date { get; set; }
        public int User { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public List<ProductsPriceDTO> Prices { get; set; }
    }
}
