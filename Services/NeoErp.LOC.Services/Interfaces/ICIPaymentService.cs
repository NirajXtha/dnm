using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.LOC.Services.Interfaces
{
   public interface ICIPaymentService
    {
        List<CIPaymentSettlementModel> getAllCISettlement();
        CIPaymentSettlementModel AddUpdateCIPaymentSettlement(CIPaymentSettlementModel cIPaymentSettlementModel);
        void UpdateImage(LcImageModels lcimagedetail, string PSCODE);
        void RemoveCIPSImage(LcImageModels imagedetail);
        List<CIPaymentSettlementHistoryModel> getAllHistoryCIPaymentSettlementList(string lctrackno);
       // CIPaymentSettlementModel LoadCIInfo(string InvoiceCode,string SettlementDate);
        CIPaymentSettlementModel LoadCIInfo(string InvoiceCode);

    }
}
