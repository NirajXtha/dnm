using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IOnSiteInspectionRepo
    {
        List<FormDetailSetup> GetOnSiteInspectionList();
        List<BatchDetails> GetBatchNoByItemCode(string itemCode);
        List<ParameterDetails> GetParameterDetailsByPlant(string Plant);
        bool InsertOnSiteInspectionData(OnSiteInspection data);
        OnSiteInspection GetEditOnSiteInspection(string transactionno);
        OnSiteInspection GetOnSiteInspectionReport(string transactionno);

    }
}
