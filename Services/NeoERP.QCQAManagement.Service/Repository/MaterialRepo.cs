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
   public class MaterialRepo : IMaterialRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public MaterialRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }

        public List<Items> GetMaterialLists()
        {
            try
            {
                List<Items> tableList = new List<Items>();
                string query = $@"SELECT master_item_code, INITCAP(item_edesc) as item_edesc, pre_item_code,item_code FROM IP_PRODUCT_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' 
                                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND group_sku_flag ='I'
                                ORDER BY LENGTH(PRE_ITEM_CODE),PRE_ITEM_CODE, MASTER_ITEM_CODE";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Items> GetSupplierLists()
        {
            try
            {
                List<Items> productList = new List<Items>();
                string query = $@"select SUPPLIER_CODE,SUPPLIER_EDESC from ip_supplier_setup WHERE COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                productList = this._dbContext.SqlQuery<Items>(query).ToList();
                return productList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Items> GetMaterialListsByCategory(string CategoryCode)
        {
            try
            {
                List<Items> tableList = new List<Items>();
                string query = $@"SELECT master_item_code, INITCAP(item_edesc) as item_edesc, pre_item_code,item_code FROM IP_PRODUCT_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' 
                                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND group_sku_flag ='I' and category_CODE ='{CategoryCode}'
                                ORDER BY LENGTH(PRE_ITEM_CODE),PRE_ITEM_CODE, MASTER_ITEM_CODE";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string GetQCQAVoucherNo(string templateName)
        {
            string query_reference_no = "";
            string reference_no = "";
            string query_voucherNo = "";
            string voucherNo = "";
            string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'IN'";
            string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();
            if (templateName == "InComingMaterial")
            {

                //query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))), 0) + 1,2,'0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION where form_code ='{form_code}'";
                string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE form_code ='{form_code}'";
                int finalbodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
                query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{finalbodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{form_code}'";
                
                
                reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();

                query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'IN'";
                voucherNo = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            }
            else if(templateName == "InComingMaterialSample")
            {
                string query_formCode = $@"SELECT form_code   FROM form_setup WHERE FORM_TYPE = 'IS'";
                String formCode_ = this._dbContext.SqlQuery<string>(query_formCode).FirstOrDefault();

                string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE form_code ='{formCode_}'";
                int finalbodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
                query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{finalbodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{formCode_}'";


                reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();

                query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'IS'";
                voucherNo = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            }
            else
            {
                //query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTON_NO, '[^/]+', 1, 2))), 0) + 1,2,'0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION where form_code ='{form_code}'";
                string bodylength = $@"SELECT BODY_LENGTH FROM form_setup WHERE form_code ='{form_code}'";
                int finalbodyLength = this._dbContext.SqlQuery<int>(bodylength).FirstOrDefault();
                query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REGEXP_SUBSTR(TRANSACTION_NO, '[^/]+', 1, 2))),0) + 1,'{finalbodyLength}','0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION WHERE form_code = '{form_code}'";
                reference_no = this._dbContext.SqlQuery<string>(query_reference_no).FirstOrDefault();
                //query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'Q' and form_code ='{form_code}'";
                query_voucherNo = $@"SELECT CONCAT(CONCAT(CUSTOM_PREFIX_TEXT, '{reference_no}'), CUSTOM_SUFFIX_TEXT) AS DSF   FROM form_setup WHERE FORM_TYPE = 'IN' and form_code ='{form_code}'";
                voucherNo = this._dbContext.SqlQuery<string>(query_voucherNo).FirstOrDefault();
            }
            return voucherNo;
        }


        public List<Items> GetVoucherDetailsByItemId(string ItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string query = $@"
SELECT
    IPM.mrr_no AS Voucher_No,
    IPM.item_code AS item_code,
    IPM.MRR_DATE AS QC_DATE,
    TO_CHAR(BS_DATE(IPM.MRR_DATE)) AS MITI,
    IPM.SUPPLIER_INV_NO AS INVOICE_NO,
    CASE
        WHEN IGE.INWARD_TYPE = 'Referencial Outward' THEN
            (SELECT CUSTOMER_EDESC
             FROM SA_CUSTOMER_SETUP
             WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE
               AND COMPANY_CODE = IGE.COMPANY_CODE)
        ELSE
            (SELECT SUPPLIER_EDESC
             FROM IP_SUPPLIER_SETUP
             WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE
               AND COMPANY_CODE = IPM.COMPANY_CODE)
    END AS SUPPLIER_EDESC,
    
    iiss.THICKNESS as thickness,
    iiss.ROLLDIAMETER,
    iiss.PH,
    iiss.UNPLEASANT_SMELL_ODOUR,
    iiss.DUST_DIRT,
    iiss.DAMAGING_MATERIAL,
    iiss.CORE_DAMAGING,
    iiss.SIZE_WIDTH,
    iiss.GSM,
    iiss.TENSILE_CD,
    iiss.TENSILE_MD,
    iiss.VISUAL_INSPECTION,
    IPM.REMARKS

FROM ip_purchase_mrr IPM
LEFT JOIN IP_GATE_ENTRY_DETAIL IGED ON IGED.item_code = IPM.item_code
LEFT JOIN IP_GATE_ENTRY IGE ON IGE.GATE_NO = IGED.GATE_NO
INNER JOIN IP_ITEM_SPEC_SETUP iiss ON iiss.item_code = IPM.item_code
WHERE IPM.mrr_no NOT IN(SELECT reference_form_code FROM reference_detail)
AND IPM.mrr_no NOT IN (SELECT REFERENCE_NO FROM QC_PARAMETER_TRANSACTION)  
AND IPM.item_code = {ItemCode}

UNION ALL

SELECT
    IGED.GATE_NO AS Voucher_No,
    IGED.item_code AS item_code,
    IGED.GATE_DATE AS QC_DATE,
    TO_CHAR(BS_DATE(IGED.GATE_DATE)) AS MITI,
    IGE.BILL_NO AS INVOICE_NO,
    CASE
        WHEN IGE.INWARD_TYPE = 'Referencial Outward' THEN
            (SELECT CUSTOMER_EDESC
             FROM SA_CUSTOMER_SETUP
             WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE
               AND COMPANY_CODE = IGE.COMPANY_CODE)
        ELSE
            (SELECT SUPPLIER_EDESC
             FROM IP_SUPPLIER_SETUP
             WHERE SUPPLIER_CODE = IGE.SUPPLIER_CODE
               AND COMPANY_CODE = IGE.COMPANY_CODE)
    END AS SUPPLIER_EDESC,

    NULL AS thickness ,
    NULL AS ROLLDIAMETER,
    NULL AS PH,
    NULL AS UNPLEASANT_SMELL_ODOUR,
    NULL AS DUST_DIRT,
    NULL AS DAMAGING_MATERIAL,
    NULL AS CORE_DAMAGING,
    NULL AS SIZE_WIDTH,
    NULL AS GSM,
    NULL AS TENSILE_CD,
    NULL AS TENSILE_MD,
    NULL AS VISUAL_INSPECTION,
    IGED.REMARKS

FROM IP_GATE_ENTRY_DETAIL IGED
LEFT JOIN IP_GATE_ENTRY IGE ON IGE.GATE_NO = IGED.GATE_NO
WHERE IGED.item_code = {ItemCode}
AND IGED.GATE_NO NOT IN (SELECT REFERENCE_NO FROM QC_PARAMETER_TRANSACTION)
  AND IGED.item_code NOT IN(SELECT item_code FROM ip_purchase_mrr WHERE item_code = {ItemCode})";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Items> GetPending_VoucherDetailsByItemId(string ItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                //string query = $@"select mrr_no as Voucher_No from ip_purchase_mrr where mrr_no  not in ( select reference_form_code from reference_detail) and item_code ={ItemCode}";
                string query = $@"select DISTINCT IPM.mrr_no as Voucher_No,IPM.item_code
,IPM.MRR_DATE AS QC_DATE,BS_DATE(IPM.MRR_DATE) MITI
,IPM.SUPPLIER_INV_NO  AS INVOICE_NO
, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_EDESC 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE 
AND COMPANY_CODE = IPM.COMPANY_CODE ) END  SUPPLIER_EDESC, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_CODE 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
ELSE  (SELECT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE 
AND COMPANY_CODE = IPM.COMPANY_CODE ) END  SUPPLIER_CODE
from ip_purchase_mrr IPM
LEFT JOIN IP_GATE_ENTRY_DETAIL IGED ON IGED.item_code = IPM.item_code
LEFT JOIN IP_GATE_ENTRY  IGE ON IGE.GATE_NO =  IGED.GATE_NO
where 
IPM.mrr_no  not in ( select reference_form_code from reference_detail) 
and IPM.mrr_no not in ( select REFERENCE_NO from QC_PARAMETER_TRANSACTION)
and 
IPM.item_code ={ItemCode}
UNION ALL
select DISTINCT IGED.GATE_NO as Voucher_No,IGED.item_code
,IGED.GATE_DATE AS QC_DATE,BS_DATE(IGED.GATE_DATE) MITI
,IGE.BILL_NO  AS INVOICE_NO
, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_EDESC 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IGE.SUPPLIER_CODE 
AND COMPANY_CODE = IGE.COMPANY_CODE ) END  SUPPLIER_EDESC, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_CODE 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
ELSE  (SELECT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IGE.SUPPLIER_CODE 
AND COMPANY_CODE = IGE.COMPANY_CODE ) END  SUPPLIER_CODE
from IP_GATE_ENTRY_DETAIL IGED
LEFT JOIN IP_GATE_ENTRY  IGE ON IGE.GATE_NO =  IGED.GATE_NO
where  
IGED.item_code ={ItemCode} 
and 
IGED.GATE_NO not in ( select REFERENCE_NO from QC_PARAMETER_TRANSACTION) AND
IGED.item_code not in (select item_code from ip_purchase_mrr where item_code  ={ItemCode})";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Items> GetMaterialDetailLists(string VoucherNo)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string qc_no = GetQCQAVoucherNo("InComingMaterial");
                string query = $@"select distinct '{qc_no}' as QC_NO,iss.SUPPLIER_CODE,iss.supplier_edesc,iims.item_edesc,ipm.mrr_date AS RECEIPT_DATE
,ipm.quantity,ipm.to_location_code,iiss.Thickness as thickness
,iiss.ROLLDIAMETER,iiss.PH,iiss.UNPLEASANT_SMELL_ODOUR,iiss.DUST_DIRT,iiss.DAMAGING_MATERIAL,
iiss.CORE_DAMAGING,iiss.SIZE_WIDTH,iiss.GSM,iiss.TENSILE_CD,iiss.TENSILE_MD,iiss.VISUAL_INSPECTION
,ipm.item_code,ipm.Created_By as CreatedBy,ipm.SUPPLIER_INV_NO as invoice_no,ipm.MANUAL_NO,ipm.Remarks,st.VEHICLE_NO,iss.REGD_OFFICE_EADDRESS as ADDRESS
,BT.MFG_DATE,BT.EXPIRY_DATE
from ip_purchase_mrr ipm
left join ip_supplier_setup iss on iss.supplier_code = ipm.supplier_code
left join IP_PRODUCT_ITEM_MASTER_SETUP iims on iims.item_code = ipm.item_code
left join IP_ITEM_SPEC_SETUP iiss ON  iiss.item_code = ipm.item_code
left join SHIPPING_TRANSACTION st ON st.VOUCHER_NO = ipm.mrr_no
LEFT join BATCH_TRANSACTION BT ON BT.item_code = ipm.item_code
where ipm.mrr_no = '{VoucherNo}'
UNION ALL
select distinct '{qc_no}' as QC_NO,iss.SUPPLIER_CODE,iss.supplier_edesc,iims.item_edesc,ige.gate_date AS RECEIPT_DATE
,ige.NET_WT as quantity,ige.LOCATION_CODE as to_location_code,iiss.Thickness as thickness
,iiss.ROLLDIAMETER,iiss.PH,iiss.UNPLEASANT_SMELL_ODOUR,iiss.DUST_DIRT,iiss.DAMAGING_MATERIAL,
iiss.CORE_DAMAGING,iiss.SIZE_WIDTH,iiss.GSM,iiss.TENSILE_CD,iiss.TENSILE_MD,iiss.VISUAL_INSPECTION
,iged.item_code,iged.Created_By as CreatedBy,ige.BILL_NO as invoice_no,ige.GATE_NO as MANUAL_NO,iged.Remarks,st.VEHICLE_NO,iss.REGD_OFFICE_EADDRESS as ADDRESS
,BT.MFG_DATE,BT.EXPIRY_DATE
from IP_GATE_ENTRY ige
left join IP_GATE_ENTRY_DETAIL iged on iged.GATE_NO= ige.GATE_NO
left join ip_supplier_setup iss on iss.supplier_code = ige.supplier_code
left join IP_PRODUCT_ITEM_MASTER_SETUP iims on iims.item_code = iged.item_code
inner join IP_ITEM_SPEC_SETUP iiss ON  iiss.item_code = iged.item_code
left join SHIPPING_TRANSACTION st ON st.VOUCHER_NO= ige.GATE_NO
LEFT join BATCH_TRANSACTION BT ON BT.item_code =  iims.item_code
where ige.GATE_NO = '{VoucherNo}'";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertQCParameterData(QCDetail data)
        {
            try
            {
                string query_serial_no = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                string serial_no = this._dbContext.SqlQuery<string>(query_serial_no).FirstOrDefault();

                string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM IP_ITEM_QC_SETUP WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();

                string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REFERENCE_NO)), 0)  + 1, 2, '0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION";
                string qc_no = GetQCQAVoucherNo("InComingMaterial");

                string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.TRANSACTION_NO}'";
                string incoming_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'IN'";
                string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();
                var specifications = new List<Specification>
                                    {
                                        new Specification("Thickness",data.Thickness0, data.Thickness1, data.Thickness2, data.Thickness3,data.Remarks1),
                                        new Specification("RollDiameter",data.RollDiameter0, data.RollDiameter1, data.RollDiameter2, data.RollDiameter3,data.Remarks2),
                                        new Specification("PH",data.PH0, data.PH1, data.PH2, data.PH3,data.Remarks3),
                                        new Specification("UnpleasantSmell",data.UnpleasantSmell0, data.UnpleasantSmell1, data.UnpleasantSmell2, data.UnpleasantSmell3,data.Remarks4),
                                        new Specification("DustDirt",data.DustDirt0, data.DustDirt1, data.DustDirt2, data.DustDirt3,data.Remarks5),
                                        new Specification("DamagingMaterial",data.DamagingMaterial0, data.DamagingMaterial1, data.DamagingMaterial2, data.DamagingMaterial3,data.Remarks6),
                                        new Specification("CoreDamaging",data.CoreDamaging0, data.CoreDamaging1, data.CoreDamaging2, data.CoreDamaging3,data.Remarks7),
                                        new Specification("Width",data.Width0, data.Width1, data.Width2, data.Width3,data.Remarks8),
                                        new Specification("GSM",data.GSM0, data.GSM1, data.GSM2, data.GSM3,data.Remarks9),
                                        new Specification("TensileCD",data.TensileCD0, data.TensileCD1, data.TensileCD2, data.TensileCD3,data.Remarks10),
                                        new Specification("TensileMD",data.TensileMD0, data.TensileMD1, data.TensileMD2, data.TensileMD3,data.Remarks11),
                                        new Specification("VisualInspection",data.VisualInspection0, data.VisualInspection1, data.VisualInspection2, data.VisualInspection3,data.Remarks12)
                                    };
                int i = 1;
                foreach (var spec in specifications)
                {
                    string query_qc_code = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(QC_CODE)), 0) + 1) FROM ip_item_qc_setup WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(QC_CODE, '^\d+$')";
                    string qc_code = this._dbContext.SqlQuery<string>(query_qc_code).FirstOrDefault();
                    if (incoming_no == null) {
                        string insertQuery = string.Format(@"
                    INSERT INTO ip_item_qc_setup (IP_ITEM_QC_SETUP_NO,ITEM_CODE, QC_CODE, QC_EDESC, QC_NDESC,SPECIFICATION, ROLL1, ROLL2, ROLL3, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG,SERIAL_NO,STANDARD_VALUE,HIGH_VALUE,MIN_VALUE,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}', TO_DATE('{11}', 'DD-MON-YYYY'),'{12}','{13}','{14}','{15}','{16}','{17}')",
                                    qc_no, data.ITEM_CODE, qc_code, spec.Name, spec.Name, spec.Value0 ?? "0",
                            spec.Value1 ?? "0", spec.Value2 ?? "0", spec.Value3 ?? "0",
                            _workContext.CurrentUserinformation.company_code,
                            _workContext.CurrentUserinformation.login_code,
                            DateTime.Now.ToString("dd-MMM-yyyy"), 'N', i, 0, 0, 0, spec.Remarks);
                        _dbContext.ExecuteSqlCommand(insertQuery);
                        i++;
                    }
                    else
                    {
                        string updateQuery = $@"UPDATE ip_item_qc_setup 
                       SET SPECIFICATION = '{spec.Value0}', ROLL1 = '{spec.Value1}',ROLL2 = '{spec.Value2}',
                        ROLL3 = '{spec.Value3}',REMARKS = '{spec.Remarks}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE IP_ITEM_QC_SETUP_NO = '{data.TRANSACTION_NO}' and QC_EDESC  = '{spec.Name}' ";
                        _dbContext.ExecuteSqlCommand(updateQuery);
                    }
                }
                string query_qc_code_transaction = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(QC_CODE)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(QC_CODE, '^\d+$')";
                string qc_code_transaction = this._dbContext.SqlQuery<string>(query_qc_code_transaction).FirstOrDefault();
                if (incoming_no == null)
                {
                    string insertQCParameterTranQuery = string.Format(@"
                            INSERT INTO QC_PARAMETER_TRANSACTION (
                                TRANSACTION_NO, REFERENCE_NO, ITEM_CODE, SERIAL_NO, QC_CODE, FORM_CODE,
                                COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG,
                                MANUAL_NO, GRN_NO, QC_DATE, VENDOR_NAME,
                                LC_NO, CUSTOMER_INVOICE_NO, GRN_DATE, QUANTITY, NAME, REMARKS
                            )
                            VALUES(
                                '{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9}','DD-MON-YYYY'),
                                '{10}','{11}','{12}',TO_DATE('{13}','DD-MON-YYYY'),'{14}',
                                '{15}','{16}',TO_DATE('{17}','DD-MON-YYYY'),{18},'{19}','{20}'
                            )",
                             qc_no, data.VOUCHER_NO, data.ITEM_CODE, serial_no_qc_setup, qc_code_transaction, form_code, _workContext.CurrentUserinformation.company_code,
                             _workContext.CurrentUserinformation.branch_code,
                             _workContext.CurrentUserinformation.login_code,
                             DateTime.Now.ToString("dd-MMM-yyyy"), 'N',
                             data.MANUAL_NO, data.VOUCHER_NO,
                             data.QC_DATE.ToString("dd-MMM-yyyy"), data.VENDOR_NAME, data.LC_NO, data.INVOICE_NO,
                             data.GRN_DATE.ToString("dd-MMM-yyyy"), data.QUANTITY, data.NAME, data.Remarks);
                    _dbContext.ExecuteSqlCommand(insertQCParameterTranQuery);

                    if (data.MATERIAL_CODE != null)
                    {
                        string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                       qc_no, data.MATERIAL_CODE);
                        _dbContext.ExecuteSqlCommand(insertParam);
                    }
                }
                else
                {
                    if (data.MATERIAL_CODE != null)
                    {
                        string updateParam = $@"
                        UPDATE PRODUCT_PARAM_MAP SET PRODUCT_ID ='{data.MATERIAL_CODE}' WHERE TRANSACTION_NO = '{data.TRANSACTION_NO}' ";
                        _dbContext.ExecuteSqlCommand(updateParam);
                    }
                    string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}',
                       WHERE TRANSACTION_NO = '{data.TRANSACTION_NO}' ";
                    _dbContext.ExecuteSqlCommand(updateQuery);
                   
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool InsertInComingMaterialSampleData(QCDetail data)
        {
            try
            {
                string query_serial_no = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                string serial_no = this._dbContext.SqlQuery<string>(query_serial_no).FirstOrDefault();

                string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM IP_ITEM_QC_SETUP WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();

                //string query_reference_no = $@"SELECT LPAD(COALESCE(MAX(TO_NUMBER(REFERENCE_NO)), 0)  + 1, 2, '0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION";
                //string qc_no = GetQCQAVoucherNo("InComingMaterialSample");
                string query_reference_no = $@"SELECT LPAD(NVL(MAX(CASE WHEN REGEXP_LIKE(REFERENCE_NO, '^[0-9]+$')  THEN TO_NUMBER(REFERENCE_NO)END),  0) + 1,2,'0') AS REFERENCE_NO FROM QC_PARAMETER_TRANSACTION";
                string qc_no = GetQCQAVoucherNo("InComingMaterialSample");

                string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.TRANSACTION_NO}'";
                string incoming_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'IS'";
                string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();

                var specifications = new List<Specification>
                                    {
                                        new Specification("Thickness",data.Thickness0, data.Thickness1, data.Thickness2, data.Thickness3,data.Remarks1),
                                        new Specification("RollDiameter",data.RollDiameter0, data.RollDiameter1, data.RollDiameter2, data.RollDiameter3,data.Remarks2),
                                        new Specification("PH",data.PH0, data.PH1, data.PH2, data.PH3,data.Remarks3),
                                        new Specification("UnpleasantSmell",data.UnpleasantSmell0, data.UnpleasantSmell1, data.UnpleasantSmell2, data.UnpleasantSmell3,data.Remarks4),
                                        new Specification("DustDirt",data.DustDirt0, data.DustDirt1, data.DustDirt2, data.DustDirt3,data.Remarks5),
                                        new Specification("DamagingMaterial",data.DamagingMaterial0, data.DamagingMaterial1, data.DamagingMaterial2, data.DamagingMaterial3,data.Remarks6),
                                        new Specification("CoreDamaging",data.CoreDamaging0, data.CoreDamaging1, data.CoreDamaging2, data.CoreDamaging3,data.Remarks7),
                                        new Specification("Width",data.Width0, data.Width1, data.Width2, data.Width3,data.Remarks8),
                                        new Specification("GSM",data.GSM0, data.GSM1, data.GSM2, data.GSM3,data.Remarks9),
                                        new Specification("TensileCD",data.TensileCD0, data.TensileCD1, data.TensileCD2, data.TensileCD3,data.Remarks10),
                                        new Specification("TensileMD",data.TensileMD0, data.TensileMD1, data.TensileMD2, data.TensileMD3,data.Remarks11),
                                        new Specification("VisualInspection",data.VisualInspection0, data.VisualInspection1, data.VisualInspection2, data.VisualInspection3,data.Remarks12)
                                    };
                int i = 1;
                foreach (var spec in specifications)
                {
                    string query_qc_code = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(QC_CODE)), 0) + 1) FROM ip_item_qc_setup WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(QC_CODE, '^\d+$')";
                    string qc_code = this._dbContext.SqlQuery<string>(query_qc_code).FirstOrDefault();
                    if (incoming_no == null)
                    {
                        string insertQuery = string.Format(@"
                    INSERT INTO ip_item_qc_setup (IP_ITEM_QC_SETUP_NO,ITEM_CODE, QC_CODE, QC_EDESC, QC_NDESC, SPECIFICATION,ROLL1, ROLL2, ROLL3, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG,SERIAL_NO,STANDARD_VALUE,HIGH_VALUE,MIN_VALUE,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}', TO_DATE('{11}', 'DD-MON-YYYY'),'{12}','{13}','{14}','{15}','{16}','{17}')",
                                    qc_no, data.ITEM_CODE, qc_code, spec.Name, spec.Name, spec.Value0 ?? "0",
                            spec.Value1 ?? "0", spec.Value2 ?? "0", spec.Value3 ?? "0",
                            _workContext.CurrentUserinformation.company_code,
                            _workContext.CurrentUserinformation.login_code,
                            DateTime.Now.ToString("dd-MMM-yyyy"), 'N', i, 0, 0, 0, spec.Remarks);
                        _dbContext.ExecuteSqlCommand(insertQuery);
                        i++;
                    }
                    else
                    {
                        string updateQuery = $@"UPDATE ip_item_qc_setup 
                       SET SPECIFICATION= '{spec.Value0}', ROLL1 = '{spec.Value1}',ROLL2 = '{spec.Value2}',
                        ROLL3 = '{spec.Value3}',REMARKS = '{spec.Remarks}',
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE IP_ITEM_QC_SETUP_NO = '{data.TRANSACTION_NO}' and QC_EDESC  = '{spec.Name}' ";
                        _dbContext.ExecuteSqlCommand(updateQuery);
                    }
                }
                string query_qc_code_transaction = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(QC_CODE)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(QC_CODE, '^\d+$')";
                string qc_code_transaction = this._dbContext.SqlQuery<string>(query_qc_code_transaction).FirstOrDefault();
                if (incoming_no == null)
                {
                    string insertQCParameterTranQuery = string.Format(@"INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO
                                    ,REFERENCE_NO,ITEM_CODE,SERIAL_NO,QC_CODE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                                    ,CREATED_DATE,DELETED_FLAG,MANUAL_NO,QUANTITY,REMARKS,CUSTOMER_INVOICE_NO,VENDOR_NAME)
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9:dd-MMM-yyyy}', 'DD-MON-YYYY'),'{10}','{11}','{12}','{13}','{14}','{15}')"
                                   , qc_no, data.VOUCHER_NO, data.ITEM_CODE, serial_no_qc_setup, qc_code_transaction, form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                                   _workContext.CurrentUserinformation.login_code
                                   , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', data.MANUAL_NO, data.QUANTITY, data.Remarks,data.INVOICE_NO,data.VENDOR_NAME);
                    _dbContext.ExecuteSqlCommand(insertQCParameterTranQuery);

                    if (data.MATERIAL_CODE != null)
                    {
                        string insertParam = string.Format(@"
                        INSERT INTO PRODUCT_PARAM_MAP (INSPECTION_NO,PRODUCT_ID)
                        VALUES('{0}', '{1}')",
                                       qc_no, data.MATERIAL_CODE);
                        _dbContext.ExecuteSqlCommand(insertParam);
                    }
                }
                else
                {
                    if (data.MATERIAL_CODE != null)
                    {
                        string updateParam = $@"
                        UPDATE PRODUCT_PARAM_MAP SET PRODUCT_ID ='{data.MATERIAL_CODE}' WHERE TRANSACTION_NO = '{data.TRANSACTION_NO}' ";
                        _dbContext.ExecuteSqlCommand(updateParam);
                    }
                    string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           QUANTITY = '{data.QUANTITY}',
                           CUSTOMER_INVOICE_NO= '{data.INVOICE_NO}',
                           VENDOR_NAME = '{data.VENDOR_NAME}', 
                           REMARKS = '{data.Remarks}',  
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE TRANSACTION_NO = '{data.TRANSACTION_NO}' ";
                    _dbContext.ExecuteSqlCommand(updateQuery);

                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Items GetEditMaterialDetails(string transactionno)
        {
            string query = $@"SELECT DISTINCT QPT.item_code,
    QPT.transaction_no AS QC_NO,
    QPT.QC_DATE,
    QPT.reference_no AS Voucher_No,
    (SELECT SUPPLIER_EDESC FROM ip_supplier_setup WHERE supplier_code = QPT.VENDOR_NAME AND ROWNUM = 1) AS supplier_edesc,
    QPT.VENDOR_NAME,
    QPT.Name AS item_edesc,
    QPT.GRN_DATE,
    QPT.GRN_NO,
    TO_NUMBER(NVL(QPT.QUANTITY, 0)) AS quantity,
    QPT.LC_NO AS to_location_code,
     qc_data.Thickness AS thickness,
    qc_data.Thick1,
    qc_data.Thickness1,
    qc_data.Thickness2,
    qc_data.Thickness3,
     qc_data.Thick2 AS RollDiameter0 ,
    qc_data.RollDiameter1,
    qc_data.RollDiameter2,
    qc_data.RollDiameter3,
    qc_data.Thick3,
    qc_data.PH1,
    qc_data.PH2,
    qc_data.PH3,
     qc_data.Thick4,
    qc_data.UnpleasantSmell1,
    qc_data.UnpleasantSmell2,
    qc_data.UnpleasantSmell3,
     qc_data.Thick5,
    qc_data.DustDirt1,
    qc_data.DustDirt2,
    qc_data.DustDirt3,
     qc_data.Thick6,
    qc_data.DamagingMaterial1,
    qc_data.DamagingMaterial2,
    qc_data.DamagingMaterial3,
     qc_data.Thick7,
    qc_data.CoreDamaging1,
    qc_data.CoreDamaging2,
    qc_data.CoreDamaging3,
     qc_data.Thick8,
    qc_data.Width1,
    qc_data.Width2,
    qc_data.Width3,
     qc_data.Thick9,
    qc_data.GSM1,
    qc_data.GSM2,
    qc_data.GSM3,
     qc_data.Thick10,
    qc_data.TensileCD1,
    qc_data.TensileCD2,
    qc_data.TensileCD3,
     qc_data.Thick11,
    qc_data.TensileMD1,
    qc_data.TensileMD2,
    qc_data.TensileMD3,
     qc_data.Thick12,
    qc_data.VisualInspection1,
    qc_data.VisualInspection2,
    qc_data.VisualInspection3,
    qc_data.Remarks1,
    qc_data.Remarks2,
    qc_data.Remarks3,
    qc_data.Remarks4,
    qc_data.Remarks5,
    qc_data.Remarks6,
    qc_data.Remarks7,
    qc_data.Remarks8,
    qc_data.Remarks9,
    qc_data.Remarks10,
    qc_data.Remarks11,
    qc_data.Remarks12,
    qc_data.PH,
    qc_data.UNPLEASANT_SMELL_ODOUR,
    qc_data.DUST_DIRT,
    qc_data.DAMAGING_MATERIAL,
    qc_data.CORE_DAMAGING,
    qc_data.SIZE_WIDTH,
    qc_data.GSM,
    qc_data.TENSILE_CD,
    qc_data.TENSILE_MD,
   qc_data.VISUAL_INSPECTION ,
    QPT.Created_By AS CreatedBy,
   QPT.CUSTOMER_INVOICE_NO AS invoice_no,
   QPT.MANUAL_NO,
    QPT.Remarks,
    PPm.PRODUCT_ID AS MATERIAL_CODE
FROM ip_purchase_mrr ipm
LEFT JOIN ip_supplier_setup iss ON iss.supplier_code = ipm.supplier_code
LEFT JOIN IP_PRODUCT_ITEM_MASTER_SETUP iims ON iims.item_code = ipm.item_code
INNER JOIN IP_ITEM_SPEC_SETUP iiss ON iiss.item_code = ipm.item_code
LEFT JOIN QC_PARAMETER_TRANSACTION QPT ON QPT.reference_no = ipm.mrr_no
LEFT JOIN PRODUCT_PARAM_MAP ppm ON ppm.inspection_no = QPT.TRANSACTION_NO
LEFT JOIN (
    SELECT
        IP_ITEM_QC_SETUP_NO,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thick1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thickness,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN ROLL1 END) AS Thickness1,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL2 END) AS Thickness2,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL3 END) AS Thickness3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN SPECIFICATION END) AS Thick2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL1 END) AS RollDiameter1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL2 END) AS RollDiameter2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL3 END) AS RollDiameter3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS Thick3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS PH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL1 END) AS PH1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL2 END) AS PH2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL3 END) AS PH3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS Thick4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS UNPLEASANT_SMELL_ODOUR,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL1 END) AS UnpleasantSmell1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL2 END) AS UnpleasantSmell2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL3 END) AS UnpleasantSmell3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS Thick5,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS DUST_DIRT,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL1 END) AS DustDirt1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL2 END) AS DustDirt2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL3 END) AS DustDirt3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS Thick6,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS DAMAGING_MATERIAL,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL1 END) AS DamagingMaterial1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL2 END) AS DamagingMaterial2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL3 END) AS DamagingMaterial3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS Thick7,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS CORE_DAMAGING,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL1 END) AS CoreDamaging1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL2 END) AS CoreDamaging2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL3 END) AS CoreDamaging3,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN SPECIFICATION END) AS Thick8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN TO_NUMBER(SPECIFICATION) END) AS SIZE_WIDTH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL1 END) AS Width1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL2 END) AS Width2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL3 END) AS Width3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS Thick9,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS GSM,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL1 END) AS GSM1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL2 END) AS GSM2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL3 END) AS GSM3,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS Thick10,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS TENSILE_CD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL1 END) AS TensileCD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL2 END) AS TensileCD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL3 END) AS TensileCD3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS Thick11,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS TENSILE_MD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL1 END) AS TensileMD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL2 END) AS TensileMD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL3 END) AS TensileMD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS Thick12,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS VISUAL_INSPECTION,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL1 END) AS VisualInspection1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL2 END) AS VisualInspection2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL3 END) AS VisualInspection3,      
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN Remarks END) AS Remarks1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('RollDiameter') THEN Remarks END) AS Remarks2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('PH') THEN Remarks END) AS Remarks3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN Remarks END) AS Remarks4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN Remarks END) AS Remarks5,     
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN Remarks END) AS Remarks6,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN Remarks END) AS Remarks7,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN Remarks END) AS Remarks8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN Remarks END) AS Remarks9,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN Remarks END) AS Remarks10,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN Remarks END) AS Remarks11,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN Remarks END) AS Remarks12
        
    FROM ip_item_qc_setup
    GROUP BY IP_ITEM_QC_SETUP_NO
) qc_data ON qc_data.IP_ITEM_QC_SETUP_NO= QPT.transaction_no

