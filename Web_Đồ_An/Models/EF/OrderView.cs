using System.ComponentModel.DataAnnotations.Schema;


namespace Web_Đồ_An.Models.EF
{
    public class OrderView
    {
        public int OrderId { get; set; }
        public string CodeOrder { get; set; }

        public DateTime OrderDate { get; set; }
        public decimal TotalMoney { get; set; }

        public string ReceiveName { get; set; }

        public string ReceiveAddress { get; set; }

        public string ReceivePhone { get; set; }

        public string Notes { get; set; }
        //khoá ngoại
        public string CustomerName { get; set; }
        public string TransactStatus { get; set; }
        public int PaymentName { get; set; }
    }
}
