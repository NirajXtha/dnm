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
   public class HandOverInspectionRepo : IHandOverInspectionRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public HandOverInspectionRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FormDetailSetup> GetHandOverInspectionList()
        {
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'HO'";
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
        public bool InsertHandOverInspectionData(HandOverInspection data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.DISPATCH_NO}'";
                    string dispatch_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'HO'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();


                    if (dispatch_no == null)
                    {
                        string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND REGEXP_LIKE(SERIAL_NO, '^\d+$')";
                        string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();
                        string insertRawMaterialTranQuery = string.Format(@"INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO
                                    ,REFERENCE_NO,ITEM_CODE,SERIAL_NO,QC_CODE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY
                                    ,CREATED_DATE,DELETED_FLAG,REMARKS,MANUAL_NO,UNIT,OVERALL_REMARKS)
                                    VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9:dd-MMM-yyyy}', 'DD-MON-YYYY'),'{10}','{11}','{12}','{13}','{14}')"
                                     , data.DISPATCH_NO, '0', '0', serial_no_qc_setup, serial_no_qc_setup, form_code, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.branch_code,
                                     _workContext.CurrentUserinformation.login_code
                                     , DateTime.Now.ToString("dd-MMM-yyyy"), 'N', data.REMARKS, data.MANUAL_NO, data.PACKING_UNIT, data.OVERALL_REMARKS);
                        _dbContext.ExecuteSqlCommand(insertRawMaterialTranQuery);

                        foreach (var raw in data.HandOverInspectionDetailsList)
                        {
                            string batch = $@"select TRANSACTION_NO from BATCH_TRANSACTION WHERE DELETED_FLAG ='N' AND BATCH_NO ='{raw.BATCH_NO}'";
                            string batch_no = this._dbContext.SqlQuery<string>(batch).FirstOrDefault();
                            string insertQuery = string.Format(@"
                    INSERT INTO HANDOVER_INSPECTION (HANDOVER_INSPECTION_NO,TIME_PERIOD, ITEM_CODE, BATCH_NO, LOOSE_PRODUCT_SAMPLE, LOOSE_PRODUCT_DEFECT, UNSEALED_PACKET_SAMPLE
                        , UNSEALED_PACKET_DEFECT, SEALED_PACKET_SAMPLE,SEALED_PACKET_DEFECT,BAG_SAMPLE,BAG_DEFECT
                        , WRAPPER_SAMPLE,WRAPPER_DEFECT
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS,PRODUCT_TYPE)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}','{11}','{12}','{13}','{14}', TO_DATE('{15}', 'DD-MON-YYYY'),'{16}','{17}','{18}','{19}','{20}','{21}','{22}')",
                                        data.DISPATCH_NO, raw.TIME_PERIOD, raw.ITEM_CODE, batch_no, raw.LOOSE_PRODUCT_SAMPLE,
                                        raw.LOOSE_PRODUCT_DEFECT, raw.UNSEALED_PACKET_SAMPLE, raw.UNSEALED_PACKET_DEFECT, raw.SEALED_PACKET_SAMPLE, raw.SEALED_PACKET_DEFECT, raw.BAG_SAMPLE, raw.BAG_DEFECT, raw.WRAPPER_SAMPLE, raw.WRAPPER_DEFECT, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, serial_no_qc_setup, raw.REMARKS,raw.PRODUCT_TYPE);  // change hard code 498

                            _dbContext.ExecuteSqlCommand(insertQuery);
                        }
                    }
                    else
                    {
                        string dailyWastageQuery = $@"Delete from HANDOVER_INSPECTION WHERE HANDOVER_INSPECTION_NO = '{dispatch_no}' ";
                        _dbContext.ExecuteSqlCommand(dailyWastageQuery);
                        int k = 1;
                        foreach (var raw in data.HandOverInspectionDetailsList)
                        {
                            string batch = $@"select TRANSACTION_NO from BATCH_TRANSACTION WHERE DELETED_FLAG ='N' AND BATCH_NO ='{raw.BATCH_NO}'";
                            string batch_no = this._dbContext.SqlQuery<string>(batch).FirstOrDefault();
                            string insertQuery = string.Format(@"
                    INSERT INTO HANDOVER_INSPECTION (HANDOVER_INSPECTION_NO,TIME_PERIOD, ITEM_CODE, BATCH_NO, LOOSE_PRODUCT_SAMPLE, LOOSE_PRODUCT_DEFECT, UNSEALED_PACKET_SAMPLE
                        , UNSEALED_PACKET_DEFECT, SEALED_PACKET_SAMPLE,SEALED_PACKET_DEFECT,BAG_SAMPLE,BAG_DEFECT
                        , WRAPPER_SAMPLE,WRAPPER_DEFECT
                        , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,FORM_CODE,SERIAL_NO,REMARKS,PRODUCT_TYPE)
                    VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','{9}','{10}','{11}','{12}','{13}','{14}', TO_DATE('{15}', 'DD-MON-YYYY'),'{16}','{17}','{18}','{19}','{20}','{21}','{22}')",
                                        data.DISPATCH_NO, raw.TIME_PERIOD, raw.ITEM_CODE, batch_no, raw.LOOSE_PRODUCT_SAMPLE,
                                        raw.LOOSE_PRODUCT_DEFECT, raw.UNSEALED_PACKET_SAMPLE, raw.UNSEALED_PACKET_DEFECT, raw.SEALED_PACKET_SAMPLE, raw.SEALED_PACKET_DEFECT, raw.BAG_SAMPLE, raw.BAG_DEFECT, raw.WRAPPER_SAMPLE, raw.WRAPPER_DEFECT, _workContext.CurrentUserinformation.login_code, data.CREATED_DATE.ToString("dd-MMM-yyyy")
                                , 'N', _workContext.CurrentUserinformation.company_code,
                                _workContext.CurrentUserinformation.branch_code, form_code, k, raw.REMARKS, raw.PRODUCT_TYPE);  // change hard code 498

                            _dbContext.ExecuteSqlCommand(insertQuery);
                            k++;
                        }

                        string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                       SET 
                           UNIT = '{data.PACKING_UNIT}',MANUAL_NO ='{data.MANUAL_NO}', 
                           REMARKS = '{data.REMARKS}', 
                           OVERALL_REMARKS = '{data.OVERALL_REMARKS}', 
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

        public HandOverInspection GetEditHandOverInspection(string transactionno)
        {
            HandOverInspection raw = new HandOverInspection();
            String query1 = $@"select TRANSACTION_NO as DISPATCH_NO,ITEM_CODE as PACKING_UNIT,MANUAL_NO,SERIAL_NO,CREATED_DATE,REMARKS,OVERALL_REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<HandOverInspection>(query1).FirstOrDefault();
            String query2 = $@" select distinct PDI.TIME_PERIOD,PDI.ITEM_CODE,PDI.PRODUCT_TYPE
 ,IT.ITEM_EDESC,(select batch_no from batch_transaction where transaction_no=PDI.BATCH_NO) AS BATCH_NO ,PDI.BATCH_NO AS BATCH_EDESC
,PDI.LOOSE_PRODUCT_SAMPLE,PDI.LOOSE_PRODUCT_DEFECT,PDI.UNSEALED_PACKET_SAMPLE,PDI.UNSEALED_PACKET_DEFECT,PDI.SEALED_PACKET_SAMPLE
 ,PDI.SEALED_PACKET_DEFECT,PDI.BAG_SAMPLE
 ,PDI.BAG_DEFECT,PDI.WRAPPER_SAMPLE,PDI.WRAPPER_DEFECT,PDI.REMARKS from HANDOVER_INSPECTION PDI
 left join IP_PRODUCT_ITEM_MASTER_SETUP IT on IT.item_code = PDI.item_code  
 WHERE PDI.HANDOVER_INSPECTION_NO= '{transactionno}'";
            List<HandOverInspectionDetails> rawMaterials = new List<HandOverInspectionDetails>();
            rawMaterials = this._dbContext.SqlQuery<HandOverInspectionDetails>(query2).ToList();
            raw.LooseProduct_SampleTotal = rawMaterials.Sum(x => decimal.TryParse(x.LOOSE_PRODUCT_SAMPLE, out var v) ? v : 0);
            raw.LooseProduct_DefectTotal = rawMaterials.Sum(x => decimal.TryParse(x.LOOSE_PRODUCT_DEFECT, out var v) ? v : 0);
            raw.UnsealedPacket_SampleTotal = rawMaterials.Sum(x => decimal.TryParse(x.UNSEALED_PACKET_SAMPLE, out var v) ? v : 0);
            raw.UnsealedPacket_DefectTotal = rawMaterials.Sum(x => decimal.TryParse(x.UNSEALED_PACKET_DEFECT, out var v) ? v : 0);
            raw.SealedPacket_SampleTotal = rawMaterials.Sum(x => decimal.TryParse(x.SEALED_PACKET_SAMPLE, out var v) ? v : 0);
            raw.SealedPacket_DefectTotal = rawMaterials.Sum(x => decimal.TryParse(x.SEALED_PACKET_DEFECT, out var v) ? v : 0);
            raw.CartonBag_SampleTotal = rawMaterials.Sum(x => decimal.TryParse(x.BAG_SAMPLE, out var v) ? v : 0);
            raw.CartonBag_DefectTotal = rawMaterials.Sum(x => decimal.TryParse(x.BAG_DEFECT, out var v) ? v : 0);
            raw.WrapperInPrinter_SampleTotal = rawMaterials.Sum(x => decimal.TryParse(x.WRAPPER_SAMPLE, out var v) ? v : 0);
            raw.WrapperInPrinter_DefectTotal = rawMaterials.Sum(x => decimal.TryParse(x.BAG_DEFECT, out var v) ? v : 0);
            raw.HandOverInspectionDetailsList = rawMaterials;
            return raw;
        }
        public HandOverInspection GetHandOverInspectionReport(string transactionno)
        {
            HandOverInspection raw = new HandOverInspection();
            String query1 = $@"select TRANSACTION_NO as DISPATCH_NO,ITEM_CODE as PACKING_UNIT,MANUAL_NO,SERIAL_NO,CREATED_DATE,REMARKS from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            raw = _dbContext.SqlQuery<HandOverInspection>(query1).FirstOrDefault();
            String query2 = $@" select distinct PDI.TIME_PERIOD,PDI.ITEM_CODE,PDI.PRODUCT_TYPE
 ,IT.ITEM_EDESC,(select batch_no from batch_transaction where transaction_no=PDI.BATCH_NO) AS BATCH_NO ,PDI.BATCH_NO AS BATCH_EDESC
,PDI.LOOSE_PRODUCT_SAMPLE,PDI.LOOSE_PRODUCT_DEFECT,PDI.UNSEALED_PACKET_SAMPLE,PDI.UNSEALED_PACKET_DEFECT,PDI.SEALED_PACKET_SAMPLE
 ,PDI.SEALED_PACKET_DEFECT,PDI.BAG_SAMPLE
 ,PDI.BAG_DEFECT,PDI.WRAPPER_SAMPLE,PDI.WRAPPER_DEFECT,PDI.REMARKS from HANDOVER_INSPECTION PDI
 left join IP_PRODUCT_ITEM_MASTER_SETUP IT on IT.item_code = PDI.item_code  
 WHERE PDI.HANDOVER_INSPECTION_NO= '{transactionno}'";
            List<HandOverInspectionDetails> rawMaterials = new List<HandOverInspectionDetails>();
            rawMaterials = this._dbContext.SqlQuery<HandOverInspectionDetails>(query2).ToList();
            raw.HandOverInspectionDetailsList = rawMaterials;
            return raw;
        }
    }
}
