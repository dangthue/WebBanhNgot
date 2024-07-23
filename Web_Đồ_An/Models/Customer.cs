using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Web_Đồ_An.Models;

namespace Web_Đồ_An.Models
{
    [Table("Customer")]
    //[Index(nameof(Customer.CodeCustomer), IsUnique = true)]
    public class Customer
    {

        public Customer()
        {
            this.Order = new HashSet<Order>();
        }
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CustomerID { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public String Fullname { get; set; }
        public DateTime Brithday { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Avatar { get; set; }

        [Column(TypeName = "nvarchar(500)")]
        public string Address { get; set; }

        [Column(TypeName = "nvarchar(100)")]
        public string Email { get; set; }

        [Column(TypeName = "varchar(64)")]
        public string Phone { get; set; }
        public DateTime CreateDate { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(500)")]
        public string Password { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }

        //

        public ICollection<Favorite> Favorites { get; set; }

        public virtual ICollection<Order> Order { get; set; }

    }
}
