using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Banking.Domain.Models.ViewModels
{
    public class UserRegisterView
    {
        [Required(ErrorMessage = "Enter Login")]
        public string Login { get; set; }        

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords don't same")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }

        public string Address { get; set; }

        [Required(ErrorMessage = "Enter Captcha")]
        public string Captcha { get; set; }
    }

    public class UserLoginView
    {
        [Required(ErrorMessage = "Enter Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotPasswordView
    {
        [Required(ErrorMessage = "Enter email")]
        public string Email { get; set; }
    }
}