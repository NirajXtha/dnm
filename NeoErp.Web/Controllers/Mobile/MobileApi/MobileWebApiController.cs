
using NeoErp.Models.Mobiles;
using NeoErp.Services.MobileWeb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Mobile.MobileApi
{
    public class MobileWebApiController : ApiController
    {
        private IMobileWeb _iMobileWeb;
        public MobileWebApiController(IMobileWeb mobileWeb)
        {
            this._iMobileWeb = mobileWeb;
        }
        [HttpPost]
        public string saveUserWiseTaskFormat(RMPurchaseModel model)
        {
            try
            {
                var result = string.Empty;
                result = _iMobileWeb.saveUserWiseTaskFromat(model);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }
        [HttpPost]
        public string saveBRTaskMsgFormat(RMPurchaseModel model)
        {
            try
            {
                var result = string.Empty;
                result = _iMobileWeb.saveBRTaskMsgFormat(model);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public string saveFilterDataForMobile(MobileWebFilterModel model)
        {
            try
            {
                var result = string.Empty;
                result = _iMobileWeb.saveFilterDataForMobile(model);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<UserModelForMobile> GetAllUsers(string filter)
        {
            List<UserModelForMobile> result = _iMobileWeb.getAllUsers(filter);
            return result;
        }
        public List<ItemModelForMobileFilter> getAllItems(string filter)
        {
            return  _iMobileWeb.getAllItems(filter);
        }
        public List<ItemModelForMobileFilter> getSavedItems()
        {
            return _iMobileWeb.getAllSavedItems();
        }
        public List<MobileWebLogTaskModel> GetAllMobileWebLog()
        {
            var result = new List<MobileWebLogTaskModel>();
            result = _iMobileWeb.GetAllMobileWebLog();
            return result;
        }
    }
}
