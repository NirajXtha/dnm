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
    public class GlobalAgroProductsRepo : IGlobalAgroProductsRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public GlobalAgroProductsRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public GlobalAgroProducts GetGlobalAgroProductLists()
        {
            GlobalAgroProducts entity = new GlobalAgroProducts();
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'GP'";
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
             entity.WEIGHTDETAILSList = this._dbContext.SqlQuery<WEIGHTDETAILS>(Query).ToList();
            entity.UNLOADEDCHHALLIList = this._dbContext.SqlQuery<UNLOADEDCHHALLI>(Query).ToList();
            entity.DAKHILADETAILSList = this._dbContext.SqlQuery<DAKHILADETAILS>(Query).ToList();
            return entity;
        }

        public List<Items> GetGlobalAgroMaterialDetailLists(string VoucherNo)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string query = $@"select distinct iss.SUPPLIER_CODE,iss.supplier_edesc,iims.item_edesc,ipm.mrr_date AS RECEIPT_DATE
,ipm.quantity,ipm.to_location_code
,ipm.item_code,ipm.Created_By as CreatedBy,ipm.SUPPLIER_INV_NO as invoice_no,ipm.MANUAL_NO,ipm.Remarks,st.VEHICLE_NO,
CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT REGD_OFFICE_EADDRESS FROM SA_CUSTOMER_SETUP 
WHERE CUSTOMER_CODE = iss.SUPPLIER_CODE AND COMPANY_CODE = ISS.COMPANY_CODE ) ELSE  (SELECT REGD_OFFICE_EADDRESS FROM IP_SUPPLIER_SETUP 
WHERE SUPPLIER_CODE = iss.SUPPLIER_CODE AND COMPANY_CODE = iss.COMPANY_CODE ) END  ADDRESS   from ip_purchase_mrr ipm
left join ip_supplier_setup iss on iss.supplier_code = ipm.supplier_code
left join ip_item_master_setup iims on iims.item_code = ipm.item_code
left join IP_ITEM_SPEC_SETUP iiss ON  iiss.item_code = ipm.item_code
left join SHIPPING_TRANSACTION st ON st.VOUCHER_NO = ipm.mrr_no
left join IP_GATE_ENTRY IGE on IGE.supplier_code = ipm.supplier_code
where ipm.mrr_no = '{VoucherNo}'
UNION ALL
select distinct iss.SUPPLIER_CODE,iss.supplier_edesc,iims.item_edesc,ige.gate_date AS RECEIPT_DATE
,ige.NET_WT as quantity,ige.LOCATION_CODE as to_location_code
,iged.item_code,iged.Created_By as CreatedBy,ige.BILL_NO as invoice_no,ige.GATE_NO as MANUAL_NO,iged.Remarks,ige.VEHICLE_NAME as VEHICLE_NO,CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT REGD_OFFICE_EADDRESS FROM SA_CUSTOMER_SETUP 
WHERE CUSTOMER_CODE = iss.SUPPLIER_CODE AND COMPANY_CODE = ISS.COMPANY_CODE ) ELSE  (SELECT REGD_OFFICE_EADDRESS FROM IP_SUPPLIER_SETUP 
WHERE SUPPLIER_CODE = iss.SUPPLIER_CODE AND COMPANY_CODE = iss.COMPANY_CODE ) END  ADDRESS  from IP_GATE_ENTRY ige
left join IP_GATE_ENTRY_DETAIL iged on iged.GATE_NO= ige.GATE_NO
left join ip_supplier_setup iss on iss.supplier_code = ige.supplier_code
left join ip_item_master_setup iims on iims.item_code = iged.item_code
left join IP_ITEM_SPEC_SETUP iiss ON  iiss.item_code = iged.item_code
left join SHIPPING_TRANSACTION st ON st.VOUCHER_NO= ige.GATE_NO
where ige.GATE_NO = '{VoucherNo}'";

                //                string query = $@"select distinct iss.SUPPLIER_CODE,iss.supplier_edesc,iims.item_edesc,ipm.mrr_date AS RECEIPT_DATE
                //,ipm.quantity,ipm.to_location_code
                //,ipm.item_code,ipm.Created_By as CreatedBy,ipm.SUPPLIER_INV_NO as invoice_no,ipm.MANUAL_NO,ipm.Remarks,st.VEHICLE_NO,iss.REGD_OFFICE_EADDRESS as ADDRESS  from ip_purchase_mrr ipm
                //left join ip_supplier_setup iss on iss.supplier_code = ipm.supplier_code
                //left join ip_item_master_setup iims on iims.item_code = ipm.item_code
                //left join IP_ITEM_SPEC_SETUP iiss ON  iiss.item_code = ipm.item_code
                //left join SHIPPING_TRANSACTION st ON st.VOUCHER_NO = ipm.mrr_no
                //where ipm.mrr_no = '{VoucherNo}'
                //UNION ALL
                //select distinct iss.SUPPLIER_CODE,iss.supplier_edesc,iims.item_edesc,ige.gate_date AS RECEIPT_DATE
                //,ige.NET_WT as quantity,ige.LOCATION_CODE as to_location_code
                //,iged.item_code,iged.Created_By as CreatedBy,ige.BILL_NO as invoice_no,ige.GATE_NO as MANUAL_NO,iged.Remarks,ige.VEHICLE_NAME as VEHICLE_NO,iss.REGD_OFFICE_EADDRESS as ADDRESS  from IP_GATE_ENTRY ige
                //left join IP_GATE_ENTRY_DETAIL iged on iged.GATE_NO= ige.GATE_NO
                //left join ip_supplier_setup iss on iss.supplier_code = ige.supplier_code
                //left join ip_item_master_setup iims on iims.item_code = iged.item_code
                //left join IP_ITEM_SPEC_SETUP iiss ON  iiss.item_code = iged.item_code
                //left join SHIPPING_TRANSACTION st ON st.VOUCHER_NO= ige.GATE_NO
                //where ige.GATE_NO = '{VoucherNo}'";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Items> GetGateEntryDetailsByItemId(string ItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                //string query = $@"select mrr_no as Voucher_No from ip_purchase_mrr where mrr_no  not in ( select reference_form_code from reference_detail) and item_code ={ItemCode}";
                //                string query = $@"
                //select IGED.GATE_NO as Voucher_No,IGED.item_code
                //,IGED.GATE_DATE AS QC_DATE,BS_DATE(IGED.GATE_DATE) MITI
                //,IGE.BILL_NO  AS INVOICE_NO
                //, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_EDESC 
                //FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
                //ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IGE.SUPPLIER_CODE 
                //AND COMPANY_CODE = IGE.COMPANY_CODE ) END  SUPPLIER_EDESC, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_CODE 
                //FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
                //ELSE  (SELECT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IGE.SUPPLIER_CODE 
                //AND COMPANY_CODE = IGE.COMPANY_CODE ) END  SUPPLIER_CODE
                //from IP_GATE_ENTRY_DETAIL IGED
                //LEFT JOIN IP_GATE_ENTRY  IGE ON IGE.GATE_NO =  IGED.GATE_NO
                //where  
                //IGED.item_code ={ItemCode} 
                //and 
                //IGED.GATE_NO not in ( select REFERENCE_NO from GLOBAL_PRODUCTS_TRANSACTION)";
                string query = $@"
