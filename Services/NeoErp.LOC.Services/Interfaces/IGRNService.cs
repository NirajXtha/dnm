using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Interfaces
{
   public interface IGRNService
    {
         List<LcGrnModel> getAllGRN();
         string AddUpdateGRN(LcGrnModel lcGrnModel);
        // LcGrnModel LoadCIInfo(string InvoiceCode,string PPDate);
         LcGrnModel LoadCIInfo(string InvoiceCode);
    }
}
