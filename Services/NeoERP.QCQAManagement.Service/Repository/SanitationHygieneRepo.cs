using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.QCQAManagement.Service.Interface;
using NeoERP.QCQAManagement.Service.Models;
using System;
using System.Collections.Generic;
using System.Data;
//using System.Configuration;
//using System.Data;
//using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.QCQAManagement.Service.Repository
{
    public class SanitationHygieneRepo: ISanitationHygieneRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public SanitationHygieneRepo(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }
        public List<FormDetailSetup> GetSanitationHygieneList()
        {
            string query_FormCode = $@"SELECT form_code FROM form_setup WHERE FORM_TYPE = 'SH'";
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
                           FROM FORM_DETAIL_SETUP FDS
                           LEFT JOIN
                              COMPANY_SETUP CS ON FDS.COMPANY_CODE = CS.COMPANY_CODE
                              LEFT JOIN FORM_SETUP FS
                               ON FDS.FORM_CODE = FS.FORM_CODE AND FDS.COMPANY_CODE = FS.COMPANY_CODE
                     WHERE  FDS.MASTER_CHILD_FLAG = 'C' AND FDS.DISPLAY_FLAG='Y' AND FDS.FORM_CODE = '{formCode}'  AND CS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' order by FDS.SERIAL_NO";
            List<FormDetailSetup> entity = this._dbContext.SqlQuery<FormDetailSetup>(Query).ToList();
            return entity;
        }
        public List<SanitationHygiene> GetMasterSanitationHygiene()
        {
            string Query = $@"select pre_location_code,location_code AS DEPARTMENT_CODE,location_edesc AS DEPARTMENT_EDESC,Storage_capacity as STANDARD,'' ACTUAL,Storage_capacity as  GAP from ip_location_setup WHERE pre_location_code = '00' and ( location_type_code IS NULL OR location_type_code <> 'PR')  order by location_code";
            List<SanitationHygiene> entity = this._dbContext.SqlQuery<SanitationHygiene>(Query).ToList();
            return entity;
        }
        public List<SanitationHygiene> GetAllSanitationHygieneDetails()
        {
            string Query = $@"select p.pre_location_code,p.location_code AS DEPARTMENT_CODE,p.location_edesc AS DEPARTMENT_EDESC
,p.Storage_capacity as STANDARD,'' ACTUAL,p.Storage_capacity as  GAP
         ,CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM ip_location_setup c
            WHERE c.pre_location_code =  (select location_code from ip_location_setup where location_code = p.location_code AND ROWNUM = 1)
        )
        THEN 0
        ELSE 1
    END AS isExpanded
 from ip_location_setup p WHERE  p.pre_location_code <> '00' and ( p.location_type_code IS NULL 
            OR p.location_type_code <> 'PR') and p.pre_location_code  in ( select location_code from ip_location_setup WHERE pre_location_code = '00' 
and ( location_type_code IS NULL OR location_type_code <> 'PR')) 
            order by p.location_code";
            List<SanitationHygiene> entity = this._dbContext.SqlQuery<SanitationHygiene>(Query).ToList();
            return entity;
        }
        public List<SanitationHygiene> GetSanitationHygieneDetails(string LocationCode)
        {
            string Query = $@"select p.pre_location_code,p.location_code AS DEPARTMENT_CODE,p.location_edesc AS DEPARTMENT_EDESC
,p.Storage_capacity as STANDARD,'' ACTUAL,p.Storage_capacity as  GAP
         ,CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM ip_location_setup c
            WHERE c.pre_location_code =  (select location_code from ip_location_setup where location_code = p.location_code AND ROWNUM = 1)
        )
        THEN 0
        ELSE 1
    END AS isExpanded
 from ip_location_setup p WHERE  p.pre_location_code <> '00' and ( p.location_type_code IS NULL 
            OR p.location_type_code <> 'PR') and p.pre_location_code like '{LocationCode}' 
            order by p.location_code";
            List<SanitationHygiene> entity = this._dbContext.SqlQuery<SanitationHygiene>(Query).ToList();
            return entity;
        }
        public bool InsertSanitationHygieneData(SanitationHygiene data)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    string query = $@"select TRANSACTION_NO from QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG ='N' AND TRANSACTION_NO ='{data.SANITATION_NO}'";
                    string inspection_no = this._dbContext.SqlQuery<string>(query).FirstOrDefault();
                    string form_code_query = $@"select FORM_CODE from form_setup where form_type = 'SH'";
                    string form_code = this._dbContext.SqlQuery<string>(form_code_query).FirstOrDefault();

                    if (inspection_no == null)
                    {
                        string query_serial_no_qc_setup = $@"SELECT TO_CHAR(COALESCE(MAX(TO_NUMBER(SERIAL_NO)), 0) + 1) FROM QC_PARAMETER_TRANSACTION WHERE DELETED_FLAG = 'N' AND form_code ='{form_code}'";
                        string serial_no_qc_setup = this._dbContext.SqlQuery<string>(query_serial_no_qc_setup).FirstOrDefault();

                        string insertMasterQuery = string.Format(@"
                            INSERT INTO QC_PARAMETER_TRANSACTION (TRANSACTION_NO,ITEM_CODE,BATCH_NO,SHIFT,REFERENCE_NO
                                , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE,SERIAL_NO,QC_CODE,FORM_CODE)
                            VALUES('{0}', '{1}', '{2}', '{3}', '{4}', '{5}',TO_DATE('{6}', 'DD-MON-YYYY'),'{7}','{8}','{9}','{10}','{11}','{12}')",
                                   data.SANITATION_NO, 0, "", "", 0, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                           , 'N', _workContext.CurrentUserinformation.company_code,
                           _workContext.CurrentUserinformation.branch_code, serial_no_qc_setup, serial_no_qc_setup, form_code);
                        int rowsInserted = _dbContext.ExecuteSqlCommand(insertMasterQuery);
                        if (rowsInserted > 0)
                        {
                            int i = 1;
                            foreach (var sat in data.SanitationHygieneList)
                            {
                                string insertQuery = string.Format(@"
                                INSERT INTO SANITATIONHYGIENE (SERIAL_NO,SANITATION_NO,DEPARTMENT_CODE,STANDARD,ACTUAL,GAP
                                    , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                                VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}', TO_DATE('{7}', 'DD-MON-YYYY'),'{8}','{9}','{10}')",
                                            i, data.SANITATION_NO, sat.DEPARTMENT_CODE, sat.STANDARD, sat.ACTUAL, sat.GAP, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                    , 'N', _workContext.CurrentUserinformation.company_code,
                                    _workContext.CurrentUserinformation.branch_code);
                                _dbContext.ExecuteSqlCommand(insertQuery);
                                i++;
                            }                          
                        }
                        else
                        {
                            transaction.Rollback();
                            throw new Exception("Transaction failed");
                        }
                    }
                    else
                    {
                        string deleteParameterDetailsQuery = $@"Delete from SANITATIONHYGIENE WHERE SANITATION_NO = '{data.SANITATION_NO}' ";
                        _dbContext.ExecuteSqlCommand(deleteParameterDetailsQuery);
                        int i = 1;
                        foreach (var sat in data.SanitationHygieneList)
                        {
                                string insertQuery = string.Format(@"
                       INSERT INTO SANITATIONHYGIENE (SERIAL_NO,SANITATION_NO,DEPARTMENT_CODE,STANDARD,ACTUAL,GAP
                                    , CREATED_BY, CREATED_DATE, DELETED_FLAG,COMPANY_CODE,BRANCH_CODE)
                                VALUES('{0}', '{1}', '{2}','{3}','{4}','{5}','{6}', TO_DATE('{7}', 'DD-MON-YYYY'),'{8}','{9}','{10}')",
                                            i, data.SANITATION_NO, sat.DEPARTMENT_CODE, sat.STANDARD, sat.ACTUAL, sat.GAP, _workContext.CurrentUserinformation.login_code, DateTime.Now.ToString("dd-MMM-yyyy")
                                    , 'N', _workContext.CurrentUserinformation.company_code,
                                    _workContext.CurrentUserinformation.branch_code);
                            _dbContext.ExecuteSqlCommand(insertQuery);
                            i++;                            
                        }
                        string updateQuery = $@"UPDATE QC_PARAMETER_TRANSACTION 
                           SET           
                           MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                           MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                            WHERE TRANSACTION_NO = '{data.SANITATION_NO}' ";
                        _dbContext.ExecuteSqlCommand(updateQuery);
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
        public SanitationHygiene GetEditSanitationHygiene(string transactionno)
        {
            SanitationHygiene sanitation = new SanitationHygiene();
            List<SanitationHygiene> detailsRaw = new List<SanitationHygiene>();
            String details = $@" select sg.SANITATION_NO,sg.CREATED_DATE,sg.DEPARTMENT_CODE,plc.LOCATION_EDESC AS DEPARTMENT_EDESC,CAST(sg.STANDARD AS DECIMAL(18,2)) AS STANDARD,
    CAST(sg.ACTUAL AS DECIMAL(18,2)) AS ACTUAL,
    CAST(sg.GAP AS DECIMAL(18,2)) AS GAP  from SANITATIONHYGIENE sg
    INNER JOIN ip_location_setup plc ON plc.location_code = sg.DEPARTMENT_CODE
                             WHERE 
                              plc.pre_location_code = '00' and
                             sg.SANITATION_NO = '{transactionno}'";
            var rawList = this._dbContext.SqlQuery<SanitationHygiene>(details).ToList();
            var sanitationHygieneList = rawList.Select(r => new SanitationHygiene
            {
                SANITATION_NO = r.SANITATION_NO,
                CREATED_DATE = r.CREATED_DATE,
                DEPARTMENT_CODE = r.DEPARTMENT_CODE,
                STANDARD = string.IsNullOrEmpty(Convert.ToString(r.STANDARD)) ? (decimal?)null : Convert.ToDecimal(r.STANDARD),
                ACTUAL = string.IsNullOrEmpty(Convert.ToString(r.ACTUAL)) ? (decimal?)null : Convert.ToDecimal(r.ACTUAL),
                GAP = string.IsNullOrEmpty(Convert.ToString(r.GAP)) ? (decimal?)null : Convert.ToDecimal(r.GAP)
            }).ToList();
            sanitationHygieneList = this._dbContext.SqlQuery<SanitationHygiene>(details).ToList();
            sanitation.SanitationHygieneList = sanitationHygieneList;


            String childDetails = $@"  select sg.SANITATION_NO,sg.CREATED_DATE,sg.DEPARTMENT_CODE,plc.LOCATION_EDESC AS DEPARTMENT_EDESC,CAST(sg.STANDARD AS DECIMAL(18,2)) AS STANDARD,
    CAST(sg.ACTUAL AS DECIMAL(18,2)) AS ACTUAL,
    CAST(sg.GAP AS DECIMAL(18,2)) AS GAP  from SANITATIONHYGIENE sg
    INNER JOIN ip_location_setup plc ON plc.location_code = sg.DEPARTMENT_CODE
                             WHERE 
                              plc.pre_location_code <> '00' and
                             SANITATION_NO = '{transactionno}'";
            var childList = this._dbContext.SqlQuery<SanitationHygiene>(childDetails).ToList();
            var sanitationHygieneChildList = childList.Select(r => new SanitationHygiene
            {
                SANITATION_NO = r.SANITATION_NO,
                CREATED_DATE = r.CREATED_DATE,
                DEPARTMENT_CODE = r.DEPARTMENT_CODE,
                STANDARD = string.IsNullOrEmpty(Convert.ToString(r.STANDARD)) ? (decimal?)null : Convert.ToDecimal(r.STANDARD),
                ACTUAL = string.IsNullOrEmpty(Convert.ToString(r.ACTUAL)) ? (decimal?)null : Convert.ToDecimal(r.ACTUAL),
                GAP = string.IsNullOrEmpty(Convert.ToString(r.GAP)) ? (decimal?)null : Convert.ToDecimal(r.GAP)
            }).ToList();
            sanitationHygieneChildList = this._dbContext.SqlQuery<SanitationHygiene>(childDetails).ToList();
            sanitation.SanitationHygieneChildList = sanitationHygieneChildList;
            return sanitation;
        }

        public SanitationHygiene GetSanitationHygieneReport(string transactionno)
        {
            SanitationHygiene sanitation = new SanitationHygiene();
            String query1 = $@"select TRANSACTION_NO as SANITATION_NO,CREATED_DATE from QC_PARAMETER_TRANSACTION
                        where TRANSACTION_NO='{transactionno}'";
            sanitation = _dbContext.SqlQuery<SanitationHygiene>(query1).FirstOrDefault();

            List<SanitationHygiene> detailsRaw = new List<SanitationHygiene>();
            String details = $@" select SANITATION_NO,CREATED_DATE,DEPARTMENT_CODE,CAST(STANDARD AS DECIMAL(18,2)) AS STANDARD,
    CAST(ACTUAL AS DECIMAL(18,2)) AS ACTUAL,
    CAST(GAP AS DECIMAL(18,2)) AS GAP  from SANITATIONHYGIENE
                             WHERE SANITATION_NO = '{transactionno}'";
            var rawList = this._dbContext.SqlQuery<SanitationHygiene>(details).ToList();
            var sanitationHygieneList = rawList.Select(r => new SanitationHygiene
            {
                SANITATION_NO = r.SANITATION_NO,
                CREATED_DATE = r.CREATED_DATE,
                DEPARTMENT_CODE = r.DEPARTMENT_CODE,
                STANDARD = string.IsNullOrEmpty(Convert.ToString(r.STANDARD)) ? (decimal?)null : Convert.ToDecimal(r.STANDARD),
                ACTUAL = string.IsNullOrEmpty(Convert.ToString(r.ACTUAL)) ? (decimal?)null : Convert.ToDecimal(r.ACTUAL),
                GAP = string.IsNullOrEmpty(Convert.ToString(r.GAP)) ? (decimal?)null : Convert.ToDecimal(r.GAP)
            }).ToList();
            sanitationHygieneList = this._dbContext.SqlQuery<SanitationHygiene>(details).ToList();
            sanitation.SanitationHygieneList = sanitationHygieneList;
            return sanitation;
        }

        #region Report
        public List<ChildModel> GetSanitationHygieneDetailsReport(string frmDate , string toDate)
        {
            //ORDER BY DEPARTMENT_EDESC";

            DateTime frommDate = Convert.ToDateTime(frmDate);
            DateTime toooDate = Convert.ToDateTime(toDate);

            // 1️⃣ Calculate number of days (inclusive)
            int dayCount = (toooDate - frommDate).Days + 1;

            // 2️⃣ Build dynamic Day columns
            List<string> dayColumns = new List<string>();
            for (int i = 1; i <= dayCount; i++)
            {
                dayColumns.Add($"{i} AS Day{i}");
            }
            string dynamicDays = string.Join(", ", dayColumns);
            string Query = $@"SELECT *
FROM (
    SELECT 
        ils.location_edesc AS DEPARTMENT_EDESC,
        ROW_NUMBER() OVER (
            PARTITION BY ils.location_edesc
            ORDER BY sh.created_date
        ) AS day_number,
        TO_NUMBER(NULLIF(sh.STANDARD, '')) AS STANDARD,
        TO_NUMBER(NULLIF(sh.ACTUAL, '')) AS ACTUAL,
        TO_NUMBER(NULLIF(sh.GAP, '')) AS GAP
    FROM sanitationhygiene sh
    INNER JOIN ip_location_setup ils
        ON ils.location_code = sh.DEPARTMENT_CODE
    WHERE sh.created_date BETWEEN TO_DATE('{frmDate}','YYYY-MON-DD') 
                              AND TO_DATE('{toDate}','YYYY-MON-DD') 
)
PIVOT (
    MAX(STANDARD) AS STD,
    MAX(ACTUAL) AS ACT,
    MAX(GAP) AS GAP
    FOR day_number IN (
         {dynamicDays}
    )
)
ORDER BY DEPARTMENT_EDESC";
            DataTable dt = this._dbContext.SqlQuery(Query);
            //List<ChildModel> entity = this._dbContext.SqlQuery<ChildModel>(Query).ToList();
            List<ChildModel> childModels = new List<ChildModel>();
            foreach (DataRow row in dt.Rows)
            {
                var cm = new ChildModel
                {
                    DEPARTMENT_EDESC = row["DEPARTMENT_EDESC"].ToString()
                };
                DateTime fromDate = Convert.ToDateTime(frmDate);
                DateTime tooDate = Convert.ToDateTime(toDate);

                int days = (tooDate - fromDate).Days;

                for (int day = 1; day <= days; day++)
                {
                    var dayData = new DayData
                    {
                        STANDARD = row.Table.Columns.Contains($"DAY{day}_STD") && row[$"DAY{day}_STD"] != DBNull.Value
                            ? Convert.ToDecimal(row[$"DAY{day}_STD"])
                            : (decimal?)null,

                        ACTUAL = row.Table.Columns.Contains($"DAY{day}_ACT") && row[$"DAY{day}_ACT"] != DBNull.Value
                            ? Convert.ToDecimal(row[$"DAY{day}_ACT"])
                            : (decimal?)null,

                        GAP = row.Table.Columns.Contains($"DAY{day}_GAP") && row[$"DAY{day}_GAP"] != DBNull.Value
                            ? Convert.ToDecimal(row[$"DAY{day}_GAP"])
                            : (decimal?)null
                    };

                    cm.Days[day] = dayData;
                }

                childModels.Add(cm);
            }


            return childModels;
        }
        #endregion

    }
}
