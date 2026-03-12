using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class CostCenterMappingModal
    {
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string TRANSACTION_TYPE { get; set; }
        public string TPB_FLAG { get; set; }
        public string ACC_TYPE_FLAG { get; set; }
        public string MASTER_ACC_CODE { get; set; }
        public string PRE_ACC_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public bool HAS_BRANCH { get; set; }
        public List<CostCenterMappingModal> ITEMS { get; set; }
    }
    public class CostCenterMapDetail
    {
        public string DESCRIPTION { get; set; }
        public string SUB_CODE { get; set; }
        public string ACC_CODE { get; set; }
        public string ACC_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string BRANCH_CODE { get; set; }
        public string SUB_LEDGER_TYPE { get; set; }
    }
}