WHERE QPT.transaction_no = '{transactionno}'";

            // Execute the query and get the result
            List<Items> items = _dbContext.SqlQuery<Items>(query).ToList();
            Items qc = null;
            // Ensure there are items returned before proceeding
            if (items.Any())
            {
                qc = new Items
                {
                    TRANSACTION_NO = transactionno,
                    QC_NO = items.FirstOrDefault()?.QC_NO,
                    QC_DATE = items.FirstOrDefault()?.QC_DATE,
                    item_code = items.FirstOrDefault()?.item_code,
                    MATERIAL_CODE = items.FirstOrDefault()?.MATERIAL_CODE,
                    Voucher_No = items.FirstOrDefault()?.Voucher_No,
                    supplier_edesc = items.FirstOrDefault()?.supplier_edesc,
                    item_edesc = items.FirstOrDefault()?.item_edesc,
                    RECEIPT_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    to_location_code = items.FirstOrDefault()?.to_location_code,
                    invoice_no = items.FirstOrDefault()?.invoice_no,
                    GRN_NO = items.FirstOrDefault()?.GRN_NO,
                    GRN_DATE = items.FirstOrDefault()?.GRN_DATE,
                    quantity = Convert.ToDecimal(items.FirstOrDefault()?.quantity),
                    MANUAL_NO = items.FirstOrDefault()?.MANUAL_NO,
                    REMARKS = items.FirstOrDefault()?.REMARKS,
                    thickness = items.FirstOrDefault()?.thickness,
                    RollDiameter = items.FirstOrDefault()?.RollDiameter,
                    PH = items.FirstOrDefault()?.PH,
                    UNPLEASANT_SMELL_ODOUR = items.FirstOrDefault()?.UNPLEASANT_SMELL_ODOUR,
                    Dust_Dirt = items.FirstOrDefault()?.Dust_Dirt,
                    Damaging_Material = items.FirstOrDefault()?.Damaging_Material,
                    Core_Damaging = items.FirstOrDefault()?.Core_Damaging,
                    SIZE_WIDTH = items.FirstOrDefault()?.SIZE_WIDTH,
                    GSM = items.FirstOrDefault()?.GSM,
                    Tensile_CD = items.FirstOrDefault()?.Tensile_CD,
                    Tensile_MD = items.FirstOrDefault()?.Tensile_MD,
                    Visual_Inspection = items.FirstOrDefault()?.Visual_Inspection,
                    Thickness0 = items.FirstOrDefault()?.Thickness0,
                    Thickness1 = items.FirstOrDefault()?.Thickness1,
                    Thickness2 = items.FirstOrDefault()?.Thickness2,
                    Thickness3 = items.FirstOrDefault()?.Thickness3,
                    RollDiameter0 = items.FirstOrDefault()?.RollDiameter0,
                    RollDiameter1 = items.FirstOrDefault()?.RollDiameter1,
                    RollDiameter2 = items.FirstOrDefault()?.RollDiameter2,
                    RollDiameter3 = items.FirstOrDefault()?.RollDiameter3,
                    PH1 = items.FirstOrDefault()?.PH1,
                    PH2 = items.FirstOrDefault()?.PH2,
                    PH3 = items.FirstOrDefault()?.PH3,
                    UnpleasantSmell1 = items.FirstOrDefault()?.UnpleasantSmell1,
                    UnpleasantSmell2 = items.FirstOrDefault()?.UnpleasantSmell2,
                    UnpleasantSmell3 = items.FirstOrDefault()?.UnpleasantSmell3,
                    DustDirt1 = items.FirstOrDefault()?.DustDirt1,
                    DustDirt2 = items.FirstOrDefault()?.DustDirt2,
                    DustDirt3 = items.FirstOrDefault()?.DustDirt3,
                    DamagingMaterial1 = items.FirstOrDefault()?.DamagingMaterial1,
                    DamagingMaterial2 = items.FirstOrDefault()?.DamagingMaterial2,
                    DamagingMaterial3 = items.FirstOrDefault()?.DamagingMaterial3,
                    CoreDamaging1 = items.FirstOrDefault()?.CoreDamaging1,
                    CoreDamaging2 = items.FirstOrDefault()?.CoreDamaging2,
                    CoreDamaging3 = items.FirstOrDefault()?.CoreDamaging3,
                    VisualInspection1 = items.FirstOrDefault()?.VisualInspection1,
                    VisualInspection2 = items.FirstOrDefault()?.VisualInspection2,
                    VisualInspection3 = items.FirstOrDefault()?.VisualInspection3,
                    Width1 = items.FirstOrDefault()?.Width1,
                    Width2 = items.FirstOrDefault()?.Width2,
                    Width3 = items.FirstOrDefault()?.Width3,
                    GSM1 = items.FirstOrDefault()?.GSM1,
                    GSM2 = items.FirstOrDefault()?.GSM2,
                    GSM3 = items.FirstOrDefault()?.GSM3,
                    TensileCD1 = items.FirstOrDefault()?.TensileCD1,
                    TensileCD2 = items.FirstOrDefault()?.TensileCD2,
                    TensileCD3 = items.FirstOrDefault()?.TensileCD3,
                    TensileMD1 = items.FirstOrDefault()?.TensileMD1,
                    TensileMD2 = items.FirstOrDefault()?.TensileMD2,
                    TensileMD3 = items.FirstOrDefault()?.TensileMD3,
                    Remarks1 = items.FirstOrDefault()?.Remarks1,
                    Remarks2 = items.FirstOrDefault()?.Remarks2,
                    Remarks3 = items.FirstOrDefault()?.Remarks3,
                    Remarks4 = items.FirstOrDefault()?.Remarks4,
                    Remarks5 = items.FirstOrDefault()?.Remarks5,
                    Remarks6 = items.FirstOrDefault()?.Remarks6,
                    Remarks7 = items.FirstOrDefault()?.Remarks7,
                    Remarks8 = items.FirstOrDefault()?.Remarks8,
                    Remarks9 = items.FirstOrDefault()?.Remarks9,
                    Remarks10 = items.FirstOrDefault()?.Remarks10,
                    Remarks11 = items.FirstOrDefault()?.Remarks11,
                    Remarks12 = items.FirstOrDefault()?.Remarks12
                };
            }
            // If no valid voucherno, return the view with a default state (qc is null)
            return qc;
        }

        public IncomingMaterial GetIncomingMaterialsDetailReport(string transactionno)
        {
            string query = $@"SELECT DISTINCT QPT.item_code,
    QPT.transaction_no AS QC_NO,
    QPT.QC_DATE,
    QPT.reference_no AS Voucher_No,
    (SELECT SUPPLIER_EDESC FROM ip_supplier_setup WHERE supplier_code = QPT.VENDOR_NAME AND ROWNUM = 1) AS supplier_edesc,
    QPT.VENDOR_NAME,
    QPT.Name AS item_edesc,
    QPT.GRN_DATE,
    QPT.GRN_NO,
    TO_NUMBER(NVL(QPT.QUANTITY, 0)) AS quantity,
    QPT.LC_NO AS to_location_code,
     qc_data.Thickness AS thickness,
    qc_data.Thick1,
    qc_data.Thickness1,
    qc_data.Thickness2,
    qc_data.Thickness3,
     qc_data.Thick2 AS RollDiameter0 ,
    qc_data.RollDiameter1,
    qc_data.RollDiameter2,
    qc_data.RollDiameter3,
    qc_data.Thick3,
    qc_data.PH1,
    qc_data.PH2,
    qc_data.PH3,
     qc_data.Thick4,
    qc_data.UnpleasantSmell1,
    qc_data.UnpleasantSmell2,
    qc_data.UnpleasantSmell3,
     qc_data.Thick5,
    qc_data.DustDirt1,
    qc_data.DustDirt2,
    qc_data.DustDirt3,
     qc_data.Thick6,
    qc_data.DamagingMaterial1,
    qc_data.DamagingMaterial2,
    qc_data.DamagingMaterial3,
     qc_data.Thick7,
    qc_data.CoreDamaging1,
    qc_data.CoreDamaging2,
    qc_data.CoreDamaging3,
     qc_data.Thick8,
    qc_data.Width1,
    qc_data.Width2,
    qc_data.Width3,
     qc_data.Thick9,
    qc_data.GSM1,
    qc_data.GSM2,
    qc_data.GSM3,
     qc_data.Thick10,
    qc_data.TensileCD1,
    qc_data.TensileCD2,
    qc_data.TensileCD3,
     qc_data.Thick11,
    qc_data.TensileMD1,
    qc_data.TensileMD2,
    qc_data.TensileMD3,
     qc_data.Thick12,
    qc_data.VisualInspection1,
    qc_data.VisualInspection2,
    qc_data.VisualInspection3,
    qc_data.Remarks1,
    qc_data.Remarks2,
    qc_data.Remarks3,
    qc_data.Remarks4,
    qc_data.Remarks5,
    qc_data.Remarks6,
    qc_data.Remarks7,
    qc_data.Remarks8,
    qc_data.Remarks9,
    qc_data.Remarks10,
    qc_data.Remarks11,
    qc_data.Remarks12,
    qc_data.PH,
    qc_data.UNPLEASANT_SMELL_ODOUR,
    qc_data.DUST_DIRT,
    qc_data.DAMAGING_MATERIAL,
    qc_data.CORE_DAMAGING,
    qc_data.SIZE_WIDTH,
    qc_data.GSM,
    qc_data.TENSILE_CD,
    qc_data.TENSILE_MD,
   qc_data.VISUAL_INSPECTION ,
    QPT.Created_By AS CreatedBy,
     QPT.CUSTOMER_INVOICE_NO AS invoice_no,
    QPT.MANUAL_NO,
    QPT.Remarks,
    PPm.PRODUCT_ID AS MATERIAL_CODE
FROM ip_purchase_mrr ipm
LEFT JOIN ip_supplier_setup iss ON iss.supplier_code = ipm.supplier_code
LEFT JOIN IP_PRODUCT_ITEM_MASTER_SETUP iims ON iims.item_code = ipm.item_code
INNER JOIN IP_ITEM_SPEC_SETUP iiss ON iiss.item_code = ipm.item_code
LEFT JOIN QC_PARAMETER_TRANSACTION QPT ON QPT.reference_no = ipm.mrr_no
LEFT JOIN PRODUCT_PARAM_MAP ppm ON ppm.inspection_no = QPT.TRANSACTION_NO
LEFT JOIN (
    SELECT
        IP_ITEM_QC_SETUP_NO,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thick1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thickness,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN ROLL1 END) AS Thickness1,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL2 END) AS Thickness2,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL3 END) AS Thickness3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN SPECIFICATION END) AS Thick2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL1 END) AS RollDiameter1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL2 END) AS RollDiameter2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL3 END) AS RollDiameter3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS Thick3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS PH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL1 END) AS PH1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL2 END) AS PH2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL3 END) AS PH3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS Thick4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS UNPLEASANT_SMELL_ODOUR,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL1 END) AS UnpleasantSmell1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL2 END) AS UnpleasantSmell2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL3 END) AS UnpleasantSmell3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS Thick5,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS DUST_DIRT,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL1 END) AS DustDirt1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL2 END) AS DustDirt2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL3 END) AS DustDirt3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS Thick6,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS DAMAGING_MATERIAL,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL1 END) AS DamagingMaterial1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL2 END) AS DamagingMaterial2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL3 END) AS DamagingMaterial3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS Thick7,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS CORE_DAMAGING,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL1 END) AS CoreDamaging1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL2 END) AS CoreDamaging2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL3 END) AS CoreDamaging3,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN SPECIFICATION END) AS Thick8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN TO_NUMBER(SPECIFICATION) END) AS SIZE_WIDTH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL1 END) AS Width1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL2 END) AS Width2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL3 END) AS Width3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS Thick9,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS GSM,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL1 END) AS GSM1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL2 END) AS GSM2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL3 END) AS GSM3,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS Thick10,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS TENSILE_CD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL1 END) AS TensileCD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL2 END) AS TensileCD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL3 END) AS TensileCD3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS Thick11,
          MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS TENSILE_MD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL1 END) AS TensileMD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL2 END) AS TensileMD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL3 END) AS TensileMD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS Thick12,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS VISUAL_INSPECTION,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL1 END) AS VisualInspection1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL2 END) AS VisualInspection2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL3 END) AS VisualInspection3,      
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN Remarks END) AS Remarks1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('RollDiameter') THEN Remarks END) AS Remarks2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('PH') THEN Remarks END) AS Remarks3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN Remarks END) AS Remarks4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN Remarks END) AS Remarks5,     
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN Remarks END) AS Remarks6,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN Remarks END) AS Remarks7,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN Remarks END) AS Remarks8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN Remarks END) AS Remarks9,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN Remarks END) AS Remarks10,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN Remarks END) AS Remarks11,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN Remarks END) AS Remarks12
        
    FROM ip_item_qc_setup
    GROUP BY IP_ITEM_QC_SETUP_NO
) qc_data ON qc_data.IP_ITEM_QC_SETUP_NO= QPT.transaction_no

