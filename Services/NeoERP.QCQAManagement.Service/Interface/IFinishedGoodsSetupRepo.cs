using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IFinishedGoodsSetupRepo
    {
        List<ParameterInspectionSetup> GetFinishedItemCheckListDetails();
        List<ParameterInspectionSetup> GetFinishedItemCheckList();
        List<ParameterInspectionSetup> GetFinishedInspectionList();
        bool InsertFinishedGoodsSetupData(ParameterInspectionSetup data);
        ParameterInspectionSetup GetFinishedGoodsById(string id);
    }
}
