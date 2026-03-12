using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Models
{
    public class ProductSetup
    {
        public string ITEM_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
    }
    public class ItemSetup
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class InternalItemSetup //used for internal inspection
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string QUANTITY { get; set; }
    }
    public class ItemDetails
    {
        public string PARAM_CODE { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string TYPE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class ProductDetails
    {
        public string ITEM_CODE { get; set; }
        public string PARAM_CODE { get; set; }
        public string PARAMETERS { get; set; }
        public string TARGET { get; set; }
        public string TOLERENCE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MODIFY_BY { get; set; }
        public string SYN_ROWID { get; set; }
        public string PRODUCT_TYPE { get; set; }
        public string TYPE { get; set; }
        public string PRODUCT_NAME { get; set; }
        public List<string> ITEMSETUPS { get; set; }
        public List<ParameterDetails> ParameterDetailsList { get; set; }
    }
    public class ParameterDetails
    {
        public string ITEM_CODE { get; set; }
        public string PARAMETER_ID { get; set; }
        public string TARGET { get; set; }
        public string TOLERENCE { get; set; }
        public string PARAMETERS { get; set; }
        public string SPECIFICATION { get; set; }
        public string UNIT { get; set; }
        public string PARAM_CODE { get; set; }
        public string SERIAL_NO { get; set; }
        public string RESULTS { get; set; }
        public string VARIANCE { get; set; }
    }
}
