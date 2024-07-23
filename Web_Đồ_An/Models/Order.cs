using Web_Đồ_An.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Web_Đồ_An.Models
{
    [Table("tb_Order")]
    public class Order : CommonAbstract
    {
		public Order()
        {
            this.OrderDetails = new HashSet<OrderDetail>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int OderId { get; set; }
    
        [Required]
        public string Code { get; set; }
        [Required(ErrorMessage = "Tên khách hàng không để trống")]
        public string NameReciver { get; set; }
        [Required(ErrorMessage = "Số điện thoại không để trống")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Địa chỉ khổng để trống")]
        public string Address { get; set; }

        public decimal TotalAmount { get; set; }
        public int Quantity { get; set; }
        public int TypePayment { get; set; }

        public string Notes { get; set; }
       
        public int CustomerID { get; set; }


        public virtual ICollection<OrderDetail> OrderDetails { get; set; }

        public virtual Customer Customer { get; set; }
    }
}
