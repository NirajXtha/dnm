using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;
using System.Text;

namespace NeoErp.LOC.Services.Services
{
    public class LcEntryService : ILcEntryService
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public LcEntryService(IDbContext dbContext, IWorkContext workcontext)
        {
            this._dbContext = dbContext;
            this._workcontext = workcontext;
        }



        public void DeleteLcByLcCode(int LocCode)
        {
            string query = string.Format(@"UPDATE LC_LOC SET DELETED_FLAG  = '{0}' WHERE LOC_CODE IN ({1})",
            'Y', LocCode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public List<LcEntryModels> getAllLcList()
        {
            var sqlquery = $@"SELECT  LP.ORDER_NO,LL.LOC_CODE,IPS.SUPPLIER_EDESC,LL.PINVOICE_CODE,LPI.PINVOICE_NO,NVL(IPS.SUPPLIER_EDESC,LL.LC_NUMBER)AS LC_NUMBER,LL.LC_TRACK_NO,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.LEAD_TIME,LL.EXPIRY_PLACE,LL.STATUS_CODE,LS.STATUS_EDESC,
            LL.LAST_SHIPMENT_DATE,LL.TOLERANCE_PER,LL.TERMS_CODE,LT.TERMS_EDESC, 
            LL.ADVISING_BANK_CODE,LB.BANK_NAME AS ADVISING_BANK_EDESC,LL.CONFIRM_BANK_CODE,LB1.BANK_NAME AS CONFIRM_BANK_EDESC, 
            LL.ISSUING_BANK_CODE,LB2.BANK_NAME AS ISSUING_BANK_EDESC,LL.PTERMS_CODE,LPT.PTERMS_EDESC,LL.CREDIT_DAYS,LL.PARTIAL_SHIPMENT,
                LL.TRANSSHIPMENT,LL.CONFIRMATION_REQ,LL.TRANSFERABLE,LL.INSURANCE_FLAG,LL.APP_OUT_CHARGE,LL.BEF_OUT_CHARGE,LL.APP_CONFIRM_CHARGE,LL.BNF_CONFIRM_CHARGE,
                LL.DOC_REQ_DAYS,LL.ORIGIN_COUNTRY_CODE,LD.FILE_DETAIL,DECODE(LL.APPROVED_BY, null, 'N', 'Y') AS APPROVED_BY FROM LC_LOC LL
                LEFT JOIN LC_PURCHASE_ORDER LP ON LL.LC_TRACK_NO =LP.LC_TRACK_NO AND LP.COMPANY_CODE= LL.COMPANY_CODE
                LEFT JOIN LC_PERFOMA_INVOICE LPI ON LL.LC_TRACK_NO = LPI.LC_TRACK_NO AND LL.PINVOICE_CODE = LPI.PINVOICE_CODE
                LEFT JOIN LC_TERMS LT ON LL.TERMS_CODE = LT.TERMS_CODE AND LT.COMPANY_CODE= LL.COMPANY_CODE
                LEFT JOIN LC_PAYMENT_TERMS LPT ON LL.PTERMS_CODE = LPT.PTERMS_CODE AND LPT.COMPANY_CODE= LL.COMPANY_CODE
                LEFT JOIN LC_STATUS LS ON LL.STATUS_CODE = LS.STATUS_CODE AND LS.COMPANY_CODE= LL.COMPANY_CODE
                LEFT JOIN LC_DOCUMENT LD ON LL.LOC_CODE = LD.LOC_CODE AND LD.COMPANY_CODE= LL.COMPANY_CODE
                LEFT JOIN FA_BANK_SETUP LB ON LL.ADVISING_BANK_CODE = LB.BANK_CODE AND LL.COMPANY_CODE = LB.COMPANY_CODE
                LEFT JOIN FA_BANK_SETUP LB1 ON LL.CONFIRM_BANK_CODE = LB1.BANK_CODE AND LL.COMPANY_CODE = LB1.COMPANY_CODE
                LEFT JOIN FA_BANK_SETUP LB2 ON LL.ISSUING_BANK_CODE = LB2.BANK_CODE AND LL.COMPANY_CODE = LB2.COMPANY_CODE
                LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE WHERE LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LL.CREATED_DATE DESC";
            var lclist = _dbContext.SqlQuery<LcEntryModels>(sqlquery).ToList();
            List<LcEntryModels> newlist = new List<LcEntryModels>();
            foreach (var item in lclist)
            {
                if (item.FILE_DETAIL != null)
                {
                    if (item.FILE_DETAIL.ToString().Contains(':'))
                    {
                        item.mylist = item.FILE_DETAIL.Split(':');
                        item.FILE_DETAIL = null;
                    }
                    else
                    {
                        item.FILE_DETAIL = item.FILE_DETAIL;
                    }
                }
                newlist.Add(item);
            }


            return newlist;
        }

        public List<LcEntryModels> GetAllLcListFilter(string perfomainvoice, string lcnumber)
        {
            List<LcEntryModels> record = new List<LcEntryModels>();
            if (string.IsNullOrEmpty(perfomainvoice) && string.IsNullOrEmpty(lcnumber))
            {
                return record;
            }
            else
            {
                if (perfomainvoice == null)
                {
                    perfomainvoice = " ";
                }
                if (lcnumber == null)
                {
                    lcnumber = " ";
                }
            }

            var sqlquery = $@"SELECT * FROM (SELECT  LP.ORDER_NO,LL.LOC_CODE,IPS.SUPPLIER_EDESC,LL.PINVOICE_CODE,LPI.PINVOICE_NO,LL.LC_NUMBER,LL.LC_TRACK_NO,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.LEAD_TIME,LL.EXPIRY_PLACE,LL.STATUS_CODE,LS.STATUS_EDESC,
            LL.LAST_SHIPMENT_DATE,LL.TOLERANCE_PER,LL.TERMS_CODE,LT.TERMS_EDESC, 
            LL.ADVISING_BANK_CODE,LB.BANK_NAME AS ADVISING_BANK_EDESC,LL.CONFIRM_BANK_CODE,LB1.BANK_NAME AS CONFIRM_BANK_EDESC, 
            LL.ISSUING_BANK_CODE,LB2.BANK_NAME AS ISSUING_BANK_EDESC,LL.PTERMS_CODE,LPT.PTERMS_EDESC,LL.CREDIT_DAYS,LL.PARTIAL_SHIPMENT,
                LL.TRANSSHIPMENT,LL.CONFIRMATION_REQ,LL.TRANSFERABLE,LL.INSURANCE_FLAG,LL.APP_OUT_CHARGE,LL.BEF_OUT_CHARGE,LL.APP_CONFIRM_CHARGE,LL.BNF_CONFIRM_CHARGE,
                LL.DOC_REQ_DAYS,LL.ORIGIN_COUNTRY_CODE,LD.FILE_DETAIL FROM LC_LOC LL
                LEFT JOIN LC_PURCHASE_ORDER LP ON LL.LC_TRACK_NO =LP.LC_TRACK_NO 
                LEFT JOIN LC_PERFOMA_INVOICE LPI ON LL.LC_TRACK_NO = LPI.LC_TRACK_NO AND LL.PINVOICE_CODE = LPI.PINVOICE_CODE
                LEFT JOIN LC_TERMS LT ON LL.TERMS_CODE = LT.TERMS_CODE
                LEFT JOIN LC_PAYMENT_TERMS LPT ON LL.PTERMS_CODE = LPT.PTERMS_CODE
                LEFT JOIN LC_STATUS LS ON LL.STATUS_CODE = LS.STATUS_CODE
                LEFT JOIN LC_DOCUMENT LD ON LL.LOC_CODE = LD.LOC_CODE
                LEFT JOIN FA_BANK_SETUP LB ON LL.ADVISING_BANK_CODE = LB.BANK_CODE AND LL.COMPANY_CODE = LB.COMPANY_CODE
                LEFT JOIN FA_BANK_SETUP LB1 ON LL.CONFIRM_BANK_CODE = LB1.BANK_CODE AND LL.COMPANY_CODE = LB1.COMPANY_CODE
                LEFT JOIN FA_BANK_SETUP LB2 ON LL.ISSUING_BANK_CODE = LB2.BANK_CODE AND LL.COMPANY_CODE = LB2.COMPANY_CODE
                LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE WHERE LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}')TT
                WHERE ((TT.PINVOICE_NO ='{perfomainvoice}') OR(TT.SUPPLIER_EDESC ='{lcnumber}'))";
            var lclist = _dbContext.SqlQuery<LcEntryModels>(sqlquery).ToList();
            List<LcEntryModels> newlist = new List<LcEntryModels>();
            foreach (var item in lclist)
            {
                if (item.FILE_DETAIL != null)
                {
                    if (item.FILE_DETAIL.ToString().Contains(':'))
                    {
                        item.mylist = item.FILE_DETAIL.Split(':');
                        item.FILE_DETAIL = null;
                    }
                    else
                    {
                        item.FILE_DETAIL = item.FILE_DETAIL;
                    }
                }
                newlist.Add(item);
            }


            return newlist;
        }



        public static string ConvertStringArrayToString(string[] array)
        {
            //
            // Concatenate all the elements into a StringBuilder.
            //
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append(':');
            }
            return builder.ToString();
        }