WHERE QPT.transaction_no = '{transactionno}'";

            // Execute the query and get the result
            List<IncomingMaterial> items = _dbContext.SqlQuery<IncomingMaterial>(query).ToList();
            IncomingMaterial qc = null;
            // Ensure there are items returned before proceeding
            if (items.Any())
            {
                qc = new IncomingMaterial
                {
                    TRANSACTION_NO = transactionno,
                    QC_NO = items.FirstOrDefault()?.QC_NO,
                    QC_DATE = items.FirstOrDefault()?.QC_DATE,
                    item_code = items.FirstOrDefault()?.item_code,
                    MATERIAL_CODE = items.FirstOrDefault()?.MATERIAL_CODE,
                    Voucher_No = items.FirstOrDefault()?.Voucher_No,
                    supplier_edesc = items.FirstOrDefault()?.supplier_edesc,
                    item_edesc = items.FirstOrDefault()?.item_edesc,
                    RECEIPT_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    to_location_code = items.FirstOrDefault()?.to_location_code,
                    invoice_no = items.FirstOrDefault()?.invoice_no,
                    GRN_NO = items.FirstOrDefault()?.GRN_NO,
                    GRN_DATE = items.FirstOrDefault()?.GRN_DATE,
                    quantity = Convert.ToDecimal(items.FirstOrDefault()?.quantity),
                    MANUAL_NO = items.FirstOrDefault()?.MANUAL_NO,
                    REMARKS = items.FirstOrDefault()?.REMARKS,
                    thickness = items.FirstOrDefault()?.thickness,
                    RollDiameter = items.FirstOrDefault()?.RollDiameter,
                    PH = items.FirstOrDefault()?.PH,
                    UNPLEASANT_SMELL_ODOUR = items.FirstOrDefault()?.UNPLEASANT_SMELL_ODOUR,
                    Dust_Dirt = items.FirstOrDefault()?.Dust_Dirt,
                    Damaging_Material = items.FirstOrDefault()?.Damaging_Material,
                    Core_Damaging = items.FirstOrDefault()?.Core_Damaging,
                    SIZE_WIDTH = items.FirstOrDefault()?.SIZE_WIDTH,
                    GSM = items.FirstOrDefault()?.GSM,
                    Tensile_CD = items.FirstOrDefault()?.Tensile_CD,
                    Tensile_MD = items.FirstOrDefault()?.Tensile_MD,
                    Visual_Inspection = items.FirstOrDefault()?.Visual_Inspection,
                    Thickness0 = items.FirstOrDefault()?.Thickness1,
                    Thickness1 = items.FirstOrDefault()?.Thickness1,
                    Thickness2 = items.FirstOrDefault()?.Thickness2,
                    Thickness3 = items.FirstOrDefault()?.Thickness3,
                    RollDiameter0 = items.FirstOrDefault()?.RollDiameter0,
                    RollDiameter1 = items.FirstOrDefault()?.RollDiameter1,
                    RollDiameter2 = items.FirstOrDefault()?.RollDiameter2,
                    RollDiameter3 = items.FirstOrDefault()?.RollDiameter3,
                    PH1 = items.FirstOrDefault()?.PH1,
                    PH2 = items.FirstOrDefault()?.PH2,
                    PH3 = items.FirstOrDefault()?.PH3,
                    UnpleasantSmell1 = items.FirstOrDefault()?.UnpleasantSmell1,
                    UnpleasantSmell2 = items.FirstOrDefault()?.UnpleasantSmell2,
                    UnpleasantSmell3 = items.FirstOrDefault()?.UnpleasantSmell3,
                    DustDirt1 = items.FirstOrDefault()?.DustDirt1,
                    DustDirt2 = items.FirstOrDefault()?.DustDirt2,
                    DustDirt3 = items.FirstOrDefault()?.DustDirt3,
                    DamagingMaterial1 = items.FirstOrDefault()?.DamagingMaterial1,
                    DamagingMaterial2 = items.FirstOrDefault()?.DamagingMaterial2,
                    DamagingMaterial3 = items.FirstOrDefault()?.DamagingMaterial3,
                    CoreDamaging1 = items.FirstOrDefault()?.CoreDamaging1,
                    CoreDamaging2 = items.FirstOrDefault()?.CoreDamaging2,
                    CoreDamaging3 = items.FirstOrDefault()?.CoreDamaging3,
                    VisualInspection1 = items.FirstOrDefault()?.VisualInspection1,
                    VisualInspection2 = items.FirstOrDefault()?.VisualInspection2,
                    VisualInspection3 = items.FirstOrDefault()?.VisualInspection3,
                    Width1 = items.FirstOrDefault()?.Width1,
                    Width2 = items.FirstOrDefault()?.Width2,
                    Width3 = items.FirstOrDefault()?.Width3,
                    GSM1 = items.FirstOrDefault()?.GSM1,
                    GSM2 = items.FirstOrDefault()?.GSM2,
                    GSM3 = items.FirstOrDefault()?.GSM3,
                    TensileCD1 = items.FirstOrDefault()?.TensileCD1,
                    TensileCD2 = items.FirstOrDefault()?.TensileCD2,
                    TensileCD3 = items.FirstOrDefault()?.TensileCD3,
                    TensileMD1 = items.FirstOrDefault()?.TensileMD1,
                    TensileMD2 = items.FirstOrDefault()?.TensileMD2,
                    TensileMD3 = items.FirstOrDefault()?.TensileMD3,
                    Remarks1 = items.FirstOrDefault()?.Remarks1,
                    Remarks2 = items.FirstOrDefault()?.Remarks2,
                    Remarks3 = items.FirstOrDefault()?.Remarks3,
                    Remarks4 = items.FirstOrDefault()?.Remarks4,
                    Remarks5 = items.FirstOrDefault()?.Remarks5,
                    Remarks6 = items.FirstOrDefault()?.Remarks6,
                    Remarks7 = items.FirstOrDefault()?.Remarks7,
                    Remarks8 = items.FirstOrDefault()?.Remarks8,
                    Remarks9 = items.FirstOrDefault()?.Remarks9,
                    Remarks10 = items.FirstOrDefault()?.Remarks10,
                    Remarks11 = items.FirstOrDefault()?.Remarks11,
                    Remarks12 = items.FirstOrDefault()?.Remarks12
                    //TRANSACTION_NO = transactionno,
                    //QC_NO = items.FirstOrDefault()?.QC_NO,
                    //QC_DATE = items.FirstOrDefault()?.QC_DATE,
                    //item_code = items.FirstOrDefault()?.item_code,
                    //Voucher_No = items.FirstOrDefault()?.Voucher_No,
                    //supplier_edesc = items.FirstOrDefault()?.supplier_edesc,
                    //item_edesc = items.FirstOrDefault()?.item_edesc,
                    //RECEIPT_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    //to_location_code = items.FirstOrDefault()?.to_location_code,
                    //invoice_no = items.FirstOrDefault()?.invoice_no,
                    //GRN_NO = items.FirstOrDefault()?.GRN_NO,
                    //GRN_DATE = items.FirstOrDefault()?.GRN_DATE,
                    //quantity = Convert.ToDecimal(items.FirstOrDefault()?.quantity),
                    //REMARKS = items.FirstOrDefault()?.REMARKS,
                    //thickness = items.FirstOrDefault()?.thickness,
                    //RollDiameter = items.FirstOrDefault()?.RollDiameter,
                    //PH = items.FirstOrDefault()?.PH,
                    //UNPLEASANT_SMELL_ODOUR = items.FirstOrDefault()?.UNPLEASANT_SMELL_ODOUR,
                    //Dust_Dirt = items.FirstOrDefault()?.Dust_Dirt,
                    //Damaging_Material = items.FirstOrDefault()?.Damaging_Material,
                    //Core_Damaging = items.FirstOrDefault()?.Core_Damaging,
                    //SIZE_WIDTH = items.FirstOrDefault()?.SIZE_WIDTH,
                    //GSM = items.FirstOrDefault()?.GSM,
                    //Tensile_CD = items.FirstOrDefault()?.Tensile_CD,
                    //Tensile_MD = items.FirstOrDefault()?.Tensile_MD,
                    //Visual_Inspection = items.FirstOrDefault()?.Visual_Inspection,
                    //Thickness1 = items.FirstOrDefault()?.Thickness1,
                    //Thickness2 = items.FirstOrDefault()?.Thickness2,
                    //Thickness3 = items.FirstOrDefault()?.Thickness3,
                    //RollDiameter1 = items.FirstOrDefault()?.RollDiameter1,
                    //RollDiameter2 = items.FirstOrDefault()?.RollDiameter2,
                    //RollDiameter3 = items.FirstOrDefault()?.RollDiameter3,
                    //PH1 = items.FirstOrDefault()?.PH1,
                    //PH2 = items.FirstOrDefault()?.PH2,
                    //PH3 = items.FirstOrDefault()?.PH3,
                    //UnpleasantSmell1 = items.FirstOrDefault()?.UnpleasantSmell1,
                    //UnpleasantSmell2 = items.FirstOrDefault()?.UnpleasantSmell2,
                    //UnpleasantSmell3 = items.FirstOrDefault()?.UnpleasantSmell3,
                    //DustDirt1 = items.FirstOrDefault()?.DustDirt1,
                    //DustDirt2 = items.FirstOrDefault()?.DustDirt2,
                    //DustDirt3 = items.FirstOrDefault()?.DustDirt3,
                    //DamagingMaterial1 = items.FirstOrDefault()?.DamagingMaterial1,
                    //DamagingMaterial2 = items.FirstOrDefault()?.DamagingMaterial2,
                    //DamagingMaterial3 = items.FirstOrDefault()?.DamagingMaterial3,
                    //CoreDamaging1 = items.FirstOrDefault()?.CoreDamaging1,
                    //CoreDamaging2 = items.FirstOrDefault()?.CoreDamaging2,
                    //CoreDamaging3 = items.FirstOrDefault()?.CoreDamaging3,
                    //VisualInspection1 = items.FirstOrDefault()?.VisualInspection1,
                    //VisualInspection2 = items.FirstOrDefault()?.VisualInspection2,
                    //VisualInspection3 = items.FirstOrDefault()?.VisualInspection3,
                    //Width1 = items.FirstOrDefault()?.Width1,
                    //Width2 = items.FirstOrDefault()?.Width2,
                    //Width3 = items.FirstOrDefault()?.Width3,
                    //GSM1 = items.FirstOrDefault()?.GSM1,
                    //GSM2 = items.FirstOrDefault()?.GSM2,
                    //GSM3 = items.FirstOrDefault()?.GSM3,
                    //TensileCD1 = items.FirstOrDefault()?.TensileCD1,
                    //TensileCD2 = items.FirstOrDefault()?.TensileCD2,
                    //TensileCD3 = items.FirstOrDefault()?.TensileCD3,
                    //TensileMD1 = items.FirstOrDefault()?.TensileMD1,
                    //TensileMD2 = items.FirstOrDefault()?.TensileMD2,
                    //TensileMD3 = items.FirstOrDefault()?.TensileMD3,
                    //Remarks1 = items.FirstOrDefault()?.Remarks1,
                    //Remarks2 = items.FirstOrDefault()?.Remarks2,
                    //Remarks3 = items.FirstOrDefault()?.Remarks3,
                    //Remarks4 = items.FirstOrDefault()?.Remarks4,
                    //Remarks5 = items.FirstOrDefault()?.Remarks5,
                    //Remarks6 = items.FirstOrDefault()?.Remarks6,
                    //Remarks7 = items.FirstOrDefault()?.Remarks7,
                    //Remarks8 = items.FirstOrDefault()?.Remarks8,
                    //Remarks9 = items.FirstOrDefault()?.Remarks9,
                    //Remarks10 = items.FirstOrDefault()?.Remarks10,
                    //Remarks11 = items.FirstOrDefault()?.Remarks11,
                    //Remarks12 = items.FirstOrDefault()?.Remarks12
                };
            }


            // If no valid voucherno, return the view with a default state (qc is null)
            return qc;
        }

        public Items GetEditMaterialDetailsSample(string transactionno)
        {

            string query = $@"SELECT DISTINCT QPT.item_code,
    QPT.transaction_no AS QC_NO,
    QPT.CREATED_DATE AS QC_DATE,
    QPT.CUSTOMER_INVOICE_NO AS invoice_no,
    QPT.VENDOR_NAME AS SUPPLIER_CODE,
    TO_NUMBER(NVL(QPT.QUANTITY, 0)) AS quantity,
    iims.item_edesc,
    qc_data.Thickness0,
    qc_data.Thickness1,
    qc_data.Thickness2,
    qc_data.Thickness3,
    qc_data.RollDiameter0,
    qc_data.RollDiameter1,
    qc_data.RollDiameter2,
    qc_data.RollDiameter3,
    qc_data.PH0,
    qc_data.PH1,
    qc_data.PH2,
    qc_data.PH3,
    qc_data.UnpleasantSmell0,
    qc_data.UnpleasantSmell1,
    qc_data.UnpleasantSmell2,
    qc_data.UnpleasantSmell3,
    qc_data.DustDirt0,
    qc_data.DustDirt1,
    qc_data.DustDirt2,
    qc_data.DustDirt3,
    qc_data.DamagingMaterial0,
    qc_data.DamagingMaterial1,
    qc_data.DamagingMaterial2,
    qc_data.DamagingMaterial3,
    qc_data.CoreDamaging0,
    qc_data.CoreDamaging1,
    qc_data.CoreDamaging2,
    qc_data.CoreDamaging3,
    qc_data.Width0,
    qc_data.Width1,
    qc_data.Width2,
    qc_data.Width3,
    qc_data.GSM0,
    qc_data.GSM1,
    qc_data.GSM2,
    qc_data.GSM3,
    qc_data.TensileCD0,
    qc_data.TensileCD1,
    qc_data.TensileCD2,
    qc_data.TensileCD3,
    qc_data.TensileMD0,
    qc_data.TensileMD1,
    qc_data.TensileMD2,
    qc_data.TensileMD3,
    qc_data.VisualInspection0,
    qc_data.VisualInspection1,
    qc_data.VisualInspection2,
    qc_data.VisualInspection3,
    qc_data.Remarks1,
    qc_data.Remarks2,
    qc_data.Remarks3,
    qc_data.Remarks4,
    qc_data.Remarks5,
    qc_data.Remarks6,
    qc_data.Remarks7,
    qc_data.Remarks8,
    qc_data.Remarks9,
    qc_data.Remarks10,
    qc_data.Remarks11,
    qc_data.Remarks12,
    qc_data.PH,
    qc_data.UNPLEASANT_SMELL_ODOUR,
    qc_data.DUST_DIRT,
    qc_data.DAMAGING_MATERIAL,
    qc_data.CORE_DAMAGING,
    qc_data.SIZE_WIDTH,
    qc_data.GSM,
    qc_data.TENSILE_CD,
    qc_data.TENSILE_MD,
   qc_data.VISUAL_INSPECTION ,
    QPT.item_code,
    QPT.Created_By AS CreatedBy,
    QPT.MANUAL_NO,
    QPT.REMARKS,
    PPm.PRODUCT_ID AS MATERIAL_CODE
FROM QC_PARAMETER_TRANSACTION QPT
LEFT JOIN IP_PRODUCT_ITEM_MASTER_SETUP iims ON iims.item_code = QPT.item_code
LEFT JOIN IP_ITEM_SPEC_SETUP iiss ON iiss.item_code = QPT.item_code
LEFT JOIN PRODUCT_PARAM_MAP ppm ON ppm.inspection_no = QPT.TRANSACTION_NO
LEFT JOIN (
    SELECT 
        IP_ITEM_QC_SETUP_NO,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thickness0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thickness,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN ROLL1 END) AS Thickness1,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL2 END) AS Thickness2,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL3 END) AS Thickness3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN SPECIFICATION END) AS RollDiameter0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL1 END) AS RollDiameter1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL2 END) AS RollDiameter2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL3 END) AS RollDiameter3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS PH0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS PH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL1 END) AS PH1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL2 END) AS PH2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL3 END) AS PH3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS UnpleasantSmell0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS UNPLEASANT_SMELL_ODOUR,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL1 END) AS UnpleasantSmell1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL2 END) AS UnpleasantSmell2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL3 END) AS UnpleasantSmell3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS DustDirt0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS DUST_DIRT,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL1 END) AS DustDirt1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL2 END) AS DustDirt2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL3 END) AS DustDirt3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS DamagingMaterial0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS DAMAGING_MATERIAL,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL1 END) AS DamagingMaterial1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL2 END) AS DamagingMaterial2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL3 END) AS DamagingMaterial3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS CoreDamaging0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS CORE_DAMAGING,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL1 END) AS CoreDamaging1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL2 END) AS CoreDamaging2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL3 END) AS CoreDamaging3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN SPECIFICATION END) AS Width0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN TO_NUMBER(SPECIFICATION) END) AS SIZE_WIDTH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL1 END) AS Width1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL2 END) AS Width2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL3 END) AS Width3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS GSM0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS GSM,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL1 END) AS GSM1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL2 END) AS GSM2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL3 END) AS GSM3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS TensileCD0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS TENSILE_CD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL1 END) AS TensileCD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL2 END) AS TensileCD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL3 END) AS TensileCD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS TensileMD0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS TENSILE_MD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL1 END) AS TensileMD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL2 END) AS TensileMD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL3 END) AS TensileMD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS VisualInspection0,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS VISUAL_INSPECTION,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL1 END) AS VisualInspection1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL2 END) AS VisualInspection2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL3 END) AS VisualInspection3,      
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN Remarks END) AS Remarks1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('RollDiameter') THEN Remarks END) AS Remarks2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('PH') THEN Remarks END) AS Remarks3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN Remarks END) AS Remarks4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN Remarks END) AS Remarks5,     
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN Remarks END) AS Remarks6,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN Remarks END) AS Remarks7,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN Remarks END) AS Remarks8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN Remarks END) AS Remarks9,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN Remarks END) AS Remarks10,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN Remarks END) AS Remarks11,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN Remarks END) AS Remarks12
        
    FROM ip_item_qc_setup
    GROUP BY IP_ITEM_QC_SETUP_NO
) qc_data ON qc_data.IP_ITEM_QC_SETUP_NO= QPT.transaction_no

