using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model.BrandingModule;
using NeoErp.Distribution.Service.Service.Branding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service
{
    public class BrandingService : IBrandingService
    {

        private NeoErpCoreEntity _objectEntity;
        public BrandingService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }
        public string saveBrandingActivity(ActivityModel model, User userInfo)
        {
            try
            {
                model.ACTIVITY_CODE = null;
                var groupFlag = _objectEntity.SqlQuery<string>($@"SELECT GROUP_ACTIVITY_FLAG FROM BRD_ACTIVITY WHERE MASTER_ACTIVITY_CODE='{model.MASTER_ACTIVITY_CODE}' AND COMPANY_CODE='{userInfo.company_code}'").FirstOrDefault();

                if (groupFlag == "Y" || groupFlag == "N" || groupFlag == null)
                {
                    if (string.IsNullOrWhiteSpace(model.ACTIVITY_CODE))
                        model.ACTIVITY_CODE = _objectEntity.SqlQuery<int>($@"SELECT (NVL(MAX(to_number(ACTIVITY_CODE)),0)+1) MAXID FROM BRD_ACTIVITY").FirstOrDefault().ToString();

                    //var data = _objectEntity.SqlQuery<ActivityModel>($@"SELECT (NVL(MAX(ACTIVITY_CODE),0)+1) ACTIVITY_CODE FROM BRD_ACTIVITY WHERE COMPANY_CODE ='{userInfo.company_code}'").FirstOrDefault();
                    var insertQuery = $@"INSERT INTO BRD_ACTIVITY(ACTIVITY_CODE,ACTIVITY_EDESC,ACTIVITY_NDESC,ACTIVITY_TYPE,PARENT_ACTIVITY_CODE,MASTER_ACTIVITY_CODE,PRE_ACTIVITY_CODE,GROUP_ACTIVITY_FLAG,EF1,EF2,REMARKS,COMPANY_CODE,
                                     BRANCH_CODE,CREATED_BY,CREATED_DATE,LAST_MODIFIED_BY,LAST_MODIFIED_DATE,APPROVED_BY,DELETED_FLAG)VALUES('{model.ACTIVITY_CODE}','{model.ACTIVITY_EDESC}','{model.ACTIVITY_NDESC}','{model.ACTIVITY_TYPE}','{model.PARENT_ACTIVITY_CODE}',
                                    '{model.MASTER_ACTIVITY_CODE}','{model.PRE_ACTIVITY_CODE}','{model.GROUP_ACTIVITY_FLAG}','{model.EF1}','{model.EF2}','{model.REMARKS}','{userInfo.company_code}',
                                    '{userInfo.branch_code}','{userInfo.login_code}',TO_DATE(SYSDATE),'{model.LAST_MODIFIED_BY}',TO_DATE(SYSDATE),'{model.APPROVED_BY}','N')";
                    var result = _objectEntity.ExecuteSqlCommand(insertQuery);
                    _objectEntity.SaveChanges();
                    return "success";
                }
                else
                {
                    return "denied";
                }

            }
            catch (Exception e)
            {
                return "failed";
            }
        }

        public string UpdateBrandingActivity(ActivityModel model, User userInfo)
        {

            //var updateQuery = $@"UPDATE BRD_ACTIVITY SET PARENT_ACTIVITY_CODE='{model.MASTER_ACTIVITY_CODE}' WHERE ACTIVITY_CODE ='{model.ACTIVITY_CODE}'";
            //_objectEntity.ExecuteSqlCommand(updateQuery);
            //_objectEntity.SaveChanges();
            try
            {
                var groupFlag = _objectEntity.SqlQuery<string>($@"SELECT GROUP_ACTIVITY_FLAG FROM BRD_ACTIVITY WHERE MASTER_ACTIVITY_CODE='{model.MASTER_ACTIVITY_CODE}' AND COMPANY_CODE='{userInfo.company_code}'").FirstOrDefault();

                if (groupFlag == "Y" || groupFlag == "N" || groupFlag == null)
                {

                    var updateQuery = $@"UPDATE BRD_ACTIVITY SET ACTIVITY_EDESC='{model.ACTIVITY_EDESC}',ACTIVITY_NDESC='{model.ACTIVITY_NDESC}',ACTIVITY_TYPE='{model.ACTIVITY_TYPE}',PARENT_ACTIVITY_CODE='{model.MASTER_ACTIVITY_CODE}',
                                               GROUP_ACTIVITY_FLAG='{model.GROUP_ACTIVITY_FLAG}',
                                     EF1='{model.EF1}',EF2='{model.EF2}',REMARKS='{model.REMARKS}' WHERE ACTIVITY_CODE ='{model.ACTIVITY_CODE}' AND COMPANY_CODE ='{userInfo.company_code}' AND DELETED_FLAG='N'";
                    var result = _objectEntity.ExecuteSqlCommand(updateQuery);
                    // recursiveFunction(model.MASTER_ACTIVITY_CODE,model.PRE_ACTIVITY_CODE, model.ACTIVITY_CODE,  userInfo.company_code);
                }
                else
                {
                    return "denied";
                }
                return "update";
            }
            catch (Exception e)
            {
                return "failed";
            }
        }

        public void recursiveFunction(string MASTER_ACTIVITY_CODE, string ROOT_ACTIVITY_CODE, string CURRENT_ACTIVITY_CODE, string company_code)
        {
            //var GetChildItem = _objectEntity.SqlQuery<string>($@"SELECT MASTER_ACTIVITY_CODE FROM BRD_ACTIVITY WHERE ACTIVITY_CODE ='{ACTIVITY_CODE}'").FirstOrDefault();

            var childItemList = _objectEntity.SqlQuery<ActivityModel>($@"SELECT * FROM BRD_ACTIVITY WHERE MASTER_ACTIVITY_CODE LIKE '%{MASTER_ACTIVITY_CODE}%'").ToList();


            foreach (var item in childItemList)
            {

                var updateQuery = $@"UPDATE BRD_ACTIVITY SET MASTER_ACTIVITY_CODE='{MASTER_ACTIVITY_CODE}.0{item.count}',PRE_ACTIVITY_CODE='{MASTER_ACTIVITY_CODE}'
                                WHERE ACTIVITY_CODE ='{CURRENT_ACTIVITY_CODE}' AND COMPANY_CODE ='{company_code}' AND DELETED_FLAG='N'";

                var GetChildItems = _objectEntity.SqlQuery<string>($@"SELECT MASTER_ACTIVITY_CODE FROM BRD_ACTIVITY WHERE ACTIVITY_CODE ='{item.ACTIVITY_CODE}'").FirstOrDefault();

                var childCount = _objectEntity.SqlQuery<int>($@"SELECT count(*) FROM BRD_ACTIVITY WHERE MASTER_ACTIVITY_CODE LIKE '%{GetChildItems}%'").SingleOrDefault();

                var masterCode = GetChildItems;
                var childActivityCode = "";
                var childPreActivityCode = "";


                if (childCount > 0)
                    recursiveFunction(masterCode, childPreActivityCode, childActivityCode, company_code);

            }

        }

        public List<ActivityModel> getBrandingActivityList(User userInfo)
        {
            string selectQuery = $@"SELECT ACTIVITY_CODE,ACTIVITY_EDESC,ACTIVITY_TYPE,PARENT_ACTIVITY_CODE,MASTER_ACTIVITY_CODE,PRE_ACTIVITY_CODE,GROUP_ACTIVITY_FLAG,REMARKS,DELETED_FLAG FROM BRD_ACTIVITY WHERE  COMPANY_CODE ={userInfo.company_code} AND DELETED_FLAG='N'";
            string Query = $@"SELECT ACTIVITY_CODE,
                       ACTIVITY_EDESC,
                       ACTIVITY_TYPE,
                       NVL(PARENT_ACTIVITY_CODE,0)PARENT_ACTIVITY_CODE,
                       MASTER_ACTIVITY_CODE,
                       PRE_ACTIVITY_CODE,
                       GROUP_ACTIVITY_FLAG,
                       REMARKS,
                       DELETED_FLAG
                       --,SYS_CONNECT_BY_PATH(ACTIVITY_EDESC, ' : ') AS PATH
                       FROM BRD_ACTIVITY
                       WHERE COMPANY_CODE = '{userInfo.company_code}' AND DELETED_FLAG = 'N'
                       --START WITH MASTER_ACTIVITY_CODE = '0'
                       --CONNECT BY PRIOR MASTER_ACTIVITY_CODE = PARENT_ACTIVITY_CODE ORDER BY ACTIVITY_EDESC ASC";
            var result = _objectEntity.SqlQuery<ActivityModel>(Query).ToList();
            return result;

        }

        public string deleteActivity(ActivityModel model, User userInfo)
        {
            var deleteQuery = $@"UPDATE BRD_ACTIVITY SET DELETED_FLAG ='Y' WHERE ACTIVITY_CODE ='{model.ACTIVITY_CODE}' AND COMPANY_CODE ='{userInfo.company_code}'";
            var result = _objectEntity.ExecuteSqlCommand(deleteQuery);
            _objectEntity.SaveChanges();
            return "deleted";
        }

        public List<ContractModel> getAllContractList(string type, User userInfo)
        {
            string selectQuery = string.Empty;
            string filter = string.Empty;

            if (type == "S")
                filter = "AND CONTRACT_TYPE='VISIT' AND AMOUNT_TYPE='SCHEME_ITEM'";
            else if (type == "E")
                filter = "AND AMOUNT_TYPE='EVENT'";
            else
                filter = "AND AMOUNT_TYPE NOT IN ('SCHEME_ITEM','EVENT')";



            //$@"SELECT  SI.ITEM_CODE,CR.RESELLER_CODE, wm_concat(CA.AREA_CODE) AS ITEM_CODE_STRING,BC.CONTRACT_CODE, BCI.ITEM_CODE AS PRODUCT_ITEM_CODE,BCI.QUANTITY AS PRODUCT_QUANTITY,BCI.MU_CODE AS MU_CODE,BC.SUPPLIER_CODE ,BC.PAYMENT_DATE,BC.CUSTOMER_CODE,BC.SET_CODE ,BC.BRANDING_TYPE,BC.CONTRACT_EDESC ,BC.CONTRACT_NDESC ,BC.BRAND_CODE ,BC.SPROVIDER_CODE ,BC.START_DATE ,BC.END_DATE ,BC.AREA_CODE ,BC.CONTRACT_TYPE ,BC.AMOUNT_TYPE ,BC.AMOUNT ,
            //                            BC.PAYMENT_DATE ,BC.ADVANCE_AMOUNT ,BC.CONTRACTOR_NAME ,BC.CONTRACTOR_ADDRESS ,BC.CONTRACTOR_EMAIL ,BC.CONTRACTOR_PHONE ,BC.CONTRACTOR_MOBILE ,BC.CONTRACTOR_DESIGNATION ,BC.CONTRACTOR_PAN_NO ,BC.CONTRACTOR_VAT_NO ,
            //                            BC.OWNER_NAME ,BC.OWNER_ADDRESS ,BC.OWNER_EMAIL ,BC.OWNER_PHONE ,BC.OWNER_MOBILE ,BC.OWNER_COMPANY_NAME ,BC.OWNER_PAN_NO ,BC.OWNER_VAT_NO ,BC.JOB_ORDER_NO ,BC.DESCRIPTION ,BC.CONTRACT_FILES ,BC.REMARKS 
            //                           FROM BRD_CONTRACT BC
            //                          FULL JOIN BRD_CONTRACT_SCHEME_ITEM SI ON SI.COMPANY_CODE = BC.COMPANY_CODE AND SI.CONTRACT_CODE = BC.CONTRACT_CODE
            //                          FULL JOIN BRD_CONTRACT_RESELLER CR ON CR.COMPANY_CODE =BC.COMPANY_CODE  AND CR.CONTRACT_CODE = BC.CONTRACT_CODE
            //                            FULL JOIN BRD_CONTRACT_AREA CA ON CA.COMPANY_CODE = BC.COMPANY_CODE AND CA.CONTRACT_CODE = BC.CONTRACT_CODE
            //                            FULL JOIN BRD_CONTRACT_ITEMS BCI ON BCI.COMPANY_CODE = BC.COMPANY_CODE AND BCI.CONTRACT_CODE = BC.CONTRACT_CODE
            //                          WHERE BC.COMPANY_CODE ='{userInfo.company_code}' AND BC.DELETED_FLAG='N' GROUP BY SI.ITEM_CODE,CR.RESELLER_CODE,BC.CONTRACT_CODE,BCI.ITEM_CODE,BCI.QUANTITY,BCI.MU_CODE,BC.SUPPLIER_CODE ,BC.PAYMENT_DATE,BC.CUSTOMER_CODE,BC.SET_CODE ,BC.BRANDING_TYPE,BC.CONTRACT_EDESC ,BC.CONTRACT_NDESC ,BC.BRAND_CODE ,BC.SPROVIDER_CODE ,BC.START_DATE ,BC.END_DATE ,BC.AREA_CODE ,BC.CONTRACT_TYPE ,BC.AMOUNT_TYPE ,BC.AMOUNT ,
            //                            BC.PAYMENT_DATE ,BC.ADVANCE_AMOUNT ,BC.CONTRACTOR_NAME ,BC.CONTRACTOR_ADDRESS ,BC.CONTRACTOR_EMAIL ,BC.CONTRACTOR_PHONE ,BC.CONTRACTOR_MOBILE ,BC.CONTRACTOR_DESIGNATION ,BC.CONTRACTOR_PAN_NO ,BC.CONTRACTOR_VAT_NO ,
            //                            BC.OWNER_NAME ,BC.OWNER_ADDRESS ,BC.OWNER_EMAIL ,BC.OWNER_PHONE ,BC.OWNER_MOBILE ,BC.OWNER_COMPANY_NAME ,BC.OWNER_PAN_NO ,BC.OWNER_VAT_NO ,BC.JOB_ORDER_NO ,BC.DESCRIPTION ,BC.CONTRACT_FILES ,BC.REMARKS 
            //                           ORDER BY UPPER(BC.CONTRACT_EDESC) ASC";

            selectQuery = $@"SELECT BC.CONTRACT_CODE,BC.SUPPLIER_CODE,BC.PAYMENT_DATE,BC.SET_CODE,BC.BRANDING_TYPE,BC.CONTRACT_EDESC,BC.CONTRACT_NDESC,BC.IS_ROUTE_PLAN,BC.HAS_GIFT_NAME,--BC.CUSTOMER_CODE
                            BC.SPROVIDER_CODE,BC.START_DATE,BC.END_DATE,BC.CONTRACT_TYPE,BC.AMOUNT_TYPE,BC.AMOUNT,BC.PAYMENT_DATE,BC.ADVANCE_AMOUNT,
                            BC.CONTRACTOR_NAME,BC.CONTRACTOR_ADDRESS,BC.CONTRACTOR_EMAIL,BC.CONTRACTOR_PHONE,BC.CONTRACTOR_MOBILE,BC.CONTRACTOR_DESIGNATION,
                            BC.CONTRACTOR_PAN_NO,BC.CONTRACTOR_VAT_NO,BC.OWNER_NAME,BC.OWNER_ADDRESS,BC.OWNER_EMAIL,BC.OWNER_PHONE,BC.OWNER_MOBILE,
                            BC.OWNER_COMPANY_NAME,BC.OWNER_PAN_NO,BC.OWNER_VAT_NO,BC.JOB_ORDER_NO,BC.DESCRIPTION,BC.CONTRACT_FILES,BC.REMARKS,
                            (SELECT WM_CONCAT (ITEM_CODE) FROM BRD_CONTRACT_SCHEME_ITEM WHERE CONTRACT_CODE = BC.CONTRACT_CODE) ITEM_CODE,
                            (SELECT WM_CONCAT (RESELLER_CODE) FROM BRD_CONTRACT_RESELLER WHERE CONTRACT_CODE = BC.CONTRACT_CODE) RESELLER_CODE_STRING,
                            (SELECT WM_CONCAT (AREA_CODE) FROM BRD_CONTRACT_AREA WHERE CONTRACT_CODE = BC.CONTRACT_CODE) ITEM_CODE_STRING,
                            (SELECT WM_CONCAT(CUSTOMER_CODE) FROM BRD_CONTRACT_CUSTOMER WHERE CONTRACT_CODE= BC.CONTRACT_CODE) CUSTOMER_CODE,
                            (SELECT WM_CONCAT(BRAND_CODE) FROM BRD_CONTRACT_SCHEME_BRAND WHERE CONTRACT_CODE= BC.CONTRACT_CODE) BRAND_CODE_STRING
                    FROM BRD_CONTRACT BC
                    WHERE BC.COMPANY_CODE = '{userInfo.company_code}' AND BC.DELETED_FLAG = 'N'
                             {filter}
                    ORDER BY BC.CONTRACT_CODE";
            var result = _objectEntity.SqlQuery<ContractModel>(selectQuery).ToList();
            foreach (var item in result)
            {
                item.PRODUCT_ITEMS = _objectEntity.SqlQuery<ItemUnitModel>($"SELECT ITEM_CODE,TO_NUMBER(QUANTITY) QUANTITY,MU_CODE,COMPANY_CODE FROM BRD_CONTRACT_ITEMS WHERE CONTRACT_CODE='{item.CONTRACT_CODE}' AND COMPANY_CODE='{userInfo.company_code}' ").ToList();
            }
            return result;
        }

        public List<SupplierModel> GetSupplierList(User userInfo)
        {
            string selectQuery = $@"SELECT SUPPLIER_CODE,SUPPLIER_EDESC FROM IP_SUPPLIER_SETUP WHERE COMPANY_CODE ='{userInfo.company_code}'";
            var result = _objectEntity.SqlQuery<SupplierModel>(selectQuery).ToList();
            return result;
        }

        public List<CustomerModel> GetCustomerList(User userInfo)
        {
            string selectQuery = $@"SELECT  CUSTOMER_CODE,CUSTOMER_EDESC FROM SA_CUSTOMER_SETUP WHERE COMPANY_CODE ='{userInfo.company_code}'";
            var result = _objectEntity.SqlQuery<CustomerModel>(selectQuery).ToList();
            return result;
        }

        public List<SetQstModel> GetQuestionList(User userInfo)
        {
            string selectQuery = $@"SELECT SET_CODE, TITLE FROM DIST_QA_SET --WHERE QA_TYPE ='S'";
            var result = _objectEntity.SqlQuery<SetQstModel>(selectQuery).ToList();
            return result;
        }

        public List<AreaModel> GetAreaList(User userInfo)
        {
            string selectQuery = $@"SELECT  AREA_CODE,AREA_NAME FROM DIST_AREA_MASTER WHERE COMPANY_CODE ='{userInfo.company_code}'";
            var result = _objectEntity.SqlQuery<AreaModel>(selectQuery).ToList();
            return result;
        }

        public List<ResellerModel> GetBrdReseller(User userInfo)
        {
            string selectQuery = $@"SELECT RESELLER_CODE,RESELLER_NAME,AREA_CODE FROM DIST_RESELLER_MASTER WHERE COMPANY_CODE = '{userInfo.company_code}' AND DELETED_FLAG ='N'";
            var result = _objectEntity.SqlQuery<ResellerModel>(selectQuery).ToList();
            return result;

        }

        public List<BrdItemModel> GetBrdItem(User userInfo)
        {
            string selectQuery = $@"SELECT ITEM_CODE,ITEM_EDESC FROM  IP_ITEM_MASTER_SETUP WHERE COMPANY_CODE ='{userInfo.company_code}' AND DELETED_FLAG='N'";
            var result = _objectEntity.SqlQuery<BrdItemModel>(selectQuery).ToList();
            return result;
        }

        public List<ItemUnitModel> GetItemUnit(User userInfo)
        {
            string query = $@"SELECT MU_CODE,MU_EDESC FROM IP_MU_CODE WHERE COMPANY_CODE = '{userInfo.company_code}' AND DELETED_FLAG ='N'";
            var data = _objectEntity.SqlQuery<ItemUnitModel>(query).ToList();
            return data;
        }


        public string saveContract(ContractModel modal, User userinfo)
        {

            using (var transaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {


                    if (modal.IS_ROUTE_PLAN == "True")
                    {
                        modal.IS_ROUTE_PLAN = "Y";
                    }
                    else
                    {
                        modal.IS_ROUTE_PLAN = "N";
                    }

                    if (modal.HAS_GIFT_NAME == "True")
                    {
                        modal.HAS_GIFT_NAME = "Y";
                    }
                    else
                    {
                        modal.HAS_GIFT_NAME = "N";

                    }

                    var MaxId = _objectEntity.SqlQuery<int>($@"SELECT (NVL(MAX(CONTRACT_CODE),0)+1) MAXID FROM BRD_CONTRACT").FirstOrDefault();
                    var Question = string.Empty;
                    if (modal.SET_CODE == null)
                    {
                        Question = "''";

                    }
                    else
                    {
                        Question = modal.SET_CODE.ToString();
                    }
                    var insertQuery = string.Format(@"INSERT INTO BRD_CONTRACT(CONTRACT_CODE,CONTRACT_EDESC,CONTRACT_NDESC,SUPPLIER_CODE,CUSTOMER_CODE,START_DATE,END_DATE,CONTRACT_TYPE,AMOUNT_TYPE,AMOUNT,PAYMENT_DATE,
                                                        ADVANCE_AMOUNT,CONTRACTOR_NAME,CONTRACTOR_ADDRESS,OWNER_EMAIL,OWNER_PHONE,CONTRACTOR_DESIGNATION,OWNER_COMPANY_NAME,OWNER_PAN_NO,OWNER_VAT_NO,
                                                        OWNER_NAME,OWNER_ADDRESS,CREATED_DATE,COMPANY_CODE,BRANCH_CODE,DELETED_FLAG,BRAND_CODE,DESCRIPTION,SET_CODE,BRANDING_TYPE,IS_ROUTE_PLAN,HAS_GIFT_NAME) VALUES({0},'{1}','{2}','{3}','{4}',TO_DATE('{5}', 'MM/dd/yyyy'),TO_DATE('{6}' ,'MM/dd/yyyy'),'{7}','{8}','{9}',TO_DATE('{10}','MM/dd/yyyy'),
                                                       '{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}',TO_DATE(SYSDATE),'{23}','{24}','{25}','{26}','{27}',{28},'{29}','{30}','{31}')", MaxId, modal.CONTRACT_EDESC, modal.CONTRACT_EDESC, modal.SUPPLIER_CODE, modal.CUSTOMER_CODE, modal.START_DATE.ToString("MM/dd/yyyy"), modal.END_DATE.ToString("MM/dd/yyyy"), modal.CONTRACT_TYPE, modal.AMOUNT_TYPE, modal.AMOUNT, modal.PAYMENT_DATE.ToString("MM/dd/yyyy"), modal.ADVANCE_AMOUNT, modal.CONTRACTOR_NAME,
                                                        modal.CONTRACTOR_ADDRESS, modal.OWNER_EMAIL, modal.OWNER_PHONE, modal.CONTRACTOR_DESIGNATION, modal.OWNER_COMPANY_NAME, modal.OWNER_PAN_NO, modal.OWNER_VAT_NO, modal.OWNER_VAT_NO, modal.OWNER_NAME, modal.OWNER_ADDRESS, userinfo.company_code, userinfo.branch_code, 'N', ' ', modal.DESCRIPTION, Question, modal.BRANDING_TYPE,modal.IS_ROUTE_PLAN, modal.HAS_GIFT_NAME);

                    _objectEntity.ExecuteSqlCommand(insertQuery);


                    foreach (var data in modal.BRAND_CODE)
                    {
                        var InsertItem = $@"INSERT INTO BRD_CONTRACT_SCHEME_BRAND (CONTRACT_CODE,BRAND_CODE,COMPANY_CODE) VALUES({MaxId},'{data}','{userinfo.company_code}') ";
                        _objectEntity.ExecuteSqlCommand(InsertItem);
                    }
                    foreach (var data in modal.ITEM_CODES)
                    {
                        var InsertItem = $@"INSERT INTO BRD_CONTRACT_SCHEME_ITEM (CONTRACT_CODE,ITEM_CODE,COMPANY_CODE) VALUES({MaxId},'{data}','{userinfo.company_code}') ";
                        _objectEntity.ExecuteSqlCommand(InsertItem);
                    }
                    foreach (var data in modal.AREA_CODE)
                    {
                        var InsertArea = $@"INSERT INTO BRD_CONTRACT_AREA (CONTRACT_CODE,AREA_CODE,COMPANY_CODE) VALUES({MaxId},'{data}','{userinfo.company_code}') ";
                        _objectEntity.ExecuteSqlCommand(InsertArea);

                    }

                    foreach (var item in modal.PRODUCT_ITEMS)
                    {
                        var insertItemQuery = $@"INSERT INTO BRD_CONTRACT_ITEMS (CONTRACT_CODE,ITEM_CODE,QUANTITY,MU_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                            VALUES({MaxId},'{item.ITEM_CODE}','{item.QUANTITY}','{item.MU_CODE}','{modal.REMARKS}','{userinfo.company_code}','{userinfo.branch_code}','{userinfo.User_id}',SYSDATE,'N') ";
                        if (!string.IsNullOrWhiteSpace(item.ITEM_CODE))
                            _objectEntity.ExecuteSqlCommand(insertItemQuery);
                    }

                    foreach (var item in modal.CUSTOMERS)
                    {
                        var insertItemQuery = $@"INSERT INTO BRD_CONTRACT_CUSTOMER (CONTRACT_CODE,CUSTOMER_CODE,COMPANY_CODE,BRANCH_CODE)
                                            VALUES({MaxId},'{item}','{userinfo.company_code}','{userinfo.branch_code}') ";
                        _objectEntity.ExecuteSqlCommand(insertItemQuery);
                    }

                    foreach (var reseller in modal.RESELLER_CODE)
                    {
                        var insertReseller = $@"INSERT INTO BRD_CONTRACT_RESELLER(CONTRACT_CODE,RESELLER_CODE,COMPANY_CODE,BRANCH_CODE)VALUES({MaxId},'{reseller}','{userinfo.company_code}','{userinfo.branch_code}')";
                        _objectEntity.ExecuteSqlCommand(insertReseller);
                    }

                    _objectEntity.SaveChanges();
                    transaction.Commit();
                    return "success";
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return "failed";
                }
            }

        }

        //public void insertReseller(ContractModel modal, User userinfo,int MaxId)
        //{
        //    foreach (var reseller in modal.RESELLER_CODE)
        //    {
        //        var insertReseller = $@"INSERT INTO BRD_CONTRACT_RESELLER(CONTRACT_CODE,RESELLER_CODE,COMPANY_CODE,BRANCH_CODE)VALUES({MaxId},'{reseller}','{userinfo.company_code}','{userinfo.company_code}')";
        //        _objectEntity.ExecuteSqlCommand(insertReseller);
        //    }
        //    _objectEntity.SaveChanges();
        //}

        public string deleteContract(ContractModel modal, User userInfo)
        {

            try
            {
                var deleteQuery = $@"UPDATE BRD_CONTRACT SET DELETED_FLAG ='Y' WHERE CONTRACT_CODE ={modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'";
                var executeQuery = _objectEntity.ExecuteSqlCommand(deleteQuery);
                _objectEntity.SaveChanges();
                return "success";
            }
            catch (Exception e)
            {
                return "failed";
            }

        }

        public string updateContract(ContractModel modal, User userInfo)
        {
            using (var transaction = _objectEntity.Database.BeginTransaction())
            {
                try
                {

                    if (modal.IS_ROUTE_PLAN == "True")
                    {
                        modal.IS_ROUTE_PLAN = "Y";
                    }
                    else
                    {
                        modal.IS_ROUTE_PLAN = "N";

                    }

                    if (modal.HAS_GIFT_NAME == "True")
                    {
                        modal.HAS_GIFT_NAME = "Y";
                    }
                    else
                    {
                        modal.HAS_GIFT_NAME = "N";

                    }




                    var updateQuery = string.Format(@"UPDATE BRD_CONTRACT SET CONTRACT_EDESC='{0}',SUPPLIER_CODE='{1}',CUSTOMER_CODE='{2}',START_DATE=TO_DATE('{3}','MM/dd/yyyy'),END_DATE=TO_DATE('{4}','MM/dd/yyyy'),CONTRACT_TYPE='{5}',AMOUNT_TYPE='{6}',AMOUNT='{7}',PAYMENT_DATE=TO_DATE('{8}','MM/dd/YYYY'),
ADVANCE_AMOUNT='{9}',CONTRACTOR_NAME='{10}',CONTRACTOR_ADDRESS='{11}',OWNER_EMAIL='{12}',OWNER_PHONE='{13}',CONTRACTOR_DESIGNATION='{14}',OWNER_COMPANY_NAME='{15}',OWNER_PAN_NO='{16}',OWNER_VAT_NO='{17}',
OWNER_NAME='{18}',OWNER_ADDRESS='{19}',BRAND_CODE='{20}', DESCRIPTION='{21}',SET_CODE='{22}',BRANDING_TYPE='{23}',IS_ROUTE_PLAN='{24}',HAS_GIFT_NAME='{25}',CREATED_DATE=TO_DATE(SYSDATE) WHERE CONTRACT_CODE IN {26} AND COMPANY_CODE IN {27}", modal.CONTRACT_EDESC, modal.SUPPLIER_CODE, modal.CUSTOMER_CODE, modal.START_DATE.ToString("MM/dd/yyyy"), modal.END_DATE.ToString("MM/dd/yyyy"), modal.CONTRACT_TYPE, modal.AMOUNT_TYPE, modal.AMOUNT, modal.PAYMENT_DATE.ToString("MM/dd/yyyy"),
    modal.ADVANCE_AMOUNT, modal.CONTRACTOR_NAME, modal.CONTRACTOR_ADDRESS, modal.OWNER_EMAIL, modal.OWNER_PHONE, modal.CONTRACTOR_DESIGNATION, modal.OWNER_COMPANY_NAME, modal.OWNER_PAN_NO, modal.OWNER_VAT_NO,
    modal.OWNER_NAME, modal.OWNER_ADDRESS, ' ', modal.DESCRIPTION, modal.SET_CODE, modal.BRANDING_TYPE,modal.IS_ROUTE_PLAN, modal.HAS_GIFT_NAME, modal.CONTRACT_CODE, userInfo.company_code);
                    var executeUpdateQuery = _objectEntity.ExecuteSqlCommand(updateQuery);
                    //UpdateMultipleItem(modal, userInfo);

                    _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_AREA WHERE CONTRACT_CODE='{modal.CONTRACT_CODE}' AND COMPANY_CODE ='{userInfo.company_code}'");
                    foreach (var data in modal.AREA_CODE)
                    {
                        string updateArea = $@"INSERT INTO BRD_CONTRACT_AREA (CONTRACT_CODE,AREA_CODE,COMPANY_CODE) VALUES({modal.CONTRACT_CODE},'{data}','{userInfo.company_code}') ";
                        _objectEntity.ExecuteSqlCommand(updateArea);
                    }

                    //Brand
                    _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_SCHEME_BRAND WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'");
                    foreach (var data in modal.BRAND_CODE)
                    {
                        var InsertItem = $@"INSERT INTO BRD_CONTRACT_SCHEME_BRAND (CONTRACT_CODE,BRAND_CODE,COMPANY_CODE) VALUES('{modal.CONTRACT_CODE}','{data}','{userInfo.company_code}') ";
                        _objectEntity.ExecuteSqlCommand(InsertItem);
                    }

                    //Items
                    _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_SCHEME_ITEM WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'");
                    foreach (var data in modal.ITEM_CODES)
                    {
                        var InsertItem = $@"INSERT INTO BRD_CONTRACT_SCHEME_ITEM (CONTRACT_CODE,ITEM_CODE,COMPANY_CODE) VALUES('{modal.CONTRACT_CODE}','{data}','{userInfo.company_code}') ";
                        _objectEntity.ExecuteSqlCommand(InsertItem);
                    }

                    //string updateScheme = $@"UPDATE BRD_CONTRACT_SCHEME_ITEM SET ITEM_CODE ='{modal.ITEM_CODE}' WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'";
                    //_objectEntity.ExecuteSqlCommand(updateScheme);

                    _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_RESELLER WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'");

                    foreach (var reseller in modal.RESELLER_CODE)
                    {
                        var Query = $@"INSERT INTO BRD_CONTRACT_RESELLER(CONTRACT_CODE,RESELLER_CODE,COMPANY_CODE,BRANCH_CODE)VALUES({modal.CONTRACT_CODE},'{reseller}','{userInfo.company_code}','{userInfo.branch_code}')";
                        _objectEntity.ExecuteSqlCommand(Query);
                    }

                    _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_CUSTOMER WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'");
                    foreach (var item in modal.CUSTOMERS)
                    {
                        var insertItemQuery = $@"INSERT INTO BRD_CONTRACT_CUSTOMER (CONTRACT_CODE,CUSTOMER_CODE,COMPANY_CODE,BRANCH_CODE)
                                            VALUES({modal.CONTRACT_CODE},'{item}','{userInfo.company_code}','{userInfo.branch_code}') ";
                        _objectEntity.ExecuteSqlCommand(insertItemQuery);
                    }

                    _objectEntity.ExecuteSqlCommand($"DELETE FROM BRD_CONTRACT_ITEMS WHERE CONTRACT_CODE='{modal.CONTRACT_CODE}' AND COMPANY_CODE ='{userInfo.company_code}'");
                    foreach (var item in modal.PRODUCT_ITEMS)
                    {
                        var insertItemQuery = $@"INSERT INTO BRD_CONTRACT_ITEMS (CONTRACT_CODE,ITEM_CODE,QUANTITY,MU_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                            VALUES({modal.CONTRACT_CODE},'{item.ITEM_CODE}','{item.QUANTITY}','{item.MU_CODE}','{modal.REMARKS}','{userInfo.company_code}','{userInfo.branch_code}','{userInfo.User_id}',SYSDATE,'N') ";
                        if (!string.IsNullOrWhiteSpace(item.ITEM_CODE))
                            _objectEntity.ExecuteSqlCommand(insertItemQuery);
                    }


                    transaction.Commit();
                    return "success";
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return "failed";
                }
            }

        }


        //public void UpdateMultipleItem(ContractModel modal, User userInfo)
        //{
        //    _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_AREA WHERE CONTRACT_CODE='{modal.CONTRACT_CODE}' AND COMPANY_CODE ='{userInfo.company_code}'");
        //    foreach (var data in modal.AREA_CODE)
        //    {
        //        string updateArea = $@"INSERT INTO BRD_CONTRACT_AREA (CONTRACT_CODE,AREA_CODE,COMPANY_CODE) VALUES({modal.CONTRACT_CODE},'{data}','{userInfo.company_code}') ";
        //        _objectEntity.ExecuteSqlCommand(updateArea);
        //    }

        //    string updateScheme = $@"UPDATE BRD_CONTRACT_SCHEME_ITEM SET ITEM_CODE ='{modal.ITEM_CODE}' WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'";
        //    _objectEntity.ExecuteSqlCommand(updateScheme);

        //        _objectEntity.ExecuteSqlCommand($@"DELETE BRD_CONTRACT_RESELLER WHERE CONTRACT_CODE = {modal.CONTRACT_CODE} AND COMPANY_CODE ='{userInfo.company_code}'");

        //        foreach (var reseller in modal.RESELLER_CODE)
        //        {
        //            var updateQuery = $@"INSERT INTO BRD_CONTRACT_RESELLER(CONTRACT_CODE,RESELLER_CODE,COMPANY_CODE,BRANCH_CODE)VALUES({modal.CONTRACT_CODE},'{reseller}','{userInfo.company_code}','{userInfo.branch_code}')";
        //            _objectEntity.ExecuteSqlCommand(updateQuery);
        //        }

        //}

        public List<ContractModel> getContractSummary(User userInfo)
        {
            var selectQuery = $@"SELECT DISTINCT BC.CONTRACT_CODE,CR.RESELLER_CODE,BC.SUPPLIER_CODE,ISS.SUPPLIER_EDESC ,BC.PAYMENT_DATE,BC.CUSTOMER_CODE,SCS.CUSTOMER_EDESC,BC.SET_CODE ,BC.BRANDING_TYPE,BC.CONTRACT_EDESC ,BC.CONTRACT_NDESC ,BC.BRAND_CODE ,BC.SPROVIDER_CODE ,BC.START_DATE ,BC.END_DATE ,BC.AREA_CODE ,BC.CONTRACT_TYPE ,BC.AMOUNT_TYPE ,BC.AMOUNT ,
                                        BC.PAYMENT_DATE ,BC.ADVANCE_AMOUNT ,BC.CONTRACTOR_NAME ,BC.CONTRACTOR_ADDRESS ,BC.CONTRACTOR_EMAIL ,BC.CONTRACTOR_PHONE ,BC.CONTRACTOR_MOBILE ,BC.CONTRACTOR_DESIGNATION ,BC.CONTRACTOR_PAN_NO ,BC.CONTRACTOR_VAT_NO ,
                                        BC.OWNER_NAME ,BC.OWNER_ADDRESS ,BC.OWNER_EMAIL ,BC.OWNER_PHONE ,BC.OWNER_MOBILE ,BC.OWNER_COMPANY_NAME ,BC.OWNER_PAN_NO ,BC.OWNER_VAT_NO ,BC.JOB_ORDER_NO ,BC.DESCRIPTION ,BC.CONTRACT_FILES ,BC.REMARKS 
                                       FROM BRD_CONTRACT BC
                                      INNER JOIN IP_SUPPLIER_SETUP SS ON SS.SUPPLIER_CODE = BC.SUPPLIER_CODE AND SS.COMPANY_CODE = BC.COMPANY_CODE
                                      INNER JOIN IP_SUPPLIER_SETUP ISS ON ISS.SUPPLIER_CODE = SS.SUPPLIER_CODE AND  ISS.COMPANY_CODE = BC.COMPANY_CODE
                                      LEFT JOIN SA_CUSTOMER_SETUP SCS ON SCS.CUSTOMER_CODE = BC.CUSTOMER_CODE AND SCS.COMPANY_CODE = BC.COMPANY_CODE
                                      FULL JOIN BRD_CONTRACT_RESELLER CR ON CR.COMPANY_CODE =BC.COMPANY_CODE  AND CR.CONTRACT_CODE = BC.CONTRACT_CODE
                                        FULL JOIN BRD_CONTRACT_AREA CA ON CA.COMPANY_CODE = BC.COMPANY_CODE AND CA.CONTRACT_CODE = BC.CONTRACT_CODE
                                      WHERE BC.COMPANY_CODE ='{userInfo.company_code}' AND BC.DELETED_FLAG='N' 
                                       ORDER BY UPPER(BC.CONTRACT_EDESC) ASC";
            var data = _objectEntity.SqlQuery<ContractModel>(selectQuery).ToList();

            foreach (var item in data)
            {
                if (item.SET_CODE != null)
                {
                    item.Questions = GetQuestion(item.SET_CODE.ToString(), userInfo);
                }
            }
            return data;
        }

        public List<BrandTypeModel> GetBrandType(User userInfo)
        {
            var selectQuery = $@"SELECT  ACTIVITY_EDESC AS BRAND_TYPE FROM brd_activity WHERE COMPANY_CODE ='{userInfo.company_code}'";
            var data = _objectEntity.SqlQuery<BrandTypeModel>(selectQuery).ToList();
            return data;
        }

        public List<GeneralQstModel> GetQuestion(string setId, User userInfo)
        {
            //general Questions
            var generalQuery = string.Empty;
            generalQuery = $@"SELECT TO_CHAR(A.QA_CODE) AS QA_CODE, A.QA_TYPE, A.QUESTION FROM DIST_QA_MASTER A
                    WHERE A.DELETED_FLAG = 'N' AND
                    A.SET_CODE = (SELECT B.SET_CODE FROM DIST_QA_SET B WHERE B.COMPANY_CODE=A.COMPANY_CODE AND B.QA_TYPE='S' AND B.SET_CODE='{setId}')
                    AND A.COMPANY_CODE='{userInfo.company_code}'";
            var generalQuestion = _objectEntity.SqlQuery<GeneralQstModel>(generalQuery).ToList();
            string AnswerQuery = string.Empty;
            string[] types = { "MCR", "MCC" };
            foreach (var ques in generalQuestion)
            {
                if (types.Contains(ques.QA_TYPE))
                {
                    AnswerQuery = $@"SELECT ANSWERS FROM DIST_QA_DETAIL WHERE QA_CODE='{ques.QA_CODE}' AND COMPANY_CODE='{userInfo.company_code}'";
                    var answers = _objectEntity.SqlQuery<string>(AnswerQuery).ToList();
                    ques.ANSWERS = answers;
                }
            }
            return generalQuestion.ToList();
        }

        public string SaveContractAnswers(GeneralSaveModel model, User userInfo)
        {
            var result = "200";
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    foreach (var general in model.ANSWERS)
                    {
                        var AnsInsertQuery = $@"INSERT INTO BRD_CONTRACT_ANSWERS (CONTRACT_ID,COLUMN_NAME,ANSWER,QA_CODE,DELETED_FLAG,CREATED_DATE,CREATED_BY,COMPANY_CODE,BRANCH_CODE)
                            VALUES('{model.CONTRACT_CODE}',(SELECT QUESTION FROM DIST_QA_MASTER WHERE QA_CODE='{general.QA_CODE}'),'{general.ANSWER}','{general.QA_CODE}','N',SYSDATE,'{userInfo.User_id}','{userInfo.company_code}','{userInfo.branch_code}')";
                        var row = _objectEntity.ExecuteSqlCommand(AnsInsertQuery);
                        if (row <= 0)
                            throw new Exception("Error Saving");
                    }
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    result = "500";
                    trans.Rollback();
                }
            }
            return result;
        }

        public List<QstReportModel> ContractQueReport(User userInfo)
        {
            var Query = $@"(SELECT BCA.CONTRACT_ID CONTRACT_CODE,BC.CONTRACT_EDESC,BCA.COLUMN_NAME QUESTION,BCA.ANSWER,
                    SAU.LOGIN_EDESC ANSWERED_BY,'Web' ""SOURCE"",BCA.CREATED_DATE
                    FROM BRD_CONTRACT_ANSWERS BCA
                    INNER JOIN BRD_CONTRACT BC ON BCA.CONTRACT_ID = BC.CONTRACT_CODE AND BCA.COMPANY_CODE = BC.COMPANY_CODE
                    LEFT JOIN SC_APPLICATION_USERS SAU ON BCA.CREATED_BY = SAU.USER_NO AND SAU.COMPANY_CODE = BC.COMPANY_CODE
                    WHERE BCA.COMPANY_CODE = '{userInfo.company_code}')
                    UNION ALL
                    (SELECT TO_CHAR(BS.CONTRACT_CODE) CONTRACT_CODE, BC.CONTRACT_EDESC, BSA.COLUMN_NAME QUESTION, BSA.ANSWER,
                    DRM.RESELLER_NAME ANSWERED_BY,'Mobile' ""SOURCE"",BSA.CREATED_DATE
                      FROM BRD_SCHEME_ANSWERS BSA
                    INNER JOIN BRD_SCHEME BS ON BSA.SCHEME_ID = BS.SCHEME_CODE
                    INNER JOIN BRD_CONTRACT BC ON BS.CONTRACT_CODE = BC.CONTRACT_CODE
                    LEFT JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = BSA.RESELLER_CODE AND DRM.COMPANY_CODE = BSA.COMPANY_CODE
                    WHERE BSA.COMPANY_CODE = '{userInfo.company_code}')";
            var data = _objectEntity.SqlQuery<QstReportModel>(Query).ToList();
            return data;
        }

        public List<SchemeReportModel> GetAllSchemeList(ReportFiltersModel model, User userInfo)
        {
            var companyCode = string.Join("','", model.CompanyFilter);
            companyCode = companyCode == "" ? userInfo.company_code : companyCode;

            var selectQuery = $@"SELECT  LU.FULL_NAME AS EmployeeName,DS.ITEM_EDESC AS ItemName,MU_EDESC AS MuName,BC.CONTRACT_EDESC AS ContractName,DRM.RESELLER_NAME AS ResellerName,BS.QUANTITY AS Quantity ,BS.HANDOVER_DATE AS HandOverDate FROM BRD_SCHEME BS
                                    INNER JOIN DIST_RESELLER_MASTER DRM ON DRM.RESELLER_CODE = BS.RESELLER_CODE
                                    INNER JOIN BRD_CONTRACT BC ON BC.CONTRACT_CODE = BS.CONTRACT_CODE
                                    INNER JOIN DIST_LOGIN_USER LU ON LU.USERID = BS.EMPLOYEE_CODE
                                    INNER JOIN IP_ITEM_MASTER_SETUP DS ON DS.ITEM_CODE = BS.ITEM_CODE  AND DS.COMPANY_CODE = BS.COMPANY_CODE
                                    INNER JOIN IP_MU_CODE IMC ON IMC.MU_CODE = BS.MU_CODE AND IMC.COMPANY_CODE = BS.COMPANY_CODE WHERE BS.COMPANY_CODE = '{userInfo.company_code}'";
            var result = _objectEntity.SqlQuery<SchemeReportModel>(selectQuery).ToList();
            return result;
        }

        public ContractModel GetContractSaveObj(ContactImportModel model, User userInfo)
        {
            model.CUSTOMERS = (model.CUSTOMERS == null) ? "" : model.CUSTOMERS;
            model.ITEM_CODES = (model.ITEM_CODES == null) ? "" : model.ITEM_CODES;
            model.AREAS = (model.AREAS == null) ? "" : model.AREAS;
            model.RESELLERS = (model.RESELLERS == null) ? "" : model.RESELLERS;
            model.BRAND = (model.BRAND == null) ? "" : model.BRAND;
            var Supplier = $"SELECT TO_CHAR(SUPPLIER_CODE) FROM IP_SUPPLIER_SETUP WHERE SUPPLIER_EDESC = TRIM(UPPER('{model.SUPPLIER}')) OR SUPPLIER_CODE LIKE '{model.SUPPLIER}' AND COMPANY_CODE = '{userInfo.company_code}'";
            var Customer = $"SELECT DISTINCT TO_CHAR(CUSTOMER_CODE) FROM SA_CUSTOMER_SETUP WHERE UPPER(CUSTOMER_EDESC) IN (TRIM(UPPER('{model.CUSTOMERS.Replace(",", "')),TRIM(UPPER('") }'))) OR CUSTOMER_CODE IN (TRIM(UPPER('{model.CUSTOMERS.Replace(",", "')),TRIM(UPPER('") }'))) AND COMPANY_CODE = '{userInfo.company_code}'";
            var Area = $"SELECT DISTINCT TO_CHAR(AREA_CODE) FROM DIST_AREA_MASTER WHERE UPPER(AREA_NAME) IN (TRIM(UPPER('{model.AREAS.Replace(",", "')),TRIM(UPPER('") }'))) OR AREA_CODE IN (TRIM(UPPER('{model.AREAS.Replace(",", "')),TRIM(UPPER('") }'))) AND COMPANY_CODE = '{userInfo.company_code}'";
            var Reseller = $"SELECT DISTINCT TO_CHAR(RESELLER_CODE) FROM DIST_RESELLER_MASTER WHERE UPPER(RESELLER_NAME) IN (TRIM(UPPER('{model.RESELLERS.Replace(",", "')),TRIM(UPPER('") }'))) OR RESELLER_CODE IN (TRIM(UPPER('{model.RESELLERS.Replace(",", "')),TRIM(UPPER('") }'))) AND COMPANY_CODE = '{userInfo.company_code}'";
            var Question = $"SELECT SET_CODE FROM DIST_QA_SET WHERE UPPER(TITLE) = UPPER(TRIM('{model.QUESTION_SET}')) OR SET_CODE LIKE '{model.QUESTION_SET}' AND QA_TYPE = 'B' AND COMPANY_CODE = '{userInfo.company_code}'";
            var Item = $"SELECT DISTINCT ITEM_CODE FROM  IP_ITEM_MASTER_SETUP WHERE ITEM_EDESC = TRIM(UPPER('{model.ITEM}')) OR ITEM_CODE LIKE '{model.ITEM}' AND COMPANY_CODE = '{userInfo.company_code}'";
            var Itemcodes = $"SELECT DISTINCT TO_CHAR(ITEM_CODE) FROM BRD_CONTRACT_SCHEME_ITEM WHERE UPPER(ITEM_EDESC) IN (TRIM(UPPER('{model.ITEM_CODES.Replace(",", "')),TRIM(UPPER('") }'))) OR ITEM_CODE IN (TRIM(UPPER('{model.ITEM_CODES.Replace(",", "')),TRIM(UPPER('") }'))) AND COMPANY_CODE = '{userInfo.company_code}'";
            var brand = $"SELECT DISTINCT TO_CHAR(BRAND_CODE) FROM BRD_CONTRACT_SCHEME_BRAND WHERE UPPER(BRAND_EDESC) IN (TRIM(UPPER('{model.BRAND.Replace(",", "')),TRIM(UPPER('") }'))) OR ITEM_CODE IN (TRIM(UPPER('{model.BRAND.Replace(",", "')),TRIM(UPPER('") }'))) AND COMPANY_CODE = '{userInfo.company_code}'";
            var result = new ContractModel
            {
                CONTRACT_EDESC = model.CONTRACT_EDESC,
                SUPPLIER_CODE = _objectEntity.SqlQuery<string>(Supplier).FirstOrDefault(),
                BRANDING_TYPE = model.BRANDING_TYPE,
                START_DATE = model.START_DATE,
                END_DATE = model.END_DATE,
                CONTRACT_TYPE = model.CONTRACT_TYPE,
                AMOUNT_TYPE = model.AMOUNT_TYPE,
                AMOUNT = model.AMOUNT,
                PAYMENT_DATE = model.PAYMENT_DATE,
                ADVANCE_AMOUNT = model.ADVANCE_AMOUNT,
                CONTRACTOR_NAME = model.CONTRACTOR_NAME,
                CONTRACTOR_ADDRESS = model.CONTRACTOR_ADDRESS,
                CONTRACTOR_EMAIL = model.CONTRACTOR_EMAIL,
                CONTRACTOR_PHONE = model.CONTRACTOR_PHONE,
                CONTRACTOR_MOBILE = model.CONTRACTOR_MOBILE,
                CONTRACTOR_DESIGNATION = model.CONTRACTOR_DESIGNATION,
                CONTRACTOR_PAN_NO = model.CONTRACTOR_PAN_NO,
                CONTRACTOR_VAT_NO = model.CONTRACTOR_VAT_NO,
                OWNER_NAME = model.OWNER_NAME,
                OWNER_ADDRESS = model.OWNER_ADDRESS,
                OWNER_EMAIL = model.OWNER_EMAIL,
                OWNER_PHONE = model.OWNER_PHONE,
                OWNER_MOBILE = model.OWNER_MOBILE,
                OWNER_COMPANY_NAME = model.OWNER_COMPANY_NAME,
                OWNER_PAN_NO = model.OWNER_PAN_NO,
                OWNER_VAT_NO = model.OWNER_VAT_NO,
                JOB_ORDER_NO = model.JOB_ORDER_NO,
                DESCRIPTION = model.DESCRIPTION,
                CONTRACT_FILES = model.CONTRACT_FILES,
                REMARKS = model.REMARKS,
                APPROVED_FLAG = model.APPROVED_FLAG[0],
                APPROVED_BY = model.APPROVED_BY,
                APPROVED_DATE = model.APPROVED_DATE,
                DELETED_FLAG = model.DELETED_FLAG[0],
                SET_CODE = _objectEntity.SqlQuery<int>(Question).FirstOrDefault(),
                AREA_CODE = _objectEntity.SqlQuery<string>(Area).ToList(),
                CUSTOMERS = _objectEntity.SqlQuery<string>(Customer).ToList(),
                RESELLER_CODE = _objectEntity.SqlQuery<string>(Reseller).ToList(),
                ITEM_CODE = _objectEntity.SqlQuery<string>(Item).FirstOrDefault(),
                ITEM_CODES = _objectEntity.SqlQuery<string>(Itemcodes).ToList(),
                BRAND_CODE = _objectEntity.SqlQuery<string>(brand).ToList(),
            };
            return result;
        }

        public string saveUserSurveyReportWeb(List<WebQueAnsModel> list, WebQueAnsCommonModel common, User userInfo)
        {
            var result = string.Empty;
            try
            {
                var query = $@"INSERT INTO DIST_WEB_QUE_ANS";
                var insqry = string.Empty;
                foreach (var item in list)
                {
                    var qry = $@" SELECT (SELECT (NVL(MAX(WEB_QUE_ANS_ID),0)+1) WEB_QUE_ANS_ID  FROM DIST_WEB_QUE_ANS),'{common.customer_code}','{common.customer_name}','{item.webqakey}','{item.webqavalue}','{userInfo.User_id}',SYSDATE,'N','{userInfo.company_code}' FROM DUAL UNION ALL";
                    insqry += qry;
                }
                if (!string.IsNullOrWhiteSpace(insqry))
                {
                    query += insqry;
                    query= query.Substring(0, query.Length - 10);
                    _objectEntity.ExecuteSqlCommand(query);
                    return result = "Success";
                }
                return result = "Error";
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
