namespace Model
{
    public class Product
    {
        public string? ProductTitle { get; set; }
        public string? ProductCode { get; set; }
        public decimal ProductPrice { get; set; }
        public bool IsActive { get; set; }
        public string? ImageUrl { get; set; } // Add this line

        public bool IsSelected { get; set; }
    }
}
