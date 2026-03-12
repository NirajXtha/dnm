using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.LOC.Services.Models;
using NeoErp.Data;
using NeoErp.Core;

namespace NeoErp.LOC.Services.Services
{
    public class PurchaseOrder : IPurchaseOrder
    {
        private IDbContext _dbContext;
        private IWorkContext _workcontext;
        public PurchaseOrder(IDbContext dbContext, IWorkContext workcontext)
        {
            _dbContext = dbContext;
            _workcontext = workcontext;
        }

        public bool CreateItems(List<Items> itemsdetails)
        {
            var query = $@"SELECT NVL(MAX(SNO + 1),1) AS SNO,CURRENCY_CODE FROM LC_ITEM WHERE LC_TRACK_NO = '{itemsdetails[0].LC_TRACK_NO}' GROUP BY CURRENCY_CODE";

            var queryresult = $@"SELECT ITEM_CODE FROM LC_ITEM WHERE LC_TRACK_NO = '{itemsdetails[0].LC_TRACK_NO}'";
            var result = _dbContext.SqlQuery<string>(queryresult).ToList();

            foreach (var item in itemsdetails)
            {
                if (result.Contains(item.ITEM_CODE))
                {
                    return false;
                }
            }

            var itemsno = _dbContext.SqlQuery<Items>(query).FirstOrDefault();
            foreach (var itemsdetail in itemsdetails)
            {
                var insertitem = $@"INSERT INTO LC_ITEM(LC_TRACK_NO,SNO,CURRENCY_CODE,ITEM_CODE,MU_CODE,QUANTITY,AMOUNT,HS_CODE,COUNTRY_OF_ORIGIN,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{itemsdetail.LC_TRACK_NO}', '{itemsno.SNO}','{itemsno.CURRENCY_CODE}', '{itemsdetail.ITEM_CODE}','{itemsdetail.MU_CODE}','{itemsdetail.QUANTITY}', '{itemsdetail.AMOUNT}','{itemsdetail.HS_CODE}',             '{itemsdetail.COUNTRY_CODE}','{itemsdetail.REMARKS}','{_workcontext.CurrentUserinformation
                                       .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                                       .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);



            }
            return true;
        }

        public bool UpdateItems(List<Items> itemsdetails)
        {
            foreach (var itemsdetail in itemsdetails)
            {

                var queryresult = $@"SELECT ITEM_CODE FROM LC_ITEM WHERE  ITEM_CODE ={itemsdetail.ITEM_CODE}and SNO!={itemsdetail.SNO} and LC_TRACK_NO={itemsdetail.LC_TRACK_NO}";
                var result = _dbContext.SqlQuery<string>(queryresult).ToList();
                if (result.Contains(itemsdetail.ITEM_CODE))
                {
                    return false;
                }
            }

            foreach (var itemsdetail in itemsdetails)
            {


                var itemupdate = $@"UPDATE LC_ITEM SET ITEM_CODE = '{itemsdetail.ITEM_CODE}',MU_CODE='{itemsdetail.MU_CODE.ToUpper()}',QUANTITY='{itemsdetail.QUANTITY}',AMOUNT='{itemsdetail.AMOUNT}',HS_CODE='{itemsdetail.HS_CODE}',COUNTRY_OF_ORIGIN='{itemsdetail.COUNTRY_CODE}',                           REMARKS='{itemsdetail.REMARKS}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                                 .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                                 .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE DELETED_FLAG = '{'N'}' AND LC_TRACK_NO = '{itemsdetail.LC_TRACK_NO}' AND SNO = '{itemsdetail.SNO}'";
                var insert = _dbContext.ExecuteSqlCommand(itemupdate);
                _dbContext.SaveChanges();

                //var shipmentupdate = $@"UPDATE LC_SHIPMENT SET ITEM_CODE = '{itemsdetail.ITEM_CODE}',MU_CODE='{itemsdetail.MU_CODE.ToUpper()}',QUANTITY='{itemsdetail.QUANTITY}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                //                .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                //                .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy') WHERE LC_TRACK_NO = '{itemsdetail.LC_TRACK_NO}' AND SNO = '{itemsdetail.SNO}' AND ITEM_CODE = '{}'";
                //var updateshipment = _dbContext.ExecuteSqlCommand(shipmentupdate);
                //_dbContext.SaveChanges();

            }
            return true;
        }



