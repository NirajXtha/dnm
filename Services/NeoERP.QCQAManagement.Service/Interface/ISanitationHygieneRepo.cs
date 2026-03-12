using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface ISanitationHygieneRepo
    {
        List<FormDetailSetup> GetSanitationHygieneList();
        List<SanitationHygiene> GetMasterSanitationHygiene();
        List<SanitationHygiene> GetAllSanitationHygieneDetails();
        List<SanitationHygiene> GetSanitationHygieneDetails(string LocationCode);
        bool InsertSanitationHygieneData(SanitationHygiene data);
        List<ChildModel> GetSanitationHygieneDetailsReport(string frmDate, string toDate);
        SanitationHygiene GetEditSanitationHygiene(string transactionno);
        SanitationHygiene GetSanitationHygieneReport(string transactionno);
    }
}
