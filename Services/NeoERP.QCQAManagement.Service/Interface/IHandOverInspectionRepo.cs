using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IHandOverInspectionRepo
    {
        List<FormDetailSetup> GetHandOverInspectionList();
        List<PACKINGUNIT> GetPackingUnit();
        bool InsertHandOverInspectionData(HandOverInspection data);
        HandOverInspection GetEditHandOverInspection(string transactionno);
        HandOverInspection GetHandOverInspectionReport(string transactionno);
    }
}
