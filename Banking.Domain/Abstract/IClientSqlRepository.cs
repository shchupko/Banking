using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Banking.Domain.Abstract
{
    public interface IClientSqlRepository
    {
        IEnumerable<Client> Clients { get; }

        bool CreateClient(Client instance);

        bool UpdateClient(Client instance);

        bool RemoveClient(int idUser);
    }
}
