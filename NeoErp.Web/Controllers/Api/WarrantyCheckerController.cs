using NeoErp.Models.WarrantyChecker;
using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class WarrantyCheckerController : ApiController
    {
        IWarrantyChecker _warrantyChecker;
        public WarrantyCheckerController(IWarrantyChecker _warrantyChecker)
        {
            this._warrantyChecker = _warrantyChecker;
        }
        //public List<WarrantyChekerModel> GetWarrantyInfo()
        //{
        //    var serialNo = "111111";
        //    return this._warrantyChecker.GetWarrantyInfo(serialNo);
        //}

        //public List<WarrantyChekerModel> GetWarrantyInfo(string serialNo) {

        //    return this._warrantyChecker.GetWarrantyInfo(serialNo);
        //}
    }
}
