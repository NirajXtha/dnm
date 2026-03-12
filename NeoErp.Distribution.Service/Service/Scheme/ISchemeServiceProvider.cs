using NeoErp.Core.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Distribution.Service.Service.Scheme
{
    public interface ISchemeServiceProvider
    {

        /// <summary>
        /// PROCESSING WITHOUT IMAGE
        /// </summary>
        /// <param name="token"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        object SelectAction(JToken token, NeoErpCoreEntity dbContext);


        /// <summary>
        /// PROCESSING WITH IMAGE
        /// </summary>
        /// <param name="Form"></param>
        /// <param name="dbContext"></param>
        /// <param name="Files"></param>
        /// <returns></returns>
        object SelectAction(NameValueCollection Form, HttpFileCollection Files, NeoErpCoreEntity dbContext);

    }
}
