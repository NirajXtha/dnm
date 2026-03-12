using NeoErp.Data;
using NeoErp.Models.QueryBuilder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Services
{
    public class QueryBuilder: IQueryBuilder
    {
        private IDbContext _dbContext;

        public QueryBuilder(IDbContext dbcontext)
        {
            this._dbContext = dbcontext;
        }

        public List<QueryBuilderModel> GetTransactionTable()
        {
            var QueryString = "select table_name as TableName,table_desc as TableNamesAlis  from TRANSACTION_TABLE_LIST";
            var tables = _dbContext.SqlQuery<QueryBuilderModel>(QueryString).ToList();
            return tables;
        }
        public List<TableColumns> GetTablesCoumnsByTableName(string tableName)
        {
            var queryString = @"SELECT table_name as tableName, column_name as columnsName, data_type as dataType, data_length as dataLength,initcap(replace(column_name,'_',' ')) columnheader
FROM USER_TAB_COLUMNS
WHERE table_name = '" + tableName + "'";
            var columns = _dbContext.SqlQuery<TableColumns>(queryString).ToList();
            return columns;
        }
        public List<NotificationBuilderModel> NotificationList()
        {
            var Query = @"SELECT ID NotificationId,NOTIFICATION_EDESC NotificationName,SQL_STATEMENT SqlQuery,NOTIFICATION_TYPE NotificationType,NOTIFICATION_RESULT NotificationResult,MINVALUES MinResult,MAXVALUES MaxResult,
                NOTIFICATION_TEMPLATE NotificationTemplate,APPEND_TEXT AppendText,APPEND_POSITION AppendPosition,NOTIFICATION_ICON Icon, NOTIFICATION_COLOR Color,TO_CHAR(USERPERMISSION) Users,MODULE_CODE ModuleCode
                FROM NOTIFICATIONS WHERE DELETED_FLAG='N' AND ISACTIVE='Y'";
            var data = _dbContext.SqlQuery<NotificationBuilderModel>(Query).ToList();
            return data;
        }

    }
}