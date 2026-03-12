using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Services
{
   public class SalesProcessingMoniteringService: ISalesProcessingMoniteringService
    {
        private NeoErpCoreEntity _objectEntity;
         string defaultdate = "2015-7-31";
        public SalesProcessingMoniteringService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
       public int GetTodaySalesOrder(string date, string company_code = "01")
        {
            defaultdate = date;
            var query = string.Format("SELECT COUNT(distinct ORDER_NO) FROM SA_SALES_ORDER WHERE ORDER_DATE=TO_DATE('{0}', 'YYYY-MM-DD') AND COMPANY_CODE = '{1}' AND DELETED_FLAG = 'N'", defaultdate,company_code);
            var ToalSalesOrder = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            //   purchaseRegisters.Select);
            return ToalSalesOrder;
        }
       public int GetTodayApprovedsalesOrder(string date, string company_code = "01")
        {
            defaultdate= date;
            var query = string.Format(@"SELECT COUNT(distinct A.ORDER_NO) FROM SA_SALES_ORDER A, MASTER_TRANSACTION B
WHERE A.ORDER_NO = B.VOUCHER_NO
AND A.FORM_CODE = B.FORM_CODE
AND A.COMPANY_CODE = B.COMPANY_CODE
AND A.ORDER_DATE = TO_DATE('{0}', 'YYYY-MM-DD')
AND A.COMPANY_CODE = '" + company_code+"' AND A.DELETED_FLAG = 'N' AND B.AUTHORISED_BY IS NOT NULL", defaultdate);
            var ToalSalesOrder = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            //   purchaseRegisters.Select);
            return ToalSalesOrder;
        }
        public int GetTodayVehicleRegister(string date, string company_code = "01")
        {
            defaultdate = date;
            var query = string.Format(@"SELECT COUNT(distinct TRANSACTION_NO) FROM IP_VEHICLE_TRACK
WHERE TRANSACTION_DATE=TO_DATE('{0}', 'YYYY-MM-DD')
AND COMPANY_CODE='" + company_code+"' AND DELETED_FLAG='N'", defaultdate);
            var ToalSalesOrder = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            //   purchaseRegisters.Select);
            return ToalSalesOrder;
        }
        public int GetTodayLoadingSlipGenerate(string date, string company_code = "01")
        {
            defaultdate = date;
            var query = string.Format(@"SELECT COUNT(distinct LS_NO) FROM SA_LOADING_SLIP_DETAIL
WHERE VOUCHER_DATE=TO_DATE('{0}', 'YYYY-MM-DD')
AND COMPANY_CODE='" + company_code+"' AND DELETED_FLAG='N'", defaultdate);
            var ToalSalesOrder = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            //   purchaseRegisters.Select);
            return ToalSalesOrder;
        }
        public int GetTodayLoadedVehicleOut(string date, string company_code = "01")
        {
            defaultdate = date;
            var query = string.Format(@"SELECT COUNT(distinct LS_NO) FROM SA_LOADING_SLIP_DETAIL
WHERE VOUCHER_DATE=SYSDATE
AND COMPANY_CODE='" + company_code+@"' AND DELETED_FLAG='N'
AND READ_FLAG='Y' AND VEHICLE_OUT_DATE=TO_DATE('{0}', 'YYYY-MM-DD')", defaultdate);
            var ToalSalesOrder = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            //   purchaseRegisters.Select);
            return ToalSalesOrder;
        }
        public int GetTodayPendingForDispatch(string date, string company_code = "01")
        {
            defaultdate = date;
            var query = string.Format(@"SELECT COUNT(SALES_NO) FROM SA_SALES_INVOICE
WHERE SALES_DATE=TO_DATE('{0}', 'YYYY-MM-DD')
AND COMPANY_CODE='"+company_code+"' AND DELETED_FLAG='N'", defaultdate);
            var ToalSalesOrder = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            //   purchaseRegisters.Select);
            return ToalSalesOrder;
        }

        public int GetTodayDistributionPurchaseOrderCount(string date,string company_code="01")
        {
            var query = $@"select count(distinct order_no) poorderdate from DIST_IP_SSD_PURCHASE_ORDER where deleted_flag='N'  
                            and approved_flag='Y' and company_code='{company_code}'  and order_date=TO_DATE('{date}', 'YYYY-MM-DD')";
            var totaldata = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            return totaldata;
        }

        public int GetDispatchMangement(string date,string company_code="01")
        {
            var query = $@"select count(distinct order_no) poorderdate from DIST_IP_SSD_PURCHASE_ORDER where deleted_flag='N'  
                            and approved_flag='Y' and company_code='{company_code}'  and order_date=TO_DATE('{date}', 'YYYY-MM-DD')";
            var totaldata = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            return totaldata;
        }
        public int GetVechicalIN(string date, string company_code = "01")
        {
            var query = $@"select count(distinct vehicle_name)
                             from IP_GATE_ENTRY  where company_code='{company_code}' and deleted_flag='N'  and gate_date=TO_DATE('{date}', 'YYYY-MM-DD')";
            var totaldata = _objectEntity.SqlQuery<int>(query).FirstOrDefault();
            return totaldata;
        }
    }
}
