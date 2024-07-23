using System.ComponentModel.DataAnnotations;

namespace Web_Đồ_An.Models
{
    public class LoginViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = " Hãy nhập tên đăng nhập ")]

        public string Username { get; set; }
        /*[Required(ErrorMessage = "Hãy nhập Email ")]

        public string Email { get; set; }*/
        [DataType(DataType.Password), Required(ErrorMessage = " hãy nhập mật khẩu  ")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; }


    }
}
