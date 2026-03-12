using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class DistributorItemResponseModel
    {
        public DistributorItemResponseModel()
        {
            PARTY = new Dictionary<string, EntityResponseModel>();
            UNIT = new Dictionary<string, Dictionary<string, MuCodeResponseModel>>();
            SALES_TYPE = new Dictionary<string, List<SalesTypeModel>>();
            SHIPPING_ADDRESS = new List<ShippingAddressModel>();
        }
        public Dictionary<string, EntityResponseModel> PARTY { get; set; }
        public Dictionary<string, Dictionary<string, MuCodeResponseModel>> UNIT { get; set; }
        public Dictionary<string, List<SalesTypeModel>> SALES_TYPE { get; set; }
        public List<ShippingAddressModel> SHIPPING_ADDRESS { get; set; }
    }

    public class DistributorItemModel
    {
        public string DISTRIBUTOR_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
    }
}