WHERE QPT.transaction_no ='{transactionno}'";

            // Execute the query and get the result
            List<Items> items = _dbContext.SqlQuery<Items>(query).ToList();
            Items qc = null;
            // Ensure there are items returned before proceeding
            if (items.Any())
            {
                qc = new Items
                {
                    TRANSACTION_NO = transactionno,
                    QC_NO = items.FirstOrDefault()?.QC_NO,
                    QC_DATE = items.FirstOrDefault()?.QC_DATE,
                    item_code = items.FirstOrDefault()?.item_code,
                    MATERIAL_CODE = items.FirstOrDefault()?.MATERIAL_CODE,
                    SUPPLIER_CODE = items.FirstOrDefault()?.SUPPLIER_CODE,
                    Voucher_No = items.FirstOrDefault()?.Voucher_No,
                    supplier_edesc = items.FirstOrDefault()?.supplier_edesc,
                    item_edesc = items.FirstOrDefault()?.item_edesc,
                    RECEIPT_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    to_location_code = items.FirstOrDefault()?.to_location_code,
                    invoice_no = items.FirstOrDefault()?.invoice_no,
                    GRN_NO = items.FirstOrDefault()?.GRN_NO,
                    GRN_DATE = items.FirstOrDefault()?.GRN_DATE,
                    MANUAL_NO = items.FirstOrDefault()?.MANUAL_NO,
                    quantity = Convert.ToDecimal(items.FirstOrDefault()?.quantity),
                    REMARKS = items.FirstOrDefault()?.REMARKS,
                    thickness = items.FirstOrDefault()?.thickness,
                    RollDiameter = items.FirstOrDefault()?.RollDiameter,
                    PH = items.FirstOrDefault()?.PH,
                    UNPLEASANT_SMELL_ODOUR = items.FirstOrDefault()?.UNPLEASANT_SMELL_ODOUR,
                    Dust_Dirt = items.FirstOrDefault()?.Dust_Dirt,
                    Damaging_Material = items.FirstOrDefault()?.Damaging_Material,
                    Core_Damaging = items.FirstOrDefault()?.Core_Damaging,
                    SIZE_WIDTH = items.FirstOrDefault()?.SIZE_WIDTH,
                    GSM = items.FirstOrDefault()?.GSM,
                    Tensile_CD = items.FirstOrDefault()?.Tensile_CD,
                    Tensile_MD = items.FirstOrDefault()?.Tensile_MD,
                    Visual_Inspection = items.FirstOrDefault()?.Visual_Inspection,
                    Thickness0 = items.FirstOrDefault()?.Thickness0,
                    Thickness1 = items.FirstOrDefault()?.Thickness1,
                    Thickness2 = items.FirstOrDefault()?.Thickness2,
                    Thickness3 = items.FirstOrDefault()?.Thickness3,
                    RollDiameter0 = items.FirstOrDefault()?.RollDiameter0,
                    RollDiameter1 = items.FirstOrDefault()?.RollDiameter1,
                    RollDiameter2 = items.FirstOrDefault()?.RollDiameter2,
                    RollDiameter3 = items.FirstOrDefault()?.RollDiameter3,
                    PH0 = items.FirstOrDefault()?.PH0,
                    PH1 = items.FirstOrDefault()?.PH1,
                    PH2 = items.FirstOrDefault()?.PH2,
                    PH3 = items.FirstOrDefault()?.PH3,
                    UnpleasantSmell0 = items.FirstOrDefault()?.UnpleasantSmell0,
                    UnpleasantSmell1 = items.FirstOrDefault()?.UnpleasantSmell1,
                    UnpleasantSmell2 = items.FirstOrDefault()?.UnpleasantSmell2,
                    UnpleasantSmell3 = items.FirstOrDefault()?.UnpleasantSmell3,
                    DustDirt0 = items.FirstOrDefault()?.DustDirt0,
                    DustDirt1 = items.FirstOrDefault()?.DustDirt1,
                    DustDirt2 = items.FirstOrDefault()?.DustDirt2,
                    DustDirt3 = items.FirstOrDefault()?.DustDirt3,
                    DamagingMaterial0 = items.FirstOrDefault()?.DamagingMaterial0,
                    DamagingMaterial1 = items.FirstOrDefault()?.DamagingMaterial1,
                    DamagingMaterial2 = items.FirstOrDefault()?.DamagingMaterial2,
                    DamagingMaterial3 = items.FirstOrDefault()?.DamagingMaterial3,
                    CoreDamaging0 = items.FirstOrDefault()?.CoreDamaging0,
                    CoreDamaging1 = items.FirstOrDefault()?.CoreDamaging1,
                    CoreDamaging2 = items.FirstOrDefault()?.CoreDamaging2,
                    CoreDamaging3 = items.FirstOrDefault()?.CoreDamaging3,
                    VisualInspection0 = items.FirstOrDefault()?.VisualInspection0,
                    VisualInspection1 = items.FirstOrDefault()?.VisualInspection1,
                    VisualInspection2 = items.FirstOrDefault()?.VisualInspection2,
                    VisualInspection3 = items.FirstOrDefault()?.VisualInspection3,
                    Width0 = items.FirstOrDefault()?.Width0,
                    Width1 = items.FirstOrDefault()?.Width1,
                    Width2 = items.FirstOrDefault()?.Width2,
                    Width3 = items.FirstOrDefault()?.Width3,
                    GSM0 = items.FirstOrDefault()?.GSM0,
                    GSM1 = items.FirstOrDefault()?.GSM1,
                    GSM2 = items.FirstOrDefault()?.GSM2,
                    GSM3 = items.FirstOrDefault()?.GSM3,
                    TensileCD0 = items.FirstOrDefault()?.TensileCD0,
                    TensileCD1 = items.FirstOrDefault()?.TensileCD1,
                    TensileCD2 = items.FirstOrDefault()?.TensileCD2,
                    TensileCD3 = items.FirstOrDefault()?.TensileCD3,
                    TensileMD0 = items.FirstOrDefault()?.TensileMD0,
                    TensileMD1 = items.FirstOrDefault()?.TensileMD1,
                    TensileMD2 = items.FirstOrDefault()?.TensileMD2,
                    TensileMD3 = items.FirstOrDefault()?.TensileMD3,
                    Remarks1 = items.FirstOrDefault()?.Remarks1,
                    Remarks2 = items.FirstOrDefault()?.Remarks2,
                    Remarks3 = items.FirstOrDefault()?.Remarks3,
                    Remarks4 = items.FirstOrDefault()?.Remarks4,
                    Remarks5 = items.FirstOrDefault()?.Remarks5,
                    Remarks6 = items.FirstOrDefault()?.Remarks6,
                    Remarks7 = items.FirstOrDefault()?.Remarks7,
                    Remarks8 = items.FirstOrDefault()?.Remarks8,
                    Remarks9 = items.FirstOrDefault()?.Remarks9,
                    Remarks10 = items.FirstOrDefault()?.Remarks10,
                    Remarks11 = items.FirstOrDefault()?.Remarks11,
                    Remarks12 = items.FirstOrDefault()?.Remarks12
                };
            }
            // If no valid voucherno, return the view with a default state (qc is null)
            return qc;
        }

        public IncomingMaterial GetIncomingMaterialSampleDetailReport(string transactionno)
        {
            string query = $@"SELECT DISTINCT QPT.item_code,
    QPT.transaction_no AS QC_NO,
    QPT.CREATED_DATE AS QC_DATE,
    QPT.reference_no AS Voucher_No,
    QPT.MANUAL_NO,
  (SELECT SUPPLIER_EDESC FROM ip_supplier_setup WHERE supplier_code = QPT.VENDOR_NAME AND ROWNUM = 1) AS supplier_edesc,
    iims.item_edesc,
   QPT.GRN_DATE AS RECEIPT_DATE,
   QPT.GRN_NO AS GRN_NO,
    TO_NUMBER(NVL(QPT.QUANTITY, 0)) AS quantity,
     qc_data.Thickness AS thickness,
    qc_data.Thickness1,
    qc_data.Thickness2,
    qc_data.Thickness3,
    qc_data.RollDiameter1,
    qc_data.RollDiameter2,
    qc_data.RollDiameter3,
    qc_data.PH1,
    qc_data.PH2,
    qc_data.PH3,
    qc_data.UnpleasantSmell1,
    qc_data.UnpleasantSmell2,
    qc_data.UnpleasantSmell3,
    qc_data.DustDirt1,
    qc_data.DustDirt2,
    qc_data.DustDirt3,
    qc_data.DamagingMaterial1,
    qc_data.DamagingMaterial2,
    qc_data.DamagingMaterial3,
    qc_data.CoreDamaging1,
    qc_data.CoreDamaging2,
    qc_data.CoreDamaging3,
    qc_data.Width1,
    qc_data.Width2,
    qc_data.Width3,
    qc_data.GSM1,
    qc_data.GSM2,
    qc_data.GSM3,
    qc_data.TensileCD1,
    qc_data.TensileCD2,
    qc_data.TensileCD3,
    qc_data.TensileMD1,
    qc_data.TensileMD2,
    qc_data.TensileMD3,
    qc_data.VisualInspection1,
    qc_data.VisualInspection2,
    qc_data.VisualInspection3,
    qc_data.Remarks1,
    qc_data.Remarks2,
    qc_data.Remarks3,
    qc_data.Remarks4,
    qc_data.Remarks5,
    qc_data.Remarks6,
    qc_data.Remarks7,
    qc_data.Remarks8,
    qc_data.Remarks9,
    qc_data.Remarks10,
    qc_data.Remarks11,
    qc_data.Remarks12,
   qc_data.ROLLDIAMETER,
    qc_data.PH,
    qc_data.UNPLEASANT_SMELL_ODOUR,
    qc_data.DUST_DIRT,
    qc_data.DAMAGING_MATERIAL,
    qc_data.CORE_DAMAGING,
    qc_data.SIZE_WIDTH,
    qc_data.GSM,
    qc_data.TENSILE_CD,
    qc_data.TENSILE_MD,
   qc_data.VISUAL_INSPECTION ,
    QPT.Created_By AS CreatedBy,
     QPT.CUSTOMER_INVOICE_NO AS invoice_no,
    QPT.MANUAL_NO,
    QPT.Remarks
FROM ip_purchase_mrr ipm
LEFT JOIN ip_supplier_setup iss ON iss.supplier_code = ipm.supplier_code
LEFT JOIN ip_product_item_master_setup iims ON iims.item_code = ipm.item_code
LEFT JOIN IP_ITEM_SPEC_SETUP iiss ON iiss.item_code = ipm.item_code
LEFT JOIN PRODUCT_PARAM_MAP PPM ON PPM.PRODUCT_ID = ipm.item_code
LEFT JOIN QC_PARAMETER_TRANSACTION QPT ON QPT.transaction_no = PPM.inspection_no
LEFT JOIN (
    SELECT 
        IP_ITEM_QC_SETUP_NO,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN SPECIFICATION END) AS Thickness,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN ROLL1 END) AS Thickness1,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL2 END) AS Thickness2,
        MAX(CASE WHEN LOWER(qc_edesc) =LOWER('Thickness') THEN ROLL3 END) AS Thickness3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN SPECIFICATION END) AS RollDiameter,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL1 END) AS RollDiameter1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL2 END) AS RollDiameter2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('rolldiameter') THEN ROLL3 END) AS RollDiameter3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN SPECIFICATION END) AS PH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL1 END) AS PH1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL2 END) AS PH2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('ph') THEN ROLL3 END) AS PH3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN SPECIFICATION END) AS UNPLEASANT_SMELL_ODOUR,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL1 END) AS UnpleasantSmell1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL2 END) AS UnpleasantSmell2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN ROLL3 END) AS UnpleasantSmell3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN SPECIFICATION END) AS DUST_DIRT,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL1 END) AS DustDirt1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL2 END) AS DustDirt2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN ROLL3 END) AS DustDirt3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN SPECIFICATION END) AS DAMAGING_MATERIAL,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL1 END) AS DamagingMaterial1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL2 END) AS DamagingMaterial2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN ROLL3 END) AS DamagingMaterial3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN SPECIFICATION END) AS CORE_DAMAGING,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL1 END) AS CoreDamaging1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL2 END) AS CoreDamaging2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN ROLL3 END) AS CoreDamaging3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN TO_NUMBER(SPECIFICATION) END) AS SIZE_WIDTH,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL1 END) AS Width1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL2 END) AS Width2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN ROLL3 END) AS Width3,
         MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN SPECIFICATION END) AS GSM,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL1 END) AS GSM1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL2 END) AS GSM2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN ROLL3 END) AS GSM3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN SPECIFICATION END) AS TENSILE_CD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL1 END) AS TensileCD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL2 END) AS TensileCD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN ROLL3 END) AS TensileCD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN SPECIFICATION END) AS TENSILE_MD,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL1 END) AS TensileMD1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL2 END) AS TensileMD2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN ROLL3 END) AS TensileMD3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN SPECIFICATION END) AS VISUAL_INSPECTION,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL1 END) AS VisualInspection1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL2 END) AS VisualInspection2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN ROLL3 END) AS VisualInspection3,      
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Thickness') THEN Remarks END) AS Remarks1,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('RollDiameter') THEN Remarks END) AS Remarks2,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('PH') THEN Remarks END) AS Remarks3,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('UnpleasantSmell') THEN Remarks END) AS Remarks4,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DustDirt') THEN Remarks END) AS Remarks5,     
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('DamagingMaterial') THEN Remarks END) AS Remarks6,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('CoreDamaging') THEN Remarks END) AS Remarks7,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('Width') THEN Remarks END) AS Remarks8,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('GSM') THEN Remarks END) AS Remarks9,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileCD') THEN Remarks END) AS Remarks10,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('TensileMD') THEN Remarks END) AS Remarks11,
        MAX(CASE WHEN LOWER(qc_edesc) = LOWER('VisualInspection') THEN Remarks END) AS Remarks12
        
    FROM ip_item_qc_setup
    GROUP BY IP_ITEM_QC_SETUP_NO
) qc_data ON qc_data.IP_ITEM_QC_SETUP_NO= QPT.transaction_no

