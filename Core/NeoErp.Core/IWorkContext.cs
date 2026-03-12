using NeoErp.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core
{
    public interface IWorkContext
    {
         User CurrentUserinformation { get; set; }

    }
}