using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using NeoERP.Planning.Models;
using System.Net;
using WebApi.OutputCache.V2;

namespace NeoERP.Planning.Controllers.Api
{
    public class MaterialPlanSetupApiController : ApiController
    {
        public IPlanSetup _iplanSetup { get; set; }
        public MaterialPlanSetupApiController(IPlanSetup iplanSetup)
        {
            this._iplanSetup = iplanSetup;
        }

        [CacheOutput(ClientTimeSpan =300,ServerTimeSpan =300)]

        public List<ItemModel> getItems()
        {
            var result = _iplanSetup.getItem();
            return result;
        }
        public List<ItemModel> getItemByCode(string planCode, string itemCode)
        {
            var result = _iplanSetup.getItemByCode(planCode, itemCode);
            var itemNodes = new List<ItemModel>();
            foreach (var item in result)
            {
                itemNodes.Add(new ItemModel()
                {
                    Level = item.Level,
                    ITEM_EDESC = item.ITEM_EDESC,
                    ITEM_CODE = item.ITEM_CODE,
                    GROUP_SKU_FLAG = item.GROUP_SKU_FLAG,
                    MASTER_ITEM_CODE = item.MASTER_ITEM_CODE,
                    PRE_ITEM_CODE = item.PRE_ITEM_CODE,
                    IS_CHILD_SELECTED = item.IS_CHILD_SELECTED,
                    hasChildren = item.GROUP_SKU_FLAG == "G" ? true : false
                });
            }
            return itemNodes;
        }

    
    }
}