WHERE QPT.transaction_no ='{transactionno}'";

            // Execute the query and get the result
            List<IncomingMaterial> items = _dbContext.SqlQuery<IncomingMaterial>(query).ToList();
            IncomingMaterial qc = null;
            // Ensure there are items returned before proceeding
            if (items.Any())
            {
                qc = new IncomingMaterial
                {
                    TRANSACTION_NO = transactionno,
                    QC_NO = items.FirstOrDefault()?.QC_NO,
                    QC_DATE = items.FirstOrDefault()?.QC_DATE,
                    item_code = items.FirstOrDefault()?.item_code,
                    Voucher_No = items.FirstOrDefault()?.Voucher_No,
                    supplier_edesc = items.FirstOrDefault()?.supplier_edesc,
                    item_edesc = items.FirstOrDefault()?.item_edesc,
                    RECEIPT_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    to_location_code = items.FirstOrDefault()?.to_location_code,
                    invoice_no = items.FirstOrDefault()?.invoice_no,
                    GRN_NO = items.FirstOrDefault()?.GRN_NO,
                    GRN_DATE = items.FirstOrDefault()?.RECEIPT_DATE,
                    MANUAL_NO = items.FirstOrDefault()?.MANUAL_NO,
                    quantity = Convert.ToDecimal(items.FirstOrDefault()?.quantity),
                    REMARKS = items.FirstOrDefault()?.REMARKS,
                    thickness = items.FirstOrDefault()?.thickness,
                    RollDiameter = items.FirstOrDefault()?.RollDiameter,
                    PH = items.FirstOrDefault()?.PH,
                    UNPLEASANT_SMELL_ODOUR = items.FirstOrDefault()?.UNPLEASANT_SMELL_ODOUR,
                    Dust_Dirt = items.FirstOrDefault()?.Dust_Dirt,
                    Damaging_Material = items.FirstOrDefault()?.Damaging_Material,
                    Core_Damaging = items.FirstOrDefault()?.Core_Damaging,
                    SIZE_WIDTH = items.FirstOrDefault()?.SIZE_WIDTH,
                    GSM = items.FirstOrDefault()?.GSM,
                    Tensile_CD = items.FirstOrDefault()?.Tensile_CD,
                    Tensile_MD = items.FirstOrDefault()?.Tensile_MD,
                    Visual_Inspection = items.FirstOrDefault()?.Visual_Inspection,
                    Thickness1 = items.FirstOrDefault()?.Thickness1,
                    Thickness2 = items.FirstOrDefault()?.Thickness2,
                    Thickness3 = items.FirstOrDefault()?.Thickness3,
                    RollDiameter1 = items.FirstOrDefault()?.RollDiameter1,
                    RollDiameter2 = items.FirstOrDefault()?.RollDiameter2,
                    RollDiameter3 = items.FirstOrDefault()?.RollDiameter3,
                    PH1 = items.FirstOrDefault()?.PH1,
                    PH2 = items.FirstOrDefault()?.PH2,
                    PH3 = items.FirstOrDefault()?.PH3,
                    UnpleasantSmell1 = items.FirstOrDefault()?.UnpleasantSmell1,
                    UnpleasantSmell2 = items.FirstOrDefault()?.UnpleasantSmell2,
                    UnpleasantSmell3 = items.FirstOrDefault()?.UnpleasantSmell3,
                    DustDirt1 = items.FirstOrDefault()?.DustDirt1,
                    DustDirt2 = items.FirstOrDefault()?.DustDirt2,
                    DustDirt3 = items.FirstOrDefault()?.DustDirt3,
                    DamagingMaterial1 = items.FirstOrDefault()?.DamagingMaterial1,
                    DamagingMaterial2 = items.FirstOrDefault()?.DamagingMaterial2,
                    DamagingMaterial3 = items.FirstOrDefault()?.DamagingMaterial3,
                    CoreDamaging1 = items.FirstOrDefault()?.CoreDamaging1,
                    CoreDamaging2 = items.FirstOrDefault()?.CoreDamaging2,
                    CoreDamaging3 = items.FirstOrDefault()?.CoreDamaging3,
                    VisualInspection1 = items.FirstOrDefault()?.VisualInspection1,
                    VisualInspection2 = items.FirstOrDefault()?.VisualInspection2,
                    VisualInspection3 = items.FirstOrDefault()?.VisualInspection3,
                    Width1 = items.FirstOrDefault()?.Width1,
                    Width2 = items.FirstOrDefault()?.Width2,
                    Width3 = items.FirstOrDefault()?.Width3,
                    GSM1 = items.FirstOrDefault()?.GSM1,
                    GSM2 = items.FirstOrDefault()?.GSM2,
                    GSM3 = items.FirstOrDefault()?.GSM3,
                    TensileCD1 = items.FirstOrDefault()?.TensileCD1,
                    TensileCD2 = items.FirstOrDefault()?.TensileCD2,
                    TensileCD3 = items.FirstOrDefault()?.TensileCD3,
                    TensileMD1 = items.FirstOrDefault()?.TensileMD1,
                    TensileMD2 = items.FirstOrDefault()?.TensileMD2,
                    TensileMD3 = items.FirstOrDefault()?.TensileMD3,
                    Remarks1 = items.FirstOrDefault()?.Remarks1,
                    Remarks2 = items.FirstOrDefault()?.Remarks2,
                    Remarks3 = items.FirstOrDefault()?.Remarks3,
                    Remarks4 = items.FirstOrDefault()?.Remarks4,
                    Remarks5 = items.FirstOrDefault()?.Remarks5,
                    Remarks6 = items.FirstOrDefault()?.Remarks6,
                    Remarks7 = items.FirstOrDefault()?.Remarks7,
                    Remarks8 = items.FirstOrDefault()?.Remarks8,
                    Remarks9 = items.FirstOrDefault()?.Remarks9,
                    Remarks10 = items.FirstOrDefault()?.Remarks10,
                    Remarks11 = items.FirstOrDefault()?.Remarks11,
                    Remarks12 = items.FirstOrDefault()?.Remarks12
                };
            }


            // If no valid voucherno, return the view with a default state (qc is null)
            return qc;
        }

        #region RAW MATERIAL
        public List<FormDetailSetup> GetRawMaterialDetails(string transactionno)
        {
            
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'RM'";
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
                     WHERE  FDS.MASTER_CHILD_FLAG = 'C' AND FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' order by FDS.SERIAL_NO";
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }
        public List<RawMaterialTree> GetMaterialListsGroupWise()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<RawMaterialTree> tableList = new List<RawMaterialTree>();
                string query = $@"SELECT master_item_code as masterItemCode, INITCAP(item_edesc) as itemName, pre_item_code as preItemCode,item_code as itemCode FROM IP_PRODUCT_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' 
                                AND COMPANY_CODE='01' AND group_sku_flag ='G' and category_CODE ='RM'
                                ORDER BY LENGTH(PRE_ITEM_CODE),PRE_ITEM_CODE, MASTER_ITEM_CODE";
                tableList = this._dbContext.SqlQuery<RawMaterialTree>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }


        public List<RawMaterialModels> GetMaterialListsByItemCode(string itemcode, string itemMasterCode, string searchText)
        {
            try
            {
                var rslt = new List<RawMaterialModels>();

                if (String.IsNullOrEmpty(searchText))
                {
                    if (itemMasterCode == "undefined")
                    {
                        itemMasterCode = "";
                    }
                    string query = $@" SELECT DISTINCT
                                INITCAP(A.ITEM_EDESC) AS ITEM_EDESC,
                                INITCAP(A.ITEM_NDESC)AS ITEM_NDESC,
                                A.ITEM_CODE AS ITEM_CODE,
                                A.MASTER_ITEM_CODE AS MASTER_ITEM_CODE,
                                A.PRE_ITEM_CODE AS PRE_ITEM_CODE,
                                A.GROUP_SKU_FLAG AS GROUP_SKU_FLAG,
                                TO_CHAR(A.CREATED_DATE,'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE,
                                A.CREATED_BY AS CREATED_BY,
                                B.MU_EDESC AS MU_EDESC,
                                TO_CHAR(ROUND(TO_CHAR(A.PURCHASE_PRICE),2)) AS PURCHASE_PRICE,
                                C.CATEGORY_EDESC AS CATEGORY_EDESC
                                FROM IP_PRODUCT_ITEM_MASTER_SETUP A, IP_MU_CODE B, IP_CATEGORY_CODE C
                                WHERE A.DELETED_FLAG = 'N'
                                AND A.INDEX_MU_CODE=B.MU_CODE
                                AND A.CATEGORY_CODE(+)=C.CATEGORY_CODE 
                                AND A.GROUP_SKU_FLAG = 'I'
                                AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                                AND A.MASTER_ITEM_CODE  like '{itemMasterCode}%'
                                ORDER BY ITEM_CODE ASC";
                    rslt = _dbContext.SqlQuery<RawMaterialModels>(query).ToList();
                    //this._cacheManager.Set($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}", rslt, 20);
                }
                else
                {
                    string query = $@" SELECT DISTINCT
                                INITCAP(A.ITEM_EDESC) AS ITEM_EDESC,
                                INITCAP(A.ITEM_NDESC)AS ITEM_NDESC,
                                A.ITEM_CODE AS ITEM_CODE,
                                A.MASTER_ITEM_CODE AS MASTER_ITEM_CODE,
                                A.PRE_ITEM_CODE AS PRE_ITEM_CODE,
                                A.GROUP_SKU_FLAG AS GROUP_SKU_FLAG,
                                TO_CHAR(A.CREATED_DATE,'DD/MM/YYYY hh24:mi:ss') AS CREATED_DATE,
                                A.CREATED_BY AS CREATED_BY,
                                B.MU_EDESC AS MU_EDESC,
                                TO_CHAR(ROUND(TO_CHAR(A.PURCHASE_PRICE),2)) AS PURCHASE_PRICE,
                                C.CATEGORY_EDESC AS CATEGORY_EDESC
                                FROM IP_PRODUCT_ITEM_MASTER_SETUP A, IP_MU_CODE B, IP_CATEGORY_CODE C
                                WHERE A.DELETED_FLAG = 'N'
                                AND A.INDEX_MU_CODE=B.MU_CODE
                                AND A.CATEGORY_CODE(+)=C.CATEGORY_CODE 
                                AND A.GROUP_SKU_FLAG = 'I'
                                AND A.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'
                               AND A.MASTER_ITEM_CODE  like '{itemMasterCode}%'
                                ORDER BY ITEM_CODE ASC";
                    rslt = _dbContext.SqlQuery<RawMaterialModels>(query).ToList();
                    //this._cacheManager.Set($"GetProductListByItemCode_{_workContext.CurrentUserinformation.User_id}_{_workContext.CurrentUserinformation.company_code}_{_workContext.CurrentUserinformation.branch_code}_{itemcode}_{itemMasterCode}_{searchText}", rslt, 20);
                }

               
                return rslt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<MuCodeModel> GetMuCode()
        {
            try
            {
                string query = $@"SELECT MU_CODE,MU_EDESC,MU_NDESC,REMARKS,
                                    COMPANY_CODE,CREATED_BY,CREATED_DATE,
                                    DELETED_FLAG,SYN_ROWID,MODIFY_DATE,
                                    MODIFY_BY FROM IP_MU_CODE
                                    WHERE DELETED_FLAG='N' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}'";
                var list = this._dbContext.SqlQuery<MuCodeModel>(query).ToList();
                return list;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public List<Items> GetPending_RawMaterialsByItemId(string ItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string query = $@"select DISTINCT IPM.mrr_no as Voucher_No,IPM.item_code
,IPM.MRR_DATE AS QC_DATE,BS_DATE(IPM.MRR_DATE) MITI
,IPM.SUPPLIER_INV_NO  AS INVOICE_NO
,BT.transaction_no
,IPM.item_code
,  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE 
AND COMPANY_CODE = IPM.COMPANY_CODE )   SUPPLIER_EDESC
, (SELECT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE 
AND COMPANY_CODE = IPM.COMPANY_CODE )   SUPPLIER_CODE
from BATCH_TRANSACTION BT 
LEFT JOIN ip_purchase_mrr IPM ON IPM.ITEM_CODE = BT.ITEM_CODE
where 
IPM.mrr_no  not in ( select reference_form_code from reference_detail)
and 
IPM.item_code ='{ItemCode}'
and BT.transaction_no  not  IN (SELECT BATCH_NO FROM IP_ITEM_QC_TESTING where item_code ='{ItemCode}')";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<RawMaterialDetails> GetRawMaterialDataByItemCode(string productcode)
        {
            try
            {
                var productdata = new List<RawMaterialDetails>();
                if (productcode != "")
                {
                    string query = $@"SELECT distinct  IT.ITEM_CODE AS ItemCode,
                                   IT.ITEM_EDESC AS ItemDescription,
                                   IT.INDEX_MU_CODE AS ItemUnit,
                                  IT.MULTI_MU_CODE AS MultiItemUnit,
                                  IC.CATEGORY_EDESC  AS Category,
                                   IT.CREATED_BY AS CreatedBy,
                                   TO_CHAR (IT.CREATED_DATE, 'dd-Mon-yyyy') AS CreatedDate,
                                   IISS.GSM,IISS.SIZE_WIDTH,IISS.STRENGTH,IISS.THICKNESS 
                              FROM IP_PRODUCT_ITEM_MASTER_SETUP IT, IP_CATEGORY_CODE IC,IP_ITEM_SPEC_SETUP IISS 
                             WHERE IT.deleted_flag = 'N'
                             AND IT.CATEGORY_CODE= IC.CATEGORY_CODE
                             AND IT.COMPANY_CODE = IC.COMPANY_CODE
                             AND IT.ITEM_CODE = IISS.ITEM_CODE   
                              AND IT.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'
                               AND IT.item_code ='{productcode}'";
                    productdata = this._dbContext.SqlQuery<RawMaterialDetails>(query).ToList();
                    return productdata;
                }
                else
                { return productdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BatchDetails> GetBatchNoByItemCode(string itemCode)
        {
            try
            {
                var itemdata = new List<BatchDetails>();
                if (itemCode != "")
                {
                    string query = $@"SELECT BT.TRANSACTION_NO,BT.ITEM_CODE,BT.REFERENCE_NO,BT.BATCH_NO
,NVL(BT.QUANTITY, 0) - NVL(RD.REFERENCE_ACTUAL_QTY, 0) AS QUANTITY,NVL(BT.QUANTITY, 0) as quan,BT.UNIT_PRICE 
FROM BATCH_TRANSACTION BT LEFT JOIN REFERENCE_DETAIL RD ON RD.REFERENCE_NO = BT.REFERENCE_NO
                                    WHERE 
                                    BT.TRANSACTION_NO not in (select NVL(IIQT.batch_no,0) from IP_ITEM_QC_TESTING IIQT
LEFT JOIN QC_PARAMETER_TRANSACTION QPT ON QPT.TRANSACTION_NO =  IIQT.IP_ITEM_QC_TESTING_NO
WHERE QPT.DELETED_FLAG = 'N') AND
                                    BT.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BT.ITEM_CODE ='{itemCode}'";
                    itemdata = this._dbContext.SqlQuery<BatchDetails>(query).ToList();
                    return itemdata;
                }
                else
                { return itemdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<BatchDetails> GetBatchNoByTransactionNo(string TransactionNo)
        {
            try
            {
                var itemdata = new List<BatchDetails>();
                if (TransactionNo != "")
                {
                    string query = $@"SELECT TRANSACTION_NO,item_code, REFERENCE_NO,BATCH_NO,QUANTITY,UNIT_PRICE from batch_transaction where
                                    COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND
                                    TRANSACTION_NO ='{TransactionNo}'";
                    itemdata = this._dbContext.SqlQuery<BatchDetails>(query).ToList();
                    return itemdata;
                }
                else
                { return itemdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<RawMaterial> GetMaterialDetailByProductType(string productType)
        {
            try
            {
                var itemdata = new List<RawMaterial>();
                if (productType != "")
                {
                    string query = $@"SELECT 
         IIMS.ITEM_EDESC,
         IIMS.item_code AS ITEM_CODE,
        IISS.GSM,IISS.SIZE_WIDTH,iiss.strength_Md,iiss.STRENGTH,IISS.THICKNESS
    FROM  IP_PRODUCT_ITEM_MASTER_SETUP IIMS 
    LEFT JOIN IP_ITEM_SPEC_SETUP IISS 
           ON IIMS.ITEM_CODE = IISS.ITEM_CODE
    WHERE IIMS.product_type =  '{productType}' AND 
    IIMS.CATEGORY_CODE IN ( SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE  WHERE CATEGORY_TYPE = 'RM')";
                    itemdata = this._dbContext.SqlQuery<RawMaterial>(query).ToList();
                    return itemdata;
                }
                else
                { return itemdata; }

            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool InsertDailyRawMaterialData(RawMaterial data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.QC_NO}'";
                    string dispatch_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'RM'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();
                    if (dispatch_no == null)
                    {
                        //string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                        //string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();
                        int i = 1;

                        string insertRawMaterialTranQuery = string.Format(@"INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO
                                    ,REFERENCE_NO,ITEM_CODE,SERIAL_NO,QC_CODE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                                    ,CREATED_DATE,DELETED_FLAG,BATCH_NO,MANUAL_NO,REMARKS)
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9:dd-MMM-yyyy}', 'DD-MON-YYYY'),'{10}','{11}','{12}','{13}')"
                                      , data.QC_NO, "0", data.ITEM_CODE, i, i, form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                                      _workContext.CurrentUserinformation.login_code
                                      , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', data.BATCH_NO, data.MANUAL_NO,data.REMARKS);
                        _dbContext.ExecuteSqlCommand(insertRawMaterialTranQuery);

                        foreach (var raw in data.RawMaterialList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO IP_ITEM_QC_TESTING (IP_ITEM_QC_TESTING_NO,ITEM_CODE, SUPPLIER_CODE, BATCH_NO, GSM, WIDTH, STRENGTH
                        , THICKNESS, ACTUAL_GSM,ACTUAL_WIDTH,ACTUAL_STRENGTH,ACTUAL_THICKNESS
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,STRENGTH_MD,ACTUAL_STRENGTH_MD,ROLL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}','{11}','{12}', TO_DATE('{13}', 'DD-MON-YYYY'),'{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')",
                                        data.QC_NO, raw.ITEM_CODE, raw.SUPPLIER_CODE, raw.BATCH_NO, raw.GSM,
                                        raw.WIDTH, raw.STRENGTH, raw.THICKNESS, raw.ACTUAL_GSM, raw.ACTUAL_WIDTH, raw.ACTUAL_STRENGTH, raw.ACTUAL_THICKNESS, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, i, raw.STRENGTH_MD, raw.ACTUAL_STRENGTH_MD, raw.ROLL_NO, raw.REMARKS);  // change hard code 498

                            _dbContext.ExecuteSqlCommand(insertQuery);
                        }

                    }
                    else
                    {
                        string deleteParameterDetailsQuery = $@"Delete from IP_ITEM_QC_TESTING WHERE IP_ITEM_QC_TESTING_NO = '{data.QC_NO}' ";
                        _dbContext.ExecuteSqlCommand(deleteParameterDetailsQuery);
                        int i = 1;
                        foreach (var raw in data.RawMaterialList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO IP_ITEM_QC_TESTING (IP_ITEM_QC_TESTING_NO,ITEM_CODE, SUPPLIER_CODE, BATCH_NO, GSM, WIDTH, STRENGTH
                        , THICKNESS, ACTUAL_GSM,ACTUAL_WIDTH,ACTUAL_STRENGTH,ACTUAL_THICKNESS
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,STRENGTH_MD,ACTUAL_STRENGTH_MD,ROLL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}','{11}','{12}', TO_DATE('{13}', 'DD-MON-YYYY'),'{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}')",
                                        data.QC_NO, raw.ITEM_CODE, raw.SUPPLIER_CODE, raw.BATCH_NO, raw.GSM,
                                        raw.WIDTH, raw.STRENGTH, raw.THICKNESS, raw.ACTUAL_GSM, raw.ACTUAL_WIDTH, raw.ACTUAL_STRENGTH, raw.ACTUAL_THICKNESS, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, i, raw.STRENGTH_MD, raw.ACTUAL_STRENGTH_MD, raw.ROLL_NO, raw.REMARKS);  // change hard code 498

                            _dbContext.ExecuteSqlCommand(insertQuery);
                        }
                        string updateParamQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           ITEM_CODE = '{data.ITEM_CODE}',MANUAL_NO = '{data.MANUAL_NO}',BATCH_NO = '{data.BATCH_NO}',
                           REMARKS = '{data.REMARKS}',                          
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE TRANSACTION_NO = '{data.QC_NO}' ";
                        _dbContext.ExecuteSqlCommand(updateParamQuery);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public RawMaterial GetEditDailyRawMaterial(string transactionno)
        {
            RawMaterial raw = new RawMaterial();
            String query1 = $@"select TRANSACTION_NO as QC_NO,ITEM_CODE,BATCH_NO,SERIAL_NO,CREATED_DATE,MANUAL_NO,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
                raw = _dbContext.SqlQuery<RawMaterial>(query1).FirstOrDefault();
            //String query2 = $@"select ITEM_CODE,SUPPLIER_CODE,BATCH_NO,GSM,WIDTH,STRENGTH,THICKNESS,ACTUAL_GSM,ACTUAL_WIDTH,ACTUAL_STRENGTH,ACTUAL_THICKNESS,REMARKS from IP_ITEM_QC_TESTING  WHERE SERIAL_NO= '{items[0].SERIAL_NO}'";
            String query2 = $@" select distinct IIQT.ITEM_CODE,IT.ITEM_EDESC,IIQT.SUPPLIER_CODE,CASE WHEN (SELECT CUSTOMER_EDESC 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE =  IIQT.SUPPLIER_CODE) != null THEN (SELECT CUSTOMER_EDESC 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE =  IIQT.SUPPLIER_CODE )
ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IIQT.SUPPLIER_CODE  ) END  SUPPLIER_EDESC,(select batch_no from batch_transaction where transaction_no=IIQT.BATCH_NO) AS BATCH_NO,IIQT.BATCH_NO AS TRANSACTION_NO,IIQT.GSM,IIQT.WIDTH,IIQT.STRENGTH,IIQT.STRENGTH_MD,IIQT.THICKNESS,IIQT.ACTUAL_GSM
 ,IIQT.ACTUAL_WIDTH,IIQT.ACTUAL_STRENGTH,IIQT.ACTUAL_STRENGTH_MD,IIQT.ROLL_NO
 ,IIQT.ACTUAL_THICKNESS,IIQT.REMARKS from IP_ITEM_QC_TESTING IIQT
 left join IP_PRODUCT_ITEM_MASTER_SETUP IT on IT.item_code = IIQT.item_code  WHERE IIQT.IP_ITEM_QC_TESTING_NO = '{transactionno}'  order by  IIQT.ITEM_CODE asc";
            //List<RawMaterial> items2 = _dbContext.SqlQuery<RawMaterial>(query2).ToList();

            List<RawMaterial> rawMaterials = new List<RawMaterial>();
            rawMaterials = this._dbContext.SqlQuery<RawMaterial>(query2).ToList();
            raw.RawMaterialList = rawMaterials;
            //return Record;

            // If no valid voucherno, return the view with a default state (qc is null)
            return raw;
        }

        public RawMaterial GetDailyRawMaterialReport(string transactionno)
        {
            RawMaterial raw = new RawMaterial();
            String query1 = $@"select TRANSACTION_NO as QC_NO,ITEM_CODE,BATCH_NO,SERIAL_NO,CREATED_DATE,MANUAL_NO,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<RawMaterial>(query1).FirstOrDefault();
            String query2 = $@" select distinct IIQT.ITEM_CODE,IT.ITEM_EDESC,IIQT.SUPPLIER_CODE,CASE WHEN (SELECT CUSTOMER_EDESC 
                                FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE =  IIQT.SUPPLIER_CODE) != null THEN (SELECT CUSTOMER_EDESC 
                                FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE =  IIQT.SUPPLIER_CODE )
                                ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IIQT.SUPPLIER_CODE  ) END  SUPPLIER_EDESC,(select batch_no from batch_transaction where transaction_no=IIQT.BATCH_NO) AS BATCH_NO,IIQT.BATCH_NO AS TRANSACTION_NO,IIQT.GSM,IIQT.WIDTH,IIQT.STRENGTH,IIQT.STRENGTH_MD,IIQT.THICKNESS,IIQT.ACTUAL_GSM
                                 ,IIQT.ACTUAL_WIDTH,IIQT.ACTUAL_STRENGTH,IIQT.ACTUAL_STRENGTH_MD,IIQT.ROLL_NO
                                 ,IIQT.ACTUAL_THICKNESS,IIQT.REMARKS from IP_ITEM_QC_TESTING IIQT
                                 left join IP_PRODUCT_ITEM_MASTER_SETUP IT on IT.item_code = IIQT.item_code  WHERE IIQT.IP_ITEM_QC_TESTING_NO = '{transactionno}'";
            List<RawMaterial> rawMaterials = new List<RawMaterial>();
            rawMaterials = this._dbContext.SqlQuery<RawMaterial>(query2).ToList();
            raw.RawMaterialList = rawMaterials;
            return raw;
        }
        #endregion
    }
}
