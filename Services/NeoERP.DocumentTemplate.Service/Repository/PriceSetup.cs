using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Repository
{
    public class PriceSetup:IPriceSetup
    {
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private DefaultValueForLog _defaultValueForLog;
        private ILogErp _logErp;
        private NeoErpCoreEntity _objectEntity;


        public PriceSetup(IWorkContext workContext,IDbContext dbContext, NeoErpCoreEntity objectEntity)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
            this._objectEntity = objectEntity;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            _logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule,_defaultValueForLog.FormCode);
        }

        public List<string> GetAllSavedItemsName()
        {
            List<string> itemName = new List<string>();
            try
            {
                string itemQuery = $@"SELECT psm.PRICE_LIST_NAME FROM PRICE_SETUP_MASTER psm";
                itemName = this._dbContext.SqlQuery<string>(itemQuery).ToList();
                return itemName;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting saved Item " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public List<PriceSetupModel> GetItemByCompany(string selectdCompany)
        {
            List<PriceSetupModel> modelByCompany = new List<PriceSetupModel>();
            try
            {
                string itemQuery = $@"SELECT itms.ITEM_EDESC,itms.ITEM_CODE,itms.INDEX_MU_CODE,MU.MU_EDESC FROM IP_ITEM_MASTER_SETUP itms, IP_MU_CODE MU WHERE  itms.COMPANY_CODE='{selectdCompany}' and itms.DELETED_FLAG='N' AND itms.COMPANY_CODE=MU.COMPANY_CODE  AND itms.INDEX_MU_CODE=MU.MU_CODE";
                _logErp.InfoInFile(itemQuery + " is a query to fetch item based on company");
                modelByCompany = this._dbContext.SqlQuery<PriceSetupModel>(itemQuery).ToList();
                return modelByCompany;
            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error while getting item by company code : " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

        public Tuple<List<PriceSetupModel>,MasterField> GetItemToEdit(string selectedPriceName)
        {
            List<PriceSetupModel> itemForEdit = new List<PriceSetupModel>();
            MasterField masterFields = new MasterField();
            try
            {
                var masterItemQuery = $@"SELECT psm.MASTER_ID,psm.PRICE_LIST_NAME,psm.COMPANY FROM PRICE_SETUP_MASTER psm WHERE psm.PRICE_LIST_NAME={selectedPriceName}";
                masterItemQuery = masterItemQuery.Replace("\"", "'");
                masterFields = this._dbContext.SqlQuery<MasterField>(masterItemQuery).FirstOrDefault();
                var itemToEditQuery = $@"SELECT itms.ITEM_EDESC,itms.ITEM_CODE,itms.INDEX_MU_CODE,MU.MU_EDESC,psc.OLD_PRICE,psc.NEW_PRICE FROM IP_ITEM_MASTER_SETUP itms LEFT JOIN PRICE_SETUP_CHILD psc on psc.ITEM_NAME = itms.ITEM_EDESC AND psc.MASTER_ID='{masterFields.MASTER_ID}',IP_MU_CODE MU WHERE itms.COMPANY_CODE = {masterFields.COMPANY} and itms.DELETED_FLAG = 'N' AND itms.COMPANY_CODE = MU.COMPANY_CODE  AND itms.INDEX_MU_CODE = MU.MU_CODE";
                itemToEditQuery = itemToEditQuery.Replace("\"", "'");
                itemForEdit = this._dbContext.SqlQuery<PriceSetupModel>(itemToEditQuery).ToList();
                return Tuple.Create(itemForEdit, masterFields);

            }catch(Exception ex)
            {
                _logErp.ErrorInDB("Error While getting item to edit " + ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }

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

        public string SaveUpdatedCell(SaveModelForPriceSetup saveModel)
        {
            var modifiedUser = this._workContext.CurrentUserinformation.User_id;
            string result = "";
            using (var trans = _objectEntity.Database.BeginTransaction())
            {
                try
                {
                    if (saveModel.MasterField == null)
                    {
                        result = "Master Details is Empty";
                        return result;
                    }
                    else if (saveModel.MasterField.isUpdated)
                    {
                        try
                        {

                        string getMasterValue = $@"SELECT MASTER_ID,PRICE_LIST_NAME,STATUS,COMPANY,DATE_ENGLISH,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE FROM PRICE_SETUP_MASTER psm WHERE psm.PRICE_LIST_NAME='{saveModel.MasterField.PriceListName}'";
                        MasterFieldForUpdate masterToUpdate = this._dbContext.SqlQuery<MasterFieldForUpdate>(getMasterValue).Single();
                        var status = saveModel.MasterField.Status == true ? 1 : 0;
                       
                        string updateMasterQuery = $@"UPDATE PRICE_SETUP_MASTER psm set psm.PRICE_LIST_NAME='{saveModel.MasterField.PriceListName}',psm.STATUS='{status}',psm.COMPANY='{saveModel.MasterField.CompanyName}',psm.MODIFIED_BY='{modifiedUser}',psm.MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/RRRR') WHERE psm.MASTER_ID={masterToUpdate.MASTER_ID}";
                        this._objectEntity.ExecuteSqlCommand(updateMasterQuery);
                        // trans.Commit();


                        foreach (var childToDel in saveModel.ChildField)
                            {
                                string deleteChildQuery = $@"DELETE FROM PRICE_SETUP_CHILD psc WHERE psc.ITEM_NAME='{childToDel.ITEM_EDESC}'";
                                this._objectEntity.ExecuteSqlCommand(deleteChildQuery);
                            }

                          
                            foreach (var childToSave in saveModel.ChildField)
                            {
                                string childQuery = $@"INSERT INTO PRICE_SETUP_CHILD(CHILD_ID,ITEM_NAME,OLD_PRICE,NEW_PRICE,MASTER_ID,STATUS,ITEM_UNIT,ITEM_CODE) VALUES(PRICE_SETUP_CHILD_SEQ.nextVal,'{childToSave.ITEM_EDESC}','{childToSave.NEW_PRICE}','{0}',{masterToUpdate.MASTER_ID},'Yes','{childToSave.MU_EDESC}','{childToSave.ITEM_CODE}')";
                                var updatedRow = _dbContext.ExecuteSqlCommand(childQuery);
                            } 

                            trans.Commit();
                            result = "Price Setup Updated Successfully";
                            return result;

                        }
                        catch (Exception ex)
                        {
                            trans.Rollback(); 
                            result = "Error while deleting price setup" + ex.StackTrace;
                            _logErp.ErrorInDB("Error while deleting price setup" + ex.StackTrace);
                            throw new Exception(ex.Message);
                        }
                    }
                    else
                    {
                        try
                        {
                            int insertedMasterRow = 0;
                            string masterQuery = $@"INSERT INTO PRICE_SETUP_MASTER(MASTER_ID,PRICE_LIST_NAME,STATUS,COMPANY,DATE_ENGLISH,CREATED_BY,CREATED_DATE,MODIFIED_BY,MODIFIED_DATE) VALUES(PRICE_SETUP_MASTER_SEQ.nextval,'{saveModel.MasterField.PriceListName}',1,'{saveModel.MasterField.CompanyName}',TO_DATE('{saveModel.MasterField.DateEnglish.ToString("MM/dd/yyyy")}','MM/DD/RRRR'),'{modifiedUser}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/RRRR'),'{modifiedUser}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/DD/RRRR'))";
                            _dbContext.ExecuteSqlCommand(masterQuery);
                            insertedMasterRow = _dbContext.SqlQuery<int>($@"SELECT MAX(MASTER_ID) FROM PRICE_SETUP_MASTER").Single();
                            foreach (var childToSave in saveModel.ChildField)
                            {
                                string childQuery = $@"INSERT INTO PRICE_SETUP_CHILD(CHILD_ID,ITEM_NAME,OLD_PRICE,NEW_PRICE,MASTER_ID,STATUS,ITEM_UNIT,ITEM_CODE) VALUES(PRICE_SETUP_CHILD_SEQ.nextVal,'{childToSave.ITEM_EDESC}','{childToSave.NEW_PRICE}','{0}',{insertedMasterRow},'Yes','{childToSave.MU_EDESC}','{childToSave.ITEM_CODE}')";
                                var updatedRow = _dbContext.ExecuteSqlCommand(childQuery);
                            }
                           trans.Commit();
                           result = "Price setup created succssfully";
                        return result;
                        }
                        catch(Exception ex)
                        {
                            // trans.Rollback();
                            result = "Error while adding price setup : " + ex.StackTrace;
                            _logErp.ErrorInDB("Error while inserting setup " + ex.StackTrace);
                            throw new Exception(ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                   trans.Rollback();
                    result = "Error while deleting price setup" + ex.StackTrace;
                    _logErp.ErrorInDB("Error while saving price list : " + ex.StackTrace);
                    throw new Exception(ex.Message);
                }
            }
        }



    }
}
