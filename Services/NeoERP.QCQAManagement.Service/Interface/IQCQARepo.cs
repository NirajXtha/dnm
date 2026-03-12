using NeoErp.Core.Domain;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Interface
{
   public interface IQCQARepo
    {
        List<FormDetailSetup> GetQCQADetails(string TableName);
        List<TableList> GetTableLists();
        List<FormDetailSetup> GetQCQADetailsByTableName(string tableName);
        string AddColumnsToTable(List<FormDetailSetup> modal, string tableName);
        List<FormSetupModel> GetFormCode(User userIndo, string tableName);
        List<FormDetailSetup> GetQCFormDetailSetup(string formCode);
    }
}
