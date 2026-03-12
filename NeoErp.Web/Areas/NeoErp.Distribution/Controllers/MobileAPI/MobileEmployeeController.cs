using NeoErp.Core;
using NeoErp.Distribution.Service.Model;
using NeoErp.Distribution.Service.Service.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Distribution.Controllers.MobileAPI
{
    [RoutePrefix("api/MobileEmployee/")]
    public class MobileEmployeeController : ApiController
    {
        public IMobileEmployee _mobilePoService;
        public MobileEmployeeController(IMobileEmployee MobilePOService)
        {
            _mobilePoService = MobilePOService;
        }
        public List<SalesPersonModel> GetSalePerson()
        {
            var data = _mobilePoService.getSalesPersonList();
            return data==null?new List<SalesPersonModel>():data;
        }

    }
}