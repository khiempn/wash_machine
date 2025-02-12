using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WashMachine.Business.Models
{
    public class ChangePasswordModel
    {
        public string Username { get; set; }

        public string Email { get; set; }

        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; }

        [Compare("NewPassword")]
        public string RetypePassword { get; set; }
    }
}
