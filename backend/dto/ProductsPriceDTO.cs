namespace backend.dto
{
    public class ProductsPriceDTO
    {
        public int Id { get; set; }
        public int CodeProducts { get; set; }
        public float Stock { get; set; }
        public string? Lote { get; set; }
        public DateTime? DateLote { get; set; }
        public decimal Price { get; set; }
    }
}
