using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Web_Đồ_An.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = " Hãy nhập tên  ")]

        public string Username { get; set; }
        [Required(ErrorMessage = "Hãy nhập Email ")]

        public string Email { get; set; }
        [DataType(DataType.Password),Required(ErrorMessage = " hãy nhập mật khẩu ")]
        public string Password { get; set; }


    }
}
