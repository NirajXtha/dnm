using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Repository
{
    public class PreDispatchInspectionRepo : IPreDispatchInspectionRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public PreDispatchInspectionRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<Items> GetInvoiceNoList()
        {
            try
            {
                List<Items> tableList = new List<Items>();
                string query = $@"select distinct sales_no as item_edesc,sales_no as item_code from sa_sales_invoice where DELETED_FLAG='N'";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PreDispatchInspection GetDispatchDetails(string InvoiceNo)
        {
            try
            {
                PreDispatchInspection tableList = new PreDispatchInspection();
                string query = $@"select distinct SSI.CUSTOMER_CODE,SCS.CUSTOMER_EDESC AS CUSTOMER_NAME,ST.DRIVER_NAME,ST.DRIVER_MOBILE_NO AS DRIVER_CONTACT_NO
                                ,ST.VEHICLE_NO
                                from sa_sales_invoice SSI 
                                LEFT join SHIPPING_TRANSACTION ST ON SSI.SALES_NO = ST.VOUCHER_NO
                                LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = SSI.CUSTOMER_CODE WHERE SSI.DELETED_FLAG= 'N' AND SSI.SALES_NO ='{InvoiceNo}'";
                tableList = this._dbContext.SqlQuery<PreDispatchInspection>(query).FirstOrDefault();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PreDispatchInspection> GetPreDispatchInspectionCheckList()
        {
            List<PreDispatchInspection> raw = new List<PreDispatchInspection>();

           
            String query_globalProducts = $@"select IPD.PARAMETER_ID,IP.PARAMETERS from INSPECTION_PARAM_DETAILS IPD
                                        INNER JOIN INSPECTION_PARAMETERS IP ON IPD.PARAMETER_ID = IP.INSPECTION_PARAM_NO ORDER BY IPD.PARAMETER_ID";
            //var raws = _dbContext.SqlQuery<PreDispatchInspection>(query_globalProducts).FirstOrDefault();
            var  raws = _dbContext.SqlQuery<PreDispatchInspection>(query_globalProducts).ToList();

            String query_globalWeight1 = $@"select ISDUST_VEHICLE,ISWATERSPILL_VEHICLE,ISCRACKSHOLES_VEHICLE,ISNAILS_VEHICLE
,ISLEAKWALL_VEHICLE,VEHICLE_DUST_REMARKS,VEHICLE_WATERSPILL_REMARKS,VEHICLE_CRACKSHOLES_REMARKS,VEHICLE_NAILS_REMARKS
,VEHICLE_WALL_REMARKS,ISVISUALDEFECT_PRODUCT,ISDIMENSIONS_PRODUCT,ISWEIGHTCHECK_PRODUCT,PRODUCT_DEFECT_REMARKS
,PRODUCT_DIMENSIONS_REMARKS,PRODUCT_WEIGHT_REMARKS,ISCORRECT_PACKAGING,ISSEALED_PACKAGING,ISPERBOX_PACKAGING
,ISSTACKING_PACKAGING,PACKAGING_CORRECT_REMARKS,PACKAGING_SEALED_REMARKS,PACKAGING_PERBOX_REMARKS,PACKAGING_STACKING_REMARKS
,ISINVOICE_DOCUMENTATION,ISQUALITY_DOCUMENTATION,ISCOMPLIANCE_DOCUMENTATION,DOCU_INVOICE_REMARKS,DOCU_QUALITY_REMARKS
,DOCU_COMP_REMARKS
from PRE_DISPATCH_INSPECTION";

            var raws1 = _dbContext.SqlQuery<PreDispatchInspectionDetails>(query_globalWeight1).FirstOrDefault();

            if (raws != null)
            {
                foreach (var pp in raws)
                {
                    PreDispatchInspection rawmodel = new PreDispatchInspection();
                    //PreDispatchInspection.PreDispatchInspectionDetailsListOutput st = new PreDispatchInspectionDetailsListOutput();
                    rawmodel.PARAMETER_ID = pp.PARAMETER_ID;
                    rawmodel.PARAMETERS = pp.PARAMETERS;
                    String query_globalWeight = $@"select PARAM_ITEM_NO,ITEM_NAME AS ITEM_EDESC,COLUMN_HEADER,'' Status, '' Remarks from PARAM_ITEM_MAP where PARAM_ITEM_NO='{pp.PARAMETER_ID}'";
                    rawmodel.PreDispatchInspectionDetailsList = this._dbContext.SqlQuery<PreDispatchInspectionDetails>(query_globalWeight).ToList();                  
                    raw.Add(rawmodel);
                }
            }

            //if (raws != null)
            //{
            //    foreach (var pp in raws)
            //    {
            //        PreDispatchInspection rawmodel = new PreDispatchInspection();
            //        rawmodel.PARAMETER_ID = pp.PARAMETER_ID;
            //        rawmodel.PARAMETERS = pp.PARAMETERS;
            //        String query_globalWeight = $@"select PARAM_ITEM_NO,ITEM_NAME AS ITEM_EDESC,COLUMN_HEADER,'' Status, '' Remarks from PARAM_ITEM_MAP where PARAM_ITEM_NO='{pp.PARAMETER_ID}'";
            //        rawmodel.PreDispatchInspectionDetailsList = this._dbContext.SqlQuery<PreDispatchInspectionDetails>(query_globalWeight).ToList();
            //        raw.Add(rawmodel);
            //    }
            //}
            return raw;
        }

        public List<PreDispatchInspection> GetPreDispatchInspectionByIdCheckList(string dispatch)
        {
            List<PreDispatchInspection> raw = new List<PreDispatchInspection>();


            String query_globalProducts = $@"select IPD.PARAMETER_ID,IP.PARAMETERS from INSPECTION_PARAM_DETAILS IPD
                                        INNER JOIN INSPECTION_PARAMETERS IP ON IPD.PARAMETER_ID = IP.INSPECTION_PARAM_NO ORDER BY IPD.PARAMETER_ID";
            var raws = _dbContext.SqlQuery<PreDispatchInspection>(query_globalProducts).ToList();

            if (raws != null)
            {
                foreach (var pp in raws)
                {
                    PreDispatchInspection rawmodel = new PreDispatchInspection();
                    rawmodel.PARAMETER_ID = pp.PARAMETER_ID;
                    rawmodel.PARAMETERS = pp.PARAMETERS;
                    String query_globalWeight = $@"select PARAM_ITEM_NO,ITEM_NAME AS ITEM_EDESC,COLUMN_HEADER,'' Status, '' Remarks from PARAM_ITEM_MAP where PARAM_ITEM_NO='{pp.PARAMETER_ID}'";
                    rawmodel.PreDispatchInspectionDetailsList = this._dbContext.SqlQuery<PreDispatchInspectionDetails>(query_globalWeight).ToList();
                    raw.Add(rawmodel);
                }
            }
            return raw;
        }
        public bool InsertPreDispatchInspectionData(PreDispatchInspection data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.DISPATCH_NO}'";
                    string dispatch_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'PD'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();


                    if (dispatch_no == null)
                    {
                        string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                        string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();
                        string insertRawMaterialTranQuery = string.Format(@"INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO
                                    ,REFERENCE_NO,ITEM_CODE,SERIAL_NO,QC_CODE
                                    ,CUSTOMER_NAME,CUSTOMER_INVOICE_NO,TRANSPORT_DETAIL,DRIVER_NAME,DRIVER_CONTACT_NO,VEHICLE_NO,DISPATCH_PERSON,QC_INSPECTOR
                                    ,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                                    ,CREATED_DATE,DELETED_FLAG,REMARKS)
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}','{14}'
                                    ,'{15}','{16}',TO_DATE('{17}', 'DD-MON-YYYY'),'{18}','{19}')"
                                      , data.DISPATCH_NO, "0", "0", serial_no_qc_setup, serial_no_qc_setup
                                      , data.CUSTOMER_NAME, data.CUSTOMER_INVOICE_NO, data.TRANSPORT_DETAIL, data.DRIVER_NAME, data.DRIVER_CONTACT_NO, data.VEHICLE_NO, data.DISPATCH_PERSON, data.QC_INSPECTOR
                                      , form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                                      _workContext.CurrentUserinformation.login_code
                                      , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', data.REMARKS);
                        _dbContext.ExecuteSqlCommand(insertRawMaterialTranQuery);
                        foreach (var raw in data.PreDispatchInspectionDetailsList)
                        {
                                  string insertQuery = string.Format(@"
                            INSERT INTO PRE_DISPATCH_INSPECTION(PRE_DISPATCH_INSPECTION_NO
,ISDUST_VEHICLE,ISWATERSPILL_VEHICLE,ISCRACKSHOLES_VEHICLE,ISNAILS_VEHICLE,ISLEAKWALL_VEHICLE
,VEHICLE_DUST_REMARKS,VEHICLE_WATERSPILL_REMARKS,VEHICLE_CRACKSHOLES_REMARKS,VEHICLE_NAILS_REMARKS
,VEHICLE_WALL_REMARKS,ISVISUALDEFECT_PRODUCT,ISDIMENSIONS_PRODUCT,ISWEIGHTCHECK_PRODUCT,PRODUCT_DEFECT_REMARKS
,PRODUCT_DIMENSIONS_REMARKS,PRODUCT_WEIGHT_REMARKS
,ISCORRECT_PACKAGING,ISSEALED_PACKAGING,ISPERBOX_PACKAGING
,ISSTACKING_PACKAGING,PACKAGING_CORRECT_REMARKS,PACKAGING_SEALED_REMARKS,PACKAGING_PERBOX_REMARKS,PACKAGING_STACKING_REMARKS
,ISINVOICE_DOCUMENTATION,ISQUALITY_DOCUMENTATION,ISCOMPLIANCE_DOCUMENTATION,DOCU_INVOICE_REMARKS,DOCU_QUALITY_REMARKS
,DOCU_COMP_REMARKS,CREATED_BY, CREATED_DATE, DELETED_FLAG, COMPANY_CODE, BRANCH_CODE, FORM_CODE, SERIAL_NO)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}'
,'{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}'
, TO_DATE('{32}', 'DD-MON-YYYY'), '{33}', '{34}', '{35}','{36}','{37}')"
                                        , data.DISPATCH_NO, raw.ISDUST_VEHICLE, raw.ISWATERSPILL_VEHICLE, raw.ISCRACKSHOLES_VEHICLE, raw.ISNAILS_VEHICLE,
                                        raw.ISLEAKWALL_VEHICLE, raw.VEHICLE_DUST_REMARKS, raw.VEHICLE_WATERSPILL_REMARKS, raw.VEHICLE_CRACKSHOLES_REMARKS, raw.VEHICLE_NAILS_REMARKS
                                        , raw.VEHICLE_WALL_REMARKS, raw.ISVISUALDEFECT_PRODUCT, raw.ISDIMENSIONS_PRODUCT, raw.ISWEIGHTCHECK_PRODUCT
                                        ,raw.PRODUCT_DEFECT_REMARKS,raw.PRODUCT_DIMENSIONS_REMARKS,raw.PRODUCT_WEIGHT_REMARKS
                                        ,raw.ISCORRECT_PACKAGING,raw.ISSEALED_PACKAGING,raw.ISPERBOX_PACKAGING                                
                                        ,raw.ISSTACKING_PACKAGING,raw.PACKAGING_CORRECT_REMARKS,raw.PACKAGING_SEALED_REMARKS
                                        ,raw.PACKAGING_PERBOX_REMARKS,raw.PACKAGING_STACKING_REMARKS,raw.ISINVOICE_DOCUMENTATION
                                        ,raw.ISQUALITY_DOCUMENTATION,raw.ISCOMPLIANCE_DOCUMENTATION,raw.DOCU_INVOICE_REMARKS
                                        ,raw.DOCU_QUALITY_REMARKS,raw.DOCU_COMP_REMARKS,
                                        _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, serial_no_qc_setup);  // change hard code 498

                            _dbContext.ExecuteSqlCommand(insertQuery);
                        }
                      

                    }
                    else
                    {
                        foreach (var raw in data.PreDispatchInspectionDetailsList)
                        {
                            string updatePreDispatchQuery = $@"UPDATE PRE_DISPATCH_INSPECTION 
                       SET 
                           ISDUST_VEHICLE = '{raw.ISDUST_VEHICLE}',ISWATERSPILL_VEHICLE ='{raw.ISWATERSPILL_VEHICLE}' 
                            ,ISCRACKSHOLES_VEHICLE ='{raw.ISCRACKSHOLES_VEHICLE}',ISNAILS_VEHICLE ='{raw.ISNAILS_VEHICLE}'
                            ,ISLEAKWALL_VEHICLE ='{raw.ISLEAKWALL_VEHICLE}',VEHICLE_DUST_REMARKS ='{raw.VEHICLE_DUST_REMARKS}'
                            ,VEHICLE_WATERSPILL_REMARKS ='{raw.VEHICLE_WATERSPILL_REMARKS}',VEHICLE_CRACKSHOLES_REMARKS ='{raw.VEHICLE_CRACKSHOLES_REMARKS}'
                            ,VEHICLE_NAILS_REMARKS ='{raw.VEHICLE_NAILS_REMARKS}',VEHICLE_WALL_REMARKS ='{raw.VEHICLE_WALL_REMARKS}'
                            ,ISVISUALDEFECT_PRODUCT ='{raw.ISVISUALDEFECT_PRODUCT}',ISDIMENSIONS_PRODUCT ='{raw.ISDIMENSIONS_PRODUCT}'
                            ,ISWEIGHTCHECK_PRODUCT ='{raw.ISWEIGHTCHECK_PRODUCT}',PRODUCT_DEFECT_REMARKS ='{raw.PRODUCT_DEFECT_REMARKS}'
                            ,PRODUCT_DIMENSIONS_REMARKS ='{raw.PRODUCT_DIMENSIONS_REMARKS}',PRODUCT_WEIGHT_REMARKS ='{raw.PRODUCT_WEIGHT_REMARKS}'
                            ,ISCORRECT_PACKAGING ='{raw.ISCORRECT_PACKAGING}',ISSEALED_PACKAGING ='{raw.ISSEALED_PACKAGING}'
                            ,ISPERBOX_PACKAGING ='{raw.ISPERBOX_PACKAGING}',ISSTACKING_PACKAGING ='{raw.ISSTACKING_PACKAGING}'
                            ,PACKAGING_CORRECT_REMARKS ='{raw.PACKAGING_CORRECT_REMARKS}',PACKAGING_SEALED_REMARKS ='{raw.PACKAGING_SEALED_REMARKS}'
                            ,PACKAGING_PERBOX_REMARKS ='{raw.PACKAGING_PERBOX_REMARKS}',PACKAGING_STACKING_REMARKS ='{raw.PACKAGING_STACKING_REMARKS}'
                            ,ISINVOICE_DOCUMENTATION ='{raw.ISINVOICE_DOCUMENTATION}',ISQUALITY_DOCUMENTATION ='{raw.ISQUALITY_DOCUMENTATION}'
                            ,ISCOMPLIANCE_DOCUMENTATION ='{raw.ISCOMPLIANCE_DOCUMENTATION}',DOCU_INVOICE_REMARKS ='{raw.PACKAGING_SEALED_REMARKS}'
                            ,DOCU_COMP_REMARKS ='{raw.DOCU_COMP_REMARKS}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE PRE_DISPATCH_INSPECTION_NO = '{data.DISPATCH_NO}' ";
                            _dbContext.ExecuteSqlCommand(updatePreDispatchQuery);
                        }
                        
                       

                        string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           CUSTOMER_INVOICE_NO = '{data.CUSTOMER_INVOICE_NO}',TRANSPORT_DETAIL ='{data.TRANSPORT_DETAIL}', 
                           DRIVER_NAME = '{data.DRIVER_NAME}', CUSTOMER_NAME = '{data.CUSTOMER_NAME}',DRIVER_CONTACT_NO = '{data.DRIVER_CONTACT_NO}',
                           VEHICLE_NO = '{data.VEHICLE_NO}', DISPATCH_PERSON = '{data.DISPATCH_PERSON}',QC_INSPECTOR = '{data.QC_INSPECTOR}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE TRANSACTION_NO = '{data.DISPATCH_NO}' ";
                        _dbContext.ExecuteSqlCommand(updateQuery);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }





        public List<FormDetailSetup> GetPreDispatchInspectionList()
        {
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'PD'";
            string formCode = this._dbContext.SqlQuery<string>(query_FormCode).FirstOrDefault();
            string Query = $@"SELECT FDS.SERIAL_NO,
                            FS.FORM_EDESC,
                            FS.FORM_TYPE,
                            FS.NEGATIVE_STOCK_FLAG,
                           FDS.FORM_CODE,
                           FDS.TABLE_NAME,
                           FDS.COLUMN_NAME,
                           FDS.COLUMN_WIDTH,
                           FDS.COLUMN_HEADER,
                           FDS.TOP_POSITION,
                           FDS.LEFT_POSITION,
                           FDS.DISPLAY_FLAG,
                           FDS.DEFA_VALUE,
                           FDS.IS_DESC_FLAG,
                           FDS.MASTER_CHILD_FLAG,
                           FDS.FORM_CODE,
                           FDS.COMPANY_CODE,
                           CS.COMPANY_EDESC,
                            CS.TELEPHONE,
                            CS.EMAIL,
                            CS.TPIN_VAT_NO,
                            CS.ADDRESS,
                           FDS.CREATED_BY,
                           FDS.CREATED_DATE,
                           FDS.DELETED_FLAG,
                           FDS.FILTER_VALUE,
                           FDS.SYN_ROWID,
                           FDS.MODIFY_DATE,
                           FDS.MODIFY_BY,
                           FS.REFERENCE_FLAG,
                           FS.FREEZE_MASTER_REF_FLAG,
                           FS.REF_FIX_QUANTITY,
                           FS.REF_FIX_PRICE,
                           FS.DISPLAY_RATE,
                           FS.RATE_SCHEDULE_FIX_PRICE,
                           FS.PRICE_CONTROL_FLAG
                      FROM    FORM_DETAIL_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE  FDS.MASTER_CHILD_FLAG = 'C' AND FDS.DISPLAY_FLAG='Y' AND FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' order by FDS.SERIAL_NO";
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }

        public List<PACKINGUNIT> GetPackingUnit()
        {
            try
            {
                List<PACKINGUNIT> tableList = new List<PACKINGUNIT>();
                string query = $@"SELECT MU_CODE,MU_EDESC from IP_MU_CODE WHERE DELETED_FLAG='N'
                                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                tableList = this._dbContext.SqlQuery<PACKINGUNIT>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public PreDispatchInspection GetEditPreDispatchInspection(string transactionno)
        {
            PreDispatchInspection raw = new PreDispatchInspection();
            String query1 = $@"select TRANSACTION_NO as DISPATCH_NO,CUSTOMER_NAME,CUSTOMER_INVOICE_NO,TRANSPORT_DETAIL
,DRIVER_NAME,DRIVER_CONTACT_NO,VEHICLE_NO,DISPATCH_PERSON,QC_INSPECTOR,SERIAL_NO,CREATED_DATE,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<PreDispatchInspection>(query1).FirstOrDefault();
           // String query2 = $@"select ITEM_CODE,SUPPLIER_CODE,BATCH_NO,GSM,WIDTH,STRENGTH,THICKNESS,ACTUAL_GSM,ACTUAL_WIDTH,ACTUAL_STRENGTH,ACTUAL_THICKNESS,REMARKS from IP_ITEM_QC_TESTING  WHERE SERIAL_NO= '{items[0].SERIAL_NO}'";
            String query2 = $@" select ISDUST_VEHICLE,ISWATERSPILL_VEHICLE
,ISCRACKSHOLES_VEHICLE,ISNAILS_VEHICLE,ISLEAKWALL_VEHICLE,VEHICLE_DUST_REMARKS,VEHICLE_WATERSPILL_REMARKS
,VEHICLE_CRACKSHOLES_REMARKS,VEHICLE_NAILS_REMARKS,VEHICLE_WALL_REMARKS,ISVISUALDEFECT_PRODUCT,ISDIMENSIONS_PRODUCT
,ISWEIGHTCHECK_PRODUCT,PRODUCT_DEFECT_REMARKS,PRODUCT_DIMENSIONS_REMARKS,PRODUCT_WEIGHT_REMARKS,ISCORRECT_PACKAGING
,ISSEALED_PACKAGING,ISPERBOX_PACKAGING,ISSTACKING_PACKAGING,PACKAGING_CORRECT_REMARKS,PACKAGING_SEALED_REMARKS
,PACKAGING_PERBOX_REMARKS,PACKAGING_STACKING_REMARKS,ISINVOICE_DOCUMENTATION,ISQUALITY_DOCUMENTATION
,ISCOMPLIANCE_DOCUMENTATION,DOCU_INVOICE_REMARKS,DOCU_QUALITY_REMARKS,DOCU_COMP_REMARKS
 from PRE_DISPATCH_INSPECTION  
 WHERE PRE_DISPATCH_INSPECTION_NO= '{transactionno}'";
            //List<RawMaterial> items2 = _dbContext.SqlQuery<RawMaterial>(query2).ToList();

            List<PreDispatchInspectionDetails> rawMaterials = new List<PreDispatchInspectionDetails>();
            rawMaterials = this._dbContext.SqlQuery<PreDispatchInspectionDetails>(query2).ToList();


            raw.PreDispatchInspectionDetailsList = rawMaterials;
            //return Record;

            // If no valid voucherno, return the view with a default state (qc is null)
            return raw;
        }

        public PreDispatchInspection GetPreDispatchInspectionReport(string transactionno)
        {
            PreDispatchInspection raw = new PreDispatchInspection();
            String query1 = $@"select TRANSACTION_NO as DISPATCH_NO,CUSTOMER_NAME,CUSTOMER_INVOICE_NO,TRANSPORT_DETAIL
,DRIVER_NAME,DRIVER_CONTACT_NO,VEHICLE_NO,DISPATCH_PERSON,QC_INSPECTOR,SERIAL_NO,CREATED_DATE,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<PreDispatchInspection>(query1).FirstOrDefault();
            String query2 = $@" select CASE WHEN ISDUST_VEHICLE = 'N' THEN 'No' WHEN ISDUST_VEHICLE = 'Y' THEN 'Yes' ELSE '' END ISDUST_VEHICLE
,CASE WHEN ISWATERSPILL_VEHICLE = 'N' THEN 'No' WHEN ISWATERSPILL_VEHICLE = 'Y' THEN 'Yes' ELSE '' END ISWATERSPILL_VEHICLE 
,CASE WHEN ISCRACKSHOLES_VEHICLE = 'N' THEN 'No' WHEN ISCRACKSHOLES_VEHICLE = 'Y' THEN 'Yes' ELSE '' END ISCRACKSHOLES_VEHICLE
,CASE WHEN ISNAILS_VEHICLE = 'N' THEN 'No' WHEN ISNAILS_VEHICLE = 'Y' THEN 'Yes' ELSE '' END ISNAILS_VEHICLE 
,CASE WHEN ISLEAKWALL_VEHICLE = 'N' THEN 'No' WHEN ISLEAKWALL_VEHICLE = 'Y' THEN 'Yes' ELSE '' END ISLEAKWALL_VEHICLE 
,VEHICLE_DUST_REMARKS,VEHICLE_WATERSPILL_REMARKS
,VEHICLE_CRACKSHOLES_REMARKS,VEHICLE_NAILS_REMARKS,VEHICLE_WALL_REMARKS
,CASE WHEN ISVISUALDEFECT_PRODUCT = 'N' THEN 'No' WHEN ISVISUALDEFECT_PRODUCT = 'Y' THEN 'Yes' ELSE '' END ISVISUALDEFECT_PRODUCT 
,CASE WHEN ISDIMENSIONS_PRODUCT = 'N' THEN 'No' WHEN ISDIMENSIONS_PRODUCT = 'Y' THEN 'Yes' ELSE '' END ISDIMENSIONS_PRODUCT  
,CASE WHEN ISWEIGHTCHECK_PRODUCT = 'N' THEN 'No' WHEN ISWEIGHTCHECK_PRODUCT = 'Y' THEN 'Yes' ELSE '' END ISWEIGHTCHECK_PRODUCT 
,PRODUCT_DEFECT_REMARKS,PRODUCT_DIMENSIONS_REMARKS,PRODUCT_WEIGHT_REMARKS
,CASE WHEN ISCORRECT_PACKAGING = 'N' THEN 'No' WHEN ISCORRECT_PACKAGING = 'Y' THEN 'Yes' ELSE '' END ISCORRECT_PACKAGING 
,CASE WHEN ISSEALED_PACKAGING = 'N' THEN 'No' WHEN ISSEALED_PACKAGING = 'Y' THEN 'Yes' ELSE '' END ISSEALED_PACKAGING 
,CASE WHEN ISPERBOX_PACKAGING = 'N' THEN 'No' WHEN ISPERBOX_PACKAGING = 'Y' THEN 'Yes' ELSE '' END ISPERBOX_PACKAGING  
,CASE WHEN ISSTACKING_PACKAGING = 'N' THEN 'No' WHEN ISSTACKING_PACKAGING = 'Y' THEN 'Yes' ELSE '' END ISSTACKING_PACKAGING  
,PACKAGING_CORRECT_REMARKS,PACKAGING_SEALED_REMARKS
,PACKAGING_PERBOX_REMARKS,PACKAGING_STACKING_REMARKS
,CASE WHEN ISINVOICE_DOCUMENTATION = 'N' THEN 'No' WHEN ISINVOICE_DOCUMENTATION = 'Y' THEN 'Yes' ELSE '' END ISINVOICE_DOCUMENTATION 
,CASE WHEN ISQUALITY_DOCUMENTATION = 'N' THEN 'No' WHEN ISQUALITY_DOCUMENTATION = 'Y' THEN 'Yes' ELSE '' END ISQUALITY_DOCUMENTATION 
,CASE WHEN ISCOMPLIANCE_DOCUMENTATION = 'N' THEN 'No' WHEN ISCOMPLIANCE_DOCUMENTATION = 'Y' THEN 'Yes' ELSE '' END ISCOMPLIANCE_DOCUMENTATION  
,DOCU_INVOICE_REMARKS,DOCU_QUALITY_REMARKS,DOCU_COMP_REMARKS
 from PRE_DISPATCH_INSPECTION  
 WHERE PRE_DISPATCH_INSPECTION_NO= '{transactionno}'";
            List<PreDispatchInspectionDetails> rawMaterials = new List<PreDispatchInspectionDetails>();
            rawMaterials = this._dbContext.SqlQuery<PreDispatchInspectionDetails>(query2).ToList();
            raw.PreDispatchInspectionDetailsList = rawMaterials;
            return raw;
        }
    }
}
