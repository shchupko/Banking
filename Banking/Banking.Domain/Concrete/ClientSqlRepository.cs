using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Domain.Abstract;

namespace Banking.Domain.Concrete
{
    public class ClientSqlRepository : IClientSqlRepository
    {
        public BankingDbDataContext Db = new BankingDbDataContext();

        public IEnumerable<Client> Clients
        {
            get
            {
                var clientes = Db.Clients.ToList();
                foreach (var c in clientes)
                {
                    c.Firstname = c.Firstname.Trim();
                    c.Lastname = c.Lastname.Trim();
                    c.Phone = c.Phone.Trim();
                    c.Status = c.Status.Trim();
                }
                return clientes;
            }
        }

        public bool CreateClient(Client instance)
        {
            if (instance.ContactNumber == 0)
            {
                Db.Clients.InsertOnSubmit(instance);
                Db.Clients.Context.SubmitChanges();
                return true;
            }

            return false;
        }

        public bool UpdateClient(Client instance)
        {
            Client cache = Db.Clients.Where(p => p.ContactNumber == instance.ContactNumber).FirstOrDefault();
            if (cache != null)
            {
                Db.Clients.DeleteOnSubmit(cache);
                Db.Clients.InsertOnSubmit(instance);
                Db.Clients.Context.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool RemoveClient(int idClient)
        {
            Client instance = Db.Clients.Where(p => p.ContactNumber == idClient).FirstOrDefault();
            if (instance != null)
            {
                Db.Clients.DeleteOnSubmit(instance);
                Db.Clients.Context.SubmitChanges();
                return true;
            }

            return false;
        }
    }
}