        public PurchaseOrderModels CreatePurchaseOrder(PurchaseOrderModels data)
        {

            if (data.LC_TRACK_NO == 0)
            {
                var lctrackno = $@"SELECT SEQ_LC_TRACK_NO.nextval as LC_TRACK_NO FROM DUAL";
                var nextValQuery = $@"SELECT SEQ_LC_PURCHASE_CODE.nextval as PO_NUMBER FROM DUAL";
                var pocode = _dbContext.SqlQuery<PurchaseOrderModels>(nextValQuery).FirstOrDefault();
                var trackno = _dbContext.SqlQuery<PurchaseOrderModels>(lctrackno).FirstOrDefault();
                var insertPO = $@"INSERT INTO LC_PURCHASE_ORDER(PO_CODE,ORDER_NO,LEAD_TIME,BNF_BANK_CODE,ORDER_DATE,LC_TRACK_NO,MANUAL_NUMBER,TERMS_CODE,PTERMS_CODE, 
                            CREDIT_DAYS,VALIDITY_DATE,TRANSSHIPMENT,CURRENCY_CODE, EST_DELIVERY_DATE,DELIVERY_PLACE_TYPE,DELIVERY_PLACE,APP_NAME,APP_ADDRESS,BILL_COMPANY_NAME,BILL_COMPANY_ADD,BILL_COMPANY_PHONE,SHIP_COMPANY_NAME,SHIP_COMPANY_ADD,SHIP_COMPANY_PHONE,CONTACT_NAME,CONTACT_PHONE,CONTACT_EMAIL,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,WEEK_NUMBER)VALUES('{pocode.PO_NUMBER}', '{data.ORDER_NO}','{data.LEAD_TIME}','{data.BNF_BANK_CODE}',TO_DATE('{data.ORDER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'), '{trackno.LC_TRACK_NO}', '{data.MANUAL_NUMBER}', '{data.TERMS_CODE}', '{data.PTERMS_CODE}', '{data.CREDIT_DAYS}',TO_DATE('{data.VALIDITY_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{data.TRANSSHIPMENT}','{data.CURRENCY_CODE}',TO_DATE('{data.EST_DELIVERY_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{data.DELIVERY_PLACE_TYPE}','{data.DELIVERY_PLACE}',
                              '{data.APP_NAME}','{data.APP_ADDRESS}','{data.BILL_COMPANY_NAME}', '{data.BILL_COMPANY_ADD}','{data.BILL_COMPANY_PHONE}','{data.SHIP_COMPANY_NAME}','{data.SHIP_COMPANY_ADD}','{data.SHIP_COMPANY_PHONE}','{data.CONTACT_NAME}','{data.CONTACT_PHONE}','{data.CONTACT_EMAIL}','{data.REMARKS}','{_workcontext.CurrentUserinformation
                                    .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                                    .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}','{data.WEEK_NUMBER}')";
                var purchaseorder = _dbContext.ExecuteSqlCommand(insertPO);
                var query = $@"SELECT NVL(MAX(SNO + 1),1) FROM LC_ITEM WHERE LC_TRACK_NO = '{trackno.LC_TRACK_NO}'";
                int itemsno = _dbContext.SqlQuery<int>(query).FirstOrDefault();
                foreach (var items in data.Itemlist)
                {

                    var insertitem = $@"INSERT INTO LC_ITEM(LC_TRACK_NO,SNO,ITEM_CODE,MU_CODE,QUANTITY,AMOUNT,HS_CODE,COUNTRY_OF_ORIGIN,CURRENCY_CODE,                           REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{trackno.LC_TRACK_NO}', '{itemsno}', '{items.ITEM_CODE}',  '{items.MU_CODE}','{items.QUANTITY}','{items.AMOUNT}','{items.HS_CODE}',
                              '{items.COUNTRY_OF_ORIGIN}','{data.CURRENCY_CODE}','{items.REMARKS}','{_workcontext.CurrentUserinformation
                                   .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
                                   .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
                    var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
                    itemsno++;
                }
                PurchaseOrderModels returnlist = new PurchaseOrderModels();
                returnlist.LC_TRACK_NO = trackno.LC_TRACK_NO;
                returnlist.PO_CODE = pocode.PO_NUMBER;
                return returnlist;
            }
            else
            {
                //var condition = "";
                //if (!string.IsNullOrEmpty(data.CURRENCY_CODE))
                //{
                //    condition = $@",CURRENCY_CODE = '{data.CURRENCY_CODE}'";
                //}

                var updatetPO = $@"UPDATE LC_PURCHASE_ORDER SET ORDER_NO = '{data.ORDER_NO}',BNF_BANK_CODE='{data.BNF_BANK_CODE}',CURRENCY_CODE='{data.CURRENCY_CODE}',ORDER_DATE=TO_DATE('{data.ORDER_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),MANUAL_NUMBER='{data.MANUAL_NUMBER}',TERMS_CODE='{data.TERMS_CODE}',PTERMS_CODE='{data.PTERMS_CODE}', 
                            CREDIT_DAYS = '{data.CREDIT_DAYS}',TRANSSHIPMENT='{data.TRANSSHIPMENT}',VALIDITY_DATE=TO_DATE('{data.VALIDITY_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'), EST_DELIVERY_DATE=TO_DATE('{data.EST_DELIVERY_DATE.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),DELIVERY_PLACE_TYPE='{data.DELIVERY_PLACE_TYPE}',DELIVERY_PLACE='{data.DELIVERY_PLACE}',APP_NAME='{data.APP_NAME}',APP_ADDRESS='{data.APP_ADDRESS}',BILL_COMPANY_NAME='{data.BILL_COMPANY_NAME}',BILL_COMPANY_ADD='{data.BILL_COMPANY_ADD}',BILL_COMPANY_PHONE='{data.BILL_COMPANY_PHONE}',SHIP_COMPANY_NAME='{data.SHIP_COMPANY_NAME}',SHIP_COMPANY_ADD='{data.SHIP_COMPANY_ADD}',SHIP_COMPANY_PHONE='{data.SHIP_COMPANY_PHONE}',CONTACT_NAME='{data.CONTACT_NAME}',CONTACT_PHONE='{data.CONTACT_PHONE}',CONTACT_EMAIL='{data.CONTACT_EMAIL}',REMARKS='{data.REMARKS}',LEAD_TIME='{data.LEAD_TIME}',COMPANY_CODE='{_workcontext.CurrentUserinformation
                             .company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
                             .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),WEEK_NUMBER='{data.WEEK_NUMBER}' WHERE LC_TRACK_NO = '{data.LC_TRACK_NO}' AND PO_CODE = '{data.PO_CODE}'";
                var updatepurchaseorder = _dbContext.ExecuteSqlCommand(updatetPO);

                if (!string.IsNullOrEmpty(data.CURRENCY_CODE))
                {
                    var updatecurrencyonitem = $@"UPDATE LC_ITEM SET CURRENCY_CODE = '{data.CURRENCY_CODE}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),LAST_MODIFIED_BY='{_workcontext
                                 .CurrentUserinformation.User_id}' WHERE LC_TRACK_NO = '{data.LC_TRACK_NO}'";
                    var updated = _dbContext.ExecuteSqlCommand(updatecurrencyonitem);
                    _dbContext.SaveChanges();
                }
                PurchaseOrderModels returnlist = new PurchaseOrderModels();
                returnlist.LC_TRACK_NO = data.LC_TRACK_NO;
                returnlist.PO_CODE = data.PO_CODE;
                return returnlist;
            }
        }

        public List<ItemDetail> GetAllItemsLists(string filter)
        {

            try
            {
                if (string.IsNullOrEmpty(filter))
                {
                    filter = string.Empty;
                    return new List<ItemDetail>();
                }

                var sqlquery = $@"select distinct
                             COALESCE(ITEM_CODE,' ') ITEM_CODE
                            ,COALESCE(ITEM_EDESC,' ') ITEM_EDESC,
                            INDEX_MU_CODE
                            from IP_ITEM_MASTER_SETUP 
                            WHERE DELETED_FLAG='N'
                            AND GROUP_SKU_FLAG = 'I'
                            AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'
                            AND (ITEM_CODE LIKE '%{filter.ToUpperInvariant()}%' 
                            OR UPPER(ITEM_EDESC) LIKE '%{filter.ToUpperInvariant()}%')";
                var result = _dbContext.SqlQuery<ItemDetail>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        public List<ItemDetail> GetAllMuCodeLists(string filter, string ItemCode)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"SELECT ITEM_CODE,MU_CODE,CONVERSION_FACTOR,FRACTION FROM IP_ITEM_UNIT_SETUP
                  where deleted_flag='N'
                   AND ITEM_CODE = '{ItemCode}'
                  and (ITEM_CODE like '%{filter.ToUpperInvariant()}%' 
                  or upper(MU_CODE) like '%{filter.ToUpperInvariant()}%' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}')";
                var result = _dbContext.SqlQuery<ItemDetail>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }



        public List<Items> GetAllPOItemsLists(string filter)
        {
            var sqlquery = $@"SELECT LI.LC_TRACK_NO,LI.SNO,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE,LI.QUANTITY,LI.AMOUNT,LI.HS_CODE,LI.COUNTRY_OF_ORIGIN,LI.COUNTRY_OF_ORIGIN AS COUNTRY_CODE,CS.COUNTRY_EDESC FROM LC_ITEM LI
                LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE
                LEFT JOIN COUNTRY_SETUP CS ON LI.COUNTRY_OF_ORIGIN = CS.COUNTRY_CODE AND LI.COMPANY_CODE = CS.COMPANY_CODE WHERE LI.LC_TRACK_NO = '{filter}' AND LI.DELETED_FLAG = 'N' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY LI.SNO ASC";

            var result = _dbContext.SqlQuery<Items>(sqlquery).ToList();
            return result;
        }

        public List<ShipmentModels> GetAllPOShipmentLists(string filter)
        {
            var sqlquery = $@"SELECT LS.LC_TRACK_NO,LS.SNO,LS.QUANTITY,LS.MU_CODE,LS.ITEM_CODE,IMS.ITEM_EDESC,LS.FROM_LOCATION,LS.TO_LOCATION,
                            LS.SHIPMENT_TYPE,LS.LOAD_TYPE,LS.EST_DAY FROM LC_SHIPMENT LS
                            JOIN IP_ITEM_MASTER_SETUP IMS ON LS.ITEM_CODE = IMS.ITEM_CODE WHERE LS.LC_TRACK_NO = '{filter}' AND LS.DELETED_FLAG = 'N' ORDER BY LS.SNO ASC";

            var result = _dbContext.SqlQuery<ShipmentModels>(sqlquery).ToList();
            return result;
        }

        public List<PurchaseOrderModels> GetAllPurchaseOrders()
        {
            try
            {
                var sqlquery = $@"SELECT PO.PO_CODE as PO_CODE,PO.ORDER_NO as ORDER_NO,PO.LEAD_TIME as LEAD_TIME,PO.BNF_BANK_CODE as BNF_BANK_CODE,LC.BNF_EDESC as BNF_EDESC,PO.ORDER_DATE as ORDER_DATE,PO.LC_TRACK_NO as LC_TRACK_NO,PO.MANUAL_NUMBER as MANUAL_NUMBER,PO.TERMS_CODE as TERMS_CODE,LT.TERMS_EDESC as TERMS_EDESC,PO.PTERMS_CODE as PTERMS_CODE,LP.PTERMS_EDESC as PTERMS_EDESC,PO.CREDIT_DAYS as CREDIT_DAYS,PO.VALIDITY_DATE as VALIDITY_DATE,LD.FILE_DETAIL as FILE_DETAIL,PO.TRANSSHIPMENT  as TRANSSHIPMENT
                    ,PO.EST_DELIVERY_DATE as EST_DELIVERY_DATE,PO.DELIVERY_PLACE_TYPE as DELIVERY_PLACE_TYPE, PO.DELIVERY_PLACE as DELIVERY_PLACE,PO.APP_NAME as APP_NAME,PO.APP_ADDRESS as APP_ADDRESS, PO.BILL_COMPANY_NAME as BILL_COMPANY_NAME,
                     PO.BILL_COMPANY_ADD as BILL_COMPANY_ADD,PO.BILL_COMPANY_PHONE as BILL_COMPANY_PHONE,
                        PO.SHIP_COMPANY_NAME as SHIP_COMPANY_NAME, PO.SHIP_COMPANY_ADD as SHIP_COMPANY_ADD, PO.SHIP_COMPANY_PHONE as SHIP_COMPANY_PHONE, PO.CONTACT_NAME as CONTACT_NAME,PO.CONTACT_PHONE as CONTACT_PHONE,
                        PO.CONTACT_EMAIL as CONTACT_EMAIL,PO.REMARKS as REMARKS,PO.CURRENCY_CODE as CURRENCY_CODE,WEEK_NUMBER FROM LC_PURCHASE_ORDER PO
                        LEFT JOIN LC_BENEFICIARY LC ON PO.BNF_BANK_CODE = LC.BNF_CODE AND PO.COMPANY_CODE= LC.COMPANY_CODE
                        LEFT JOIN LC_TERMS LT ON PO.TERMS_CODE = LT.TERMS_CODE AND PO.COMPANY_CODE= LT.COMPANY_CODE
                        LEFT JOIN LC_PAYMENT_TERMS LP ON PO.PTERMS_CODE = LP.PTERMS_CODE  AND PO.COMPANY_CODE= LP.COMPANY_CODE
                        LEFT JOIN LC_DOCUMENT LD ON PO.PO_CODE = LD.PO_CODE WHERE PO.DELETED_FLAG= 'N' AND PO.COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}'ORDER BY PO.CREATED_DATE DESC";

                var result = _dbContext.SqlQuery<PurchaseOrderModels>(sqlquery).ToList();
                //var sqlquerydocument = $@"SELECT * FROM LC_PURCHASE_ORDER";

                //var documentlist = _dbContext.SqlQuery<Documents>(sqlquerydocument).ToList();
                List<PurchaseOrderModels> newlist = new List<PurchaseOrderModels>();
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

        public List<PurchaseOrderModels> getAllPurchaseOrdersFilter(string purchaseOrder, string beneficiaryname, string orderdate)
        {
            try
            {
                List<PurchaseOrderModels> record = new List<PurchaseOrderModels>();
                if (string.IsNullOrEmpty(purchaseOrder) && string.IsNullOrEmpty(beneficiaryname) && string.IsNullOrEmpty(orderdate))
                {
                    return record;
                }
                else
                {
                    if (purchaseOrder == null)
                    {
                        purchaseOrder = string.Empty;
                    }
                    if (beneficiaryname == null)
                    {
                        beneficiaryname = string.Empty;
                    }
                    if (orderdate == null || orderdate=="undefined")
                    {
                        orderdate = string.Empty;
                    }
                  
                }
                var sqlquery = $@"SELECT PO.PO_CODE,PO.ORDER_NO,PO.LEAD_TIME,PO.BNF_BANK_CODE,LC.BNF_EDESC,PO.ORDER_DATE,PO.LC_TRACK_NO,PO.MANUAL_NUMBER,PO.TERMS_CODE,LT.TERMS_EDESC,PO.PTERMS_CODE,LP.PTERMS_EDESC,PO.CREDIT_DAYS,PO.VALIDITY_DATE,LD.FILE_DETAIL,PO.TRANSSHIPMENT
                    ,PO.EST_DELIVERY_DATE,PO.DELIVERY_PLACE_TYPE, PO.DELIVERY_PLACE,PO.APP_NAME,PO.APP_ADDRESS, PO.BILL_COMPANY_NAME,
                     PO.BILL_COMPANY_ADD,PO.BILL_COMPANY_PHONE,
                        PO.SHIP_COMPANY_NAME, PO.SHIP_COMPANY_ADD, PO.SHIP_COMPANY_PHONE, PO.CONTACT_NAME,PO.CONTACT_PHONE,
                        PO.CONTACT_EMAIL,PO.REMARKS,PO.CURRENCY_CODE FROM LC_PURCHASE_ORDER PO
                        LEFT JOIN LC_BENEFICIARY LC ON PO.BNF_BANK_CODE = LC.BNF_CODE
                        LEFT JOIN LC_TERMS LT ON PO.TERMS_CODE = LT.TERMS_CODE
                        LEFT JOIN LC_PAYMENT_TERMS LP ON PO.PTERMS_CODE = LP.PTERMS_CODE
                        LEFT JOIN LC_DOCUMENT LD ON PO.PO_CODE = LD.PO_CODE WHERE ((PO.DELETED_FLAG= 'N' ) AND (PO.COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}')AND( (PO.ORDER_NO LIKE'%{purchaseOrder}%')OR (LC.BNF_EDESC ='{beneficiaryname}')OR (PO.ORDER_DATE=TO_DATE('{orderdate}','mm-dd-yyyy'))))";
                var result = _dbContext.SqlQuery<PurchaseOrderModels>(sqlquery).ToList();
                //var sqlquerydocument = $@"SELECT * FROM LC_PURCHASE_ORDER";

                //var documentlist = _dbContext.SqlQuery<Documents>(sqlquerydocument).ToList();
                List<PurchaseOrderModels> newlist = new List<PurchaseOrderModels>();
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
            catch (Exception ex)
            {
                throw;
            }
        }

        public void UpdateImage(LcImageModels lcimagedetail, string purchaseorder)
        {
            var sqlquery = $@"SELECT FILE_DETAIL FROM LC_DOCUMENT WHERE LC_TRACK_NO  ='{lcimagedetail.LocCode}' AND PO_CODE = '{purchaseorder}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<Documents>(sqlquery).FirstOrDefault();

            if (result != null)
            {
                if (result.FILE_DETAIL != null)
                {

                    string dbpath = result.FILE_DETAIL + ":" + lcimagedetail.Path;
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND PO_CODE IN ({6})",
                   dbpath, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, purchaseorder);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL= '{0}',COMPANY_CODE='{1}',BRANCH_CODE='{2}',LAST_MODIFIED_BY='{3}',LAST_MODIFIED_DATE=TO_DATE('{4}', 'MM/dd/yyyy') WHERE LC_TRACK_NO IN ({5}) AND PO_CODE IN ({6})",
                   lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), lcimagedetail.LocCode, purchaseorder);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                var lcdocument = $@"SELECT SEQ_LC_DOCUMENT_NO.nextval as SNO FROM DUAL";
                var documentno = _dbContext.SqlQuery<Documents>(lcdocument).FirstOrDefault();
                string query = string.Format(@"INSERT INTO LC_DOCUMENT (LC_TRACK_NO,PO_CODE,SNO,FILE_DETAIL,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}',TO_DATE('{7}', 'MM/dd/yyyy'),'{8}')",
          lcimagedetail.LocCode, purchaseorder, documentno.SNO, lcimagedetail.Path, _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy"), 'N');
                var rowCount = _dbContext.ExecuteSqlCommand(query);

            }

        }



        public List<IpPurchaseOrderModels> GetAllIpPurchaseOrders(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"SELECT DISTINCT ORDER_NO FROM IP_PURCHASE_ORDER 
                            where deleted_flag='N'
                            and upper(ORDER_NO) like '%{filter.ToUpperInvariant()}%'  AND UPPER(ORDER_NO) NOT LIKE '%LOC%' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<IpPurchaseOrderModels>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public List<IpPurchaseOrderModels> GetAllIpPurchaseOrdersfilter(string filter)
        {
            try
            {
                if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

                var sqlquery = $@"select ORDER_NO from LC_PURCHASE_ORDER
                            where deleted_flag='N'
                            and upper(ORDER_NO) like '%{filter.ToUpperInvariant()}%' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<IpPurchaseOrderModels>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string RemovePoImage(LcImageModels lcimage)
        {

            var sqlquery = $@"select FILE_DETAIL from LC_DOCUMENT where PO_CODE ='{lcimage.LocCode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
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
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE PO_CODE IN ({1})",
                     Paths, lcimage.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
                else
                {
                    string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE PO_CODE IN ({1})",
                   "", lcimage.LocCode);
                    var rowCount = _dbContext.ExecuteSqlCommand(query);
                }
            }
            else
            {
                string query = string.Format(@"UPDATE LC_DOCUMENT SET FILE_DETAIL  = '{0}' WHERE PO_CODE IN ({1})",
               "", lcimage.LocCode);
                var rowCount = _dbContext.ExecuteSqlCommand(query);
            }
            return "Success";
        }

        public List<ItemDetails> GetAllItemsByTrackOrderNo(string OrderCode)
        {
            var sqlquery = $@"SELECT LC_TRACK_NO FROM LC_PURCHASE_ORDER WHERE ORDER_NO = '{OrderCode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var lctrack = _dbContext.SqlQuery<PerformaInvoiceModel>(sqlquery).FirstOrDefault();
            int lctrackno = 0;
            if (lctrack != null)
            {
                lctrackno = lctrack.LC_TRACK_NO;
            }
            var sqlquerys = $@"SELECT LI.LC_TRACK_NO,LI.SNO,LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE,LI.QUANTITY,LI.AMOUNT,LI.COUNTRY_OF_ORIGIN,LI.REMARKS FROM LC_ITEM LI
               JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE AND LI.COMPANY_CODE = IMS.COMPANY_CODE WHERE LI.LC_TRACK_NO = '{lctrackno}' AND LI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
            var result = _dbContext.SqlQuery<ItemDetails>(sqlquerys).ToList();
            return result;

        }

        public List<Currency> GetAllCurrency()
        {
            try
            {
                var sqlquery = $@"SELECT DISTINCT CURRENCY_CODE, CURRENCY_EDESC FROM CURRENCY_SETUP WHERE DELETED_FLAG = 'N' AND COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}' ";
                var result = _dbContext.SqlQuery<Currency>(sqlquery).ToList();
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void deletePO(string lctrack, string sno)
        {
            string purchaseorder = string.Format(@"UPDATE LC_PURCHASE_ORDER SET DELETED_FLAG= 'Y' WHERE PO_CODE = {0} AND LC_TRACK_NO = {1}",
                 sno, lctrack);
            var rowCount = _dbContext.ExecuteSqlCommand(purchaseorder);
            string items = string.Format(@"UPDATE LC_ITEM SET DELETED_FLAG= 'Y' WHERE LC_TRACK_NO = {0}",
                 lctrack);
            var resultitems = _dbContext.ExecuteSqlCommand(items);
            string shipment = string.Format(@"UPDATE LC_SHIPMENT SET DELETED_FLAG= 'Y' WHERE LC_TRACK_NO = {0}",
                 lctrack);
            var resultshipment = _dbContext.ExecuteSqlCommand(shipment);
        }

        public List<ItemHistoryModels> GetAllPOHistoryItemsLists(string filter)
        {
            var sqlquery = $@"SELECT ALI.VERSION_CODE,ALI.LC_TRACK_NO,ALI.SNO,ALI.ITEM_CODE,IMS.ITEM_EDESC,ALI.MU_CODE,ALI.QUANTITY,ALI.CURRENCY_CODE,ALI.AMOUNT,ALI.HS_CODE,ALI.COUNTRY_OF_ORIGIN,
                ALI.CREATED_DATE,ALI.CREATED_BY,SAU.LOGIN_EDESC AS CREATED_BY_EDESC,ALI.LAST_MODIFIED_BY,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC,ALI.LAST_MODIFIED_DATE AS LAST_MODIFIED_DATE,ALI.REMARKS
                FROM AUD_LC_ITEM ALI 
                LEFT JOIN SC_APPLICATION_USERS  SAU ON ALI.CREATED_BY  = SAU.USER_NO AND ALI.COMPANY_CODE = SAU.COMPANY_CODE
                LEFT JOIN SC_APPLICATION_USERS  SA ON ALI.LAST_MODIFIED_BY = SA.USER_NO AND ALI.COMPANY_CODE = SA.COMPANY_CODE
                LEFT JOIN IP_ITEM_MASTER_SETUP IMS ON ALI.ITEM_CODE = IMS.ITEM_CODE AND ALI.COMPANY_CODE = IMS.COMPANY_CODE WHERE ALI.LC_TRACK_NO = '{filter}' AND ALI.DELETED_FLAG = 'N' AND ALI.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}' ORDER BY ALI.SNO ASC";

            var result = _dbContext.SqlQuery<ItemHistoryModels>(sqlquery).ToList();
            //result.Any(x => x.LAST_MODIFIED_DATE == null ? "" : x.LAST_MODIFIED_DATE);
            return result;
        }

        public List<ShipmentHistoryModels> GetAllPOHistoryShipmentList(string filter)
        {
            var sqlquery = $@"SELECT ALS.VERSION_CODE,ALS.LC_TRACK_NO,ALS.SNO,ALS.ITEM_CODE,IMS.ITEM_EDESC,ALS.QUANTITY,ALS.MU_CODE,ALS.FROM_LOCATION,ALS.TO_LOCATION,ALS.SHIPMENT_TYPE
            ,ALS.LOAD_TYPE,ALS.EST_DAY,ALS.CREATED_DATE,SAU.LOGIN_EDESC AS CREATED_BY_EDESC,ALS.LAST_MODIFIED_DATE,SA.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC  FROM AUD_LC_SHIPMENT ALS 
            JOIN IP_ITEM_MASTER_SETUP IMS ON ALS.ITEM_CODE = IMS.ITEM_CODE
            LEFT JOIN SC_APPLICATION_USERS  SAU ON ALS.CREATED_BY  = SAU.USER_NO
             LEFT JOIN SC_APPLICATION_USERS  SA ON ALS.LAST_MODIFIED_BY = SA.USER_NO WHERE ALS.LC_TRACK_NO = '{filter}' AND ALS.DELETED_FLAG = 'N' ORDER BY ALS.SNO ASC";

            var result = _dbContext.SqlQuery<ShipmentHistoryModels>(sqlquery).ToList();
            //result.Any(x => x.LAST_MODIFIED_DATE == null ? "" : x.LAST_MODIFIED_DATE);
            return result;
        }

        public List<DocumentHistoryModels> GetAllPOHistoryDocumentList(string filter)
        {
            var sqlquery = $@"SELECT DISTINCT LD.LC_TRACK_NO,LD.PO_CODE,PO.ORDER_NO,LD.PINVOICE_CODE,LP.PINVOICE_NO,LD.LOC_CODE,LL.LC_NUMBER,IPS.SUPPLIER_EDESC,LD.INVOICE_CODE,LI.INVOICE_NUMBER,LD.SNO,LD.FILE_DETAIL,LD.REMARKS,
                LD.CREATED_DATE,LD.CREATED_BY,SA.LOGIN_EDESC AS CREATED_BY_EDESC
                ,LD.LAST_MODIFIED_DATE,LD.LAST_MODIFIED_BY,SAU.LOGIN_EDESC AS LAST_MODIFIED_BY_EDESC
                FROM AUD_LC_DOCUMENT LD
                LEFT JOIN SC_APPLICATION_USERS SA ON LD.CREATED_BY = SA.USER_NO AND LD.COMPANY_CODE = SA.COMPANY_CODE
                LEFT JOIN SC_APPLICATION_USERS SAU ON LD.LAST_MODIFIED_BY = SAU.USER_NO AND LD.COMPANY_CODE = SAU.COMPANY_CODE
                LEFT JOIN LC_PURCHASE_ORDER PO ON LD.PO_CODE = PO.PO_CODE
                LEFT JOIN LC_PERFOMA_INVOICE LP ON LD.PINVOICE_CODE = LP.PINVOICE_CODE
                LEFT JOIN LC_LOC LL ON LD.LOC_CODE = LL.LOC_CODE
                LEFT JOIN LC_INVOICE LI ON LD.INVOICE_CODE = LI.INVOICE_CODE
                LEFT JOIN IP_SUPPLIER_SETUP IPS ON LL.LC_NUMBER = IPS.SUPPLIER_CODE AND LL.COMPANY_CODE = IPS.COMPANY_CODE
                WHERE LD.LC_TRACK_NO = '{filter}' AND LD.DELETED_FLAG = 'N' AND LD.COMPANY_CODE = '{_workcontext
                .CurrentUserinformation.company_code}' ORDER BY LD.CREATED_DATE ASC";

            var result = _dbContext.SqlQuery<DocumentHistoryModels>(sqlquery).ToList();
            List<DocumentHistoryModels> newlist = new List<DocumentHistoryModels>();
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

        public string OrderNumberExist(string ordernumber, string action, int pocode)
        {
            if (action == "create")
            {
                var sqlquery = $@"SELECT ORDER_NO FROM LC_PURCHASE_ORDER WHERE ORDER_NO = '{ordernumber}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                return result;
            }
            else
            {
                var sqlquery = $@"SELECT ORDER_NO FROM LC_PURCHASE_ORDER WHERE ORDER_NO = '{ordernumber}' AND PO_CODE != '{pocode}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var result = _dbContext.SqlQuery<string>(sqlquery).FirstOrDefault();
                return result;
            }
        }

        public POModel getPurchaseOrderdateandsupplierByOrderCode(string OrderCode)
        {
               POModel Record = new POModel();

            PurchaseOrderModels data = new PurchaseOrderModels();


            var SUPPLIER_CODE_query = $@"SELECT DISTINCT ISS.SUPPLIER_CODE as SUPPLIER_CODE ,ISS.SUPPLIER_EDESC as SUPPLIER_EDESC FROM IP_PURCHASE_ORDER IPO, IP_SUPPLIER_SETUP ISS
                                WHERE IPO.SUPPLIER_CODE=ISS.SUPPLIER_CODE
                                AND IPO.COMPANY_CODE=ISS.COMPANY_CODE
                               and  IPO.order_no='{OrderCode}' AND IPO.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";

            data = _dbContext.SqlQuery<PurchaseOrderModels>(SUPPLIER_CODE_query).FirstOrDefault();

            if (!string.IsNullOrEmpty(data.SUPPLIER_CODE))
            {
                var CHECK_query = $@"SELECT DISTINCT SUPPLIER_CODE from LC_BENEFICIARY where SUPPLIER_CODE='{data.SUPPLIER_CODE}' AND COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
                var response = _dbContext.SqlQuery<string>(CHECK_query).FirstOrDefault();


                
                if (string.IsNullOrEmpty(response))
                {
                    var insertquey = string.Format(@"INSERT INTO LC_BENEFICIARY(BNF_CODE,BNF_EDESC,BNF_NDESC,COUNTRY_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,ADDRESS,SUPPLIER_CODE)VALUES({0}, '{1}', '{2}','{3}','{4}', '{5}','{6}','{7}',TO_DATE('{8}', 'mm/dd/yyyy hh24:mi:ss'),'{9}','{10}','{11}')",
                   "LCSTATUS_SEQ.nextval", data.SUPPLIER_EDESC, data.SUPPLIER_EDESC, "", "", _workcontext.CurrentUserinformation.company_code, _workcontext.CurrentUserinformation.branch_code, _workcontext.CurrentUserinformation.User_id, DateTime.Now.ToString("MM/dd/yyyy/ HH:mm:ss"), 'N', "", data.SUPPLIER_CODE);

                    var rowCount = _dbContext.ExecuteSqlCommand(insertquey);
                    _dbContext.SaveChanges();
                }
                
            }
            

               var sqlquery = $@"SELECT DISTINCT TO_CHAR(IPO.ORDER_date,'MM/dd/yyyy') AS PODate, TO_CHAR(LB.BNF_CODE) AS POBenificary
                  FROM IP_PURCHASE_ORDER IPO, LC_BENEFICIARY LB
                 WHERE IPO.SUPPLIER_CODE=LB.SUPPLIER_CODE
                 AND IPO.ORDER_NO = '{OrderCode}'
                AND IPO.COMPANY_CODE = '{_workcontext.CurrentUserinformation.company_code}'";
               Record = _dbContext.SqlQuery<POModel>(sqlquery).FirstOrDefault();
               return Record;
        }
        

        #region Commented Lines
        //public string CreateShipment(ShipmentModels shipmentdetail)
        //{
        //    if (shipmentdetail.SNO == 0)
        //    {
        //        var nextshipmentno = $@"SELECT  SEQ_LC_SHIPMENT_NO.nextval as SNO FROM DUAL";
        //        var shipmentno = _dbContext.SqlQuery<ShipmentModels>(nextshipmentno).FirstOrDefault();

        //        var insertitem = $@"INSERT INTO LC_SHIPMENT(LC_TRACK_NO,SNO,ITEM_CODE,MU_CODE,QUANTITY,FROM_LOCATION,TO_LOCATION,SHIPMENT_TYPE,                           LOAD_TYPE,EST_DAY,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)VALUES('{shipmentdetail.LC_TRACK_NO}', '{shipmentno.SNO}', '{shipmentdetail.ITEM_CODE}','{shipmentdetail.MU_CODE}','{shipmentdetail.QUANTITY}', '{shipmentdetail.FROM_LOCATION}','{shipmentdetail.TO_LOCATION}',             '{shipmentdetail.SHIPMENT_TYPE}','{shipmentdetail.LOAD_TYPE}','{shipmentdetail.EST_DAY}','{_workcontext.CurrentUserinformation
        //                               .company_code}','{_workcontext.CurrentUserinformation.branch_code}','{_workcontext
        //                               .CurrentUserinformation.User_id}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{'N'}')";
        //        var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);


        //        return "Success";
        //    }
        //    else
        //    {
        //        if (shipmentdetail.ITEM_CODE != "")
        //        {
        //            var insertitem = $@"UPDATE LC_SHIPMENT SET ITEM_CODE = '{shipmentdetail.ITEM_CODE}',MU_CODE='{shipmentdetail.MU_CODE.ToUpper()}',QUANTITY='{shipmentdetail.QUANTITY}',FROM_LOCATION='{shipmentdetail.FROM_LOCATION}',TO_LOCATION='{shipmentdetail.TO_LOCATION}',SHIPMENT_TYPE='{shipmentdetail.SHIPMENT_TYPE}',LOAD_TYPE='{shipmentdetail.LOAD_TYPE}',EST_DAY='{shipmentdetail.EST_DAY}',COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
        //                             .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),DELETED_FLAG = '{'N'}' WHERE LC_TRACK_NO = '{shipmentdetail.LC_TRACK_NO}' AND SNO = '{shipmentdetail.SNO}'";
        //            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);
        //        }
        //        else
        //        {
        //            var insertitem = $@"UPDATE LC_SHIPMENT SET MU_CODE='{shipmentdetail.MU_CODE.ToUpper()}', QUANTITY='{shipmentdetail.QUANTITY}',FROM_LOCATION='{shipmentdetail.FROM_LOCATION}',TO_LOCATION='{shipmentdetail.TO_LOCATION}',SHIPMENT_TYPE='{shipmentdetail.SHIPMENT_TYPE}',LOAD_TYPE='{shipmentdetail.LOAD_TYPE}',EST_DAY='{shipmentdetail.EST_DAY}',COMPANY_CODE='{_workcontext.CurrentUserinformation.company_code}',BRANCH_CODE='{_workcontext.CurrentUserinformation.branch_code}',LAST_MODIFIED_BY='{_workcontext
        //                            .CurrentUserinformation.User_id}',LAST_MODIFIED_DATE=TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),DELETED_FLAG = '{'N'}' WHERE LC_TRACK_NO = '{shipmentdetail.LC_TRACK_NO}' AND SNO = '{shipmentdetail.SNO}'";
        //            var iteminsert = _dbContext.ExecuteSqlCommand(insertitem);


        //        }
        //        return "Success";
        //    }
        //}

        //public List<ItemDetail> GetAllShipmentItemsLists(string filter, string lctrackno)
        //{
        //    try
        //    {
        //        if (string.IsNullOrEmpty(filter)) { filter = string.Empty; }

        //        var sqlquery = string.Format(@"SELECT DISTINCT LI.ITEM_CODE,IMS.ITEM_EDESC,LI.MU_CODE AS INDEX_MU_CODE FROM LC_ITEM LI 
        //                       JOIN IP_ITEM_MASTER_SETUP IMS ON LI.ITEM_CODE = IMS.ITEM_CODE 
        //                    where LI.deleted_flag='N'
        //                    AND LI.LC_TRACK_NO = '{1}'
        //                    and GROUP_SKU_FLAG = 'I' 
        //                    and (LI.ITEM_CODE like '%{0}%' 
        //                    or upper(IMS.ITEM_EDESC) like '%{0}%')",
        //                    filter.ToUpperInvariant(), lctrackno);
        //        var result = _dbContext.SqlQuery<ItemDetail>(sqlquery).ToList();
        //        return result;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //}

        #endregion

         
    }
}