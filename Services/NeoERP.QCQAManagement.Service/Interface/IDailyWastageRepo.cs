using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
    public interface IDailyWastageRepo
    {
        List<FormDetailSetup> GetDailyWastageList();
        List<Items> GetMaterialGroupLists();
        bool InsertDailyWastage(DailyWastage data);
        DailyWastage GetEditDailyWastage(string transactionno);
        DailyWastage GetDailyWastageReport(string transactionno);
    }
}
