using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WashMachine.Business.Models
{
    public class ResetPasswordModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(3)]
        public string NewPassword { get; set; }
        [Required]
        [MinLength(3)]
        [Compare("NewPassword")]
        public string RetypePassword { get; set; }
        public string Data { get; set; }
    }
}
