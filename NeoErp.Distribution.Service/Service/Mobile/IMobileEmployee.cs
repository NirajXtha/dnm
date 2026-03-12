using NeoErp.Core.Domain;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public interface IMobileEmployee
    {
        List<SalesPersonModel> getSalesPersonList();
    }
}
