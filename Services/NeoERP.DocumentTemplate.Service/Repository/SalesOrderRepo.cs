using NeoERP.DocumentTemplate.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoERP.DocumentTemplate.Service.Models;
using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Core.Caching;



namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class SalesOrderRepo : ISalesOrderRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private ICacheManager _cacheManager;
        public SalesOrderRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
        }

        public List<SalesOrderDetailView> GetSalesOrderDetails()
        {

            string Query = $@"select * from(SELECT DISTINCT ORDER_NO,FORM_CODE,ORDER_DATE,CREATED_BY,CREATED_DATE,MODIFY_BY,MODIFY_DATE FROM SA_SALES_ORDER order by order_date desc) where rownum<=50 order by rownum";
           
            List<SalesOrderDetailView> entity = this._dbContext.SqlQuery<SalesOrderDetailView>(Query).ToList();
            return entity;
        }

        public List<SalesOrderDetail> GetSalesOrderDetailsByOrderNo(string orderno)
        {
            string Query = $@"SELECT ITEM_CODE,FN_FETCH_DESC(COMPANY_CODE,'IP_ITEM_MASTER_SETUP',ITEM_CODE) ITEM_NAME,MU_CODE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE FROM SA_SALES_ORDER WHERE ORDER_NO='{orderno}'";
            List<SalesOrderDetail> entity = this._dbContext.SqlQuery<SalesOrderDetail>(Query).ToList();
            return entity;
        }
    }
}
