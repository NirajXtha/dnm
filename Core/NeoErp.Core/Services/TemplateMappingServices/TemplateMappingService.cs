using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Models.CustomModels.SettingsEntities;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;

namespace NeoErp.Core.Services.TemplateMappingServices
{
    public class TemplateMappingService : ITemplateMappingService
    {
        private IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;

        public TemplateMappingService(IDbContext dbContext,IWorkContext workContext,NeoErpCoreEntity objectEntity)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule,_defaultValueForLog.FormCode);
        }

        public string AddBulkMappedTemplate(MappedFormTemplateModel mappedFormTemplate)
        {
            try
            {
                var formList = mappedFormTemplate.FORM_LIST[0].Split(',');
                var templateList = mappedFormTemplate.TEMPLATE_LIST[0].Split(',');
                var userList = mappedFormTemplate.USER_LIST[0].Split(',');
                var totalBulkCount = Math.Max(formList.Count(), Math.Max(templateList.Count(), userList.Count()));
                var userCount = 0;

                //foreach(var template in templateList)
                //{
                //    foreach(var form in formList)
                //    {

                //        string bulkInsertQuery = $@"INSERT INTO FORM_TEMPLATE_MAPPING(FORM_TEMPLATE_ID,FORM_CODE,TEMPLATE_NAME,USER_ID,COMPANY_CODE,DELETED_FLAG,MODIFIED_BY,MODIFIED_DATE,CREATED_BY,CREATED_DATE) values(PRINTTEMPLATE_SEQ.nextval,'{form}','{template}','{userList[userCount]}','{mappedFormTemplate.COMPANY_EDESC}', 'N','{this._workContext.CurrentUserinformation.USER_NO}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR'),'{this._workContext.CurrentUserinformation.USER_NO}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR'))";
                //        try
                //        {
                //            var insertRow = _dbContext.ExecuteSqlCommand(bulkInsertQuery);
                //        }catch(Exception ex)
                //        {
                //            throw ex;
                //        }
                //        userCount++;
                //    }
                //}
                for (int i = 0; i < totalBulkCount; i++)
                {
                    string bulkInsertQuery = $@"INSERT INTO FORM_TEMPLATE_MAPPING(FORM_TEMPLATE_ID,FORM_CODE,TEMPLATE_NAME,USER_ID,COMPANY_CODE,DELETED_FLAG,MODIFIED_BY,MODIFIED_DATE,CREATED_BY,CREATED_DATE) values(PRINTTEMPLATE_SEQ.nextval,'{formList[i]}','{templateList[i]}','{userList[i]}','{mappedFormTemplate.COMPANY_EDESC}', 'N','{this._workContext.CurrentUserinformation.USER_NO}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR'),'{this._workContext.CurrentUserinformation.USER_NO}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR'))";
                    try
                     {
                            var insertRow = _dbContext.ExecuteSqlCommand(bulkInsertQuery);
                     }
                    catch (Exception ex)
                        {
                            throw ex;
                        }
                 }
                return "sucessfull";
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while saving bulk template mapping " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string AddSingleMappedTemplate(MappedFormTemplateModel mappedFormTemplateModel)
        {

            try
            {
                string insertQuery = $@"INSERT INTO FORM_TEMPLATE_MAPPING(FORM_TEMPLATE_ID,FORM_CODE,TEMPLATE_NAME,USER_ID,COMPANY_CODE,DELETED_FLAG,MODIFIED_BY,MODIFIED_DATE,CREATED_BY,CREATED_DATE,TEMPLATE_PATH) values(PRINTTEMPLATE_SEQ.nextval,'{mappedFormTemplateModel.FORM_NAME}','{mappedFormTemplateModel.TEMPLATE_NAME}','{mappedFormTemplateModel.USER_NO}','{_workContext.CurrentUserinformation.company_code}', 'N','{this._workContext.CurrentUserinformation.USER_NO}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR'),'{this._workContext.CurrentUserinformation.USER_NO}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/DD/RRRR'),'{mappedFormTemplateModel.TEMPLATE_PATH}')";
                var insertResponse = _dbContext.ExecuteSqlCommand(insertQuery);

                _logErp.InfoInFile(insertResponse + " row inserted");
                return "SuccessFull";
            }
            catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while adding form to template" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public string DeleteTemplateMapping(string formCode,string templateName)
        {
          
            var num = 0;
            try
            {
                
                string deleteQuery = $@"DELETE FROM FORM_TEMPLATE_MAPPING WHERE FORM_CODE={formCode} and TEMPLATE_NAME={templateName}";
                deleteQuery = deleteQuery.Replace("\"", "'");
                num = this._dbContext.ExecuteSqlCommand(deleteQuery);
                _logErp.InfoInFile("Mapping template deleted successfully");
                return "Deleted SuccessFully";
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while deleting mapping template" + ex.StackTrace);
                throw ex;
            }
        }

        public List<FormModelToMap> GetAllFormWithCode()
        {
            List<FormModelToMap> availableForm = new List<FormModelToMap>();
            try
            {
                string getFormWithCodeQuery = $@"SELECT DISTINCT FORM_CODE,FORM_CODE as id,FORM_EDESC,FORM_EDESC as label FROM FORM_SETUP WHERE DELETED_FLAG='N'";
                availableForm = _dbContext.SqlQuery<FormModelToMap>(getFormWithCodeQuery).ToList();
                _logErp.InfoInFile(availableForm.Count() + " form with code fetched using " + getFormWithCodeQuery + " query");
                return availableForm;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting form with code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<MappedFormTemplateModel> GetAllMappedFormTemplate(ReportFiltersModel reportFilter)
        {

            List<MappedFormTemplateModel> availableMapping = new List<MappedFormTemplateModel>();
            try
            {

                if (reportFilter.CompanyFilter.Count == 0)
                {
                       // string getAllMapFormTemplateQuery = $@"SELECT FORM_TEMPLATE_ID,FORM_CODE,TEMPLATE_NAME,USER_ID,COMPANY_CODE,DELETED_FLAG,MODIFIED_BY,MODIFIED_DATE,CREATED_BY,CREATED_DATE FROM FORM_TEMPLATE_MAPPING";
                        string getAllMapFormTemplateQuery = $@"SELECT ftm.FORM_TEMPLATE_ID,ftm.FORM_CODE,fs.FORM_EDESC,ftm.TEMPLATE_NAME,ftm.USER_ID,sau.LOGIN_EDESC,ftm.COMPANY_CODE,cs.COMPANY_EDESC,ftm.DELETED_FLAG,ftm.MODIFIED_BY,ftm.MODIFIED_DATE,ftm.CREATED_BY,ftm.CREATED_DATE
                                                               FROM FORM_TEMPLATE_MAPPING ftm INNER JOIN FORM_SETUP fs on fs.form_code = ftm.form_code INNER JOIN SC_APPLICATION_USERS sau on sau.user_no=ftm.user_id INNER JOIN COMPANY_SETUP cs on cs.company_code=ftm.company_code";
                        availableMapping = _dbContext.SqlQuery<MappedFormTemplateModel>(getAllMapFormTemplateQuery).ToList();
                        _logErp.InfoInFile(availableMapping.Count() + " mapped form template found using " + getAllMapFormTemplateQuery + " query");
                        return availableMapping;
                }
                else
                {

                       // string getAllMapFormTemplateQuery = $@"SELECT FORM_TEMPLATE_ID,FORM_CODE,TEMPLATE_NAME,USER_ID,COMPANY_CODE,DELETED_FLAG,MODIFIED_BY,MODIFIED_DATE,CREATED_BY,CREATED_DATE FROM FORM_TEMPLATE_MAPPING WHERE COMPANY_CODE IN('{reportFilter.CompanyFilter}')";
                        string getAllMapFormTemplateQuery = $@"SELECT ftm.FORM_TEMPLATE_ID,ftm.FORM_CODE,fs.FORM_EDESC,ftm.TEMPLATE_NAME,ftm.USER_ID,sau.LOGIN_EDESC,ftm.COMPANY_CODE,cs.COMPANY_EDESC,ftm.DELETED_FLAG,ftm.MODIFIED_BY,ftm.MODIFIED_DATE,ftm.CREATED_BY,ftm.CREATED_DATE
                                                               FROM FORM_TEMPLATE_MAPPING ftm INNER JOIN FORM_SETUP fs on fs.form_code = ftm.form_code INNER JOIN SC_APPLICATION_USERS sau on sau.user_no=ftm.user_id INNER JOIN COMPANY_SETUP cs on cs.company_code=ftm.company_code WHERE ftm.COMPANY_CODE IN('{reportFilter.CompanyFilter}')";
                        availableMapping = _dbContext.SqlQuery<MappedFormTemplateModel>(getAllMapFormTemplateQuery).ToList();
                        _logErp.InfoInFile(availableMapping.Count() + " mapped form template found using " + getAllMapFormTemplateQuery + " query");
                        return availableMapping;

                   
                }


            }
            catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting all mapped template :" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<TemplateModelToMap> GetAllTemplateWithCode()
        {
            _logErp.InfoInFile("Getting list of available print template");
            List<TemplateModelToMap> templateModelToMaps=new List<TemplateModelToMap>();
            try
            {
                string path = HttpContext.Current.Server.MapPath("~/App_Data/AvailablePrintTemplate.txt");
                _logErp.InfoInFile(path + " is a location where print template information are saved");
                using (StreamReader reader = new StreamReader(path))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        templateModelToMaps.Add(new TemplateModelToMap {id=line.Split(',')[0].ToString(), TEMPLATE_NAME = line.Split(',')[0].ToString(), PARTIAL_VIEW_NAME = line.Split(',')[1].ToString(),label=line.Split(',')[1].ToString(),TEMPLATE_PATH=line.Split(',')[2].ToString()});
                    }
                }
                _logErp.InfoInFile(templateModelToMaps.Count + " available template are found");
                return templateModelToMaps;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting template info from AvailablePrintTemplate.txt file :" + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<TemplateModelToMap> GetMappedTemplateForEdit(string formCode, string templateName)
        {
            List<TemplateModelToMap> templateToEdit = new List<TemplateModelToMap>();
            try
            {
                string dataToEdit = $@"SELECT FORM_CODE,TEMPLATE_NAME FROM FORM_TEMPLATE_MAPPING ftm WHERE ftm.FORM_CODE={formCode} and ftm.TEMPLATE_NAME={templateName}";
                dataToEdit = dataToEdit.Replace("\"", "'");
                templateToEdit = this._dbContext.SqlQuery<TemplateModelToMap>(dataToEdit).ToList();
                return templateToEdit;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while edition mapped template " + ex.StackTrace);
                throw ex;
            }
        }

        public List<MenuControlEntities> GetUsersWithCompany()
        {
            var query = $@"SELECT  TO_CHAR(USER_NO) as id, LOGIN_EDESC as label, COMPANY_CODE as COMPANY_EDESC from sc_application_users  WHERE deleted_flag='N'";
            
            List<MenuControlEntities> menuControlList;
            try
            {
                menuControlList = _dbContext.SqlQuery<MenuControlEntities>(query).ToList();
                _logErp.InfoInFile(menuControlList.Count() + " record of user with comapny fetched using : " + query + " query ");
            }
            catch (Exception ex)
            {
                _logErp.ErrorInDB("Error while getting company with user: " + ex.StackTrace);
                throw ex;
            }
            return menuControlList;
        }

        public string UpdateMappedTemplate(MappedFormTemplateModel mapppedModelToUpdate)
        {
            try
            {
                string MenucountSql = $@"SELECT FORM_CODE,TEMPLATE_NAME FROM FORM_TEMPLATE_MAPPING ftm WHERE ftm.FORM_CODE='{mapppedModelToUpdate.FORM_NAME}' and ftm.TEMPLATE_NAME='{mapppedModelToUpdate.TEMPLATE_NAME}'";
                var count = _objectEntity.SqlQuery<List<int>>(MenucountSql).SingleOrDefault();
                if (count !=null)
                {
                    return null;
                }

                var num = 0;     
                string MenuSql = $@"UPDATE FORM_TEMPLATE_MAPPING SET TEMPLATE_NAME='{mapppedModelToUpdate.TEMPLATE_NAME}' WHERE USER_ID='{mapppedModelToUpdate.USER_NO}' AND FORM_CODE ='{mapppedModelToUpdate.FORM_NAME}'"; 
                num = this._dbContext.ExecuteSqlCommand(MenuSql);
                return num.ToString() + " : Record updated successfully";

            }
            catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while updating mapped template : " + ex.StackTrace);
                return "Error";
               // throw new Exception(ex.Message);
            }
        }
    }
}