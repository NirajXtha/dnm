using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Integration
{
    public class PublicPageHandler:IPublicPageHandler
    {   
        public PublicPageHandler()
        {
            AllowedPath = new ArrayList();
            AllowedPath.Add("/Security/Account/Login");
            AllowedPath.Add("/Security/Account/AdminLogin");
            AllowedPath.Add("/Security/Account/ForgotPassword");
        }

        public ArrayList AllowedPath { get; set; }
    }
}