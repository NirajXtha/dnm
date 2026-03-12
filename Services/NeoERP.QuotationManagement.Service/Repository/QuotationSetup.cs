using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using NeoErp.Data;
using System;
using System.Globalization;
using NeoErp.Core.Services.CommonSetting;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Core.Models.CustomModels;
using System.Text;
using NeoERP.QuotationManagement.Service.Interface;
using NeoERP.QuotationManagement.Service.Models;
using System.Web;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;

namespace NeoERP.QuotationManagment.Service.Repository
{
    public class QuotationSetup : IQuotationRepo
    {
        IWorkContext _workContext;
        IDbContext _dbContext;

        public QuotationSetup(IWorkContext workContext, IDbContext dbContext)
        {
            this._workContext = workContext;
            this._dbContext = dbContext;
        }
        public List<Company> GetCompany()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            string query = $@"select COMPANY_CODE,COMPANY_EDESC,ADDRESS,EMAIL,LOGO_FILE_NAME from COMPANY_SETUP WHERE COMPANY_CODE='{company_code}'";
            List<Company> company = _dbContext.SqlQuery<Company>(query).ToList();
            return company;
        }
        public string getUserType()
        {
            var login = _workContext.CurrentUserinformation.login_code;
            var comp = _workContext.CurrentUserinformation.company_code;
            string query = $@"SELECT USER_TYPE from SC_APPLICATION_USERS WHERE COMPANY_CODE = '{comp}' and lower(login_code) = LOWER('{login}')";
            string type = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return type;
        }
        public List<Products> GetAllProducts()
        {
            try
            {
                List<Products> ProductsList = new List<Products>();

                //and iims.branch_code='{_workContext.CurrentUserinformation.branch_code}'

                string query = $@"select 
                    COALESCE(iims.item_code,' ') as ItemCode
                    ,COALESCE(iims.item_edesc,' ') as ItemDescription
                    ,COALESCE(iims.index_mu_code,' ') as ItemUnit
                    ,COALESCE(iiss.ITEM_SPECIFICATION,' ') as SPECIFICATION
                    ,COALESCE(iiss.BRAND_NAME,' ') as BRAND_NAME
                    ,COALESCE(iiss.INTERFACE,' ') as INTERFACE
                    ,COALESCE(iiss.TYPE,' ') as TYPE
                     ,COALESCE(iiss.LAMINATION,' ') as LAMINATION
                    ,COALESCE(iiss.ITEM_SIZE,' ') as ITEM_SIZE
                    ,COALESCE(iiss.THICKNESS,' ') as THICKNESS
                    ,COALESCE(iiss.COLOR,' ') as COLOR
                    ,COALESCE(iiss.GRADE,' ') as GRADE
                     ,COALESCE(iiss.SIZE_LENGHT,0) as SIZE_LENGHT
                    ,COALESCE(iiss.SIZE_WIDTH,0) as SIZE_WIDTH
                    from ip_item_master_setup iims,IP_ITEM_SPEC_SETUP iiss
                    where iims.item_code=iiss.item_code(+) 
                     and iims.company_code=iiss.company_code(+)
                     and iims.deleted_flag='N'
                    AND IIMS.GROUP_SKU_FLAG = 'I'
                    AND iims.company_code='{_workContext.CurrentUserinformation.company_code}' 
                     order by iims.item_code desc";
                ProductsList = this._dbContext.SqlQuery<Products>(query).ToList();
                return ProductsList;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public List<QuotationLogModel> GetQuotationLogs()
        {
            try
            {
                List<QuotationLogModel> Logs = new List<QuotationLogModel>();
                var company_code = _workContext.CurrentUserinformation.company_code;
                string query = $@"SELECT * FROM QUOTATION_BACK_LOG WHERE COMPANY_CODE = '{company_code}' order by 1 desc";
                Logs = _dbContext.SqlQuery<QuotationLogModel>(query).ToList();
                return Logs;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<Quotation_setup> GetQuotationId(string tender)
        {
            string query = $@"select GENERATE_TENDER_NO('{_workContext.CurrentUserinformation.company_code}', '{tender}') as TENDER_NO from dual";
            List<Quotation_setup> id = _dbContext.SqlQuery<Quotation_setup>(query).ToList();

            while (id == null || id.Count == 0 || string.IsNullOrEmpty(id[0].TENDER_NO))
            {
                string updateBodyLengthQuery = $@"UPDATE FORM_SETUP 
                                          SET BODY_LENGTH = (SELECT NVL(MAX(BODY_LENGTH), 0)+1 
                                                             FROM FORM_SETUP 
                                                             WHERE FORM_CODE = '{tender}') 
                                          WHERE FORM_CODE = '{tender}'";

                _dbContext.ExecuteSqlCommand(updateBodyLengthQuery);

                id = _dbContext.SqlQuery<Quotation_setup>(query).ToList();
            }

            return id;
        }
        private void InsertRemarks(QuotationLogModel model)
        {
            var login_code = _workContext.CurrentUserinformation.login_code;
            string logQuery = $@"INSERT INTO QUOTATION_BACK_LOG VALUES
                    (QUOTATION_BACK_LOG_SEQ.NEXTVAL, '{model.QUOTATION_ID}', '{model.QUOTATION_NO}', '{model.TENDER_NO}', '{model.TYPE}', '{model.ACTION}', '{login_code}', SYSDATE, '{model.CHANGED}', '{model.REMARKS_ENCODED}', '{_workContext.CurrentUserinformation.company_code}')";
            _dbContext.ExecuteSqlCommand(logQuery);
        }

        public bool CloneQuotation(Quotation_setup model)
        {
            if (model == null)
                return false;
            var login_code = _workContext.CurrentUserinformation.login_code;
            var company_code = _workContext.CurrentUserinformation.company_code;
            try
            {
                var idquery = $@"SELECT COALESCE(MAX(ID) + 1, 1) AS id FROM sa_quotation_setup";
                int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();
                string form_code = _dbContext.SqlQuery<string>($@"select form_code from sa_quotation_setup where tender_no = '{model.TENDER_NO}' and ID = '{model.ID}'").FirstOrDefault().ToString();
                string getNewTender = GetQuotationId(form_code).Select(a => a.TENDER_NO).FirstOrDefault().ToString();
                string query = $@"
                            INSERT INTO SA_QUOTATION_SETUP (
                                TENDER_NO, VALID_DATE, ISSUE_DATE, CREATED_DATE, CREATED_BY, COMPANY_CODE, STATUS, REMARKS,
                                ID, APPROVED_STATUS, MODIFIED_DATE, MODIFIED_BY, BRANCH_CODE, MANUAL_NO, APPROVED_BY, LOCAL_FLAG, FORM_CODE, VAT_INCLUDE_FLAG
                            )
                            SELECT
                                '{getNewTender}', VALID_DATE, ISSUE_DATE, SYSDATE, '{login_code}', COMPANY_CODE, 'E', REMARKS,
                                {id}, 'N', NULL, NULL, BRANCH_CODE, MANUAL_NO, NULL, LOCAL_FLAG, FORM_CODE, 'Y'
                            FROM SA_QUOTATION_SETUP
                            WHERE ID = '{model.ID}'
                              AND TENDER_NO = '{model.TENDER_NO}'
                            ";
                int row = _dbContext.ExecuteSqlCommand(query);
                if (row == 0)
                    throw new Exception("Unable to Clone quotation. Please try again!");
                row = 0;
                string itemQuery = $@"
                            INSERT INTO SA_QUOTATION_ITEMS (
                                ID, TENDER_NO,
                                ITEM_CODE, SPECIFICATION, IMAGE, UNIT, QUANTITY, CATEGORY, BRAND_NAME, INTERFACE, TYPE, LAMINATION, ITEM_SIZE, THICKNESS, COLOR, GRADE, SIZE_LENGTH, SIZE_WIDTH, DELETED_FLAG, REMARKS,
                                QUOTATION_NO, FORM_CODE
                            )
                            SELECT
                                (SELECT NVL(MAX(ID), 0) + 1 FROM SA_QUOTATION_ITEMS), '{getNewTender}',
                                ITEM_CODE, SPECIFICATION, IMAGE, UNIT, QUANTITY, CATEGORY, BRAND_NAME, INTERFACE, TYPE, LAMINATION, ITEM_SIZE, THICKNESS, COLOR, GRADE,SIZE_LENGTH, SIZE_WIDTH, 'N', REMARKS, 
                                '{id}', FORM_CODE
                            FROM SA_QUOTATION_ITEMS
                            WHERE TENDER_NO = '{model.TENDER_NO}'
                              AND QUOTATION_NO = '{model.ID}'
                            ";
                row = _dbContext.ExecuteSqlCommand(itemQuery);
                if (row == 0) throw new Exception("Something went wront during item cloning.");

                QuotationLogModel log = new QuotationLogModel
                {
                    QUOTATION_ID = Convert.ToInt32(id),
                    TENDER_NO = getNewTender,
                    TYPE = "CLONE",
                    ACTION = "CLONE",
                    CHANGED = model.TENDER_NO,
                    REMARKS = model.REMARKS
                };
                InsertRemarks(log);

                //string logQuery = $@"INSERT INTO QUOTATION_BACK_LOG VALUES
                //    (QUOTATION_BACK_LOG_SEQ.NEXTVAL, '{id}', NULL, '{getNewTender}', 'CLONE', 'CLONE', '{login_code}', SYSDATE, '{model.ID}', '{model.REMARKS}', '{company_code}')";
                //_dbContext.ExecuteSqlCommand(logQuery);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool InsertQuotationData(Quotation_setup data)
        {
            try
            {
                int tenderId = data.ID;
                if (tenderId == 0)
                {
                    var checkQuery = $@"select count(id) from sa_quotation_setup where tender_no = '{data.TENDER_NO}'";
                    int count = _dbContext.SqlQuery<int>(checkQuery).FirstOrDefault();
                    if (count > 0)
                    {
                        // Tender already exists, must be duplicate value
                        throw new Exception("The Following Quotation Number Already Exists.");
                    }

                    var idquery = $@"SELECT COALESCE(MAX(ID) + 1, 1) AS id FROM sa_quotation_setup";
                    int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();
                    string insertQuery = string.Format(@"INSERT INTO sa_quotation_setup(TENDER_NO, VALID_DATE, ISSUE_DATE, CREATED_DATE, CREATED_BY, COMPANY_CODE, STATUS,REMARKS,ID,APPROVED_STATUS,BRANCH_CODE, LOCAL_FLAG, FORM_CODE) 
                                 VALUES('{0}', TO_DATE('{1}', 'DD-MON-YYYY'), TO_DATE('{2}', 'DD-MON-YYYY'), TO_DATE('{3}', 'DD-MON-YYYY'), '{4}', '{5}', '{6}','{7}',{8},'{9}','{10}', '{11}', '{12}')",
                                              data.TENDER_NO,
                                              data.VALID_DATE.HasValue ? $"{data.VALID_DATE.Value.ToString("dd-MMM-yyyy")}" : null,
                                              data.ISSUE_DATE.HasValue ? $"{data.ISSUE_DATE.Value.ToString("dd-MMM-yyyy")}" : null,
                                              DateTime.Now.ToString("dd-MMM-yyyy"),
                                              _workContext.CurrentUserinformation.login_code,
                                              _workContext.CurrentUserinformation.company_code,
                                              "E", data.REMARKS, id, "N", _workContext.CurrentUserinformation.branch_code, data.LOCAL_FLAG, data.FORM_CODE);
                    _dbContext.ExecuteSqlCommand(insertQuery);
                    List<Item> itemData = data.Items;
                    if (itemData != null)
                    {
                        foreach (var item in itemData)
                        {

                            InsertItemData(item, data.TENDER_NO, id, data.FORM_CODE); // Pass each item individually to InsertItemData
                        }
                    }
                    QuotationLogModel log = new QuotationLogModel
                    {
                        QUOTATION_ID = Convert.ToInt32(id),
                        TENDER_NO = data.TENDER_NO,
                        TYPE = "MODIFY",
                        ACTION = "ADD",
                        ACTION_BY = _workContext.CurrentUserinformation.login_code,
                        CHANGED = data.TENDER_NO,
                        REMARKS = data.LOG_REMARKS
                    };
                    InsertRemarks(log);
                }
                else
                {
                    string updateQuery = $@"UPDATE sa_quotation_setup 
                       SET VALID_DATE = {(data.VALID_DATE.HasValue ? $"'{data.VALID_DATE.Value.ToString("dd-MMM-yyyy")}'" : "null")},
                           ISSUE_DATE = {(data.ISSUE_DATE.HasValue ? $"'{data.ISSUE_DATE.Value.ToString("dd-MMM-yyyy")}'" : "null")},
                           MODIFIED_DATE = '{DateTime.Now.ToString("dd-MMM-yyyy")}',REMARKS='{data.REMARKS}',
                           MODIFIED_BY = '{_workContext.CurrentUserinformation.login_code}',
                           COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}',
                           LOCAL_FLAG = '{data.LOCAL_FLAG}'
                       WHERE id = '{data.ID}' 
                       AND tender_no = '{data.TENDER_NO}'";
                    _dbContext.ExecuteSqlCommand(updateQuery);
                    List<Item> itemData = data.Items;
                    if (itemData != null)
                    {
                        foreach (var item in itemData)
                        {
                            var query = $@"SELECT * FROM sa_quotation_Items WHERE tender_no='{data.TENDER_NO}' AND id='{item.ID}' AND QUOTATION_NO = '{data.QUOTATION_NO}'";
                            List<Item> itemDetails = _dbContext.SqlQuery<Item>(query).ToList();

                            if (itemDetails.Any())
                            {
                                UpdateItemData(item, data.TENDER_NO, data.QUOTATION_NO, data.FORM_CODE);
                            }
                            else
                            {
                                InsertItemData(item, data.TENDER_NO, data.QUOTATION_NO, data.FORM_CODE);
                            }

                        }
                    }
                    QuotationLogModel log = new QuotationLogModel
                    {
                        QUOTATION_ID = Convert.ToInt32(data.ID),
                        TENDER_NO = data.TENDER_NO,
                        TYPE = "MODIFY",
                        ACTION = "MODIFY",
                        ACTION_BY = _workContext.CurrentUserinformation.login_code,
                        CHANGED = data.TENDER_NO,
                        REMARKS = data.LOG_REMARKS
                    };
                    InsertRemarks(log);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool InsertItemData(Item item, string tenderNo, int q_no, string form_code)
        {
            try
            {
                item = ProcessImageData(item);
                var idquery = $@"SELECT COALESCE(MAX(ID) + 1, 1) AS id FROM sa_quotation_Items";
                int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();
                string insertItemQuery = string.Format(@"INSERT INTO sa_quotation_Items (ID,TENDER_NO, ITEM_CODE, SPECIFICATION, IMAGE, UNIT, QUANTITY, Category, BRAND_NAME, INTERFACE, TYPE, LAMINATION, ITEM_SIZE, THICKNESS, COLOR, GRADE, SIZE_LENGTH, SIZE_WIDTH,DELETED_FLAG, QUOTATION_NO, FORM_CODE) 
                             VALUES({0}, '{1}', '{2}', '{3}', '{4}', '{5}', {6}, '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', {16}, {17},'{18}','{19}', '{20}')",
                                     id, tenderNo,item.ITEM_CODE, item.SPECIFICATION, item.IMAGE, item.UNIT, item.QUANTITY, item.CATEGORY, item.BRAND_NAME, item.INTERFACE, item.TYPE, item.LAMINATION, item.ITEM_SIZE, item.THICKNESS, item.COLOR, item.GRADE, item.SIZE_LENGTH, item.SIZE_WIDTH,"N",q_no, form_code);

                _dbContext.ExecuteSqlCommand(insertItemQuery);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private Item ProcessImageData(Item item)
        {
            if (!string.IsNullOrEmpty(item.IMAGE))
            {
                byte[] imageBytes = Convert.FromBase64String(item.IMAGE);

                string folderPath = "~/Areas/NeoERP.QuotationManagement/Image/Items/";
                string imageName = $"{Guid.NewGuid()}.png"; // Generating unique image name
                string imagePath = $"{folderPath}{imageName}"; // Combining folder path and image name
                string physicalPath = HttpContext.Current.Server.MapPath(imagePath);
                File.WriteAllBytes(physicalPath, imageBytes);
                item.IMAGE = imageName;
            }
            else
            {
                item.IMAGE = item.IMAGE_NAME;
            }

            return item;
        }

        public bool UpdateItemData(Item item, string tenderNo, int q_no, string form_code)
        {
            try
            {
                item = ProcessImageData(item);

                string updateItemQuery = string.Format(@"UPDATE sa_quotation_Items  SET ITEM_CODE = '{2}', 
                 SPECIFICATION = '{3}', IMAGE = '{4}',UNIT = '{5}', QUANTITY = {6},Category = '{7}',BRAND_NAME = '{8}', 
                 INTERFACE = '{9}',TYPE = '{10}',LAMINATION = '{11}', ITEM_SIZE = '{12}', THICKNESS = '{13}', COLOR = '{14}', 
                 GRADE = '{15}',SIZE_LENGTH = {16},SIZE_WIDTH = {17} WHERE TENDER_NO = '{1}' AND ID = {0} AND QUOTATION_NO = '{18}'",
                                         item.ID, tenderNo, item.ITEM_CODE, item.SPECIFICATION, item.IMAGE, item.UNIT, item.QUANTITY, item.CATEGORY, item.BRAND_NAME, item.INTERFACE, item.TYPE, item.LAMINATION, item.ITEM_SIZE, item.THICKNESS, item.COLOR, item.GRADE, item.SIZE_LENGTH, item.SIZE_WIDTH, q_no, "N");
                _dbContext.ExecuteSqlCommand(updateItemQuery);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Quotation_setup> GetTenderId(string tenderNo)
        {
            string query = $@"select id from sa_quotation_setup where id='{tenderNo}' AND STATUS = 'E'";
            List<Quotation_setup> id = _dbContext.SqlQuery<Quotation_setup>(query).ToList();
            return id;
        }

        public List<Quotation_setup> ListAllTenders()
        {
            string query = $@"SELECT ID, TENDER_NO,ISSUE_DATE,VALID_DATE,CREATED_DATE,bs_date(ISSUE_DATE) as NEPALI_DATE,
       CASE WHEN status = 'D' THEN 'Rejected' WHEN approved_status = 'Y' THEN 'Approved'  ELSE 'Pending'  END AS approved_status FROM sa_quotation_setup WHERE company_code='{_workContext.CurrentUserinformation.company_code}' ORDER BY id desc";
            List<Quotation_setup> tenderDetails = _dbContext.SqlQuery<Quotation_setup>(query).ToList();
            return tenderDetails;
        }
        public List<Quotation_setup> ListAllPendingTenders()
        {
            string query = $@"SELECT ID, TENDER_NO,ISSUE_DATE,VALID_DATE,CREATED_DATE,bs_date(ISSUE_DATE) as NEPALI_DATE,
       CASE   WHEN approved_status = 'Y' THEN 'Approved'  ELSE 'Pending'  END AS approved_status FROM sa_quotation_setup WHERE status = 'E' and company_code='{_workContext.CurrentUserinformation.company_code}' and approved_status = 'N' ORDER BY id desc";
            List<Quotation_setup> tenderDetails = _dbContext.SqlQuery<Quotation_setup>(query).ToList();
            return tenderDetails;
        }
        public bool deleteQuotationId(ApprovalRequest request)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE sa_quotation_setup SET STATUS ='D' WHERE ID='{request.ID}' AND TENDER_NO = '{request.TENDER_NO}'";
                var update_item_query = $@"UPDATE sa_quotation_Items SET DELETED_FLAG = 'Y' WHERE QUOTATION_NO = '{request.ID}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                _dbContext.ExecuteSqlCommand(update_item_query);
                QuotationLogModel log = new QuotationLogModel
                {
                    QUOTATION_ID = Convert.ToInt32(request.ID),
                    TENDER_NO = request.TENDER_NO,
                    TYPE = "MODIFY",
                    ACTION = "REJECTED",
                    ACTION_BY = _workContext.CurrentUserinformation.login_code,
                    CHANGED = request.TENDER_NO,
                    REMARKS = request.REMARKS
                };
                InsertRemarks(log);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Quotation_setup> GetQuotationById(string tenderNo)
        {
            try
            {
                // Fetch project data
                string Query = $@"SELECT ID, TENDER_NO,ISSUE_DATE,VALID_DATE,CREATED_DATE,bs_date(ISSUE_DATE) as NEPALI_DATE,bs_date(VALID_DATE) as DELIVERY_DT_BS,REMARKS,ID,
                (CASE WHEN APPROVED_STATUS='Y' THEN 'Approved' else 'Pending' END) AS APPROVED_STATUS, LOCAL_FLAG FROM sa_quotation_setup WHERE ID = '{tenderNo}' AND STATUS = 'E'";
                List<Quotation_setup> quotations = this._dbContext.SqlQuery<Quotation_setup>(Query).ToList();
                foreach (var quotation in quotations)
                {
                    string query = $@"select * from sa_quotation_Items where QUOTATION_NO = '{tenderNo}' AND TENDER_NO='{quotation.TENDER_NO}' AND DELETED_FLAG='N' order by id";
                    List<Item> itemData = this._dbContext.SqlQuery<Item>(query).ToList();
                    quotation.Items = itemData;
                }

                return quotations;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool updateItemsById(string tenderNo,string id, string q_no)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE sa_quotation_Items SET deleted_flag ='Y' WHERE TENDER_NO='{tenderNo}' and id='{id}' AND QUOTATION_NO = '{q_no}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Quotation_Details> ListQuotationDetails()
        {
            string query = $@"SELECT QD.QUOTATION_NO,QD.TENDER_NO,QD.PAN_NO,QD.PARTY_NAME,QD.ADDRESS,QD.CONTACT_NO,QD.EMAIL,QD.CURRENCY,QD.CURRENCY_RATE,QD.DELIVERY_DATE,
            QD.TOTAL_AMOUNT,QD.TOTAL_DISCOUNT,QD.TOTAL_EXCISE,QD.TOTAL_TAXABLE_AMOUNT,QD.TOTAL_VAT,QD.TOTAL_NET_AMOUNT,
            (CASE WHEN QD.STATUS='RQ' then 'Pending' when QD.status='AP' then 'Approved' else 'Reject' end) AS STATUS,
            QD.TERM_CONDITION FROM  QUOTATION_DETAILS QD,SA_QUOTATION_SETUP SQS WHERE SQS.TENDER_NO=QD.TENDER_NO AND
            SQS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' ORDER BY QD.QUOTATION_NO DESC";
            List<Quotation_Details> tenderDetails = _dbContext.SqlQuery<Quotation_Details>(query).ToList();
            return tenderDetails;
        }
        public List<CurrencyModel> getCurrency()
        {
            var company = _workContext.CurrentUserinformation.company_code;
            List<CurrencyModel> rate = new List<CurrencyModel>();
            string query = $@"select * from currency_setup where COMPANY_CODE='{company}'";
            rate = _dbContext.SqlQuery<CurrencyModel>(query).ToList();
            return rate;
        }
        public List<Quotation_Details> QuotationDetailsById(string quotationNo,string tenderNo)
        {
            try
            {
                var login_code = _workContext.CurrentUserinformation.login_code;

                string Query = $@"SELECT SQS.ISSUE_DATE,SQS.VALID_DATE,BS_DATE(SQS.ISSUE_DATE) AS NEPALI_DATE,BS_DATE(SQS.VALID_DATE) AS DELIVERY_DT_BS,QD.QUOTATION_NO,QD.TENDER_NO,QD.PAN_NO,scs.supplier_edesc as party_name,scs.regd_office_eaddress as address,
                scs.tel_mobile_no1 as contact_no,scs.EMAIL,QD.CURRENCY,QD.CURRENCY_RATE,QD.DELIVERY_DATE,BS_DATE(QD.DELIVERY_DATE) as DELIVERY_DT_NEP,
                QD.TOTAL_AMOUNT,QD.TOTAL_DISCOUNT,QD.TOTAL_EXCISE,QD.TOTAL_TAXABLE_AMOUNT,QD.TOTAL_VAT,QD.TOTAL_NET_AMOUNT,
                CASE 
                WHEN QD.STATUS = 'AP' THEN 'Approved' WHEN QD.STATUS = 'R' THEN 'Reject' WHEN QD.STATUS = 'C' THEN 'Checked' WHEN QD.STATUS = 'V' THEN 'Verified' WHEN QD.STATUS = 'RE' THEN 'Recommended' WHEN QD.STATUS = 'P' THEN 'Posted'
                WHEN NOT EXISTS (SELECT 1 FROM QUOTATION_DETAILS WHERE TENDER_NO = QD.TENDER_NO AND STATUS = 'AP') THEN 'Pending'
                ELSE 'Reject' END AS STATUS,QD.DISCOUNT_TYPE,SQS.REMARKS,
                   CASE
                       WHEN     qd.TOTAL_NET_AMOUNT >= 50000
                            AND (SELECT COUNT (*)
                                   FROM quotation_detail_itemwise
                                  WHERE     quotation_no = qd.quotation_no
                                        AND RECOMMENDED4_BY IS NOT NULL) >
                                0
                       THEN
                           'Y'
                       ELSE
                           'N'
                   END  AS is_full_recommendation,
                   CASE
                        WHEN qd.TOTAL_NET_AMOUNT >= 50000
                             AND EXISTS (
                                    SELECT 1
                                    FROM quotation_detail_itemwise qdi
                                    WHERE qdi.quotation_no = qd.quotation_no
                                          AND (
                                                    LOWER(qdi.RECOMMENDED1_BY) = LOWER('{login_code}')
                                                 OR LOWER(qdi.RECOMMENDED2_BY) = LOWER('{login_code}')
                                                 OR LOWER(qdi.RECOMMENDED3_BY) = LOWER('{login_code}')
                                                 OR LOWER(qdi.RECOMMENDED4_BY) = LOWER('{login_code}')
                                              )
                             )
                        THEN 'Y'
                        ELSE 'N'
                    END AS IS_SELF_RECOMMEND,
                (
                        SELECT CASE
                                   WHEN SUM(CASE WHEN status IN ('AP', 'P') THEN 1 ELSE 0 END) > 0 
                                        THEN 'Y'
                                   ELSE 'N'
                               END
                        FROM quotation_details
                        WHERE tender_no = QD.TENDER_NO
                    ) AS IS_APPROVED,
                case 
                         when (
                         select distinct count(item_code) from quotation_detail_itemwise 
                            where quotation_no in (
                            select quotation_no from quotation_details 
                            where tender_no = (select tender_no from SA_QUOTATION_SETUP where id = '{tenderNo}')
                            ) and approved_by is null
                            ) > 0 then 'N'
                    else 'Y'
                         end AS IS_ALL_APPROVED
                FROM  QUOTATION_DETAILS  QD,SA_QUOTATION_SETUP SQS , ip_supplier_setup scs WHERE SQS.TENDER_NO=QD.TENDER_NO and scs.supplier_code=qd.supplier_code and sqs.company_code=scs.company_code AND  QD.QUOTATION_NO='{quotationNo}' and sqs.id = '{tenderNo}' and sqs.company_code = '{_workContext.CurrentUserinformation.company_code}' and sqs.status = 'E'";
                List<Quotation_Details> quotations = this._dbContext.SqlQuery<Quotation_Details>(Query).ToList();
                foreach (var quotation in quotations)
                {
                    string query =$@"SELECT QDI.ID, QDI.ITEM_CODE,SQI.SPECIFICATION, SQI.IMAGE,SQI.UNIT,SQI.QUANTITY,SQI.CATEGORY, SQI.BRAND_NAME,SQI.INTERFACE,
                        SQI.TYPE, SQI.LAMINATION, SQI.ITEM_SIZE,SQI.THICKNESS,SQI.COLOR,SQI.GRADE,SQI.SIZE_LENGTH, SQI.SIZE_WIDTH,QDI.RATE,
                        QDI.AMOUNT, QDI.DISCOUNT, QDI.DISCOUNT_AMOUNT,QDI.EXCISE,QDI.TAXABLE_AMOUNT, QDI.VAT_AMOUNT,QDI.NET_AMOUNT,
                        CASE
                             WHEN qd.status = 'C' AND QDI.CHECKED_BY IS NOT NULL
                             THEN
                                 'TRUE'
                             WHEN qd.status = 'V' AND QDI.VERIFY_BY IS NOT NULL
                             THEN
                                 'TRUE'
                             WHEN     qd.status = 'RE'
                                  AND (   QDI.RECOMMENDED1_BY IS NOT NULL
                                       OR QDI.RECOMMENDED2_BY IS NOT NULL
                                       OR QDI.RECOMMENDED3_BY IS NOT NULL
                                       OR QDI.RECOMMENDED4_BY IS NOT NULL)
                             THEN
                                 'TRUE'
                             WHEN qd.status = 'AP' AND QDI.APPROVED_BY IS NOT NULL
                             THEN
                                 'TRUE'
                             WHEN qd.status = 'P' AND QDI.APPROVED_BY IS NOT NULL
                             THEN
                                 'TRUE'
                             ELSE
                                 'FALSE'
                         END
                             AS ISSELECTED,
                            CASE
                                         WHEN QDI.APPROVED_BY IS NOT NULL
                                          THEN 'TRUE'
                                         ELSE
                                             'FALSE'
                                     END    AS IS_APPROVED,
                                     CASE
                                         WHEN QDI.RECOMMENDED4_BY IS NOT NULL
                                          THEN 'TRUE'
                                         ELSE
                                             'FALSE'
                                     END    AS IS_RECOMMENDED,
                                    case
                                        when (select count(approved_by) from quotation_detail_itemwise where quotation_no in (select quotation_no from quotation_details where tender_no = SQI.TENDER_NO) and item_code = qdi.item_code) > 0
                                        then 'TRUE'
                                        else 'FALSE' 
                                     end as IS_ITEM_APPROVED
                        FROM   (select a.*,ROW_NUMBER() OVER (ORDER BY a.id) AS SERIAL_NO from SA_QUOTATION_ITEMS a where a.quotation_no='{tenderNo}') SQI,
                        (select a.*,ROW_NUMBER() OVER (ORDER BY a.id) AS SERIAL_NO from  QUOTATION_DETAIL_ITEMWISE a where a.QUOTATION_NO='{quotationNo}') QDI,
                        quotation_details qd  
                        WHERE SQI.QUOTATION_NO = '{tenderNo}' AND QD.QUOTATION_NO = '{quotationNo}' and SQI.serial_no = QDI.serial_no(+) and qd.tender_no = sqi.tender_no(+) AND SQI.DELETED_FLAG = 'N'
                        order by QDI.ID asc";
                    List<Item_details> itemData = this._dbContext.SqlQuery<Item_details>(query).ToList();
                    quotation.Item_Detail = itemData;
                    string vquery = $@"SELECT * FROM QUOTATION_TERM_CONDITION WHERE TENDER_NO='{quotation.TENDER_NO}' AND QUOTATION_NO='{quotationNo}'";
                    List<Term_Conditions> TermCondition = this._dbContext.SqlQuery<Term_Conditions>(vquery).ToList();
                    quotation.TermsCondition = TermCondition;
                }
                List<QuotationTransaction> imagelist = new List<QuotationTransaction>();
                if (quotations.Count > 0)
                {
                    string imagequery = $@"SELECT * FROM QUOTATION_TRANSACTION WHERE QUOTATION_NO ='{quotationNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and TENDER_NO=(SELECT TENDER_NO from sa_quotation_setup where id = '{tenderNo}') and DELETED_FLAG='N'";
                    imagelist = this._dbContext.SqlQuery<QuotationTransaction>(imagequery).ToList();
                    quotations[0].IMAGES_LIST = imagelist;
                }
                return quotations;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Quotation_Details> QuotationDetailsId(string quotationNo, string tenderNo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(quotationNo))
                {
                    string vQuery = $@"
                        SELECT TENDER_NO,
                           SQS.ISSUE_DATE,
                           SQS.VALID_DATE,
                           BS_DATE (SQS.ISSUE_DATE) AS NEPALI_DATE,
                           BS_DATE (SQS.VALID_DATE) AS DELIVERY_DT_BS,
                           SQS.REMARKS, TRIM(A.LOGIN_EDESC) as CREATED_BY
                      FROM SA_QUOTATION_SETUP SQS
                       JOIN
                          SC_APPLICATION_USERS A
                       ON     LOWER (SQS.CREATED_BY) = LOWER (A.LOGIN_CODE)
                          AND SQS.COMPANY_CODE = A.COMPANY_CODE
                          AND ROWNUM = 1
                      WHERE SQS.ID = '{tenderNo}'
                      AND SQS.STATUS = 'E'
                      AND SQS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                    ";
                    List<Quotation_Details> vendor_quotations = this._dbContext.SqlQuery<Quotation_Details>(vQuery).ToList();

                    foreach (var vendors in vendor_quotations)
                    {
                        string quotationQuery = $@"
                    with Agg
                AS
                    (  SELECT qd.Quotation_no,
                              LISTAGG (UPPER(cb), ', ') WITHIN GROUP (ORDER BY UPPER(cb))
                                  AS CHECKED_BY,
                              LISTAGG (UPPER(vb), ', ') WITHIN GROUP (ORDER BY UPPER(vb))
                                  AS VERIFIED_BY,
                              LISTAGG (UPPER(ab), ', ') WITHIN GROUP (ORDER BY UPPER(ab))
                                  AS APPROVED_BY,
                              LISTAGG (UPPER(rb), ', ') WITHIN GROUP (ORDER BY UPPER(rb))
                                  AS RECOMMENDED_BY
                         FROM QUOTATION_DETAILS qd
                              LEFT JOIN
                              (SELECT DISTINCT
                                      qdi.QUOTATION_NO, TRIM (qdi.CHECKED_BY) AS cb
                                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                                WHERE TRIM (qdi.CHECKED_BY) IS NOT NULL) cb_list
                                  ON cb_list.QUOTATION_NO = qd.QUOTATION_NO
                              LEFT JOIN
                              (SELECT DISTINCT qdi.QUOTATION_NO, TRIM (qdi.VERIFY_BY) AS vb
                                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                                WHERE TRIM (qdi.VERIFY_BY) IS NOT NULL) vb_list
                                  ON vb_list.QUOTATION_NO = qd.QUOTATION_NO
                              LEFT JOIN
                              (SELECT DISTINCT
                                      qdi.QUOTATION_NO, TRIM (qdi.APPROVED_BY) AS ab
                                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                                WHERE TRIM (qdi.APPROVED_BY) IS NOT NULL) ab_list
                                  ON ab_list.QUOTATION_NO = qd.QUOTATION_NO
                              LEFT JOIN
                              (SELECT DISTINCT QUOTATION_NO, rb
                                 FROM (SELECT QUOTATION_NO, TRIM (RECOMMENDED1_BY) AS rb
                                         FROM QUOTATION_DETAIL_ITEMWISE
                                       UNION
                                       SELECT QUOTATION_NO, TRIM (RECOMMENDED2_BY)
                                         FROM QUOTATION_DETAIL_ITEMWISE
                                       UNION
                                       SELECT QUOTATION_NO, TRIM (RECOMMENDED3_BY)
                                         FROM QUOTATION_DETAIL_ITEMWISE
                                       UNION
                                       SELECT QUOTATION_NO, TRIM (RECOMMENDED4_BY)
                                         FROM QUOTATION_DETAIL_ITEMWISE)
                                WHERE rb IS NOT NULL) rb_list
                                  ON rb_list.QUOTATION_NO = qd.QUOTATION_NO
                     GROUP BY qd.QUOTATION_NO)
                    SELECT DISTINCT
                    SQS.ISSUE_DATE,
                    SQS.VALID_DATE,
                    BS_DATE(SQS.ISSUE_DATE) AS NEPALI_DATE,
                    BS_DATE(SQS.VALID_DATE) AS DELIVERY_DT_BS,
                    QD.QUOTATION_NO,
                    QD.TENDER_NO,
                    QD.PAN_NO,
                    SCS.SUPPLIER_EDESC AS PARTY_NAME,
                    SCS.REGD_OFFICE_EADDRESS AS ADDRESS,
                    SCS.TEL_MOBILE_NO1 AS CONTACT_NO,
                    SCS.EMAIL,
                    QD.CURRENCY,
                    QD.CURRENCY_RATE,
                    QD.DELIVERY_DATE,
                    BS_DATE(QD.DELIVERY_DATE) AS DELIVERY_DT_NEP,
                    QD.TOTAL_AMOUNT,
                    QD.TOTAL_DISCOUNT,
                    QD.TOTAL_EXCISE,
                    QD.TOTAL_TAXABLE_AMOUNT,
                    QD.TOTAL_VAT,
                    QD.TOTAL_NET_AMOUNT,
                    CASE
                        WHEN QD.STATUS = 'AP' THEN 'Approved'
                        WHEN QD.STATUS = 'R' THEN 'Reject'
                        WHEN QD.STATUS = 'C' THEN 'Checked'
                        WHEN QD.STATUS = 'RE' THEN 'Recommended'
                        WHEN QD.STATUS = 'P' THEN 'Posted'
                        WHEN NOT EXISTS (
                            SELECT 1 FROM QUOTATION_DETAILS
                            WHERE TENDER_NO = QD.TENDER_NO AND STATUS = 'AP'
                        ) THEN 'Pending'
                        ELSE 'Reject'
                    END AS STATUS,
                    QD.DISCOUNT_TYPE,
                    SQS.REMARKS,
                    EU.LOGIN_EDESC AS CREATED_BY,
                    A.CHECKED_BY AS CHECKED_BY,
                    A.RECOMMENDED_BY AS RECOMMEND_BY,
                    A.APPROVED_BY AS APPROVED_BY,
                    A.VERIFIED_BY AS VERIFIED_BY
                FROM 
                    QUOTATION_DETAILS QD
                JOIN 
                    SA_QUOTATION_SETUP SQS ON SQS.TENDER_NO = QD.TENDER_NO
                JOIN 
                    IP_SUPPLIER_SETUP SCS ON SCS.SUPPLIER_CODE = QD.SUPPLIER_CODE 
                        AND SQS.COMPANY_CODE = SCS.COMPANY_CODE
                LEFT JOIN SC_APPLICATION_USERS EU ON SQS.COMPANY_CODE = EU.COMPANY_CODE AND LOWER(SQS.CREATED_BY) = LOWER(EU.LOGIN_CODE)
                LEFT JOIN SC_APPLICATION_USERS CU ON  LOWER(QD.CHECKED_BY) = LOWER(CU.LOGIN_CODE)
                LEFT JOIN SC_APPLICATION_USERS RU ON  LOWER(QD.RECOMMENDED_BY) = LOWER(RU.LOGIN_CODE)
                LEFT JOIN SC_APPLICATION_USERS AU ON  LOWER(QD.APPROVED_BY) = LOWER(AU.LOGIN_CODE)
                LEFT JOIN SC_APPLICATION_USERS PU ON  LOWER(QD.POSTED_BY) = LOWER(PU.LOGIN_CODE)
                left join Agg a on QD.QUOTATION_NO = a.QUOTATION_NO
                WHERE 
                    SQS.ID = '{tenderNo}'
                    AND SQS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
                    AND SQS.STATUS = 'E'
                ORDER BY 
                    SQS.ID DESC
        ";
                        vendors.Vendors = this._dbContext.SqlQuery<Vendor_Details>(quotationQuery).ToList();
                        foreach (var quotation in vendors.Vendors)
                        {
                            string query = $@"SELECT QDI.ID, iims.ITEM_EDESC as ITEM_CODE, SQI.SPECIFICATION, SQI.IMAGE,SQI.UNIT,SQI.QUANTITY,SQI.CATEGORY, SQI.BRAND_NAME,SQI.INTERFACE,
            SQI.TYPE, SQI.LAMINATION, SQI.ITEM_SIZE,SQI.THICKNESS,SQI.COLOR,SQI.GRADE,SQI.SIZE_LENGTH, SQI.SIZE_WIDTH,QDI.RATE,
            QDI.AMOUNT, QDI.DISCOUNT, QDI.DISCOUNT_AMOUNT,QDI.EXCISE,QDI.TAXABLE_AMOUNT, QDI.VAT_AMOUNT,QDI.NET_AMOUNT,
            QDI.CHECKED_BY, QDI.VERIFY_BY AS VERIFIED_BY, QDI.APPROVED_BY,
            RTRIM(
                NVL2(recommended1_by, recommended1_by || ', ', '') ||
                NVL2(recommended2_by, recommended2_by || ', ', '') ||
                NVL2(recommended3_by, recommended3_by || ', ', '') ||
                NVL2(recommended4_by, recommended4_by || ', ', ''),
                ', '
            ) AS RECOMMEND_BY
            FROM   (select a.*,ROW_NUMBER() OVER (ORDER BY a.id) AS SERIAL_NO from SA_QUOTATION_ITEMS a where a.quotation_no='{tenderNo}') SQI,
            (select a.*,ROW_NUMBER() OVER (ORDER BY a.id) AS SERIAL_NO from  QUOTATION_DETAIL_ITEMWISE a where a.QUOTATION_NO='{quotation.QUOTATION_NO}') QDI,
            quotation_details qd , ip_item_master_setup iims
            WHERE SQI.QUOTATION_NO = '{tenderNo}' AND QD.QUOTATION_NO = '{quotation.QUOTATION_NO}' and SQI.serial_no = QDI.serial_no(+) and qd.tender_no = sqi.tender_no(+) and qdi.item_code = iims.item_code (+) AND SQI.DELETED_FLAG = 'N'
            and iims.deleted_flag = 'N' AND iims.company_code = '{_workContext.CurrentUserinformation.company_code}'
            order by QDI.ID asc";
                            List<Item_details> itemData = this._dbContext.SqlQuery<Item_details>(query).ToList();
                            quotation.Item_Detail = itemData;
                            string vquery = $@"SELECT * FROM QUOTATION_TERM_CONDITION WHERE TENDER_NO='{quotation.TENDER_NO}' AND QUOTATION_NO='{quotation.QUOTATION_NO}'";
                            List<Term_Conditions> TermCondition = this._dbContext.SqlQuery<Term_Conditions>(vquery).ToList();
                            quotation.TermsCondition = TermCondition;
                        }
                    }
                    return vendor_quotations;
                }
                string Query = $@"
    with Agg
AS
    (  SELECT qd.Quotation_no,
              LISTAGG (UPPER(cb), ', ') WITHIN GROUP (ORDER BY UPPER(cb))
                  AS CHECKED_BY,
              LISTAGG (UPPER(vb), ', ') WITHIN GROUP (ORDER BY UPPER(vb))
                  AS VERIFIED_BY,
              LISTAGG (UPPER(ab), ', ') WITHIN GROUP (ORDER BY UPPER(ab))
                  AS APPROVED_BY,
              LISTAGG (UPPER(rb), ', ') WITHIN GROUP (ORDER BY UPPER(rb))
                  AS RECOMMENDED_BY
         FROM QUOTATION_DETAILS qd
              LEFT JOIN
              (SELECT DISTINCT
                      qdi.QUOTATION_NO, TRIM (qdi.CHECKED_BY) AS cb
                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                WHERE TRIM (qdi.CHECKED_BY) IS NOT NULL) cb_list
                  ON cb_list.QUOTATION_NO = qd.QUOTATION_NO
              LEFT JOIN
              (SELECT DISTINCT qdi.QUOTATION_NO, TRIM (qdi.VERIFY_BY) AS vb
                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                WHERE TRIM (qdi.VERIFY_BY) IS NOT NULL) vb_list
                  ON vb_list.QUOTATION_NO = qd.QUOTATION_NO
              LEFT JOIN
              (SELECT DISTINCT
                      qdi.QUOTATION_NO, TRIM (qdi.APPROVED_BY) AS ab
                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                WHERE TRIM (qdi.APPROVED_BY) IS NOT NULL) ab_list
                  ON ab_list.QUOTATION_NO = qd.QUOTATION_NO
              LEFT JOIN
              (SELECT DISTINCT QUOTATION_NO, rb
                 FROM (SELECT QUOTATION_NO, TRIM (RECOMMENDED1_BY) AS rb
                         FROM QUOTATION_DETAIL_ITEMWISE
                       UNION
                       SELECT QUOTATION_NO, TRIM (RECOMMENDED2_BY)
                         FROM QUOTATION_DETAIL_ITEMWISE
                       UNION
                       SELECT QUOTATION_NO, TRIM (RECOMMENDED3_BY)
                         FROM QUOTATION_DETAIL_ITEMWISE
                       UNION
                       SELECT QUOTATION_NO, TRIM (RECOMMENDED4_BY)
                         FROM QUOTATION_DETAIL_ITEMWISE)
                WHERE rb IS NOT NULL) rb_list
                  ON rb_list.QUOTATION_NO = qd.QUOTATION_NO
     GROUP BY qd.QUOTATION_NO)
    SELECT 
    SQS.ISSUE_DATE,
    SQS.VALID_DATE,
    BS_DATE(SQS.ISSUE_DATE) AS NEPALI_DATE,
    BS_DATE(SQS.VALID_DATE) AS DELIVERY_DT_BS,
    QD.QUOTATION_NO,
    QD.TENDER_NO,
    QD.PAN_NO,
    SCS.SUPPLIER_EDESC AS PARTY_NAME,
    SCS.REGD_OFFICE_EADDRESS AS ADDRESS,
    SCS.TEL_MOBILE_NO1 AS CONTACT_NO,
    SCS.EMAIL,
    QD.CURRENCY,
    QD.CURRENCY_RATE,
    QD.DELIVERY_DATE,
    BS_DATE(QD.DELIVERY_DATE) AS DELIVERY_DT_NEP,
    QD.TOTAL_AMOUNT,
    QD.TOTAL_DISCOUNT,
    QD.TOTAL_EXCISE,
    QD.TOTAL_TAXABLE_AMOUNT,
    QD.TOTAL_VAT,
    QD.TOTAL_NET_AMOUNT,
    CASE
        WHEN QD.STATUS = 'AP' THEN 'Approved'
        WHEN QD.STATUS = 'R' THEN 'Reject'
        WHEN QD.STATUS = 'C' THEN 'Checked'
        WHEN QD.STATUS = 'RE' THEN 'Recommended'
        WHEN QD.STATUS = 'P' THEN 'Posted'
        WHEN NOT EXISTS (
            SELECT 1 FROM QUOTATION_DETAILS
            WHERE TENDER_NO = QD.TENDER_NO AND STATUS = 'AP'
        ) THEN 'Pending'
        ELSE 'Reject'
    END AS STATUS,
    QD.DISCOUNT_TYPE,
    SQS.REMARKS,
    EU.LOGIN_EDESC AS CREATED_BY,
    A.CHECKED_BY AS CHECKED_BY,
    A.RECOMMENDED_BY AS RECOMMEND_BY,
    A.APPROVED_BY AS APPROVED_BY,
    A.VERIFIED_BY AS VERIFIED_BY
FROM 
    QUOTATION_DETAILS QD
JOIN 
    SA_QUOTATION_SETUP SQS ON SQS.TENDER_NO = QD.TENDER_NO
JOIN 
    IP_SUPPLIER_SETUP SCS ON SCS.SUPPLIER_CODE = QD.SUPPLIER_CODE 
        AND SQS.COMPANY_CODE = SCS.COMPANY_CODE
LEFT JOIN SC_APPLICATION_USERS EU ON SQS.COMPANY_CODE = EU.COMPANY_CODE AND LOWER(SQS.CREATED_BY) = LOWER(EU.LOGIN_CODE)
LEFT JOIN SC_APPLICATION_USERS CU ON  LOWER(QD.CHECKED_BY) = LOWER(CU.LOGIN_CODE)
LEFT JOIN SC_APPLICATION_USERS RU ON  LOWER(QD.RECOMMENDED_BY) = LOWER(RU.LOGIN_CODE)
LEFT JOIN SC_APPLICATION_USERS AU ON  LOWER(QD.APPROVED_BY) = LOWER(AU.LOGIN_CODE)
LEFT JOIN SC_APPLICATION_USERS PU ON  LOWER(QD.POSTED_BY) = LOWER(PU.LOGIN_CODE)
left join Agg a on QD.QUOTATION_NO = a.QUOTATION_NO
WHERE 
    QD.QUOTATION_NO = '{quotationNo}'
    AND SQS.ID = '{tenderNo}'
    AND SQS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'
    AND SQS.STATUS = 'E'
ORDER BY 
    SQS.ID DESC";
                List<Quotation_Details> quotations = this._dbContext.SqlQuery<Quotation_Details>(Query).ToList();
                foreach (var quotation in quotations)
                {
                    quotation.CHECKED_BY = MakeDistinct(quotation.CHECKED_BY);
                    quotation.VERIFIED_BY = MakeDistinct(quotation.VERIFIED_BY);
                    quotation.APPROVED_BY = MakeDistinct(quotation.APPROVED_BY);
                    quotation.RECOMMEND_BY = MakeDistinct(quotation.RECOMMEND_BY);
                    string query = $@"SELECT QDI.ID, iims.ITEM_EDESC as ITEM_CODE, SQI.SPECIFICATION, SQI.IMAGE,SQI.UNIT,SQI.QUANTITY,SQI.CATEGORY, SQI.BRAND_NAME,SQI.INTERFACE,
                        SQI.TYPE, SQI.LAMINATION, SQI.ITEM_SIZE,SQI.THICKNESS,SQI.COLOR,SQI.GRADE,SQI.SIZE_LENGTH, SQI.SIZE_WIDTH,QDI.RATE,
                        QDI.AMOUNT, QDI.DISCOUNT, QDI.DISCOUNT_AMOUNT,QDI.EXCISE,QDI.TAXABLE_AMOUNT, QDI.VAT_AMOUNT,QDI.NET_AMOUNT
                        FROM   (select a.*,ROW_NUMBER() OVER (ORDER BY a.id) AS SERIAL_NO from SA_QUOTATION_ITEMS a where a.quotation_no='{tenderNo}') SQI,
                        (select a.*,ROW_NUMBER() OVER (ORDER BY a.id) AS SERIAL_NO from  QUOTATION_DETAIL_ITEMWISE a where a.QUOTATION_NO='{quotationNo}') QDI,
                        quotation_details qd , ip_item_master_setup iims
                        WHERE SQI.QUOTATION_NO = '{tenderNo}' AND QD.QUOTATION_NO = '{quotationNo}' and SQI.serial_no = QDI.serial_no(+) and qd.tender_no = sqi.tender_no(+) and qdi.item_code = iims.item_code (+) AND SQI.DELETED_FLAG = 'N'
                        and iims.deleted_flag = 'N' AND iims.company_code = '{_workContext.CurrentUserinformation.company_code}' and qdi.approved_by is not null
                        order by QDI.ID asc";
                    List<Item_details> itemData = this._dbContext.SqlQuery<Item_details>(query).ToList();
                    quotation.Item_Detail = itemData;
                    string vquery = $@"SELECT * FROM QUOTATION_TERM_CONDITION WHERE TENDER_NO='{quotation.TENDER_NO}' AND QUOTATION_NO='{quotationNo}'";
                    List<Term_Conditions> TermCondition = this._dbContext.SqlQuery<Term_Conditions>(vquery).ToList();
                    quotation.TermsCondition = TermCondition;
                }
                List<QuotationTransaction> imagelist = new List<QuotationTransaction>();
                if (quotations.Count > 0)
                {
                    string imagequery = $@"SELECT * FROM QUOTATION_TRANSACTION WHERE QUOTATION_NO ='{quotationNo}' and COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' and TENDER_NO=(SELECT TENDER_NO from sa_quotation_setup where id = '{tenderNo}') and DELETED_FLAG='N'";
                    imagelist = this._dbContext.SqlQuery<QuotationTransaction>(imagequery).ToList();
                    quotations[0].IMAGES_LIST = imagelist;
                }
                return quotations;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<SummaryReport> TendersItemWise(ReportFiltersModel model)
        {
            try
            {
                string fromDate = model.FromDate ?? string.Empty;
                string toDate = model.ToDate ?? string.Empty;
                string dateFilter = string.Empty;
                if (!string.IsNullOrWhiteSpace(fromDate) && !string.IsNullOrWhiteSpace(toDate))
                    dateFilter = $"AND TRUNC(SQS.CREATED_DATE) BETWEEN TO_DATE('{fromDate}','YYYY-MON-DD') AND TO_DATE('{toDate}','YYYY-MON-DD')";
                string Query = $@"WITH StatusPriority AS (
    SELECT 
        TENDER_NO,
        MIN(CASE STATUS
            WHEN 'P' THEN 1
            WHEN 'AP' THEN 2
            WHEN 'RE' THEN 3
            WHEN 'V' Then 4
            WHEN 'C' THEN 5
            WHEN 'RQ' THEN 6
            WHEN 'R' THEN 7
            ELSE 8
        END) AS PRIORITY
    FROM 
        QUOTATION_DETAILS
    GROUP BY 
        TENDER_NO
), 
StatusLabel AS (
    SELECT 
        TENDER_NO,
        CASE PRIORITY
            WHEN 1 THEN 'Posted'
            WHEN 2 THEN 'Approved'
            WHEN 3 THEN 'Recommended'
            WHEN 4 THEN 'Verified'
            WHEN 5 THEN 'Checked'
            WHEN 6 THEN 'Requested'
            WHEN 7 THEN 'Rejected'
            ELSE 'Waiting'
        END AS STATUS
    FROM StatusPriority
),
PersonAggregation
AS
    (  SELECT qd.TENDER_NO,
              LISTAGG (UPPER(cb), ', ') WITHIN GROUP (ORDER BY UPPER(cb))
                  AS CHECKED_BY,
              LISTAGG (UPPER(vb), ', ') WITHIN GROUP (ORDER BY UPPER(vb))
                  AS VERIFIED_BY,
              LISTAGG (UPPER(ab), ', ') WITHIN GROUP (ORDER BY UPPER(ab))
                  AS APPROVED_BY,
              LISTAGG (UPPER(rb), ', ') WITHIN GROUP (ORDER BY UPPER(rb))
                  AS RECOMMENDED_BY
         FROM QUOTATION_DETAILS qd
              LEFT JOIN
              (SELECT DISTINCT
                      qdi.QUOTATION_NO, TRIM (qdi.CHECKED_BY) AS cb
                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                WHERE TRIM (qdi.CHECKED_BY) IS NOT NULL) cb_list
                  ON cb_list.QUOTATION_NO = qd.QUOTATION_NO
              LEFT JOIN
              (SELECT DISTINCT qdi.QUOTATION_NO, TRIM (qdi.VERIFY_BY) AS vb
                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                WHERE TRIM (qdi.VERIFY_BY) IS NOT NULL) vb_list
                  ON vb_list.QUOTATION_NO = qd.QUOTATION_NO
              LEFT JOIN
              (SELECT DISTINCT
                      qdi.QUOTATION_NO, TRIM (qdi.APPROVED_BY) AS ab
                 FROM QUOTATION_DETAIL_ITEMWISE qdi
                WHERE TRIM (qdi.APPROVED_BY) IS NOT NULL) ab_list
                  ON ab_list.QUOTATION_NO = qd.QUOTATION_NO
              LEFT JOIN
              (SELECT DISTINCT QUOTATION_NO, rb
                 FROM (SELECT QUOTATION_NO, TRIM (RECOMMENDED1_BY) AS rb
                         FROM QUOTATION_DETAIL_ITEMWISE
                       UNION
                       SELECT QUOTATION_NO, TRIM (RECOMMENDED2_BY)
                         FROM QUOTATION_DETAIL_ITEMWISE
                       UNION
                       SELECT QUOTATION_NO, TRIM (RECOMMENDED3_BY)
                         FROM QUOTATION_DETAIL_ITEMWISE
                       UNION
                       SELECT QUOTATION_NO, TRIM (RECOMMENDED4_BY)
                         FROM QUOTATION_DETAIL_ITEMWISE)
                WHERE rb IS NOT NULL) rb_list
                  ON rb_list.QUOTATION_NO = qd.QUOTATION_NO
     GROUP BY qd.TENDER_NO)
SELECT 
    SQS.ID,
    SQS.TENDER_NO,
    SQS.CREATED_DATE,
    SQS.VALID_DATE,
    IIMS.ITEM_EDESC AS ITEM_DESC,
    SQI.SPECIFICATION,
    SQI.QUANTITY,
    SQI.UNIT,
    COALESCE(SL.STATUS, 'Waiting') AS STATUS,
         PA.CHECKED_BY,
         PA.VERIFIED_BY,
         PA.RECOMMENDED_BY,
         PA.APPROVED_BY
FROM 
    SA_QUOTATION_SETUP SQS
JOIN SA_QUOTATION_ITEMS SQI ON SQS.TENDER_NO = SQI.TENDER_NO
JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = SQI.ITEM_CODE AND SQS.COMPANY_CODE = IIMS.COMPANY_CODE
LEFT JOIN StatusLabel SL ON SL.TENDER_NO = SQS.TENDER_NO
LEFT JOIN PersonAggregation PA ON PA.TENDER_NO = SQS.TENDER_NO
WHERE 
    SQS.STATUS = 'E' 
    AND SQI.DELETED_FLAG = 'N'
    AND SQS.COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' {dateFilter}
ORDER BY 
    SQS.ID DESC";
                List<SummaryReport> tenderItemwise = this._dbContext.SqlQuery<SummaryReport>(Query).ToList();
                foreach (var item in tenderItemwise)
                {
                    item.CHECKED_BY = MakeDistinct(item.CHECKED_BY);
                    item.VERIFIED_BY = MakeDistinct(item.VERIFIED_BY);
                    item.APPROVED_BY = MakeDistinct(item.APPROVED_BY);
                    item.RECOMMENDED_BY = MakeDistinct(item.RECOMMENDED_BY);
                }
                return tenderItemwise;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private string MakeDistinct(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var unique = input
                .Split(',')
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return string.Join(", ", unique);
        }
        public List<Quotation> ItemDetailsTenderNo(string tenderNo)
        {
            try
            {
                string query = $@"SELECT ID, TENDER_NO, ISSUE_DATE, VALID_DATE, CREATED_DATE, bs_date(ISSUE_DATE) as NEPALI_DATE,bs_date(VALID_DATE) as DELIVERY_DT_BS, COMPANY_CODE, LOCAL_FLAG FROM SA_QUOTATION_SETUP WHERE STATUS='E' AND ID='{tenderNo}'";
                List<Quotation> quotations = this._dbContext.SqlQuery<Quotation>(query).ToList();
                foreach (var quotation in quotations)
                {
                    string vquery = $@"SELECT distinct QD.QUOTATION_NO,scs.supplier_edesc as party_name,ROUND((QDI.TAXABLE_AMOUNT-QDI.EXCISE)/SQI.QUANTITY,2) AS ACTUAL_PRICE,QDI.ITEM_CODE,QDI.checked_by, qdi.recommended1_by, qdi.recommended2_by, qdi.recommended3_by, qdi.recommended4_by, qdi.verify_by, qdi.approved_by,TRIM(QD.STATUS) STATUS,QD.REVISE FROM QUOTATION_DETAILS QD,QUOTATION_DETAIL_ITEMWISE QDI,SA_QUOTATION_ITEMS SQI,ip_supplier_setup scs
                    WHERE QD.QUOTATION_NO=QDI.QUOTATION_NO AND SQI.TENDER_NO=QD.TENDER_NO AND QDI.ITEM_CODE=SQI.ITEM_CODE AND ((qd.supplier_code != 'null' AND scs.supplier_code = qd.supplier_code) OR (qd.supplier_code  = 'null' AND scs.tpin_vat_no   = qd.pan_no)) AND QD.TENDER_NO = '{quotation.TENDER_NO}' AND SCS.COMPANY_CODE='{_workContext.CurrentUserinformation.company_code}' AND SQI.QUOTATION_NO = '{quotation.ID}' AND SQI.deleted_flag = 'N' order by quotation_no";
                    List<PARTY_DETAIL> partyData = this._dbContext.SqlQuery<PARTY_DETAIL>(vquery).ToList();
                    quotation.PartDetails = partyData;
                    string vQuery = $@"SELECT SQI.*, 
                                   IIMS.ITEM_EDESC AS ITEM_DESC,IIR.CALC_UNIT_PRICE AS LAST_PRICE,(select supplier_edesc from ip_supplier_setup iss where iss.supplier_code = iir.supplier_code and iss.company_code = '{quotation.COMPANY_CODE}' and iss.DELETED_FLAG = 'N') AS LAST_VENDOR
                            FROM SA_QUOTATION_ITEMS SQI
                            LEFT JOIN IP_ITEM_MASTER_SETUP IIMS ON IIMS.ITEM_CODE = SQI.ITEM_CODE
                            LEFT JOIN (SELECT * FROM ( SELECT v.*, ROW_NUMBER() OVER (PARTITION BY ITEM_CODE ORDER BY invoice_DATE DESC) AS rn
                            FROM ip_purchase_invoice v ) WHERE rn = 1) IIR ON IIR.ITEM_CODE = SQI.ITEM_CODE
                            WHERE SQI.TENDER_NO = '{quotation.TENDER_NO}'
                            AND SQI.QUOTATION_NO = '{tenderNo}'
                            AND SQI.deleted_flag = 'N'
                            AND IIMS.COMPANY_CODE ='{quotation.COMPANY_CODE}' order by id";
                    List<Item_Detail> itemDetail = this._dbContext.SqlQuery<Item_Detail>(vQuery).ToList();
                    quotation.Items = itemDetail;
                }
                return quotations;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        internal class RollBackTender
        {
            public int? ID { get; set; }
            public string TENDER_NO { get; set; }
            public string SUPPLIER_CODE { get; set; }
        }
        private bool rollBackQuotation(string quotationNo, string status, string type, string items, string itemId, string Remarks)
        {
            try
            {
                string loginCode = _workContext.CurrentUserinformation.login_code;
                string companyCode = _workContext.CurrentUserinformation.company_code;
                string userTypeQ = $@"select user_type from sc_application_users where lower(login_code) = lower('{loginCode}') and group_sku_flag = 'I' and company_code = '{companyCode}'";
                string userType = _dbContext.SqlQuery<string>(userTypeQ).FirstOrDefault().ToString();
                if (userType != "ADMIN") return false;

                dynamic itemList = "";
                dynamic itemIdList = "";
                if (!string.IsNullOrEmpty(items))
                {
                    itemList = items.Split(',').Select(i => i.Trim()).ToList();
                }
                if (!string.IsNullOrEmpty(itemId))
                {
                    itemIdList = itemId.Split(',').Select(i => i.Trim()).ToList();
                }
                string itemName = "";
                RollBackTender tender = _dbContext.SqlQuery<RollBackTender>($@"SELECT TENDER_NO, SUPPLIER_CODE FROM QUOTATION_DETAILS WHERE QUOTATION_NO = '{quotationNo}'").FirstOrDefault();
                tender.ID = _dbContext.SqlQuery<int>($@"SELECT to_number(ID) as ID FROM SA_QUOTATION_SETUP WHERE TENDER_NO = '{tender.TENDER_NO}' and ROWNUM = 1").FirstOrDefault();
                foreach (var singleItem in itemIdList)
                {
                    string getRollCol = $@"
                        SELECT CASE
                                  WHEN APPROVED_BY IS NOT NULL THEN 'APPROVED'
                                  WHEN RECOMMENDED4_BY IS NOT NULL THEN 'RECOMMENDED4'
                                  WHEN RECOMMENDED3_BY IS NOT NULL THEN 'RECOMMENDED3'
                                  WHEN RECOMMENDED2_BY IS NOT NULL THEN 'RECOMMENDED2'
                                  WHEN RECOMMENDED1_BY IS NOT NULL THEN 'RECOMMENDED1'
                                  WHEN VERIFY_BY IS NOT NULL THEN 'VERIFY'
                                  WHEN CHECKED_BY IS NOT NULL THEN 'CHECKED'
                                  ELSE ''
                               END
                                  AS COLCHANGE
                          FROM QUOTATION_DETAIL_ITEMWISE
                         WHERE QUOTATION_NO = '{quotationNo}' AND ID = '{singleItem}'
                        ";
                    string getStatusQ = $@"
                        SELECT CASE
                                  WHEN APPROVED_BY IS NOT NULL THEN 'RE'
                                  WHEN RECOMMENDED4_BY IS NOT NULL OR RECOMMENDED3_BY IS NOT NULL OR RECOMMENDED2_BY IS NOT NULL THEN 'RE'
                                  WHEN RECOMMENDED1_BY IS NOT NULL THEN 'V'
                                  WHEN VERIFY_BY IS NOT NULL THEN 'C'
                                  WHEN CHECKED_BY IS NOT NULL THEN 'RQ'
                                  ELSE ''
                               END
                                  AS COLCHANGE
                          FROM QUOTATION_DETAIL_ITEMWISE
                         WHERE QUOTATION_NO = '{quotationNo}' AND ID = '{singleItem}'
                        ";
                    string rollbackColumn = _dbContext.SqlQuery<string>(getRollCol).FirstOrDefault();
                    string rollStatus = _dbContext.SqlQuery<string>(getStatusQ).FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(rollbackColumn))
                    {
                        continue;
                    }
                    else
                    {
                        int check = 0;
                        string checkQ = string.Empty;
                        string tempItem = _dbContext.SqlQuery<string>($@"SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE in (select item_code from QUOTATION_DETAIL_ITEMWISE where id = '{singleItem}') AND COMPANY_CODE = '{companyCode}' AND ROWNUM = 1").FirstOrDefault().ToString();
                        if (!string.IsNullOrEmpty(tempItem))
                        {
                            itemName = string.IsNullOrEmpty(itemName)
                                ? tempItem
                                : itemName + ", " + tempItem;
                        }
                        string rollBackCol = Regex.Replace(rollbackColumn, @"\d", "");
                        switch (rollbackColumn)
                        {
                            case "APPROVED":
                                checkQ = $@"UPDATE QUOTATION_DETAIL_ITEMWISE SET {rollbackColumn}_BY = NULL, {rollbackColumn}_DATE = NULL WHERE QUOTATION_NO = '{quotationNo}' AND ID = '{singleItem}'";
                                check = _dbContext.ExecuteSqlCommand(checkQ);
                                if(check > 0)
                                {
                                    _dbContext.ExecuteSqlCommand($@"UPDATE QUOTATION_DETAILS SET {rollBackCol}_BY = NULL, {rollBackCol}_DATE = NULL, STATUS = '{rollStatus}' WHERE QUOTATION_NO = '{quotationNo}'");
                                    _dbContext.ExecuteSqlCommand($@"DELETE FROM IP_QUOTATION_INQUIRY WHERE QUOTE_NO = '{tender.TENDER_NO}' AND SUPPLIER_CODE = '{tender.SUPPLIER_CODE}'");
                                }
                                break;
                            default:
                                checkQ = $@"UPDATE QUOTATION_DETAIL_ITEMWISE SET {rollbackColumn}_BY = NULL, {rollbackColumn}_DATE = NULL WHERE QUOTATION_NO = '{quotationNo}' AND ID = '{singleItem}'";
                                check = _dbContext.ExecuteSqlCommand(checkQ);
                                if (check > 0)
                                {
                                    _dbContext.ExecuteSqlCommand($@"UPDATE QUOTATION_DETAILS SET {rollBackCol}_BY = NULL, {rollBackCol}_DATE = NULL, STATUS = '{rollStatus}' WHERE QUOTATION_NO = '{quotationNo}'");
                                }
                                break;
                        }
                        
                        //_dbContext.ExecuteSqlCommand($@"INSERT INTO QUOTATION_BACK_LOG VALUES (QUOTATION_BACK_LOG_SEQ.NEXTVAL, '{tenderId}', '{quotationNo}', '{tender.TENDER_NO}', 'ROLLBACK', 'ROLLBACK', '{loginCode}', SYSDATE, '{rollbackColumn}', '{Remarks}')");
                    }
                }
                itemName = MakeDistinct(itemName);
                QuotationLogModel log = new QuotationLogModel
                {
                    QUOTATION_NO = Convert.ToInt32(quotationNo),
                    QUOTATION_ID = Convert.ToInt32(tender.ID),
                    TENDER_NO = tender.TENDER_NO,
                    TYPE = "ROLLBACK",
                    ACTION = "ROLLBACK",
                    ACTION_BY = _workContext.CurrentUserinformation.login_code,
                    CHANGED = tender.TENDER_NO,
                    REMARKS = string.IsNullOrWhiteSpace(Remarks) ? $"Rolled Back {tender.TENDER_NO} Approval procedure. This is an automated message, please contact {loginCode} for further details." : Remarks
                };
                InsertRemarks(log);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool acceptQuotation(string quotationNo, string status, string type, string items, string itemId, string Remarks)
        {
            try
            {
                if (status == "RR")
                {
                    return rollBackQuotation(quotationNo, status, type, items, itemId, Remarks);
                }
                bool insertedData = false;
                string loginCode = _workContext.CurrentUserinformation.login_code;
                string currentDate = DateTime.Now.ToString("dd-MMM-yyyy");
                string updateFields = "";
                dynamic itemList = "";
                if (!string.IsNullOrEmpty(items))
                {
                    itemList = items.Split(',').Select(i => i.Trim()).ToList();

                }

                string priceCheckQuery = $@"select TOTAL_NET_AMOUNT from quotation_details where quotation_no = '{quotationNo}' and ROWNUM = 1";
                var price = _dbContext.SqlQuery<decimal>(priceCheckQuery).FirstOrDefault();

                //if (price >= 30) { return false; }
                RollBackTender tender = _dbContext.SqlQuery<RollBackTender>($@"select a.TENDER_NO, to_number(b.ID) as ID from quotation_details a join sa_quotation_setup b on a.tender_no = b.tender_no where a.quotation_no = '{quotationNo}' and rownum = 1").FirstOrDefault();

                QuotationLogModel log = new QuotationLogModel
                {
                    QUOTATION_NO = Convert.ToInt32(quotationNo),
                    QUOTATION_ID = Convert.ToInt32(tender.ID),
                    TENDER_NO = tender.TENDER_NO,
                    TYPE = "REMARKS",
                    ACTION_BY = _workContext.CurrentUserinformation.login_code,
                    CHANGED = tender.TENDER_NO,
                    REMARKS = Remarks
                };
                switch (type)
                {
                    case "Checked":
                        updateFields = $@"checked_by = '{loginCode}', 
                          checked_date = SYSDATE";
                        log.ACTION = "CHECKED";
                        break;

                    case "Verify":
                        updateFields = $@"
            verify_by = '{loginCode}', 
            verify_date = SYSDATE,
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        log.ACTION = "VERIFY";
                        break;

                    case "Recommended":
                        updateFields = $@"
            recommended_by = '{loginCode}', 
            recommended_date = SYSDATE,
            verify_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            verify_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE),
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        log.ACTION = "RECOMMENDED";
                        break;

                    case "Approved":
                        updateFields = $@"
            approved_by = '{loginCode}', 
            approved_date = SYSDATE,
            recommended_by = COALESCE(NULLIF(recommended_by, ''), '{loginCode}'),
            recommended_date = COALESCE(NULLIF(recommended_date, NULL), SYSDATE),
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        log.ACTION = "APPROVED";
                        break;

                    case "Posted":
                        updateFields = $@"
            posted_by = '{loginCode}', 
            posted_date = SYSDATE,
            approved_by = COALESCE(NULLIF(approved_by, ''), '{loginCode}'),
            approved_date = COALESCE(NULLIF(approved_date, NULL), SYSDATE),
            recommended_by = COALESCE(NULLIF(recommended_by, ''), '{loginCode}'),
            recommended_date = COALESCE(NULLIF(recommended_date, NULL), SYSDATE),
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        break;
                }

                if(type == "Recommended")
                {
                    int checkRecommend = _dbContext.SqlQuery<int>($@"
                                                    SELECT DISTINCT
                                                             CASE
                                                                WHEN UPPER ('{loginCode}') IN
                                                                        (UPPER (RECOMMENDED1_BY),
                                                                         UPPER (RECOMMENDED2_BY),
                                                                         UPPER (RECOMMENDED3_BY),
                                                                         UPPER (RECOMMENDED4_BY))
                                                                THEN
                                                                   1
                                                                ELSE
                                                                   0
                                                             END
                                                                AS is_allowed
                                                        FROM quotation_detail_itemwise
                                                       WHERE quotation_no = '{quotationNo}' AND id IN ({itemId})
                                                    ORDER BY is_allowed DESC
                                            ").FirstOrDefault();
                    if (checkRecommend == 1) return true; // For when a user has already recommended but is again sending request, to block them from updating further [For future reference]
                }
                // Final update query
                var UPDATE_QUERY = $@"
    UPDATE quotation_details 
    SET status = '{status}', 
        {updateFields}
    WHERE QUOTATION_NO = '{quotationNo}'";
                //var UPDATE_QUERY = $@"UPDATE quotation_details SET status ='{status}',approved_by='{_workContext.CurrentUserinformation.login_code}',approved_date='{DateTime.Now.ToString("dd - MMM - yyyy")}' WHERE QUOTATION_NO='{quotationNo}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                updateFields = string.Empty;
                switch (type)
                {
                    case "Checked":
                        updateFields = $@"checked_by = '{loginCode}', 
                          checked_date = SYSDATE";
                        break;

                    case "Verify":
                        updateFields = $@"
            verify_by = '{loginCode}', 
            verify_date = SYSDATE,
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        break;

                    case "Recommended":
                        updateFields = $@"
    recommended1_by = CASE 
        WHEN recommended1_by IS NULL THEN '{loginCode}' 
        ELSE recommended1_by 
    END,
    recommended1_date = CASE 
        WHEN recommended1_by IS NULL THEN SYSDATE 
        ELSE recommended1_date 
    END,

    recommended2_by = CASE 
        WHEN recommended1_by IS NOT NULL AND recommended2_by IS NULL THEN '{loginCode}' 
        ELSE recommended2_by 
    END,
    recommended2_date = CASE 
        WHEN recommended1_by IS NOT NULL AND recommended2_by IS NULL THEN SYSDATE 
        ELSE recommended2_date 
    END,

    recommended3_by = CASE 
        WHEN recommended1_by IS NOT NULL AND recommended2_by IS NOT NULL AND recommended3_by IS NULL THEN '{loginCode}' 
        ELSE recommended3_by 
    END,
    recommended3_date = CASE 
        WHEN recommended1_by IS NOT NULL AND recommended2_by IS NOT NULL AND recommended3_by IS NULL THEN SYSDATE 
        ELSE recommended3_date 
    END,

    recommended4_by = CASE 
        WHEN recommended1_by IS NOT NULL AND recommended2_by IS NOT NULL AND recommended3_by IS NOT NULL AND recommended4_by IS NULL THEN '{loginCode}' 
        ELSE recommended4_by 
    END,
    recommended4_date = CASE 
        WHEN recommended1_by IS NOT NULL AND recommended2_by IS NOT NULL AND recommended3_by IS NOT NULL AND recommended4_by IS NULL THEN SYSDATE 
        ELSE recommended4_date 
    END,

    verify_by = COALESCE(verify_by, '{loginCode}'),
    verify_date = COALESCE(verify_date, SYSDATE),
    checked_by = COALESCE(checked_by, '{loginCode}'),
    checked_date = COALESCE(checked_date, SYSDATE)";
                        break;

                    case "Approved":
                        updateFields = $@"
            approved_by = '{loginCode}', 
            approved_date = SYSDATE,
            recommended1_by = COALESCE(NULLIF(recommended1_by, ''), '{loginCode}'),
            recommended1_date = COALESCE(NULLIF(recommended1_date, NULL), SYSDATE),
            VERIFY_BY = COALESCE(NULLIF(VERIFY_BY, ''), '{loginCode}'),
            VERIFY_date = COALESCE(NULLIF(verify_date, NULL), SYSDATE),
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        break;

                    case "Posted":
                        updateFields = $@"
            posted_by = '{loginCode}', 
            posted_date = SYSDATE,
            approved_by = COALESCE(NULLIF(approved_by, ''), '{loginCode}'),
            approved_date = COALESCE(NULLIF(approved_date, NULL), SYSDATE),
            recommended_by = COALESCE(NULLIF(recommended1_by, ''), '{loginCode}'),
            recommended_date = COALESCE(NULLIF(recommended1_date, NULL), SYSDATE),
            checked_by = COALESCE(NULLIF(checked_by, ''), '{loginCode}'),
            checked_date = COALESCE(NULLIF(checked_date, NULL), SYSDATE)";
                        break;
                }

                // Final update query
                var UPDATE_ITEM_QUERY = $@"
    UPDATE quotation_detail_itemwise 
    SET 
        {updateFields}
    WHERE QUOTATION_NO = '{quotationNo}' 
--and item_code in ({items}) 
and ID in ({itemId})";

                _dbContext.ExecuteSqlCommand(UPDATE_ITEM_QUERY);

                string fQuery = $@"select FORM_CODE from form_setup where quotation_flag='Y' and  company_code= '{_workContext.CurrentUserinformation.company_code}'";
                string formCode = _dbContext.SqlQuery<string>(fQuery).FirstOrDefault();

                string vquery = $@"select qs.quotation_no,qs.tender_no,qs.created_date,qs.supplier_code,scs.regd_office_eaddress as address,scs.tel_mobile_no1 as contact_no ,iims.item_code,sqi.specification,
                iims.index_mu_code,sqi.quantity,qdi.rate,qdi.amount as total_net_amount,sqs.company_code,sqs.branch_code,qs.currency,qs.currency_rate,qs.delivery_date
                from quotation_details qs ,ip_supplier_setup scs,sa_quotation_items sqi,ip_item_master_setup iims ,sa_quotation_setup sqs,quotation_detail_itemwise qdi
                where scs.supplier_code=qs.supplier_code and iims.item_code=sqi.item_code and qs.tender_no=sqi.tender_no and sqs.tender_no=qs.tender_no and 
                sqs.company_code=iims.company_code and sqs.company_code=scs.company_code and qdi.quotation_no=qs.quotation_no and  sqi.item_code=qdi.item_code
                and  qs.quotation_no='{quotationNo}' and sqi.deleted_flag='N' and sqs.status='E' AND QDI.ID in ({itemId})";
                List<QuotationDetails> quoteData = _dbContext.SqlQuery<QuotationDetails>(vquery).ToList();
                quoteData.ForEach(q => q.Form_Code = formCode);

                insertedData = InsertQuotesData(quoteData, type);
                if (insertedData)
                {
                    string query = $@"SELECT qs.tender_no,  qs.created_date,  qs.supplier_code,  qs.total_net_amount, sqs.company_code,
                      sqs.branch_code,  qs.currency, qs.currency_rate,  qs.delivery_date FROM  quotation_details qs,
                       sa_quotation_setup sqs WHERE qs.tender_no =sqs.tender_no  AND qs.quotation_no = '{quotationNo}' AND sqs.status = 'E'";
                    List<QuotationDetails> masterData = _dbContext.SqlQuery<QuotationDetails>(query).ToList();
                    masterData.ForEach(q => q.Form_Code = formCode);
                    string que = $@"select SUM(qdi.NET_AMOUNT) as Total_Net_Amount
from quotation_detail_itemwise qdi join quotation_details qd on qdi.quotation_no = qd.quotation_no 
join quotation_details qd2 on qd.tender_no = qd2.tender_no 
where qd2.quotation_no = '{quotationNo}' and qdi.approved_by is not null";
                    var totalNetAmount = _dbContext.SqlQuery<decimal?>(que).FirstOrDefault() ?? 0;

                    masterData.ForEach(m => m.Total_Net_Amount = totalNetAmount);
                    SaveMasterColumnValue(masterData, type);
                }
                
                InsertRemarks(log);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool InsertQuotesData(List<QuotationDetails> quoteData, string type)
        {
            try
            {
                if(type == "Approved" || type == "Posted")
                {
                    foreach (var data in quoteData)
                    {
                        var idquery = $@"SELECT COALESCE(MAX(SERIAL_NO) + 1, 1) AS id FROM ip_quotation_inquiry where QUOTE_NO='{data.Tender_No}'";
                        int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();
                        //var date=data.Created_Date-data.Delivery_Date;
                        TimeSpan dateDifference = data.Delivery_Date.Date - data.Created_Date.Date;
                        int deliveryDays = (int)dateDifference.TotalDays;
                        string sQuery = $@"SELECT MYSEQUENCE.NEXTVAL FROM DUAL";
                        var sessionRowId = _dbContext.SqlQuery<int>(sQuery).FirstOrDefault();


                        string insertItemQuery = $@"
                            INSERT INTO ip_quotation_inquiry (
                            QUOTE_NO, QUOTE_DATE, ORDER_NO, REQUEST_NO, MANUAL_NO, SUPPLIER_CODE, ADDRESS, CONTACT_PERSON, PHONE_NO, SERIAL_NO, ITEM_CODE,
                            SPECIFICATION, MU_CODE, QUANTITY, UNIT_PRICE, TOTAL_PRICE, REMARKS, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, CURRENCY_CODE, EXCHANGE_RATE,
                            BRAND_NAME, DELIVERY_DATE, APPROVED_FLAG, APPROVED_BY, APPROVED_DATE,SESSION_ROWID,DELETED_FLAG,DELIVERY_DAYS
                            ) VALUES (
                            '{data.Tender_No}', TO_DATE('{data.Created_Date.ToString("dd-MMM-yyyy")}', 'DD-MON-YYYY'), null,null, '{id}', '{data.SUPPLIER_Code}',
                            '{data.Address}', NULL, '{data.Contact_No}', {id}, '{data.Item_Code}', '{data.Specification}', '{data.Index_Mu_Code}', {data.Quantity}, '{data.Rate}', '{data.Total_Net_Amount}', null, '{data.Form_Code}',
                            '{data.Company_Code}', '{data.Branch_Code}', '{_workContext.CurrentUserinformation.login_code}', '{DateTime.Now.ToString("dd-MMM-yyyy")}','{data.Currency}', '{data.Currency_Rate}', '{data.Brand_Name}', TO_DATE('{data.Delivery_Date.ToString("dd-MMM-yyyy")}', 'DD-MON-YYYY'), 'Y', '{_workContext.CurrentUserinformation.login_code}', '{DateTime.Now.ToString("dd-MMM-yyyy")}'
                            ,'{sessionRowId}','N',{deliveryDays} )";
                        var row = _dbContext.ExecuteSqlCommand(insertItemQuery);
                        if (row > 0)
                            return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }
        public bool SaveMasterColumnValue(List<QuotationDetails> quodata, string type)
        {
            try
            {
                var companyCode = _workContext.CurrentUserinformation.company_code;
                var branchCode = _workContext.CurrentUserinformation.branch_code;
                var loginCode = _workContext.CurrentUserinformation.login_code.ToUpper();
                var currentDate = DateTime.Now.ToString("dd-MMM-yyyy");

                /*var checked_by = "";
                var approved_by = "";
                var recommended_by = "";
                var posted_by = "";
                var checked_date = "";
                var approved_date = "";
                var recommended_date = "";
                var posted_date = "";*/

                foreach (var data in quodata)
                {
                    string sQuery = $@"SELECT MYSEQUENCE.NEXTVAL FROM DUAL";
                    var sessionRowId = _dbContext.SqlQuery<int>(sQuery).FirstOrDefault();
                    //List<Quotation_Details> check = new List<Quotation_Details>();
                    string checkQuery = $@"select voucher_no as tender_no, checked_by, authorised_by as APPROVED_BY, posted_by, recommended_by from master_transaction where voucher_no = '{data.Tender_No}' and company_code = '{_workContext.CurrentUserinformation.company_code}' and branch_code = '{_workContext.CurrentUserinformation.branch_code}' and deleted_flag = 'N'";
                    var check = _dbContext.SqlQuery<Quotation_Details>(checkQuery).FirstOrDefault();
                    if (check == null)
                    {
                        string checkedBy = "";
                        string recommendedBy = "";
                        string approvedBy = "";
                        string postedBy = "";
                        var checked_date = "''";
                        var approved_date = "''";
                        var recommended_date = "''";
                        var posted_date = "''";
                        var posted_flag = "";

                        // Populate based on type
                        if (type == "Checked")
                        {
                            checkedBy = loginCode;
                            checked_date = "SYSDATE";
                        }
                        else if (type == "Recommended")
                        {
                            checkedBy = loginCode;
                            checked_date = "SYSDATE";
                            recommendedBy = loginCode;
                            recommended_date = "SYSDATE";
                        }
                        else if (type == "Approved")
                        {
                            checkedBy = loginCode;
                            checked_date = "SYSDATE";
                            recommendedBy = loginCode;
                            recommended_date = "SYSDATE";
                            approvedBy = loginCode;
                            approved_date = "SYSDATE";
                            // Just for the posted to be present in the master column
                            postedBy = loginCode;
                            posted_date = "SYSDATE";
                            posted_flag = "Y";
                        }
                        else if (type == "Posted")
                        {
                            checkedBy = loginCode;
                            checked_date = "SYSDATE";
                            recommendedBy = loginCode;
                            recommended_date = "SYSDATE";
                            approvedBy = loginCode;
                            approved_date = "SYSDATE";
                            postedBy = loginCode;
                            posted_date = "SYSDATE";
                            posted_flag = "Y";
                        }

                        string insertQuery = $@"
                            INSERT INTO MASTER_TRANSACTION(
                                VOUCHER_NO, VOUCHER_AMOUNT, FORM_CODE, COMPANY_CODE, 
                                BRANCH_CODE, CREATED_BY, DELETED_FLAG, CURRENCY_CODE, 
                                CREATED_DATE, VOUCHER_DATE, SESSION_ROWID, SYN_ROWID, 
                                EXCHANGE_RATE, IS_SYNC_WITH_IRD, IS_REAL_TIME,
                                CHECKED_BY, RECOMMENDED_BY, AUTHORISED_BY, POSTED_BY,
                                CHECKED_DATE, RECOMMENDED_DATE, AUTHORISED_DATE, POSTED_DATE, POSTED_FLAG
                            ) VALUES (
                                '{data.Tender_No}', '{data.Total_Net_Amount}', '{data.Form_Code}', '{companyCode}', 
                                '{branchCode}', '{loginCode}', 'N', '{data.Currency}', 
                                '{currentDate}', '{currentDate}', '{sessionRowId}', '1', 
                                '{data.Currency_Rate}', 'N', 'N',
                                '{checkedBy}', '{recommendedBy}', '{approvedBy}', '{postedBy}',
                                {checked_date}, {recommended_date}, {approved_date}, {posted_date}, '{posted_flag}'
                            )";
                        _dbContext.ExecuteSqlCommand(insertQuery);
                    }
                    else
                    {
                        var updates = new List<string>();

                        if (type == "Checked")
                        {
                            updates.Add($"checked_by = '{loginCode}', checked_date = SYSDATE");
                        }
                        else if (type == "Recommended")
                        {
                            if (string.IsNullOrWhiteSpace(check.CHECKED_BY))
                                updates.Add($"checked_by = '{loginCode}', checked_date = SYSDATE");

                            updates.Add($"recommended_by = '{loginCode}', recommended_date = SYSDATE");
                        }
                        else if (type == "Approved")
                        {
                            if (string.IsNullOrWhiteSpace(check.CHECKED_BY))
                                updates.Add($"checked_by = '{loginCode}', checked_date = SYSDATE");

                            if (string.IsNullOrWhiteSpace(check.RECOMMEND_BY))
                                updates.Add($"recommended_by = '{loginCode}', recommended_date = SYSDATE");

                            updates.Add($"authorised_by = '{loginCode}', authorised_date = SYSDATE");
                            // Extra psted for the master column
                            updates.Add($"posted_by = '{loginCode}', POSTED_date = sysdate, posted_flag = 'Y'");
                        }
                        else if (type == "Posted")
                        {
                            if (string.IsNullOrWhiteSpace(check.CHECKED_BY))
                                updates.Add($"checked_by = '{loginCode}', checked_date = SYSDATE");

                            if (string.IsNullOrWhiteSpace(check.RECOMMEND_BY))
                                updates.Add($"recommended_by = '{loginCode}', recommended_date = SYSDATE");

                            if (string.IsNullOrWhiteSpace(check.APPROVED_BY))
                                updates.Add($"authorised_by = '{loginCode}', authorised_date = sysdate");

                            updates.Add($"posted_by = '{loginCode}', POSTED_date = sysdate, posted_flag = 'Y'");
                        }

                        if (updates.Any())
                        {
                            string updateQuery = $@"
                UPDATE master_transaction 
                SET {string.Join(", ", updates)} 
                WHERE voucher_no = '{data.Tender_No}' 
                      AND company_code = '{companyCode}' 
                      AND branch_code = '{branchCode}'";

                            _dbContext.ExecuteSqlCommand(updateQuery);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.StackTrace);
                throw new Exception(ex.Message);
            }
        }
        public bool rejectQuotation(string quotationNo, string status, string Remarks)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE quotation_details SET status ='{status}',rejected_by='{_workContext.CurrentUserinformation.login_code}',rejected_date='{DateTime.Now.ToString("dd - MMM - yyyy")}' WHERE QUOTATION_NO='{quotationNo}'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                RollBackTender tender = _dbContext.SqlQuery<RollBackTender>($@"select a.TENDER_NO, to_number(b.ID) as ID from quotation_details a join sa_quotation_setup b on a.tender_no = b.tender_no where a.quotation_no = '{quotationNo}' and rownum = 1").FirstOrDefault();
                QuotationLogModel log = new QuotationLogModel{
                    QUOTATION_NO = Convert.ToInt32(quotationNo),
                    QUOTATION_ID = Convert.ToInt32(tender.ID),
                    TENDER_NO = tender.TENDER_NO,
                    TYPE = "REMARKS",
                    ACTION = "REJECTED",
                    ACTION_BY = _workContext.CurrentUserinformation.login_code,
                    CHANGED = tender.TENDER_NO,
                    REMARKS = Remarks
                };
                InsertRemarks(log);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool InsertTenderData(Tender data)
        {
            try
            {
                string tenderId = data.ID;
                int prefixLength = string.IsNullOrEmpty(data.PREFIX) ? 0 : data.PREFIX.Length;
                int suffixLength = string.IsNullOrEmpty(data.SUFFIX) ? 0 : data.SUFFIX.Length;

                if (tenderId == null || tenderId == "")
                {
                    var checkQuery = $@"
                        SELECT FORM_CODE
                        FROM FORM_SETUP
                        WHERE QUOTATION_FLAG = 'Y'
                        AND CUSTOM_PREFIX_TEXT = '{data.PREFIX}'
                        AND CUSTOM_SUFFIX_TEXT = '{data.SUFFIX}'
                        AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                    string existingFormCode = _dbContext.SqlQuery<string>(checkQuery).FirstOrDefault();

                    if (!string.IsNullOrEmpty(existingFormCode))
                    {
                        int prefLength = string.IsNullOrEmpty(data.PREFIX) ? 0 : data.PREFIX.Length;
                        int sufLength = string.IsNullOrEmpty(data.SUFFIX) ? 0 : data.SUFFIX.Length;
                        string updateQuery = $@"
                            UPDATE FORM_SETUP
                            SET 
                                CUSTOM_PREFIX_TEXT = '{data.PREFIX}',
                                CUSTOM_SUFFIX_TEXT = '{data.SUFFIX}',
                                BODY_LENGTH = {data.BODY_LENGTH},
                                PREFIX_LENGTH = '{prefLength}',
                                SUFFIX_LENGTH = '{sufLength}',
                                DELETED_FLAG = 'N',
                                MODIFY_DATE = TO_DATE('{DateTime.Now:dd-MMM-yyyy}', 'DD-MON-YYYY'),
                                MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                            WHERE FORM_CODE = '{existingFormCode}' 
                            AND QUOTATION_FLAG = 'Y' 
                            AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                        _dbContext.ExecuteSqlCommand(updateQuery);
                        return true;
                    }
                    var idquery = $@"select max(to_number(form_code) +1) as form_code from form_setup";
                    int id = _dbContext.SqlQuery<int>(idquery).FirstOrDefault();
                    string insertQuery = string.Format(@"
                        INSERT INTO FORM_SETUP (
                            FORM_CODE, FORM_EDESC, FORM_NDESC, MASTER_FORM_CODE, PRE_FORM_CODE,
                            MODULE_CODE, GROUP_SKU_FLAG, NUMBERING_FORMAT, DATE_FORMAT,
                            START_ID_FLAG, ID_GENERATION_FLAG, CUSTOM_PREFIX_TEXT, CUSTOM_SUFFIX_TEXT,
                            PREFIX_LENGTH, SUFFIX_LENGTH, BODY_LENGTH, START_NO, LAST_NO,
                            START_DATE, LAST_DATE, REF_COLUMN_NAME, PRINT_REPORT_FLAG,
                            PRIMARY_MANUAL_FLAG, COPY_VALUES_FLAG, QUALITY_CHECK_FLAG,
                            SERIAL_TRACKING_FLAG, BATCH_TRACKING_FLAG, COMPANY_CODE, CREATED_BY,
                            CREATED_DATE, DELETED_FLAG, FORM_TYPE, QUOTATION_FLAG
                        ) VALUES (
                            '{0}', '{1}', '{2}', '05.06.00', '05.06',
                            '02', 'I', '123,456,789.00', 'dd-MMM-yyyy',
                            'M', 'C', '{3}', '{4}', 
                        '{5}', '{6}', '{7}', 1, 99999,
                            TO_DATE('{8}', 'YYYY-MM-DD'), TO_DATE('{8}', 'YYYY-MM-DD'), 'REQUEST_NO', 'N',
                            'N', 'N', 'N',
                            'N', 'N', '{9}', '{10}',
                            TO_DATE('{8} 21:52:43', 'YYYY-MM-DD HH24:MI:SS'), 'N', 'OT', 'Y'
                        )",
                                              id,data.FORM_EDESC, data.FORM_NDESC, data.PREFIX, data.SUFFIX,
                                              prefixLength, suffixLength, data.BODY_LENGTH,
                                              DateTime.Now.ToString("yyyy-MM-dd"),
                                              _workContext.CurrentUserinformation.company_code,
                                              _workContext.CurrentUserinformation.login_code
                                              );
                    _dbContext.ExecuteSqlCommand(insertQuery);
                    return true;
                }
                else
                {
                    int prefLength = string.IsNullOrEmpty(data.PREFIX) ? 0 : data.PREFIX.Length;
                    int sufLength = string.IsNullOrEmpty(data.SUFFIX) ? 0 : data.SUFFIX.Length;
                    string updateQuery = $@"
                            UPDATE FORM_SETUP
                                SET 
                                    CUSTOM_PREFIX_TEXT = '{data.PREFIX}',
                                    CUSTOM_SUFFIX_TEXT = '{data.SUFFIX}',
                                    BODY_LENGTH = '{data.BODY_LENGTH}',
                                    PREFIX_LENGTH = '{prefLength}',
                                    SUFFIX_LENGTH = '{sufLength}',
                                    DELETED_FLAG = 'N',
                                    MODIFY_DATE = TO_DATE('{DateTime.Now:dd-MMM-yyyy}', 'DD-MON-YYYY'),
                                    MODIFY_BY = '{_workContext.CurrentUserinformation.login_code}'
                                WHERE FORM_CODE = '{data.ID}' 
                                AND QUOTATION_FLAG = 'Y' 
                                AND FORM_EDESC = '{data.FORM_EDESC}'
                                AND FORM_NDESC = '{data.FORM_NDESC}'
                                AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}'";

                    _dbContext.ExecuteSqlCommand(updateQuery);
                    return true;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Tender> getTenderDetails()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                string query = $@"select 
                    FORM_CODE as ID, FORM_EDESC, FORM_NDESC, CUSTOM_PREFIX_TEXT as PREFIX, CUSTOM_SUFFIX_TEXT as SUFFIX, BODY_LENGTH, DELETED_FLAG as STATUS, CREATED_DATE, CREATED_BY, COMPANY_CODE
                    from form_setup 
                    where company_code = '{company_code}' AND DELETED_FLAG = 'N' AND quotation_flag = 'Y' AND GROUP_SKU_FLAG = 'I'";

                List<Tender> tenders = this._dbContext.SqlQuery<Tender>(query).ToList();
                return tenders;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool deleteTenderId(string id)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var UPDATE_QUERY = $@"UPDATE FORM_SETUP SET DELETED_FLAG = 'Y' WHERE FORM_CODE ='{id}' AND COMPANY_CODE = '{company_code}' AND QUOTATION_FLAG = 'Y' AND GROUP_SKU_FLAG = 'I'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Tender> getTenderById( string id)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                string query = $@"SELECT FORM_CODE as ID, FORM_EDESC, FORM_NDESC, CUSTOM_PREFIX_TEXT as PREFIX, CUSTOM_SUFFIX_TEXT as SUFFIX, BODY_LENGTH, DELETED_FLAG as STATUS, CREATED_DATE, CREATED_BY, COMPANY_CODE
                    from form_setup WHERE DELETED_FLAG = 'N' AND QUOTATION_FLAG = 'Y' AND FORM_CODE='{id}' AND COMPANY_CODE = '{company_code}'";
                List<Tender> tenders = this._dbContext.SqlQuery<Tender>(query).ToList();
                return tenders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<QuotationCount> GetQuotationCount()
        {
            try
            {
                string query = $@"SELECT COUNT(*) AS count, 'Quotation Request' AS heading, '#bbbbf0' AS color, 'fa fa-star fa-2x' AS icon, 1 AS sortOrder
                                FROM sa_quotation_setup
                                WHERE status = 'E'
                                UNION 
                                SELECT COUNT(*) AS count, 'Quotation Received ' AS heading, '#f0bbee' AS color, 'fa fa-asterisk' AS icon, 2 AS sortOrder
                                FROM quotation_details sps 
                                LEFT JOIN sa_quotation_setup ps ON (ps.tender_no = sps.tender_no)
                                WHERE  ps.status = 'E' 
                                UNION 
                                SELECT COUNT(DISTINCT sps.quotation_no) AS count, 'Quotation Approved ' AS heading, '#f0d19c' AS color, 'fa fa-spinner' AS icon, 3 AS sortOrder
                                 FROM quotation_details sps 
                                LEFT JOIN sa_quotation_setup ps ON (ps.tender_no = sps.tender_no)
                                WHERE  ps.status = 'E' and sps.status='AP'
                                UNION 
                                SELECT ((SELECT COUNT(*) FROM sa_quotation_setup WHERE status = 'E') - (SELECT COUNT(DISTINCT sps.tender_no) 
                                 FROM quotation_details sps WHERE sps.status = 'AP' AND sps.tender_no IN (SELECT tender_no FROM sa_quotation_setup WHERE status = 'E'))) AS count, 
                                'Quotation Open' AS heading,  '#d4fa93' AS color,'fa fa-opera' AS icon, 4 AS sortOrder from dual ORDER BY sortOrder";

                List<QuotationCount> entity = this._dbContext.SqlQuery<QuotationCount>(query).ToList();
                return entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<QuotationNotification> GetAllNotification()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                string form = $@"select 
                    FORM_CODE as ID, CUSTOM_PREFIX_TEXT as PREFIX, CUSTOM_SUFFIX_TEXT as SUFFIX, BODY_LENGTH, DELETED_FLAG as STATUS, CREATED_DATE, CREATED_BY, COMPANY_CODE
                    from form_setup 
                    where company_code = '{company_code}' AND DELETED_FLAG = 'N' AND quotation_flag = 'Y'";
                var form_data = _dbContext.SqlQuery<Tender>(form).ToList();
                var form_code = form_data.FirstOrDefault()?.ID;
                string query = $@"
                    WITH notifications AS (
                      SELECT 
                        'IP_QUOTATION_INQUIRY' AS table_name,
                        QUOTE_NO AS TENDER_NO,
                        'Created' AS action,
                        CREATED_BY AS performed_by,
                        CREATED_DATE AS NOTIF_DATE,
                        'Quote ' || QUOTE_NO || ' created' AS MESSAGE
                      FROM IP_QUOTATION_INQUIRY
                      WHERE DELETED_FLAG IS NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'IP_QUOTATION_INQUIRY' AS table_name,
                        QUOTE_NO AS TENDER_NO,
                        'Modified' AS action,
                        MODIFY_BY AS performed_by,
                        MODIFY_DATE AS NOTIF_DATE,
                        'Quote ' || QUOTE_NO || ' modified' AS MESSAGE
                      FROM IP_QUOTATION_INQUIRY
                      WHERE MODIFY_DATE IS NOT NULL 
                        AND DELETED_FLAG IS NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'IP_QUOTATION_INQUIRY' AS table_name,
                        QUOTE_NO AS TENDER_NO,
                        'Deleted' AS action,
                        MODIFY_BY AS performed_by,
                        MODIFY_DATE AS NOTIF_DATE,
                        'Quote ' || QUOTE_NO || ' deleted' AS MESSAGE
                      FROM IP_QUOTATION_INQUIRY
                      WHERE DELETED_FLAG = 'Y'
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'IP_QUOTATION_INQUIRY' AS table_name,
                        QUOTE_NO AS TENDER_NO,
                        'Approved' AS action,
                        APPROVED_BY AS performed_by,
                        APPROVED_DATE AS NOTIF_DATE,
                        'Quote ' || QUOTE_NO || ' approved' AS MESSAGE
                      FROM IP_QUOTATION_INQUIRY
                      WHERE APPROVED_FLAG = 'Y'
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'QUOTATION_DETAILS' AS table_name,
                        TO_CHAR(QUOTATION_NO) AS TENDER_NO,
                        'Created' AS action,
                        NULL AS performed_by,
                        CREATED_DATE AS NOTIF_DATE,
                        'Quotation ID ' || TO_CHAR(QUOTATION_NO) || ' created' AS MESSAGE
                      FROM QUOTATION_DETAILS
                      WHERE STATUS NOT IN ('RJ', 'CN')
                      UNION ALL
                      SELECT 
                        'QUOTATION_DETAILS' AS table_name,
                        TO_CHAR(QUOTATION_NO) AS TENDER_NO,
                        'Approved' AS action,
                        APPROVED_BY AS performed_by,
                        APPROVED_DATE AS NOTIF_DATE,
                        'Quotation ID ' || TO_CHAR(QUOTATION_NO) || ' approved' AS MESSAGE
                      FROM QUOTATION_DETAILS
                      WHERE APPROVED_DATE IS NOT NULL
                      UNION ALL
                      SELECT 
                        'QUOTATION_DETAILS' AS table_name,
                        TO_CHAR(QUOTATION_NO) AS TENDER_NO,
                        'Rejected' AS action,
                        REJECTED_BY AS performed_by,
                        REJECTED_DATE AS NOTIF_DATE,
                        'Quotation ID ' || TO_CHAR(QUOTATION_NO) || ' rejected' AS MESSAGE
                      FROM QUOTATION_DETAILS
                      WHERE REJECTED_DATE IS NOT NULL
                      UNION ALL
                      SELECT 
                        'SA_QUOTATION_SETUP' AS table_name,
                        TENDER_NO AS TENDER_NO,
                        'Created' AS action,
                        CREATED_BY AS performed_by,
                        CREATED_DATE AS NOTIF_DATE,
                        'Tender ' || TENDER_NO || ' created' AS MESSAGE
                      FROM SA_QUOTATION_SETUP
                      WHERE STATUS = 'A'
                        AND COMPANY_CODE = '{company_code}'
                      UNION ALL
                      SELECT 
                        'SA_QUOTATION_SETUP' AS table_name,
                        TENDER_NO AS TENDER_NO,
                        'Modified' AS action,
                        MODIFIED_BY AS performed_by,
                        MODIFIED_DATE AS NOTIF_DATE,
                        'Tender ' || TENDER_NO || ' modified' AS MESSAGE
                      FROM SA_QUOTATION_SETUP
                      WHERE MODIFIED_DATE IS NOT NULL
                        AND COMPANY_CODE = '{company_code}'
                      UNION ALL
                      SELECT 
                        'SA_QUOTATION_SETUP' AS table_name,
                        TENDER_NO AS TENDER_NO,
                        'Approved' AS action,
                        NULL AS performed_by,
                        MODIFIED_DATE AS NOTIF_DATE,
                        'Tender ' || TENDER_NO || ' approved' AS MESSAGE
                      FROM SA_QUOTATION_SETUP
                      WHERE APPROVED_STATUS = 'APPROVED'
                        AND COMPANY_CODE = '{company_code}'
                      UNION ALL
                      SELECT 
                        'FORM_SETUP' AS table_name,
                        FORM_CODE AS TENDER_NO,
                        'Created' AS action,
                        CREATED_BY AS performed_by,
                        CREATED_DATE AS NOTIF_DATE,
                        'Form ' || FORM_CODE || ' created' AS MESSAGE
                      FROM FORM_SETUP
                      WHERE DELETED_FLAG IS NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'FORM_SETUP' AS table_name,
                        FORM_CODE AS TENDER_NO,
                        'Modified' AS action,
                        MODIFY_BY AS performed_by,
                        MODIFY_DATE AS NOTIF_DATE,
                        'Form ' || FORM_CODE || ' modified' AS MESSAGE
                      FROM FORM_SETUP
                      WHERE MODIFY_DATE IS NOT NULL 
                        AND DELETED_FLAG IS NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'FORM_SETUP' AS table_name,
                        FORM_CODE AS TENDER_NO,
                        'Deleted' AS action,
                        MODIFY_BY AS performed_by,
                        MODIFY_DATE AS NOTIF_DATE,
                        'Form ' || FORM_CODE || ' deleted' AS MESSAGE
                      FROM FORM_SETUP
                      WHERE DELETED_FLAG = 'Y'
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'MASTER_TRANSACTION' AS table_name,
                        VOUCHER_NO AS TENDER_NO,
                        'Created' AS action,
                        CREATED_BY AS performed_by,
                        CREATED_DATE AS NOTIF_DATE,
                        'Voucher ' || VOUCHER_NO || ' created' AS MESSAGE
                      FROM MASTER_TRANSACTION
                      WHERE DELETED_FLAG IS NULL 
                        AND CANCEL_FLAG IS NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'MASTER_TRANSACTION' AS table_name,
                        VOUCHER_NO AS TENDER_NO,
                        'Modified' AS action,
                        MODIFY_BY AS performed_by,
                        MODIFY_DATE AS NOTIF_DATE,
                        'Voucher ' || VOUCHER_NO || ' modified' AS MESSAGE
                      FROM MASTER_TRANSACTION
                      WHERE MODIFY_DATE IS NOT NULL 
                        AND DELETED_FLAG IS NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'MASTER_TRANSACTION' AS table_name,
                        VOUCHER_NO AS TENDER_NO,
                        'Deleted' AS action,
                        DELETED_BY AS performed_by,
                        DELETED_DATE AS NOTIF_DATE,
                        'Voucher ' || VOUCHER_NO || ' deleted' AS MESSAGE
                      FROM MASTER_TRANSACTION
                      WHERE DELETED_FLAG = 'Y'
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'MASTER_TRANSACTION' AS table_name,
                        VOUCHER_NO AS TENDER_NO,
                        'Approved' AS action,
                        AUTHORISED_BY AS performed_by,
                        AUTHORISED_DATE AS NOTIF_DATE,
                        'Voucher ' || VOUCHER_NO || ' approved' AS MESSAGE
                      FROM MASTER_TRANSACTION
                      WHERE AUTHORISED_DATE IS NOT NULL
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                      UNION ALL
                      SELECT 
                        'MASTER_TRANSACTION' AS table_name,
                        VOUCHER_NO AS TENDER_NO,
                        'Canceled' AS action,
                        CANCEL_BY AS performed_by,
                        CANCEL_DATE AS NOTIF_DATE,
                        'Voucher ' || VOUCHER_NO || ' canceled' AS MESSAGE
                      FROM MASTER_TRANSACTION
                      WHERE CANCEL_FLAG = 'Y'
                        AND COMPANY_CODE = '{company_code}'
                        AND FORM_CODE = '{form_code}'
                    )
                    SELECT 
                      table_name,
                      TENDER_NO,
                      action,
                      performed_by,
                      NOTIF_DATE,
                      MESSAGE
                    FROM notifications
                    WHERE NOTIF_DATE IS NOT NULL
                    ORDER BY NOTIF_DATE DESC";

                var notifications = _dbContext.SqlQuery<QuotationNotification>(query).ToList();
                return notifications;
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving quotations: " + ex.Message);
            }
        }
        public bool QuotationApproval(ApprovalRequest request)
        {
            try
            {
                var UPDATE_QUERY = $@"UPDATE sa_quotation_setup SET APPROVED_STATUS ='Y',   
                           APPROVED_BY = '{_workContext.CurrentUserinformation.login_code}',
                           MODIFIED_DATE = '{DateTime.Now.ToString("dd-MMM-yyyy")}',
                           MODIFIED_BY = '{_workContext.CurrentUserinformation.login_code}'
                           WHERE TENDER_NO='{request.TENDER_NO}' AND ID = '{request.ID}' AND COMPANY_CODE = '{_workContext.CurrentUserinformation.company_code}' AND STATUS = 'E'";
                _dbContext.ExecuteSqlCommand(UPDATE_QUERY);
                RollBackTender tender = _dbContext.SqlQuery<RollBackTender>($@"select a.TENDER_NO, to_number(a.ID) as ID from sa_quotation_setup a where a.TENDER_NO = '{request.TENDER_NO}' and rownum = 1").FirstOrDefault();

                QuotationLogModel log = new QuotationLogModel
                {
                    QUOTATION_ID = Convert.ToInt32(tender.ID),
                    TENDER_NO = tender.TENDER_NO,
                    TYPE = "MODIFY",
                    ACTION = "APPROVAL",
                    ACTION_BY = _workContext.CurrentUserinformation.login_code,
                    CHANGED = tender.TENDER_NO,
                    REMARKS = request.REMARKS
                };
                InsertRemarks(log);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool ApprovalProceeding()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            var login_code = _workContext.CurrentUserinformation.login_code;
            var query = $@"select USER_NO as ID, USER_TYPE as Type, LOGIN_CODE as Employee_Name, EMPLOYEE_CODE, LOGIN_EDESC as EMPLOYEE_EDESC, QUOTATION_APPROVAL_LIMIT from sc_application_users where lower(login_code) = lower('{login_code}') and company_code = '{company_code}' AND DELETED_FLAG = 'N' ";
            var approve = _dbContext.SqlQuery<Employee>(query).ToList();
                if(approve != null && approve.FirstOrDefault().Type == "ADMIN") // iF ADMIN can approve
            {
                return true;
            }
                else if(approve != null)
            {
                var user_id = approve.FirstOrDefault().ID;
                string menuQuery = $@"select APPROVE_FLAG from web_menu_control where user_no = '{user_id}' and menu_no = 
                            (
                            select menu_no from web_menu_management where full_path = '/QuotationManagement/Home/Index#!QM/QuotationDetailItemwise/'
                            )";
                string flag = _dbContext.SqlQuery<string>(menuQuery).FirstOrDefault();
                    if(flag == "Y")
                {
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public List<UserAcess> UserAccess(double amount)
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            var login_code = _workContext.CurrentUserinformation.login_code;
            var query = $@"select USER_NO as ID, USER_TYPE as Type, LOGIN_CODE as Employee_Name, EMPLOYEE_CODE, LOGIN_EDESC as EMPLOYEE_EDESC, QUOTATION_APPROVAL_LIMIT from sc_application_users where lower(login_code) = lower('{login_code}') and company_code = '{company_code}' AND DELETED_FLAG = 'N' ";
            var approve = _dbContext.SqlQuery<Employee>(query).ToList();
            var user_id = approve.FirstOrDefault().ID;
            if (approve != null && amount <= approve.FirstOrDefault().QUOTATION_APPROVAL_LIMIT && approve.FirstOrDefault().Type != "ADMIN") //Approval Limit and not admin
            {
                /*return new List<UserAcess> {
                    new UserAcess { APPROVE_FLAG = "Y" }
                };*/
                string userAccess = $@"select APPROVE_FLAG, CHECK_FLAG, RECOMMEND_FLAG, POST_FLAG, VERIFY_FLAG from web_menu_control where user_no = '{user_id}' and menu_no = 
                            (
                            select menu_no from web_menu_management where full_path = '/QuotationManagement/Home/Index#!QM/QuotationDetailItemwise/'
                            )";
                List<UserAcess> access = _dbContext.SqlQuery<UserAcess>(userAccess).ToList();
                return access;
            }
            else if (approve != null && approve.FirstOrDefault().Type == "ADMIN") // For admin
            {
                return new List<UserAcess> {
                    new UserAcess { APPROVE_FLAG = "Y", CHECK_FLAG = "Y", POST_FLAG = "Y", RECOMMEND_FLAG = "Y", VERIFY_FLAG = "Y", RECYCLE = true }
                };
            }
            /*else if (approve != null)
            {
                string userAccess = $@"select APPROVE_FLAG, CHECK_FLAG, RECOMMEND_FLAG, POST_FLAG from web_menu_control where user_no = '{user_id}' and menu_no = 
                            (
                            select menu_no from web_menu_management where full_path = '/QuotationManagement/Home/Index#!QM/QuotationDetailItemwise/'
                            )";
                List<UserAcess> access = _dbContext.SqlQuery<UserAcess>(userAccess).ToList();
                return access;
            }*/
            return new List<UserAcess> { new UserAcess { APPROVE_FLAG = "N", CHECK_FLAG = "N", POST_FLAG = "N", RECOMMEND_FLAG = "N", VERIFY_FLAG = "N" } };
        }
        public List<Tender> getSelectQuotationOptions()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                string query = $@"select 
                    FORM_CODE as ID, FORM_EDESC, CUSTOM_PREFIX_TEXT as PREFIX, CUSTOM_SUFFIX_TEXT as SUFFIX, BODY_LENGTH, DELETED_FLAG as STATUS, CREATED_DATE, CREATED_BY, COMPANY_CODE
                    from form_setup 
                    where company_code = '{company_code}' AND DELETED_FLAG = 'N' AND quotation_flag = 'Y'";

                List<Tender> tenders = this._dbContext.SqlQuery<Tender>(query).ToList();
                return tenders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Tender> getTemplateOptions()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                string query = $@"select 
                    FORM_CODE as ID, FORM_EDESC, CUSTOM_PREFIX_TEXT as PREFIX, CUSTOM_SUFFIX_TEXT as SUFFIX, BODY_LENGTH, DELETED_FLAG as STATUS, CREATED_DATE, CREATED_BY, COMPANY_CODE
                    from form_setup where form_code in  (SELECT  DISTINCT FORM_CODE FROM form_detail_setup WHERE table_name LIKE 'IP_PURCHASE_REQUEST' AND COMPANY_CODE = '{company_code}')
                    and company_code = '{company_code}' AND DELETED_FLAG = 'N'";

                List<Tender> tenders = this._dbContext.SqlQuery<Tender>(query).ToList();
                return tenders;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Quotation_setup> getTemplateData(Template data)
        {
            if (data == null) return null;

            try
            {
                string row = data.Row;
                string voucher = data.Voucher_no;
                string item = data.Item;
                DateTime? startDate = data.fromDate;
                DateTime? endDate = data.toDate;
                string form_code = data.Reference;
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch = _workContext.CurrentUserinformation.branch_code;

                string prefix = null;
                string suffix = null;

                if (!string.IsNullOrEmpty(data.prefix) && data.prefix != "all" && data.prefix.Contains(';'))
                {
                    var parts = data.prefix.Split(';');
                    if (parts.Length == 2)
                    {
                        prefix = parts[0];
                        suffix = parts[1];
                    }
                }
                if (string.IsNullOrEmpty(company_code) || string.IsNullOrEmpty(branch))
                    throw new Exception("Company code or Branch code is null.");

                string query = $@"
                    SELECT ip.FORM_CODE,ip.SERIAL_NO AS ID,ip.REQUEST_NO AS TENDER_NO,ip.REQUEST_DATE AS ISSUE_DATE,ip.CREATED_DATE,ip.CREATED_BY,ip.COMPANY_CODE,ip.REMARKS,ip.BRANCH_CODE,ip.MODIFY_DATE AS MODIFIED_DATE
                    FROM ip_purchase_request ip
                ";
                if(row != "all")
                {
                    query += $@",(  SELECT reference_form_code,
                   company_code,
                   reference_no,
                   reference_item_code,
                   SUM (reference_quantity) reference_quantity
              FROM REFERENCE_DETAIL
          GROUP BY reference_form_code,
                   company_code,
                   reference_no,
                   reference_item_code) b
   WHERE     ip.request_no = b.reference_no(+)
         AND ip.company_code = b.company_code(+)
         AND ip.item_code = b.reference_item_code(+)
         and ip.form_code=b.reference_form_code(+) 
         and ip.deleted_flag = 'N'
                      AND ip.company_code = '{company_code}'
                      AND ip.branch_code = '{branch}'";
                }
                else
                {
                    query += $@"WHERE ip.deleted_flag = 'N'
                      AND ip.company_code = '{company_code}'
                      AND ip.branch_code = '{branch}'";
                }
                if (!string.IsNullOrEmpty(voucher))
                {
                    query += $" AND ip.REQUEST_NO = '{voucher}'";
                }

                if (!string.IsNullOrEmpty(item))
                {
                    query += $" AND ip.item_code = '{item}'";
                }

                if (!string.IsNullOrEmpty(prefix) && !string.IsNullOrEmpty(suffix))
                {
                    // Extract middle number from tender_no and compare pattern
                    query += $@"
              AND REGEXP_LIKE(IP.REQUEST_NO, '^' || '{prefix}' || '[0-9]+' || '{suffix}')
            ";
                }

                if (startDate.HasValue && endDate.HasValue)
                {
                    string start = startDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    string end = endDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
                    if (string.IsNullOrEmpty(voucher))
                    {
                        query += $" AND ip.created_date BETWEEN TO_DATE('{start}', 'MM/DD/YYYY HH:MI:SS AM') AND TO_DATE('{end}', 'MM/DD/YYYY HH:MI:SS AM')";
                    }
                }

                if (row == "nonreference")
                {
                    query += $@" and ip.quantity-b.reference_quantity =0";
                }
                else if (row == "incomplete")
                {
                    query += $@"and ip.quantity-b.reference_quantity >=0";
                }
                query += $@" order by 3,2";
                var result = _dbContext.SqlQuery<Quotation_setup>(query).ToList();
                foreach (var quotation in result)
                {
                    string items = $@"select R.SERIAL_NO AS ID,R.ITEM_CODE,R.SPECIFICATION,R.MU_CODE AS UNIT,R.QUANTITY from IP_PURCHASE_REQUEST R where REQUEST_NO='{quotation.TENDER_NO}' AND DELETED_FLAG='N' AND SERIAL_NO = '{quotation.ID}'";
                    if (!string.IsNullOrEmpty(item))
                    {
                        query += $" AND R.item_code = '{item}'";
                    }
                    //switch (row)
                    //{
                    //    case "nonreference":
                    //        items += $@" AND NOT EXISTS (SELECT 1 FROM REFERENCE_DETAIL RD WHERE RD.REFERENCE_ITEM_CODE = R.ITEM_CODE AND form_code in(select FORM_CODE from form_setup where quotation_flag = 'Y' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{company_code}'))";
                    //        break;
                    //    default:
                    //        break;
                    //}
                    List<Item> itemData = this._dbContext.SqlQuery<Item>(items).ToList();
                    quotation.Items = itemData;
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Quotation> getVoucherList(string code, string row)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch = _workContext.CurrentUserinformation.branch_code;
                string decodedCode = System.Net.WebUtility.UrlDecode(code);
                string prefix = "";
                string suffix = "";
                if (!string.IsNullOrEmpty(decodedCode) && decodedCode.Contains(";"))
                {
                    var parts = decodedCode.Split(';');
                    prefix = parts[0];
                    suffix = parts.Length > 1 ? parts[1] : "";
                }
                string query = $@"
SELECT DISTINCT ip.REQUEST_NO AS TENDER_NO
FROM ip_purchase_request ip
JOIN (
    SELECT reference_form_code,
           company_code,
           reference_no,
           reference_item_code,
           SUM(reference_quantity) AS reference_quantity
    FROM REFERENCE_DETAIL
    GROUP BY reference_form_code,
             company_code,
             reference_no,
             reference_item_code
) b ON ip.request_no = b.reference_no
   AND ip.company_code = b.company_code
   AND ip.item_code = b.reference_item_code
   AND ip.form_code = b.reference_form_code
WHERE ip.deleted_flag = 'N'
  AND ip.company_code = '{company_code}'
  AND ip.branch_code = '{branch}'
  ";
                if(row == "nonreference" || row == "incomplete")
                {
                    query += "AND ip.quantity - b.reference_quantity = 0";
                }
                if (code != "")
                {
                    query += $@" AND REGEXP_LIKE(ip.REQUEST_NO, '{prefix}' || '\d+' || '{suffix}')";
                }
                query += $@" ORDER BY ip.REQUEST_NO";
                List<Quotation> data = this._dbContext.SqlQuery<Quotation>(query).ToList();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool AddReference(List<Reference> reference, string form_code, string voucher_no)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var branch = _workContext.CurrentUserinformation.branch_code;
                var srNo = 1;
                foreach (var item in reference)
                {
                    var maxRefNoQry = $@"SELECT TO_CHAR(SYSDATE,'RRRRMMDD')||'.'||LPAD(MAX(REGEXP_SUBSTR(TRANSACTION_NO,'[^.]+', 1, 2))+1,11,'0')TRANSACTION_NO FROM REFERENCE_DETAIL";
                    var maxRefNo = this._dbContext.SqlQuery<string>(maxRefNoQry).FirstOrDefault();
                    var refInsQuery = $@"INSERT INTO REFERENCE_DETAIL (TRANSACTION_NO,VOUCHER_NO,FORM_CODE,COMPANY_CODE,SERIAL_NO,REFERENCE_NO,REFERENCE_FORM_CODE,REFERENCE_ITEM_CODE,
                                                      REFERENCE_QUANTITY,REFERENCE_MU_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,REFERENCE_UNIT_PRICE,REFERENCE_TOTAL_PRICE,REFERENCE_CALC_UNIT_PRICE,REFERENCE_CALC_TOTAL_PRICE, REFERENCE_REMARKS,REFERENCE_DATE,BRANCH_CODE,REFERENCE_BRANCH_CODE,REFERENCE_SERIAL_NO,SYN_ROWID,BATCH_NO,REFERENCE_BATCH_NO,VOUCHER_DATE, REFERENCE_ACTUAL_QTY) 
                                                    VALUES('{maxRefNo}','{voucher_no}','{form_code}','{_workContext.CurrentUserinformation.company_code}','{srNo}','{item.REFERENCE_NO}','{item.REF_FORM_CODE}','{item.ITEM_CODE}','{item.REF_QTY}','{item.UNIT}','{_workContext.CurrentUserinformation.login_code}',SYSDATE,
                                                           'N',0,0,0,0,'',SYSDATE,'{_workContext.CurrentUserinformation.branch_code}','{_workContext.CurrentUserinformation.branch_code}','','','','',SYSDATE, '{item.QUANTITY}')";
                    this._dbContext.ExecuteSqlCommand(refInsQuery);
                    srNo++;
                }
                return true;
            } catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<Employee> getUserValue()
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var menuQuery = $@"select menu_no from web_menu_management where full_path = '/QuotationManagement/Home/Index#!QM/QuotationDetailItemwise/'";
                List<String> menu = _dbContext.SqlQuery<String>(menuQuery).ToList();
                var menu_no = menu.FirstOrDefault();
                var query = $@"SELECT a.USER_NO as ID, a.USER_TYPE as Type, a.LOGIN_CODE as Employee_Name, a.EMPLOYEE_CODE, a.LOGIN_EDESC as EMPLOYEE_EDESC,
       a.QUOTATION_APPROVAL_LIMIT,
       b.APPROVE_FLAG,
       b.POST_FLAG,
       b.VERIFY_FLAG,
       b.CHECK_FLAG,
       b.RECOMMEND_FLAG,
       C.MENU_NO,C.MENU_EDESC
  FROM sc_application_users a,(select * from  web_menu_control where  menu_no='{menu_no}')  b, WEB_MENU_MANAGEMENT C
 WHERE     a.USER_NO = b.User_no(+)
       AND a.company_code = b.company_code(+)
       AND a.company_code = '{company_code}'
       AND A.group_sku_flag = 'I'
       AND A.DELETED_FLAG = 'N'
       AND B.MENU_NO=C.MENU_NO(+)
       AND B.COMPANY_CODE=C.COMPANY_CODE(+) order by a.LOGIN_CODE";
                List<Employee> res = this._dbContext.SqlQuery<Employee>(query).ToList();
                return res;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool UserTypeToSetValue()
        {
            var company_code = _workContext.CurrentUserinformation.company_code;
            var login = _workContext.CurrentUserinformation.login_code;
            var query = $@"select user_type from sc_application_users where company_code = '{company_code}' and lower(login_code) = lower('{login}') AND DELETED_FLAG = 'N'";
            string res = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            if(res == "ADMIN")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool setUserValue(Employee employee)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var login_code = _workContext.CurrentUserinformation.login_code;
                var query = $@"update sc_application_users set QUOTATION_APPROVAL_LIMIT = '{employee.QUOTATION_APPROVAL_LIMIT}',
                            MODIFY_DATE = TO_DATE('{DateTime.Now:dd-MMM-yyyy}', 'DD-MON-YYYY'), MODIFY_BY = '{login_code}'
                            where user_no = '{employee.ID}' and company_code = '{company_code}' and LOGIN_CODE = '{employee.Employee_Name}' 
                            AND EMPLOYEE_FLAG = 'Y' AND DELETED_FLAG = 'N'";
                _dbContext.ExecuteSqlCommand(query);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public bool setUserAccess(UserAcess access)
        {
            try
            {
                var company_code = _workContext.CurrentUserinformation.company_code;
                var login_code = _workContext.CurrentUserinformation.login_code;
                var menuQuery = $@"select menu_no from web_menu_management where full_path = '/QuotationManagement/Home/Index#!QM/QuotationDetailItemwise/'";
                List<String> menu = _dbContext.SqlQuery<String>(menuQuery).ToList();
                var menu_no = menu.FirstOrDefault();
                string checkQuery = $@"select user_no from web_menu_control where user_no = '{access.ID}' and menu_no = '{menu_no}' and company_code = '{company_code}'";
                var check = _dbContext.SqlQuery<int>(checkQuery).FirstOrDefault();
                string query = "";
                if(check == 0)
                {
                    query = $@"insert into web_menu_control
                    (
                        user_no, menu_no, access_flag, company_code,
                        created_by, created_date, approve_flag, check_flag, post_flag, recommend_flag, VERIFY_FLAG
                    ) values
                    (
                        '{access.ID}', '{menu_no}', 'Y', '{company_code}',
                        '{login_code}', sysdate, '{access.APPROVE_FLAG}', '{access.CHECK_FLAG}', '{access.POST_FLAG}', '{access.RECOMMEND_FLAG}', '{access.VERIFY_FLAG}'
                    )";
                }
                else
                {
                    query = $@"UPDATE WEB_MENU_CONTROL SET 
                                approve_flag = '{access.APPROVE_FLAG}', check_flag = '{access.CHECK_FLAG}', post_flag = '{access.POST_FLAG}', recommend_flag = '{access.RECOMMEND_FLAG}', VERIFY_FLAG = '{access.VERIFY_FLAG}'
                                where company_code = '{company_code}' and menu_no = '{menu_no}' and user_no = '{access.ID}'";
                }

                _dbContext.ExecuteSqlCommand(query);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public List<PartyDetails> partyDetails(string id)
        {
            string tenderQuery = $@"SELECT TENDER_NO FROM SA_QUOTATION_SETUP WHERE ID = '{id}' AND STATUS = 'E'";
            string tenderNo = _dbContext.SqlQuery<string>(tenderQuery).FirstOrDefault();

            List<PartyDetails> rate = new List<PartyDetails>();
            string query = $@"select PAN_NO, SUPPLIER_CODE, CURRENCY, CURRENCY_RATE, DELIVERY_DATE, DISCOUNT_TYPE
                from quotation_details where tender_no='{tenderNo}'";
            rate = _dbContext.SqlQuery<PartyDetails>(query).ToList();
            return rate;
        }
        public List<PartyDetailsItems> partyDetailsItems(string id)
        {
            string tenderQuery = $@"SELECT TENDER_NO FROM SA_QUOTATION_SETUP WHERE ID = '{id}' AND STATUS = 'E'";
            string tenderNo = _dbContext.SqlQuery<string>(tenderQuery).FirstOrDefault();

            string query = $@"SELECT A.*
                FROM QUOTATION_DETAIL_ITEMWISE a
                where quotation_no = (
                SELECT QUOTATION_NO
                FROM (
                  SELECT QUOTATION_NO
                  FROM QUOTATION_DETAILS
                  WHERE TENDER_NO = '{tenderNo}'
                  ORDER BY 
                    CASE 
                      WHEN REVISE IS NULL THEN 0
                      WHEN REGEXP_LIKE(REVISE, '^Revised [0-9]+$') THEN TO_NUMBER(REGEXP_SUBSTR(REVISE, '[0-9]+'))
                      ELSE 0
                    END DESC
                )
                WHERE ROWNUM = 1)";
            List<PartyDetailsItems> items = _dbContext.SqlQuery<PartyDetailsItems>(query).ToList();
            return items;
        }
        public List<TermsAndConditions> termsAndConditions(string id)
        {
            string tenderQuery = $@"SELECT TENDER_NO FROM SA_QUOTATION_SETUP WHERE ID = '{id}' AND STATUS = 'E'";
            string tenderNo = _dbContext.SqlQuery<string>(tenderQuery).FirstOrDefault();

            string query = $@"SELECT A.*
                FROM QUOTATION_TERM_CONDITION a
                where quotation_no = (
                SELECT QUOTATION_NO
                FROM (
                  SELECT QUOTATION_NO
                  FROM QUOTATION_DETAILS
                  WHERE TENDER_NO = '{tenderNo}'
                  ORDER BY 
                    CASE 
                      WHEN REVISE IS NULL THEN 0
                      WHEN REGEXP_LIKE(REVISE, '^Revised [0-9]+$') THEN TO_NUMBER(REGEXP_SUBSTR(REVISE, '[0-9]+'))
                      ELSE 0
                    END DESC
                )
                WHERE ROWNUM = 1) ORDER BY ID";
            List<TermsAndConditions> items = _dbContext.SqlQuery<TermsAndConditions>(query).ToList();
            return items;
        }
    }
}