SELECT DISTINCT A.GATE_NO as Voucher_No,C.item_code, A.BILL_NO AS INVOICE_NO, A.RECEIVED_BY, A.VEHICLE_NAME AS VEHICLE_NO, A.IN_TIME, A.OUT_TIME, A.REMARKS, A.GATE_IN_FLAG, A.GATE_IN_BY
, A.WEIGHT_BRIDGE_FLAG, A.WEIGHT_BRIDGE_BY, A.UNLOADING_FLAG, A.UNLOADING_BY, A.WB_OUT_FLAG, A.WB_OUT_BY, A.GATE_OUT_FLAG, A.GATE_OUT_BY
, A.GATE_DATE AS QC_DATE, BS_DATE(A.BILL_DATE) AS MITI, B.LOCATION_EDESC,  A.REFERENCE_NO, A.GROSS_WT, A.TEAR_WT, A.NET_WT
, A.TRANSPORT_NAME,
 CASE WHEN INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP 
WHERE CUSTOMER_CODE = A.SUPPLIER_CODE AND COMPANY_CODE = A.COMPANY_CODE ) ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP 
WHERE SUPPLIER_CODE = A.SUPPLIER_CODE AND COMPANY_CODE = A.COMPANY_CODE ) END  SUPPLIER_EDESC
, CASE WHEN INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_CODE 
FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = A.SUPPLIER_CODE AND COMPANY_CODE = A.COMPANY_CODE )
ELSE  (SELECT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = A.SUPPLIER_CODE 
AND COMPANY_CODE = A.COMPANY_CODE ) END  SUPPLIER_CODE
,D.ITEM_EDESC, C.BILL_QTY  
FROM IP_GATE_ENTRY A, IP_LOCATION_SETUP B, IP_GATE_ENTRY_DETAIL C, IP_ITEM_MASTER_SETUP D WHERE A.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
AND A.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}' AND A.COMPANY_CODE = B.COMPANY_CODE(+) AND A.LOCATION_CODE = B.LOCATION_CODE(+) 
AND A.COMPANY_CODE = C.COMPANY_CODE(+) AND A.GATE_NO = C.GATE_NO(+) AND A.BRANCH_CODE = C.BRANCH_CODE(+) AND C.COMPANY_CODE = D.COMPANY_CODE(+) 
AND C.ITEM_CODE = D.ITEM_CODE(+) 
AND
C.item_code ={ItemCode} 
 ORDER BY A.GATE_NO DESC";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<Items> GetGRNDetailsByItemId(string ItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                //                string query = $@"select distinct IPM.mrr_no as Voucher_No,IPM.item_code
                //,IPM.MRR_DATE AS QC_DATE,BS_DATE(IPM.MRR_DATE) MITI
                //,IPM.SUPPLIER_INV_NO  AS INVOICE_NO
                //, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_EDESC 
                //FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
                //ELSE  (SELECT SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE 
                //AND COMPANY_CODE = IPM.COMPANY_CODE ) END  SUPPLIER_EDESC, CASE WHEN IGE.INWARD_TYPE IN ('Referencial Outward' )THEN (SELECT CUSTOMER_CODE 
                //FROM SA_CUSTOMER_SETUP WHERE CUSTOMER_CODE = IGE.SUPPLIER_CODE AND COMPANY_CODE = IGE.COMPANY_CODE )
                //ELSE  (SELECT SUPPLIER_CODE FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_CODE = IPM.SUPPLIER_CODE 
                //AND COMPANY_CODE = IPM.COMPANY_CODE ) END  SUPPLIER_CODE
                //from ip_purchase_mrr IPM
                //LEFT JOIN IP_GATE_ENTRY_DETAIL IGED ON IGED.item_code = IPM.item_code
                //LEFT JOIN IP_GATE_ENTRY  IGE ON IGE.GATE_NO =  IGED.GATE_NO
                //where 
                //IPM.mrr_no  not in ( select reference_form_code from reference_detail) 
                //and IPM.mrr_no not in ( select REFERENCE_NO from GLOBAL_PRODUCTS_TRANSACTION)
                //and 
                //IPM.item_code ={ItemCode}";
                string query = $@"SELECT A.ROWID, C.SUPPLIER_EDESC,C.REGD_OFFICE_EADDRESS, C.TPIN_VAT_NO, B.MRR_NO as Voucher_No,B.item_code, B.FORM_CODE, B.MRR_DATE AS QC_DATE, BS_DATE(MRR_DATE) MITI
