using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Domain.Abstract
{
    public interface IUserSqlRepository
    {
        IEnumerable<User> Users { get; }

        bool CreateUser(User instance);

        bool UpdateUser(User instance);

        bool RemoveUser(int idUser);

        User GetUserByLogin(string login);

        User GetUserByEmail(string email);

        User GetUserByGuid(string guid);

        bool Login(string login, string password, out string msg, out int attemptCounter);
    }
}
