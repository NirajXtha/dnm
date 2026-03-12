using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IPreDispatchInspectionRepo
    {
        List<Items> GetInvoiceNoList();
        PreDispatchInspection GetDispatchDetails(string InvoiceNo);
        List<PreDispatchInspection> GetPreDispatchInspectionCheckList();
        List<PreDispatchInspection> GetPreDispatchInspectionByIdCheckList(string predispatch);
        bool InsertPreDispatchInspectionData(PreDispatchInspection data);
        List<FormDetailSetup> GetPreDispatchInspectionList();
        List<PACKINGUNIT> GetPackingUnit();
       
        PreDispatchInspection GetEditPreDispatchInspection(string transactionno);
        PreDispatchInspection GetPreDispatchInspectionReport(string transactionno);
    }
}
