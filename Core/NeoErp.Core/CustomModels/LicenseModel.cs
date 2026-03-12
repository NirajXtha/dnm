using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Models
{
    public class LicenseModel
    {
        public LicenseModel()
        {

        }

        public string ProductSrNo { get; set; }
        public int LicenseType  { get; set; }
        public string LicenseTo { get; set; }
      
    }
}