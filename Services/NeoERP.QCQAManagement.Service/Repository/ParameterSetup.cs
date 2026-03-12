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
    public class ParameterSetup : IParameterRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        public ParameterSetup(IWorkContext workContext, IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
        }

        public List<SubMenu> GetSubMenuList()
        {
            try
            {
                List<SubMenu> tableList = new List<SubMenu>();
                string query = $@"select * from ( SELECT DISTINCT  FS.FORM_CODE,FS.FORM_EDESC,FDS.TABLE_NAME FROM FORM_SETUP FS 
                    INNER JOIN QC_PARAMETER_TRANSACTION MT 
                    ON MT.FORM_CODE=FS.FORM_CODE  AND MT.COMPANY_CODE=FS.COMPANY_CODE 
                    INNER JOIN FORM_DETAIL_SETUP FDS 
                    ON FDS.FORM_CODE=FS.FORM_CODE AND FDS.COMPANY_CODE=FS.COMPANY_CODE
                    WHERE  FS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MT.BRANCH_CODE='{_workContext.CurrentUserinformation.branch_code}' 
                    )
                     ORDER BY TO_NUMBER(FORM_CODE) ASC";
                tableList = this._dbContext.SqlQuery<SubMenu>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<QCQASubMenu> GetQCQADetailByFormCode(string formCode, string docVer = "All")
        {
            try
            {
                List<QCQASubMenu> tableList = new List<QCQASubMenu>();
                string query = $@"SELECT FORM_CODE, TRANSACTION_NO AS VOUCHER_NO,0 VOUCHER_AMOUNT,CREATED_DATE AS VOUCHER_DATE
                ,CREATED_BY,CREATED_DATE
                ,'' CHECKED_BY,CREATED_DATE AS CHECKED_DATE,'' AUTHORISED_BY,CREATED_DATE AS POSTED_DATE
                ,MODIFY_DATE,SYN_ROWID,REFERENCE_NO
                ,SESSION_ROWID,'' ITEM_EDESC,'' VEHICLE_NO,'' PARTY_NAME,'' ADDRESS,'' BILL_NO
                FROM QC_PARAMETER_TRANSACTION WHERE FORM_CODE ='{formCode}' 
                and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND DELETED_FLAG='N'";
                tableList = this._dbContext.SqlQuery<QCQASubMenu>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<ParameterSetup> MasterItemList()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<ParameterSetup> FormDetailList = new List<ParameterSetup>();
                return FormDetailList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<Items> GetGroupMaterialLists()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string query = $@"SELECT master_item_code, INITCAP(item_edesc) as item_edesc, pre_item_code,item_code,category_CODE FROM IP_PRODUCT_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N' 
                                AND COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND group_sku_flag ='G'
                                and PRE_ITEM_CODE ='00'
                                ORDER BY LENGTH(PRE_ITEM_CODE),PRE_ITEM_CODE, MASTER_ITEM_CODE";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Items> GetChildItems(string masterItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string query = $@"SELECT master_item_code, 
       INITCAP(item_edesc) as item_edesc, 
       pre_item_code, 
       item_code, 
       CATEGORY_CODE 
FROM IP_PRODUCT_ITEM_MASTER_SETUP 
WHERE DELETED_FLAG = 'N' 
  AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
  AND group_sku_flag = 'G' 
  AND (PRE_ITEM_CODE LIKE '%{masterItemCode}%')
ORDER BY LENGTH(PRE_ITEM_CODE), PRE_ITEM_CODE, MASTER_ITEM_CODE";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<Products> GetProductDetails(string masterItemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Products> tableList = new List<Products>();
                string query = $@"SELECT A.ITEM_CODE,INITCAP(A.ITEM_EDESC) AS ITEM_EDESC,TO_CHAR(A.DIMENSION), A.CURRENT_STOCK, A.INDEX_MU_CODE
, A.BATCH_FLAG, TO_CHAR(A.MAX_LEVEL),TO_CHAR(A.MIN_LEVEL),TO_CHAR(A.PREFERRED_LEVEL),TO_CHAR(A.REORDER_LEVEL)
,TO_CHAR(A.DANGER_LEVEL),CASE WHEN A.PRODUCT_CODE IS NULL THEN A.ITEM_CODE ELSE A.PRODUCT_CODE END AS PRODUCT_CODE, (SELECT DISTINCT C.CATEGORY_EDESC 
FROM IP_CATEGORY_CODE C WHERE C.CATEGORY_CODE = A.CATEGORY_CODE AND 
C.COMPANY_CODE = A.COMPANY_CODE) CATEGORY_EDESC, (SELECT DISTINCT B.MU_EDESC FROM IP_MU_CODE B 
WHERE B.MU_CODE = A.INDEX_MU_CODE AND B.COMPANY_CODE = A.COMPANY_CODE) MU_EDESC, A.PURCHASE_PRICE
, PRE_ITEM_CODE, A.DELETED_FLAG, RACK_LOCATION FROM IP_PRODUCT_ITEM_MASTER_SETUP A WHERE 1 = 1  AND 
A.DELETED_FLAG = 'N'  AND A.COMPANY_CODE ='{company_code}' 
AND A.PRE_ITEM_CODE='{masterItemCode}' AND A.GROUP_SKU_FLAG='I' ORDER BY A.PRODUCT_CODE, A.ITEM_EDESC";
                tableList = this._dbContext.SqlQuery<Products>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public bool InsertParameterData(Items data)
        {
            try
            {
                string query = $@"SELECT ITEM_CODE from IP_ITEM_SPEC_SETUP WHERE ITEM_CODE='{data.item_code}' and DELETED_FLAG ='N' and COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";
                string check_itemCode = _dbContext.SqlQuery<string>(query).FirstOrDefault();
                if (check_itemCode == null)
                {
                    var idquery = $@"SELECT (MAX(NVL(TO_NUMBER(form_code), 0)) + 1) as form_code FROM form_setup";
                    int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();

                    string insertQuery = string.Format(@"INSERT INTO IP_ITEM_SPEC_SETUP (
    ITEM_CODE, PART_NUMBER, BRAND_NAME, ITEM_SPECIFICATION,ITEM_APPLY_ON, INTERFACE, TYPE,
    LAMINATION, ITEM_SIZE, THICKNESS, COLOR, GRADE, 
    REMARKS, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, GSM, SIZE_LENGHT, SIZE_WIDTH,SPEC_COMPULSORY_FLAG
,ROLLDIAMETER,PH,UNPLEASANT_SMELL_ODOUR,DUST_DIRT,DAMAGING_MATERIAL,CORE_DAMAGING,TENSILE_CD,TENSILE_MD,STRENGTH,STRENGTH_MD,VISUAL_INSPECTION) 
                                 VALUES({0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}','{13}',
'{14}',TO_DATE('{15:dd-MMM-yyyy}', 'DD-MON-YYYY'),'{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','{27}','{28}','{29}','{30}','{31}')",
                                             data.item_code, data.PART_NUMBER,data.BRAND_NAME, data.ITEM_SPECIFICATION, data.ITEM_APPLY_ON, data.INTERFACE, data.TYPE, data.LAMINATION
                                             , data.ITEM_SIZE, data.thickness, data.COLOR, data.GRADE, data.REMARKS, _workContext.CurrentUserinformation.company_code, _workContext.CurrentUserinformation.login_code
                                             , DateTime.Now.ToString("dd-MMM-yyyy"), 'N'
                                             , data.GSM, data.SIZE_LENGHT, data.SIZE_WIDTH, 'N', data.RollDiameter, data.PH, data.UNPLEASANT_SMELL_ODOUR, data.Dust_Dirt, data.Damaging_Material
                                             , data.Core_Damaging, data.Tensile_CD
                                             , data.Tensile_MD,data.Strength,data.Strength_MD, data.Visual_Inspection);
                                _dbContext.ExecuteSqlCommand(insertQuery);
                    if (data.REEM_WEIGHT_KG != null) {
                        string updateMasterSetup = $@"UPDATE IP_PRODUCT_ITEM_MASTER_SETUP 
                                           SET REEM_WEIGHT_KG = '{data.REEM_WEIGHT_KG}' WHERE ITEM_CODE = '{data.item_code}' and DELETED_FLAG ='N'";
                                            _dbContext.ExecuteSqlCommand(updateMasterSetup);
                    }

                }
                else
                {
                    string updateQuery = $@"UPDATE IP_ITEM_SPEC_SETUP 
                                           SET PART_NUMBER = '{data.PART_NUMBER}',BRAND_NAME = '{data.BRAND_NAME}',
                    ITEM_SPECIFICATION = '{data.ITEM_SPECIFICATION}',ITEM_APPLY_ON = '{data.ITEM_APPLY_ON}',INTERFACE='{data.INTERFACE}'
                    ,TYPE='{data.TYPE}',LAMINATION='{data.LAMINATION}',ITEM_SIZE = '{data.ITEM_SIZE}',THICKNESS='{data.thickness}',COLOR='{data.COLOR}',GRADE='{data.GRADE}'
                    ,REMARKS = '{data.REMARKS}',GSM='{data.GSM}',SIZE_LENGHT='{data.SIZE_LENGHT}',SIZE_WIDTH = '{data.SIZE_WIDTH}',ROLLDIAMETER='{data.RollDiameter}',PH='{data.PH}'
                    ,UNPLEASANT_SMELL_ODOUR = '{data.UNPLEASANT_SMELL_ODOUR}',DUST_DIRT='{data.Dust_Dirt}',DAMAGING_MATERIAL='{data.Damaging_Material}',CORE_DAMAGING = '{data.Core_Damaging}',TENSILE_CD='{data.Tensile_CD}',Tensile_MD='{data.Tensile_MD}'
                    ,STRENGTH='{data.Strength}',STRENGTH_MD='{data.Strength_MD}'
                    ,Visual_Inspection='{data.Visual_Inspection}'
                    ,MODIFY_BY='{_workContext.CurrentUserinformation.login_code}',MODIFY_DATE =TO_DATE('{DateTime.Now.ToString("dd-MMM-yyyy"):dd-MMM-yyyy}','DD-MON-YYYY'),
                     COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                                           WHERE ITEM_CODE = '{data.item_code}' and DELETED_FLAG ='N'";
                                        _dbContext.ExecuteSqlCommand(updateQuery);
                    if (data.REEM_WEIGHT_KG != null)
                    {
                        string updateMasterSetup = $@"UPDATE IP_PRODUCT_ITEM_MASTER_SETUP 
                                           SET REEM_WEIGHT_KG = '{data.REEM_WEIGHT_KG}' WHERE ITEM_CODE = '{data.item_code}' and DELETED_FLAG ='N'";
                        _dbContext.ExecuteSqlCommand(updateMasterSetup);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Items> GetSpecDetailsByItemID(string itemCode)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                List<Items> tableList = new List<Items>();
                string query = $@"SELECT IISS.ITEM_CODE,IISS.PART_NUMBER,IIMS.ITEM_EDESC,IISS.ITEM_SPECIFICATION
,IIMS.ITEM_NDESC
,IISS.BRAND_NAME,IISS.ITEM_SPECIFICATION,IISS.ITEM_APPLY_ON,IISS.INTERFACE,IISS.TYPE
,IISS.LAMINATION,IISS.ITEM_SIZE,IISS.THICKNESS,IISS.COLOR,IISS.GRADE,IISS.GSM,IISS.REMARKS,IISS.item_code, IISS.SIZE_LENGHT, IISS.SIZE_WIDTH
,IISS.ROLLDIAMETER,IISS.PH,IISS.UNPLEASANT_SMELL_ODOUR,IISS.DUST_DIRT,IISS.DAMAGING_MATERIAL,IISS.CORE_DAMAGING
,IISS.TENSILE_CD,IISS.TENSILE_MD,IISS.STRENGTH,IISS.STRENGTH_MD,IISS.VISUAL_INSPECTION,IIMS.REEM_WEIGHT_KG,IISS.PH
FROM IP_ITEM_SPEC_SETUP IISS
LEFT JOIN IP_PRODUCT_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = IISS.ITEM_CODE 
WHERE IISS.DELETED_FLAG ='N' AND IISS.COMPANY_CODE='{company_code}' and IISS.item_code='{itemCode}'";
                tableList = this._dbContext.SqlQuery<Items>(query).ToList();
                return tableList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
