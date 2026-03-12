using NeoErp.Models.WarrantyChecker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services
{
    public interface IWarrantyChecker
    {
        WarrantyChekerModel GetWarrantyInfo(string serialNo);

        string SaveWarrantyMsgService(DefectMessage param);

        List<DefactModel> GetDefact();
    }
}