,B.SUPPLIER_INV_NO  AS INVOICE_NO
,  A.TRANSPORTER_CODE, D.TRANSPORTER_EDESC, A.VEHICLE_NO, A.CN_NO, A.FREGHT_AMOUNT, D.SUPPLIER_CODE  
, A.FREIGHT_VAT VAT , A.FREIGHT_TDS TDS, FREIGHT_ADVANCE,  FREIGHT_JV, FREIGHT_FORM_CODE, C.ACC_CODE, C.LINK_SUB_CODE  
FROM SHIPPING_TRANSACTION A, IP_PURCHASE_MRR B, IP_SUPPLIER_SETUP C, TRANSPORTER_SETUP D  
Where a.VOUCHER_NO = b.MRR_NO  
AND A.FORM_CODE = B.FORM_CODE  
AND A.COMPANY_CODE = B.COMPANY_CODE  
AND B.SERIAL_NO = 1  
AND B.SUPPLIER_CODE = C.SUPPLIER_CODE  
AND B.COMPANY_CODE = C.COMPANY_CODE  
AND b.COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}'  
AND A.TRANSPORTER_CODE = D.TRANSPORTER_CODE (+)  
AND A.COMPANY_CODE = D.COMPANY_CODE (+)  
AND A.FREIGHT_JV IS NULL  
AND B.item_code = {ItemCode}
ORDER BY b.FORM_CODE, b.MRR_NO";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertGlobalAgroProductsData(GlobalAgroProducts data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from GLOBAL_PRODUCTS_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.TRANSACTION_NO}'";
                    string transaction_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'GP'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();
                    string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM GLOBAL_PRODUCTS_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                    string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();
                    if (transaction_no == null)
                    {
                        string isPlastic_bagValue = data.ISPLASTIC_BAG == null ? "N" : "Y";
                        string isJute_bagValue = data.ISJUTE_BAG == null ? "N" : "Y";
                        string isPlastic_weightValue = data.ISPLASTIC_WEIGHT == null ? "N" : "Y";
                        string isJute_weightValue = data.ISJUTE_WEIGHT == null ? "N" : "Y";
                        string isUnloadValue = data.ISUNLOAD == null ? "N" : "Y";
                        string isProteinValue = data.ISPROTEIN == null ? "N" : "Y";
                        string isPtValue = data.ISPT == null ? "N" : "Y";
                        string isOutValue = data.ISOUT == null ? "N" : "Y";
                        string insertRawMaterialTranQuery = string.Format(@"INSERT INTO GLOBAL_PRODUCTS_TRANSACTION (TRANSACTION_NO
                                    ,REFERENCE_NO,ITEM_CODE,PARTY_NAME,ADDRESS,PHYSICAL_TEST_RAWMATERIAL,                                   
                                    WEIGHT, MOISTURE,TEMPERATURE,WET,
                                    FUNGUS, DUST,GRADING,SMELL,
                                    COLOR,PIECES,IMMATURITY_OF_GRAINS,OTHER_ITEMS,ROTTEN_HOLED,
                                    DAMAGED,BROKEN,HUSK,OVERTOASTED,
                                    USEABLE,UNUSEABLE,FAT,QUALITY_OF_GOODS,
                                    EXCELLENT,GREAT,GOODS_NORMAL,WAREHOUSE,
                                    SILO,GHAN,PROTEIN,QUALITY_OF_FIREWOOD,
                                    PRODUCT_SIZE,PRODUCT_TYPE,DEDUCT_IN_BAG,DEDUCT_IN_WT,NET_WEIGHT,
                                    UNLOAD_UNIT,SERIAL_NO,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                                    ,CREATED_DATE,DELETED_FLAG,CHECKED_BY,AUTHORISED_BY,ISPLASTIC_BAG,ISJUTE_BAG,ISPLASTIC_WEIGHT,ISJUTE_WEIGHT,VEHICLE_NO,BILL_NO,GATE_OR_GRN_NO,PRODUCT_REMARKS,REMARKS,SLIDER_VALUE,ISUNLOAD,ISPROTEIN,ISPT,ISOUT,DEDUCT_IN_PLASTIC,DEDUCT_IN_JUTE)
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}'
                                    ,'{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}'
                                    ,'{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}'
                                    ,'{27}','{28}','{29}','{30}','{31}','{32}','{33}','{34}','{35}'
                                    ,'{36}','{37}','{38}','{39}','{40}','{41}','{42}','{43}','{44}','{45}'
                                    ,TO_DATE('{46}', 'DD-MON-YYYY'),'{47}','{48}','{49}','{50}','{51}','{52}','{53}','{54}','{55}','{56}','{57}','{58}','{59}','{60}','{61}','{62}','{63}','{64}','{65}')"
                                        , data.TRANSACTION_NO, data.TRANSACTION_NO, data.ITEM_CODE,
                                        data.PARTY_NAME, data.ADDRESS, data.PHYSICAL_TEST_RAWMATERIAL,
                                        data.WEIGHT, data.MOISTURE, data.TEMPERATURE, data.WET,
                                        data.FUNGUS, data.DUST, data.GRADING, data.SMELL,
                                        data.COLOR, data.PIECES, data.IMMATURITY_OF_GRAINS, data.OTHER_ITEMS, data.ROTTEN_HOLED,
                                        data.DAMAGED, data.BROKEN, data.HUSK, data.OVERTOASTED,
                                        data.USEABLE, data.UNUSEABLE, data.FAT, data.QUALITY_OF_GOODS,
                                        data.EXCELLENT, data.GREAT, data.GOODS_NORMAL, data.WAREHOUSE,
                                        data.SILO, data.GHAN, data.PROTEIN, data.QUALITY_OF_FIREWOOD,
                                        data.PRODUCT_SIZE, data.PRODUCT_TYPE, data.DEDUCT_IN_BAG,data.DEDUCT_IN_WT, data.NET_WEIGHT,
                                        data.UNLOAD_UNIT, serial_no_qc_setup, form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                                        _workContext.CurrentUserinformation.login_code,
                                        DateTime.Now.ToString("dd-MMM-yyyy"), 'N', _workContext.CurrentUserinformation.login_code, _workContext.CurrentUserinformation.login_code, isPlastic_bagValue, isJute_bagValue, isPlastic_weightValue, isJute_weightValue, data.VEHICLE_NO, data.BILL_NO, data.GATE_OR_GRN_NO, data.PRODUCT_REMARKS, data.REMARKS,data.SLIDER_VALUE,isUnloadValue, isProteinValue, isPtValue, isOutValue,data.DEDUCT_IN_PLASTIC,data.DEDUCT_IN_JUTE);
                        _dbContext.Database.ExecuteSqlCommand(insertRawMaterialTranQuery);

                        int p = 1;
                        foreach (var raw in data.WEIGHTDETAILSList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO GLOBAL_Weight_Details (GLOBAL_WEIGHT_NO,FIRST_WEIGHT, SECOND_WEIGHT, NET_WEIGHT, CHALLAN_WEIGHT, WEIGHT_DIFFERENCE
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}', TO_DATE('{7}', 'DD-MON-YYYY'), '{8}','{9}','{10}','{11}','{12}','{13}')",
                                        data.TRANSACTION_NO, raw.FIRST_WEIGHT, raw.SECOND_WEIGHT, raw.NET_WEIGHT, raw.CHALLAN_WEIGHT,
                                        raw.WEIGHT_DIFFERENCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, p, raw.REMARKS);  // change hard code 498
                            _dbContext.Database.ExecuteSqlCommand(insertQuery);
                            p++;
                        }
                        int q = 1;
                        foreach (var raw in data.UNLOADEDCHHALLIList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO GLOBAL_UNLOADED_CHHALLI (GLOBAL_UNLOADED_NO,FIRST_CHHALLI, SECOND_CHHALLI, THIRD_CHHALLI, FOURTH_CHHALLI, FIFTH_CHHALLI
                        ,SIXTH_CHHALLI, SEVENTH_CHHALLI, EIGHTH_CHHALLI, NINETH_CHHALLI, TENTH_CHHALLI
                        ,ELEVEN_CHHALLI,TWELVE_CHHALLI,TOTAL
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}', '{7}', '{8}', '{9}', '{10}','{11}', '{12}','{13}','{14}', TO_DATE('{15}', 'DD-MON-YYYY'),'{16}','{17}','{18}','{19}','{20}','{21}')",
                                        data.TRANSACTION_NO, raw.FIRST_CHHALLI, raw.SECOND_CHHALLI, raw.THIRD_CHHALLI, raw.FOURTH_CHHALLI,
                                        raw.FIFTH_CHHALLI
                                        , raw.SIXTH_CHHALLI, raw.SEVENTH_CHHALLI, raw.EIGHTH_CHHALLI, raw.NINETH_CHHALLI,
                                        raw.TENTH_CHHALLI, raw.ELEVEN_CHHALLI, raw.TWELVE_CHHALLI, raw.TOTAL

                                        , _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, q, raw.REMARKS);  // change hard code 498
                            _dbContext.Database.ExecuteSqlCommand(insertQuery);
                            q++;
                        }
                        int r = 1;
                        foreach (var raw in data.DAKHILADETAILSList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO GLOBAL_DAKHILA_DETAILS (GLOBAL_DAKHILA_NO,ENTRY_NO, BILL_NO, CHALAN_NO, ITEM, TOTAL_BAG, WEIGHT
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}','{7}', TO_DATE('{8}', 'DD-MON-YYYY'),'{9}','{10}','{11}','{12}','{13}','{14}')",
                                        data.TRANSACTION_NO, raw.ENTRY_NO, raw.BILL_NO, raw.CHALAN_NO, raw.ITEM,
                                        raw.TOTAL_BAG, raw.WEIGHT, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, r, raw.REMARKS);  // change hard code 498
                            _dbContext.Database.ExecuteSqlCommand(insertQuery);
                            r++;
                        }
                    }
                    else
                    {
                        string deleteWeightDetailsQuery = $@"Delete from GLOBAL_Weight_Details WHERE GLOBAL_WEIGHT_NO = '{transaction_no}' ";
                        _dbContext.ExecuteSqlCommand(deleteWeightDetailsQuery);
                        int i = 1;
                        foreach (var raw in data.WEIGHTDETAILSList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO GLOBAL_Weight_Details (GLOBAL_WEIGHT_NO,FIRST_WEIGHT, SECOND_WEIGHT, NET_WEIGHT, CHALLAN_WEIGHT, WEIGHT_DIFFERENCE
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}', TO_DATE('{7}', 'DD-MON-YYYY'), '{8}','{9}','{10}','{11}','{12}','{13}')",
                                       transaction_no, raw.FIRST_WEIGHT, raw.SECOND_WEIGHT, raw.NET_WEIGHT, raw.CHALLAN_WEIGHT,
                                       raw.WEIGHT_DIFFERENCE, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                               , 'N', _workContext.CurrentUserinformation.company_code,
                               _workContext.CurrentUserinformation.branch_code, form_code, i, raw.REMARKS);  // change hard code 498

                            _dbContext.Database.ExecuteSqlCommand(insertQuery);
                            i++;
                        }
                        string deleteUnloadedChhalliQuery = $@"Delete from GLOBAL_UNLOADED_CHHALLI WHERE GLOBAL_UNLOADED_NO = '{transaction_no}' ";
                        _dbContext.ExecuteSqlCommand(deleteUnloadedChhalliQuery);
                        int j = 1;
                        foreach (var raw in data.UNLOADEDCHHALLIList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO GLOBAL_UNLOADED_CHHALLI (GLOBAL_UNLOADED_NO,FIRST_CHHALLI, SECOND_CHHALLI, THIRD_CHHALLI, FOURTH_CHHALLI, FIFTH_CHHALLI
                        ,SIXTH_CHHALLI, SEVENTH_CHHALLI, EIGHTH_CHHALLI, NINETH_CHHALLI, TENTH_CHHALLI
                        ,ELEVEN_CHHALLI,TWELVE_CHHALLI,TOTAL
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}','{6}', '{7}', '{8}', '{9}', '{10}','{11}', '{12}','{13}','{14}', TO_DATE('{15}', 'DD-MON-YYYY'),'{16}','{17}','{18}','{19}','{20}','{21}')",
                                        data.TRANSACTION_NO, raw.FIRST_CHHALLI, raw.SECOND_CHHALLI, raw.THIRD_CHHALLI, raw.FOURTH_CHHALLI,
                                        raw.FIFTH_CHHALLI
                                        , raw.SIXTH_CHHALLI, raw.SEVENTH_CHHALLI, raw.EIGHTH_CHHALLI, raw.NINETH_CHHALLI,
                                        raw.TENTH_CHHALLI, raw.ELEVEN_CHHALLI, raw.TWELVE_CHHALLI, raw.TOTAL

                                        , _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, j, raw.REMARKS);  // change hard code 498

                            _dbContext.Database.ExecuteSqlCommand(insertQuery);
                            j++;
                        }
                        string deleteDakhilaDetailsQuery = $@"Delete from GLOBAL_DAKHILA_DETAILS WHERE GLOBAL_DAKHILA_NO = '{transaction_no}' ";
                        _dbContext.ExecuteSqlCommand(deleteDakhilaDetailsQuery);
                        int k = 1;
                        foreach (var raw in data.DAKHILADETAILSList)
                        {
                            string insertQuery = string.Format(@"
                    INSERT INTO GLOBAL_DAKHILA_DETAILS (GLOBAL_DAKHILA_NO,ENTRY_NO, BILL_NO, CHALAN_NO, ITEM, TOTAL_BAG, WEIGHT
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}','{7}', TO_DATE('{8}', 'DD-MON-YYYY'),'{9}','{10}','{11}','{12}','{13}','{14}')",
                                        data.TRANSACTION_NO, raw.ENTRY_NO, raw.BILL_NO, raw.CHALAN_NO, raw.ITEM,
                                        raw.TOTAL_BAG, raw.WEIGHT, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, k, raw.REMARKS);  // change hard code 498

                            _dbContext.Database.ExecuteSqlCommand(insertQuery);
                            k++;
                        }

                        string updateQuery = $@"UPDATE GLOBAL_PRODUCTS_TRANSACTION 
                       SET 
                           ITEM_CODE = '{data.ITEM_CODE}', 
                            PARTY_NAME = '{data.PARTY_NAME}',ADDRESS = '{data.ADDRESS}',PHYSICAL_TEST_RAWMATERIAL = '{data.PHYSICAL_TEST_RAWMATERIAL}'
                            ,WEIGHT = '{data.WEIGHT}',MOISTURE = '{data.MOISTURE}',TEMPERATURE = '{data.TEMPERATURE}'
                            ,WET = '{data.WET}',FUNGUS = '{data.FUNGUS}',DUST = '{data.DUST}'
                            ,GRADING = '{data.GRADING}',SMELL = '{data.SMELL}',COLOR = '{data.COLOR}'
                            ,PIECES = '{data.PIECES}',IMMATURITY_OF_GRAINS = '{data.IMMATURITY_OF_GRAINS}',OTHER_ITEMS = '{data.OTHER_ITEMS}'
                            ,ROTTEN_HOLED = '{data.ROTTEN_HOLED}',DAMAGED = '{data.DAMAGED}',BROKEN = '{data.BROKEN}'
                            ,HUSK = '{data.HUSK}',OVERTOASTED = '{data.OVERTOASTED}',USEABLE = '{data.USEABLE}'
                            ,UNUSEABLE = '{data.UNUSEABLE}',FAT = '{data.FAT}',QUALITY_OF_GOODS = '{data.QUALITY_OF_GOODS}'
                            ,EXCELLENT = '{data.EXCELLENT}',GREAT = '{data.GREAT}',GOODS_NORMAL = '{data.GOODS_NORMAL}'
                            ,WAREHOUSE = '{data.WAREHOUSE}',SILO = '{data.SILO}',GHAN = '{data.GHAN}'
                            ,PROTEIN = '{data.PROTEIN}',QUALITY_OF_FIREWOOD = '{data.QUALITY_OF_FIREWOOD}',PRODUCT_SIZE = '{data.PRODUCT_SIZE}'
                            ,PRODUCT_TYPE = '{data.PRODUCT_TYPE}',DEDUCT_IN_BAG = '{data.DEDUCT_IN_BAG}',DEDUCT_IN_WT = '{data.DEDUCT_IN_WT}',NET_WEIGHT = '{data.NET_WEIGHT}'
                            ,UNLOAD_UNIT = '{data.UNLOAD_UNIT}',ISPLASTIC_BAG = '{data.ISPLASTIC_BAG}',ISJUTE_BAG = '{data.ISJUTE_BAG}'
                            ,ISPLASTIC_WEIGHT = '{data.ISPLASTIC_WEIGHT}',ISJUTE_WEIGHT = '{data.ISJUTE_WEIGHT}',VEHICLE_NO = '{data.VEHICLE_NO}'
                            ,BILL_NO = '{data.BILL_NO}',GATE_OR_GRN_NO = '{data.GATE_OR_GRN_NO}',PRODUCT_REMARKS = '{data.PRODUCT_REMARKS}'
                           ,REMARKS = '{data.REMARKS}'
                            ,DEDUCT_IN_PLASTIC = '{data.DEDUCT_IN_PLASTIC}'
                            ,DEDUCT_IN_JUTE = '{data.DEDUCT_IN_JUTE}'
                            ,SLIDER_VALUE = '{data.SLIDER_VALUE}'
                            ,ISUNLOAD = '{data.ISUNLOAD}',ISPROTEIN = '{data.ISPROTEIN}',ISPT = '{data.ISPT}',ISOUT = '{data.ISOUT}'
                           ,MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                       WHERE TRANSACTION_NO = '{transaction_no}' ";
                        _dbContext.Database.ExecuteSqlCommand(updateQuery);
                    }
                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Transaction failed: " + ex.Message, ex);
                }
            }
        }

        public GlobalAgroProducts GetEditGlobalAgroProductLists(string transactionno)
        {
            GlobalAgroProducts raw = new GlobalAgroProducts();
            String query_globalProducts = $@"select TRANSACTION_NO,ITEM_CODE,SERIAL_NO,CREATED_DATE
,VEHICLE_NO,PARTY_NAME,ADDRESS,BILL_NO,GATE_OR_GRN_NO,PHYSICAL_TEST_RAWMATERIAL,
WEIGHT,MOISTURE,TEMPERATURE,WET,FUNGUS,DUST,GRADING,SMELL,COLOR,PIECES,IMMATURITY_OF_GRAINS
,OTHER_ITEMS,ROTTEN_HOLED,DAMAGED,BROKEN,HUSK,OVERTOASTED,USEABLE,UNUSEABLE,FAT,QUALITY_OF_GOODS
,EXCELLENT,GREAT,GOODS_NORMAL,WAREHOUSE,SILO,GHAN,PROTEIN,QUALITY_OF_FIREWOOD,PRODUCT_SIZE,
PRODUCT_TYPE,DEDUCT_IN_BAG,DEDUCT_IN_WT,NET_WEIGHT,ISPLASTIC_BAG,ISJUTE_BAG,ISPLASTIC_WEIGHT,ISJUTE_WEIGHT
,PRODUCT_REMARKS,REMARKS,UNLOAD_UNIT,SLIDER_VALUE,ISUNLOAD,ISPROTEIN,ISPT,ISOUT,DEDUCT_IN_PLASTIC,DEDUCT_IN_JUTE from GLOBAL_PRODUCTS_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<GlobalAgroProducts>(query_globalProducts).FirstOrDefault();

            String query_globalWeight = $@"select FIRST_WEIGHT,SECOND_WEIGHT,NET_WEIGHT,CHALLAN_WEIGHT,WEIGHT_DIFFERENCE ,REMARKS from GLOBAL_Weight_Details where GLOBAL_WEIGHT_NO='{transactionno}'";
            raw.WEIGHTDETAILSList = this._dbContext.SqlQuery<WEIGHTDETAILS>(query_globalWeight).ToList();


            String query_globalUnload = $@"select FIRST_CHHALLI,SECOND_CHHALLI,THIRD_CHHALLI,FOURTH_CHHALLI,FIFTH_CHHALLI
                    ,SIXTH_CHHALLI,SEVENTH_CHHALLI,EIGHTH_CHHALLI,NINETH_CHHALLI,TENTH_CHHALLI,ELEVEN_CHHALLI,TWELVE_CHHALLI,TOTAL
                    from GLOBAL_UNLOADED_CHHALLI where GLOBAL_UNLOADED_NO='{transactionno}'";

            raw.UNLOADEDCHHALLIList = this._dbContext.SqlQuery<UNLOADEDCHHALLI>(query_globalUnload).ToList();

            String query_dakhilaDetails = $@"select ENTRY_NO,BILL_NO,CHALAN_NO,ITEM,TOTAL_BAG,WEIGHT,REMARKS   from GLOBAL_DAKHILA_DETAILS where GLOBAL_DAKHILA_NO='{transactionno}'";
            raw.DAKHILADETAILSList = this._dbContext.SqlQuery<DAKHILADETAILS>(query_dakhilaDetails).ToList();
            return raw;
        }

        public GlobalAgroProducts GetGlobalAgroProductsReport(string transactionno)
        {
            GlobalAgroProducts raw = new GlobalAgroProducts();
            var transaction_no = transactionno.Replace("'", "");
            String query_globalProducts = $@"select GPT.TRANSACTION_NO,GPT.ITEM_CODE,(SELECT DISTINCT INITCAP(item_edesc) as ITEM_EDESC FROM ip_item_master_setup WHERE 
DELETED_FLAG='N' AND ITEM_CODE = GPT.ITEM_CODE AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}') AS ITEM_EDESC,GPT.SERIAL_NO,TO_CHAR (GPT.CREATED_DATE, 'dd-Mon-yyyy')AS CREATED_DATE_STR
,GPT.VEHICLE_NO,GPT.PARTY_NAME,GPT.ADDRESS,GPT.BILL_NO,GPT.GATE_OR_GRN_NO,GPT.PHYSICAL_TEST_RAWMATERIAL,
GPT.WEIGHT,GPT.MOISTURE,GPT.TEMPERATURE,GPT.WET,GPT.FUNGUS,GPT.DUST,GPT.GRADING,GPT.SMELL,GPT.COLOR,GPT.PIECES,GPT.IMMATURITY_OF_GRAINS
,GPT.OTHER_ITEMS,GPT.ROTTEN_HOLED,GPT.DAMAGED,GPT.BROKEN,GPT.HUSK,GPT.OVERTOASTED,GPT.USEABLE,GPT.UNUSEABLE,GPT.FAT,GPT.QUALITY_OF_GOODS
,GPT.EXCELLENT,GPT.GREAT,GPT.GOODS_NORMAL,GPT.WAREHOUSE,GPT.SILO,GPT.GHAN,GPT.PROTEIN,GPT.QUALITY_OF_FIREWOOD,GPT.PRODUCT_SIZE,
GPT.PRODUCT_TYPE,GPT.DEDUCT_IN_BAG,GPT.DEDUCT_IN_WT,GPT.NET_WEIGHT,GPT.ISPLASTIC_BAG,GPT.ISJUTE_BAG,GPT.ISPLASTIC_WEIGHT,GPT.ISJUTE_WEIGHT
,GPT.REMARKS,GPT.PRODUCT_REMARKS,GPT.UNLOAD_UNIT,ISUNLOAD,ISPROTEIN,ISPT,ISOUT,GPT.DEDUCT_IN_PLASTIC,GPT.DEDUCT_IN_JUTE from GLOBAL_PRODUCTS_TRANSACTION GPT
                        where GPT.TRANSACTION_NO='{transaction_no}'";
            raw = _dbContext.SqlQuery<GlobalAgroProducts>(query_globalProducts).FirstOrDefault();

            if (raw != null)
            {
                String query_globalWeight = $@"select FIRST_WEIGHT,SECOND_WEIGHT,NET_WEIGHT,CHALLAN_WEIGHT,WEIGHT_DIFFERENCE ,REMARKS from GLOBAL_Weight_Details where GLOBAL_WEIGHT_NO='{transaction_no}'";
                raw.WEIGHTDETAILSList = this._dbContext.SqlQuery<WEIGHTDETAILS>(query_globalWeight).ToList();


                String query_globalUnload = $@"select FIRST_CHHALLI,SECOND_CHHALLI,THIRD_CHHALLI,FOURTH_CHHALLI,FIFTH_CHHALLI
                    ,SIXTH_CHHALLI,SEVENTH_CHHALLI,EIGHTH_CHHALLI,NINETH_CHHALLI,TENTH_CHHALLI,ELEVEN_CHHALLI,TWELVE_CHHALLI,TOTAL
                    from GLOBAL_UNLOADED_CHHALLI where GLOBAL_UNLOADED_NO='{transaction_no}'";

                raw.UNLOADEDCHHALLIList = this._dbContext.SqlQuery<UNLOADEDCHHALLI>(query_globalUnload).ToList();

                String query_dakhilaDetails = $@"select ENTRY_NO,BILL_NO,CHALAN_NO,ITEM,TOTAL_BAG,WEIGHT,REMARKS   from GLOBAL_DAKHILA_DETAILS where GLOBAL_DAKHILA_NO='{transaction_no}'";
                raw.DAKHILADETAILSList = this._dbContext.SqlQuery<DAKHILADETAILS>(query_dakhilaDetails).ToList();
            }
            return raw;

            // If no valid voucherno, return the view with a default state (qc is null)
        }
    }
}
