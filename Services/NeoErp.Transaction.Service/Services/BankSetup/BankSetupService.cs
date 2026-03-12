using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Transaction.Service.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.Transaction.Service.Services.BankSetup
{
    public class BankSetupService : IBankSetupService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public BankSetupService(IDbContext dbContext, IWorkContext workcontext)
        {
            this._dbContext = dbContext;
            this._workcontext = workcontext;
        }

        public string SaveBank(BankSetupModel model)
        {
            string InsertQuery = string.Empty;
            InsertQuery = @"INSERT INTO FA_BANK_SETUP(BANK_CODE,BANK_NAME,BANK_ACC_NO,SWIFT_CODE,ADDRESS,BRANCH,PHONE_NO,REMARKS,DELETED_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE)
                            VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','N','{8}','{9}',TO_DATE('{10}', 'MM/dd/yyyy'))";
            InsertQuery = string.Format(InsertQuery, "FA_BANK_SETUP_SEQ.NEXTVAL", model.BankName, model.AccountNumber, model.SwiftCode, model.Address, model.Branch, model.PhoneNumber, model.Remarks,
                _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id.ToString(), DateTime.Now.ToString("MM/dd/yyyy"));
            var rowCount = _dbContext.ExecuteSqlCommand(InsertQuery);

            string ContactQuery = string.Empty;
            foreach (var contact in model.Contacts)
            {
                ContactQuery = @"INSERT INTO FA_BANK_CONTACT_DETAIL (BANK_CODE,SERIAL_NO,CONTACT_PERSON,DESIGNATION,MOBILE_NO,TEL_NO,REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE)
                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9}', 'MM/dd/yyyy'))";
                ContactQuery = string.Format(ContactQuery, model.BankCode, contact.Sn, contact.ContactPerson, contact.Designation, contact.MobileNumber, contact.TelephoneNumber, contact.Remarks,
                    _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"));

                rowCount = _dbContext.ExecuteSqlCommand(ContactQuery);
            }
            return "Success";
            //_dbContext.SaveChanges();
        }

        public List<BankSetupModel> GetAllBanks()
        {
            string QueryBank = string.Format(@"SELECT BANK_CODE AS BankCode,BANK_NAME AS BankName,BANK_ACC_NO AS AccountNumber,SWIFT_CODE AS SwiftCode,ADDRESS AS Address,BRANCH AS Branch,PHONE_NO AS PhoneNumber,REMARKS AS Remarks
                               FROM FA_BANK_SETUP
                                WHERE DELETED_FLAG='N'");
            var allBanks = _dbContext.SqlQuery<BankSetupModel>(QueryBank).ToList();

            for (int i = 0; i < allBanks.Count; i++)
            {
                string QueryContact = string.Format(@"SELECT SERIAL_NO AS SN,CONTACT_PERSON AS ContactPerson,DESIGNATION AS Designation,MOBILE_NO AS MobileNumber,TEL_NO AS TelephoneNumber,REMARKS AS Remarks
                                                    FROM FA_BANK_CONTACT_DETAIL WHERE BANK_CODE='{0}'", allBanks[i].BankCode);
                var contacts = _dbContext.SqlQuery<BankContacts>(QueryContact).ToList();
                allBanks[i].Contacts = contacts;
            }
            return allBanks.ToList();
        }

        public BankSetupModel GetBank(int BankCode)
        {
            string QueryContact = string.Format(@"SELECT SERIAL_NO AS SN,CONTACT_PERSON AS ContactPerson,DESIGNATION AS Designation,MOBILE_NO AS MobileNumber,TEL_NO AS TelephoneNumber,REMARKS AS Remarks
                                                    FROM FA_BANK_CONTACT_DETAIL WHERE BANK_CODE='{0}'", BankCode);
            var contacts = _dbContext.SqlQuery<BankContacts>(QueryContact).ToList();

            string QueryBank = string.Format(@"SELECT BANK_CODE AS BankCode,BANK_NAME AS BankName,BANK_ACC_NO AS AccountNumber,SWIFT_CODE AS SwiftCode,ADDRESS AS Address,BRANCH AS Branch,PHONE_NO AS PhoneNumber,REMARKS AS Remarks
                                                    FROM FA_BANK_SETUP WHERE BANK_CODE='{0}' AND DELETED_FLAG='N'", BankCode);
            var Result = _dbContext.SqlQuery<BankSetupModel>(QueryBank).ToList().FirstOrDefault();
            if (Result != null)
                Result.Contacts = contacts;
            return Result;
        }

        public string DeleteBank(int BankCode)
        {
            string UpdateQuery = $@"UPDATE FA_BANK_SETUP SET DELETED_FLAG='Y'
                            WHERE BANK_CODE='{BankCode}'";
            var rowCount = _dbContext.ExecuteSqlCommand(UpdateQuery);
            if (rowCount > 0)
                return "Success";
            else
                return "Bank Not Found";
        }

        public string UpdateBank(BankSetupModel model)
        {
            string UpdateQuery = string.Empty;
            UpdateQuery = @"UPDATE FA_BANK_SETUP SET BANK_NAME='{0}',BANK_ACC_NO='{1}',SWIFT_CODE='{2}',ADDRESS='{3}',BRANCH='{4}',PHONE_NO='{5}',
                            REMARKS='{6}',MODIFY_BY='{7}',MODIFY_DATE=TO_DATE('{8}', 'MM/dd/yyyy')
                            WHERE BANK_CODE='{9}'";
            UpdateQuery = string.Format(UpdateQuery, model.BankName, model.AccountNumber, model.SwiftCode, model.Address, model.Branch, model.PhoneNumber, model.Remarks,
                _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), model.BankCode);
            var rowCount = _dbContext.ExecuteSqlCommand(UpdateQuery);

            string ContactQuery = string.Empty;
            string DeleteQuery = string.Empty;
            //delete all previous contact informations
            DeleteQuery = string.Format(@"DELETE FROM FA_BANK_CONTACT_DETAIL WHERE BANK_CODE='{0}'", model.BankCode);
            rowCount = _dbContext.ExecuteSqlCommand(DeleteQuery);
            //add new contact in formations
            foreach (var contact in model.Contacts)
            {
                ContactQuery = @"INSERT INTO FA_BANK_CONTACT_DETAIL (BANK_CODE,SERIAL_NO,CONTACT_PERSON,DESIGNATION,MOBILE_NO,TEL_NO,REMARKS,COMPANY_CODE,CREATED_BY,CREATED_DATE)
                                VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',TO_DATE('{9}', 'MM/dd/yyyy'))";
                ContactQuery = string.Format(ContactQuery, model.BankCode, contact.Sn, contact.ContactPerson, contact.Designation, contact.MobileNumber, contact.TelephoneNumber, contact.Remarks,
                    _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"));

                rowCount = _dbContext.ExecuteSqlCommand(ContactQuery);
            }
            return "Success";
        }

        public string SaveLimit(BankLimitModel model)
        {
            string transactionNoQuery = "SELECT TRANSACTION_NO_SEQ.NEXTVAL FROM DUAL";
            model.TransactionNumber = _dbContext.SqlQuery<int>(transactionNoQuery).FirstOrDefault();
            foreach (var loan in model.LoanList)
            {
                string InsertQuery = string.Empty;
                string InsertHisQuery = string.Empty;
                InsertQuery = @"INSERT INTO FA_BANK_CR_LIMIT_SETUP (ID,SERIAL_NO,BANK_CODE,TRANS_NO,EXPAIRY_DATE,EFFECTIVE_DATE,REMARKS,LOAN_CATEGORY,LIMIT_AMOUNT,
                            CR_LIMIT_TYPE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                            VALUES ({0},'{1}','{2}','{3}',TO_DATE('{4}', 'MM/dd/yyyy'),TO_DATE('{5}', 'MM/dd/yyyy')
                            ,'{6}','{7}','{8}','{9}','{10}','{11}',TO_DATE('{12}', 'MM/dd/yyyy'),'N')";
                InsertQuery = string.Format(InsertQuery, "FA_BANK_CR_LIMIT_SETUP_SEQ.NEXTVAL", loan.Sn, model.BankCode, model.TransactionNumber,
                    loan.ExpiryDate.ToString("MM/dd/yyyy"), loan.EffectiveDate.ToString("MM/dd/yyyy"), loan.Remarks,
                    loan.LoanCategory, loan.LimitAmount, model.Type,
                    _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id.ToString(), DateTime.Now.ToString("MM/dd/yyyy"));
                var rowCount = _dbContext.ExecuteSqlCommand(InsertQuery);

                //history Table
                InsertHisQuery = @"INSERT INTO FA_BANK_CR_LIMIT_HISTORICAL (ID,SERIAL_NO,BANK_CODE,TRANS_NO,EXPAIRY_DATE,EFFECTIVE_DATE,REMARKS,LOAN_CATEGORY,LIMIT_AMOUNT,
                            CR_LIMIT_TYPE,COMPANY_CODE,CREATED_BY,CREATED_DATE,ACTION_TYPE) 
                            VALUES ({0},'{1}','{2}','{3}',TO_DATE('{4}', 'MM/dd/yyyy'),TO_DATE('{5}', 'MM/dd/yyyy')
                            ,'{6}','{7}','{8}','{9}','{10}','{11}',TO_DATE('{12}', 'MM/dd/yyyy'),'I')";
                InsertHisQuery = string.Format(InsertHisQuery, "FA_CR_LIMIT_HISTORICAL_SEQ.NEXTVAL", loan.Sn, model.BankCode, model.TransactionNumber,
                    loan.ExpiryDate.ToString("MM/dd/yyyy"), loan.EffectiveDate.ToString("MM/dd/yyyy"), loan.Remarks,
                    loan.LoanCategory, loan.LimitAmount, model.Type,
                    _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id.ToString(), DateTime.Now.ToString("MM/dd/yyyy"));
                rowCount = _dbContext.ExecuteSqlCommand(InsertHisQuery);
            }

            return "Success";
        }

        public string UpdateLimit(BankLimitModel model)
        {

            string DeleteQuery = string.Format(@"DELETE FROM FA_BANK_CR_LIMIT_SETUP WHERE TRANS_NO='{0}'", model.TransactionNumber);
            var rowCount = _dbContext.ExecuteSqlCommand(DeleteQuery);
            //add new contact in formations
            foreach (var loan in model.LoanList)
            {
                string InsertQuery = string.Empty;
                string InsertHisQuery = string.Empty;
                InsertQuery = @"INSERT INTO FA_BANK_CR_LIMIT_SETUP (ID,SERIAL_NO,BANK_CODE,TRANS_NO,EXPAIRY_DATE,EFFECTIVE_DATE,REMARKS,LOAN_CATEGORY,LIMIT_AMOUNT,
                            CR_LIMIT_TYPE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                            VALUES ({0},'{1}','{2}','{3}',TO_DATE('{4}', 'MM/dd/yyyy'),TO_DATE('{5}', 'MM/dd/yyyy')
                            ,'{6}','{7}','{8}','{9}','{10}','{11}',TO_DATE('{12}', 'MM/dd/yyyy'),'N')";
                InsertQuery = string.Format(InsertQuery, "FA_BANK_CR_LIMIT_SETUP_SEQ.NEXTVAL", loan.Sn, model.BankCode, model.TransactionNumber,
                    loan.ExpiryDate.ToString("MM/dd/yyyy"), loan.EffectiveDate.ToString("MM/dd/yyyy"), loan.Remarks,
                    loan.LoanCategory, loan.LimitAmount, model.Type,
                    _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id.ToString(), DateTime.Now.ToString("MM/dd/yyyy"));
                rowCount = _dbContext.ExecuteSqlCommand(InsertQuery);

                //history Table
                InsertHisQuery = @"INSERT INTO FA_BANK_CR_LIMIT_HISTORICAL (ID,SERIAL_NO,BANK_CODE,TRANS_NO,EXPAIRY_DATE,EFFECTIVE_DATE,REMARKS,LOAN_CATEGORY,LIMIT_AMOUNT,
                            CR_LIMIT_TYPE,COMPANY_CODE,CREATED_BY,CREATED_DATE,ACTION_TYPE) 
                            VALUES ({0},'{1}','{2}','{3}',TO_DATE('{4}', 'MM/dd/yyyy'),TO_DATE('{5}', 'MM/dd/yyyy')
                            ,'{6}','{7}','{8}','{9}','{10}','{11}',TO_DATE('{12}', 'MM/dd/yyyy'),'I')";
                InsertHisQuery = string.Format(InsertHisQuery, "FA_CR_LIMIT_HISTORICAL_SEQ.NEXTVAL", loan.Sn, model.BankCode, model.TransactionNumber,
                    loan.ExpiryDate.ToString("MM/dd/yyyy"), loan.EffectiveDate.ToString("MM/dd/yyyy"), loan.Remarks,
                    loan.LoanCategory, loan.LimitAmount, model.Type,
                    _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.User_id.ToString(), DateTime.Now.ToString("MM/dd/yyyy"));
                rowCount = _dbContext.ExecuteSqlCommand(InsertHisQuery);
            }

            return "Success";
        }

        public string RenewByid(int Id, DateTime date)
        {
            string historyQuery = $@"INSERT INTO FA_BANK_CR_LIMIT_HISTORICAL (
                                   TRANS_NO, SERIAL_NO, 
                                   REMARKS, LOAN_CATEGORY, 
                                   LIMIT_AMOUNT, ID, EXPAIRY_DATE, 
                                   EFFECTIVE_DATE, CREATED_DATE, CREATED_BY, 
                                   CR_LIMIT_TYPE, COMPANY_CODE, BANK_CODE, 
                                   ACTION_TYPE)
                                   SELECT TRANS_NO, SERIAL_NO,
                                   REMARKS, LOAN_CATEGORY, LIMIT_AMOUNT,FA_CR_LIMIT_HISTORICAL_SEQ.NEXTVAL as Id,
                                   EXPAIRY_DATE, EFFECTIVE_DATE,
                                   CREATED_DATE, CREATED_BY, TRIM(CR_LIMIT_TYPE),
                                   COMPANY_CODE, BANK_CODE, 'U' as ACTION_TYPE
                                FROM FA_BANK_CR_LIMIT_SETUP 
                                WHERE Id='{Id}'";
            var rowCount = _dbContext.ExecuteSqlCommand(historyQuery);

            string query = string.Empty;
            query = $@"UPDATE FA_BANK_CR_LIMIT_SETUP SET EXPAIRY_DATE=TO_DATE('{date.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy'),MODIFY_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy')
                    ,MODIFY_BY='{_workcontext.CurrentUserinformation.User_id.ToString()}' WHERE ID='{Id}'";
            rowCount = _dbContext.ExecuteSqlCommand(query);
            return "Success";
        }

        public List<BankLimitModel> GetLimitList(string limitType = "B")
        {
            string type = string.Empty;
            if (limitType == "N")
                type = @"'N'";
            else if (limitType == "F")
                type = @"'F'";
            else
                type = @"'N','F'";

            List<BankLimitModel> Result = new List<BankLimitModel>();
            string Query = string.Empty;
            Query = $@"SELECT DISTINCT TRANS_NO FROM FA_BANK_CR_LIMIT_SETUP WHERE TRIM(CR_LIMIT_TYPE) IN ({type}) AND DELETED_FLAG='N'";
            var allTransaction = _dbContext.SqlQuery<int>(Query).ToList();
            foreach (var tran in allTransaction)
            {
                var tempTransaction = GetLimitByTransactionNumber(tran, limitType);
                Result.Add(tempTransaction);
            }
            return Result;
        }

        public List<BankLimitModel> GetHistory(int transNo, int sn, string type)
        {
            List<BankLimitModel> Res = new List<BankLimitModel>();

            string QueryList = string.Format(@"SELECT ID, SERIAL_NO as Sn,LOAN_CATEGORY AS LoanCategory,LIMIT_AMOUNT AS LimitAmount,EFFECTIVE_DATE AS EffectiveDate,EXPAIRY_DATE AS ExpiryDate,REMARKS AS Remarks
                                    FROM FA_BANK_CR_LIMIT_HISTORICAL WHERE TRANS_NO='{0}' AND SERIAL_NO='{1}' AND TRIM(CR_LIMIT_TYPE)='{2}'", transNo, sn, type);
            var List = _dbContext.SqlQuery<LoanModel>(QueryList).ToList();

            string QueryBank = string.Format(@"SELECT C.BANK_CODE AS BankCode,TRIM(C.CR_LIMIT_TYPE) as Type,C.TRANS_NO AS TransactionNumber,B.BANK_NAME  AS BankName
                                                FROM FA_BANK_CR_LIMIT_HISTORICAL C, FA_BANK_SETUP B
                                                WHERE C.BANK_CODE=B.BANK_CODE AND B.DELETED_FLAG='N' AND C.TRANS_NO='{0}' AND TRIM(C.CR_LIMIT_TYPE)='{1}'", transNo, type);
            var Result = _dbContext.SqlQuery<BankLimitModel>(QueryBank).ToList().FirstOrDefault();
            if (Result != null)
            {
                Result.LoanList = List;
                Res.Add(Result);
            }
            return Res;
        }

        public BankLimitModel GetLimitByTransactionNumber(int transactionNumber, string limitType = "B")
        {
            string type = string.Empty;
            if (limitType == "N")
                type = @"'N'";
            else if (limitType == "F")
                type = @"'F'";
            else
                type = @"'N','F'";

            string QueryList = string.Format(@"SELECT ID, SERIAL_NO as Sn, LOAN_CATEGORY AS LoanCategory,LIMIT_AMOUNT AS LimitAmount,EFFECTIVE_DATE AS EffectiveDate,EXPAIRY_DATE AS ExpiryDate,REMARKS AS Remarks
                                    FROM FA_BANK_CR_LIMIT_SETUP WHERE TRANS_NO='{0}' AND TRIM(CR_LIMIT_TYPE) IN({1})", transactionNumber, type);
            var List = _dbContext.SqlQuery<LoanModel>(QueryList).ToList();

            string QueryBank = string.Format(@"SELECT C.BANK_CODE AS BankCode,TRIM(C.CR_LIMIT_TYPE) as Type,C.TRANS_NO AS TransactionNumber,B.BANK_NAME  AS BankName
                                                FROM FA_BANK_CR_LIMIT_SETUP C, FA_BANK_SETUP B WHERE C.BANK_CODE=B.BANK_CODE AND TRANS_NO='{0}'
                                                AND B.DELETED_FLAG='N' AND TRIM(CR_LIMIT_TYPE) IN({1})", transactionNumber, type);
            var Result = _dbContext.SqlQuery<BankLimitModel>(QueryBank).ToList().FirstOrDefault();
            if (Result != null)
                Result.LoanList = List;
            return Result;
        }

        public string SaveCategory(LoanCategoryModel cat)
        {
            string Query = string.Empty;
            if (cat.CategoryId == 0)
            {
                Query = $@"INSERT INTO FA_LOAN_CATEGORY (CATEGORY_ID,CATEGORY_NAME,LIMIT_TYPE,DELETED_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE)
                        VALUES(FA_LOAN_CATEGORY_SEQ.NEXTVAL,'{cat.CategoryName}','{cat.CategoryType}','N','{_workcontext.CurrentUserinformation.company_code}',
                        '{_workcontext.CurrentUserinformation.User_id.ToString()}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy'))";
            }
            else
            {
                Query = $@"UPDATE FA_LOAN_CATEGORY SET CATEGORY_NAME ='{cat.CategoryName}',LIMIT_TYPE='{cat.CategoryType}',MODIFY_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy')
                        MODIFY_BY='{_workcontext.CurrentUserinformation.User_id.ToString()}' WHERE CATEGORY_ID='{cat.CategoryId}'";
            }
            var rowCount = _dbContext.ExecuteSqlCommand(Query);
            return "Success";
        }

        public List<LoanCategoryModel> GetLoanCategories()
        {
            string Query = string.Empty;
            Query = @"SELECT CATEGORY_ID AS CategoryId,CATEGORY_NAME AS CategoryName,LIMIT_TYPE AS CategoryType FROM FA_LOAN_CATEGORY
                    WHERE DELETED_FLAG='N'";
            var result = _dbContext.SqlQuery<LoanCategoryModel>(Query).ToList();
            return result;
        }

        public LoanCategoryModel GetLoanCategory(int id)
        {
            string Query = string.Empty;
            Query = $@"SELECT CATEGORY_ID AS CategoryId,CATEGORY_NAME AS CategoryName,LIMIT_TYPE AS CategoryType FROM FA_LOAN_CATEGORY
                    WHERE DELETED_FLAG='N' AND CATEGORY_ID=' { id.ToString() } '";
            var result = _dbContext.SqlQuery<LoanCategoryModel>(Query).ToList().FirstOrDefault();
            return result;
        }

        public string DeleteCategory(int Id)
        {
            string Query = string.Empty;
            Query = $@"UPDATE FA_LOAN_CATEGORY SET DELETED_FLAG='N'";
            var result = _dbContext.ExecuteSqlCommand(Query);
            return "Success";
        }

    }
}
