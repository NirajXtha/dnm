using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Domain
{
    public class UserInformation
    {
        public UserInformation()
        {
            UserGuid = Guid.NewGuid();
        }

        public string UserName { get; set; }
        public string UserId { get; set; }
        public Guid UserGuid { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
        public string FNyear { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
    }
}