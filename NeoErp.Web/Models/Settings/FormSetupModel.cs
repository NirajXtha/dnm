using DocumentFormat.OpenXml.Drawing.Charts;
using dotless.Core.Parser.Tree;
using Microsoft.Ajax.Utilities;
using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Services;
using NeoErp.Data;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace NeoErp.Models.Settings
{
    public class FormSetupModel : NeoErp.Core.Services.IFormSetup
    {
        private ICacheManager _cacheManager;
        private IDbContext _dbContext;
        private IWorkContext _workContext;

        public FormSetupModel(ICacheManager cacheManager, IDbContext dbContext, IWorkContext workContext)
        {
            this._cacheManager = cacheManager;
            this._dbContext = dbContext;
            this._workContext = workContext;
        }

        public List<FormTreeStructureModel> GetFormTreeStructureList(string moduleId = "")
        {
            var dataList = new FormTreeStructureModel();
            string group_sku_flag = "G";  // this will be dynamic from Database
            string sqlQuery = $@"SELECT form_code,
                                                      form_edesc,
                                                      master_form_code,
                                                      group_sku_flag,
                                                      pre_form_code,
                                                      form_ndesc,
                                                        remarks
                                                 FROM form_setup
                                                WHERE module_code = '{moduleId}'
                                                  AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                                  AND DELETED_FLAG = 'N'
                                                  AND group_sku_flag = 'G'
                                             ORDER BY master_form_code, form_code";
            var result = _dbContext.SqlQuery<FormTreeStructureModel>(sqlQuery).ToList();
            return result;
        }


        public List<FormSetupResultModel> GetFormListByPreFormCode(string preFormCode, string moduleId = "04")
        {

            string companyCode = _workContext.CurrentUserinformation.company_code;
            string CREATED_BY = "ADMIN";
            string PUBLIC_FLAG = "Y";
            string GROUP_SKU_FLAG = "I";

            string sqlQuery = $@"
                                SELECT FORM_CODE,
                                       FORM_EDESC,
                                       MODULE_CODE,
                                       NUMBERING_FORMAT,
                                       DATE_FORMAT,
                                       START_ID_FLAG,
                                       ID_GENERATION_FLAG,
                                       CUSTOM_PREFIX_TEXT,
                                       CUSTOM_SUFFIX_TEXT,
                                       TO_CHAR(PREFIX_LENGTH) AS PREFIX_LENGTH,
                                       TO_CHAR(SUFFIX_LENGTH) AS SUFFIX_LENGTH,
                                       TO_CHAR(BODY_LENGTH) AS BODY_LENGTH,
                                       TO_CHAR(START_NO) AS START_NO,
                                       TO_CHAR(LAST_NO) AS LAST_NO,
                                       START_DATE,
                                       LAST_DATE,
                                       PRINT_REPORT_FLAG,
                                       COPY_VALUES_FLAG,
                                       QUALITY_CHECK_FLAG,
                                       SERIAL_TRACKING_FLAG,
                                       BATCH_TRACKING_FLAG,
                                       REMARKS
                                  FROM FORM_SETUP
                                 WHERE DELETED_FLAG = 'N'
                                   AND PRE_FORM_CODE = '{preFormCode}'
                                   AND COMPANY_CODE = '{companyCode}'
                                   AND (CREATED_BY = '{CREATED_BY}' OR PUBLIC_FLAG = '{PUBLIC_FLAG}')
                                   AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                                   AND MODULE_CODE = '{moduleId}'
                              ORDER BY TO_NUMBER(FORM_CODE)";

            var result = _dbContext.SqlQuery<FormSetupResultModel>(sqlQuery).ToList();
            return result;
        }

        public List<AllFormModel> GetMasterFormList()
        {

            // Defaults
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var group_sku_flag = "G";
            string sqlQuery = $@"
                            SELECT '00' MASTER_ITEM_CODE, '<PRIMARY>' ITEM_EDESC, null as Module_code FROM DUAL
                            UNION ALL
                            SELECT master_form_code, form_edesc, module_code
                              FROM form_setup
                             WHERE     DELETED_FLAG = 'N'
                                   AND COMPANY_CODE = '{companyCode}'
                                   AND pre_form_code = '00'
                                   AND group_sku_flag = '{group_sku_flag}'";

            var result = _dbContext.SqlQuery<AllFormModel>(sqlQuery).ToList();
            return result;
        }

        public int GetNextFormCode()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                SELECT NVL(MAX(TO_NUMBER(FORM_CODE)), 0) + 1 AS NEXT_FORM_CODE
                  FROM FORM_SETUP
                 WHERE COMPANY_CODE = '{companyCode}'";

            var result = _dbContext.SqlQuery<int>(sqlQuery).FirstOrDefault();
            return result;
        }


        public List<ModuleCodeAndDetailModel> GetAllModuleList()
        {
            string sqlQuery = @"SELECT module_code, module_edesc FROM module_setup";

            var result = _dbContext.SqlQuery<ModuleCodeAndDetailModel>(sqlQuery).ToList();
            return result;
        }


        public List<BranchCodeDtlModel> GetAllBranchList(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var GROUP_SKU_FLAG = "I";

            string sqlQuery = $@"
                        SELECT DISTINCT 
                            BRANCH_CODE, 
                            BRANCH_EDESC,
                            (SELECT DISTINCT 'X' 
                               FROM FORM_BRANCH_MAP 
                              WHERE FORM_CODE = '{formCode}' 
                                AND BRANCH_CODE = FA_BRANCH_SETUP.BRANCH_CODE) CHECKCED
                        FROM FA_BRANCH_SETUP 
                        WHERE COMPANY_CODE = '{companyCode}' 
                          AND DELETED_FLAG = 'N' 
                          AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                        ORDER BY TO_NUMBER(REGEXP_REPLACE(BRANCH_CODE, '[^0-9]', ''))";

            var result = _dbContext.SqlQuery<BranchCodeDtlModel>(sqlQuery).ToList();
            return result;
        }


        public bool InsertFormGroupItem(FormSetupGroupEntryModel model)
        {
            // INSERT INTO FORM_SETUP(FORM_CODE, FORM_EDESC, FORM_NDESC, MASTER_FORM_CODE, PRE_FORM_CODE, GROUP_SKU_FLAG, REMARKS, MODULE_CODE, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG)
            // VALUES('518', 'test', 'test1', '10.06', '10', 'G', 'test', '04', '01', 'ADMIN', SYSDATE, 'N')


            string group_sku_flag = "G";
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var createdBy = _workContext.CurrentUserinformation.login_code;

            model.GROUP_SKU_FLAG = group_sku_flag;

            // ✅ Use your database function for MASTER_FORM_CODE
            string sqlQuery = $@"
        INSERT INTO FORM_SETUP
            (FORM_CODE, FORM_EDESC, FORM_NDESC, MASTER_FORM_CODE, PRE_FORM_CODE,
             GROUP_SKU_FLAG, REMARKS, MODULE_CODE, COMPANY_CODE, CREATED_BY,
             CREATED_DATE, DELETED_FLAG)
        VALUES
            ('{model.FORM_CODE}', 
             '{model.FORM_EDESC}', 
             '{model.FORM_NDESC}', 
             FN_GET_NEXT_MASTER_FORM_CODE('{model.PRE_FORM_CODE}'),  -- ← function used here
             '{model.PRE_FORM_CODE}', 
             '{model.GROUP_SKU_FLAG}', 
             '{model.REMARKS}', 
             '{model.MODULE_CODE}', 
             '{companyCode}', 
             '{createdBy}', 
             SYSDATE, 
             'N')";

            var result = _dbContext.ExecuteSqlCommand(sqlQuery);
            return result > 0;

        }

        public bool UpdateFormGroupItem(FormSetupGroupEntryModel model)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var modifiedBy = _workContext.CurrentUserinformation.login_code;

            // ✅ Build the SQL update query
            string sqlQuery = $@"
            UPDATE FORM_SETUP
               SET FORM_EDESC   = '{model.FORM_EDESC}',
                   FORM_NDESC   = '{model.FORM_NDESC}',
                   REMARKS      = '{model.REMARKS}',
                   MODULE_CODE  = '{model.MODULE_CODE}'
             WHERE FORM_CODE    = '{model.FORM_CODE}'
               AND COMPANY_CODE = '{companyCode}'
              ";

            var result = _dbContext.ExecuteSqlCommand(sqlQuery);

            //UPDATE FORM_SETUP
            //SET MODULE_CODE = '04'
            //WHERE PRE_FORM_CODE LIKE '10.05.01%' AND COMPANY_CODE = '01'
            string sqlQuery1 = $@"UPDATE FORM_SETUP SET MODULE_CODE = '{model.MODULE_CODE}' WHERE PRE_FORM_CODE LIKE '{model.PRE_FORM_CODE}%' AND COMPANY_CODE = '{companyCode}'";
            result = _dbContext.ExecuteSqlCommand(sqlQuery1);

            return result > 0;
        }


        public FormSetupAddOrDuplicateStepModel GetFormSetupClone(string formCode)
        {

            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
        SELECT
                    TO_CHAR(
                (SELECT NVL(MAX(TO_NUMBER(FORM_CODE)),0) + 1 
                 FROM FORM_SETUP 
                 WHERE COMPANY_CODE = '{companyCode}')
            ) AS FORM_CODE,
           FORM_EDESC,
            FORM_NDESC,
            PRE_FORM_CODE,
            MODULE_CODE
        FROM FORM_SETUP
        WHERE FORM_CODE = '{formCode}' AND COMPANY_CODE = '{companyCode}'";

            var result = _dbContext.SqlQuery<FormSetupAddOrDuplicateStepModel>(sqlQuery).FirstOrDefault();

            return result;
        }
        // 

        public FormSetupAddOrDuplicateStepModel GetForEditFormSetupData(string formCode, bool isDuplicate = false)
        {

            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"SELECT
                                FORM_CODE,
                                FORM_EDESC,
                                FORM_NDESC,
                                MASTER_FORM_CODE,
                                PRE_FORM_CODE,
                                MODULE_CODE,
                                GROUP_SKU_FLAG,

                    --            TEMPLATE_CODE,                              
                    --            NUMBERING_FORMAT,
                    --            DATE_FORMAT,
                    --            START_ID_FLAG,
                    --            ID_GENERATION_FLAG,
                    --            CUSTOM_PREFIX_TEXT,
                    --            CUSTOM_SUFFIX_TEXT,
                    --            PREFIX_LENGTH,
                    --            SUFFIX_LENGTH,
                    --            BODY_LENGTH,
                    --            START_NO,
                    --            LAST_NO,
                    --            START_DATE,
                    --            LAST_DATE,
                    --            REFERENCE_FLAG,
                    --            REF_TABLE_NAME,
                    --            REF_COLUMN_NAME,
                    --            REF_FORM_CODE,
                    --            PRINT_REPORT_FLAG,
                    --            PRIMARY_MANUAL_FLAG,
                    --            COPY_VALUES_FLAG,
                    --            QUALITY_CHECK_FLAG,
                    --            SERIAL_TRACKING_FLAG,
                    --            BATCH_TRACKING_FLAG,
                    --            ACC_CODE,
                    --            REMARKS,
                    --            COMPANY_CODE,
                    --            CREATED_BY,
                    --            CREATED_DATE,
                    --            DELETED_FLAG,
                    --            COINAGE_FLAG,
                    --            COINAGE_SUB_CODE,
                    --            INTER_BRANCH_FLAG,
                    --            TOTAL_ROUND_FLAG,
                    --            TOTAL_ROUND_INDEX,
                    --            REPORT_NO,
                    --            MULTI_UNIT_FLAG,
                    --            REF_FIX_QUANTITY,
                    --            REF_FIX_PRICE,
                    --            SALES_INVOICE_CHALAN_FLAG,
                    --            FREEZE_BACK_DAYS,
                    --            AUTO_GL_POST,
                    --            RATE_SCHEDULE_FIX_PRICE,
                    --            FREEZE_MANUAL_ENTRY_FLAG,
                    --            ADVANCE_FLAG,
                    --            PURCHASE_EXPENSES_FLAG,
                    --            FORM_TYPE,
                    --            COSTING_FLAG,
                    --            NEGATIVE_STOCK_FLAG,
                    --            QC_PARAMETER_FLAG,
                    --            OTHER_INFO_FLAG,
                    --            AFTER_VERIFY_FLAG,
                    --            PRICE_CONTROL_FLAG,

                                  INTER_BRANCH_FLAG,
                                  FORM_ACTION_FLAG,
                                  FORM_TYPE,
                                   REMARKS
                            FROM FORM_SETUP
                            WHERE FORM_CODE = '{formCode}' AND COMPANY_CODE = '{companyCode}'";

            var result = _dbContext.SqlQuery<FormSetupAddOrDuplicateStepModel>(sqlQuery).FirstOrDefault();


            if (isDuplicate)
            {
                var queryString = $@"SELECT TO_NUMBER(form_code) + 1 AS next_form_code FROM (
                                                            SELECT *
                                                            FROM Form_Setup
                                                            WHERE Company_Code = '{companyCode}'
                                                              AND REGEXP_LIKE(Form_Code, '^\d+$')
                                                            ORDER BY TO_NUMBER(Form_Code) DESC
                                                        )
                                                        WHERE ROWNUM = 1";
                var next_form_code = _dbContext.SqlQuery<int>(queryString).FirstOrDefault();

                result.ORIGINAL_FORM_CODE = result.FORM_CODE;
                result.NEXT_FORM_CODE = next_form_code.ToString();

                //result.FORM_CODE = next_form_code.ToString();

                // logic to crate new using dublicate
            }



            return result;
        }

        public bool UpdateFormSetupGridItem(FormSetupAddOrDuplicateStepModel model)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var modifiedBy = _workContext.CurrentUserinformation.login_code;

            if (!string.IsNullOrEmpty(model.NEXT_FORM_CODE))
            {
                string insertQuery = GetInsertFormDetailDataForDuplicateEntryQuery(model);

                var result1 = _dbContext.ExecuteSqlCommand(insertQuery);
                return result1 > 0;
            }

            // ✅ Build the SQL update query
            string sqlQuery = $@"
                            UPDATE FORM_SETUP
                   SET FORM_EDESC = '{model.FORM_EDESC}',
                       FORM_NDESC = '{model.FORM_NDESC}',
                       REMARKS = '{model.REMARKS}',
                       FORM_ACTION_FLAG = '{model.FORM_ACTION_FLAG}',
                       INTER_BRANCH_FLAG = '{model.INTER_BRANCH_FLAG}',
                       FORM_TYPE = '{model.FORM_TYPE}'
                 WHERE FORM_CODE = '{model.FORM_CODE}' AND COMPANY_CODE = '{companyCode}'
              ";
            var result = _dbContext.ExecuteSqlCommand(sqlQuery);
            return result > 0;
        }

        private string GetInsertFormDetailDataForDuplicateEntryQuery(FormSetupAddOrDuplicateStepModel model)
        {

            var companyCode = _workContext.CurrentUserinformation.company_code;
            var modifiedBy = _workContext.CurrentUserinformation.login_code;

            var queryString = $@"SELECT TO_NUMBER(form_code) + 1 AS next_form_code FROM (
                                                            SELECT *
                                                            FROM Form_Setup
                                                            WHERE Company_Code = '{companyCode}'
                                                              AND REGEXP_LIKE(Form_Code, '^\d+$')
                                                            ORDER BY TO_NUMBER(Form_Code) DESC
                                                        )
                                                        WHERE ROWNUM = 1";
            var next_form_code = _dbContext.SqlQuery<int>(queryString).FirstOrDefault();

            //result.ORIGINAL_FORM_CODE = result.FORM_CODE;
            ///result.NEXT_FORM_CODE = next_form_code.ToString();

            string sqlEntryQuery = $@"INSERT INTO FORM_SETUP (FORM_CODE,
                                                                    FORM_EDESC,
                                                                    FORM_NDESC,
                                                                    DR_ACC_CODE,
                                                                    CR_ACC_CODE,
                                                                    MASTER_FORM_CODE,
                                                                    PRE_FORM_CODE,
                                                                    MODULE_CODE,
                                                                    TEMPLATE_CODE,
                                                                    GROUP_SKU_FLAG,
                                                                    NUMBERING_FORMAT,
                                                                    DATE_FORMAT,
                                                                    START_ID_FLAG,
                                                                    ID_GENERATION_FLAG,
                                                                    CUSTOM_PREFIX_TEXT,
                                                                    CUSTOM_SUFFIX_TEXT,
                                                                    PREFIX_LENGTH,
                                                                    SUFFIX_LENGTH,
                                                                    BODY_LENGTH,
                                                                    START_NO,
                                                                    LAST_NO,
                                                                    START_DATE,
                                                                    LAST_DATE,
                                                                    REF_COLUMN_NAME,
                                                                    PRINT_REPORT_FLAG,
                                                                    PRIMARY_MANUAL_FLAG,
                                                                    COPY_VALUES_FLAG,
                                                                    QUALITY_CHECK_FLAG,
                                                                    SERIAL_TRACKING_FLAG,
                                                                    BATCH_TRACKING_FLAG,
                                                                    ACC_CODE,
                                                                    REMARKS,
                                                                    COMPANY_CODE,
                                                                    CREATED_BY,
                                                                    CREATED_DATE,
                                                                    DELETED_FLAG,
                                                                    COINAGE_FLAG,
                                                                    COINAGE_SUB_CODE,
                                                                    REFERENCE_FLAG,
                                                                    REF_TABLE_NAME,
                                                                    PUBLIC_FLAG,
                                                                    FORM_ACTION_FLAG,
                                                                    TOTAL_ROUND_FLAG,
                                                                    TOTAL_ROUND_INDEX,
                                                                    REF_FORM_CODE,
                                                                    INTER_BRANCH_FLAG,
                                                                    REPORT_NO,
                                                                    MULTI_UNIT_FLAG,
                                                                    DELTA_FLAG,
                                                                    SYN_ROWID,
                                                                    SALES_INVOICE_CHALAN_FLAG,
                                                                    REF_FIX_QUANTITY,
                                                                    REF_FIX_PRICE,
                                                                    FREEZE_BACK_DAYS,
                                                                    AUTO_GL_POST,
                                                                    PURCHASE_INVOICE_MRR_FLAG,
                                                                    DECIMAL_PLACE,
                                                                    PURCHASE_EXPENSES_FLAG,
                                                                    BATCH_FLAG,
                                                                    RATE_SCHEDULE_FIX_PRICE,
                                                                    FREEZE_MANUAL_ENTRY_FLAG,
                                                                    ADVANCE_FLAG,
                                                                    RT_CONTROL_FLAG,
                                                                    FORM_TYPE,
                                                                    COSTING_FLAG,
                                                                    NEGATIVE_STOCK_FLAG,
                                                                    QC_PARAMETER_FLAG,
                                                                    MODIFY_DATE,
                                                                    OTHER_INFO_FLAG,
                                                                    CUSTOM_MANUAL_PREFIX_TEXT,
                                                                    CUSTOM_MANUAL_SUFFIX_TEXT,
                                                                    BODY_MANUAL_LENGTH,
                                                                    START_MANUAL_NO,
                                                                    ORDER_ACCESS_FAB_FLAG,
                                                                    LOT_GEN_FLAG,
                                                                    AFTER_VERIFY_FLAG,
                                                                    ONLINE_FLAG,
                                                                    ADD_ROW_FLAG,
                                                                    PRICE_CONTROL_FLAG,
                                                                    RATE_DIFF_FLAG,
                                                                    FREEZE_MASTER_REF_FLAG,
                                                                    ACCESS_BDFSM_FLAG,
                                                                    INFO_FLAG,
                                                                    WM_FLAG,
                                                                    FREEQTY_FLAG,
                                                                    DC_VAT_FLAG,
                                                                    COMMITMENT_FLAG,
                                                                    ORDER_DISPATCH_FLAG,
                                                                    PURCHASE_MRR_GRNI_FLAG,
                                                                    PENDING_INFO_FLAG,
                                                                    BACK_DATE_VNO_SAVE_FLAG,
                                                                    AFTER_POSTING_FLAG,
                                                                    DC_TDS_FLAG,
                                                                    PAYMENT_MODE_FLAG,
                                                                    INVOICE_PJV_FORM_CODE,
                                                                    DISCOUNT_SCHEDULE_FLAG,
                                                                    MODIFY_BY,
                                                                    ROLL_TRACKING_FLAG,
                                                                    VNO_AS_DOC_ID_CONTROL,
                                                                    DEFAULT_SHIPMENT,
                                                                    DEFAULT_FROM_LOCATION,
                                                                    RECEIPT_FLAG,
                                                                    RECEIPT_FORM_CODE,
                                                                    RECEIPT_CASH_ACC_CODE,
                                                                    NON_RETURN_FLAG,
                                                                    DISPLAY_RATE,
                                                                    EMAIL_FLAG,
                                                                    EMAIL_MESSAGE,
                                                                    EMAIL_SUBJECT,
                                                                    EMAIL_BCC,
                                                                    DEACTIVATE_TAXABLE_RATE_FLAG,
                                                                    QUOTATION_FLAG,
                                                                    AUTO_CONV_FLAG,
                                                                    PROD_IO_EQUAL_FLAG,
                                                                    QR_FLAG,
                                                                    CALL_API,
                                                                    MAX_ROWS)
                                               SELECT '{next_form_code}' AS FORM_CODE,
                                                      FORM_EDESC,
                                                      FORM_NDESC,
                                                      DR_ACC_CODE,
                                                      CR_ACC_CODE,
                                                      MASTER_FORM_CODE,
                                                      PRE_FORM_CODE,
                                                      MODULE_CODE,
                                                      TEMPLATE_CODE,
                                                      GROUP_SKU_FLAG,
                                                      NUMBERING_FORMAT,
                                                      DATE_FORMAT,
                                                      START_ID_FLAG,
                                                      ID_GENERATION_FLAG,
                                                      CUSTOM_PREFIX_TEXT,
                                                      CUSTOM_SUFFIX_TEXT,
                                                      PREFIX_LENGTH,
                                                      SUFFIX_LENGTH,
                                                      BODY_LENGTH,
                                                      START_NO,
                                                      LAST_NO,
                                                      START_DATE,
                                                      LAST_DATE,
                                                      REF_COLUMN_NAME,
                                                      PRINT_REPORT_FLAG,
                                                      PRIMARY_MANUAL_FLAG,
                                                      COPY_VALUES_FLAG,
                                                      QUALITY_CHECK_FLAG,
                                                      SERIAL_TRACKING_FLAG,
                                                      BATCH_TRACKING_FLAG,
                                                      ACC_CODE,
                                                      REMARKS,
                                                      COMPANY_CODE,
                                                      'ADMIN' AS CREATED_BY,
                                                      SYSDATE AS CREATED_DATE,
                                                      'N' AS DELETED_FLAG,
                                                      COINAGE_FLAG,
                                                      COINAGE_SUB_CODE,
                                                      REFERENCE_FLAG,
                                                      REF_TABLE_NAME,
                                                      PUBLIC_FLAG,
                                                      FORM_ACTION_FLAG,
                                                      TOTAL_ROUND_FLAG,
                                                      TOTAL_ROUND_INDEX,
                                                      REF_FORM_CODE,
                                                      INTER_BRANCH_FLAG,
                                                      REPORT_NO,
                                                      MULTI_UNIT_FLAG,
                                                      DELTA_FLAG,
                                                      SYN_ROWID,
                                                      SALES_INVOICE_CHALAN_FLAG,
                                                      REF_FIX_QUANTITY,
                                                      REF_FIX_PRICE,
                                                      FREEZE_BACK_DAYS,
                                                      AUTO_GL_POST,
                                                      PURCHASE_INVOICE_MRR_FLAG,
                                                      DECIMAL_PLACE,
                                                      PURCHASE_EXPENSES_FLAG,
                                                      BATCH_FLAG,
                                                      RATE_SCHEDULE_FIX_PRICE,
                                                      FREEZE_MANUAL_ENTRY_FLAG,
                                                      ADVANCE_FLAG,
                                                      RT_CONTROL_FLAG,
                                                      FORM_TYPE,
                                                      COSTING_FLAG,
                                                      NEGATIVE_STOCK_FLAG,
                                                      QC_PARAMETER_FLAG,
                                                      MODIFY_DATE,
                                                      OTHER_INFO_FLAG,
                                                      CUSTOM_MANUAL_PREFIX_TEXT,
                                                      CUSTOM_MANUAL_SUFFIX_TEXT,
                                                      BODY_MANUAL_LENGTH,
                                                      START_MANUAL_NO,
                                                      ORDER_ACCESS_FAB_FLAG,
                                                      LOT_GEN_FLAG,
                                                      AFTER_VERIFY_FLAG,
                                                      ONLINE_FLAG,
                                                      ADD_ROW_FLAG,
                                                      PRICE_CONTROL_FLAG,
                                                      RATE_DIFF_FLAG,
                                                      FREEZE_MASTER_REF_FLAG,
                                                      ACCESS_BDFSM_FLAG,
                                                      INFO_FLAG,
                                                      WM_FLAG,
                                                      FREEQTY_FLAG,
                                                      DC_VAT_FLAG,
                                                      COMMITMENT_FLAG,
                                                      ORDER_DISPATCH_FLAG,
                                                      PURCHASE_MRR_GRNI_FLAG,
                                                      PENDING_INFO_FLAG,
                                                      BACK_DATE_VNO_SAVE_FLAG,
                                                      AFTER_POSTING_FLAG,
                                                      DC_TDS_FLAG,
                                                      PAYMENT_MODE_FLAG,
                                                      INVOICE_PJV_FORM_CODE,
                                                      DISCOUNT_SCHEDULE_FLAG,
                                                      MODIFY_BY,
                                                      ROLL_TRACKING_FLAG,
                                                      VNO_AS_DOC_ID_CONTROL,
                                                      DEFAULT_SHIPMENT,
                                                      DEFAULT_FROM_LOCATION,
                                                      RECEIPT_FLAG,
                                                      RECEIPT_FORM_CODE,
                                                      RECEIPT_CASH_ACC_CODE,
                                                      NON_RETURN_FLAG,
                                                      DISPLAY_RATE,
                                                      EMAIL_FLAG,
                                                      EMAIL_MESSAGE,
                                                      EMAIL_SUBJECT,
                                                      EMAIL_BCC,
                                                      DEACTIVATE_TAXABLE_RATE_FLAG,
                                                      QUOTATION_FLAG,
                                                      AUTO_CONV_FLAG,
                                                      PROD_IO_EQUAL_FLAG,
                                                      QR_FLAG,
                                                      CALL_API,
                                                      MAX_ROWS
                                                 FROM FORM_SETUP F
                                                WHERE     F.FORM_CODE = '{model.FORM_CODE}'
                                                      AND F.COMPANY_CODE = '{companyCode}'
                                                      AND NOT EXISTS
                                                             (SELECT 1
                                                                FROM FORM_SETUP X
                                                               WHERE X.FORM_CODE = '{next_form_code}' AND X.COMPANY_CODE = '{companyCode}')";

            return sqlEntryQuery;

        }


        public List<TransactionTableListModel> FormattingTab_GetTransactionTableList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string sqlQuery = @"
                SELECT 
                    TABLE_NAME, 
                    TABLE_DESC 
                FROM TRANSACTION_TABLE_LIST 
                ORDER BY TABLE_NAME";

            var result = _dbContext.SqlQuery<TransactionTableListModel>(sqlQuery).ToList();
            return result;
        }


        public List<SubLedgerListModel> FormattingTab_GetSubLedgerList(string searchText)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            // Base SQL
            var sqlQuery = $@"
        SELECT 
            SUB_EDESC, 
            SUB_CODE 
        FROM FA_SUB_LEDGER_SETUP 
        WHERE SUB_CODE IN (
            SELECT SUB_CODE 
            FROM FA_SUB_LEDGER_MAP 
            WHERE COMPANY_CODE = '{companyCode}'
        ) 
        AND COMPANY_CODE = '{companyCode}'";

            // Apply filter only if searchText has a value
            if (!string.IsNullOrWhiteSpace(searchText))
            {
                sqlQuery += $" AND UPPER(SUB_EDESC) LIKE '%' || UPPER('{searchText}') || '%' ";
            }

            sqlQuery += "";

            var result = _dbContext.SqlQuery<SubLedgerListModel>(sqlQuery).ToList();
            return result;
        }


        public List<FormattingFormDetailSetupModel> GetFormDetailSetupList(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string master_child_flag = "M";

            string query = $@"
                        SELECT serial_no,
                               table_name,
                               column_name,
                               column_header,
                               column_width,
                               top_position,
                               left_position,
                               master_child_flag,
                               is_desc_flag,
                               defa_value,
                               display_flag,
                               filter_value
                          FROM form_detail_setup
                         WHERE form_code = '{formCode}'
                           AND COMPANY_CODE = '{companyCode}'
                         ORDER BY CASE WHEN master_child_flag = '{master_child_flag}' THEN 0 ELSE 1 END,
                                  CASE WHEN master_child_flag = '{master_child_flag}' THEN TOP_POSITION ELSE 0 END,
                                  CASE WHEN master_child_flag = '{master_child_flag}' THEN LEFT_POSITION ELSE 0 END,
                                  serial_no";

            var result = _dbContext.SqlQuery<FormattingFormDetailSetupModel>(query).ToList();

            foreach (var item in result)
            {
                if (item.COLUMN_NAME == "CURRENCY_CODE")
                {
                    item.DEFA_VALUE_DESC = GetCurrencyDesc(item.DEFA_VALUE);
                }

                if (item.COLUMN_NAME == "PRIORITY_CODE")
                {
                    item.DEFA_VALUE_DESC = GetPriorityDesc(item.DEFA_VALUE);
                }
                if (item.COLUMN_NAME == "MU_CODE")
                {
                    item.DEFA_VALUE_DESC = GetMUDesc(item.DEFA_VALUE);
                }

                if (item.COLUMN_NAME == "ITEM_CODE")
                {
                    item.DEFA_VALUE_DESC = GetMUDesc(item.DEFA_VALUE);
                }

                if (item.COLUMN_NAME == "SALES_TYPE_CODE")
                {
                    item.DEFA_VALUE_DESC = GetSalesTypeDesc(item.DEFA_VALUE);
                }

                if (item.COLUMN_NAME == "CUSTOMER_CODE")
                {
                    string queryFilterValue = $@"
                        select CUSTOMER_EDESC from SA_CUSTOMER_SETUP where company_code='{companyCode}' and master_customer_code='{item.FILTER_VALUE}'";

                    item.FILTER_VALUE_DESC = _dbContext.SqlQuery<string>(queryFilterValue).FirstOrDefault();
                }

                if (item.COLUMN_NAME == "ITEM_CODE")
                {
                    string itemCodeFilterValueQuery = $@"SELECT ITEM_EDESC
                                                  FROM IP_ITEM_MASTER_SETUP
                                                 WHERE company_code = '{companyCode}' AND MASTER_ITEM_CODE = '{item.FILTER_VALUE}'";

                    item.FILTER_VALUE_DESC = _dbContext.SqlQuery<string>(itemCodeFilterValueQuery).FirstOrDefault();
                }


                if (item.COLUMN_NAME == "EMPLOYEE_CODE")
                {
                    string empCodeFilterValueQuery = $@"SELECT employee_edesc FROM hr_employee_setup WHERE DELETED_FLAG = 'N' AND COMPANY_CODE = '{companyCode}' AND master_employee_code = '{item.FILTER_VALUE}'";
                    item.FILTER_VALUE_DESC = _dbContext.SqlQuery<string>(empCodeFilterValueQuery).FirstOrDefault();
                }



            }


            return result;
        }



        public FormSetupFormatingModel FormattingTab_GetEditData(string form_code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"SELECT COINAGE_FLAG, NUMBERING_FORMAT, DATE_FORMAT, COINAGE_SUB_CODE from Form_Setup where Form_code='{form_code}'";
            var result = _dbContext.SqlQuery<FormSetupFormatingModel>(sqlQuery).FirstOrDefault();

            string sqlQuery1 = $@"SELECT TABLE_NAME FROM form_detail_setup WHERE UPPER(TRIM(FORM_CODE))= TRIM('{form_code}') AND COMPANY_CODE = '{companyCode}' AND DELETED_FLAG = 'N'";
            var table_name = _dbContext.SqlQuery<string>(sqlQuery1).FirstOrDefault();
            result.TABLE_NAME = table_name;

            return result;
        }

        public List<string> FormattingTab_GetUnmappedColumnList(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var table_Name = this.FormattingTab_GetTableNameList(formCode);
            string sqlQuery = $@"
                                SELECT CNAME
                                  FROM COL
                                 WHERE TNAME = '{table_Name}'
                                   AND CNAME NOT IN (
                                        SELECT COLUMN_NAME
                                          FROM FORM_DETAIL_SETUP
                                         WHERE TABLE_NAME = '{table_Name}'
                                           AND COMPANY_CODE = '{companyCode}'
                                           AND FORM_CODE = '{formCode}'
                                   )
                                   AND CNAME NOT IN (
                                        'COMPANY_CODE','BRANCH_CODE','FORM_CODE','CREATED_BY',
                                        'CREATED_DATE','DELETED_FLAG','SYN_ROWID','OMIT_FLAG',
                                        'TRACKING_NO','SESSION_ROWID','MODIFY_DATE','MODIFY_BY',
                                        'SERIAL_NO','SMS_FLAG'
                                   )
                                 ORDER BY CNAME";

            var result = _dbContext.SqlQuery<string>(sqlQuery).ToList();

            return result;
        }


        //public bool UpdateFormSetupFormattingTabInfo(FormSetupFormattingTabModel model)
        //{
        //    var companyCode = _workContext.CurrentUserinformation.company_code;
        //    var modifiedBy = _workContext.CurrentUserinformation.login_code;

        //    //var result = _dbContext.ExecuteSqlCommand(sqlQuery);

        //    return true;
        //}


        public bool UpdateFormSetupFormattingTabInfo(FormSetupFormattingTabModel modelData)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var modifiedBy = _workContext.CurrentUserinformation.login_code;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {

                    var form_code = modelData.FormDataInfoModel.FORM_CODE;
                    // delete code here 

                    string sqlQueryDeleteFormDetailSetup = $@"
                                        DELETE FROM FORM_DETAIL_SETUP
                                        WHERE COMPANY_CODE = '{companyCode}'
                                          AND FORM_CODE = '{modelData.FormDataInfoModel.FORM_CODE}'";
                    var result = _dbContext.ExecuteSqlCommand(sqlQueryDeleteFormDetailSetup);


                    foreach (var modelItem in modelData.FormattingFormDetailSetupList)
                    {
                        string sqlQueryInsert = $@"
                                            INSERT INTO FORM_DETAIL_SETUP 
                                                (SERIAL_NO, FORM_CODE, TABLE_NAME, COLUMN_NAME, COLUMN_WIDTH, COLUMN_HEADER, 
                                                 TOP_POSITION, LEFT_POSITION, DISPLAY_FLAG, DEFA_VALUE, IS_DESC_FLAG, 
                                                 MASTER_CHILD_FLAG, FILTER_VALUE, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG)
                                            VALUES 
                                                ('{modelItem.SERIAL_NO}', '{form_code}', '{modelItem.TABLE_NAME}', '{modelItem.COLUMN_NAME}', '{modelItem.COLUMN_WIDTH}', '{modelItem.COLUMN_HEADER}', 
                                                 '{modelItem.TOP_POSITION}', '{modelItem.LEFT_POSITION}', '{modelItem.DISPLAY_FLAG}', '{modelItem.DEFA_VALUE}', '{modelItem.IS_DESC_FLAG}', 
                                                 '{modelItem.MASTER_CHILD_FLAG}', '{modelItem.FILTER_VALUE}', '{companyCode}', '{modifiedBy}', SYSDATE, 'N')";

                        _dbContext.ExecuteSqlCommand(sqlQueryInsert);
                    }

                    string sqlQuery = $@"
                                    UPDATE FORM_SETUP
                                       SET 
                                           MODIFY_BY = '{modifiedBy}',
                                           NUMBERING_FORMAT = '{modelData.FormFormattingModel.NUMBERING_FORMAT}',
                                           DATE_FORMAT = '{modelData.FormFormattingModel.DATE_FORMAT}',
                                           COINAGE_FLAG = '{modelData.FormFormattingModel.COINAGE_FLAG}',
                                           COINAGE_SUB_CODE = '{modelData.FormFormattingModel.COINAGE_SUB_CODE}'
                                     WHERE FORM_CODE = '{form_code}'
                                       AND COMPANY_CODE = '{companyCode}'";

                    _dbContext.ExecuteSqlCommand(sqlQuery);




                    // ✅ Commit the transaction if all updates succeed
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // ❌ Rollback if any update fails
                    transaction.Rollback();
                    // Optional: log exception
                    throw new Exception("Error while updating form group items", ex);
                }
            }
        }



        public List<CustomerLookupDataModel> GetCustomersList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string groupSkuFlag = "I";
            string deletedFlag = "N";

            string query = $@"
                        SELECT CUSTOMER_CODE,
                               CUSTOMER_EDESC
                          FROM SA_CUSTOMER_SETUP
                         WHERE COMPANY_CODE = '{companyCode}'
                           AND group_sku_flag = '{groupSkuFlag}'
                           AND DELETED_FLAG = '{deletedFlag}'
                         ORDER BY CUSTOMER_EDESC";

            var result = _dbContext.SqlQuery<CustomerLookupDataModel>(query).ToList();
            return result;
        }

        public List<ItemMasterLookupModel> GetItemMasterList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string groupSkuFlag = "I";
            string deletedFlag = "N";

            string sqlQuery = $@"
                                SELECT ITEM_CODE, ITEM_EDESC
                                  FROM IP_ITEM_MASTER_SETUP
                                 WHERE COMPANY_CODE = '{companyCode}'
                                   AND GROUP_SKU_FLAG = '{groupSkuFlag}'
                                   AND DELETED_FLAG = '{deletedFlag}'
                                 ORDER BY ITEM_EDESC";

            var result = _dbContext.SqlQuery<ItemMasterLookupModel>(sqlQuery).ToList();
            return result;
        }


        public List<EmployeeLookupDataModel> GetEmployeeList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string groupSkuFlag = "I";
            string deletedFlag = "N";

            string query = $@"
                SELECT EMPLOYEE_CODE,
                       EMPLOYEE_EDESC
                  FROM HR_EMPLOYEE_SETUP
                 WHERE COMPANY_CODE = '{companyCode}'
                   AND group_sku_flag = '{groupSkuFlag}'
                   AND DELETED_FLAG = '{deletedFlag}'
                 ORDER BY EMPLOYEE_EDESC";

            var result = _dbContext.SqlQuery<EmployeeLookupDataModel>(query).ToList();
            return result;
        }

        public List<CurrencyLookupModel> GetCurrencyList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                SELECT CURRENCY_CODE,
                       CURRENCY_EDESC
                  FROM CURRENCY_SETUP
                 WHERE COMPANY_CODE = '{companyCode}'
                   AND DELETED_FLAG = 'N'
                 ORDER BY CURRENCY_EDESC";
            var result = _dbContext.SqlQuery<CurrencyLookupModel>(query).ToList();
            return result;
        }

        public string GetCurrencyDesc(string code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                select CURRENCY_EDESC from CURRENCY_SETUP where company_code='{companyCode}' and CURRENCY_CODE= '{code}'";
            var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }


        public List<MULookupModel> GetMUList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                            SELECT MU_CODE,
                                   MU_EDESC
                              FROM IP_MU_CODE
                             WHERE COMPANY_CODE = '{companyCode}'
                               AND DELETED_FLAG = 'N'
                             ORDER BY MU_EDESC";

            var result = _dbContext.SqlQuery<MULookupModel>(query).ToList();
            return result;
        }

        public string GetMUDesc(string code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
        SELECT MU_EDESC 
          FROM IP_MU_CODE 
         WHERE COMPANY_CODE = '{companyCode}' 
           AND MU_CODE = '{code}' 
           AND DELETED_FLAG = 'N'";

            var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }

        public List<SalesTypeLookupModel> GetSalesTypeList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                            SELECT SALES_TYPE_CODE,
                                   SALES_TYPE_EDESC
                              FROM SA_SALES_TYPE
                             WHERE COMPANY_CODE = '{companyCode}'";

            var result = _dbContext.SqlQuery<SalesTypeLookupModel>(query).ToList();
            return result;
        }

        public string GetSalesTypeDesc(string code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                            SELECT SALES_TYPE_EDESC 
                              FROM SA_SALES_TYPE 
                             WHERE COMPANY_CODE = '{companyCode}' 
                               AND SALES_TYPE_CODE = '{code}'";

            var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }


        public List<PriorityLookupModel> GetPriorityList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                        SELECT PRIORITY_CODE,
                               PRIORITY_EDESC
                          FROM IP_PRIORITY_CODE
                         WHERE COMPANY_CODE = '{companyCode}'
                           AND DELETED_FLAG = 'N'
                         ORDER BY PRIORITY_EDESC";

            var result = _dbContext.SqlQuery<PriorityLookupModel>(query).ToList();
            return result;
        }


        public string GetPriorityDesc(string code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                        SELECT PRIORITY_EDESC 
                          FROM IP_PRIORITY_CODE 
                         WHERE COMPANY_CODE = '{companyCode}' 
                           AND PRIORITY_CODE = '{code}' 
                           AND DELETED_FLAG = 'N'";

            var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }


        public class CustomerModel
        {
            public string MasterCustomerCode { get; set; }
            public string CustomerEDesc { get; set; }
            public string PreCustomerCode { get; set; }
        }

        public List<CustomerModel> GetCustomerList()
        {
            var customers = new List<CustomerModel>
                                {
                                    new CustomerModel { MasterCustomerCode = "01", CustomerEDesc = "Sundry Debtors", PreCustomerCode = "00" },
                                    new CustomerModel { MasterCustomerCode = "01.01", CustomerEDesc = "Dealers", PreCustomerCode = "01" },
                                    new CustomerModel { MasterCustomerCode = "01.01.01", CustomerEDesc = "Cable Industry", PreCustomerCode = "01.01" },
                                    new CustomerModel { MasterCustomerCode = "01.01.02", CustomerEDesc = "Grocery", PreCustomerCode = "01.01" },
                                    new CustomerModel { MasterCustomerCode = "01.01.03", CustomerEDesc = "Shoe PVC", PreCustomerCode = "01.01" },
                                    new CustomerModel { MasterCustomerCode = "01.01.04", CustomerEDesc = "INTERNAL", PreCustomerCode = "01.01" },
                                    new CustomerModel { MasterCustomerCode = "01.01.05", CustomerEDesc = "HTSG Group", PreCustomerCode = "01.01" },
                                    new CustomerModel { MasterCustomerCode = "01.01.06", CustomerEDesc = "Other Dealers", PreCustomerCode = "01.01" },
                                    new CustomerModel { MasterCustomerCode = "01.04", CustomerEDesc = "Project", PreCustomerCode = "01" },
                                    new CustomerModel { MasterCustomerCode = "01.05", CustomerEDesc = "Retail", PreCustomerCode = "01" },
                                    new CustomerModel { MasterCustomerCode = "01.06", CustomerEDesc = "Corporate", PreCustomerCode = "01" }
                                };

            return customers;
        }



        public List<CustomerTreeNode> BuildCustomerTree(List<CustomerLookupModel> customers)
        {
            // Step 1: Convert flat CustomerModel list to CustomerTreeNode list
            var nodes = customers.Select(c => new CustomerTreeNode
            {
                id = c.MASTER_CUSTOMER_CODE,
                text = c.CUSTOMER_EDESC,
                parentCode = c.PRE_CUSTOMER_CODE
            }).ToList();

            // Step 2: Build lookup for parent-child linking
            var lookup = nodes.ToDictionary(x => x.id, x => x);
            var rootNodes = new List<CustomerTreeNode>();

            foreach (var node in nodes)
            {
                if (node.parentCode == "00" || !lookup.ContainsKey(node.parentCode))
                {
                    // Root node (no valid parent)
                    rootNodes.Add(node);
                }
                else
                {
                    // Attach node to its parent
                    lookup[node.parentCode].items.Add(node);
                }
            }

            return rootNodes;
        }

        public List<CustomerTreeNode> GetCustomerTreeStructureList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            var DELETED_FLAG = "N";
            var GROUP_SKU_FLAG = "G";

            string query = $@"
                            SELECT MASTER_CUSTOMER_CODE,
                                   CUSTOMER_EDESC,
                                   PRE_CUSTOMER_CODE
                              FROM SA_CUSTOMER_SETUP
                             WHERE COMPANY_CODE = '{companyCode}'
                               AND DELETED_FLAG = '{DELETED_FLAG}'
                               AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                             ORDER BY MASTER_CUSTOMER_CODE";
            var result = _dbContext.SqlQuery<CustomerLookupModel>(query).ToList();


            //var result = GetCustomerList();
            var resultTreeData = BuildCustomerTree(result);

            List<CustomerTreeNode> tempList = new List<CustomerTreeNode>();
            var cu = new CustomerTreeNode()
            {
                text = "<Primary>",
                id = "00",
                parentCode = "00",
                items = resultTreeData
            };
            tempList.Add(cu);

            return tempList;
        }

        public List<CustomerDetailWIthIDModel> GetCustomerListByPreCode(string preCustomerCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                            SELECT PRE_CUSTOMER_CODE,
                                   CUSTOMER_CODE,
                                   CUSTOMER_EDESC,
                                   CUSTOMER_ID
                              FROM SA_CUSTOMER_SETUP
                             WHERE PRE_CUSTOMER_CODE LIKE '{preCustomerCode}%'
                               AND UPPER(CUSTOMER_EDESC) LIKE '%'
                               AND GROUP_SKU_FLAG = 'I'
                               AND COMPANY_CODE = '{companyCode}'
                               AND DELETED_FLAG = 'N'
                             ORDER BY UPPER(CUSTOMER_EDESC)";

            var result = _dbContext.SqlQuery<CustomerDetailWIthIDModel>(query).ToList();
            return result;
        }

        public CustomerDetailModel GetCustomerDetailByCode(string customerCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                            SELECT CUSTOMER_CODE CODE,
                               CUSTOMER_EDESC NAME,
                               REGD_OFFICE_EADDRESS ADDRESS,
                               TEL_MOBILE_NO1 TEL1,
                               TEL_MOBILE_NO2 TEL2,
                               EMAIL,
                               CREDIT_LIMIT,
                               CREDIT_DAYS,
                               TPIN_VAT_NO PAN,
                               (SELECT CITY_EDESC
                                  FROM CITY_CODE
                                 WHERE CITY_CODE = A.CITY_CODE 
                                   AND COMPANY_CODE = A.COMPANY_CODE) CITY,
                               EXCISE_NO,
                               '' AREA
                          FROM SA_CUSTOMER_SETUP A
                         WHERE COMPANY_CODE = '{companyCode}'
                           AND CUSTOMER_CODE = '{customerCode}'";

            var result = _dbContext.SqlQuery<CustomerDetailModel>(query).FirstOrDefault();
            return result;
        }


        public List<CustomerDetailWIthIDModel> GetCustomerListBySearch(string searchKey)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            searchKey = searchKey?.ToUpper() ?? "";


            var GROUP_SKU_FLAG = "I";
            var DELETED_FLAG = "N";
            string query = $@"
                            SELECT PRE_CUSTOMER_CODE,
                                   CUSTOMER_CODE,
                                   CUSTOMER_EDESC,
                                   CUSTOMER_ID
                              FROM SA_CUSTOMER_SETUP
                             WHERE (UPPER(CUSTOMER_EDESC) LIKE '{searchKey}%'
                                    OR UPPER(CUSTOMER_CODE) LIKE '{searchKey}%'
                                    OR UPPER(CUSTOMER_ID) LIKE '{searchKey}%')
                               AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                               AND COMPANY_CODE = '{companyCode}'
                               AND DELETED_FLAG = '{DELETED_FLAG}'
                             ORDER BY UPPER(CUSTOMER_EDESC)";

            var result = _dbContext.SqlQuery<CustomerDetailWIthIDModel>(query).ToList();
            return result;
        }



        public List<ItemGroupedTreeNode> BuildItemGroupedTree(List<ItemGroupLookupModel> items)
        {

            // Step 1: Convert flat CustomerModel list to CustomerTreeNode list
            var nodes = items.Select(i => new ItemGroupedTreeNode
            {
                id = i.MASTER_ITEM_CODE,
                text = i.ITEM_EDESC,
                parentCode = i.PRE_ITEM_CODE
            }).ToList();

            // Step 2: Build lookup for parent-child linking
            var lookup = nodes.ToDictionary(x => x.id, x => x);
            var rootNodes = new List<ItemGroupedTreeNode>();

            foreach (var node in nodes)
            {
                if (node.parentCode == "00" || !lookup.ContainsKey(node.parentCode))
                {
                    // Root node (no valid parent)
                    rootNodes.Add(node);
                }
                else
                {
                    // Attach node to its parent
                    lookup[node.parentCode].items.Add(node);
                }
            }
            return rootNodes;
        }

        public List<ItemGroupedTreeNode> GetItemGroupList()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                        SELECT MASTER_ITEM_CODE,
                               ITEM_EDESC,
                               PRE_ITEM_CODE
                          FROM IP_ITEM_MASTER_SETUP
                         WHERE DELETED_FLAG = 'N'
                           AND COMPANY_CODE = '{companyCode}'
                           AND GROUP_SKU_FLAG = 'G'
                         ORDER BY MASTER_ITEM_CODE";

            var result = _dbContext.SqlQuery<ItemGroupLookupModel>(query).ToList();
            var resultTreeData = BuildItemGroupedTree(result);

            List<ItemGroupedTreeNode> tempList = new List<ItemGroupedTreeNode>();
            var cu = new ItemGroupedTreeNode()
            {
                text = "<Primary>",
                id = "00",
                parentCode = "00",
                items = resultTreeData
            };
            tempList.Add(cu);

            return tempList;
        }


        public List<ItemDetailWithIDModel> GetItemListByPreCode(string preItemCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var GROUP_SKU_FLAG = "I";
            string query = $@"
                            SELECT PRE_ITEM_CODE,
                                   ITEM_CODE,
                                   ITEM_EDESC,
                                   PRODUCT_CODE
                              FROM IP_ITEM_MASTER_SETUP
                             WHERE PRE_ITEM_CODE LIKE '{preItemCode}%'
                               AND UPPER(ITEM_EDESC) LIKE '%'
                               AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                               AND COMPANY_CODE = '{companyCode}'
                               AND DELETED_FLAG = 'N'
                               AND FREEZE_FLAG = 'N'
                             ORDER BY UPPER(ITEM_EDESC)";

            var result = _dbContext.SqlQuery<ItemDetailWithIDModel>(query).ToList();
            return result;
        }

        public ItemDetailModel GetItemDetailByItemCode(string itemCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string query = $@"
                    SELECT ITEM_CODE CODE,
                           ITEM_EDESC NAME,
                           CATEGORY_EDESC CATEGORY,
                           INDEX_MU_CODE UNIT,
                           (SELECT MU_CODE 
                              FROM IP_ITEM_UNIT_SETUP 
                             WHERE ITEM_CODE = A.ITEM_CODE 
                               AND COMPANY_CODE = A.COMPANY_CODE 
                               AND SERIAL_NO = 1) ALT_UNIT,
                           A.HS_CODE
                      FROM IP_ITEM_MASTER_SETUP A,
                           IP_CATEGORY_CODE B
                     WHERE A.COMPANY_CODE = '{companyCode}'
                       AND A.ITEM_CODE = '{itemCode}'
                       AND A.COMPANY_CODE = B.COMPANY_CODE(+)
                       AND A.CATEGORY_CODE = B.CATEGORY_CODE(+)";

            var result = _dbContext.SqlQuery<ItemDetailModel>(query).FirstOrDefault();
            return result;
        }


        public List<EmployeeGroupedTreeNode> GetEmployeeGroupedTreeData()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var GROUP_SKU_FLAG = "G";

            string query = $@"
                    SELECT MASTER_EMPLOYEE_CODE,
                           EMPLOYEE_EDESC,
                           PRE_EMPLOYEE_CODE
                      FROM HR_EMPLOYEE_SETUP
                     WHERE DELETED_FLAG = 'N'
                       AND COMPANY_CODE = '{companyCode}'
                       AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                  ORDER BY MASTER_EMPLOYEE_CODE";

            var empGroupdData = _dbContext.SqlQuery<EmployeeGroupModel>(query).FirstOrDefault();

            var listObj = new List<EmployeeGroupedTreeNode>();
            listObj.Add(new EmployeeGroupedTreeNode()
            {
                id = empGroupdData.MASTER_EMPLOYEE_CODE,
                text = empGroupdData.EMPLOYEE_EDESC,
                parentCode = empGroupdData.PRE_EMPLOYEE_CODE,
                items = new List<EmployeeGroupedTreeNode>()
            });
            var finalList = new EmployeeGroupedTreeNode()
            {
                text = "<Primary>",
                id = "01",
                parentCode = "00",
                items = listObj
            };
            var finalResiltList = new List<EmployeeGroupedTreeNode>();

            finalResiltList.Add(finalList);
            return finalResiltList;
        }


        public List<EmployeeDetailWithIDModel> GetEmployeeListByPreCode(string preEmployeeCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var GROUP_SKU_FLAG = "I";

            string query = $@"
                    SELECT PRE_EMPLOYEE_CODE,
                           EMPLOYEE_CODE,
                           EMPLOYEE_EDESC,
                           EMPLOYEE_CODE EMP_ID
                      FROM HR_EMPLOYEE_SETUP
                     WHERE PRE_EMPLOYEE_CODE LIKE '{preEmployeeCode}%'
                       AND UPPER(EMPLOYEE_EDESC) LIKE '%'
                       AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                       AND COMPANY_CODE = '{companyCode}'
                       AND DELETED_FLAG = 'N'
                  ORDER BY UPPER(EMPLOYEE_EDESC)";

            var result = _dbContext.SqlQuery<EmployeeDetailWithIDModel>(query).ToList();
            return result;
        }









        // Reference tab info

        public FormSetupReferenceTabInfoModel GetReferenceTabData(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"SELECT
                            FORM_CODE,
                            REFERENCE_FLAG,
                            REF_TABLE_NAME,
                            REF_COLUMN_NAME,
                            REF_FORM_CODE,
                            FREEZE_MANUAL_ENTRY_FLAG,
                            PUBLIC_FLAG,
                            AFTER_VERIFY_FLAG,
                            ORDER_DISPATCH_FLAG,
                            AFTER_POSTING_FLAG,
                            FREEZE_MASTER_REF_FLAG,
                            ORDER_DISPATCH_FLAG,
                            ORDER_ACCESS_FAB_FLAG,

                            INVOICE_PJV_FORM_CODE,
                            RECEIPT_FLAG,
                            RECEIPT_FORM_CODE,
                            RECEIPT_CASH_ACC_CODE,
                            DEFAULT_FROM_LOCATION,
                            DEFAULT_SHIPMENT

                        FROM FORM_SETUP
                        WHERE FORM_CODE = '{formCode}' AND COMPANY_CODE = '{companyCode}'";

            // SELECT FORM_EDESC, FORM_CODE FROM FORM_SETUP where FORM_CODE = 505;





            var result = _dbContext.SqlQuery<FormSetupReferenceTabInfoModel>(sqlQuery).FirstOrDefault();

            if (!string.IsNullOrEmpty(result.INVOICE_PJV_FORM_CODE))
            {
                var tmpQuery = $@"SELECT FORM_EDESC FROM FORM_SETUP where FORM_CODE = '{result.INVOICE_PJV_FORM_CODE}'";
                result.INVOICE_PJV_FORM_EDESC = _dbContext.SqlQuery<string>(tmpQuery).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(result.RECEIPT_FORM_CODE))
            {
                var tmpQuery = $@"SELECT FORM_EDESC FROM FORM_SETUP where FORM_CODE = '{result.RECEIPT_FORM_CODE}'";
                result.RECEIPT_FORM_CODE_EDESC = _dbContext.SqlQuery<string>(tmpQuery).FirstOrDefault();
            }

            if (!string.IsNullOrEmpty(result.RECEIPT_CASH_ACC_CODE))
            {
                var tmpQuery = $@"select ACC_EDESC from FA_CHART_OF_ACCOUNTS_SETUP where ACC_CODE = '{result.RECEIPT_CASH_ACC_CODE}' and COMPANY_CODE = '{companyCode}'";
                result.RECEIPT_CASH_ACC_CODE_EDESC = _dbContext.SqlQuery<string>(tmpQuery).FirstOrDefault();
            }

            var branchList = GetBranchListByFormCode(formCode);


            if (branchList.Count() > 0)
            {
                result.BRANCH_LIST = new List<BranchListModel>();
                result.BRANCH_LIST.AddRange(branchList);
            }

            return result;
        }

        public List<TransactionTableListModel> GetTransactionTableListWithoutVoucher()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                        SELECT TABLE_NAME, TABLE_DESC
                          FROM TRANSACTION_TABLE_LIST
                         WHERE TABLE_NAME NOT LIKE '%VOUCHER%'
                         ORDER BY TABLE_NAME";

            var result = _dbContext.SqlQuery<TransactionTableListModel>(sqlQuery).ToList();
            return result;
        }



        public object GetQuotationFormList(string table_name)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                                SELECT form_code AS form_code, form_edesc AS form_edesc
                                  FROM form_setup
                                 WHERE COMPANY_CODE = '{companyCode}'
                                   AND form_code IN (
                                        SELECT DISTINCT form_code
                                          FROM form_detail_setup
                                         WHERE table_name = '{table_name}'
                                           AND COMPANY_CODE = '{companyCode}'
                                   )
                                UNION ALL
                                SELECT '0' AS FormCode, 'ALL DOCUMENTS' AS FormEDesc FROM DUAL";

            var result = _dbContext.SqlQuery<FormDropdownModel>(sqlQuery).ToList();

            string sqlQuery1 = $@"SELECT column_name FROM transaction_table_list WHERE table_name = '{table_name}'";
            var result1 = _dbContext.SqlQuery<string>(sqlQuery1).FirstOrDefault();

            return new { result, result1 };
        }

        //        SELECT FORM_EDESC, FORM_CODE
        //    FROM FORM_SETUP
        //   WHERE FORM_CODE IN
        //                (SELECT form_code
        //                   FROM form_detail_setup
        //                  WHERE COMPANY_CODE = '01'
        //                        AND TABLE_NAME = 'FA_DOUBLE_VOUCHER'
        //                        AND DELETED_FLAG = 'N')
        //         AND COMPANY_CODE = '01'
        //         AND GROUP_SKU_FLAG = 'I'
        //         AND DELETED_FLAG = 'N'
        //ORDER BY FORM_EDESC


        public object GetFormListForInvoiceToBeMatched()
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            string table_name = "FA_DOUBLE_VOUCHER";
            string GROUP_SKU_FLAG = "I";
            string sqlQuery = $@"
                        SELECT FORM_EDESC,
                               FORM_CODE 
                          FROM FORM_SETUP
                         WHERE FORM_CODE IN (
                                    SELECT FORM_CODE
                                      FROM FORM_DETAIL_SETUP
                                     WHERE COMPANY_CODE = '{companyCode}'
                                       AND TABLE_NAME = '{table_name}'
                                       AND DELETED_FLAG = 'N')
                           AND COMPANY_CODE = '{companyCode}'
                           AND GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'
                           AND DELETED_FLAG = 'N'
                      ORDER BY FORM_EDESC";

            var result = _dbContext.SqlQuery<FormDropdownModel>(sqlQuery).ToList();

            return result;
        }

        //public bool UpdateFormReferenceTabData(FormSetupReferenceTabInfoModel modelData)
        //{
        //    var companyCode = _workContext.CurrentUserinformation.company_code;
        //    var modifiedBy = _workContext.CurrentUserinformation.login_code;

        //    string sqlQuery = $@"
        //        UPDATE FORM_SETUP
        //           SET 
        //               REFERENCE_FLAG = '{modelData.REFERENCE_FLAG}',
        //               REF_TABLE_NAME = '{modelData.REF_TABLE_NAME}',
        //               REF_COLUMN_NAME = '{modelData.REF_COLUMN_NAME}',
        //               REF_FORM_CODE = '{modelData.REF_FORM_CODE}',
        //               FREEZE_MANUAL_ENTRY_FLAG = '{modelData.FREEZE_MANUAL_ENTRY_FLAG}',
        //               PUBLIC_FLAG = '{modelData.PUBLIC_FLAG}',
        //               AFTER_VERIFY_FLAG = '{modelData.AFTER_VERIFY_FLAG}',
        //               ORDER_DISPATCH_FLAG = '{modelData.ORDER_DISPATCH_FLAG}',
        //               AFTER_POSTING_FLAG = '{modelData.AFTER_POSTING_FLAG}',
        //               FREEZE_MASTER_REF_FLAG = '{modelData.FREEZE_MASTER_REF_FLAG}',
        //               ORDER_ACCESS_FAB_FLAG = '{modelData.ORDER_ACCESS_FAB_FLAG}',

        //               INVOICE_PJV_FORM_CODE = '{modelData.INVOICE_PJV_FORM_CODE}',
        //               RECEIPT_FLAG = '{modelData.RECEIPT_FLAG}',
        //               RECEIPT_FORM_CODE = '{modelData.RECEIPT_FORM_CODE}',
        //               RECEIPT_CASH_ACC_CODE = '{modelData.RECEIPT_CASH_ACC_CODE}',
        //               DEFAULT_FROM_LOCATION = '{modelData.DEFAULT_FROM_LOCATION}',
        //               DEFAULT_SHIPMENT = '{modelData.DEFAULT_SHIPMENT}',

        //               MODIFY_BY = '{modifiedBy}',
        //                 WHERE FORM_CODE = '{modelData.FORM_CODE}'
        //                   AND COMPANY_CODE = '{companyCode}'";

        //    if (modelData.BRANCH_LIST.Count() > 0)
        //    {
        //        foreach (var item in modelData.BRANCH_LIST)
        //        {
        //            string itemSqlQuery = $@"
        //                            INSERT INTO FORM_BRANCH_MAP 
        //                                (FORM_CODE, BRANCH_CODE, COMPANY_CODE, DELETED_FLAG, CREATED_BY, CREATED_DATE, MODIFY_DATE, MODIFY_BY)
        //                            VALUES 
        //                                ('{modelData.FORM_CODE}', '{item.BRANCH_CODE}', '{companyCode}', 'N', '{modifiedBy}', 
        //                                 SYSDATE,
        //                                 SYSDATE, 
        //                                 '{modifiedBy}')";
        //            _dbContext.ExecuteSqlCommand(itemSqlQuery);
        //        }
        //    }

        //    _dbContext.ExecuteSqlCommand(sqlQuery);
        //    return true;
        //}


        public bool UpdateFormReferenceTabData(FormSetupReferenceTabInfoModel modelData)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var modifiedBy = _workContext.CurrentUserinformation.login_code;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    // 1️⃣ Update FORM_SETUP
                    string sqlQuery = $@"
                UPDATE FORM_SETUP
                   SET 
                       REFERENCE_FLAG = '{modelData.REFERENCE_FLAG}',
                       REF_TABLE_NAME = '{modelData.REF_TABLE_NAME}',
                       REF_COLUMN_NAME = '{modelData.REF_COLUMN_NAME}',
                       REF_FORM_CODE = '{modelData.REF_FORM_CODE}',
                       FREEZE_MANUAL_ENTRY_FLAG = '{modelData.FREEZE_MANUAL_ENTRY_FLAG}',
                       PUBLIC_FLAG = '{modelData.PUBLIC_FLAG}',
                       AFTER_VERIFY_FLAG = '{modelData.AFTER_VERIFY_FLAG}',
                       ORDER_DISPATCH_FLAG = '{modelData.ORDER_DISPATCH_FLAG}',
                       AFTER_POSTING_FLAG = '{modelData.AFTER_POSTING_FLAG}',
                       FREEZE_MASTER_REF_FLAG = '{modelData.FREEZE_MASTER_REF_FLAG}',
                       ORDER_ACCESS_FAB_FLAG = '{modelData.ORDER_ACCESS_FAB_FLAG}',
                       INVOICE_PJV_FORM_CODE = '{modelData.INVOICE_PJV_FORM_CODE}',
                       RECEIPT_FLAG = '{modelData.RECEIPT_FLAG}',
                       RECEIPT_FORM_CODE = '{modelData.RECEIPT_FORM_CODE}',
                       RECEIPT_CASH_ACC_CODE = '{modelData.RECEIPT_CASH_ACC_CODE}',
                       DEFAULT_FROM_LOCATION = '{modelData.DEFAULT_FROM_LOCATION}',
                       DEFAULT_SHIPMENT = '{modelData.DEFAULT_SHIPMENT}',
                       MODIFY_BY = '{modifiedBy}',
                       MODIFY_DATE = SYSDATE
                 WHERE FORM_CODE = '{modelData.FORM_CODE}'
                   AND COMPANY_CODE = '{companyCode}'";

                    _dbContext.ExecuteSqlCommand(sqlQuery);

                    // 2️⃣ Delete existing branch mappings (optional: to refresh)
                    string deleteQuery = $@"
                DELETE FROM FORM_BRANCH_MAP 
                 WHERE FORM_CODE = '{modelData.FORM_CODE}'
                   AND COMPANY_CODE = '{companyCode}'";
                    _dbContext.ExecuteSqlCommand(deleteQuery);

                    // 3️⃣ Insert new branch mappings
                    if (modelData.BRANCH_LIST != null && modelData.BRANCH_LIST.Any())
                    {
                        foreach (var item in modelData.BRANCH_LIST.Where(x => x.CHECKCED == "X"))
                        {
                            string itemSqlQuery = $@"
                        INSERT INTO FORM_BRANCH_MAP 
                            (FORM_CODE, BRANCH_CODE, COMPANY_CODE, DELETED_FLAG, CREATED_BY, CREATED_DATE, MODIFY_DATE, MODIFY_BY)
                        VALUES 
                            ('{modelData.FORM_CODE}', '{item.BRANCH_CODE}', '{companyCode}', 'N', '{modifiedBy}', 
                             SYSDATE, 
                             SYSDATE, 
                             '{modifiedBy}')";
                            _dbContext.ExecuteSqlCommand(itemSqlQuery);
                        }
                    }

                    // 4️⃣ Commit if everything succeeds
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    // Rollback if anything fails
                    transaction.Rollback();
                    // Optionally log the error
                    throw new Exception("Error updating form reference tab data.", ex);
                }
            }
        }


        public List<BranchListModel> GetBranchListByFormCode(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                                SELECT DISTINCT
                                       BRANCH_CODE,
                                       BRANCH_EDESC,
                                       (SELECT DISTINCT 'X'
                                          FROM FORM_BRANCH_MAP
                                         WHERE FORM_CODE = '{formCode}'
                                               AND BRANCH_CODE = FA_BRANCH_SETUP.BRANCH_CODE)
                                         AS CHECKCED
                                  FROM FA_BRANCH_SETUP
                                 WHERE COMPANY_CODE = '{companyCode}'
                                   AND DELETED_FLAG = 'N'
                                   AND GROUP_SKU_FLAG = 'I'
                              ORDER BY TO_NUMBER(REGEXP_REPLACE(BRANCH_CODE, '[^0-9]', ''))";

            var result = _dbContext.SqlQuery<BranchListModel>(sqlQuery).ToList();
            return result;
        }


        public List<AccountListModel> GetAccountList(string searchText = "")
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                SELECT ACC_EDESC, ACC_CODE, ACC_CODE
                  FROM FA_CHART_OF_ACCOUNTS_SETUP
                 WHERE COMPANY_CODE = '{companyCode}'
                   AND ACC_TYPE_FLAG = 'T'
                   AND DELETED_FLAG = 'N'
                   AND UPPER(ACC_EDESC) LIKE UPPER('%{searchText}%')";

            var result = _dbContext.SqlQuery<AccountListModel>(sqlQuery).ToList();
            return result;
        }





        public FormSetupNumberingTabInfoModel GetNumberingTabData(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"SELECT
                            id_generation_flag,
                               custom_prefix_text,
                               custom_suffix_text,
                               TO_CHAR (prefix_length) as prefix_length,
                               TO_CHAR (suffix_length) as suffix_length,
                               TO_CHAR (body_length) as body_length,
                               TO_CHAR (start_no) as start_no,
                               TO_CHAR (last_no) as last_no,
                               start_date,
                               last_date,
	                           back_date_vno_save_flag,
                                CUSTOM_MANUAL_PREFIX_TEXT,
                                CUSTOM_MANUAL_SUFFIX_TEXT,
                                BODY_MANUAL_LENGTH,
                                START_MANUAL_NO,
                                FORM_CODE
                        FROM FORM_SETUP
                        WHERE FORM_CODE = '{formCode}' AND COMPANY_CODE = '{companyCode}'";

            var result = _dbContext.SqlQuery<FormSetupNumberingTabInfoModel>(sqlQuery).FirstOrDefault();

            return result;
        }

        public bool UpdateFormNumberingTabData(FormSetupNumberingTabInfoModel modelData)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            var modifiedBy = _workContext.CurrentUserinformation.login_code;

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var startDate = Convert.ToDateTime(modelData.START_DATE).ToString("yyyy-MM-dd");
                    var lastDate = Convert.ToDateTime(modelData.LAST_DATE).ToString("yyyy-MM-dd");


                    // ---------- VALIDATION: Check duplicate prefix ----------
                    string duplicateCheckQuery = $@"
                                                    SELECT COUNT(*) 
                                                      FROM FORM_SETUP
                                                     WHERE COMPANY_CODE = '{companyCode}'
                                                       AND CUSTOM_PREFIX_TEXT = '{modelData.CUSTOM_PREFIX_TEXT}'
                                                       AND FORM_CODE <> '{modelData.FORM_CODE}'
                                                       AND DELETED_FLAG = 'N'";

                    int duplicateCount = _dbContext.Database.SqlQuery<int>(duplicateCheckQuery).FirstOrDefault();

                    if (duplicateCount > 0)
                    {
                        // prefix already exists for another form_code
                        //return new
                        //{
                        //    Success = false,
                        //    Message = "Prefix already assigned to another form. Please use a different prefix."
                        //};

                        throw new ValidationException("Prefix already assigned to another form. Please use a different prefix.");

                    }



                    string sqlQuery = $@"
                UPDATE FORM_SETUP
                   SET 
                       ID_GENERATION_FLAG = '{modelData.ID_GENERATION_FLAG}',
                       CUSTOM_PREFIX_TEXT = '{modelData.CUSTOM_PREFIX_TEXT}',
                       CUSTOM_SUFFIX_TEXT = '{modelData.CUSTOM_SUFFIX_TEXT}',
                       PREFIX_LENGTH = '{modelData.PREFIX_LENGTH}',
                       SUFFIX_LENGTH = '{modelData.SUFFIX_LENGTH}',
                       BODY_LENGTH   = '{modelData.BODY_LENGTH}',
                       START_NO      = '{modelData.START_NO}',
                       LAST_NO       = '{modelData.LAST_NO}',
                       START_DATE    = TO_DATE('{startDate}', 'YYYY-MM-DD'),
                       LAST_DATE     = TO_DATE('{lastDate}', 'YYYY-MM-DD'),
                       BACK_DATE_VNO_SAVE_FLAG = '{modelData.BACK_DATE_VNO_SAVE_FLAG}',
                       CUSTOM_MANUAL_PREFIX_TEXT = '{modelData.CUSTOM_MANUAL_PREFIX_TEXT}',
                       CUSTOM_MANUAL_SUFFIX_TEXT = '{modelData.CUSTOM_MANUAL_SUFFIX_TEXT}',
                       BODY_MANUAL_LENGTH = '{modelData.BODY_MANUAL_LENGTH}',
                       START_MANUAL_NO    = '{modelData.START_MANUAL_NO}',
                       MODIFY_BY = '{modifiedBy}',
                       MODIFY_DATE = SYSDATE
                 WHERE FORM_CODE = '{modelData.FORM_CODE}'
                   AND COMPANY_CODE = '{companyCode}'";

                    _dbContext.ExecuteSqlCommand(sqlQuery);

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }


        public List<ChargeCodeModel> GetChargeCodeList(string form_code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            //string sqlQuery = $@"
            //    SELECT 
            //        CHARGE_CODE,
            //        CHARGE_EDESC
            //    FROM IP_CHARGE_CODE
            //    WHERE COMPANY_CODE = '{companyCode}'";

            string sqlQuery = $@"
                SELECT icc.CHARGE_CODE, icc.CHARGE_EDESC
                      FROM    IP_CHARGE_CODE icc
                           INNER JOIN
                              charge_setup cs
                           ON CS.CHARGE_CODE = ICC.CHARGE_CODE
                      WHERE    CS.COMPANY_CODE = '{companyCode}'
                           AND CS.FORM_CODE = '{form_code}'
                           AND ICC.COMPANY_CODE = '{companyCode}'";

            var result = _dbContext.SqlQuery<ChargeCodeModel>(sqlQuery).ToList();

            return result;
        }

        public List<AccountInfoModel> ChargeSetupGetAccountList(string searchText)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            // If searchText is null/empty, use empty string for LIKE '%%'
            if (string.IsNullOrWhiteSpace(searchText))
            {
                searchText = "";
            }
            string sqlQuery = $@"SELECT 
                                                ACC_EDESC, 
                                                ACC_CODE, 
                                                ACC_CODE 
                                            FROM FA_CHART_OF_ACCOUNTS_SETUP
                                            WHERE 
                                                COMPANY_CODE = '{companyCode}'
                                                AND ACC_TYPE_FLAG = 'T'
                                                AND DELETED_FLAG = 'N'
                                                AND UPPER(ACC_EDESC) LIKE '%{searchText.ToUpper()}%'";
            var result = _dbContext.SqlQuery<AccountInfoModel>(sqlQuery).ToList();
            return result;
        }



        public ChargeSetupDataModel GetChargeSetupData(string formCode, string chargeCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                            SELECT 
                                  FORM_CODE,
                                  ACC_CODE,
                                    LINK_SUB_CODE,
                                    CHARGE_TYPE_FLAG,
                                    GL_FLAG,
                                    PRIORITY_INDEX_NO,
                                    CHARGE_APPLY_ON,
                                    VALUE_PERCENT_FLAG,
                                    VALUE_PERCENT_AMOUNT,
                                    APPLY_FROM_DATE,
                                    APPLY_TO_DATE,
                                    APPLY_ON,
                                    CHARGE_ACTIVE_FLAG,
                                    NON_GL_FLAG,
                                    CALC_ITEM_BASED_ON,
                                    MANUAL_CALC_CHARGE,
                                    IMPACT_ON,
                                    APPORTION_ON
                            FROM charge_setup
                            WHERE company_code = '{companyCode}'
                              AND form_code = '{formCode}'
                              AND charge_code = '{chargeCode}'";

            var result = _dbContext.SqlQuery<ChargeSetupDataModel>(sqlQuery).FirstOrDefault();

            if (!string.IsNullOrEmpty(result.ACC_CODE))
            {
                var tmpQuery = $@"select ACC_EDESC from FA_CHART_OF_ACCOUNTS_SETUP where ACC_CODE = '{result.ACC_CODE}' and COMPANY_CODE = '{companyCode}'";
                result.ACC_CODE_EDESC = _dbContext.SqlQuery<string>(tmpQuery).FirstOrDefault();
            }

            return result;
        }

        public bool UpdateChargeSetupData(ChargeSetupDataModel model)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                UPDATE CHARGE_SETUP SET
                    ACC_CODE = '{model.ACC_CODE}',
                    LINK_SUB_CODE = '{model.LINK_SUB_CODE}',
                    CHARGE_TYPE_FLAG = '{model.CHARGE_TYPE_FLAG}',
                    GL_FLAG = '{model.GL_FLAG}',
                    PRIORITY_INDEX_NO = {model.PRIORITY_INDEX_NO},
                    CHARGE_APPLY_ON = '{model.CHARGE_APPLY_ON}',
                    VALUE_PERCENT_FLAG = '{model.VALUE_PERCENT_FLAG}',
                    VALUE_PERCENT_AMOUNT = '{model.VALUE_PERCENT_AMOUNT}',
                    APPLY_FROM_DATE = TO_DATE('{model.APPLY_FROM_DATE:yyyy-MM-dd}', 'YYYY-MM-DD'),
                    APPLY_TO_DATE = TO_DATE('{model.APPLY_TO_DATE:yyyy-MM-dd}', 'YYYY-MM-DD'),
                    APPLY_ON = '{model.APPLY_ON}',
                    CHARGE_ACTIVE_FLAG = '{model.CHARGE_ACTIVE_FLAG}',
                    NON_GL_FLAG = '{model.NON_GL_FLAG}',
                    CALC_ITEM_BASED_ON = '{model.CALC_ITEM_BASED_ON}',
                    MANUAL_CALC_CHARGE = '{model.MANUAL_CALC_CHARGE}',
                    IMPACT_ON = '{model.IMPACT_ON}',
                    APPORTION_ON = '{model.APPORTION_ON}'
                WHERE COMPANY_CODE = '{companyCode}'
                  AND FORM_CODE = '{model.FORM_CODE}'
                  AND CHARGE_CODE = '{model.CHARGE_CODE}'
            ";

            _dbContext.ExecuteSqlCommand(sqlQuery);

            return true;
        }

        public object GetQualityCheckTabData(string form_code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"SELECT
                                SERIAL_TRACKING_FLAG,
                                BATCH_TRACKING_FLAG,
                                FORM_CODE
                                 FROM form_setup
                                 WHERE     DELETED_FLAG = 'N'
                                       AND COMPANY_CODE = '{companyCode}'
                                       AND form_code = '{form_code}'";
            var result = _dbContext.SqlQuery<QualityCheckGetDataModel>(sqlQuery).FirstOrDefault();

            return result;
        }


        public bool UpdateChargeSetupData(QualityCheckGetDataModel model)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            if (model.batchSerialFlag == "None")
            {
                model.SERIAL_TRACKING_FLAG = "N";
                model.BATCH_TRACKING_FLAG = "N";
            }

            if (model.batchSerialFlag == "Batch")
            {
                model.BATCH_TRACKING_FLAG = "Y";
            }

            if (model.batchSerialFlag == "Serial")
            {
                model.SERIAL_TRACKING_FLAG = "Y";
            }

            string sqlQuery = $@"
            UPDATE form_setup
               SET 
                    serial_tracking_flag = '{model.SERIAL_TRACKING_FLAG}',
                    batch_tracking_flag  = '{model.BATCH_TRACKING_FLAG}'
             WHERE 
                    DELETED_FLAG = 'N'
                AND COMPANY_CODE = '{companyCode}'
                AND form_code = '{model.FORM_CODE}'";

            var rowsAffected = _dbContext.ExecuteSqlCommand(sqlQuery);

            return true;
        }


        public object GetMiscellaneousTabData(string form_code)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"SELECT

                                   FORM_CODE,
                                   MULTI_UNIT_FLAG,             ---
                                   PRINT_REPORT_FLAG,           --- Print voucher after saving record --
                                   PUBLIC_FLAG,                 --- Make available for current user only --
                                   PRICE_CONTROL_FLAG,          --- Price Control --
                                   RATE_SCHEDULE_FIX_PRICE,     --- Freeze Rate Schedule ---
                                   PRIMARY_MANUAL_FLAG,         --- Replicate Manual No. as Voucher No --
                                   MULTI_UNIT_FLAG,             --- Convert in Multi Unit ---
                                   TOTAL_ROUND_FLAG,            --- Rounding on total -- 
                                   COPY_VALUES_FLAG,            --- Copy common values at each new record insert--                                  
                                   ACCESS_BDFSM_FLAG,           --- Back Days Access for Same Month ---
                                   NEGATIVE_STOCK_FLAG,         --- Block Negative Stock--
                                   FREEQTY_FLAG,                --- Freeze Discount Schedule Rate--
                                   DC_VAT_FLAG,                 --- DC VAT Flag,--
                                   COMMITMENT_FLAG,             --- Party Commitment Flag--
                                   PURCHASE_MRR_GRNI_FLAG,      --- Purchase GRNI Account--
                                   DISPLAY_RATE,                --- Display Rate Schedule--
                                   PENDING_INFO_FLAG,           --- Pending Indent/ Order Info---
                                   WM_FLAG,                     --- Warehouse Management Flag--
                                   OTHER_INFO_FLAG,             --- Other Infor Transection--
                                   VNO_AS_DOC_ID_CONTROL,       --- V No. Generate as Session ID--
                                   REF_FIX_QUANTITY,            --- Freeze Referenced Quantity --
                                   REF_FIX_PRICE,               --- Freeze Referenced Units Price --
                                   DISCOUNT_SCHEDULE_FLAG,      --- Freeze Discount Schedule Rate --
                                   INFO_FLAG,                   --- Ledger/Stock Information Flag ---

                                   FREEZE_BACK_DAYS,            --- Freeze Back Days ---
                                   DECIMAL_PLACE,               --- Upto Decimal ---
                                   MAX_ROWS,                    --- Max Rows ---
                                   TOTAL_ROUND_INDEX,            --- Upto Rounding ---
                                   REPORT_NO,
                                   PURCHASE_INVOICE_MRR_FLAG,
                                   AUTO_GL_POST,
                                   SALES_INVOICE_CHALAN_FLAG

                                  FROM form_setup
                                 WHERE     DELETED_FLAG = 'N'
                                       AND COMPANY_CODE = '{companyCode}'
                                       AND form_code = '{form_code}'";

            var result = _dbContext.SqlQuery<MiscellaneousTabDataModel>(sqlQuery).FirstOrDefault();

            if (result.REPORT_NO != null && result.REPORT_NO > 0)
            {
                var tmpQuery = $@"SELECT report_edesc FROM document_report_setup WHERE report_no = '{result.REPORT_NO}' and COMPANY_CODE = '{companyCode}'";
                result.REPORT_EDESC = _dbContext.SqlQuery<string>(tmpQuery).FirstOrDefault();
            }

            return result;
        }



        public object GetDocumentReportSetupData(string formCode, string reportSearchText = "")
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;
            if (reportSearchText == null)
            {
                reportSearchText = "";
            }

            string sqlQuery = $@"
                    SELECT 
                        REPORT_EDESC,
                        REPORT_NO
                    FROM DOCUMENT_REPORT_SETUP
                    WHERE DELETED_FLAG = 'N'
                      AND COMPANY_CODE = '{companyCode}'
                      AND LINK_FORM_CODE = '{formCode}'
                      AND UPPER(REPORT_EDESC) LIKE '%{reportSearchText.ToUpper()}%'";

            var result = _dbContext
                            .SqlQuery<DocumentReportSetupModel>(sqlQuery)
                            .ToList();

            return result;
        }

        public bool UpdateMiscellaneousData(MiscellaneousTabDataModel model)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                        UPDATE FORM_SETUP SET
                            MULTI_UNIT_FLAG = '{model.MULTI_UNIT_FLAG}',
                            PRINT_REPORT_FLAG = '{model.PRINT_REPORT_FLAG}',
                            PUBLIC_FLAG = '{model.PUBLIC_FLAG}',
                            PRICE_CONTROL_FLAG = '{model.PRICE_CONTROL_FLAG}',
                            RATE_SCHEDULE_FIX_PRICE = '{model.RATE_SCHEDULE_FIX_PRICE}',
                            PRIMARY_MANUAL_FLAG = '{model.PRIMARY_MANUAL_FLAG}',
                            TOTAL_ROUND_FLAG = '{model.TOTAL_ROUND_FLAG}',
                            COPY_VALUES_FLAG = '{model.COPY_VALUES_FLAG}',
                            ACCESS_BDFSM_FLAG = '{model.ACCESS_BDFSM_FLAG}',
                            NEGATIVE_STOCK_FLAG = '{model.NEGATIVE_STOCK_FLAG}',
                            FREEQTY_FLAG = '{model.FREEQTY_FLAG}',
                            DC_VAT_FLAG = '{model.DC_VAT_FLAG}',
                            COMMITMENT_FLAG = '{model.COMMITMENT_FLAG}',
                            PURCHASE_MRR_GRNI_FLAG = '{model.PURCHASE_MRR_GRNI_FLAG}',
                            DISPLAY_RATE = '{model.DISPLAY_RATE}',
                            PENDING_INFO_FLAG = '{model.PENDING_INFO_FLAG}',
                            WM_FLAG = '{model.WM_FLAG}',
                            OTHER_INFO_FLAG = '{model.OTHER_INFO_FLAG}',
                            VNO_AS_DOC_ID_CONTROL = '{model.VNO_AS_DOC_ID_CONTROL}',
                            REF_FIX_QUANTITY = '{model.REF_FIX_QUANTITY}',
                            REF_FIX_PRICE = '{model.REF_FIX_PRICE}',
                            DISCOUNT_SCHEDULE_FLAG = '{model.DISCOUNT_SCHEDULE_FLAG}',
                            INFO_FLAG = '{model.INFO_FLAG}',
                            FREEZE_BACK_DAYS = {model.FREEZE_BACK_DAYS},
                            DECIMAL_PLACE = {model.DECIMAL_PLACE},
                            MAX_ROWS = {model.MAX_ROWS},
                            TOTAL_ROUND_INDEX = {model.TOTAL_ROUND_INDEX},
                            REPORT_NO = '{model.REPORT_NO}'
                        WHERE COMPANY_CODE = '{companyCode}'
                          AND FORM_CODE = '{model.FORM_CODE}'
                          AND DELETED_FLAG = 'N'
                    ";

            _dbContext.ExecuteSqlCommand(sqlQuery);

            return true;
        }




















        private string FormattingTab_GetTableNameList(string formCode)
        {
            var companyCode = _workContext.CurrentUserinformation.company_code;

            string sqlQuery = $@"
                                SELECT TABLE_NAME
                                  FROM FORM_DETAIL_SETUP
                                 WHERE DELETED_FLAG = 'N'
                                   AND COMPANY_CODE = '{companyCode}'
                                   AND FORM_CODE = '{formCode}'";

            return _dbContext.SqlQuery<string>(sqlQuery).FirstOrDefault();
        }

    }

}









