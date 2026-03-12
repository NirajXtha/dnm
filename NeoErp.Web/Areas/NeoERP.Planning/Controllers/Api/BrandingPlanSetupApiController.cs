using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.Planning.Controllers.Api
{
    public class BrandingPlanSetupApiController : ApiController
    {
        public IBrandingSetupPlanRepo _IBrandingSetupPlanRepo { get; set; }
        public BrandingPlanSetupApiController(IBrandingSetupPlanRepo IBrandingSetupPlanRepo)
        {
            this._IBrandingSetupPlanRepo = IBrandingSetupPlanRepo;
        }
        public List<ItemModel> getItems()
        {
            var result = _IBrandingSetupPlanRepo.getItem();
            return result;
        }
        public List<ItemModel> getItemByCode(string planCode, string itemCode)
        {
            var result = _IBrandingSetupPlanRepo.getItemByCode(planCode, itemCode);
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
