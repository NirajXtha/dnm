using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IParameterInspectionSetupRepo
    {
        List<ParameterInspectionSetup> GetItemCheckListDetails();
        List<ParameterInspectionSetup> GetItemCheckList();
        List<ParameterInspectionSetup> GetParameterInspectionList();
        bool InsertParameterInspectionSetupData(ParameterInspectionSetup data);
        ParameterInspectionSetup GetParameterInspectionById(string id);
    }
}
