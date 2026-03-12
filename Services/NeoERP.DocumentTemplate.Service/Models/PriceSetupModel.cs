using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class PriceSetupModel
    {
        public string ITEM_CODE { get; set; }

        public string ITEM_EDESC { get; set; }

        public int? OLD_PRICE { get; set; }

        public int? NEW_PRICE { get; set; }

        public string STATUS { get; set; }

        public bool Active { get; set; } = true;

        public string MU_EDESC { get; set; }

    }



    public class SaveModelForPriceSetup
    {
        public List<ChildField> ChildField { get; set; }

        public MasterField MasterField { get; set; }
    }



    public class MasterFieldForUpdate
    {
        public int MASTER_ID { get; set; }
        public string PRICE_LIST_NAME { get; set; }
        public int STATUS { get; set; }
        public string COMPANY { get; set; }
        public DateTime DATE_ENGLISH { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime MODIFIED_DATE { get; set; }

    }
    public class MasterField
    {
        public int MASTER_ID { get; set; }
        public string PriceListName { get; set; }

        public string PRICE_LIST_NAME { get; set; }
        public string CompanyName { get; set; }
        public DateTime DateEnglish { get; set; }
        public DateTime DateNepali { get; set; }
        public bool Status { get; set; }

        public string COMPANY { get; set; }

        public bool isUpdated { get; set; }

    }

    public class ChildField
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string OLD_PRICE { get; set; }
        public string NEW_PRICE { get; set; }
        public string STATUS { get; set; }
        public string Active { get; set; }
        public string MU_EDESC { get; set; }

    }
    public class PriceList
    {
        public int MASTER_ID { get; set; }
        public string PRICE_LIST_NAME { get; set; }
        public string DEFAULT_SHOW { get; set; }


    }
}
