using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model.Mobile;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public interface IActionSelector
    {
        object SelectAction(JToken token, NeoErpCoreEntity dbContext);
        object SelectAction(NameValueCollection Form, HttpFileCollection Files, NeoErpCoreEntity dbContext);
    }
}