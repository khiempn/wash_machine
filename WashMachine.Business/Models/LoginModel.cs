using System.ComponentModel.DataAnnotations;

namespace WashMachine.Business.Models
{
    public class LoginModel
    {
        [Required]
        public string Username { get; set; }

        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        public string CaptchaCode { get; set; }

        public string ReturnUrl { get; set; }

    }
}
