using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.Ledger
{
   public class Accounts
    {
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
    }

    public class AccountTreeModel
    {
        public int Level { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string AccountTypeFlag { get; set; }
        public string MasterAccCode { get; set; }
        public string PreAccCode { get; set; }
        public string BranchCode { get; set; }
        public bool HasChildren { get; set; }
        public  Decimal Child_rec { get; set; }
        public IEnumerable<AccountTreeModel> Items { get; set; }
    }

    public class CustomerTreeModel
    {
        public int Level { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string AccountTypeFlag { get; set; }
        public string MasterCustomerCode { get; set; }
        public string PreCustomerCode { get; set; }
        public string BranchCode { get; set; }
        public bool HasChildren { get; set; }
        public Decimal Child_rec { get; set; }
        public IEnumerable<CustomerTreeModel> Items { get; set; }
    }

    public class EmployeeTreeModel
    {
        public int Level { get; set; }
        public string AccountName { get; set; }
        public string AccountCode { get; set; }
        public string AccountTypeFlag { get; set; }
        public string MasterAccCode { get; set; }
        public string PreAccCode { get; set; }
        public string BranchCode { get; set; }
        public bool HasChildren { get; set; }
        public Decimal Child_rec { get; set; }
        public IEnumerable<AccountTreeModel> Items { get; set; }
    }

    public class LedgerModel
    {
        public string LedgerName { get; set; }
        public string LedgerCode { get; set; }
        public string AccountTypeFlag { get; set; }
        public string MasterAccCode { get; set; }
        public string PreAccCode { get; set; }
        public string BranchCode { get; set; }
    }
}