        public List<LCTermModels> getTermsListByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select TERMS_CODE AS TermCode
                            ,COALESCE(TERMS_EDESC,' ') TermName
                            from LC_TERMS 
                            where deleted_flag='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            and (TERMS_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(TERMS_EDESC) like '%{filter.ToUpperInvariant()}%')";
            var terms = _dbContext.SqlQuery<LCTermModels>(sqlquery).ToList();
            return terms;
        }

        public List<LCBankModels> getBanksListByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select BANK_CODE AS BANK_CODE
                            ,COALESCE(BANK_NAME,' ') BANK_NAME, ADDRESS,BANK_ACC_NO,SWIFT_CODE,BRANCH
                            FROM FA_BANK_SETUP 
                            WHERE DELETED_FLAG='N' 
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            AND (BANK_CODE LIKE '%{filter.ToUpperInvariant()}%' 
                            OR UPPER(BANK_NAME) LIKE '%{filter.ToUpperInvariant()}%')";
            var bankname = _dbContext.SqlQuery<LCBankModels>(sqlquery).ToList();
            return bankname;
        }




        public List<LCBankModels> getSupplierBanksListByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select BANK_CODE AS BANK_CODE
                            ,COALESCE(BANK_EDESC,' ') AS BANK_NAME,BANK_ADDRESS AS ADDRESS,BRANCH,SWIFT_CODE
                            FROM LC_BANK 
                            WHERE DELETED_FLAG='N'
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            AND (BANK_CODE LIKE '%{filter.ToUpperInvariant()}%' 
                            OR UPPER(BANK_EDESC) LIKE '%{filter.ToUpperInvariant()}%')";
            var bankname = _dbContext.SqlQuery<LCBankModels>(sqlquery).ToList();
            return bankname;
        }

        public List<SupplierModels> getSupplierListByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select SUPPLIER_CODE
                            ,COALESCE(SUPPLIER_EDESC,' ') AS SUPPLIER_EDESC
                            FROM IP_SUPPLIER_SETUP
                            WHERE PARTY_TYPE_CODE = 'LC' AND DELETED_FLAG = 'N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND GROUP_SKU_FLAG = 'I' 
                            AND (SUPPLIER_CODE LIKE '%{filter.ToUpperInvariant()}%' 
                            OR UPPER(SUPPLIER_EDESC) LIKE '%{filter.ToUpperInvariant()}%')";
            var suppliers = _dbContext.SqlQuery<SupplierModels>(sqlquery).ToList();
            return suppliers;
        }



        public List<LCPTermModels> getPaymentTermsListByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select PTERMS_CODE AS PTermCode
                            ,COALESCE(PTERMS_EDESC,' ') PTermName
                            from LC_PAYMENT_TERMS 
                            where deleted_flag='N'
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            and (PTERMS_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(PTERMS_EDESC) like '%{filter.ToUpperInvariant()}%')";
            var paymentermname = _dbContext.SqlQuery<LCPTermModels>(sqlquery).ToList();
            return paymentermname;
        }

        public List<LCStatusModels> getStatusListByFlter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select STATUS_CODE AS StatusCode
                            ,COALESCE(STATUS_EDESC,' ') StatusName
                            from LC_STATUS 
                            where deleted_flag='N' 
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            and (STATUS_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(STATUS_EDESC) like '%{filter.ToUpperInvariant()}%')";
            var statuslist = _dbContext.SqlQuery<LCStatusModels>(sqlquery).ToList();
            return statuslist;
        }
        //
        public List<LCNumberModels> GetAllLCNumberByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"SELECT LL.LOC_CODE AS LocCode
	                                --,ISS.SUPPLIER_EDESC AS LcNumber
                                    ,LL.LC_NUMBER AS LcNumber   
                                FROM LC_LOC LL
                               -- INNER JOIN IP_SUPPLIER_SETUP ISS ON ISS.SUPPLIER_CODE = LL.LC_NUMBER
                            where LL.deleted_flag='N' 
                            AND LL.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LcNumber DESC";
            var lcNumberList = _dbContext.SqlQuery<LCNumberModels>(sqlquery).ToList();
            return lcNumberList;
        }

        public List<ItemNameModels> GetAllItemNameByFilter(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"SELECT LIMS.ITEM_CODE AS ItemCode
	                                ,LIMS.ITEM_EDESC AS ItemName
                                FROM LC_ITEM LI
	                                ,IP_ITEM_MASTER_SETUP LIMS
                                WHERE LIMS.ITEM_CODE = LI.ITEM_CODE
                                AND LIMS.deleted_flag='N' 
                                AND LIMS.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY ItemName ASC";
            var itemNameList = _dbContext.SqlQuery<ItemNameModels>(sqlquery).ToList();
            return itemNameList;
        }
        //
        public List<PerformaInvoiceCurrencyModel> getCurrencyListByFlter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"select distinct
                             COALESCE(CURRENCY_CODE,' ') CURRENCY_CODE
                            ,COALESCE(CURRENCY_EDESC,' ') CURRENCY_EDESC
                            from CURRENCY_SETUP 
                            where deleted_flag='N' 
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            and CURRENCY_CODE like '%{filter.ToUpperInvariant()}%' 
                            or upper(CURRENCY_EDESC) like '%{filter.ToUpperInvariant()}%'";
                var result = _dbContext.SqlQuery<PerformaInvoiceCurrencyModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PerformaInvoiceModel> GetAllLcIpPurchaseOrder(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"select PINVOICE_NO,LC_TRACK_NO,CREATED_DATE from LC_PERFOMA_INVOICE
                                where UPPER(PINVOICE_NO) like '%{filter.ToUpperInvariant()}%' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND  DELETED_FLAG = 'N' ORDER BY CREATED_DATE DESC";
                var result = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public LCItemsModels GetAllItemsByOrderCode(string perfomainvoice)
        {
            if (perfomainvoice != null)
            {
                var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_PERFOMA_INVOICE WHERE PINVOICE_NO = '{perfomainvoice}'";
                var lctrackno = _dbContext.SqlQuery<int?>(sqlquery).FirstOrDefault();
                if (lctrackno != null)
                {
                    var sqlquery1 = $@"select LP.PINVOICE_DATE,LP.PINVOICE_NO,LP.LC_TRACK_NO,LP.BNF_CODE,LPO.TERMS_CODE,LPO.PTERMS_CODE,LPO.LEAD_TIME,LB.BNF_EDESC AS BNF_NAME,LPO.CREDIT_DAYS,LPO.VALIDITY_DATE
                                ,LPO.EST_DELIVERY_DATE,LPO.ORDER_NO,LP.PINVOICE_CODE from 
                                LC_PERFOMA_INVOICE LP
                                LEFT JOIN LC_PURCHASE_ORDER LPO ON LP.LC_TRACK_NO = LPO.LC_TRACK_NO
                                LEFT JOIN LC_BENEFICIARY LB ON LP.BNF_CODE = LB.BNF_CODE
                                  WHERE LP.DELETED_FLAG = 'N' AND LP.LC_TRACK_NO = '{lctrackno}' AND LP.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                    var result1 = _dbContext.SqlQuery<LCItemsModels>(sqlquery1).ToList().FirstOrDefault();
                    var items = $@"SELECT LI.LC_TRACK_NO,LI.SNO,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE,LI.QUANTITY,LI.AMOUNT,LI.HS_CODE,
                    LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE,CS.COUNTRY_CODE || '-' || CS.COUNTRY_EDESC AS COUNTRY_EDESC,LI.CURRENCY_CODE FROM 
                    LC_ITEM LI LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE  
                    LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE WHERE LC_TRACK_NO = '{lctrackno}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                    var itemsresult = _dbContext.SqlQuery<Items>(items).ToList();
                    result1.Itemlist.AddRange(itemsresult);

                    return result1;
                }
                else
                {
                    LCItemsModels itemModels = new LCItemsModels();
                    return itemModels;
                }
            }


            else
            {
                LCItemsModels itemModels = new LCItemsModels();
                return itemModels;
            }
        }

        public LcEntryModels CreateLCEntry(LcEntryModels lcentrydetails)
        {
            if (lcentrydetails.LOC_CODE == 0)
            {
                foreach (var items in lcentrydetails.Itemlist)
                {
                    if (items.EDITED == "Y" || items.AMTEDITED == "Y")
                    {
                        var itemupdate = $@"UPDATE LC_ITEM SET QUANTITY='{items.QUANTITY}',ITEM_CODE = '{items.ITEM_CODE}',AMOUNT='{items.AMOUNT}',                           REMARKS='{items.AMENDMENT}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                                        .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                        .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE DELETED_FLAG = '{'N'}' AND LC_TRACK_NO = '{lcentrydetails.LC_TRACK}' AND SNO = '{items.SNO}'";
                        var insert = _dbContext.ExecuteSqlCommand(itemupdate);
                        _dbContext.SaveChanges();
                    }
                }
                var lccodequery = $@"SELECT LC_LOC_SEQ.nextval as PINVOICE_CODE FROM DUAL";
                var lccode = _dbContext.SqlQuery<int>(lccodequery).FirstOrDefault();
                var insertPI = $@"INSERT INTO LC_LOC(LOC_CODE,PINVOICE_CODE,LC_NUMBER,LC_TRACK_NO,OPEN_DATE,EXPIRY_DATE,LEAD_TIME,EXPIRY_PLACE,STATUS_CODE,LAST_SHIPMENT_DATE,TOLERANCE_PER,TERMS_CODE,ADVISING_BANK_CODE,CONFIRM_BANK_CODE,ISSUING_BANK_CODE,PTERMS_CODE,CREDIT_DAYS,PARTIAL_SHIPMENT,TRANSSHIPMENT,CONFIRMATION_REQ,TRANSFERABLE,INSURANCE_FLAG,APP_OUT_CHARGE,BEF_OUT_CHARGE,APP_CONFIRM_CHARGE,BNF_CONFIRM_CHARGE,DOC_REQ_DAYS,ORIGIN_COUNTRY_CODE,REMARKS,COMPANY_CODE, BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{lccode}','{lcentrydetails.PINVOICE_CODE}','{lcentrydetails.LC_NUMBER}', '{lcentrydetails.LC_TRACK}',TO_DATE('{lcentrydetails.OPEN_DATE}','MM/dd/yyyy HH:MI:SS AM'),TO_DATE('{lcentrydetails.EXPIRY_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{lcentrydetails.LEAD_TIME}','{lcentrydetails.EXPIRY_PLACE}','{lcentrydetails.STATUS_CODE}',TO_DATE('{lcentrydetails.LAST_SHIPMENT_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{lcentrydetails.TOLERANCE_PER}', '{lcentrydetails.TERMS_CODE}','{lcentrydetails.ADVISING_BANK_CODE}', '{lcentrydetails.CONFIRM_BANK_CODE}', '{lcentrydetails.ISSUING_BANK_CODE}', '{lcentrydetails.PTERMS_CODE}', '{lcentrydetails.CREDIT_DAYS}','{lcentrydetails.PARTIAL_SHIPMENT}','{lcentrydetails.TRANSSHIPMENT}','{lcentrydetails.CONFIRMATION_REQ}','{lcentrydetails.TRANSFERABLE}','{lcentrydetails.INSURANCE_FLAG}','{lcentrydetails.APP_OUT_CHARGE}','{lcentrydetails.BEF_OUT_CHARGE}','{lcentrydetails.APP_CONFIRM_CHARGE}','{lcentrydetails.BNF_CONFIRM_CHARGE}','{lcentrydetails.DOC_REQ_DAYS}','{lcentrydetails.ORIGIN_COUNTRY_CODE}','{lcentrydetails.REMARKS}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                var purchaseorder = _dbContext.ExecuteSqlCommand(insertPI);
                LcEntryModels returnlist = new LcEntryModels();
                returnlist.LC_TRACK_NO = Convert.ToInt32(lcentrydetails.LC_TRACK);
                returnlist.LOC_CODE = lccode;
                return returnlist;
            }
            else
            {
                var updatetPI = $@"UPDATE LC_LOC SET LC_NUMBER = '{lcentrydetails.LC_NUMBER}',OPEN_DATE=TO_DATE('{lcentrydetails.OPEN_DATE}','MM/dd/yyyy HH:MI:SS AM'),EXPIRY_DATE=TO_DATE('{lcentrydetails.EXPIRY_DATE}','MM/dd/yyyy HH:MI:SS AM'),EXPIRY_PLACE='{lcentrydetails.EXPIRY_PLACE}',LEAD_TIME='{lcentrydetails.LEAD_TIME}', STATUS_CODE='{lcentrydetails.STATUS_CODE}',LAST_SHIPMENT_DATE=TO_DATE('{lcentrydetails.LAST_SHIPMENT_DATE}','MM/dd/yyyy HH:MI:SS AM'), 
                                            TOLERANCE_PER= '{lcentrydetails.TOLERANCE_PER}',TERMS_CODE='{lcentrydetails.TERMS_CODE}',ADVISING_BANK_CODE='{lcentrydetails.ADVISING_BANK_CODE}',CONFIRM_BANK_CODE='{lcentrydetails.CONFIRM_BANK_CODE}',ISSUING_BANK_CODE='{lcentrydetails.ISSUING_BANK_CODE}',PTERMS_CODE='{lcentrydetails.PTERMS_CODE}',CREDIT_DAYS='{lcentrydetails.CREDIT_DAYS}',PARTIAL_SHIPMENT='{lcentrydetails.PARTIAL_SHIPMENT}',TRANSSHIPMENT='{lcentrydetails.TRANSSHIPMENT}',CONFIRMATION_REQ='{lcentrydetails.CONFIRMATION_REQ}',TRANSFERABLE='{lcentrydetails.TRANSFERABLE}',INSURANCE_FLAG='{lcentrydetails.INSURANCE_FLAG}',APP_OUT_CHARGE='{lcentrydetails.APP_OUT_CHARGE}',BEF_OUT_CHARGE='{lcentrydetails.BEF_OUT_CHARGE}',APP_CONFIRM_CHARGE='{lcentrydetails.APP_CONFIRM_CHARGE}',BNF_CONFIRM_CHARGE='{lcentrydetails.BNF_CONFIRM_CHARGE}',DOC_REQ_DAYS='{lcentrydetails.DOC_REQ_DAYS}',ORIGIN_COUNTRY_CODE='{lcentrydetails.ORIGIN_COUNTRY_CODE}',REMARKS='{lcentrydetails.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                                   .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                   .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LC_TRACK_NO = '{lcentrydetails.LC_TRACK_NO}' AND LOC_CODE = '{lcentrydetails.LOC_CODE}'";
                var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetPI);
                LcEntryModels returnlist = new LcEntryModels();
                returnlist.LC_TRACK_NO = lcentrydetails.LC_TRACK_NO;
                returnlist.LOC_CODE = lcentrydetails.LOC_CODE;
                return returnlist;
            }
        }

        public void UpdateImage(LcImageModels lcimagedetail, string loccode)
        {
            var sqlquery = $@"SELECT FILE_DETAIL FROM LC_DOCUMENT WHERE LC_TRACK_NO  ='{lcimagedetail.LocCode}' AND LOC_CODE = '{loccode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();

            if (result != null)
            {
                if (result.FILE_DETAIL != null)
                {
                    string dbpath = result.FILE_DETAIL + ":" + lcimagedetail.Path;
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND LOC_CODE IN ({6})",
                   dbpath, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, loccode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND LOC_CODE IN ({6})",
                   lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, loccode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                var lcdocument = $@"SELECT SEQ_LC_DOCUMENT_NO.nextval as SNO FROM DUAL";
                var documentno = _dbContext.SqlQuery<Documents>(lcdocument).FirstOrDefault();
                string query = string.Format(@"INSERT INTO LC_DOCUMENT (LC_TRACK_NO,LOC_CODE,SNO,FILE_DETAIL,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',TO_DATE('{7}', 'MM/dd/yyyy'),'{8}')",
          lcimagedetail.LocCode, loccode, documentno.SNO, lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }

        }

        public string RemoveLCImage(LcImageModels lcimage)
        {

            var sqlquery = $@"select FILE_DETAIL from LC_DOCUMENT where LOC_CODE ='{lcimage.LocCode}' AND COMPANY_CODE ='{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();
            string dbpath = result.FILE_DETAIL;
            if (result.FILE_DETAIL != null)
            {
                if (result.FILE_DETAIL.ToString().Contains(':'))
                {
                    result.mylist = result.FILE_DETAIL.Split(':');
                    result.mylist = result.mylist.Where(s => s != lcimage.Path).ToArray();
                    result.FILE_DETAIL = null;
                    string Paths = LcEntryService.ConvertStringArrayToString(result.mylist);
                    Paths = Paths.Remove(Paths.Length - 1);
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE LOC_CODE IN ({1})",
                     Paths, lcimage.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE LOC_CODE IN ({1})",
                   "", lcimage.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE LOC_CODE IN ({1})",
               "", lcimage.LocCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
            return "Success";
        }

        public void SaveStatus(string status, string lctrack)
        {
            string query = string.Format(@"UPDATE LC_LOC SET STATUS_CODE= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5})",
                    status, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lctrack);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
        }

        public LCDetailsViewModels LCDetailList(string lctracknumber)
        {
            var sqlquery = $@"SELECT DISTINCT LP.ORDER_NO,LP.BNF_BANK_CODE,LP.ORDER_DATE,LB.BNF_EDESC,LBB.BANK_EDESC AS BANK_NAME,LLT.TERMS_EDESC AS LC_TERMS_EDESC,LLPT.PTERMS_EDESC AS LC_PTERMS_EDESC,LBB.BANK_ADDRESS AS ADDRESS,LS.STATUS_EDESC,LBB.BANK_ACC_NO,FBS.BANK_NAME AS INTM_BANK_EDESC,LBB.SWIFT_CODE,LBB.BRANCH,LBB.PHONE_NO,LB.ADDRESS,LT.TERMS_EDESC,LPT.PTERMS_EDESC,LP.LC_TRACK_NO,LP.CURRENCY_CODE,LP.MANUAL_NUMBER,LP.TERMS_CODE,LP.PTERMS_CODE,LP.CREDIT_DAYS,LP.VALIDITY_DATE, LP.EST_DELIVERY_DATE,LP.DELIVERY_PLACE_TYPE, LP.DELIVERY_PLACE,LP.APP_NAME,LP.APP_ADDRESS,LP.BILL_COMPANY_NAME,LP.BILL_COMPANY_ADD,
                            LP.BILL_COMPANY_PHONE,LP.SHIP_COMPANY_NAME,AFA.BANK_EDESC AS ADVISING_BANK_EDESC,LP.SHIP_COMPANY_ADD, LP.SHIP_COMPANY_PHONE,LP.TRANSSHIPMENT,LP.CONTACT_NAME,LP.CONTACT_PHONE,LP.CONTACT_EMAIL,
                            LPI.PINVOICE_NO,LPI.PINVOICE_DATE,LPI.BNF_CODE,LPI.BNF_ADDRESS,LPI.BANK_CODE, LPI.BANK_BRANCH,LPI.SWIFT_CODE,LPI.INTM_BANK_CODE,LPI.INTM_SWIFT_CODE,LPI.ACCEPTED_DOC_DATE,
                            LL.LC_NUMBER,LL.OPEN_DATE,LL.EXPIRY_DATE,LL.LEAD_TIME,LL.EXPIRY_PLACE,LL.STATUS_CODE, LL.LAST_SHIPMENT_DATE,LL.TOLERANCE_PER,LL.TERMS_CODE,LL.ADVISING_BANK_CODE,LL.CONFIRM_BANK_CODE,
                            LL.ISSUING_BANK_CODE,LL.PTERMS_CODE,LL.CREDIT_DAYS,LL.PARTIAL_SHIPMENT, LL.CONFIRMATION_REQ,LL.TRANSFERABLE,LL.INSURANCE_FLAG,LL.APP_OUT_CHARGE,LL.BEF_OUT_CHARGE,LL.APP_CONFIRM_CHARGE,
                            LL.BNF_CONFIRM_CHARGE,LL.DOC_REQ_DAYS,LL.ORIGIN_COUNTRY_CODE,CFA.BANK_EDESC AS CONFIRM_BANK_EDESC, IFA.BANK_NAME AS ISSUING_BANK_EDESC
                             FROM LC_PURCHASE_ORDER LP 
                             RIGHT JOIN LC_PERFOMA_INVOICE LPI ON LP.LC_TRACK_NO = LPI.LC_TRACK_NO 
                             LEFT JOIN LC_LOC LL ON  LP.LC_TRACK_NO = LL.LC_TRACK_NO OR LPI.LC_TRACK_NO = LL.LC_TRACK_NO
                             LEFT JOIN LC_BENEFICIARY LB ON LP.BNF_BANK_CODE = LB.BNF_CODE
                             LEFT JOIN LC_BANK LBB ON LPI.BANK_CODE = LBB.BANK_CODE
                             LEFT JOIN FA_BANK_SETUP FBS ON LPI.INTM_BANK_CODE = FBS.BANK_CODE
                             LEFT JOIN LC_TERMS LT ON LP.TERMS_CODE = LT.TERMS_CODE 
                             LEFT JOIN LC_PAYMENT_TERMS LPT ON LP.PTERMS_CODE = LPT.PTERMS_CODE 
                             LEFT JOIN LC_STATUS LS ON LL.STATUS_CODE = LS.STATUS_CODE
                             LEFT JOIN LC_TERMS LLT ON LL.TERMS_CODE = LLT.TERMS_CODE
                             LEFT JOIN LC_PAYMENT_TERMS LLPT ON LL.PTERMS_CODE = LLPT.PTERMS_CODE
                             LEFT JOIN LC_BANK AFA ON LL.ADVISING_BANK_CODE = AFA.BANK_CODE
                              LEFT JOIN LC_BANK CFA ON LL.CONFIRM_BANK_CODE = CFA.BANK_CODE
                              LEFT JOIN FA_BANK_SETUP IFA ON LL.ISSUING_BANK_CODE = IFA.BANK_CODE
                             WHERE LPI.LC_TRACK_NO = '{lctracknumber}' AND LP.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' AND ROWNUM = 1";
            var lclist = _dbContext.SqlQuery<LCDetailsViewModels>(sqlquery).FirstOrDefault();
            return lclist;
        }



        public bool LCNumberExist(int pinvoiceno, string action, int loccode, string lcnumber, out string Message)
        {
            if (action == "create")
            {

                var sqlquery = $@"SELECT DISTINCT PINVOICE_CODE  FROM LC_LOC WHERE pinvoice_code  = '{pinvoiceno}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ";
                var result = _dbContext.SqlQuery<int>(sqlquery).FirstOrDefault();
                if (result > 0)
                {
                    Message = "Perfoma Invoice Number Already Exists.";
                    return true;
                }
                else
                {
                    Message = "";
                    return false;
                }

            }
            else
            {
                var sqlquery = $@"SELECT LC_NUMBER FROM LC_LOC WHERE LC_NUMBER = '{lcnumber}' AND LOC_CODE != '{loccode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ";
                var result2 = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                if (result2 == null)
                {
                    Message = "";
                    return false;
                }
                else
                {
                    Message = "Perfoma Invoice Number Already Exists.";
                    return true;
                }
            }
        }
        public List<SupplierModels> GetAllLcSuppliersFromSynergy(string filter)
        {
            if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

            var sqlquery = $@"select SUPPLIER_CODE
                            ,COALESCE(SUPPLIER_EDESC,' ') SUPPLIER_EDESC
                            from IP_SUPPLIER_SETUP 
                            WHERE DELETED_FLAG='N' 
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            AND (SUPPLIER_CODE LIKE '%{filter.ToUpperInvariant()}%' 
                            or upper(SUPPLIER_EDESC) like '%{filter.ToUpperInvariant()}%')";
            var result = _dbContext.SqlQuery<SupplierModels>(sqlquery).ToList();
            return result;
        }
        public void insertIntoMasterTransaction(string lcNumber, string trackNumber)
        {
            var approvedby = _workcontext.CurrentUserinformation.User_id;
            #region update in lc_loc

            var updateLC_LOC = $@"UPDATE LC_LOC SET APPROVED_BY='{approvedby}',APPROVED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') ,COMPANY_CODE='{_workcontext.CurrentUserinformation
                                                 .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                                 .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LC_TRACK_NO = '{trackNumber}'";
            var lcLogisticPlanItem = _dbContext.ExecuteSqlCommand(updateLC_LOC);
            #endregion


            #region inset int master_transaction
            //var getPInoiceCodeQuery = $@"SELECT PINVOICE_CODE FROM LC_LOC WHERE LC_TRACK_NO='{trackNumber}' AND  LC_NUMBER = '{lcNumber}' ";
            //int pinvoiceCode = _dbContext.SqlQuery<int>(getPInoiceCodeQuery).FirstOrDefault();

            var getPInoiceCodeQuery = $@"SELECT PINVOICE_CODE FROM LC_LOC WHERE LC_TRACK_NO='{trackNumber}'";
            int pinvoiceCode = _dbContext.SqlQuery<int>(getPInoiceCodeQuery).FirstOrDefault();

            var getCurrencyCodeQuery = $@"SELECT CURRENCY_CODE FROM LC_PERFOMA_INVOICE WHERE PINVOICE_CODE='{pinvoiceCode}' ";
            string currencyCode = _dbContext.SqlQuery<string>(getCurrencyCodeQuery).FirstOrDefault();

            //var getLCCurrencyCodeQuery = $@"SELECT SUM(QUANTITY*AMOUNT) AS AMOUNT FROM LC_ITEM WHERE LC_TRACK_NO='{trackNumber}'  GROUP BY '{trackNumber}' ";
            //double amount = _dbContext.SqlQuery<double>(getCurrencyCodeQuery).FirstOrDefault();


            var getLCAmountQuery = $@"SELECT SUM(QUANTITY*AMOUNT) AS AMOUNT FROM LC_ITEM WHERE LC_TRACK_NO='{trackNumber}'  GROUP BY '{trackNumber}' ";
            double amount = _dbContext.SqlQuery<double>(getLCAmountQuery).FirstOrDefault();

            string selectExistingVoucherNumber = $@"Select VOUCHER_NO from MASTER_TRANSACTION";
            List<string> voucherNumber = this._dbContext.SqlQuery<string>(selectExistingVoucherNumber).ToList();
            if (!voucherNumber.Contains(lcNumber))
            {
                var INSERTLC_LOGISTIC_PLAN_QUERY = $@"INSERT INTO MASTER_TRANSACTION (
	                                                VOUCHER_NO
	                                                ,VOUCHER_AMOUNT
                                                    ,CURRENCY_CODE
	                                                ,FORM_CODE
	                                                ,COMPANY_CODE
	                                                ,BRANCH_CODE
	                                                ,CREATED_BY
	                                                ,CREATED_DATE
	                                                )
                                                VALUES (
	                                                '{lcNumber}'
	                                                ,'{amount}'
                                                    ,'{currencyCode}'
	                                                ,'1'
	                                                ,'{_workcontext.CurrentUserinformation.company_code}'
	                                                ,'{_workcontext.CurrentUserinformation.branch_code}'
	                                                ,'{_workcontext.CurrentUserinformation.User_id}'
	                                                ,TO_DATE('{DateTime.Now:MM/dd/yyyy/ HH:mm:ss}', 'mm/dd/yyyy hh24:mi:ss')
	                                                )";
                _dbContext.ExecuteSqlCommand(INSERTLC_LOGISTIC_PLAN_QUERY);
            }
            #endregion



        }
    }
}