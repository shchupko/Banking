using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Banking.Domain.Abstract;
using Banking.Domain.Mail;
using Ninject.Activation;

namespace Banking.Domain.Concrete
{
    public class UserSqlRepository : IUserSqlRepository
    {
        public BankingDbDataContext Db;
        INotifyMail mailProvider;

        public UserSqlRepository(BankingDbDataContext db = null, INotifyMail mail = null)
        {
            if (db == null)
            {
                Db = new BankingDbDataContext();
            }
            else
            {
                Db = db;
            }

            mailProvider = mail;
        }

        public IEnumerable<User> Users
        {
            get { return Db.Users; }
        }

        public static string GetGuid()
        {
            return Guid.NewGuid().ToString("N");
        }

        public bool CreateUser(User instance)
        {
            if (instance.Id == 0)
            {
                instance.RegDate = DateTime.Now;
                instance.AttemptCounter = 0;
                instance.IsBlock = false;
                instance.Guid = GetGuid(); 
                instance.isConfirmedEmail = false;
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

        public User GetUserByLogin(string login)
        {
            return Db.Users.FirstOrDefault(p => string.Compare(p.Login, login, true) == 0);
        }

        public User GetUserByEmail(string email)
        {
            return Db.Users.FirstOrDefault(p => string.Compare(p.Email, email, true) == 0);
        }

        public User GetUserByGuid(string guid)
        {
            return Db.Users.FirstOrDefault(p => string.Compare(p.Guid, guid, true) == 0);
        }

        [HandleError( View = "Error")]
        public bool Login(string login, string password, out string msg, out int attemptCounter)
        {
            bool retVal = false;
            msg = null;
            attemptCounter = 0;
            User user = Db.Users.FirstOrDefault(p => string.Compare(p.Login, login, true) == 0);// && p.Password == password);
            if (user == null)
            {
                msg = string.Format("User {0} don't found!", login);
                Logger.Log.InfoFormat("User {0} don't found!", login);
            }
            else
            {
                attemptCounter = user.AttemptCounter ?? 0;
                if (user.IsBlock)
                {
                    msg = string.Format("User {0} is bloked!", user.Login);
                    attemptCounter++;
                }
                else if (!user.isConfirmedEmail)
                {
                    msg = string.Format("Please confirm email.");
                    Logger.Log.DebugFormat("UserSqlRepository.Login(). User {0} try login with unconfirmed Email {1}", user.Login, user.Email);
                }
                else if (user.Password.Trim() != password)
                {
                    if (attemptCounter++ > 4)
                    {
                        user.IsBlock = true;
                        msg = string.Format("User {0} bloked!", user.Login);
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
