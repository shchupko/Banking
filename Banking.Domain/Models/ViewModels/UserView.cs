using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Banking.Domain.Models.ViewModels
{
    public class UserRegisterView : BaseView
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

        public bool SkipEmailConfirmation { get; set; }
    }

    public class UserLoginView : BaseView
    {
        [Required(ErrorMessage = "Enter Login")]
        public string Login { get; set; }

        //s[Display(Name = "Date published")]
        [Required(ErrorMessage = "Enter Password")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

    }

    public class BaseView
    {
        public int minL = 4;
        public int maxL = 8;
        public int BlockedAttemptNumber = 5;
    }
}