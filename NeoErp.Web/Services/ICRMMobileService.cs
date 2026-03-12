using NeoErp.Core.Models.CustomModels;
using NeoErp.Models.Mobiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services
{
    public interface ICRMMobileService
    {
        List<CRMMobileModel> GetCRMData();
    }
}
