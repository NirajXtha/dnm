using NeoErp.Models.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services
{
   public interface IQueryBuilder
    {

        List<QueryBuilderModel> GetTransactionTable();
        List<TableColumns> GetTablesCoumnsByTableName(string tableName);
        List<NotificationBuilderModel> NotificationList();


    }
}
