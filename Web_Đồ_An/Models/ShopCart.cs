namespace Web_Đồ_An.Models
{
    public class ShopCart
    {
        public int ProductId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int Quantity { get; set; }
        public int MaxQuantity { get; set; }
        public decimal Price { get; set; } //giá bán
		public decimal PriceSale { get; set; }//giá sale
		public decimal TotalPrice { get; set; } //giá bán cuối cùng 

    }
}
