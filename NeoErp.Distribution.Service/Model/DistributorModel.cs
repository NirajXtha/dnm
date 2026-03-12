using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
  public  class DistributorModel
    {
        public string DISTRIBUTOR_CODE { get; set; }
        public string REG_OFFICE_ADDRESS { get; set; }
        public string CONTACT_NO { get; set; }
        public string EMAIL { get; set; }
        public string PAN_NO { get; set; }
        public string VAT_NO { get; set; }
        //public int CREATED_BY { get; set; }
        //public int LUPDATE_BY { get; set; }
        //public DateTime? CREATED_DATE { get; set; }
        //public DateTime? LUPDATE_DATE { get; set; }
        public string ACTIVE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string COMPANY_CODE { get; set; }




    }

    public class NewPartyTypeModel
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
    }

    
    
}
