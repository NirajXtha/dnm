using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models
{
    public class SubsidiaryModels
    {
    }

    public class MultiSelectModels {

        public string Name { get; set; }
        public string Code { get; set; }
        public string MasterCode { get; set; }
        public string PreCode { get; set; }
        public string LinkSubCode { get; set; }
        public Decimal Child_rec { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }
    public class TreeModels
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string MasterCode { get; set; }
        public string PreCode { get; set; }
        public string BranchCode { get; set; }
        public bool HasChildren { get; set; }
        public Decimal Child_rec { get; set; }
        public IEnumerable<TreeModels> Items { get; set; }
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
        //public IEnumerable<AccountTreeModel> Items { get; set; }
    }


}
