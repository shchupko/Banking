using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Banking.Domain.Abstract;


namespace Banking.Domain.Concrete
{
    public partial class SqlRepository: IRepository
    {
        //public BankingDbDataContext Db { get; set; }
        public BankingDbDataContext Db = new BankingDbDataContext();
    }
}
