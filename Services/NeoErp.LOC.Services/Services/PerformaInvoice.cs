using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.LOC.Services.Services
{
    public class PerformaInvoice : IPerfomaInvoice
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public PerformaInvoice(IDbContext dbContext, IWorkContext workcontext)
        {
            _dbContext = dbContext;
            _workcontext = workcontext;
        }

        public PerformaInvoiceModel CreatePerfomaInvoice(PerformaInvoiceModel perfomadetail)
        {
            if (perfomadetail.PINVOICE_CODE == 0)
            {

                var ordernumber = $@"SELECT ORDER_NO,LC_TRACK_NO,CURRENCY_CODE FROM LC_PURCHASE_ORDER WHERE ORDER_NO = '{perfomadetail.ORDER_NO}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                PerformaInvoiceModel ordernumberexist = _dbContext.SqlQuery<PerformaInvoiceModel>(ordernumber).FirstOrDefault();
                int lctracknumber = 0;

                if (ordernumberexist != null)
                {
                    //SUBIN CHANGES
                    lctracknumber = ordernumberexist.LC_TRACK_NO;
                    var query = $@"SELECT NVL(MAX(SNO + 1),1) FROM LC_ITEM WHERE LC_TRACK_NO = '{perfomadetail.LC_TRACK}'";
                    var itemsno = _dbContext.SqlQuery<int>(query).FirstOrDefault();
                    foreach (var items in perfomadetail.Itemlist)
                    {
                        if (items.ADDED == "Y")
                        {

                            var insertitem = $@"INSERT INTO LC_ITEM(LC_TRACK_NO,SNO,ITEM_CODE,MU_CODE,QUANTITY,AMOUNT,HS_CODE,COUNTRY_OF_ORIGIN,CURRENCY_CODE,                           REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{perfomadetail.LC_TRACK}', '{itemsno}', '{items.ITEM_CODE}','{items.MU_CODE}','{items.QUANTITY}','{items.AMOUNT}','{items.HS_CODE}',
                              '{items.COUNTRY_OF_ORIGIN}','{perfomadetail.CURRENCY_CODE}','{items.REMARKS}','{_workcontext.CurrentUserinformation
                                           .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                                           .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N')";
                            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
                            itemsno++;

                        }
                        else if (items.EDITED == "Y")
                        {
                            var itemupdate = $@"UPDATE LC_ITEM SET ITEM_CODE = '{items.ITEM_CODE}',MU_CODE='{items.MU_CODE.ToUpper()}',QUANTITY='{items.QUANTITY}',AMOUNT='{items.AMOUNT}',HS_CODE='{items.HS_CODE}',COUNTRY_OF_ORIGIN='{items.COUNTRY_OF_ORIGIN}',                           REMARKS='{items.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                                            .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                            .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE DELETED_FLAG = '{'N'}' AND LC_TRACK_NO = '{perfomadetail.LC_TRACK}' AND ITEM_CODE = '{items.ITEM_CODE}'";
                            var insert = _dbContext.ExecuteSqlCommand(itemupdate);
                            _dbContext.SaveChanges();
                        }

                    }
                }
                else
                {
                    var lctrackno = $@"SELECT SEQ_LC_TRACK_NO.nextval as LC_TRACK_NO FROM DUAL";
                    lctracknumber = _dbContext.SqlQuery<int>(lctrackno).FirstOrDefault();
                    var query = $@"SELECT NVL(MAX(SNO + 1),1) FROM LC_ITEM WHERE LC_TRACK_NO = '{lctracknumber}'";
                    var itemsno = _dbContext.SqlQuery<int>(query).FirstOrDefault();
                    foreach (var items in perfomadetail.Itemlist)
                    {
                        try
                        {
                            string insertitem = $@"INSERT INTO LC_ITEM(LC_TRACK_NO,SNO,ITEM_CODE,MU_CODE,QUANTITY,AMOUNT,HS_CODE,COUNTRY_OF_ORIGIN,CURRENCY_CODE,                           REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG) 
                            VALUES('{lctracknumber}', '{itemsno}', '{items.ITEM_CODE}',  '{items.MU_CODE}','{items.QUANTITY}','{items.AMOUNT}','{items.HS_CODE}',
                              '{items.COUNTRY_OF_ORIGIN}','{perfomadetail.CURRENCY_CODE}','{items.REMARKS}','{_workcontext.CurrentUserinformation
                                         .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                                         .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'N')";

                            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
                            itemsno++;
                        }
                        catch (Exception)
                        {

                            throw;
                        }

                    }
                }
                if (lctracknumber == 0)
                {
                    lctracknumber = perfomadetail.LC_TRACK;
                }
                var getpocodebyorderno = $@"SELECT PO_CODE FROM LC_PURCHASE_ORDER WHERE ORDER_NO = '{perfomadetail.ORDER_NO}'";
                var pocode = _dbContext.SqlQuery<int>(getpocodebyorderno).FirstOrDefault();

                var perfomacodequery = $@"SELECT LC_PERFOMA_INVOICE_SEQ.nextval as PINVOICE_CODE FROM DUAL";
                var picode = _dbContext.SqlQuery<PerformaInvoiceModel>(perfomacodequery).FirstOrDefault();
                var insertPI = $@"INSERT INTO LC_PERFOMA_INVOICE(PINVOICE_CODE,PINVOICE_DATE,CURRENCY_CODE,PINVOICE_NO,LC_TRACK_NO,PO_CODE,ORDER_NO,BNF_CODE,BNF_ADDRESS,BANK_CODE,BANK_BRANCH,SWIFT_CODE,INTM_BANK_CODE,INTM_SWIFT_CODE,ACCEPTED_DOC_DATE,REMARKS,COMPANY_CODE, BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{picode.PINVOICE_CODE}',TO_DATE('{perfomadetail.PINVOICE_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{perfomadetail.CURRENCY_CODE}','{perfomadetail.PINVOICE_NO}','{lctracknumber}', '{pocode}','{perfomadetail.ORDER_NO}', '{perfomadetail.BNF_CODE}', '{perfomadetail.BNF_ADDRESS}', '{perfomadetail.BANK_CODE}', '{perfomadetail.BANK_BRANCH}','{perfomadetail.SWIFT_CODE}','{perfomadetail.INTM_BANK_CODE}','{perfomadetail.INTM_SWIFT_CODE}',TO_DATE('{perfomadetail.ACCEPTED_DOC_DATE}','MM/dd/yyyy HH:MI:SS AM'),'{perfomadetail.REMARKS}','{_workcontext.CurrentUserinformation.company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext.CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                var purchaseorder = _dbContext.ExecuteSqlCommand(insertPI);
                PerformaInvoiceModel returnlist = new PerformaInvoiceModel();
                returnlist.LC_TRACK_NO = Convert.ToInt32(perfomadetail.LC_TRACK);
                returnlist.PINVOICE_CODE = picode.PINVOICE_CODE;
                return returnlist;
            }
            else
            {
                var updatetPI = $@"UPDATE LC_PERFOMA_INVOICE SET PINVOICE_NO='{perfomadetail.PINVOICE_NO}',PINVOICE_DATE=TO_DATE('{perfomadetail.PINVOICE_DATE}','MM/dd/yyyy HH:MI:SS AM'),BNF_CODE='{perfomadetail.BNF_CODE}',BNF_ADDRESS='{perfomadetail.BNF_ADDRESS}',BANK_CODE='{perfomadetail.BANK_CODE}', 
                            BANK_BRANCH = '{perfomadetail.BANK_BRANCH}',CURRENCY_CODE='{perfomadetail.CURRENCY_CODE}',ACCEPTED_DOC_DATE=TO_DATE('{perfomadetail.ACCEPTED_DOC_DATE}','MM/dd/yyyy HH:MI:SS AM'),SWIFT_CODE='{perfomadetail.SWIFT_CODE}',INTM_BANK_CODE='{perfomadetail.INTM_BANK_CODE}',INTM_SWIFT_CODE='{perfomadetail.INTM_SWIFT_CODE}',REMARKS='{perfomadetail.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                   .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LC_TRACK_NO = '{perfomadetail.LC_TRACK_NO}' AND PINVOICE_CODE = '{perfomadetail.PINVOICE_CODE}'";
                var updatepinvoice = _dbContext.ExecuteSqlCommand(updatetPI);
                var updatecurrencyonitem = $@"UPDATE LC_ITEM SET CURRENCY_CODE = '{perfomadetail.CURRENCY_CODE}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),LAST_MODIFIED_BY='{_workcontext
                                .CurrentUserinformation.User_id}' WHERE LC_TRACK_NO = '{perfomadetail.LC_TRACK_NO}'";
                var updated = _dbContext.ExecuteSqlCommand(updatecurrencyonitem);
                _dbContext.SaveChanges();
                PerformaInvoiceModel returnlist = new PerformaInvoiceModel();
                returnlist.LC_TRACK_NO = perfomadetail.LC_TRACK_NO;
                returnlist.PINVOICE_CODE = perfomadetail.PINVOICE_CODE;
                return returnlist;
            }
        }

        public List<PerformaInvoiceModel> GetAllLcIpPurchaseOrder(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"SELECT T1.ORDER_NO FROM (SELECT ORDER_NO FROM IP_PURCHASE_ORDER 
                                UNION SELECT ORDER_NO FROM LC_PURCHASE_ORDER WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}') T1
                                where UPPER(T1.ORDER_NO) like '%{filter.ToUpperInvariant()}%'";
                var result = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<PerformaInvoiceModel> getAllIpPurchaseOrderfilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"SELECT T1.ORDER_NO FROM (SELECT ORDER_NO FROM LC_PURCHASE_ORDER 
                                UNION SELECT ORDER_NO FROM LC_PURCHASE_ORDER WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}') T1
                                where UPPER(T1.ORDER_NO) like '%{filter.ToUpperInvariant()}%'";
                var result = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

         public List<PerformaInvoiceModel> getAllIpipPurchaseInvoicefilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"SELECT T1.PINVOICE_NO as PINVOICE_CODE FROM ( SELECT LPI.PINVOICE_NO FROM LC_PERFOMA_INVOICE  LPI 
                                 WHERE DELETED_FLAG='N' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}') T1
                                where UPPER(T1.PINVOICE_NO) like '%{filter.ToUpperInvariant()}%'";
                var result = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<PerformaInvoiceModel> GetAllPerfomaInvoice()
        {
            try
            {
                var sqlquery = $@"SELECT LP.PINVOICE_CODE,LP.ORDER_NO,LP.CURRENCY_CODE,LP.SUPPLIER_CODE,ISS.SUPPLIER_EDESC,LP.PINVOICE_DATE, LP.PINVOICE_NO,LP.LC_TRACK_NO,LP.BNF_CODE,
                    LP.PO_CODE,LBC.BNF_EDESC AS BNF_NAME,LP.BNF_ADDRESS,LP.BANK_CODE,LB.BANK_EDESC AS BANK_NAME,LP.BANK_BRANCH,
                    LP.SWIFT_CODE,LP.INTM_BANK_CODE,LB1.BANK_NAME AS INTM_BANK_EDESC,LP.INTM_SWIFT_CODE,LP.ACCEPTED_DOC_DATE,LD.FILE_DETAIL,LP.REMARKS
                     FROM LC_PERFOMA_INVOICE LP
                    LEFT JOIN LC_BANK LB ON LP.BANK_CODE = LB.BANK_CODE AND LP.COMPANY_CODE = LB.COMPANY_CODE
                    LEFT JOIN FA_BANK_SETUP LB1 ON LP.INTM_BANK_CODE = LB1.BANK_CODE AND LP.COMPANY_CODE = LB1.COMPANY_CODE
                    LEFT JOIN LC_DOCUMENT LD ON LP.PINVOICE_CODE = LD.PINVOICE_CODE AND LD.COMPANY_CODE = LP.COMPANY_CODE
                    LEFT JOIN LC_BENEFICIARY LBC ON LP.BNF_CODE = LBC.BNF_CODE AND LP.COMPANY_CODE = LBC.COMPANY_CODE
                    LEFT JOIN IP_SUPPLIER_SETUP ISS ON LP.SUPPLIER_CODE = ISS.SUPPLIER_CODE AND LP.COMPANY_CODE = ISS.COMPANY_CODE
                    WHERE LP.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LP.CREATED_DATE DESC";

                var result = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).ToList();

                List<PerformaInvoiceModel> newlist = new List<PerformaInvoiceModel>();
                foreach (var item in result)
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
            catch (Exception)
            {
                throw;
            }
        }

        public List<PerformaInvoiceModel> getAllPerfomaInvoiceFilter(string purchaseOrder, string pinvoiceno, string pinvoicedate)
        {
            try
            {
                List<PerformaInvoiceModel> record = new List<PerformaInvoiceModel>();
                if (string.IsNullOrEmpty(purchaseOrder) && string.IsNullOrEmpty(pinvoiceno) && string.IsNullOrEmpty(pinvoicedate))
                {
                    return record;
                }
                else
                {


                    if (purchaseOrder == null)
                    {
                        purchaseOrder = string.Empty;
                    }
                    if (pinvoiceno == null)
                    {
                        pinvoiceno = " ";
                    }
                    if (pinvoicedate == null)
                    {
                        pinvoicedate = string.Empty;
                    }
                }
                var sqlquery = $@"SELECT LP.PINVOICE_CODE,LP.ORDER_NO,LP.CURRENCY_CODE,LP.SUPPLIER_CODE,ISS.SUPPLIER_EDESC,LP.PINVOICE_DATE, LP.PINVOICE_NO,LP.LC_TRACK_NO,LP.BNF_CODE,
                    LP.PO_CODE,LBC.BNF_EDESC AS BNF_NAME,LP.BNF_ADDRESS,LP.BANK_CODE,LB.BANK_EDESC AS BANK_NAME,LP.BANK_BRANCH,
                    LP.SWIFT_CODE,LP.INTM_BANK_CODE,LB1.BANK_NAME AS INTM_BANK_EDESC,LP.INTM_SWIFT_CODE,LP.ACCEPTED_DOC_DATE,LD.FILE_DETAIL,LP.REMARKS
                     FROM LC_PERFOMA_INVOICE LP
                    LEFT JOIN LC_BANK LB ON LP.BANK_CODE = LB.BANK_CODE AND LP.COMPANY_CODE = LB.COMPANY_CODE
                    LEFT JOIN FA_BANK_SETUP LB1 ON LP.INTM_BANK_CODE = LB1.BANK_CODE AND LP.COMPANY_CODE = LB1.COMPANY_CODE
                    LEFT JOIN LC_DOCUMENT LD ON LP.PINVOICE_CODE = LD.PINVOICE_CODE AND LD.COMPANY_CODE = LP.COMPANY_CODE
                    LEFT JOIN LC_BENEFICIARY LBC ON LP.BNF_CODE = LBC.BNF_CODE AND LP.COMPANY_CODE = LBC.COMPANY_CODE
                    LEFT JOIN IP_SUPPLIER_SETUP ISS ON LP.SUPPLIER_CODE = ISS.SUPPLIER_CODE AND LP.COMPANY_CODE = ISS.COMPANY_CODE
                    WHERE ((LP.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}') AND( (LP.ORDER_NO LIKE '% {purchaseOrder} %') OR (LP.PINVOICE_NO LIKE '% {pinvoiceno} %') OR (LP.PINVOICE_DATE=TO_DATE('{pinvoicedate}','mm-dd-yyyy'))))";

                var result = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).ToList();

                List<PerformaInvoiceModel> newlist = new List<PerformaInvoiceModel>();
                foreach (var item in result)
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
            catch (Exception)
            {
                throw;
            }
        }

        public List<ItemDetails> GetAllItemsByTrackOrderNo(string OrderCode)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_PURCHASE_ORDER WHERE ORDER_NO = '{OrderCode}'";
            var lctrack = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).FirstOrDefault();
            int lctrackno = 0;
            if (lctrack != null)
            {
                lctrackno = lctrack.LC_TRACK_NO;
            }
            var sqlquerys = $@"SELECT LI.LC_TRACK_NO,LI.HS_CODE,LI.SNO,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE,LI.QUANTITY AS CALC_QUANTITY,LI.AMOUNT AS CALC_UNIT_PRICE,
            LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE,LI.REMARKS,LP.BNF_BANK_CODE,LB.BNF_EDESC,LB.ADDRESS,LP.CURRENCY_CODE FROM LC_ITEM LI  
            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
            LEFT JOIN LC_PURCHASE_ORDER LP ON LI.LC_TRACK_NO = LP.LC_TRACK_NO
            LEFT JOIN LC_BENEFICIARY LB ON LP.BNF_BANK_CODE = LB.BNF_CODE
               WHERE LI.LC_TRACK_NO = '{lctrackno}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<ItemDetails>(sqlquerys).ToList();
            if (result.Count == 0)
            {
                var lcpurchasesqlquery = $@"SELECT IPO.ORDER_NO, IPO.ITEM_CODE,IMS.ITEM_EDESC,IPO.CALC_QUANTITY,IPO.CALC_UNIT_PRICE,IPO.CALC_TOTAL_PRICE, IPO.MU_CODE,IPO.CURRENCY_CODE FROM IP_PURCHASE_ORDER IPO JOIN IP_ITEM_MASTER_SETUP IMS ON IPO.ITEM_CODE = IMS.ITEM_CODE AND IPO.COMPANY_CODE = IMS.COMPANY_CODE WHERE IPO.ORDER_NO = '{OrderCode}' AND IPO.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                result = _dbContext.SqlQuery<ItemDetails>(lcpurchasesqlquery).ToList();
            }

            return result;

        }

        public void UpdateImage(LcImageModels lcimagedetail, string pinvoicecode)
        {
            var sqlquery = "SELECT FILE_DETAIL FROM LC_DOCUMENT WHERE LC_TRACK_NO  ='" + lcimagedetail.LocCode + "' AND PINVOICE_CODE = '" + pinvoicecode + "'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();

            if (result != null)
            {
                if (result.FILE_DETAIL != null)
                {
                    string dbpath = result.FILE_DETAIL + ":" + lcimagedetail.Path;
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND PINVOICE_CODE IN ({6})",
                   dbpath, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, pinvoicecode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND PINVOICE_CODE IN ({6})",
                   lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, pinvoicecode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                var lcdocument = $@"SELECT SEQ_LC_DOCUMENT_NO.nextval as SNO FROM DUAL";
                var documentno = _dbContext.SqlQuery<Documents>(lcdocument).FirstOrDefault();
                string query = string.Format(@"INSERT INTO LC_DOCUMENT (LC_TRACK_NO,PINVOICE_CODE,SNO,FILE_DETAIL,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',TO_DATE('{7}', 'MM/dd/yyyy'),'{8}')",
          lcimagedetail.LocCode, pinvoicecode, documentno.SNO, lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }

        }

        public string RemovePoImage(LcImageModels lcimage)
        {

            var sqlquery = $@"select FILE_DETAIL from LC_DOCUMENT where PINVOICE_CODE ='{lcimage.LocCode}'";
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
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE PINVOICE_CODE IN ({1})",
                     Paths, lcimage.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE PINVOICE_CODE IN ({1})",
                   "", lcimage.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE PINVOICE_CODE IN ({1})",
               "", lcimage.LocCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
            return "Success";
        }

        public bool CreateItems(List<Items> itemsdetails)
        {
            var query = $@"SELECT NVL(MAX(SNO + 1),1) FROM LC_ITEM WHERE LC_TRACK_NO = '{itemsdetails[0].LC_TRACK_NO}'";
            var itemsno = _dbContext.SqlQuery<int>(query).FirstOrDefault();

            var queryresult = $@"SELECT ITEM_CODE FROM LC_ITEM WHERE LC_TRACK_NO = '{itemsdetails[0].LC_TRACK_NO}'";
            var result = _dbContext.SqlQuery<string>(queryresult).ToList();

            foreach (var item in itemsdetails)
            {
                if (result.Contains(item.ITEM_CODE))
                {
                    return false;
                }
            }

            foreach (var itemsdetail in itemsdetails)
            {
                var insertitem = $@"INSERT INTO LC_ITEM(LC_TRACK_NO,SNO,ITEM_CODE,MU_CODE,QUANTITY,AMOUNT,HS_CODE,COUNTRY_OF_ORIGIN,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{itemsdetail.LC_TRACK_NO}', '{itemsno}', '{itemsdetail.ITEM_CODE}','{itemsdetail.MU_CODE}','{itemsdetail.QUANTITY}', '{itemsdetail.AMOUNT}','{itemsdetail.HS_CODE}',             '{itemsdetail.COUNTRY_EDESC}','{itemsdetail.REMARKS}','{_workcontext.CurrentUserinformation
                                       .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                                       .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);

                //SHIPMENT STARTS HERE//
                //var nextshipmentno = $@"SELECT NVL(MAX(SNO + 1),1) FROM LC_SHIPMENT WHERE LC_TRACK_NO = '{itemsdetail.LC_TRACK_NO}'";
                //var shipmentno = _dbContext.SqlQuery<int>(nextshipmentno).FirstOrDefault();
                //var lcshipment = $@"SELECT LC_TRACK_NO,ITEM_CODE,FROM_LOCATION,TO_LOCATION,SHIPMENT_TYPE,EST_DAY,LOAD_TYPE FROM LC_SHIPMENT WHERE LC_TRACK_NO = '{itemsdetail.LC_TRACK_NO}'";
                //var lcshipmentinfo = _dbContext.SqlQuery<ShipmentModels>(lcshipment).FirstOrDefault();

                //var insertshipment = $@"INSERT INTO LC_SHIPMENT(LC_TRACK_NO,SNO,FROM_LOCATION,TO_LOCATION,SHIPMENT_TYPE, LOAD_TYPE,EST_DAY,ITEM_CODE,QUANTITY,MU_CODE,COMPANY_CODE,BRANCH_CODE, CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{itemsdetail.LC_TRACK_NO}', '{shipmentno}','{lcshipmentinfo.FROM_LOCATION}','{lcshipmentinfo.TO_LOCATION}', '{lcshipmentinfo.SHIPMENT_TYPE}', '{lcshipmentinfo.LOAD_TYPE}', '{lcshipmentinfo.EST_DAY}','{itemsdetail.ITEM_CODE}','{itemsdetail.QUANTITY}','{itemsdetail.MU_CODE}','{_workcontext.CurrentUserinformation
                //                    .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                //                    .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                //var shipment = _dbContext.ExecuteSqlCommand(insertshipment);
                //ENDS HERE//

            }
            return true;
        }

        public bool UpdateItems(List<Items> itemsdetails)
        {
            foreach (var itemsdetail in itemsdetails)
            {

                var queryresult = $@"SELECT ITEM_CODE FROM LC_ITEM WHERE  ITEM_CODE ={itemsdetail.ITEM_CODE}and SNO!={itemsdetail.SNO} and LC_TRACK_NO={itemsdetail.LC_TRACK_NO} AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<string>(queryresult).ToList();
                if (result.Contains(itemsdetail.ITEM_CODE))
                {
                    return false;
                }
            }


            foreach (var itemsdetail in itemsdetails)
            {

                var itemupdate = $@"UPDATE LC_ITEM SET ITEM_CODE = '{itemsdetail.ITEM_CODE}',MU_CODE='{itemsdetail.MU_CODE.ToUpper()}',QUANTITY='{itemsdetail.QUANTITY}',AMOUNT='{itemsdetail.AMOUNT}',HS_CODE='{itemsdetail.HS_CODE}',COUNTRY_OF_ORIGIN='{itemsdetail.COUNTRY_OF_ORIGIN}',                           REMARKS='{itemsdetail.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE DELETED_FLAG = '{'N'}' AND LC_TRACK_NO = '{itemsdetail.LC_TRACK_NO}' AND SNO = '{itemsdetail.SNO}'";
                var insert = _dbContext.ExecuteSqlCommand(itemupdate);
                _dbContext.SaveChanges();

            }
            return true;
        }



        public void deletePI(string lctrack, string sno)
        {
            string perfomainvoice = string.Format(@"UPDATE LC_PERFOMA_INVOICE SET DELETED_FLAG= 'Y' WHERE PINVOICE_CODE = {0} AND LC_TRACK_NO = {1}",
                 sno, lctrack);
            var rowCount = _dbContext.ExecuteSqlCommand(perfomainvoice);
            string items = string.Format(@"UPDATE LC_ITEM SET DELETED_FLAG= 'Y' WHERE LC_TRACK_NO = {0}",
                 lctrack);
            var resultitems = _dbContext.ExecuteSqlCommand(items);
            string shipment = string.Format(@"UPDATE LC_SHIPMENT SET DELETED_FLAG= 'Y' WHERE LC_TRACK_NO = {0}",
                 lctrack);
            var resultshipment = _dbContext.ExecuteSqlCommand(shipment);

        }

        public bool ProformaNumberExist(string pinvoiceno, string action, int pinvoicecode, string orderno, out string Message)
        {
            if (action == "create")
            {
                var sqlquery = $@"SELECT PINVOICE_NO FROM LC_PERFOMA_INVOICE WHERE PINVOICE_NO = '{pinvoiceno}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                var sqlquery1 = $@"SELECT ORDER_NO FROM LC_PERFOMA_INVOICE WHERE ORDER_NO = '{orderno}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var ordernumber = _dbContext.SqlQuery<string>(sqlquery1).FirstOrDefault();
                if (result == null)
                {
                    if (ordernumber != null)
                    {
                        Message = "The Particular Order Number has been already created.";
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
                    Message = "Perfoma Invoice Number Already Exists.";
                    return true;
                }
            }
            else
            {
                var sqlquery = $@"SELECT PINVOICE_NO FROM LC_PERFOMA_INVOICE WHERE PINVOICE_NO = '{pinvoiceno}' AND PINVOICE_CODE != '{pinvoicecode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                if (result == null)
                {
                    Message = "Perfoma Invoice Number Already Exists.";
                    return false;
                }
                else
                {
                    Message = "";
                    return true;
                }
            }
        }

        public List<PendingItemModels> PendingItemsList()
        {
            var sqlquery = $@"SELECT LI.LC_TRACK_NO,LI.QUANTITY,LI.ITEM_CODE,IMS.ITEM_EDESC,LIN.QUANTITY AS INVOICE_QUANTITY,LI.MU_CODE from LC_ITEM LI
            LEFT JOIN LC_INVOICE LIN ON LI.LC_TRACK_NO = LIN.LC_TRACK_NO AND LI.ITEM_CODE = LIN.ITEM_CODE AND LI.SNO = LIN.SNO
            LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
            AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
            ORDER BY LI.QUANTITY DESC";
            var result = _dbContext.SqlQuery<PendingItemModels>(sqlquery).ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (result[i].QUANTITY == result[i].INVOICE_QUANTITY)
                {
                    result.RemoveAt(i);
                }
            }

            var gdf = result.Take(5).ToList();
            return gdf; ;
        }
    }
}