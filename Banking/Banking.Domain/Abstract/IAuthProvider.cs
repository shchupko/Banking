using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Banking.Domain.Models.ViewModels;

namespace Banking.Domain.Abstract
{
    public interface IAuthProvider
    {
        //User CurrentUser { get; }        
        //HttpContext HttpContext { get; set; }
        bool Authenticate(UserLoginView model, out string msg);
        void SignOut();

    }
}
