using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.ProjectManagement.Service.Interface;
using NeoERP.ProjectManagement.Service.Models;
using NeoERP.DocumentTemplate.Service.Models;
using NeoERP.DocumentTemplate.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.IO;

namespace NeoERP.ProjectManagement.Service.Repository
{
    public class ProjectRepo : IFormProjectRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        NeoErpCoreEntity _coreentity;
        private ICacheManager _cacheManager;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        public ProjectRepo(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager, NeoErpCoreEntity coreentity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._cacheManager = cacheManager;
            _coreentity = coreentity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        public bool InsertProjectData(Project projectData)
        {
            var query = $@"SELECT COALESCE(MAX(ID) + 1, 1) AS ID FROM PROJECT_SETUP";
            int id = _dbContext.SqlQuery<int>(query).FirstOrDefault();
            string insertQuery = string.Format(@"INSERT INTO PROJECT_SETUP(ID, PROJECT_NAME, CREATED_DT, CREATED_BY, STATUS) 
                                         VALUES({0}, '{1}', TO_DATE('{2}', 'DD-MON-YYYY'), '{3}', '{4}')",
                                          id, projectData.PROJECT_NAME, DateTime.Now.ToString("dd-MMM-yyyy"),
                                          _workContext.CurrentUserinformation.company_code, "E");
            _dbContext.ExecuteSqlCommand(insertQuery);
            List<SubProjectData> subProjects = projectData.SubProjects;
            if (subProjects != null)
            {
                foreach (var subProject in subProjects)
                {
                    var subquery = $@"SELECT COALESCE(MAX(SUB_PROJECT_ID) + 1, 1) AS subProjectId FROM SUB_PROJECT_SETUP";
                    int subProjectId = _dbContext.SqlQuery<int>(subquery).FirstOrDefault();
                    string insertSubQuery = string.Format(@"INSERT INTO SUB_PROJECT_SETUP(SUB_PROJECT_ID, PROJECT_ID, SUB_PROJECT_NAME, IMAGE, AREA,BUDGET_PLANNING,PROJECT_MANAGER,CONTRACTOR,START_DT,END_DT,STATUS) 
                                         VALUES({0}, '{1}',  '{2}', '{3}', '{4}','{5}','{6}','{7}',TO_DATE('{8}', 'DD-MON-YYYY'),TO_DATE('{9}', 'DD-MON-YYYY'),'{10}')",
                                                  subProjectId, id, subProject.SUB_PROJECTNAME, subProject.IMAGE_NAME, subProject.AREA, subProject.BUDGET_PLANNING, subProject.PROJECT_MANAGER, subProject.CONTRACTOR, subProject.START_DT.ToString("dd-MMM-yyyy"), subProject.END_DT.ToString("dd-MMM-yyyy"), subProject.STATUS);
                    _dbContext.ExecuteSqlCommand(insertSubQuery);
                    if (subProject.MaterialPlanningData != null)
                    {
                        List<MaterialPlanning> materialPlannings = subProject.MaterialPlanningData;
                        InsertMaterialData(materialPlannings, subProjectId); // Pass subProjectId to InsertMaterialData
                    }
                    if (subProject.LabourPlanningData != null)
                    {
                        List<LabourPlanning> labourPlannings = subProject.LabourPlanningData;
                        InsertLabourData(labourPlannings, subProjectId); // Pass subProjectId to InsertMaterialData
                    }
                }
            }
            return true;

        }
        public void InsertMaterialData(List<MaterialPlanning> materialPlannings, int subProjectId)
        {
            try
            {
                foreach (var materialPlanning in materialPlannings)
                {
                    var materialquery = $@"SELECT COALESCE(MAX(ID) + 1, 1) AS materialId FROM MATERIAL_PLANNING_SETUP";
                    int materialId = _dbContext.SqlQuery<int>(materialquery).FirstOrDefault();
                    string insertMaterialQuery = string.Format(@"INSERT INTO MATERIAL_PLANNING_SETUP(ID, SUB_PROJECT_ID, DESCRIPTION, QUANTITY, RATE,AMOUNT) 
                             VALUES({0}, '{1}',  '{2}', '{3}', '{4}','{5}')",
                                  materialId, subProjectId, materialPlanning.DESCRIPTION, materialPlanning.QUANTITY, materialPlanning.RATE, materialPlanning.AMOUNT);
                    _dbContext.ExecuteSqlCommand(insertMaterialQuery);
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while inserting material data: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public void InsertLabourData(List<LabourPlanning> labourPlannings, int subProjectId)
        {
            try
            {
                foreach (var labourPlanning in labourPlannings)
                {
                    var labourquery = $@"SELECT COALESCE(MAX(ID) + 1, 1) AS labourId FROM LABOUR_PLANNING_SETUP";
                    int labourId = _dbContext.SqlQuery<int>(labourquery).FirstOrDefault();
                    string insertLabourQuery = string.Format(@"INSERT INTO LABOUR_PLANNING_SETUP(ID, SUB_PROJECT_ID, DESCRIPTION, QUANTITY, RATE,AMOUNT) 
                             VALUES({0}, '{1}',  '{2}', '{3}', '{4}','{5}')",
                                  labourId, subProjectId, labourPlanning.DESCRIPTION, labourPlanning.QUANTITY, labourPlanning.RATE, labourPlanning.AMOUNT);
                    _dbContext.ExecuteSqlCommand(insertLabourQuery);
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while inserting labour data: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<ProjectCount> ListAllProjects()
        {
            try
            {
                string Query = $@"SELECT 
            PS.PROJECT_NAME,
            PS.ID,PS.CREATED_DT,
            COUNT(DISTINCT SPS.SUB_PROJECT_ID) AS SUB_PROJECT_COUNT,
            SUM(TO_NUMBER(REGEXP_SUBSTR(SPS.area, '[[:digit:]]+'))) AS TOTAL_AREA,
            SUM(TO_NUMBER(REGEXP_SUBSTR(SPS.BUDGET_PLANNING, '[[:digit:]]+'))) AS TOTAL_BUDGET,
            MIN(TO_DATE(SPS.START_DT, 'DD-MON-YY')) AS START_DATE,
            MAX(TO_DATE(SPS.END_DT, 'DD-MON-YY')) AS END_DATE,
            COUNT(DISTINCT LPS.ID) AS LABOUR_COUNT,
            COUNT(DISTINCT MPS.ID) AS MATERIAL_COUNT
        FROM 
            PROJECT_SETUP PS
        LEFT JOIN 
            SUB_PROJECT_SETUP SPS ON PS.ID = SPS.PROJECT_ID
        LEFT JOIN 
            LABOUR_PLANNING_SETUP LPS ON LPS.SUB_PROJECT_ID = SPS.SUB_PROJECT_ID
        LEFT JOIN 
            MATERIAL_PLANNING_SETUP MPS ON MPS.SUB_PROJECT_ID = SPS.SUB_PROJECT_ID
        WHERE 
            PS.STATUS = 'E' AND SPS.DELETED_FLAG='N'
        GROUP BY 
            PS.ID, PS.PROJECT_NAME,PS.CREATED_DT order by id desc";
                List<ProjectCount> entity = this._dbContext.SqlQuery<ProjectCount>(Query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public bool DeleteProjects(int id)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE PROJECT_SETUP SET STATUS ='D' WHERE ID='{id}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                return true;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<Project> GetProjectById(int id)
        {
            try
            {
                // Fetch project data
                string projectQuery = $@"SELECT * FROM PROJECT_SETUP WHERE ID = '{id}' AND STATUS = 'E'";
                List<Project> projects = this._dbContext.SqlQuery<Project>(projectQuery).ToList();
                foreach (var project in projects)
                {
                    string subProjectQuery = $@"SELECT SPS.SUB_PROJECT_ID as SubProjectId,SPS.SUB_PROJECT_NAME as SUB_PROJECTNAME ,SPS.IMAGE AS IMAGE_NAME,SPS.AREA,SPS.BUDGET_PLANNING,HE.EMPLOYEE_EDESC AS PROJECT_MANAGER,SPS.STATUS,
                    ISS.SUPPLIER_EDESC AS CONTRACTOR,SPS.START_DT,SPS.END_DT, CASE WHEN EXISTS ( SELECT 1  FROM MATERIAL_PLANNING_SETUP MS  WHERE MS.SUB_PROJECT_ID = SPS.SUB_PROJECT_ID) 
                    THEN 'PLANNED' ELSE 'N/A'  END AS MaterialPlanningData, CASE  WHEN EXISTS ( SELECT 1  FROM LABOUR_PLANNING_SETUP LS  WHERE LS.SUB_PROJECT_ID = SPS.SUB_PROJECT_ID) 
                    THEN 'PLANNED'  ELSE 'N/A' END AS LabourPlanningData FROM SUB_PROJECT_SETUP SPS LEFT JOIN HR_EMPLOYEE_SETUP HE ON (HE.EMPLOYEE_CODE=SPS.PROJECT_MANAGER)
                    LEFT JOIN IP_SUPPLIER_SETUP ISS ON (ISS.SUPPLIER_CODE=SPS.CONTRACTOR) WHERE SPS.PROJECT_ID ='{project.ID}' AND ISS.DELETED_FLAG='N' AND HE.DELETED_FLAG='N' and iss.company_code='{_workContext.CurrentUserinformation.company_code}'";
                    List<SubProjectData> subProjectData = this._dbContext.SqlQuery<SubProjectData>(subProjectQuery).ToList();
                    foreach (var subProject in subProjectData)
                    {
                        string materialQuery = $@"SELECT MS.ID,IP.ITEM_EDESC AS ITEM_NAME,MS.DESCRIPTION,MS.QUANTITY,MS.RATE,AMOUNT FROM MATERIAL_PLANNING_SETUP MS LEFT JOIN IP_ITEM_MASTER_SETUP IP ON (IP.ITEM_CODE=MS.DESCRIPTION) 
                        WHERE MS.SUB_PROJECT_ID='{subProject.SubProjectId}' AND IP.DELETED_FLAG='N' and IP.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' "; /*and IP.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'*/
                        List<MaterialPlanning> materialData = this._dbContext.SqlQuery<MaterialPlanning>(materialQuery).ToList();
                        subProject.MaterialPlanningData = materialData;
                    }
                    foreach (var subProject in subProjectData)
                    {
                        string labourQuery = $@"SELECT ID,DESCRIPTION,QUANTITY,RATE,AMOUNT FROM LABOUR_PLANNING_SETUP WHERE SUB_PROJECT_ID='{subProject.SubProjectId}'";
                        List<LabourPlanning> labourData = this._dbContext.SqlQuery<LabourPlanning>(labourQuery).ToList();
                        subProject.LabourPlanningData = labourData;
                    }
                    project.SubProjects = subProjectData;
                }

                return projects;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting data: " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<Employee> GetAllEmployees()
        {
            try
            {
                string query = $@"SELECT
                        COALESCE(EMPLOYEE_CODE,' ') as EMPLOYEE_CODE, 
                        COALESCE(EMPLOYEE_EDESC,' ') as EMPLOYEE_EDESC 
                        FROM hr_employee_setup
                        WHERE DELETED_FLAG='N' AND COMPANY_CODE ='{_workContext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}'";
                List<Employee> entity = this._dbContext.SqlQuery<Employee>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<ProjectCount> GetProjectCount()
        {
            try
            {
                string query = $@"SELECT COUNT(*) AS count, 'Total Project' AS heading, 'lightskyblue' AS color, 'fa fa-star fa-2x' AS icon, 1 AS sortOrder
                                FROM project_setup
                                WHERE status = 'E'
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'Total Sub Project' AS heading, 'purple' AS color, 'fa fa-minus-circle' AS icon, 2 AS sortOrder
                                FROM sub_project_setup sps
                                LEFT JOIN project_setup ps ON(ps.id = sps.project_id)
                                WHERE ps.status = 'E' 
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'Completed Sub-Project' AS heading, 'lawngreen' AS color, 'fa fa-asterisk' AS icon, 3 AS sortOrder
                                FROM sub_project_setup sps 
                                LEFT JOIN project_setup ps ON (ps.id = sps.project_id)
                                WHERE sps.status = 'C' AND ps.status = 'E' 
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'Running Sub-Project' AS heading, 'olive' AS color, 'fa fa-spinner' AS icon, 4 AS sortOrder
                                FROM sub_project_setup sps 
                                LEFT JOIN project_setup ps ON (ps.id = sps.project_id)
                                WHERE sps.status = 'R' AND ps.status = 'E'  
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'On Hold Sub-Project' AS heading, 'hotpink' AS color, 'fa fa-minus-circle' AS icon, 5 AS sortOrder
                                FROM sub_project_setup sps 
                                LEFT JOIN project_setup ps ON (ps.id = sps.project_id)
                                WHERE sps.status = 'H' AND ps.status = 'E' 
                                ORDER BY sortOrder";
                
                List<ProjectCount> entity = this._dbContext.SqlQuery<ProjectCount>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<ProjectCount> CountBarGraph()
        {
            try
            {
                string query = $@"SELECT COUNT(*) AS count, 'Total Project' AS heading, 'lightskyblue' AS color, 'fa fa-star fa-2x' AS icon, 1 AS sortOrder
                                FROM project_setup
                                WHERE status = 'E'
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'Completed Sub-Project' AS heading, 'lawngreen' AS color, 'fa fa-asterisk' AS icon, 3 AS sortOrder
                                FROM sub_project_setup sps 
                                LEFT JOIN project_setup ps ON (ps.id = sps.project_id)
                                WHERE sps.status = 'C' AND ps.status = 'E' 
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'Running Sub-Project' AS heading, 'olive' AS color, 'fa fa-spinner' AS icon, 4 AS sortOrder
                                FROM sub_project_setup sps 
                                LEFT JOIN project_setup ps ON (ps.id = sps.project_id)
                                WHERE sps.status = 'R' AND ps.status = 'E'  
                                UNION 
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'On Hold Sub-Project' AS heading, 'hotpink' AS color, 'fa fa-minus-circle' AS icon, 5 AS sortOrder
                                FROM sub_project_setup sps 
                                LEFT JOIN project_setup ps ON (ps.id = sps.project_id)
                                WHERE sps.status = 'H' AND ps.status = 'E' 
                                UNION
                                SELECT COUNT(DISTINCT sps.sub_project_id) AS count, 'Total Sub Project' AS heading, 'purple' AS color, 'fa fa-minus-circle' AS icon, 2 AS sortOrder
                                FROM sub_project_setup sps
                                LEFT JOIN project_setup ps ON(ps.id = sps.project_id)
                                WHERE ps.status = 'E'
                                ORDER BY sortOrder";

                List<ProjectCount> entity = this._dbContext.SqlQuery<ProjectCount>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<SubProjectData> GetCompletedProject()
        {
            try
            {
                string query = $@"SELECT PS.PROJECT_NAME,SPS.SUB_PROJECT_NAME AS SUB_PROJECTNAME,SPS.END_DT FROM PROJECT_SETUP PS LEFT JOIN SUB_PROJECT_SETUP
                                SPS ON (SPS.PROJECT_ID=PS.ID) WHERE SPS.STATUS='C' AND PS.STATUS='E' AND SPS.STATUS IS NOT NULL AND SPS.START_DT >= TRUNC(SYSDATE, 'MM') AND SPS.START_DT < ADD_MONTHS(TRUNC(SYSDATE, 'MM'), 1)";
                List<SubProjectData> entity = this._dbContext.SqlQuery<SubProjectData>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<SubProjectData> GetStartedProject()
        {
            try
            {
                string query = $@"SELECT PS.PROJECT_NAME,SPS.SUB_PROJECT_NAME AS SUB_PROJECTNAME,SPS.START_DT FROM PROJECT_SETUP PS LEFT JOIN SUB_PROJECT_SETUP
                                SPS ON (SPS.PROJECT_ID=PS.ID) WHERE SPS.STATUS='R' AND PS.STATUS='E' AND SPS.STATUS IS NOT NULL AND SPS.START_DT >= TRUNC(SYSDATE, 'MM') AND SPS.START_DT < ADD_MONTHS(TRUNC(SYSDATE, 'MM'), 1)";
                List<SubProjectData> entity = this._dbContext.SqlQuery<SubProjectData>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<SubProjectData> GetSubProjectList()
        {
            try
            {
                string query = $@"SELECT SPS.SUB_PROJECT_ID AS SubProjectId,SPS.SUB_PROJECT_NAME AS SUB_PROJECTNAME FROM SUB_PROJECT_SETUP SPS 
                                  LEFT JOIN PROJECT_SETUP PS ON (SPS.PROJECT_ID=PS.ID) WHERE PS.STATUS='E' ORDER BY SubProjectId DESC";
                List<SubProjectData> entity = this._dbContext.SqlQuery<SubProjectData>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting data : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public List<EntryReport> GetConsumptionTransDetailByFormCode(string formCode, string docVer = "all")
        {
            try
            {
                string query = $@"SELECT 
    MT.VOUCHER_NO,
    MT.VOUCHER_DATE,
    MT.CREATED_DATE,
    MT.CREATED_BY,
    IIMS.ITEM_EDESC,
    IGI.MU_CODE,
    IGI.CALC_QUANTITY,
    IGI.CALC_UNIT_PRICE,
    IGI.CALC_TOTAL_PRICE,
    CASE
        WHEN BT.BUDGET_FLAG = 'A' THEN 'Assets'
        WHEN BT.BUDGET_FLAG = 'I' THEN 'Income'
        WHEN BT.BUDGET_FLAG = 'L' THEN 'Liability'
        ELSE 'Expenses'
    END AS BUDGET_FLAG,
    BT.BUDGET_AMOUNT,
    SPS.SUB_PROJECT_NAME,
    PS.PROJECT_NAME,
    CS.COMPANY_EDESC,
    FBS.BRANCH_EDESC,
    ILS.LOCATION_EDESC
FROM
    MASTER_TRANSACTION MT
    LEFT JOIN IP_GOODS_ISSUE IGI ON MT.VOUCHER_NO = IGI.ISSUE_NO
    LEFT JOIN BUDGET_TRANSACTION BT ON IGI.ISSUE_NO = BT.REFERENCE_NO AND IGI.SUB_PROJECT_CODE = BT.SUB_PROJECT_CODE
    LEFT JOIN SUB_PROJECT_SETUP SPS ON SPS.SUB_PROJECT_ID = IGI.SUB_PROJECT_CODE
    LEFT JOIN PROJECT_SETUP PS ON PS.ID = SPS.PROJECT_ID
    LEFT JOIN COMPANY_SETUP CS ON CS.COMPANY_CODE = IGI.COMPANY_CODE
    LEFT JOIN FA_BRANCH_SETUP FBS ON FBS.BRANCH_CODE = IGI.BRANCH_CODE
    LEFT JOIN IP_LOCATION_SETUP ILS ON FROM_LOCATION_CODE=ILS.LOCATION_CODE AND ILS.COMPANY_CODE=IGI.COMPANY_CODE
    LEFT JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = IGI.ITEM_CODE AND IIMS.COMPANY_CODE = IGI.COMPANY_CODE AND IIMS.BRANCH_CODE = IGI.BRANCH_CODE
WHERE
    BT.SUB_PROJECT_CODE IS NOT NULL AND MT.FORM_CODE='{formCode}'
    AND MT.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
    AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
ORDER BY
    MT.CREATED_DATE DESC";
                var result = _dbContext.SqlQuery<EntryReport>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EntryReport> GetRequisitionTransDetailByFormCode(string formCode, string docVer = "all")
        {
            try
            {
                string query = $@"SELECT 
    MT.VOUCHER_NO,
    MT.VOUCHER_DATE,
    MT.CREATED_DATE,
    MT.CREATED_BY,
    IIMS.ITEM_EDESC,
    IGR.MU_CODE,
    IGR.CALC_QUANTITY,
    IGR.CALC_UNIT_PRICE,
    IGR.CALC_TOTAL_PRICE,
    SPS.SUB_PROJECT_NAME,
    PS.PROJECT_NAME,
    CS.COMPANY_EDESC,
    FBS.BRANCH_EDESC,
    ILS.LOCATION_EDESC
FROM
    MASTER_TRANSACTION MT
        LEFT JOIN IP_GOODS_REQUISITION IGR ON MT.VOUCHER_NO = IGR.REQUISITION_NO
    LEFT JOIN SUB_PROJECT_SETUP SPS ON SPS.SUB_PROJECT_ID = IGR.SUB_PROJECT_CODE
    LEFT JOIN PROJECT_SETUP PS ON PS.ID = SPS.PROJECT_ID
    LEFT JOIN COMPANY_SETUP CS ON CS.COMPANY_CODE = IGR.COMPANY_CODE
    LEFT JOIN FA_BRANCH_SETUP FBS ON FBS.BRANCH_CODE = IGR.BRANCH_CODE
    LEFT JOIN IP_LOCATION_SETUP ILS ON FROM_LOCATION_CODE=ILS.LOCATION_CODE AND ILS.COMPANY_CODE=IGR.COMPANY_CODE
    LEFT JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = IGR.ITEM_CODE AND IIMS.COMPANY_CODE = IGR.COMPANY_CODE AND IIMS.BRANCH_CODE = IGR.BRANCH_CODE
WHERE
    IGR.SUB_PROJECT_CODE IS NOT NULL AND MT.FORM_CODE='{formCode}'
    AND MT.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
    AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
ORDER BY
    MT.VOUCHER_DATE DESC";
                var result = _dbContext.SqlQuery<EntryReport>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EntryReport> GetRequisitionPendingTransByFormCode(string formCode, string docVer = "all")
        {
            try
            {
                string query = $@"SELECT 
    MT.VOUCHER_NO,
    MT.VOUCHER_DATE,
    MT.CREATED_DATE,
    MT.CREATED_BY,
    IIMS.ITEM_EDESC,
    IGR.MU_CODE,
    IGR.CALC_QUANTITY,
    IGR.CALC_UNIT_PRICE,
    IGR.CALC_TOTAL_PRICE,
    SPS.SUB_PROJECT_NAME,
    PS.PROJECT_NAME,
    CS.COMPANY_EDESC,
    FBS.BRANCH_EDESC,
    ILS.LOCATION_EDESC
FROM
    MASTER_TRANSACTION MT
        LEFT JOIN IP_GOODS_REQUISITION IGR ON MT.VOUCHER_NO = IGR.REQUISITION_NO
    LEFT JOIN SUB_PROJECT_SETUP SPS ON SPS.SUB_PROJECT_ID = IGR.SUB_PROJECT_CODE
    LEFT JOIN PROJECT_SETUP PS ON PS.ID = SPS.PROJECT_ID
    LEFT JOIN COMPANY_SETUP CS ON CS.COMPANY_CODE = IGR.COMPANY_CODE
    LEFT JOIN FA_BRANCH_SETUP FBS ON FBS.BRANCH_CODE = IGR.BRANCH_CODE
    LEFT JOIN IP_LOCATION_SETUP ILS ON FROM_LOCATION_CODE=ILS.LOCATION_CODE AND ILS.COMPANY_CODE=IGR.COMPANY_CODE
    LEFT JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = IGR.ITEM_CODE AND IIMS.COMPANY_CODE = IGR.COMPANY_CODE AND IIMS.BRANCH_CODE = IGR.BRANCH_CODE
WHERE
    IGR.SUB_PROJECT_CODE IS NOT NULL AND MT.FORM_CODE='{formCode}'
    AND MT.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND MT.AUTHORISED_DATE IS NULL
    AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
ORDER BY
    MT.VOUCHER_DATE DESC";
                var result = _dbContext.SqlQuery<EntryReport>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<EntryReport> GetPurchaseTransByFormCode(string formCode, string docVer = "all")
        {
            try
            {
                string query = $@"SELECT 
    MT.VOUCHER_NO,
    MT.VOUCHER_DATE,
    MT.CREATED_DATE,
    MT.CREATED_BY,
    IIMS.ITEM_EDESC,
    IPI.MU_CODE,
    IPI.CALC_QUANTITY,
    IPI.CALC_UNIT_PRICE,
    IPI.CALC_TOTAL_PRICE,
    SPS.SUB_PROJECT_NAME,
    PS.PROJECT_NAME,
    CS.COMPANY_EDESC,
    FBS.BRANCH_EDESC,
    ILS.LOCATION_EDESC
FROM
    MASTER_TRANSACTION MT
        LEFT JOIN IP_PURCHASE_INVOICE IPI ON MT.VOUCHER_NO = IPI.INVOICE_NO
    LEFT JOIN SUB_PROJECT_SETUP SPS ON SPS.SUB_PROJECT_ID = IPI.SUB_PROJECT_CODE
    LEFT JOIN PROJECT_SETUP PS ON PS.ID = SPS.PROJECT_ID
    LEFT JOIN COMPANY_SETUP CS ON CS.COMPANY_CODE = IPI.COMPANY_CODE
    LEFT JOIN FA_BRANCH_SETUP FBS ON FBS.BRANCH_CODE = IPI.BRANCH_CODE
    LEFT JOIN IP_LOCATION_SETUP ILS ON IPI.TO_LOCATION_CODE=ILS.LOCATION_CODE AND ILS.COMPANY_CODE=IPI.COMPANY_CODE
    LEFT JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = IPI.ITEM_CODE AND IIMS.COMPANY_CODE = IPI.COMPANY_CODE AND IIMS.BRANCH_CODE = IPI.BRANCH_CODE
WHERE
    IPI.SUB_PROJECT_CODE IS NOT NULL AND MT.FORM_CODE='{formCode}'
    AND MT.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
    AND MT.BRANCH_CODE = '{_workContext.CurrentUserinformation.branch_code}'
ORDER BY
    MT.VOUCHER_DATE DESC";
                var result = _dbContext.SqlQuery<EntryReport>(query).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //public List<SupplierModel> GetAllSupplier()
        //{
        //    try
        //    {
        //        string supplierQuery = $@"SELECT SUPPLIER_CODE,SUPPLIER_EDESC,REGD_OFFICE_EADDRESS,TEL_MOBILE_NO1 FROM IP_SUPPLIER_SETUP WHERE GROUP_SKU_FLAG='I' AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
        //        var typeData = _dbContext.SqlQuery<SupplierModel>(supplierQuery).ToList();
        //        return typeData;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting supplier: " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}
        public List<PriceSetupModel> ListAllItemWithName()
        {
            var companyCode = this._workContext.CurrentUserinformation.company_code;
            List<PriceSetupModel> allItemList = new List<PriceSetupModel>();
            try
            {
                string itemQuery = $@"SELECT itms.ITEM_CODE,itms.ITEM_EDESC,psc.OLD_PRICE,psc.NEW_PRICE,psc.STATUS FROM IP_ITEM_MASTER_SETUP itms LEFT JOIN PRICE_SETUP_CHILD psc on psc.ITEM_NAME=itms.ITEM_EDESC where itms.DELETED_FLAG='N' and itms.COMPANY_CODE='{companyCode}'";
                allItemList = this._dbContext.SqlQuery<PriceSetupModel>(itemQuery).ToList();
                return allItemList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while gettting item name : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
    }

}
