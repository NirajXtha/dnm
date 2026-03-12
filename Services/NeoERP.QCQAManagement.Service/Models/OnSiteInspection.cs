using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class OnSiteInspection
    {
        public string Inspection_No { get; set; }
        public string Plant_Id { get; set; }
        public string Product_Id { get; set; }
        public string ITEM_CODE { get; set; }
        public string Batch_No { get; set; }
        public string Shift { get; set; }
        public string REMARKS { get; set; }
        public string Reference_No { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SYN_ROWID { get; set; }
        public string PARAM_CODE { get; set; }
        public string SERIAL_NO { get; set; }
        public string SUPPLIER_CODE { get; set; }
        public string DISPATCH_PERSON { get; set; }  //HERE kept in Vendor Name
        public List<ParameterDetails> ParameterDetailsList { get; set; }
        public List<string> ITEMSETUPS { get; set; }
        public List<PRODUCTSLIST> PRODUCTS { get; set; }
        public List<INTERNALPRODUCTLIST> INTERNAL_PRODUCTS { get; set; }
        public List<InternalItemSetup> InternalItemSetupList { get; set; }
    }
    public class INTERNALPRODUCTLIST
    {
        public string PRODUCT_TYPE { get; set; }
        public string PRODUCT { get; set; }
    }

}
