using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;




namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class ContraVoucherRepo : IContraVoucher
    {
        private NeoErpCoreEntity _coreEntity;
        IDbContext _dbContext;
        IWorkContext _workContext;
        public ContraVoucherRepo(NeoErpCoreEntity coreEntity, IDbContext dbContext, IWorkContext workContext)
        {
            _coreEntity = coreEntity;
            this._dbContext = dbContext;
            this._workContext = workContext;
        }
        public List<SalesOrderDetailView> GetAllOptionsList()
        {
            //var result = _coreEntity.FORM_SETUP.Where(x=>x.DELETED_FLAG == "N" && x.GROUP_SKU_FLAG == "I" && x.COMPANY_CODE == "01" (_coreEntity.FORM_DETAIL_SETUP.Contains().Where(y=>y.TABLE_NAME == "IP_PURCHASE_INVOICE" || y.TABLE_NAME == "SA_SALES_INVOICE"))

            return null;
        }
        public List<SalesOrderDetailView> GetSalesOrderDetails()
        {

            string Query = $@"select * from(SELECT DISTINCT ORDER_NO,FORM_CODE,ORDER_DATE,CREATED_BY,CREATED_DATE,MODIFY_BY,MODIFY_DATE FROM SA_SALES_ORDER order by order_date desc) where rownum<=50 order by rownum";

            //List<SalesOrderDetailView> entity = this._dbContext.SqlQuery<SalesOrderDetailView>(Query).ToList();
            return null;
        }
        public List<FinanceVoucherReference> GetFinanceVoucherReferenceList(string formcode)
        {
            string Query = $@"  select  f.voucher_no,f.reference_form_code ,FS.FORM_EDESC  from FINANCIAL_REFERENCE_DETAIL  f ,
                form_setup  fs
               where fs.company_code=f.company_code
               and f.deleted_flag=fs.deleted_flag
               and f.form_code=fs.form_code
               and f.company_code='{_workContext.CurrentUserinformation.company_code}'
               and f.form_code=" + formcode;

            List<FinanceVoucherReference> entity = this._dbContext.SqlQuery<FinanceVoucherReference>(Query).ToList();
            return entity;

        }
      public List<FVPURCHASEEXPSHEETRERERENCE> GetFVReferencePurchaseexpsheet(string voucherno)
        {
            string Query = $@"  SELECT distinct c.reference_no,f.form_edesc FROM charge_transaction c,form_setup f
              WHERE f.form_code = c.form_code AND f.company_code = c.company_code             
               and c.company_code='{_workContext.CurrentUserinformation.company_code}'
               and c.voucher_no='{voucherno}'" ;
            List<FVPURCHASEEXPSHEETRERERENCE> entity = this._dbContext.SqlQuery<FVPURCHASEEXPSHEETRERERENCE>(Query).ToList();
            return entity;

        }
        public int? GetTotalVoucher(string form_code, string table_name)
        {

            string Query = $@"SELECT COUNT(DISTINCT VOUCHER_NO) AS TOTAL FROM {table_name} WHERE FORM_CODE = '{form_code}' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' ";
            int? entity = this._dbContext.SqlQuery<int>(Query).FirstOrDefault();
            return entity;
        }
        public List<FinancialSubLedger> Getsubledgerdetail(string voucherno, string acccode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            //           string Query = $@"SELECT FVSD.TRANSACTION_TYPE,
            //      FVSD.ACC_CODE,
            //      FSLS.SUB_EDESC, 
            //      FVSD.SUB_CODE,
            //      FVSD.PARTICULARS,
            //      FVSD.PARTY_TYPE_CODE,
            //      iptc.party_type_code,
            //      iptc.party_type_edesc,
            //      CASE WHEN FVSD.DR_AMOUNT = 0 THEN FVSD.CR_AMOUNT ELSE FVSD.DR_AMOUNT END
            //         AS BALANCE_AMOUNT,
            //      (FVSD.DR_AMOUNT - FVSD.CR_AMOUNT) AS REMAINING_AMOUNT,
            //      FVSD.SERIAL_NO,
            //      CASE WHEN FVSD.TRANSACTION_TYPE = 'DR' THEN FVSD.DR_AMOUNT ELSE FVSD.CR_AMOUNT END
            //         AS AMOUNT
            // FROM FA_VOUCHER_SUB_DETAIL FVSD,FA_SUB_LEDGER_SETUP FSLS,IP_PARTY_TYPE_CODE iptc
            //WHERE FVSD.VOUCHER_NO = '{voucherno}' AND FVSD.ACC_CODE = '{acccode}'
            //AND FSLS.SUB_CODE=FVSD.SUB_CODE
            //AND  FSLS.COMPANY_CODE=FVSD.COMPANY_CODE  and fvsd.party_type_code=iptc.party_type_code
            //   and fvsd.company_code=iptc.company_code";
            string Query = $@"SELECT FVSD.TRANSACTION_TYPE,
       FVSD.ACC_CODE,
       FSLS.SUB_EDESC, 
       FVSD.SUB_CODE,
       FVSD.PARTICULARS,
       FVSD.PARTY_TYPE_CODE,
       iptc.party_type_code,
       iptc.party_type_edesc,
       CASE WHEN FVSD.DR_AMOUNT = 0 THEN FVSD.CR_AMOUNT ELSE FVSD.DR_AMOUNT END
          AS BALANCE_AMOUNT,
       (FVSD.DR_AMOUNT - FVSD.CR_AMOUNT) AS REMAINING_AMOUNT,
       FVSD.SERIAL_NO,
       CASE WHEN FVSD.TRANSACTION_TYPE = 'DR' THEN FVSD.DR_AMOUNT ELSE FVSD.CR_AMOUNT END
          AS AMOUNT
  FROM FA_VOUCHER_SUB_DETAIL FVSD,FA_SUB_LEDGER_SETUP FSLS,IP_PARTY_TYPE_CODE iptc
 WHERE FVSD.VOUCHER_NO = '{voucherno}' AND FVSD.ACC_CODE = '{acccode}'
 AND FSLS.SUB_CODE=FVSD.SUB_CODE
 AND  FSLS.COMPANY_CODE=FVSD.COMPANY_CODE  and fvsd.party_type_code=iptc.party_type_code(+)
    and fvsd.company_code=iptc.company_code(+)
 and FVSD.DELETED_FLAG = 'N' AND FVSD.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND FVSD.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
            var entity = this._dbContext.SqlQuery<FinancialSubLedger>(Query).ToList();
            return entity;
        }
        public List<FinancialVAT> Getvatdetail(string voucherno, string acccode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"SELECT INVOICE_NO,
       MANUAL_NO,
       INVOICE_DATE,
       FORM_CODE,
       CASE WHEN DOC_TYPE = 'P' THEN CS_CODE END AS SUPPLIER_CODE,
       CASE WHEN DOC_TYPE = 'S' THEN CS_CODE END AS CUSTOMER_CODE,
       TAXABLE_AMOUNT,
       VAT_AMOUNT,
       P_TYPE AS TYPE,
       DOC_TYPE,
       REMARKS,
       CURRENCY_CODE,
       EXCHANGE_RATE,
       SERIAL_NO,
       DIVISION_CODE,
       ACC_CODE,
       VEHICLE_PUR_FLAG,
       EXEMPTED_FLAG,
       PURCHASE_ITEM,
       PURCHASE_QUANTITY,
       PURCHASE_UNIT,
        PP_NO
       FROM FA_DC_VAT_INVOICE
        where INVOICE_NO='{voucherno}' and ACC_CODE='{acccode}'  and DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
            var entity = this._dbContext.SqlQuery<FinancialVAT>(Query).ToList();
            return entity;
        }
        public List<FinancialTDS> Gettdsdetail(string voucherno, string acccode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string Query = $@"select  SERIAL_NO,INVOICE_NO,MANUAL_NO,INVOICE_DATE,FORM_CODE,CS_CODE as SUPPLIER_CODE,ACC_CODE,TAXABLE_AMOUNT as NET_AMOUNT,TDS_PERCENT as TDS_PERCENTAGE,TDS_AMOUNT,REMARKS,CURRENCY_CODE,EXCHANGE_RATE,MEETING_CODE,VOUCHER_NO,SUBMISSION_NO,TDS_TYPE_CODE from FA_DC_TDS_INVOICE  where INVOICE_NO='{voucherno}' and DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
            var entity = this._dbContext.SqlQuery<FinancialTDS>(Query).ToList();
            return entity;
        }
        public List<FinancialBudgetTransaction> Getbudgetdetail(string voucherno, string acccode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            //string Query = $@"select ACC_CODE, BUDGET_FLAG, BUDGET_CODE as BUDGET_VAL,SERIAL_NO,BUDGET_AMOUNT as AMOUNT,PARTICULARS as NARRATION  from budget_transaction where REFERENCE_NO = '{voucherno}' and ACC_CODE='{acccode}'";
            string Query = $@"select ACC_CODE, BUDGET_FLAG, BUDGET_CODE,SERIAL_NO,BUDGET_AMOUNT as AMOUNT,PARTICULARS as NARRATION  from budget_transaction where REFERENCE_NO = '{voucherno}' and ACC_CODE='{acccode}'  and DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'";
            var entity = this._dbContext.SqlQuery<FinancialBudgetTransaction>(Query).ToList();
            return entity;
        }
        public string InsertFinanceImage(DocumentTransaction documentdetail)
        {
            var insertitem = $@"INSERT INTO DOCUMENT_TRANSACTION(VOUCHER_NO,VOUCHER_DATE,SERIAL_NO,FORM_CODE,DOCUMENT_NAME,DOCUMENT_FILE_NAME,COMPANY_CODE,BRANCH_CODE,                           CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID,SYN_ROWID)VALUES('{documentdetail.VOUCHER_NO}',TO_DATE('{documentdetail.VOUCHER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'), '{documentdetail.SERIAL_NO}','{documentdetail.FORM_CODE}','{documentdetail.DOCUMENT_FILE_NAME}', '{documentdetail.DOCUMENT_NAME}','{_workContext.CurrentUserinformation.company_code}', '{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}','{documentdetail.SESSION_ID}','{documentdetail.SYN_ROWID}')";
            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
            return null;
        }
        public string GetPrimaryColumnByTableName(string tablename)
        {

            var primarycolumn = string.Empty;
            if (tablename == "FA_SINGLE_VOUCHER" || tablename == "FA_DOUBLE_VOUCHER" || tablename == "FA_PAY_ORDER")
            {
                primarycolumn = "VOUCHER_NO";
            }
            else if (tablename == "IP_PURCHASE_MRR" || tablename == "IP_ADVICE_MRR" || tablename == "IP_PRODUCTION_MRR")
            {
                primarycolumn = "MRR_NO";
            }
            else if (tablename == "IP_PURCHASE_REQUEST")
            {
                primarycolumn = "REQUEST_NO";
            }
            else if (tablename == "IP_PURCHASE_INVOICE")
            {
                primarycolumn = "INVOICE_NO";
            }
            else if (tablename == "IP_QC_ENTRY")
            {
                primarycolumn = "ENTRY_NO";
            }
            else if (tablename == "IP_PURCHASE_RETURN" || tablename == "SA_SALES_RETURN" || tablename == "IP_GOODS_ISSUE_RETURN")
            {
                primarycolumn = "RETURN_NO";
            }
            else if (tablename == "IP_GOODS_REQUISITION")
            {
                primarycolumn = "REQUISITION_NO";
            }
            else if (tablename == "IP_QUOTATION_INQUIRY")
            {
                primarycolumn = "QUOTE_NO";
            }
            else if (tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_GOODS_ISSUE" || tablename == "IP_GATE_PASS_ENTRY" || tablename == "IP_TRANSFER_ISSUE" || tablename == "IP_PRODUCTION_ISSUE" || tablename == "IP_RETURNABLE_GOODS_ISSUE" || tablename == "IP_RETURNABLE_GOODS_RETURN")
            {
                primarycolumn = "ISSUE_NO";
            }
            else if (tablename == "IP_PURCHASE_ORDER" || tablename == "SA_SALES_ORDER")
            {
                primarycolumn = "ORDER_NO";
            }
            else if (tablename == "SA_SALES_CHALAN")
            {
                primarycolumn = "CHALAN_NO";
            }
            else if (tablename == "SA_SALES_INVOICE")
            {
                primarycolumn = "SALES_NO";
            }

            return primarycolumn;
        }

        public List<DocumentTemplateMenu> DocumentList()
        {
            var query = $@"select distinct FS.FORM_EDESC,FS.FORM_CODE from form_setup FS, form_detail_setup FD where   FS.COMPANY_CODE=FD.COMPANY_CODE AND FS.FORM_CODE=FD.FORM_CODE AND  FD.table_name = UPPER('IP_PURCHASE_INVOICE') AND FD.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND FS.DELETED_FLAG = 'N'  ORDER BY FORM_CODE ASC";
            var result = this._dbContext.SqlQuery<DocumentTemplateMenu>(query).ToList();
            return result;
        }
        public List<PurExpSheet> DocumentListDropDown(string tableName, string formCode,string fromdate,string todate)
        {
            var query = $@"select DISTINCT INVOICE_NO,MANUAL_NO,PP_NO from IP_PURCHASE_INVOICE where form_code = '{formCode}' AND trunc(INVOICE_DATE) BETWEEN TO_DATE('" + fromdate + "', 'YYYY-MM-DD') AND TO_DATE('" + todate + "', 'YYYY-MM-DD')";
            var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            return result;
        }
        public List<PurExpSheet> InvoiceDetails(string invoiceNo)
        {
            var query = $@"select DISTINCT B.SUPPLIER_EDESC,A.INVOICE_DATE,A.CURRENCY_CODE,A.EXCHANGE_RATE  from IP_PURCHASE_INVOICE A,IP_SUPPLIER_SETUP B  WHERE INVOICE_NO ='{invoiceNo}' AND B.SUPPLIER_CODE=A.SUPPLIER_CODE";
            var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            return result;
        }
        public List<PurExpSheet> InvoiceDetailsForGrid(string invoiceNo)
        {
            var query = $@"select B.ITEM_CODE,B.ITEM_EDESC, A.MU_CODE,A.QUANTITY,A.UNIT_PRICE,A.TOTAL_PRICE,A.CALC_QUANTITY,A.CALC_UNIT_PRICE,A.CALC_TOTAL_PRICE from IP_PURCHASE_INVOICE A,IP_ITEM_MASTER_SETUP B WHERE INVOICE_NO ='{invoiceNo}' AND A.ITEM_CODE = B.ITEM_CODE";
            var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            return result;
        }
        //public List<IPChargeEdesc> GetIPChargeEdesc()
        //{
        //    var query = $@"select UPPER(CHARGE_EDESC) CHARGE_EDESC,CHARGE_CODE,SPECIFIC_CHARGE_FLAG CHARGE_TYPE_FLAG  from IP_CHARGE_CODE where DELETED_FLAG ='N' and company_code='{_workContext.CurrentUserinformation.company_code}' and SPECIFIC_CHARGE_FLAG='D' and DELETED_FLAG='N' AND (SPECIFIC_CHARGE_FLAG NOT IN('V') OR SPECIFIC_CHARGE_FLAG IS NULL) ORDER BY UPPER(CHARGE_EDESC)";
           
        //  //  var query = $@"SELECT DISTINCT UPPER(b.CHARGE_EDESC) CHARGE_EDESC, a.CHARGE_CODE, a.CHARGE_TYPE_FLAG FROM CHARGE_SETUP a, IP_CHARGE_CODE b  WHERE a.CHARGE_CODE = b.CHARGE_CODE AND a.COMPANY_CODE = b.COMPANY_CODE  AND b.COMPANY_CODE='01'  AND b.DELETED_FLAG = 'N' AND a.FORM_CODE='419'  AND (b.SPECIFIC_CHARGE_FLAG NOT IN('V') OR b.SPECIFIC_CHARGE_FLAG IS NULL)ORDER BY UPPER(b.CHARGE_EDESC)";
        //    var result = this._dbContext.SqlQuery<IPChargeEdesc>(query).ToList();
        //    return result;
        //}
        public List<IPChargeEdesc> GetIPChargeEdesc(string formCode)
        {
            //var query = $@"select CHARGE_EDESC from IP_CHARGE_CODE where DELETED_FLAG ='N' and company_code='{_workContext.CurrentUserinformation.company_code}' and SPECIFIC_CHARGE_FLAG='D' and  CREATED_BY='ADMIN'";

            var query = $@"SELECT DISTINCT UPPER(b.CHARGE_EDESC) CHARGE_EDESC, a.CHARGE_CODE, a.CHARGE_TYPE_FLAG, a.APPLY_ON,a.GL_FLAG, a.APPORTION_ON, a.IMPACT_ON FROM CHARGE_SETUP a, IP_CHARGE_CODE b  WHERE a.CHARGE_CODE = b.CHARGE_CODE AND a.COMPANY_CODE = b.COMPANY_CODE  AND b.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND b.DELETED_FLAG = 'N' AND a.FORM_CODE='{formCode}'  AND (b.SPECIFIC_CHARGE_FLAG NOT IN('V') OR b.SPECIFIC_CHARGE_FLAG IS NULL)ORDER BY UPPER(b.CHARGE_EDESC)";
            var result = this._dbContext.SqlQuery<IPChargeEdesc>(query).ToList();
            return result;
        }
        //public List<IPChargeCode> GetIPChargeCode()
        //{
        //    var query = $@"select UPPER(CHARGE_EDESC) CHARGE_EDESC,CHARGE_CODE,SPECIFIC_CHARGE_FLAG CHARGE_TYPE_FLAG  from IP_CHARGE_CODE where DELETED_FLAG ='N' and company_code='{_workContext.CurrentUserinformation.company_code}' and SPECIFIC_CHARGE_FLAG='D' and DELETED_FLAG='N' AND (SPECIFIC_CHARGE_FLAG NOT IN('V') OR SPECIFIC_CHARGE_FLAG IS NULL) ORDER BY UPPER(CHARGE_EDESC)";


        //   // var query = $@"SELECT DISTINCT UPPER(b.CHARGE_EDESC) CHARGE_EDESC, a.CHARGE_CODE, a.CHARGE_TYPE_FLAG FROM CHARGE_SETUP a, IP_CHARGE_CODE b  WHERE a.CHARGE_CODE = b.CHARGE_CODE AND a.COMPANY_CODE = b.COMPANY_CODE  AND b.COMPANY_CODE='01'  AND b.DELETED_FLAG = 'N' AND a.FORM_CODE='419'  AND (b.SPECIFIC_CHARGE_FLAG NOT IN('V') OR b.SPECIFIC_CHARGE_FLAG IS NULL)ORDER BY UPPER(b.CHARGE_EDESC)";
        //    var result = this._dbContext.SqlQuery<IPChargeCode>(query).ToList();
        //    return result;
        //}
        public List<PurExpSheet> DocumentListDropDown(string tableName, string formCode, string fromdate, string todate, string manualNo = null, string ppNo = null, string supplierCode = null)
        {
            if (string.IsNullOrEmpty(manualNo) && string.IsNullOrEmpty(ppNo) && string.IsNullOrEmpty(supplierCode))
            {
                var query = $@"select DISTINCT A.INVOICE_NO,A.MANUAL_NO,A.PP_NO,B.SUPPLIER_EDESC,A.SUPPLIER_CODE from IP_PURCHASE_INVOICE A,IP_SUPPLIER_SETUP B where A.form_code = '{formCode}' AND trunc(A.INVOICE_DATE) BETWEEN TO_DATE('" + fromdate + "', 'YYYY-MM-DD') AND TO_DATE('" + todate + "', 'YYYY-MM-DD') AND B.SUPPLIER_CODE=A.SUPPLIER_CODE";
                var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
                return result;
            }
            else
            {
                List<PurExpSheet> result = new List<PurExpSheet>();
                if (!string.IsNullOrEmpty(ppNo) && ppNo != "undefined")
                {
                    var query = $@"select DISTINCT A.INVOICE_NO,A.MANUAL_NO,A.PP_NO,B.SUPPLIER_EDESC,A.SUPPLIER_CODE from IP_PURCHASE_INVOICE A,IP_SUPPLIER_SETUP B where A.form_code = '{formCode}' AND A.PP_NO = '{ppNo}' AND trunc(A.INVOICE_DATE) BETWEEN TO_DATE('" + fromdate + "', 'YYYY-MM-DD') AND TO_DATE('" + todate + "', 'YYYY-MM-DD') AND B.SUPPLIER_CODE=A.SUPPLIER_CODE";
                    result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
                    return result;
                }
                if (!string.IsNullOrEmpty(supplierCode) && supplierCode != "undefined")
                {
                    var query = $@"select DISTINCT A.INVOICE_NO,A.MANUAL_NO,A.PP_NO,B.SUPPLIER_EDESC,A.SUPPLIER_CODE from IP_PURCHASE_INVOICE A,IP_SUPPLIER_SETUP B where A.form_code = '{formCode}' AND A.SUPPLIER_CODE = '{supplierCode}' AND trunc(A.INVOICE_DATE) BETWEEN TO_DATE('" + fromdate + "', 'YYYY-MM-DD') AND TO_DATE('" + todate + "', 'YYYY-MM-DD') AND B.SUPPLIER_CODE=A.SUPPLIER_CODE";
                    result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
                    return result;
                }
                if (!string.IsNullOrEmpty(manualNo) && manualNo != "undefined")
                {
                    var query = $@"select DISTINCT A.INVOICE_NO,A.MANUAL_NO,A.PP_NO,B.SUPPLIER_EDESC,A.SUPPLIER_CODE from IP_PURCHASE_INVOICE A,IP_SUPPLIER_SETUP B where A.form_code = '{formCode}'AND A.MANUAL_NO ='{manualNo}' AND trunc(A.INVOICE_DATE) BETWEEN TO_DATE('" + fromdate + "', 'YYYY-MM-DD') AND TO_DATE('" + todate + "', 'YYYY-MM-DD') AND B.SUPPLIER_CODE=A.SUPPLIER_CODE";
                    result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
                    return result;
                }
                return result;
            }
        }

         
        public string GetPurchaseExpensesFlag(string formCode)
        {
            var query = $@"select PURCHASE_EXPENSES_FLAG from form_setup where form_code = '{formCode}'";
            var result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }
        public List<PurExpSheet> GetChargeExpList(string voucherNo,string accCode,string accEdesc)
        {
            var query = string.Empty;
            if (string.IsNullOrEmpty(accEdesc))
            {
                query = $@"SELECT A.ACC_CODE,E.ACC_EDESC,A.FORM_CODE,A.REFERENCE_NO,A.VOUCHER_NO,B.MANUAL_NO,B.PP_NO,B.SUPPLIER_CODE,C.SUPPLIER_EDESC,A.CURRENCY_CODE,A.EXCHANGE_RATE,B.ITEM_CODE,D.ITEM_EDESC,B.QUANTITY
                ,B.UNIT_PRICE,B.TOTAL_PRICE,A.CHARGE_AMOUNT,A.CHARGE_CODE,A.CHARGE_TYPE_FLAG,B.CALC_QUANTITY,B.CALC_UNIT_PRICE,B.CALC_TOTAL_PRICE,B.INVOICE_DATE
                FROM CHARGE_TRANSACTION A, IP_PURCHASE_INVOICE B,IP_SUPPLIER_SETUP C,IP_ITEM_MASTER_SETUP D,FA_CHART_OF_ACCOUNTS_SETUP E
                WHERE VOUCHER_NO = '{voucherNo}' AND A.ACC_CODE = '{accCode}'  AND A.REFERENCE_NO = B.INVOICE_NO AND A.ITEM_CODE = B.ITEM_CODE AND C.SUPPLIER_CODE = B.SUPPLIER_CODE AND D.ITEM_CODE = B.ITEM_CODE AND A.ACC_CODE = E.ACC_CODE
                ORDER BY A.CREATED_DATE DESC";
            }
            else
            {
                query = $@"SELECT A.ACC_CODE,E.ACC_EDESC,A.FORM_CODE,A.REFERENCE_NO,A.VOUCHER_NO,B.MANUAL_NO,B.PP_NO,B.SUPPLIER_CODE,C.SUPPLIER_EDESC,A.CURRENCY_CODE,A.EXCHANGE_RATE,B.ITEM_CODE,D.ITEM_EDESC,B.QUANTITY
                ,B.UNIT_PRICE,B.TOTAL_PRICE,A.CHARGE_AMOUNT,A.CHARGE_CODE,A.CHARGE_TYPE_FLAG,B.CALC_QUANTITY,B.CALC_UNIT_PRICE,B.CALC_TOTAL_PRICE,B.INVOICE_DATE  
                FROM CHARGE_TRANSACTION A, IP_PURCHASE_INVOICE B,IP_SUPPLIER_SETUP C,IP_ITEM_MASTER_SETUP D,FA_CHART_OF_ACCOUNTS_SETUP E
                WHERE VOUCHER_NO = '{voucherNo}' AND E.ACC_EDESC LIKE '%{accEdesc}%'  AND A.REFERENCE_NO = B.INVOICE_NO AND A.ITEM_CODE = B.ITEM_CODE AND C.SUPPLIER_CODE = B.SUPPLIER_CODE AND D.ITEM_CODE = B.ITEM_CODE AND A.ACC_CODE = E.ACC_CODE
                ORDER BY A.CREATED_DATE DESC";
            }
            var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            if (result.Count == 0)
            {
                result = null;
            }
            return result;
        }

        //public List<FVPURCHASEEXPSHEETRERERENCE> GetFVReferencePurchaseexpsheet(string voucherno)
        //{
        //    string Query = $@"  SELECT distinct c.reference_no,f.form_edesc FROM charge_transaction c,form_setup f
        //      WHERE f.form_code = c.form_code AND f.company_code = c.company_code             
        //       and c.company_code='{_workContext.CurrentUserinformation.company_code}'
        //       and c.voucher_no='{voucherno}'";
        //    List<FVPURCHASEEXPSHEETRERERENCE> entity = this._dbContext.SqlQuery<FVPURCHASEEXPSHEETRERERENCE>(Query).ToList();
        //    return entity;

        //}

        public List<IPChargeCode> GetIPChargeCode(string formCode)
        {
            //var query = $@"select CHARGE_CODE from IP_CHARGE_CODE where DELETED_FLAG ='N' and company_code='{_workContext.CurrentUserinformation.company_code}' and SPECIFIC_CHARGE_FLAG='D' and  CREATED_BY='ADMIN'";

            var query = $@"SELECT DISTINCT UPPER(b.CHARGE_EDESC) CHARGE_EDESC, a.CHARGE_CODE, a.CHARGE_TYPE_FLAG, a.APPLY_ON,a.GL_FLAG, a.APPORTION_ON, a.IMPACT_ON FROM CHARGE_SETUP a, IP_CHARGE_CODE b  WHERE a.CHARGE_CODE = b.CHARGE_CODE AND a.COMPANY_CODE = b.COMPANY_CODE  AND b.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND b.DELETED_FLAG = 'N' AND a.FORM_CODE='{formCode}'  AND (b.SPECIFIC_CHARGE_FLAG NOT IN('V') OR b.SPECIFIC_CHARGE_FLAG IS NULL)ORDER BY UPPER(b.CHARGE_EDESC)";
            var result = this._dbContext.SqlQuery<IPChargeCode>(query).ToList();
            return result;
        }
        public List<IPChargeDtls> GetIPChargedetls(string formCode)
        {
            //var query = $@"select CHARGE_CODE from IP_CHARGE_CODE where DELETED_FLAG ='N' and company_code='{_workContext.CurrentUserinformation.company_code}' and SPECIFIC_CHARGE_FLAG='D' and  CREATED_BY='ADMIN'";

            var query = $@"SELECT DISTINCT UPPER(b.CHARGE_EDESC) CHARGE_EDESC, a.CHARGE_CODE, a.CHARGE_TYPE_FLAG, a.APPLY_ON,a.GL_FLAG, a.APPORTION_ON, a.IMPACT_ON FROM CHARGE_SETUP a, IP_CHARGE_CODE b  WHERE a.CHARGE_CODE = b.CHARGE_CODE AND a.COMPANY_CODE = b.COMPANY_CODE  AND b.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'  AND b.DELETED_FLAG = 'N' AND a.FORM_CODE='{formCode}'  AND (b.SPECIFIC_CHARGE_FLAG NOT IN('V') OR b.SPECIFIC_CHARGE_FLAG IS NULL)ORDER BY UPPER(b.CHARGE_EDESC)";
            var result = this._dbContext.SqlQuery<IPChargeDtls>(query).ToList();
            return result;
        }
        //public string GetPurchaseExpensesFlag(string formCode)
        //{
        //    var query = $@"select PURCHASE_EXPENSES_FLAG from form_setup where form_code = '{formCode}'";
        //    var result = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
        //    return result;
        //}
        public List<string> GetVoucherNoFrmCharge(string invoiceNo)
        {
            var query = string.Empty;
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                query = $@"SELECT  DISTINCT VOUCHER_NO FROM CHARGE_TRANSACTION WHERE REFERENCE_NO = '{invoiceNo}' AND VOUCHER_NO IS NOT NULL";
            }
            var result = this._dbContext.SqlQuery<string>(query).ToList();
            //if (result.Count == 0)
            //{
            //    result = null;
            //}
            return result;
        }
        public List<PurExpSheet> GetChargeDtlFrmInvoice(string invoiceNo)
        {
            var query = string.Empty;
            if (!string.IsNullOrEmpty(invoiceNo))
            {
                query = $@"SELECT A.ACC_CODE,E.ACC_EDESC,A.FORM_CODE,A.REFERENCE_NO,A.VOUCHER_NO,B.MANUAL_NO,B.PP_NO,B.SUPPLIER_CODE,C.SUPPLIER_EDESC,A.CURRENCY_CODE,A.EXCHANGE_RATE,B.ITEM_CODE,D.ITEM_EDESC,B.QUANTITY
                ,B.UNIT_PRICE,B.TOTAL_PRICE,A.CHARGE_AMOUNT,A.CHARGE_CODE,A.CHARGE_TYPE_FLAG,B.CALC_QUANTITY,B.CALC_UNIT_PRICE,B.CALC_TOTAL_PRICE,B.INVOICE_DATE
                FROM CHARGE_TRANSACTION A, IP_PURCHASE_INVOICE B,IP_SUPPLIER_SETUP C,IP_ITEM_MASTER_SETUP D,FA_CHART_OF_ACCOUNTS_SETUP E
                WHERE A.REFERENCE_NO = '{invoiceNo}' AND A.REFERENCE_NO = B.INVOICE_NO 
                AND A.ITEM_CODE = B.ITEM_CODE 
                AND C.SUPPLIER_CODE = B.SUPPLIER_CODE 
               AND D.ITEM_CODE = B.ITEM_CODE 
              AND A.ACC_CODE = E.ACC_CODE
                AND A.COMPANY_CODE= '{_workContext.CurrentUserinformation.company_code}' AND A.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
              and A.DELETED_FLAG = 'N'
                ORDER BY A.CREATED_DATE DESC";
            }
            var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            return result;
        }
        public List<PurExpSheet> GetChargeDtlFrmVoucherNo(string orderNo)
        {
            var query = string.Empty;
            if (!string.IsNullOrEmpty(orderNo))
            {
                query = $@"SELECT  DISTINCT REFERENCE_NO FROM CHARGE_TRANSACTION WHERE VOUCHER_NO = '{orderNo}' AND VOUCHER_NO IS NOT NULL";
            }
            var result = this._dbContext.SqlQuery<string>(query).ToList();
            if (result.Count > 0)
            {
                query = $@"SELECT A.ACC_CODE,E.ACC_EDESC,A.FORM_CODE,A.REFERENCE_NO,A.VOUCHER_NO,B.MANUAL_NO,B.PP_NO,B.SUPPLIER_CODE,C.SUPPLIER_EDESC,A.CURRENCY_CODE,A.EXCHANGE_RATE,B.ITEM_CODE,D.ITEM_EDESC,B.QUANTITY
                ,B.UNIT_PRICE,B.TOTAL_PRICE,A.CHARGE_AMOUNT,A.CHARGE_CODE,A.CHARGE_TYPE_FLAG,B.CALC_QUANTITY,B.CALC_UNIT_PRICE,B.CALC_TOTAL_PRICE,B.INVOICE_DATE
                FROM CHARGE_TRANSACTION A, IP_PURCHASE_INVOICE B,IP_SUPPLIER_SETUP C,IP_ITEM_MASTER_SETUP D,FA_CHART_OF_ACCOUNTS_SETUP E
                WHERE A.REFERENCE_NO = '{result[0].ToString()}' AND A.REFERENCE_NO = B.INVOICE_NO AND A.ITEM_CODE = B.ITEM_CODE AND C.SUPPLIER_CODE = B.SUPPLIER_CODE AND D.ITEM_CODE = B.ITEM_CODE AND A.ACC_CODE = E.ACC_CODE AND A.COMPANY_CODE= '{_workContext.CurrentUserinformation.company_code}' AND A.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' and A.DELETED_FLAG = 'N'
                ORDER BY A.CREATED_DATE DESC";
            }
            var data = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            return data;
        }
        public List<PurExpSheet> BindAccountCode(string formCode, string itemEdesc)
        {
            var query = string.Empty;
            if (!string.IsNullOrEmpty(formCode) && !string.IsNullOrEmpty(itemEdesc))
            {
                query = $@"select a.ACC_CODE,a.ACC_EDESC,item.ITEM_CODE,item.ITEM_EDESC from FA_CHART_OF_ACCOUNTS_SETUP a,IP_INTEGRATION_SETUP i,IP_ITEM_MASTER_SETUP item where a.ACC_CODE = i.ACC_CODE and item.ITEM_CODE = i.ITEM_CODE and i.FORM_CODE = '{formCode}' and i.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' and item.ITEM_EDESC = '{itemEdesc}'";
            }
            var result = this._dbContext.SqlQuery<PurExpSheet>(query).ToList();
            return result;
        }
    }
}
