using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Integration
{
    public interface IPublicPageHandler
    {
        ArrayList AllowedPath { get; set; }
    }
}