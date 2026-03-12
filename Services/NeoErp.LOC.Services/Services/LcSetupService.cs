using NeoErp.Core;
using NeoErp.Data;
using NeoErp.LOC.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoErp.LOC.Services.Services
{
    public class LcSetupService : ILcSetupService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public LcSetupService(IDbContext dbContext, IWorkContext workcontext)
        {
            this._dbContext = dbContext;
            this._workcontext = workcontext;
        }

        #region LCBank 

        public void AddUpdateLcBank(LCBankModels lcbank)
        {
            if (lcbank.BANK_CODE == 0)
            {
                var insertQuery = string.Format(@"INSERT INTO LC_BANK(BANK_CODE,BANK_EDESC,BANK_NDESC,BANK_ADDRESS,BANK_ACC_NO,SWIFT_CODE,BRANCH,PHONE_NO,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({0}, '{1}', '{2}', '{3}', '{4}','{5}','{6}','{7}','{8}','{9}','{10}',TO_DATE('{11}', 'mm/dd/yyyy hh24:mi:ss'),'{12}')",
                    "LCBANK_SEQ.nextval", lcbank.BANK_NAME, lcbank.BANK_NAME, lcbank.ADDRESS, lcbank.BANK_ACC_NO, lcbank.SWIFT_CODE, lcbank.BRANCH, lcbank.PHONE_NO, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = string.Format(@"UPDATE LC_BANK SET BANK_EDESC  = '{0}',BANK_NDESC= '{1}', BANK_ADDRESS='{2}',BANK_ACC_NO='{3}',SWIFT_CODE = '{4}',BRANCH='{5}',PHONE_NO = '{6}',LAST_MODIFIED_BY = '{7}',LAST_MODIFIED_DATE = TO_DATE('{8}', 'mm/dd/yyyy hh24:mi:ss') WHERE BANK_CODE IN ({9})",
           lcbank.BANK_NAME, lcbank.BANK_NAME, lcbank.ADDRESS, lcbank.BANK_ACC_NO, lcbank.SWIFT_CODE, lcbank.BRANCH, lcbank.PHONE_NO, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), lcbank.BANK_CODE);
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
        }

        public void deleteLCBanks(int BankCode)
        {
            string query = string.Format(@"UPDATE LC_BANK SET DELETED_FLAG  = '{0}' WHERE BANK_CODE IN ({1})",
            'Y', BankCode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<LCBankModels> getAllLCBanks()
        {
            var sqlquery = $@"select BANK_CODE,BANK_ACC_NO,SWIFT_CODE,BRANCH,PHONE_NO, BANK_EDESC AS BANK_NAME, BANK_ADDRESS AS ADDRESS from LC_BANK where deleted_flag='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var banks = _dbContext.SqlQuery<LCBankModels>(sqlquery).ToList();
            return banks;
        }


        #endregion

        #region LC Term

        public void deleteLCTerm(int TermCode)
        {
            string query = string.Format(@"UPDATE LC_TERMS SET DELETED_FLAG  = '{0}' WHERE TERMS_CODE IN ({1})",
          'Y', TermCode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public void AddUpdateLcTerm(LCTermModels lcterm)
        {
            if (lcterm.TermCode == 0)
            {
                var insertQuery = string.Format(@"INSERT INTO LC_TERMS(TERMS_CODE,TERMS_EDESC,TERMS_NDESC,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({0}, '{1}', '{2}', '{3}', '{4}','{5}',TO_DATE('{6}', 'mm/dd/yyyy hh24:mi:ss'),'{7}')",
                    "LCTERMS_SEQ.nextval", lcterm.TermName, lcterm.TermName, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = string.Format(@"UPDATE LC_TERMS SET TERMS_EDESC  = '{0}',TERMS_NDESC= '{1}', LAST_MODIFIED_BY = '{2}',LAST_MODIFIED_DATE = TO_DATE('{3}', 'mm/dd/yyyy hh24:mi:ss') WHERE TERMS_CODE IN ({4})",
           lcterm.TermName, lcterm.TermName, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), lcterm.TermCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
        }

        public List<LCTermModels> getAllLCTerms()
        {
            var sqlquery = $@"select TERMS_CODE as TermCode, TERMS_EDESC as TermName from LC_TERMS where deleted_flag='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var terms = _dbContext.SqlQuery<LCTermModels>(sqlquery).ToList();
            return terms;
        }


        #endregion

        #region LC Status
        public void AddUpdateLcStatus(LCStatusModels lcstatus)
        {
            if (lcstatus.StatusCode == 0)
            {
                var insertQuery = string.Format(@"INSERT INTO LC_STATUS(STATUS_CODE,STATUS_EDESC,STATUS_NDESC,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({0}, '{1}', '{2}', '{3}', '{4}','{5}',TO_DATE('{6}', 'mm/dd/yyyy hh24:mi:ss'),'{7}')",
                    "LCSTATUS_SEQ.nextval", lcstatus.StatusName, lcstatus.StatusName, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = string.Format(@"UPDATE LC_STATUS SET STATUS_EDESC  = '{0}',STATUS_NDESC= '{1}', LAST_MODIFIED_BY = '{2}',LAST_MODIFIED_DATE = TO_DATE('{3}', 'mm/dd/yyyy hh24:mi:ss') WHERE STATUS_CODE IN ({4})",
           lcstatus.StatusName, lcstatus.StatusName, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), lcstatus.StatusCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
        }

        public void deleteLCStatus(int StatusCode)
        {
            string query = string.Format(@"UPDATE LC_STATUS SET DELETED_FLAG  = '{0}' WHERE STATUS_CODE IN ({1})",
             'Y', StatusCode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<LCStatusModels> getAllLCStatus()
        {
            var sqlquery = $@"select STATUS_CODE as StatusCode, STATUS_EDESC as StatusName from LC_STATUS where deleted_flag='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var status = _dbContext.SqlQuery<LCStatusModels>(sqlquery).ToList();
            return status;
        }

        public void AddUpdateLcBeneficiary(LCBeneficiaryModels lcbeneficiary)
        {
            if (lcbeneficiary.BNF_CODE == 0)
            {
                var insertquey = string.Format(@"INSERT INTO LC_BENEFICIARY(BNF_CODE,BNF_EDESC,BNF_NDESC,COUNTRY_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,ADDRESS,SUPPLIER_CODE)VALUES({0}, '{1}', '{2}','{3}','{4}', '{5}','{6}','{7}',TO_DATE('{8}', 'mm/dd/yyyy hh24:mi:ss'),'{9}','{10}','{11}')",
                   "LCSTATUS_SEQ.nextval", lcbeneficiary.BNF_EDESC, lcbeneficiary.BNF_EDESC, (lcbeneficiary.COUNTRY_CODE), lcbeneficiary.REMARKS, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), 'N', lcbeneficiary.ADDRESS, lcbeneficiary.SUPPLIER_CODE);
                var rowCount = _dbContext.ExecuteSqlCommand(insertquey);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = string.Format(@"UPDATE LC_BENEFICIARY SET BNF_EDESC  = '{0}',BNF_NDESC= '{1}',ADDRESS = '{7}',COUNTRY_CODE='{2}',REMARKS='{3}', LAST_MODIFIED_BY = '{4}',LAST_MODIFIED_DATE = TO_DATE('{5}', 'mm/dd/yyyy hh24:mi:ss'),SUPPLIER_CODE='{8}' WHERE BNF_CODE IN ({6})",
           lcbeneficiary.BNF_EDESC, lcbeneficiary.BNF_EDESC, lcbeneficiary.COUNTRY_CODE, lcbeneficiary.REMARKS, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), lcbeneficiary.BNF_CODE, lcbeneficiary.ADDRESS, lcbeneficiary.SUPPLIER_CODE);
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
        }

        public void DeleteLCBeneficiary(int BNF_CODE)
        {
            string query = string.Format(@"UPDATE LC_BENEFICIARY SET DELETED_FLAG  = '{0}' WHERE BNF_CODE IN ({1})",
           'Y', BNF_CODE);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<LCBeneficiaryModels> GetAllLCBeneficiary()
        {
            var query = $@"SELECT LB.BNF_CODE,LB.BNF_EDESC,LB.BNF_NDESC,LB.ADDRESS,LB.COUNTRY_CODE,CS.COUNTRY_EDESC,LB.REMARKS,ISS.SUPPLIER_EDESC AS SUPPLIER_EDESC,ISS.SUPPLIER_CODE AS SUPPLIER_CODE
                FROM LC_BENEFICIARY LB 
                LEFT JOIN COUNTRY_SETUP CS ON LB.COUNTRY_CODE = CS.COUNTRY_CODE AND LB.COMPANY_CODE = CS.COMPANY_CODE
                LEFT JOIN IP_SUPPLIER_SETUP ISS ON LB.SUPPLIER_CODE=ISS.SUPPLIER_CODE
                WHERE LB.DELETED_FLAG ='N' AND LB.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var Beneficiary = _dbContext.SqlQuery<LCBeneficiaryModels>(query).ToList();
            return Beneficiary;
        }

        public bool AddUpdateHs(LCHSModals lchs)
        {
            var hsquery = $@"SELECT HS_NO, HS_CODE,DELETED_FLAG FROM LC_HS";
            List<LCHSModals> result = _dbContext.SqlQuery<LCHSModals>(hsquery).ToList();
            var hsCodeExists = result.Any(x => x.HS_CODE == lchs.HS_CODE && x.HS_NO != lchs.HS_NO && x.DELETED_FLAG == 'N');
            if (hsCodeExists)
            {
                return false;
            }

            if (lchs.HS_NO == 0)
            {


                var maxHsCodeQuery = $@"SELECT COALESCE(MAX(HS_NO)+1,1) FROM LC_HS";
                int maxHsNo = _dbContext.SqlQuery<int>(maxHsCodeQuery).FirstOrDefault();
                var insertquey = $@"INSERT INTO LC_HS(HS_NO,HS_CODE,HS_EDESC,HS_NDESC,DUTY_RATE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({maxHsNo}, '{lchs.HS_CODE}', '{lchs.HS_NDESC}','{lchs.HS_NDESC}','{lchs.DUTY_RATE}', '{lchs.REMARKS}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertquey);
                _dbContext.SaveChanges();
            }
            else
            {

                string query = $@"UPDATE LC_HS SET HS_CODE='{lchs.HS_CODE}', HS_EDESC= '{lchs.HS_NDESC}',HS_NDESC= '{lchs.HS_NDESC}',DUTY_RATE='{lchs.DUTY_RATE}',REMARKS='{lchs.REMARKS}', LAST_MODIFIED_BY = '{ _workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE HS_NO = ({lchs.HS_NO})";
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
            return true;
        }


        public List<LCHSModals> getallhs()
        {
            string query = $@"select HS_NO,HS_EDESC,HS_NDESC,DUTY_RATE,REMARKS,COMPANY_CODE ,HS_CODE FROM lc_hs WHERE DELETED_FLAG ='N'";
            var lchs = _dbContext.SqlQuery<LCHSModals>(query).ToList();
            return lchs;
        }

        public void deleteHs(string HS_CODE)
        {
            string query = string.Format(@"UPDATE LC_HS SET DELETED_FLAG  = '{0}' WHERE HS_CODE = ('{1}')",
           'Y', HS_CODE);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }
        #endregion

        #region LC Payment Terms
        public void AddUpdatePLcTerm(LCPTermModels lcpterm)
        {
            if (lcpterm.PTermCode == 0)
            {
                var insertQuery = string.Format(@"INSERT INTO LC_PAYMENT_TERMS(PTERMS_CODE,PTERMS_EDESC,PTERMS_NDESC,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({0}, '{1}', '{2}', '{3}', '{4}','{5}',TO_DATE('{6}', 'mm/dd/yyyy hh24:mi:ss'),'{7}')",
                    "LCPTERMS_SEQ.nextval", lcpterm.PTermName, lcpterm.PTermName, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = string.Format(@"UPDATE LC_PAYMENT_TERMS SET PTERMS_EDESC  = '{0}',PTERMS_NDESC= '{1}', LAST_MODIFIED_BY = '{2}',LAST_MODIFIED_DATE = TO_DATE('{3}', 'mm/dd/yyyy hh24:mi:ss') WHERE PTERMS_CODE IN ({4})",
           lcpterm.PTermName, lcpterm.PTermName, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), lcpterm.PTermCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
        }

        public void deleteLCPTerm(int PTermCode)
        {
            string query = string.Format(@"UPDATE LC_PAYMENT_TERMS SET DELETED_FLAG  = '{0}' WHERE PTERMS_CODE IN ({1})",
           'Y', PTermCode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<LCPTermModels> getAllLCPTerms()
        {
            var sqlquery = $@"select PTERMS_CODE as PTermCode, PTERMS_EDESC as PTermName from LC_PAYMENT_TERMS where deleted_flag='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var pterms = _dbContext.SqlQuery<LCPTermModels>(sqlquery).ToList();
            return pterms;
        }
        #endregion


        #region Location Setup

        public bool AddLocation(LocationModels locationdetails)
        {
            var locationquery = $@"SELECT LOCATION_CODE,LOCATION_EDESC,DELETED_FLAG FROM LC_LOCATION_SETUP";
            List<LocationModels> result = _dbContext.SqlQuery<LocationModels>(locationquery).ToList();
            var locationListExists = result.Any(x => x.LOCATION_EDESC == locationdetails.LOCATION_EDESC && x.LOCATION_CODE != locationdetails.LOCATION_CODE && x.DELETED_FLAG == 'N');
            if (locationListExists)
            {
                return false;
            }
            if (locationdetails.LOCATION_CODE == 0)
            {
                var maxlocationquery = $@"SELECT COALESCE(MAX(LOCATION_CODE)+1,1) FROM LC_LOCATION_SETUP";
                int maxlocationcode = _dbContext.SqlQuery<int>(maxlocationquery).FirstOrDefault();
                var insertQuery = $@"INSERT INTO LC_LOCATION_SETUP(LOCATION_CODE,LOCATION_ID,LOCATION_EDESC,LOCATION_NDESC,MAX_STORING_DAYS,PER_DAY_CHARGE,CURRENCY_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{maxlocationcode}','{locationdetails.LOCATION_ID}','{locationdetails.LOCATION_EDESC}','{locationdetails.LOCATION_EDESC}','{locationdetails.MAX_STORING_DAYS}','{locationdetails.PER_DAY_CHARGE}','{locationdetails.CURRENCY_CODE}','{ _workcontext.CurrentUserinformation.company_code}','{ _workcontext.CurrentUserinformation.branch_code}', '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = $@"UPDATE LC_LOCATION_SETUP SET LOCATION_ID  = '{locationdetails.LOCATION_ID}',LOCATION_EDESC= '{locationdetails.LOCATION_EDESC}',LOCATION_NDESC = '{locationdetails.LOCATION_EDESC}',CURRENCY_CODE= '{locationdetails.CURRENCY_CODE}',MAX_STORING_DAYS ='{locationdetails.MAX_STORING_DAYS}',PER_DAY_CHARGE = '{locationdetails.PER_DAY_CHARGE}', LAST_MODIFIED_BY = '{ _workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE =TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LOCATION_CODE = ('{locationdetails.LOCATION_CODE}')";
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
            return true;
        }

        public List<LocationModels> getAllLocations()
        {
            var sqlquery = $@"SELECT LOCATION_CODE,LOCATION_ID,LOCATION_EDESC,MAX_STORING_DAYS,PER_DAY_CHARGE,CURRENCY_CODE FROM LC_LOCATION_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var locationlist = _dbContext.SqlQuery<LocationModels>(sqlquery).ToList();
            return locationlist;
        }

        public void deleteLocations(string LocationCode)
        {
            var sqlquery = $@"UPDATE LC_LOCATION_SETUP SET DELETED_FLAG = 'Y' WHERE LOCATION_CODE = '{LocationCode}'";
            _dbContext.ExecuteSqlCommand(sqlquery);
        }



        #endregion

        #region Document Setup

        public void AddDocumentInfo(DocumentModels documentdetails)
        {
            if (documentdetails.DOCUMENT_CODE == 0)
            {
                var maxdocumentquery = $@"SELECT COALESCE(MAX(DOCUMENT_CODE)+1,1) FROM LC_DOCUMENT_SETUP";
                int maxdocumentcode = _dbContext.SqlQuery<int>(maxdocumentquery).FirstOrDefault();
                var insertQuery = $@"INSERT INTO LC_DOCUMENT_SETUP(DOCUMENT_CODE,DOCUMENT_EDESC,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{maxdocumentcode}','{documentdetails.DOCUMENT_EDESC}','{documentdetails.REMARKS}','{ _workcontext.CurrentUserinformation.company_code}','{ _workcontext.CurrentUserinformation.branch_code}', '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string query = $@"UPDATE LC_DOCUMENT_SETUP SET DOCUMENT_EDESC= '{documentdetails.DOCUMENT_EDESC}',REMARKS= '{documentdetails.REMARKS}', LAST_MODIFIED_BY = '{ _workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE =TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE DOCUMENT_CODE= ('{documentdetails.DOCUMENT_CODE}')";
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }
        }

        public List<DocumentModels> getAllDocumentInfo()
        {
            var sqlquery = $@"SELECT DOCUMENT_CODE,DOCUMENT_EDESC,REMARKS FROM LC_DOCUMENT_SETUP WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var locationlist = _dbContext.SqlQuery<DocumentModels>(sqlquery).ToList();
            return locationlist;
        }

        public void deleteDocumentInfo(string documentCode)
        {
            var sqlquery = $@"UPDATE LC_DOCUMENT_SETUP SET DELETED_FLAG = 'Y' WHERE DOCUMENT_CODE= '{documentCode}'";
            _dbContext.ExecuteSqlCommand(sqlquery);
        }
        #endregion

        #region Issuing Carrier

        public string AddUpdateIc(IssuingCarrierModels lcic)
        {
            if (lcic.CARRIER_CODE == 0)
            {
                var maxIcCodeQuery = $@"SELECT COALESCE(MAX(CARRIER_CODE)+1,1) FROM LC_ISSUING_CARRIER";
                int maxIcNo = _dbContext.SqlQuery<int>(maxIcCodeQuery).FirstOrDefault();
                var insertquey = $@"INSERT INTO LC_ISSUING_CARRIER(CARRIER_CODE,CARRIER_EDESC,CARRIER_NDESC,CARRIER_ADDRESS1,CARRIER_ADDRESS2,CARRIER_PHONE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({maxIcNo},'{lcic.CARRIER_EDESC}','{lcic.CARRIER_NDESC}','{lcic.CARRIER_ADDRESS1}', '{lcic.CARRIER_ADDRESS2}','{lcic.CARRIER_PHONE_NO}','{lcic.REMARKS}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertquey);
                _dbContext.SaveChanges();

                return "success";

            }
            else
            {

                string query = $@"UPDATE LC_ISSUING_CARRIER SET CARRIER_CODE='{lcic.CARRIER_CODE}', CARRIER_EDESC= '{lcic.CARRIER_EDESC}',CARRIER_NDESC= '{lcic.CARRIER_NDESC}',CARRIER_ADDRESS1='{lcic.CARRIER_ADDRESS1}',CARRIER_ADDRESS2='{lcic.CARRIER_ADDRESS2}',CARRIER_PHONE='{lcic.CARRIER_PHONE_NO}',REMARKS='{lcic.REMARKS}', LAST_MODIFIED_BY = '{ _workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE CARRIER_CODE = ({lcic.CARRIER_CODE})";
                var rowCount = _dbContext.ExecuteSqlCommand(query);
                return "success";

            }

        }

        public List<IssuingCarrierModels> getallic()
        {
            string query = $@"select CARRIER_CODE,CARRIER_EDESC,CARRIER_NDESC,CARRIER_ADDRESS1,CARRIER_ADDRESS2,CARRIER_PHONE as CARRIER_PHONE_NO,REMARKS,COMPANY_CODE FROM LC_ISSUING_CARRIER WHERE DELETED_FLAG ='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var lcic = _dbContext.SqlQuery<IssuingCarrierModels>(query).ToList();
            return lcic;
        }
        public void deleteIc(string CARRIER_CODE)
        {
            string query = string.Format(@"UPDATE LC_ISSUING_CARRIER SET DELETED_FLAG  = '{0}' WHERE CARRIER_CODE = ('{1}')",
           'Y', CARRIER_CODE);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }
        #endregion
        #region CONTAINER
        public string AddUpdateContainer(ContainerModels lcc)
        {
            if (lcc.CONTAINER_CODE == 0)
            {
                var maxcontainerCodeQuery = $@"SELECT COALESCE(MAX(CONTAINER_CODE)+1,1) FROM LC_CONTAINER";
                int maxcontainerNo = _dbContext.SqlQuery<int>(maxcontainerCodeQuery).FirstOrDefault();
                var insertquey = $@"INSERT INTO LC_CONTAINER(CONTAINER_CODE,CONTAINER_EDESC,CONTAINER_NDESC,CONTAINER_NO,CONTAINER_SIZE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES({maxcontainerNo},'{lcc.CONTAINER_EDESC}','{lcc.CONTAINER_NDESC}','{lcc.CONTAINER_NO}', '{lcc.CONTAINER_SIZE}','{lcc.REMARKS}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertquey);
                _dbContext.SaveChanges();

                return "success";

            }
            else
            {

                string query = $@"UPDATE LC_CONTAINER SET CONTAINER_CODE='{lcc.CONTAINER_CODE}', CONTAINER_EDESC= '{lcc.CONTAINER_EDESC}',CONTAINER_NDESC= '{lcc.CONTAINER_NDESC}',CONTAINER_NO='{lcc.CONTAINER_NO}',CONTAINER_SIZE='{lcc.CONTAINER_SIZE}',REMARKS='{lcc.REMARKS}', LAST_MODIFIED_BY = '{ _workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE CONTAINER_CODE = ({lcc.CONTAINER_CODE})";
                var rowCount = _dbContext.ExecuteSqlCommand(query);
                return "success";

            }

        }
        public List<ContainerModels> getallc()
        {
            string query = $@"select CONTAINER_CODE,CONTAINER_EDESC,CONTAINER_NDESC,CONTAINER_NO,CONTAINER_SIZE,REMARKS  FROM LC_CONTAINER WHERE DELETED_FLAG ='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var lcic = _dbContext.SqlQuery<ContainerModels>(query).ToList();
            return lcic;
        }
        public void deletec(string CONTAINER_CODE)
        {
            string query = string.Format(@"UPDATE LC_CONTAINER SET DELETED_FLAG  = '{0}' WHERE CONTAINER_CODE = ('{1}')",
           'Y', CONTAINER_CODE);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }
        #endregion

        #region lc applicant setup
        public List<LCApplicantBank> GetLC_ApplicantBank()
        {
            try
            {
                var sqlquery = $@"select ACC_CODE AS ACC_CODE, ACC_EDESC AS ACC_EDESC  from fa_chart_of_accounts_setup where ACC_NATURE ='AC' AND ACC_TYPE_FLAG='T' AND DELETED_FLAG='N'  AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                var result = _dbContext.SqlQuery<LCApplicantBank>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ApplicantBankModel> GetallApplicantBankList()
        {
            try
            {
                var sqlquery = $@"  select TO_CHAR(B.BANK_CODE) as BANK_CODE, B.BANK_NAME AS BANK_NAME,B.BANK_ACC_NO AS BANK_ACC_NO,B.BRANCH BRANCH,B.ADDRESS AS ADDRESS,B.LINK_ACC_CODE AS LINK_ACC_CODE,B.PHONE_NO AS PHONE_NO,B.REMARKS AS REMARKS,B.SWIFT_CODE AS SWIFT_CODE
                                  from FA_BANK_SETUP B where B.DELETED_FLAG='N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                //AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                var result = _dbContext.SqlQuery<ApplicantBankModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        public void AddUpdateLcApplicantBank(ApplicantBankModel applicantBankModel)
        {
            if (string.IsNullOrEmpty(applicantBankModel.BANK_CODE))
            {
                var maxCodeQuery = $@"SELECT COALESCE(MAX(BANK_CODE)+1,1) FROM FA_BANK_SETUP WHERE COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                //AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                int maxBankCode = _dbContext.SqlQuery<int>(maxCodeQuery).FirstOrDefault();

                //var insertQuery = $@"INSERT INTO FA_BANK_SETUP(BANK_CODE,BANK_NAME,BANK_ACC_NO,SWIFT_CODE,LINK_ACC_CODE,ADDRESS,BRANCH,PHONE_NO,REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                //                                  VALUES('{maxBankCode}','{applicantBankModel.BANK_NAME}','{applicantBankModel.BANK_ACC_NO}','{applicantBankModel.SWIFT_CODE}','{applicantBankModel.LINK_ACC_CODE}','{applicantBankModel.ADDRESS}','{applicantBankModel.BRANCH}','{applicantBankModel.PHONE_NO}','{applicantBankModel.REMARKS}','{_workcontext.CurrentUserinformation.company_code}', '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N','{_workcontext.CurrentUserinformation.branch_code}')";
                //var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                var insertQuery = $@"INSERT INTO FA_BANK_SETUP(BANK_CODE,BANK_NAME,BANK_ACC_NO,SWIFT_CODE,LINK_ACC_CODE,ADDRESS,BRANCH,PHONE_NO,REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                                                  VALUES('{maxBankCode}','{applicantBankModel.BANK_NAME}','{applicantBankModel.BANK_ACC_NO}','{applicantBankModel.SWIFT_CODE}','{applicantBankModel.LINK_ACC_CODE}','{applicantBankModel.ADDRESS}','{applicantBankModel.BRANCH}','{applicantBankModel.PHONE_NO}','{applicantBankModel.REMARKS}','{_workcontext.CurrentUserinformation.company_code}', '{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                //string updatequery = $@"UPDATE FA_BANK_SETUP SET BANK_NAME  = '{applicantBankModel.BANK_NAME}',BANK_ACC_NO= '{applicantBankModel.BANK_ACC_NO}', SWIFT_CODE='{applicantBankModel.SWIFT_CODE}',LINK_ACC_CODE='{applicantBankModel.LINK_ACC_CODE}',ADDRESS = '{applicantBankModel.ADDRESS}',BRANCH='{applicantBankModel.BRANCH}',PHONE_NO = '{applicantBankModel.PHONE_NO}',MODIFY_BY = '{_workcontext.CurrentUserinformation.User_id}',MODIFY_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),REMARKS='{applicantBankModel.REMARKS}' WHERE BANK_CODE = '{applicantBankModel.BANK_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";

                string updatequery = $@"UPDATE FA_BANK_SETUP SET BANK_NAME  = '{applicantBankModel.BANK_NAME}',BANK_ACC_NO= '{applicantBankModel.BANK_ACC_NO}', SWIFT_CODE='{applicantBankModel.SWIFT_CODE}',LINK_ACC_CODE='{applicantBankModel.LINK_ACC_CODE}',ADDRESS = '{applicantBankModel.ADDRESS}',BRANCH='{applicantBankModel.BRANCH}',PHONE_NO = '{applicantBankModel.PHONE_NO}',MODIFY_BY = '{_workcontext.CurrentUserinformation.User_id}',MODIFY_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),REMARKS='{applicantBankModel.REMARKS}' WHERE BANK_CODE = '{applicantBankModel.BANK_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }

        }

        public void DeleteApplicantBank(string BANK_CODE)
        {
            try
            {
                string updatequery = $@"UPDATE FA_BANK_SETUP SET DELETED_FLAG='Y' WHERE BANK_CODE = '{BANK_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                //AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }
            catch (Exception EX)
            {

                throw EX;
            }
        }

        public bool ApplicantBankAccountNumberExists(string BANK_NAME, string BANK_ACC_NO)
        {
            try
            {
                string selectquery = $@"Select count(*) from  FA_BANK_SETUP WHERE BANK_NAME = '{BANK_NAME}' AND BANK_ACC_NO= '{BANK_ACC_NO}'AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'";
                //AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
                var rowCount = _dbContext.SqlQuery<int>(selectquery).FirstOrDefault();
                if (rowCount > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }
        #endregion
        #region LC Contractor Setup
        //public List<LCApplicantBank> GetLC_ApplicantBank()
        //{
        //    try
        //    {
        //        var sqlquery = $@"select ACC_CODE AS ACC_CODE, ACC_EDESC AS ACC_EDESC  from fa_chart_of_accounts_setup where ACC_NATURE ='AC' AND ACC_TYPE_FLAG='T' AND DELETED_FLAG='N'  AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
        //        var result = _dbContext.SqlQuery<LCApplicantBank>(sqlquery).ToList();
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        public List<LC_ContractorModel> getAll_LC_ContractorList()
        {
            try
            {
                var sqlquery = $@"  select TO_CHAR(B.CONTRACTOR_CODE) as CONTRACTOR_CODE, B.CONTRACTOR_EDESC AS CONTRACTOR_EDESC,B.CONTRACTOR_NDESC AS CONTRACTOR_NDESC,B.CONTRACTOR_ADDRESS CONTRACTOR_ADDRESS,B.PHONE_NUMBER AS PHONE_NUMBER
                                  from LC_CONTRACTOR B where B.DELETED_FLAG='N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                var result = _dbContext.SqlQuery<LC_ContractorModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        public void AddUpdateLC_Contractor(LC_ContractorModel lcContractorModel)
        {
            if (string.IsNullOrEmpty(lcContractorModel.CONTRACTOR_CODE))
            {
                var maxCodeQuery = $@"SELECT COALESCE(MAX(CONTRACTOR_CODE)+1,1) FROM LC_CONTRACTOR WHERE COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                int maxLcContractorCode = _dbContext.SqlQuery<int>(maxCodeQuery).FirstOrDefault();

                var insertQuery = $@"INSERT INTO LC_CONTRACTOR(CONTRACTOR_CODE,CONTRACTOR_EDESC,CONTRACTOR_NDESC,CONTRACTOR_ADDRESS,PHONE_NUMBER,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG)
                                                  VALUES('{maxLcContractorCode}','{lcContractorModel.CONTRACTOR_EDESC}','{lcContractorModel.CONTRACTOR_NDESC}','{lcContractorModel.CONTRACTOR_ADDRESS}','{lcContractorModel.PHONE_NUMBER}','{_workcontext.CurrentUserinformation.company_code}', '{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string updatequery = $@"UPDATE LC_CONTRACTOR SET CONTRACTOR_EDESC  = '{lcContractorModel.CONTRACTOR_EDESC}',CONTRACTOR_NDESC= '{lcContractorModel.CONTRACTOR_NDESC}', CONTRACTOR_ADDRESS='{lcContractorModel.CONTRACTOR_ADDRESS}',PHONE_NUMBER='{lcContractorModel.PHONE_NUMBER}',LAST_MODIFIED_BY = '{_workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE CONTRACTOR_CODE = '{lcContractorModel.CONTRACTOR_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }

        }

        public void DeleteLC_Contractor(string CONTRACTOR_CODE)
        {
            try
            {
                string updatequery = $@"UPDATE LC_CONTRACTOR SET DELETED_FLAG='Y' WHERE CONTRACTOR_CODE = '{CONTRACTOR_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }
            catch (Exception EX)
            {

                throw EX;
            }
        }

        //public bool ApplicantBankAccountNumberExists(string BANK_NAME, string BANK_ACC_NO)
        //{
        //    try
        //    {
        //        string selectquery = $@"Select count(*) from  FA_BANK_SETUP WHERE BANK_NAME = '{BANK_NAME}' AND BANK_ACC_NO= '{BANK_ACC_NO}'AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
        //        var rowCount = _dbContext.SqlQuery<int>(selectquery).FirstOrDefault();
        //        if (rowCount > 0)
        //        {
        //            return true;
        //        }
        //        return false;
        //    }
        //    catch (Exception EX)
        //    {
        //        throw EX;
        //    }
        //}



        #endregion

        #region LC Clearing Agent Setup


        public List<LC_ClearingAgentModel> GetAllLcClearingAgentList()
        {
            try
            {
                var sqlquery = $@"  select TO_CHAR(B.CAGENT_CODE) as CAGENT_CODE, B.CAGENT_EDESC AS CAGENT_EDESC,B.CAGENT_NDESC AS CAGENT_NDESC,B.CAGENT_ADDRESS CAGENT_ADDRESS,B.PHONE_NUMBER AS PHONE_NUMBER
                                  from LC_CLEARING_AGENT B where B.DELETED_FLAG='N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                var result = _dbContext.SqlQuery<LC_ClearingAgentModel>(sqlquery).ToList();
                return result;
                //List<LC_ClearingAgentModel> record= new List<LC_ClearingAgentModel>();
                //var sqlquery = $@"select TO_CHAR(lcca.CAGENT_CODE) AS CAGENT_CODE, lcca.CAGENT_EDESC AS CAGENT_EDESC,lcca.CAGENT_NDESC AS CAGENT_NDESC,lcca.CAGENT_ADDRESS AS CAGENT_ADDRESS,lcca.PHONE_NUMBER AS PHONE_NUMBER from LC_CLEARING_AGENT lcca where lcca.DELETED_FLAG='N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                // record = _dbContext.SqlQuery<LC_ClearingAgentModel>(sqlquery).ToList();
                //return record;
            }
            catch (Exception EX)
            {
                throw EX;
            }
        }

        public void AddUpdateLcClearingAgent(LC_ClearingAgentModel lcClearingAgentModel)
        {
            if (string.IsNullOrEmpty(lcClearingAgentModel.CAGENT_CODE))
            {
                var maxCodeQuery = $@"SELECT COALESCE(MAX(CAGENT_CODE)+1,1) FROM LC_CLEARING_AGENT WHERE COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}'";
                int maxCAgentCode = _dbContext.SqlQuery<int>(maxCodeQuery).FirstOrDefault();

                var insertQuery = $@"INSERT INTO LC_CLEARING_AGENT(CAGENT_CODE,CAGENT_EDESC,CAGENT_NDESC,CAGENT_ADDRESS,PHONE_NUMBER,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,APPROVED_BY,APPROVED_DATE,DELETED_FLAG)
                                                  VALUES('{maxCAgentCode}','{lcClearingAgentModel.CAGENT_EDESC}','{lcClearingAgentModel.CAGENT_NDESC}','{lcClearingAgentModel.CAGENT_ADDRESS}','{lcClearingAgentModel.PHONE_NUMBER}','{_workcontext.CurrentUserinformation.company_code}', '{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss'),'N')";
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                _dbContext.SaveChanges();
            }
            else
            {
                string updatequery = $@"UPDATE LC_CLEARING_AGENT SET CAGENT_EDESC  = '{lcClearingAgentModel.CAGENT_EDESC}',CAGENT_NDESC= '{lcClearingAgentModel.CAGENT_NDESC}', CAGENT_ADDRESS='{lcClearingAgentModel.CAGENT_ADDRESS}',PHONE_NUMBER='{lcClearingAgentModel.PHONE_NUMBER}',LAST_MODIFIED_BY = '{_workcontext.CurrentUserinformation.User_id}',LAST_MODIFIED_DATE = TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss")}', 'mm/dd/yyyy hh24:mi:ss') WHERE CAGENT_CODE = '{lcClearingAgentModel.CAGENT_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }

        }

        public void DeleteLcClearingAgent(string CAGENT_CODE)
        {
            try
            {
                string updatequery = $@"UPDATE LC_CLEARING_AGENT SET DELETED_FLAG='Y' WHERE CAGENT_CODE = '{CAGENT_CODE}' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' AND BRANCH_CODE ='{_workcontext.CurrentUserinformation.branch_code}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }
            catch (Exception EX)
            {

                throw EX;
            }
        }
        #endregion

        #region Preference Setup
        public LC_PREFERENCE_SETUP SavePreferenceSetup(string Beneficiary, string GetPOfromERP)
        {
            LC_PREFERENCE_SETUP lC_PREFERENCE_SETUP = new LC_PREFERENCE_SETUP();
            Beneficiary = (Beneficiary == "true") ? "Y" : "N";
            GetPOfromERP = (GetPOfromERP == "true") ? "Y" : "N";

            var selectcountquery = $@"SELECT count(*) from LC_PREFERENCE_SETUP";
            var count = _dbContext.SqlQuery<int>(selectcountquery).FirstOrDefault();

            if (count == 0)
            {
                string insertquery = $@"INSERT INTO  LC_PREFERENCE_SETUP (BENEFICIARY_SUPPLIER_MAP,PO_FROM_ERPS)
                                        VALUES('{Beneficiary}','{GetPOfromERP}')";
                var irowcount = _dbContext.ExecuteSqlCommand(insertquery);
            }
            else
            {
                string updatequery = $@"UPDATE LC_PREFERENCE_SETUP SET BENEFICIARY_SUPPLIER_MAP  = '{Beneficiary}' , PO_FROM_ERPS ='{GetPOfromERP}'";
                var rowCount = _dbContext.ExecuteSqlCommand(updatequery);
                _dbContext.SaveChanges();
            }

            var selectquery1 = $@"SELECT TO_CHAR(CASE  
                             WHEN BENEFICIARY_SUPPLIER_MAP= 'Y' THEN 'true' ELSE 'false' END ) AS Beneficiary,
                                (CASE 
                             WHEN PO_FROM_ERPS = 'Y' THEN 'true' ELSE 'false' END ) AS PO From LC_PREFERENCE_SETUP";
            lC_PREFERENCE_SETUP = _dbContext.SqlQuery<LC_PREFERENCE_SETUP>(selectquery1).FirstOrDefault();
            return lC_PREFERENCE_SETUP;
        }

        public LC_PREFERENCE_SETUP GetPreferenceSetup()
        {
            LC_PREFERENCE_SETUP lC_PREFERENCE_SETUP = new LC_PREFERENCE_SETUP();
            var selectquery = $@"SELECT TO_CHAR(CASE  
                             WHEN BENEFICIARY_SUPPLIER_MAP= 'Y' THEN 'true' ELSE 'false' END ) AS Beneficiary,
                                (CASE 
                             WHEN PO_FROM_ERPS = 'Y' THEN 'true' ELSE 'false' END ) AS PO From LC_PREFERENCE_SETUP";
            lC_PREFERENCE_SETUP = _dbContext.SqlQuery<LC_PREFERENCE_SETUP>(selectquery).FirstOrDefault();
            return lC_PREFERENCE_SETUP;
        }
        #endregion



    }
}