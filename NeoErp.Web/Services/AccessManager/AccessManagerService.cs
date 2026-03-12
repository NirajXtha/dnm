using System;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoErp.Models;

namespace NeoErp.Services.AccessManager
{
    public class AccessManagerService : IAccessManager
    {

        private IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;

        public AccessManagerService(IDbContext dbContext, IWorkContext workContext, NeoErpCoreEntity objectEntity)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }

        public List<DropdownUsers> GetDropdownUsers()
        {
            try
            {
                string userQuery = $@"SELECT  INITCAP(sac.LOGIN_EDESC) LOGIN_EDESC,sac.LOGIN_CODE,sac.USER_NO as USER_NO,sac.GROUP_SKU_FLAG,sac.COMPANY_CODE,
                                   sac.USER_TYPE FROM sc_application_users sac";
                var userData = _dbContext.SqlQuery<DropdownUsers>(userQuery).ToList();
                _logErp.InfoInFile("all user fetched in tree format by : " + _workContext.CurrentUserinformation.login_code);
                return userData;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting User : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<AppModule> GetAppModuleDDL()
        {
            try
            {
                var appModuleQuery = $@"SELECT * FROM MODULE_SETUP";
                var appModule = _dbContext.SqlQuery<AppModule>(appModuleQuery).ToList();
                return appModule;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting app module : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<UserTreeModel> GetAllUserTree()
        {
            try
            {
                string userTreeQuery = $@"SELECT distinct LEVEL,INITCAP(sac.LOGIN_EDESC) as  LOGIN_EDESC,sac.LOGIN_CODE,sac.USER_NO as USER_NO,sac.GROUP_SKU_FLAG,sac.USER_NO as PARENT_USER_CODE,sac.PRE_USER_NO,sac.ABBR_CODE
                                   FROM sc_application_users sac WHERE company_code='{_workContext.CurrentUserinformation.company_code}' and DELETED_FLAG='N'  CONNECT BY PRIOR USER_NO=PRE_USER_NO  START WITH PRE_USER_NO='00'";

                var userTreeNode = new List<UserTreeModel>();
                //string userTreeQuery = $@"SELECT DISTINCT 
                //        INITCAP(LOGIN_EDESC) AS LOGIN_EDESC,
                //        INITCAP(LOGIN_NDESC) AS LOGIN_NDESC,
                //        LOGIN_CODE,
                //        USER_NO, 
                //        PRE_USER_NO,
                //        GROUP_SKU_FLAG
                //        FROM SC_APPLICATION_USERS
                //        WHERE DELETED_FLAG = 'N'
                //        CONNECT BY PRIOR USER_NO = PRE_USER_NO
                //        ORDER BY PRE_USER_NO";

                var userTreeData = _dbContext.SqlQuery<UserTreeModel>(userTreeQuery).ToList();

                 // var customerNode = generateCustomerTree(userTreeData, userTreeNode, "00");
                _logErp.InfoInFile("all user fetched in tree format by : " + _workContext.CurrentUserinformation.login_code);
                 return userTreeData;
                //return customerNode;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while generating user tree :" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private List<UserTreeModel> generateCustomerTree(List<UserTreeModel> model, List<UserTreeModel> userNodes, string preItemCode)
        {
            foreach (var users in model.Where(x => Convert.ToDecimal(x.PRE_USER_NO) == Convert.ToDecimal(preItemCode)))
            {
                var customerNodesChild = new List<UserTreeModel>();
                userNodes.Add(new UserTreeModel()
                {
                    LEVEL = users.LEVEL,
                    LOGIN_EDESC = users.LOGIN_EDESC,
                    LOGIN_CODE = users.LOGIN_CODE,
                    USER_NO = users.USER_NO,
                    PRE_USER_NO = users.PRE_USER_NO,
                    GROUP_SKU_FLAG = users.GROUP_SKU_FLAG,
                    hasBranch = users.GROUP_SKU_FLAG == "G" ? false : false,
                    ABBR_CODE = users.ABBR_CODE,
                    PARENT_USER_CODE = users.PARENT_USER_CODE,
                    Items = users.GROUP_SKU_FLAG == "G" ? generateCustomerTree(model, customerNodesChild, users.PARENT_USER_CODE.ToString()) : null,
                });
            }
            return userNodes;
        }

        public List<AvailableControl> GetAvailableControls(string userNo,string selectedControl)
        {
            try
            {
                var availableControlList = new List<AvailableControl>();
                var parentComp = 0;
                var parentCtrl = 0;
                if (userNo == "0")
                {
                    var company = GetCompanyAsControl();
                    var control = GetAvailableDocManagerAsControl();
                    var masterDefinition = GetMasterDefinitionCtrlAsConrol();
                    var moduleDefCntrl = GetAvailableModuleCntrl(userNo,selectedControl);
                    var masterSetupViewCntrl = GetMasterSetupListCntrl(userNo, selectedControl);

                    foreach (var cm in company)
                    {
                        availableControlList.Add(new AvailableControl { CONTROL_CODE = cm.COMPANY_CODE, CONTROL_NAME =cm.COMPANY_EDESC, REPORTSTO = null, CONTROL_HEADING = "COMPANY AND BRANCH" });
                        foreach (var br in cm.CompanyBranch)
                        {
                            availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, CONTROL_NAME = br.BRANCH_EDESC, REPORTSTO = cm.COMPANY_CODE, CONTROL_HEADING = "COMPANY AND BRANCH" });
                        }

                    }

                    foreach (var ctl in control)
                    {
                        foreach (var ct in ctl.CHILDREN)
                        {
                            if (parentCtrl == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = ctl.FORM_CODE, CONTROL_NAME = ctl.FORM_EDESC, REPORTSTO = null, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                            else availableControlList.Add(new AvailableControl { CONTROL_CODE = ct.FORM_CODE, CONTROL_NAME = ct.FORM_EDESC, REPORTSTO = ctl.FORM_CODE, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                            parentCtrl++;
                        }

                    }

                    availableControlList.AddRange(masterDefinition);
                    availableControlList.AddRange(moduleDefCntrl);
                    availableControlList.AddRange(masterSetupViewCntrl);
                    return availableControlList;
                }
                else
                {
                    if (selectedControl == "" || selectedControl==null || selectedControl=="null")
                    {
                        var company = GetCompanyAsControl(userNo);
                        var control = GetAvailableDocManagerAsControl(userNo);
                        var masterDefinition = GetMasterDefinitionCtrlAsConrol(userNo);
                        var modulecontrol = GetAvailableModuleCntrl(userNo, selectedControl);
                        var masterSetupControl = GetMasterSetupListCntrl(userNo, selectedControl);


                        foreach (var cm in company)
                        {
                            availableControlList.Add(new AvailableControl { CONTROL_CODE = cm.COMPANY_CODE, CONTROL_NAME = cm.COMPANY_EDESC, REPORTSTO = null, CONTROL_HEADING = "COMPANY AND BRANCH", NEW = true, VIEW = true, EDIT = true, RECYCLE = true, POSTPRINT = true, UNPOST = true, CHECK = true, VERIFY = true });
                            foreach (var br in cm.CompanyBranch)
                            {
                                 availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, CONTROL_NAME = br.BRANCH_EDESC, REPORTSTO = cm.COMPANY_CODE, CONTROL_HEADING = "COMPANY AND BRANCH", NEW = true, VIEW = true, EDIT = true, RECYCLE = true, POSTPRINT = true, UNPOST = true, CHECK = true, VERIFY = true });
                            }

                        }

                        foreach (var ctl in control)
                        {
                            
                            availableControlList.Add(new AvailableControl { CONTROL_CODE = ctl.FORM_CODE, CONTROL_NAME = ctl.FORM_EDESC, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)",NEW=true,VIEW=true,EDIT=true,RECYCLE=true,POSTPRINT=true,UNPOST=true,CHECK=true,VERIFY=true });

                        }

                        availableControlList.AddRange(masterDefinition);
                        availableControlList.AddRange(modulecontrol);
                        availableControlList.AddRange(masterSetupControl);
                       
                        return availableControlList;

                    }
                    else if (selectedControl == "CNB")
                    {
                        var company = GetCompanyAsControl();
                        foreach (var cm in company)
                        {
                            foreach (var br in cm.CompanyBranch)
                            {
                                if (parentComp == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, CONTROL_NAME = br.BRANCH_EDESC, REPORTSTO = null, CONTROL_HEADING = "COMPANY AND BRANCH" });
                                else availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, CONTROL_NAME = br.BRANCH_EDESC, REPORTSTO = cm.COMPANY_CODE, CONTROL_HEADING = "COMPANY AND BRANCH" });
                                parentComp++;
                            }

                        }
                        return availableControlList;

                    }
                    else if (selectedControl == "DMT")
                    {
                        var control = GetAvailableDocManagerAsControl();
                        foreach (var ctl in control)
                        {
                            foreach (var ct in ctl.CHILDREN)
                            {
                                if (parentCtrl == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = ctl.FORM_CODE, CONTROL_NAME = ctl.FORM_EDESC, REPORTSTO = null, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                                else availableControlList.Add(new AvailableControl { CONTROL_CODE = ct.FORM_CODE, CONTROL_NAME = ct.FORM_EDESC, REPORTSTO = ctl.FORM_CODE, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                                parentCtrl++;
                            }

                        }
                        return availableControlList;

                    }
                    else if (selectedControl == "MDC")
                    {
                        var masterDefinition = GetMasterDefinitionCtrlAsConrol();
                        availableControlList.AddRange(masterDefinition);
                        return availableControlList;
                    }
                    else
                    {
                        return availableControlList;
                    }
                }
               
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all controls : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private List<CompanyModal> GetCompanyAsControl(string userNo="0")
        {
            try
            {
                if (userNo == "0")
                {
                    var companyQuery = $@"SELECT * FROM COMPANY_SETUP CS WHERE CS.DELETED_FLAG='N'";
                    var companyData = _dbContext.SqlQuery<CompanyModal>(companyQuery).ToList();
                    foreach (var cd in companyData)
                    {
                        var bQuery = $@"SELECT * FROM FA_BRANCH_SETUP FBS WHERE FBS.DELETED_FLAG='N' AND FBS.COMPANY_CODE=" + cd.COMPANY_CODE;
                        cd.CompanyBranch = _dbContext.SqlQuery<BranchModal>(bQuery).ToList();
                    }
                    return companyData;
                }
                else
                {
                    var companyQuery = $@"SELECT DISTINCT SCS.ACCESS_FLAG as NEW , SCS.ACCESS_FLAG as EDIT , SCS.ACCESS_FLAG as ""VIEW"" , SCS.COMPANY_CODE,CS.COMPANY_EDESC,SCS.USER_NO,CS.COMPANY_CODE as CONTROL_CODE,CS.COMPANY_EDESC as CONTROL_NAME,
                                        SCS.ACCESS_FLAG as RECYCLE,SCS.ACCESS_FLAG as POSTPRINT , SCS.ACCESS_FLAG as UNPOST, SCS.ACCESS_FLAG as ""CHECK"", SCS.ACCESS_FLAG as VERIFY,'COMPANY AND BRANCH' as CONTROL_HEADING
                                        FROM COMPANY_SETUP CS
                                        LEFT JOIN SC_COMPANY_CONTROL SCS ON CS.COMPANY_CODE = SCS.COMPANY_CODE AND SCS.USER_NO = '{userNo}'
                                        WHERE SCS.DELETED_FLAG = 'N'";
                    var companyData = _dbContext.SqlQuery<CompanyModal>(companyQuery).ToList();
                    if (companyData.Count > 0)
                    {
                        foreach (var cd in companyData)
                        {
                            var bQuery = $@"SELECT SBC.USER_NO,SBC.BRANCH_CODE,FBS.BRANCH_EDESC FROM SC_BRANCH_CONTROL SBC 
                                        INNER JOIN FA_BRANCH_SETUP FBS ON FBS.BRANCH_CODE = SBC.BRANCH_CODE 
                                        WHERE SBC.DELETED_FLAG='N' AND SBC.COMPANY_CODE='{cd.COMPANY_CODE}' AND SBC.USER_NO='{userNo}'";
                            cd.CompanyBranch = _dbContext.SqlQuery<BranchModal>(bQuery).ToList();
                        }
                    }
                   
                    return companyData;
                }
               
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting company : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private List<AvailableModuleModal> GetAvailableDocManagerAsControl(string userNo="0",string selectedControl="0")
        {
            try
            {
                if (userNo == "0" || userNo=="")
                {
                    if (selectedControl == "0")
                    {
                        var moduleQuery = $@"SELECT DISTINCT 
                        INITCAP(FORM_EDESC) AS FORM_EDESC,
                        INITCAP(FORM_NDESC) AS FORM_NDESC,
                        FORM_CODE,
                        CUSTOM_PREFIX_TEXT AS CUSTOMER_PREFIX,
                        MODULE_CODE,
                        MASTER_FORM_CODE,
                        PRE_FORM_CODE,
                        GROUP_SKU_FLAG
                        FROM FORM_SETUP
                        WHERE DELETED_FLAG = 'N'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                        ORDER BY PRE_FORM_CODE";
                        var moduleData = _objectEntity.SqlQuery<AvailableModuleModal>(moduleQuery).ToList();
                        foreach (var md in moduleData)
                        {
                            var query = $@"SELECT DISTINCT 
                        INITCAP(FORM_EDESC) AS FORM_EDESC,
                        INITCAP(FORM_NDESC) AS FORM_NDESC,
                        FORM_CODE,
                        CUSTOM_PREFIX_TEXT AS CUSTOMER_PREFIX,
                        MODULE_CODE,
                        MASTER_FORM_CODE,
                        PRE_FORM_CODE,
                        GROUP_SKU_FLAG
                        FROM FORM_SETUP
                        WHERE DELETED_FLAG = 'N' AND GROUP_SKU_FLAG !='G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' 
                        AND PRE_FORM_CODE= '{md.MASTER_FORM_CODE}'";

                            md.CHILDREN = _dbContext.SqlQuery<AvailableModuleModal>(query).ToList();
                        }

                        return moduleData;
                    }
                    else
                    {
                        var moduleQuery = $@"SELECT DISTINCT 
                        INITCAP(FORM_EDESC) AS FORM_EDESC,
                        INITCAP(FORM_NDESC) AS FORM_NDESC,
                        FORM_CODE,
                        CUSTOM_PREFIX_TEXT AS CUSTOMER_PREFIX,
                        MODULE_CODE,
                        MASTER_FORM_CODE,
                        PRE_FORM_CODE,
                        GROUP_SKU_FLAG
                        FROM FORM_SETUP
                        WHERE DELETED_FLAG = 'N' 
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE='{selectedControl}'
                        ORDER BY PRE_FORM_CODE";
                        var moduleData = _objectEntity.SqlQuery<AvailableModuleModal>(moduleQuery).ToList();
                        foreach (var md in moduleData)
                        {
                            var query = $@"SELECT DISTINCT 
                        INITCAP(FORM_EDESC) AS FORM_EDESC,
                        INITCAP(FORM_NDESC) AS FORM_NDESC,
                        FORM_CODE,
                        CUSTOM_PREFIX_TEXT AS CUSTOMER_PREFIX,
                        MODULE_CODE,
                        MASTER_FORM_CODE,
                        PRE_FORM_CODE,
                        GROUP_SKU_FLAG
                        FROM FORM_SETUP
                        WHERE DELETED_FLAG = 'N' AND GROUP_SKU_FLAG !='G'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE='{selectedControl}'
                        AND PRE_FORM_CODE= '{md.MASTER_FORM_CODE}'";

                            md.CHILDREN = _dbContext.SqlQuery<AvailableModuleModal>(query).ToList();
                        }

                        return moduleData;
                    }
                   
                }
                else
                {
                    if (selectedControl == "0")
                    {
                        // var docMangerquery = $@"SELECT * FROM SC_FORM_CONTROL SFC WHERE SFC.DELETED_FLAG='N' AND SFC.USER_NO='{userNo}'";
                        var docManagerQuery = $@"  SELECT SFC.USER_NO,SFC.FORM_CODE,FS.FORM_EDESC,SFC.CREATE_FLAG as NEW1,SFC.READ_FLAG as VIEW1,SFC.UPDATE_FLAG as EDIT1,SFC.CHECK_FLAG as CHECK1,SFC.VERIFY_FLAG as VERIFY1,SFC.POST_FLAG as POSTPRINT1,
                                             SFC.UNPOST_FLAG as UNPOST1
                                            FROM SC_FORM_CONTROL SFC 
                                            INNER JOIN FORM_SETUP FS ON FS.FORM_CODE = SFC.FORM_CODE
                                            WHERE SFC.DELETED_FLAG='N' AND SFC.USER_NO='{userNo}'";
                        var moduleData = _objectEntity.SqlQuery<AvailableModuleModal>(docManagerQuery).ToList();

                        return moduleData;
                    }
                    else
                    {
                        // var docMangerquery = $@"SELECT * FROM SC_FORM_CONTROL SFC WHERE SFC.DELETED_FLAG='N' AND SFC.USER_NO='{userNo}'";
                        var docManagerQuery = $@"  SELECT SFC.USER_NO,SFC.FORM_CODE,FS.FORM_EDESC,SFC.CREATE_FLAG as NEW1,SFC.READ_FLAG as VIEW1,SFC.UPDATE_FLAG as EDIT1,SFC.CHECK_FLAG as CHECK1,SFC.VERIFY_FLAG as VERIFY1,SFC.POST_FLAG as POSTPRINT1,
                                             SFC.UNPOST_FLAG as UNPOST1
                                            FROM SC_FORM_CONTROL SFC 
                                            INNER JOIN FORM_SETUP FS ON FS.FORM_CODE = SFC.FORM_CODE
                                            WHERE SFC.DELETED_FLAG='N' AND SFC.USER_NO='{userNo}' AND FS.MODULE_CODE='{selectedControl}'";
                        var moduleData = _objectEntity.SqlQuery<AvailableModuleModal>(docManagerQuery).ToList();

                        return moduleData;
                    }
                   
                }
                
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting module control : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        private List<AvailableControl> GetMasterDefinitionCtrlAsConrol(string userNo="0")
        {
            try
            {
                if (userNo == "0")
                {
                    var masterDefQuery = $@"SELECT wm.MENU_NO as CONTROL_CODE , wm.MENU_EDESC as CONTROL_NAME,wm.PRE_MENU_NO ,'Master Definition Control' as CONTROL_HEADING FROM WEB_MENU_MANAGEMENT wm ORDER BY wm.PRE_MENU_NO";
                    var masterDefData = _objectEntity.SqlQuery<AvailableControl>(masterDefQuery).ToList();
                    return masterDefData;
                }
                else
                {
                   // var masterDefQuery = $@"SELECT wm.MENU_NO as CONTROL_CODE ,'Master Definition Control' as CONTROL_HEADING FROM SC_MENU_CONTROL wm";
                   var masterDefQuery = $@"SELECT SMC.USER_NO,SMC.MENU_NO as CONTROL_CODE,WMM.MENU_EDESC as CONTROL_NAME,'MASTER DEFINITION CONTROL' as CONTROL_HEADING ,SMC.CREATE_FLAG as NEW1, 
                                           SMC.READ_FLAG as VIEW1 , SMC.UPDATE_FLAG as EDIT1 ,SMC.CHECK_FLAG as CHECK1 , SMC.POST_FLAG as POSTPRINT1 FROM SC_MENU_CONTROL SMC
                                           INNER JOIN WEB_MENU_MANAGEMENT WMM ON WMM.MENU_NO = SMC.MENU_NO
                                           WHERE SMC.DELETED_FLAG = 'N' AND SMC.USER_NO ='{userNo}'";
                    var masterDefData = _objectEntity.SqlQuery<AvailableControl>(masterDefQuery).ToList();
                    foreach(var mdd in masterDefData)
                    {
                        mdd.NEW = true;
                        mdd.VIEW = true;
                        mdd.EDIT = true;
                        mdd.RECYCLE = true;
                        mdd.POSTPRINT = true;
                        mdd.UNPOST = true;
                        mdd.CHECK = true;
                        mdd.VERIFY = true;
                    }
                    return masterDefData;
                }
               
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting master and definition control : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<AvailableControl> GetAvailableCompanyControl(string userNo)
        {
            try
            {
                var availableControlList = new List<AvailableControl>();
                var parentComp = 0;
                //var parentCtrl = 0;
                var company = GetCompanyAsControl(userNo);
                if (userNo == "0")
                {
                    foreach (var cm in company)
                    {
                        foreach (var br in cm.CompanyBranch)
                        {
                            if (parentComp == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE,  CONTROL_NAME = br.BRANCH_EDESC, REPORTSTO = null, CONTROL_HEADING = "COMPANY AND BRANCH" });
                            else availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, CONTROL_NAME = br.BRANCH_EDESC,  REPORTSTO = cm.COMPANY_CODE, CONTROL_HEADING = "COMPANY AND BRANCH" });
                            parentComp++;
                        }

                    }
                }
                else
                {
                    foreach (var cm in company)
                    {
                        foreach (var br in cm.CompanyBranch)
                        {
                            if (parentComp == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, NEW = true, EDIT = true, VIEW = true, CONTROL_NAME = br.BRANCH_EDESC, REPORTSTO = null, CONTROL_HEADING = "COMPANY AND BRANCH" });
                            else availableControlList.Add(new AvailableControl { CONTROL_CODE = br.BRANCH_CODE, CONTROL_NAME = br.BRANCH_EDESC, NEW = true, VIEW = true, EDIT = true, REPORTSTO = cm.COMPANY_CODE, CONTROL_HEADING = "COMPANY AND BRANCH" });
                            parentComp++;
                        }

                    }
                }
               
                return availableControlList;

            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting company : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<AvailableControl> GetAvailableMasterDefinitionCntrl(string userNo)
        {
            try
            {
                var availableMstrDefCntrl = GetMasterDefinitionCtrlAsConrol(userNo);
                foreach(var amdc in availableMstrDefCntrl)
                {
                    amdc.NEW = amdc.NEW1 == "Y" ? true : false;
                    amdc.VIEW = amdc.VIEW1 == "Y" ? true : false;
                    amdc.EDIT = amdc.EDIT1 == "Y" ? true : false;
                    amdc.CHECK = amdc.CHECK1 == "Y" ? true : false;
                    amdc.POSTPRINT = amdc.POSTPRINT1 == "Y" ? true : false;
                }
                return availableMstrDefCntrl;
               
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting master and definition control : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<AvailableControl> GetAvailableDocumentManagerCntrl(string userNo,string selectedControl)
        {
            try
            {
                var availableControlList = new List<AvailableControl>();
                var parentCtrl = 0;
                var availableDocManager = GetAvailableDocManagerAsControl(userNo,selectedControl);
                if (userNo == "0")
                {
                    foreach (var adm in availableDocManager)
                    {
                        foreach (var admCldrn in adm.CHILDREN)
                        {
                            if (parentCtrl == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = adm.FORM_CODE, CONTROL_NAME = adm.FORM_EDESC, REPORTSTO = null, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                            else availableControlList.Add(new AvailableControl { CONTROL_CODE = admCldrn.FORM_CODE, CONTROL_NAME = admCldrn.FORM_EDESC, REPORTSTO = admCldrn.FORM_CODE, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                            parentCtrl++;
                        }

                    }
                }
                else
                {
                    foreach (var adm in availableDocManager)
                    {
                        //foreach (var admCldrn in adm.CHILDREN)
                        //{
                            if (parentCtrl == 0) availableControlList.Add(new AvailableControl { CONTROL_CODE = adm.FORM_CODE, CONTROL_NAME = adm.FORM_EDESC, NEW=adm.NEW1=="Y"? true : false ,EDIT=adm.EDIT1=="Y" ? true:false,VIEW=adm.VIEW1=="Y"?true:false,CHECK=adm.CHECK1=="Y"?true:false,VERIFY=adm.VERIFY1== "Y" ? true:false,POSTPRINT=adm.POSTPRINT1=="Y"?true:false,UNPOST=adm.UNPOST1=="Y"?true:false,REPORTSTO = null, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                            else availableControlList.Add(new AvailableControl { CONTROL_CODE = adm.FORM_CODE, CONTROL_NAME = adm.FORM_EDESC, NEW = adm.NEW1 == "Y" ? true : false, EDIT = adm.EDIT1 == "Y" ? true : false, VIEW = adm.VIEW1 == "Y" ? true : false, CHECK = adm.CHECK1 == "Y" ? true : false, VERIFY = adm.VERIFY1 == "Y" ? true : false, POSTPRINT = adm.POSTPRINT1 == "Y" ? true : false, UNPOST = adm.UNPOST1 == "Y" ? true : false,REPORTSTO = adm.FORM_CODE, CONTROL_HEADING = "DOCUMENT MANAGER(TRANSACTION)" });
                            parentCtrl++;
                        //}

                    }
                }
                
                return availableControlList;
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting document : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<AvailableControl> GetAvailableModuleControl(string userNo="0",string selectedControl="")
        {
            var avaibleMDControl = GetAvailableModuleCntrl(userNo, selectedControl);
            return avaibleMDControl;

        }

        public List<AvailableControl> GetMasterSetupListControl(string userNo="0",string selectedControl = "0")
        {
            var availableMasterSetupList = GetMasterSetupListCntrl(userNo, selectedControl);
            return availableMasterSetupList;
        }

        private List<AvailableControl> GetMasterSetupListCntrl(string userNo="0",string selectedControl = "0")
        {
            try
            {
                if (userNo == "0")
                {
                    if (selectedControl == "0")
                    {
                        var menuQuery = $@"SELECT DISTINCT MENU_NO as CONTROL_CODE,MENU_EDESC as CONTROL_NAME,'MASTER SETUP ' as CONTROL_HEADING ,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' 
                                          ORDER by MENU_NO ASC";
                        var menuData = _objectEntity.SqlQuery<AvailableControl>(menuQuery).ToList();
                        return menuData;
                    }
                    else
                    {
                        var menuQuery = $@"SELECT DISTINCT MENU_NO as CONTROL_CODE,MENU_EDESC as CONTROL_NAME,'MASTER SETUP ' as CONTROL_HEADING ,MENU_OBJECT_NAME,MODULE_CODE,FULL_PATH,MODULE_ABBR,COLOR,ICON_PATH FROM WEB_MENU_MANAGEMENT WHERE COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND MODULE_CODE = '{selectedControl}'
                                          ORDER by MENU_NO ASC";
                        var menuData = _objectEntity.SqlQuery<AvailableControl>(menuQuery).ToList();
                        return menuData;
                    }
                    
                }
                else
                {
                    string menuQuery = string.Empty;
                    if (selectedControl == "0")
                    {
                         menuQuery = $@"SELECT DISTINCT  SMC.MENU_NO as CONTROL_CODE ,WMM.MENU_EDESC as CONTROL_NAME,SMC.USER_NO ,'MASTER SETUP' as CONTROL_HEADING, SMC.CREATE_FLAG as NEW , SMC.READ_FLAG as ""VIEW"" , SMC.UPDATE_FLAG as EDIT,SMC.DELETE_FLAG as RECYCLE,
                                      SMC.POST_FLAG as POSTPRINT , CASE WHEN SMC.POST_FLAG = 'Y' THEN 'N' ELSE 'Y' END AS UNPOST ,SMC.CHECK_FLAG as ""CHECK"",SMC.ACCESS_FLAG as VERIFY
                                      FROM SC_MENU_CONTROL SMC
                                      INNER JOIN WEB_MENU_MANAGEMENT WMM ON WMM.MENU_NO = SMC.MENU_NO
                                      WHERE SMC.USER_NO = '{userNo}'";
                    }
                    else
                    {
                         menuQuery = $@"SELECT DISTINCT  SMC.MENU_NO as CONTROL_CODE ,WMM.MENU_EDESC as CONTROL_NAME,SMC.USER_NO ,'MASTER SETUP' as CONTROL_HEADING, SMC.CREATE_FLAG as NEW , SMC.READ_FLAG as ""VIEW"" , SMC.UPDATE_FLAG as EDIT,SMC.DELETE_FLAG as RECYCLE,
                                      SMC.POST_FLAG as POSTPRINT , CASE WHEN SMC.POST_FLAG = 'Y' THEN 'N' ELSE 'Y' END AS UNPOST ,SMC.CHECK_FLAG as ""CHECK"",SMC.ACCESS_FLAG as VERIFY
                                      FROM SC_MENU_CONTROL SMC
                                      INNER JOIN WEB_MENU_MANAGEMENT WMM ON WMM.MENU_NO = SMC.MENU_NO
                                      WHERE SMC.USER_NO = '{userNo}' AND WMM.MODULE_CODE='{selectedControl}'";
                    }
                    
                    var accessData = _objectEntity.SqlQuery(menuQuery);

                    var accessedControl = new List<AvailableControl>();
                    for (int i = 0; i < accessData.Rows.Count; i++)
                    {
                        var accessComp = new AvailableControl
                        {
                            CONTROL_CODE = accessData.Rows[i]["CONTROL_CODE"].ToString(),
                            CONTROL_NAME = accessData.Rows[i]["CONTROL_NAME"].ToString(),
                            CONTROL_HEADING = accessData.Rows[i]["CONTROL_HEADING"].ToString(),
                            NEW = accessData.Rows[i]["NEW"].ToString() == "Y" ? true : false,
                            VIEW = accessData.Rows[i]["VIEW"].ToString() == "Y" ? true : false,
                            EDIT = accessData.Rows[i]["EDIT"].ToString() == "Y" ? true : false,
                            RECYCLE = accessData.Rows[i]["RECYCLE"].ToString() == "Y" ? true : false,
                            POSTPRINT = accessData.Rows[i]["POSTPRINT"].ToString() == "Y" ? true : false,
                            CHECK = accessData.Rows[i]["CHECK"].ToString() == "Y" ? true : false,
                            VERIFY = accessData.Rows[i]["VERIFY"].ToString() == "Y" ? true : false

                        };

                        accessedControl.Add(accessComp);
                    }
                    return accessedControl;
                }
                
                  
                
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting master setup list view : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

       private List<AvailableControl> GetAvailableModuleCntrl(string userNo,string selectedControl)
        {
            try
            {
                if (userNo == "0")
                {
                    string query = $@"SELECT DISTINCT 
                        INITCAP(MODULE_CODE) AS CONTROL_CODE,
                        INITCAP(MODULE_EDESC) AS CONTROL_NAME,
                        'MODULE_SETUP' as CONTROL_HEADING
                        FROM MODULE_SETUP
                        ORDER BY CONTROL_CODE";
                    var moduleData = _objectEntity.SqlQuery<AvailableControl>(query).ToList();
                    return moduleData;
                }
                else
                {
                    string accessModal = $@"SELECT SMUC.USER_NO,SMUC.MODULE_CODE as CONTROL_CODE,MS.MODULE_EDESC as CONTROL_NAME,'MODULE SETUP' as CONTROL_HEADING,SMUC.ACCESS_FLAG AS NEW,SMUC.ACCESS_FLAG as ""VIEW"",SMUC.ACCESS_FLAG as EDIT,SMUC.ACCESS_FLAG as RECYCLE,
                                            SMUC.ACCESS_FLAG as POSTPRINT,SMUC.ACCESS_FLAG as UNPOST,SMUC.ACCESS_FLAG as ""CHECK"",SMUC.ACCESS_FLAG as VERIFY
                                            FROM SC_MODULE_USER_CONTROL SMUC
                                            INNER JOIN MODULE_SETUP MS ON MS.MODULE_CODE = SMUC.MODULE_CODE
                                            WHERE SMUC.DELETED_FLAG = 'N' AND
                                            SMUC.USER_NO = '{userNo}'";
                    //var accessData = _objectEntity.SqlQuery<AvailableControl>(accessModal).ToList();
                    var acccessData = _objectEntity.SqlQuery(accessModal);
                    //var nextRes = _objectEntity.SqlQuery(accessedCompany);
                    var accessedControl = new List<AvailableControl>();
                    for (int i = 0; i < acccessData.Rows.Count; i++)
                    {
                        var accessComp = new AvailableControl
                        {
                            CONTROL_CODE = acccessData.Rows[i]["CONTROL_CODE"].ToString(),
                            CONTROL_NAME = acccessData.Rows[i]["CONTROL_NAME"].ToString(),
                            CONTROL_HEADING = acccessData.Rows[i]["CONTROL_HEADING"].ToString(),
                            NEW = acccessData.Rows[i]["NEW"].ToString() == "Y" ? true : false,
                            VIEW = acccessData.Rows[i]["VIEW"].ToString() == "Y" ? true : false,
                            EDIT = acccessData.Rows[i]["EDIT"].ToString() == "Y" ? true : false,
                            RECYCLE = acccessData.Rows[i]["RECYCLE"].ToString() == "Y" ? true : false,
                            POSTPRINT = acccessData.Rows[i]["POSTPRINT"].ToString() == "Y" ? true : false,
                            CHECK = acccessData.Rows[i]["CHECK"].ToString() == "Y" ? true:false,
                            VERIFY = acccessData.Rows[i]["VERIFY"].ToString() == "Y" ? true :false

                        };

                         accessedControl.Add(accessComp);
                    }
                    return accessedControl;
                }
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while fetching module : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string SaveUserAccessControl(UserAccessSaveModel modal)
        {
            using (var saveTran = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    if (modal.checkedUser.Count > 0)
                    {
                        foreach (var user in modal.checkedUser)
                        {
                            foreach (var cntrl in modal.checkedControl)
                            {
                                if (cntrl.CONTROL_HEADING.Contains("COMPANY"))
                                {
                                    if (modal.isUpdate)
                                    {
                                        if(cntrl.REPORTSTO==null || cntrl.REPORTSTO == "null")
                                        {
                                            var delQuery = $@"DELETE FROM SC_COMPANY_CONTROL WHERE USER_NO='{user.USER_NO}' AND COMPANY_CODE='{cntrl.CONTROL_CODE}'";
                                            var rowaffected = _objectEntity.ExecuteSqlCommand(delQuery);
                                        }
                                        else
                                        {
                                            var delQuery = $@"DELETE FROM SC_BRANCH_CONTROL WHERE USER_NO='{user.USER_NO}' AND BRANCH_CODE='{cntrl.CONTROL_CODE}'";
                                            var rowaffected = _objectEntity.ExecuteSqlCommand(delQuery);
                                        }
                                    }
                                    if(cntrl.NEW && cntrl.VIEW && cntrl.EDIT)
                                    {
                                        if (cntrl.REPORTSTO == null || cntrl.REPORTSTO == "null")
                                        {

                                            try
                                            {
                                                var companyInsert = $@"INSERT INTO SC_COMPANY_CONTROL(USER_NO,COMPANY_CODE,ACCESS_FLAG,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES({user.USER_NO},'{cntrl.CONTROL_CODE}','Y','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                                _objectEntity.ExecuteSqlCommand(companyInsert);
                                            }
                                            catch(Exception ex)
                                            {

                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{cntrl.CONTROL_CODE}','Y','{cntrl.REPORTSTO}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                                _objectEntity.ExecuteSqlCommand(branchInsert);
                                            }
                                            catch(Exception ex)
                                            {

                                            }
                                            
                                        }
                                    }
                                }
                                else if (cntrl.CONTROL_HEADING.Contains("TRANSACTION"))
                                {
                                    if (modal.isUpdate)
                                    {
                                        var delQuery = $@"DELETE FROM SC_FORM_CONTROL WHERE USER_NO='{user.USER_NO}' AND FORM_CODE='{cntrl.CONTROL_CODE}'";
                                        var rowaffected = _objectEntity.ExecuteSqlCommand(delQuery);

                                    }
                                    if(cntrl.NEW && cntrl.VIEW && cntrl.EDIT)
                                    {
                                        try
                                        {
                                            var formInsert = $@"INSERT INTO SC_FORM_CONTROL(USER_NO,FORM_CODE,CREATE_FLAG,READ_FLAG,UPDATE_FLAG,DELETE_FLAG,POST_FLAG,UNPOST_FLAG,CHECK_FLAG,VERIFY_FLAG,MORE_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,BRANCH_CODE)
                                         VALUES({user.USER_NO},{cntrl.CONTROL_CODE},'Y','Y','Y','Y','Y','Y','Y','Y','Y','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}')";
                                            _objectEntity.ExecuteSqlCommand(formInsert);
                                        }
                                        catch(Exception ex)
                                        {

                                        }
                                    }
                                }
                                else if (cntrl.CONTROL_HEADING.Contains("Master Definition Control"))
                                {
                                    if (modal.isUpdate)
                                    {
                                        var delQuery = $@"DELETE FROM SC_MENU_CONTROL WHERE USER_NO='{user.USER_NO}' AND MENU_NO='{cntrl.CONTROL_CODE}'";
                                        var rowaffected = _objectEntity.ExecuteSqlCommand(delQuery);

                                    }
                                    if(cntrl.NEW && cntrl.VIEW && cntrl.EDIT)
                                    {
                                        try
                                        {
                                            var definitionSC_MENUInsert = $@"INSERT INTO SC_MENU_CONTROL(USER_NO,MENU_NO,ACCESS_FLAG,CREATE_FLAG,READ_FLAG,UPDATE_FLAG,DELETE_FLAG,POST_FLAG,
                                                  CHECK_FLAG,MORE_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE) 
                                   VALUES({user.USER_NO},{cntrl.CONTROL_CODE},'Y','Y','Y','Y','Y','Y','Y','{cntrl.MORE}',
                                   '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                   'N','{_workContext.CurrentUserinformation.branch_code}')";
                                            _objectEntity.ExecuteSqlCommand(definitionSC_MENUInsert);
                                        }
                                        catch(Exception ex)
                                        {

                                        }

                                        try
                                        {
                                            var definitionWEB_MENUInsert = $@"INSERT INTO WEB_MENU_CONTROL(USER_NO,MENU_NO,ACCESS_FLAG,CREATE_FLAG,READ_FLAG,UPDATE_FLAG,DELETE_FLAG,POST_FLAG,
                                                  CHECK_FLAG,MORE_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE) 
                                   VALUES({user.USER_NO},{cntrl.CONTROL_CODE},'Y','Y','Y','Y','Y','Y','Y','{cntrl.MORE}',
                                   '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                   'N','{_workContext.CurrentUserinformation.branch_code}')";
                                            _objectEntity.ExecuteSqlCommand(definitionWEB_MENUInsert);
                                        }
                                        catch(Exception ex)
                                        {

                                        }

                                       
                                    }
                                }
                                else if (cntrl.CONTROL_HEADING.Contains("Module"))
                                {
                                    if (modal.isUpdate)
                                    {
                                        var delQuery = $@"DELETE FROM SC_MODULE_USER_CONTROL WHERE USER_NO='{user.USER_NO}' AND MODULE_CODE='{cntrl.CONTROL_CODE}'";
                                        var rowaffected = _objectEntity.ExecuteSqlCommand(delQuery);

                                    }
                                    if(cntrl.NEW && cntrl.VIEW && cntrl.EDIT)
                                    {
                                        try
                                        {
                                            var moduleInsert = $@"INSERT INTO SC_MODULE_USER_CONTROL(USER_NO,MODULE_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                                                          VALUES('{user.USER_NO}','{cntrl.CONTROL_CODE}','Y','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
                                            var rowAff = _objectEntity.ExecuteSqlCommand(moduleInsert);
                                        }
                                        catch(Exception ex)
                                        {

                                        }
                                        
                                    }
                                    
                                }
                                else if(cntrl.CONTROL_HEADING.Contains("MASTER SETUP"))
                                {
                                    if (modal.isUpdate)
                                    {
                                        var delQuery = $@"DELETE FROM SC_MENU_CONTROL WHERE USER_NO='{user.USER_NO}' AND MENU_NO='{cntrl.CONTROL_CODE}'";
                                        var rowaffected = _objectEntity.ExecuteSqlCommand(delQuery);
                                    }
                                    if(cntrl.NEW && cntrl.VIEW && cntrl.EDIT)
                                    {
                                        try
                                        {
                                            var definitionSC_MENUInsert = $@"INSERT INTO SC_MENU_CONTROL(USER_NO,MENU_NO,ACCESS_FLAG,CREATE_FLAG,READ_FLAG,UPDATE_FLAG,DELETE_FLAG,POST_FLAG,
                                                  CHECK_FLAG,MORE_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE) 
                                           VALUES({user.USER_NO},{cntrl.CONTROL_CODE},'Y','Y','Y','Y','Y','Y','Y','{cntrl.MORE}',
                                           '{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                           'N','{_workContext.CurrentUserinformation.branch_code}')";
                                            _objectEntity.ExecuteSqlCommand(definitionSC_MENUInsert);
                                        }
                                        catch(Exception ex)
                                        {

                                        }
                                       
                                    }
                                }
                            }
                            saveTran.Commit();
                        }
                    }

                    return "Successfull";
                }
                catch (Exception ex)
                {
                    _logErp.ErrorInDB("Error while saving user access control : " + ex.StackTrace);
                    saveTran.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        //public List<AvailableControl> GetAccessCompanyNBranch(string selectedUser)
        //{
        //    try
        //    {
        //        var accessCompBrnchLst = new List<AvailableControl>();
        //      //  var accCom = new List<AvailableControl>();
        //      //  var accessComp = new AvailableControl();
        //      //  var accessBrnch = new AvailableControl();
        //        selectedUser = selectedUser.Replace("\"", "'");
        //        if (selectedUser == null || string.IsNullOrEmpty(selectedUser)) return null;
        //        else
        //        {


        //            //string accessedQuery = $@"SELECT scc.COMPANY_CODE as CONTROL_CODE,sac.USER_NO,sbc.BRANCH_CODE FROM SC_COMPANY_CONTROL scc
        //            //                          INNER JOIN SC_APPLICATION_USERS sac on scc.USER_NO=sac.USER_NO 
        //            //                          INNER JOIN SC_BRANCH_CONTROL sbc on sbc.USER_NO = sac.USER_NO WHERE sac.USER_NO={selectedUser}";
        //            //var accessedData = _objectEntity.SqlQuery<AvailableControl>(accessedQuery).ToList();
        //            string accessedCompany = $@"SELECT scc.COMPANY_CODE as CONTROL_CODE,cs.COMPANY_EDESC as CONTROL_NAME, 'true' as ""NEW"" ,'true' as ""EDIT"" ,
        //                                    'true' as ""VIEW"" ,'true' as RECYCLE , 'true' as POSTPRINT , 'true' as ""CHECK"" , 'true' as VERIFY FROM SC_COMPANY_CONTROL scc
        //                                    INNER JOIN SC_APPLICATION_USERS sau on sau.USER_NO=scc.USER_NO 
        //                                    INNER JOIN COMPANY_SETUP cs on cs.COMPANY_CODE=scc.COMPANY_CODE WHERE sau.USER_NO={selectedUser}" ;
        //            var nextRes = _objectEntity.SqlQuery(accessedCompany);
        //            for(int i = 0; i < nextRes.Rows.Count; i++)
        //            {
        //                var accessComp = new AvailableControl
        //                {
        //                    CONTROL_CODE = nextRes.Rows[i]["CONTROL_CODE"].ToString(),
        //                    CONTROL_NAME = nextRes.Rows[i]["CONTROL_NAME"].ToString(),
        //                   NEW = Convert.ToBoolean(nextRes.Rows[i]["NEW"].ToString()),
        //                   VIEW = Convert.ToBoolean(nextRes.Rows[i]["VIEW"].ToString()),
        //                  EDIT = Convert.ToBoolean(nextRes.Rows[i]["EDIT"].ToString()),
        //                  RECYCLE = Convert.ToBoolean(nextRes.Rows[i]["RECYCLE"].ToString()),
        //                  POSTPRINT = Convert.ToBoolean(nextRes.Rows[i]["POSTPRINT"].ToString()),
        //                  CHECK = Convert.ToBoolean(nextRes.Rows[i]["CHECK"].ToString()),
        //                VERIFY = Convert.ToBoolean(nextRes.Rows[i]["VERIFY"].ToString())

        //                };

        //                accessCompBrnchLst.Add(accessComp);
        //            }


        //            string accessedBranch = $@"SELECT sbc.BRANCH_CODE as CONTROL_CODE, fbs.BRANCH_EDESC as CONTROL_NAME, 'true' as ""NEW"" ,'true' as ""EDIT"" ,
        //                                    'true' as ""VIEW"" ,'true' as RECYCLE ,'true' as POSTPRINT , 'true' as ""CHECK"" , 'true' as VERIFY FROM SC_BRANCH_CONTROL sbc
        //                                    INNER JOIN SC_APPLICATION_USERS sau on sau.USER_NO=sbc.USER_NO 
        //                                    INNER JOIN FA_BRANCH_SETUP fbs on fbs.BRANCH_CODE=sbc.BRANCH_CODE WHERE sau.USER_NO={selectedUser}";
        //            var branchRes = _objectEntity.SqlQuery(accessedBranch);
        //            for(int i = 0; i < branchRes.Rows.Count; i++)
        //            {
        //                var accessBrnch = new AvailableControl
        //                {
        //                    CONTROL_CODE = branchRes.Rows[i]["CONTROL_CODE"].ToString(),
        //                    CONTROL_NAME = branchRes.Rows[i]["CONTROL_NAME"].ToString(),
        //                    NEW = Convert.ToBoolean(branchRes.Rows[i]["NEW"].ToString()),
        //                    VIEW = Convert.ToBoolean(branchRes.Rows[i]["VIEW"].ToString()),
        //                    EDIT = Convert.ToBoolean(branchRes.Rows[i]["EDIT"].ToString()),
        //                    RECYCLE = Convert.ToBoolean(branchRes.Rows[i]["RECYCLE"].ToString()),
        //                    POSTPRINT = Convert.ToBoolean(branchRes.Rows[i]["POSTPRINT"].ToString()),
        //                   CHECK = Convert.ToBoolean(branchRes.Rows[i]["CHECK"].ToString()),
        //                   VERIFY = Convert.ToBoolean(branchRes.Rows[i]["VERIFY"].ToString())
        //                };
        //                accessCompBrnchLst.Add(accessBrnch);
        //            }

        //            return accessCompBrnchLst;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting accessed control : " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}

        //private List<UserTreeModel> generateCustomerTree(List<UserTreeModel> model, List<UserTreeModel> userNodes, string preItemCode)
        //{
        //    foreach (var users in model.Where(x => Convert.ToDecimal(x.PRE_USER_NO) == Convert.ToDecimal(preItemCode)))
        //    {
        //        var customerNodesChild = new List<UserTreeModel>();
        //        userNodes.Add(new UserTreeModel()
        //        {
        //            LEVEL = users.LEVEL,
        //            LOGIN_EDESC = users.LOGIN_EDESC,
        //            LOGIN_CODE = users.LOGIN_CODE,
        //            USER_NO = users.USER_NO,
        //            PRE_USER_NO = users.PRE_USER_NO,
        //            GROUP_SKU_FLAG = users.GROUP_SKU_FLAG,
        //            hasBranch = users.GROUP_SKU_FLAG == "G" ? false : false,
        //            ABBR_CODE = users.ABBR_CODE,
        //            PARENT_USER_CODE = users.PARENT_USER_CODE,
        //            Items = users.GROUP_SKU_FLAG == "G" ? generateCustomerTree(model, customerNodesChild, users.PARENT_USER_CODE.ToString()) : null,
        //        });
        //    }
        //    return userNodes;
        //}

        //public List<BranchModal> GetBranch()
        //{
        //    try
        //    {
        //        var branchQuery = $@"SELECT * FROM FA_BRANCH_SETUP FBS WHERE FBS.DELETED_FLAG='N'";
        //        var branchData = _dbContext.SqlQuery<BranchModal>(branchQuery).ToList();
        //        return branchData;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting company : " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}



        //public string SaveUserAndCompany(CompanyBranchSaveModal modal)
        //{
        //    using(var trans = _objectEntity.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            if (modal.checkedUser.Count() > 0)
        //            {
        //                foreach (var user in modal.checkedUser)
        //                {
        //                    foreach(var cntrl in modal.checkedControl)
        //                    {
        //                        //if(branch.pre_branch_code=="00" && branch.hasBranch == true)
        //                        //{
        //                        //    var companyInsert = $@"INSERT INTO SC_COMPANY_CONTROL(USER_NO,COMPANY_CODE,ACCESS_FLAG,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES({user.USER_NO},'{branch.pre_branch_code}','Y','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                        //    _objectEntity.ExecuteSqlCommand(companyInsert);
        //                        //}
        //                        //else
        //                        //{
        //                        //    var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{branch.branch_Code}','Y','{branch.pre_branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                        //    _objectEntity.ExecuteSqlCommand(branchInsert);
        //                        //}

        //                        if (cntrl.CONTROL_HEADING.Contains("COMPANY")){

        //                            var companyInsert = $@"INSERT INTO SC_COMPANY_CONTROL(USER_NO,COMPANY_CODE,ACCESS_FLAG,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES({user.USER_NO},'{cntrl.CONTROL_CODE}','Y','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                            _objectEntity.ExecuteSqlCommand(companyInsert);
        //                            var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{cntrl.REPORTSTO}','Y','{cntrl.CONTROL_CODE}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                            _objectEntity.ExecuteSqlCommand(branchInsert);

        //                        }
        //                        else if (cntrl.CONTROL_HEADING.Contains("TRANSACTION")) {
        //                            var formInsert = $@"INSERT INTO SC_FORM_CONTROL(USER_NO,FORM_CODE,CREATE_FLAG,READ_FLAG,UPDATE_FLAG,DELETE_FLAG,POST_FLAG,UNPOST_FLAG,CHECK_FLAG,VERIFY_FLAG,MORE_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,BRANCH_CODE)
        //                                 VALUES({user.USER_NO},{cntrl.CONTROL_CODE},'Y','Y','Y','Y','Y','Y','Y','Y','Y','{_workContext.CurrentUserinformation.company_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}')";
        //                            _objectEntity.ExecuteSqlCommand(formInsert);
        //                        }
        //                    }
        //                    //if (modal.checkedCompany[0].Items.Count() > 0)
        //                    //{
        //                    //    var companyInsert = $@"INSERT INTO SC_COMPANY_CONTROL(USER_NO,COMPANY_CODE,ACCESS_FLAG,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES({user.USER_NO},'{modal.checkedCompany[0].branch_Code}','Y','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                    //    _objectEntity.ExecuteSqlCommand(companyInsert);
        //                    //    foreach (var branch in modal.checkedCompany[0].Items)
        //                    //    {
        //                    //        var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{branch.branch_Code}','Y','{modal.checkedCompany[0].branch_Code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                    //        _objectEntity.ExecuteSqlCommand(branchInsert);
        //                    //    }
        //                    //}
        //                    //else
        //                    //{

        //                    //    var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{modal.checkedCompany[0].branch_Code}','Y','{modal.checkedCompany[0].branch_Code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                    //    _objectEntity.ExecuteSqlCommand(branchInsert);

        //                    //}
        //                }
        //                trans.Commit();
        //                return "Access Successfully Added";
        //            }
        //            else
        //            {
        //                return "User or Company are not provided";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            _logErp.ErrorInDB("Error while saving access for company and branch for user : " + ex.StackTrace);
        //            throw new Exception(ex.Message);
        //        }
        //    }

        //}

        //public string UpdateUserAndCompany(CompanyBranchSaveModal modal)
        //{
        //    using (var trans = _objectEntity.Database.BeginTransaction())
        //    {
        //        try
        //        {
        //            if (modal.checkedUser.Count() > 0)
        //            {
        //                foreach (var user in modal.checkedUser)
        //                {
        //                    foreach (var branch in modal.checkedControl)
        //                    {
        //                                                           //if(branch.pre_branch_code=="00" && branch.hasBranch == true)
        //                        //{
        //                        //    var delCompQuery = $@"DELETE FROM SC_COMPANY_CONTROL WHERE USER_NO={user.USER_NO} and COMPANY_CODE={branch.pre_branch_code}";
        //                        //    _objectEntity.ExecuteSqlCommand(delCompQuery);

        //                        //    var companyInsert = $@"INSERT INTO SC_COMPANY_CONTROL(USER_NO,COMPANY_CODE,ACCESS_FLAG,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES({user.USER_NO},'{branch.pre_branch_code}','Y','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                        //    _objectEntity.ExecuteSqlCommand(companyInsert);
        //                        //}
        //                        //else
        //                        //{
        //                        //    var delBraQuery = $@"DELETE FROM SC_BRANCH_CONTROL WHERE USER_NO={user.USER_NO} and BRANCH_CODE={branch.branch_Code}";
        //                        //    _objectEntity.ExecuteSqlCommand(delBraQuery);

        //                        //    var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{branch.branch_Code}','Y','{branch.pre_branch_code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                        //    _objectEntity.ExecuteSqlCommand(branchInsert);
        //                        //}
        //                    }
        //                    //if (modal.checkedCompany[0].Items.Count() > 0)
        //                    //{
        //                    //    var delCompanyQuery = $@"DELETE FROM SC_COMPANY_CONTROL SCC WHERE SCC.USER_NO={user.USER_NO} and SCC.COMPANY_CODE={modal.checkedCompany[0].branch_Code}";
        //                    //    _objectEntity.ExecuteSqlCommand(delCompanyQuery);
        //                    //    var companyInsert = $@"INSERT INTO SC_COMPANY_CONTROL(USER_NO,COMPANY_CODE,ACCESS_FLAG,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES({user.USER_NO},'{modal.checkedCompany[0].branch_Code}','Y','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                    //    _objectEntity.ExecuteSqlCommand(companyInsert);
        //                    //    foreach (var branch in modal.checkedCompany[0].Items)
        //                    //    {
        //                    //        var delBranchQuery = $@"DELETE FROM SC_BRANCH_CONTROL SBC WHERE SBC.USER_NO={user.USER_NO} and SBC.BRANCH_CODE={branch.branch_Code} and SBC.COMPANY_CODE={modal.checkedCompany[0].branch_Code}";
        //                    //        _objectEntity.ExecuteSqlCommand(delBranchQuery);
        //                    //        var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{modal.checkedCompany[0].branch_Code}','Y','{modal.checkedCompany[0].branch_Code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                    //        _objectEntity.ExecuteSqlCommand(branchInsert);
        //                    //    }
        //                    //}
        //                    //else
        //                    //{
        //                    //    var branchInsert = $@"INSERT INTO SC_BRANCH_CONTROL(USER_NO,BRANCH_CODE,ACCESS_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) VALUES('{user.USER_NO}','{modal.checkedCompany[0].branch_Code}','Y','{modal.checkedCompany[0].branch_Code}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,'N')";
        //                    //    _objectEntity.ExecuteSqlCommand(branchInsert);

        //                    //}
        //                }
        //                trans.Commit();
        //                return "Access Successfully Updated";
        //            }
        //            else
        //            {
        //                return "User or Company are not provided";
        //            }

        //        }
        //        catch (Exception ex)
        //        {
        //            trans.Rollback();
        //            _logErp.ErrorInDB("Error while updating access for company and branch for user : " + ex.StackTrace);
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //}

        //public List<AvailableModuleModalTree> GetAvailableModuleTree()
        //{
        //    var moduleNodes = new List<AvailableModuleModalTree>();
        //    try
        //    {
        //        var moduleQuery = $@"SELECT DISTINCT 
        //                INITCAP(FORM_EDESC) AS FORM_EDESC,
        //                INITCAP(FORM_NDESC) AS FORM_NDESC,
        //                FORM_CODE,
        //                CUSTOM_PREFIX_TEXT AS CUSTOMER_PREFIX,
        //                MODULE_CODE,
        //                MASTER_FORM_CODE, 
        //                PRE_FORM_CODE,
        //                GROUP_SKU_FLAG
        //                FROM FORM_SETUP 
        //                WHERE DELETED_FLAG = 'N'
        //                AND COMPANY_CODE = '01'
        //                CONNECT BY PRIOR MASTER_FORM_CODE = PRE_FORM_CODE
        //                ORDER BY PRE_FORM_CODE";
        //        var moduleData = _dbContext.SqlQuery<AvailableModuleModal>(moduleQuery).ToList();
        //        var moduleTree = generateModuleTree(moduleData, moduleNodes, "00");
        //        return moduleTree;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting available module tree : " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}



        //public List<AvailableControl> GetAccessedDocument(string selectedUser)
        //{
        //    try
        //    {
        //        selectedUser = selectedUser.Replace("\"", "'");
        //        if (selectedUser == null || string.IsNullOrEmpty(selectedUser)) return null;
        //        else
        //        {


        //            string accessedQuery = $@"SELECT sfc.FORM_CODE as CONTROL_CODE,sfc.CREATE_FLAG as ""NEW"",
        //                                    sfc.READ_FLAG as ""VIEW"",sfc.UPDATE_FLAG as ""EDIT"",sfc.POST_FLAG as POSTPRINT,sfc.UNPOST_FLAG as UNPOST,
        //                                    sfc.CHECK_FLAG as ""CHECK"",sfc.VERIFY_FLAG as VERIFY,sac.USER_NO FROM SC_FORM_CONTROL sfc
        //                                   INNER JOIN SC_APPLICATION_USERS sac on sac.USER_NO=sfc.USER_NO 
        //                                   WHERE sac.USER_NO={selectedUser}";
        //            var accessedData = _objectEntity.SqlQuery<AvailableControl>(accessedQuery).ToList();
        //            return accessedData;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting accessed control : " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}

        //public List<AvailableControl> GetAccessedMasterDefinitionCntrl(string selectedUser)
        //{
        //    try
        //    {
        //        selectedUser = selectedUser.Replace("\"", "'");
        //        if (selectedUser == null || string.IsNullOrEmpty(selectedUser)) return null;
        //        else
        //        {


        //            string accessedQuery = $@"SELECT smc.MENU_NO as CONTROL_CODE,wmm.MENU_EDESC as CONTROL_NAME,smc.CREATE_FLAG as NEW1,
        //                                    smc.READ_FLAG as VIEW1,smc.UPDATE_FLAG as EDIT1,smc.POST_FLAG as POSTPRINT1,
        //                                    smc.CHECK_FLAG as CHECK1,sac.USER_NO FROM SC_MENU_CONTROL smc
        //                                    INNER JOIN SC_APPLICATION_USERS sac on sac.USER_NO=smc.USER_NO 
        //                                    INNER JOIN WEB_MENU_MANAGEMENT wmm on wmm.MENU_NO = smc.MENU_NO
        //                                   WHERE sac.USER_NO={selectedUser}";
        //            var accessedData = _objectEntity.SqlQuery<AvailableControl>(accessedQuery).ToList();
        //            return accessedData;

        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logErp.ErrorInDB("Error while getting accessed control : " + ex.StackTrace);
        //        throw new Exception(ex.Message);
        //    }
        //}



        //private List<AvailableModuleModalTree> generateModuleTree(List<AvailableModuleModal> model, List<AvailableModuleModalTree> moduleNodes, string preItemCode)
        //{
        //    foreach (var mNodes in model.Where(x => x.PRE_FORM_CODE == preItemCode))
        //    {
        //        var customerNodesChild = new List<AvailableModuleModalTree>();
        //        moduleNodes.Add(new AvailableModuleModalTree()
        //        {
        //            Level = mNodes.LEVEL,
        //            FORM_EDESC = mNodes.FORM_EDESC,
        //            FORM_CODE = mNodes.FORM_CODE,
        //            MASTER_FORM_CODE = mNodes.MASTER_FORM_CODE,
        //            PRE_FORM_CODE = mNodes.PRE_FORM_CODE,
        //            GROUP_SKU_FLAG = mNodes.GROUP_SKU_FLAG,
        //            hasSubModule = mNodes.GROUP_SKU_FLAG == "G" ? false : false,
        //            FORM_NDESC = mNodes.FORM_NDESC,
        //            CUSTOMER_PREFIX = mNodes.CUSTOMER_PREFIX,
        //            Items = mNodes.GROUP_SKU_FLAG == "G" ? generateModuleTree(model, customerNodesChild, mNodes.MASTER_FORM_CODE) : null,
        //        });
        //    }
        //    return moduleNodes;
        //}


    }


}