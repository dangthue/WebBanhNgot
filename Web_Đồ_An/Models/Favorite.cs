using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Web_Đồ_An.Models
{
    [Table("Favorite")]
    public class Favorite
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int FavoriteId { get; set; }
        public string Name { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }
        // khoá ngoại
        public int ProductId { get; set; }
        public int CustomerID { get; set; }
        public Product Product { get; set; }
        public Customer Customer { get; set; }

    }
}
