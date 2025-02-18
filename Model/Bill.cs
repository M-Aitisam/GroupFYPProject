namespace Model
{
    public class Bill
    {
        public int Id { get; set; }
        public string ClientName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string BarberName { get; set; } = string.Empty;
        public List<BillItem> Items { get; set; } = new List<BillItem>();
        public decimal TotalAmount { get; set; }
    }

    public class BillItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}

