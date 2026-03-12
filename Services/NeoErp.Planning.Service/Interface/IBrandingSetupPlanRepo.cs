using NeoErp.Planning.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Interface
{
  public interface IBrandingSetupPlanRepo
    {
        List<ItemModel> getItem();
        List<ItemModel> getItemByCode(string planCode, string itemCode);
    }
}
