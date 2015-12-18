using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Banking.Domain.Concrete
{
    public partial class SqlRepository
    {
        public IEnumerable<User> Users
        {
            get { return Db.Users; }
        }

        public bool CreateUser(User instance)
        {
            if (instance.Id == 0)
            {
                instance.RegDate = DateTime.Now;
                instance.AttemptCounter = 0;
                instance.IsBlock = false;
                Db.Users.InsertOnSubmit(instance);
                Db.Users.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool UpdateUser(User instance)
        {
            User cache = Db.Users.Where(p => p.Id == instance.Id).FirstOrDefault();
            if (cache != null)
            {
                cache.RegDate = instance.RegDate;
                cache.Email = instance.Email;
                Db.Users.Context.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool RemoveUser(int idUser)
        {
            User instance = Db.Users.Where(p => p.Id == idUser).FirstOrDefault();
            if (instance != null)
            {
                Db.Users.DeleteOnSubmit(instance);
                Db.Users.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public User GetUser(string login)
        {
            return Db.Users.FirstOrDefault(p => string.Compare(p.Login, login, true) == 0);
        }

        [HandleError( View = "Error")]
        public bool Login(string login, string password, out string msg)
        {
            bool retVal = false;
            msg = null;
            User user = Db.Users.FirstOrDefault(p => string.Compare(p.Login, login, true) == 0);// && p.Password == password);
            if (user == null)
            {
                //msg = string.Format("User {0} don't found!", login);
                Logger.Log.InfoFormat("User {0} don't found!", login);
            }
            else
            {
                int attemptCounter = user.AttemptCounter ?? 0;
                if (user.IsBlock ?? false)
                {
                    msg = string.Format("User {0} is bloked!", user.Login);
                    attemptCounter++;
                }
                else if (user.Password.Trim() != password)
                {
                    if (attemptCounter++ > 4)
                    {
                        user.IsBlock = true;
                        msg = string.Format("User {0} bloked! Send email.", user.Login);
                    }
                    else
                    {
                        msg = string.Format("User {0} Login failed. Attemp {1}.", user.Login, attemptCounter);
                    }

                    Db.Users.Context.SubmitChanges();
                }
                else
                {
                    attemptCounter = 0;
                    retVal = true;
                }

                if (user.AttemptCounter != attemptCounter)
                {
                    user.AttemptCounter = attemptCounter;
                    Db.Users.Context.SubmitChanges();
                }
            }
            return retVal;
        }

    }
}
