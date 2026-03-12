using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text.RegularExpressions;
using System.Transactions;

namespace NeoErp.Services.Repository.ThirdPartyApi
{
    public class ShymphonyService : IShymphonyService
    {
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private IDbContext _dbContext;
        public ShymphonyService(IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;

        }
        public string New_Sales_No(string companycode, string formcode, string transactiondate, string tablename)
        {
            try
            {
                if (companycode != "" && formcode != "" && transactiondate != "" && tablename != "")
                {
                    string query = string.Format(@"select FN_NEW_VOUCHER_NO('{0}','{1}','{2}','{3}') FROM DUAL", companycode, formcode, transactiondate, tablename);
                    string voucherNo = this._dbContext.SqlQuery<string>(query).First();
                    return voucherNo;
                }
                else
                { return ""; }

            }
            catch (Exception)
            {
                throw;
            }
        }
        public void InsertInvoiceRecord(Shymphony Record)
        {

            string SALES_NO;
            //var settlementName = Record.TenderMedia[0].Name.ToLower(); if (Record.TenderMedia.Count > 0 && Record.TenderMedia[0].Name.ToLower() != "room settlement")
            string[] roomSettlementValueArray = ConfigurationManager.AppSettings["roomSettlement"].Split(',');
            roomSettlementValueArray = roomSettlementValueArray.Select(x => x.ToLower()).ToArray();
;            if (Record.TenderMedia != null)
            {
                if (Record.MenuItem.Count > 0)
                {
                    if (Record.TenderMedia.Count > 0)
                    {

                        //if (Record.TenderMedia[0].Name.ToLower() != roomSettlementValue && Record.TenderMedia[0].Name.ToLower() != ("*Room Charge").ToLower() && Record.TenderMedia[0].Name.ToLower() != ("Roomposting").ToLower())//added Roomposting later because the bill was getting settled while posting to room and the name was Roomposting in xml. (Animesh)
                        if(! roomSettlementValueArray.Contains(Record.TenderMedia[0].Name.ToLower()))
                        {
                            InsertData(Record);

                        }
                        else
                        {
                            SALES_NO = "Room Charge";
                            Record.Sales_No = SALES_NO;

                        }
                    }
                    else
                    {
                        SALES_NO = "No Tax";
                        Record.Sales_No = SALES_NO;


                    }


                }
                else
                {
                    SALES_NO = "No MenuItem";
                }
            }


        }

        public void InsertData(Shymphony Record)
        {

            int SERIAL_NO = 0;
            var deletedflag = "N";
            string SALES_NO = "0";
            var max_item_code = 1;
            var isSyncToIrd = ConfigurationManager.AppSettings["isSyncToIrd"];
            var iS_SYNC_WITH_IRD = "N";
            var iS_REAL_TIME = "N";
            if (isSyncToIrd == "true")
            {
                iS_SYNC_WITH_IRD = "Y";
                iS_REAL_TIME = "Y";
            }
            var vat_charge_acc_code = "";
            var lt_charge_acc_code = "";
            var st_charge_acc_code = "";
            var discount_acc_code = "";
            var Customer_Acc_Code = "";
            var accode = "";
            var formcode = "";
            var companycode = "01";
            var branchcode = "";
            var masterCustomerCode = "";
            var preCustomerCode = "";
            var masterItemCode = "";
            var preItemCode = "";
            var isParentCustomerCreated = "";
            var isParentItemCreated = "";
            string osPreferenceSetupPQuery;

            if (bool.Parse(ConfigurationManager.AppSettings["IsRetailBusiness"]))
            {
                osPreferenceSetupPQuery = $@"Select SVAT_CHARGE_ACC_CODE,LT_CHARGE_ACC_CODE,SDISCOUNT_ACC_CODE,SSERCIVE_ACC_CODE,SCUS_ACC_CODE,SITEM_ACC_CODE,SFORM_CODE,STABLE_NAME,S_ISPARENTCUSTOMER_CREATED,S_ISPARENTITEM_CREATED,MASTER_CUSTOMER_CODE,
                                                       PRE_CUSTOMER_CODE,MASTER_ITEM_CODE,PRE_ITEM_CODE,COMPANY_CODE from OS_PREFERENCE_SETUP WHERE STABLE_NAME='{"SA_SALES_INVOICE"}' AND RETAIL_FLAG='Y'";
                branchcode = "01.02";
            }
            else
            {
                osPreferenceSetupPQuery = $@"Select SVAT_CHARGE_ACC_CODE,LT_CHARGE_ACC_CODE,SDISCOUNT_ACC_CODE,SSERCIVE_ACC_CODE,SCUS_ACC_CODE,SITEM_ACC_CODE,SFORM_CODE,STABLE_NAME,S_ISPARENTCUSTOMER_CREATED,S_ISPARENTITEM_CREATED,MASTER_CUSTOMER_CODE,
                                                       PRE_CUSTOMER_CODE,MASTER_ITEM_CODE,PRE_ITEM_CODE,COMPANY_CODE from OS_PREFERENCE_SETUP WHERE STABLE_NAME='{"SA_SALES_INVOICE"}' AND RETAIL_FLAG='N'";
                branchcode = "01.01";
            }
            var resultModel = this._dbContext.SqlQuery<OS_PREFERENCE_SETUP>(osPreferenceSetupPQuery).FirstOrDefault();

            vat_charge_acc_code = resultModel.SVAT_CHARGE_ACC_CODE;
            st_charge_acc_code = resultModel.SSERCIVE_ACC_CODE;
            lt_charge_acc_code = resultModel.LT_CHARGE_ACC_CODE;
            discount_acc_code = resultModel.SDISCOUNT_ACC_CODE;
            Customer_Acc_Code = resultModel.SCUS_ACC_CODE;
            accode = resultModel.SITEM_ACC_CODE;
            formcode = resultModel.SFORM_CODE;
            masterCustomerCode = resultModel.MASTER_CUSTOMER_CODE;
            preCustomerCode = resultModel.PRE_CUSTOMER_CODE;
            masterItemCode = resultModel.MASTER_ITEM_CODE;
            preItemCode = resultModel.PRE_ITEM_CODE;
            isParentCustomerCreated = resultModel.S_ISPARENTCUSTOMER_CREATED;
            isParentItemCreated = resultModel.S_ISPARENTITEM_CREATED;
            var customer_code = Record.Header[0].RvcID;
            //var customer_code = Record.Header[0].ClientId;
            if (customer_code == null)
            {
                customer_code = "sycust";
            }

            var InvoiceNumber = Record.Checkxml.CheckNum;
            // var InvoiceNumber = Record.Header[0].PrimaryCheckIdentification;
            // var InvoiceNumber = Record.Checkxml.Id;
            //opera.PositiveCharge.Where(x => x.TYPE == "X").Select(x => Convert.ToDouble(x.NETAMOUNT)).Sum();
            //var MasterAmount = Math.Abs(Convert.ToDecimal(Record.TenderMedia[0].Total)).ToString();
            //double MasterAmount = 0;
            //if (Record.TenderMedia.Count > 0)
            //{
            //    MasterAmount = Record.TenderMedia.Select(x => Convert.ToDouble(x.Total)).Sum();
            //}

            //


            //

            double MasterAmount = 0.00;
            double cumulative_discount_amount = 0;
            double cumulative_st_amount = 0.00;
            double total_tax_amount = 0;
            var newTotal = 0.00;

            var commulative_service_charge = 0.00;
            var commulative_vat_amount = 0.00;
            var commulative_lt_amount = 0.00;
            var commulative_sub_total_amount = 0.00;
            var commulative_master_amount = 0.00;

            var underamountpositivedata = new List<string>();
            var negativedata = new List<string>();
            List<ItemChargeModel> ItemChargeModelList = new List<ItemChargeModel>();
            var CustomerName = "";
            CustomerName = Regex.Replace(Record.Header[0].RvcName, "[^0-9a-zA-Z]+", " ");

            var tblNumber = string.IsNullOrEmpty(Record.Header[0].Table) ? "" : Record.Header[0].Table;
            var guestCount = string.IsNullOrEmpty(Record.Header[0].GuestCount) ? "" : Record.Header[0].GuestCount;
            var TBL = guestCount + "/" + tblNumber;
            //Creating customer details
            var Guest_Name = "Guest";
            //making address;
            var PayeeAddress = string.IsNullOrEmpty(Record.Header[0].Address) ? "" : Record.Header[0].Address;
            var PayeeCity = string.IsNullOrEmpty(Record.Header[0].City) ? "" : ", " + Record.Header[0].City;
            var PayeePostalCode = string.IsNullOrEmpty(Record.Header[0].PostalCode) ? "" : ", " + Record.Header[0].PostalCode;
            var PayeeState = string.IsNullOrEmpty(Record.Header[0].State) ? "" : ", " + Record.Header[0].State;

            var Customer_Address = "LOCAL";
            var customer_phone = "";
            var customerPanNumber = "";
            //if (Record.Header[0].CaptureCustomerData == "True")
            //{
            //    Customer_Address = PayeeAddress + " " + PayeeCity + " " + PayeeState + " " + PayeePostalCode;
            //    customer_phone = string.IsNullOrEmpty(Record.Header[0].Number) ? "" : Record.Header[0].Number;
            //    customerPanNumber = string.IsNullOrEmpty(Record.Header[0].Document) ? "N/A" : Record.Header[0].Document;
            //    Guest_Name = string.IsNullOrEmpty(Record.Header[0].Name) ? "Customer" : Record.Header[0].Name;
            //}

            //if (customer_code == "")
            //{
            //    customer_code = "sycust";
            //    CustomerName = Regex.Replace(Record.Header[0].RvcName, "[^0-9a-zA-Z]+", " ");
            //}
            Shymphony.buyer_name = CustomerName;
            Shymphony.buyer_pan = customerPanNumber;
            //Creating customer details end
            #region query for invoice_date
            string InvoiceDateQuery = $@"SELECT BS_DATE(TRUNC(TO_DATE('{DateTime.Now.ToString("yyyy/MM/dd")}','yyyy/MM/dd'))) as INVOICE_DATE FROM DUAL";
            string invoiceDate = _dbContext.SqlQuery<string>(InvoiceDateQuery).FirstOrDefault();

            Shymphony.invoice_date = invoiceDate;
            Shymphony.form_code = formcode;
            #endregion



            #region Rejectable Items
            //checking if the table exists

            var rejectableItem = new List<RejectedItem>();
            string rejectableItemQuery = $@"Select ITEM_CODE,ITEM_NAME FROM REJECTABLE_ITEMS WHERE DELETED_FLAG='N'";
            try  //added try in case if the database does not have the table so the application wont stop 
            {
                rejectableItem = this._dbContext.SqlQuery<RejectedItem>(rejectableItemQuery).ToList();
            }
            catch (Exception ex)
            {

            }
            #endregion

            #region CHECK NUMBER EXSITS
            string CheckQuery = $@"SELECT Count(*) FROM SA_SALES_INVOICE WHERE MANUAL_NO='{InvoiceNumber}' and FORM_CODE='{formcode}' and deleted_flag='N'";
            int count = _dbContext.SqlQuery<int>(CheckQuery).FirstOrDefault();
            if (count > 0)
            {
                SALES_NO = "EXISTED_INV";
            }
            else if (string.IsNullOrEmpty(InvoiceNumber))
            {
                SALES_NO = "NO_INV";
            }
            else
            {
                #region Tax SC Discount Calculation

                //Discount from xml
                if (Record.Discount.Count > 0)
                {
                    foreach (var discountdata in Record.Discount)
                    {
                        cumulative_discount_amount += Convert.ToDouble(discountdata.Total);
                    }
                    cumulative_discount_amount = Math.Abs(cumulative_discount_amount);
                }


                //Previous Discount Calculation Animesh
                //Percentage wise discount
                //if (Record.Discount.Count > 0)
                //{
                //    var discountitems = Record.MenuItem.Select(x => x.Total).ToList();

                //    //for Hardrock % wise discount
                //    //foreach(var amount in discountitems)
                //    //{
                //    //    var percentage = Record.Discount.Select(x => x.Percentage).FirstOrDefault();


                //    //    cumulative_discount_amount += Convert.ToDouble(amount) * Convert.ToDouble(percentage);


                //    //}
                //    //cumulative_discount_amount = Math.Abs(cumulative_discount_amount);

                //    foreach (var discountdata in Record.Discount.Select((value, i) => new { i, value }))
                //    {

                //        cumulative_discount_amount = Convert.ToDouble(discountitems[discountdata.i]) * Convert.ToDouble(discountdata.value.Percentage);
                //    }
                //    cumulative_discount_amount = Math.Abs(cumulative_discount_amount);
                //}




                //foreach (var servicedata in Record.ServiceCharge)
                //{
                //    total_tax_amount += Convert.ToDouble(servicedata.Total);
                //}
                //total_tax_amount = Math.Abs(total_tax_amount);
                var checkTax = Record.Header[0].CheckTax != null ? Convert.ToDouble(Record.Header[0].CheckTax) : 0;
                var checkAutoServiceCharge = Record.Header[0].CheckAutoServiceCharge != null ? Convert.ToDouble(Record.Header[0].CheckAutoServiceCharge) : 0;
                total_tax_amount = Convert.ToDouble(checkTax);
                cumulative_st_amount = Math.Abs(Convert.ToDouble(checkAutoServiceCharge));
                //foreach (var taxitem in Record.MenuItem)
                //{

                //    foreach (var taxdata in taxitem.ItemTaxList)
                //    {
                //        total_tax_amount += Convert.ToDecimal(taxdata.TaxValue);
                //    }
                //}
                var totalSalesAmount = 0.00;
                #endregion
                #endregion
                // var customer_sublink_code = "";
                var ItemId = "";
                var CustomerCode = "";
                var SYN_ROWID = "";
                //List<ShymphonyInvoiceModel> ShymphonyInvoiceModelList = new List<ShymphonyInvoiceModel>();
                //ShymphonyInvoiceModel ShymphonyInvoiceModel = new ShymphonyInvoiceModel();
                #region Inserting customer
                if (isParentCustomerCreated == "N")
                {
                    //masterCustomerCode = "01";
                    //preCustomerCode = "00";
                    string CustomerCodeQuery = $@"SELECT TO_CHAR(NVL(MAX(TO_NUMBER(C.CUSTOMER_CODE )+1),1)) AS C  FROM SA_CUSTOMER_SETUP C";
                    CustomerCode = this._dbContext.SqlQuery<string>(CustomerCodeQuery).FirstOrDefault();
                    string InsertSaCustomerSetupQuery = $@"INSERT INTO SA_CUSTOMER_SETUP(CUSTOMER_CODE,CUSTOMER_EDESC,TEL_MOBILE_NO1,GROUP_SKU_FLAG,CREDIT_ACTION_FLAG,CASH_CUSTOMER_FLAG,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE,PARTY_TYPE_CODE,APPROVED_FLAG,CUSTOMER_ID,REGD_OFFICE_EADDRESS,ACC_CODE)
                            VALUES('{CustomerCode}','{"OperaSymphonyCustomer"}','{""}','G','{""}','{""}','{masterCustomerCode}','{preCustomerCode}','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','{branchcode}','CP','Y','{customer_code}','{Customer_Address}','{Customer_Acc_Code}')";
                    _dbContext.ExecuteSqlCommand(InsertSaCustomerSetupQuery);

                    string CustomerParentUpdateCodeQuery = $@"UPDATE OS_PREFERENCE_SETUP SET S_ISPARENTCUSTOMER_CREATED='{"Y"}' WHERE  STABLE_NAME='{"SA_SALES_INVOICE"}' AND SFORM_CODE='{formcode}'";
                    _dbContext.ExecuteSqlCommand(CustomerParentUpdateCodeQuery);
                }
                else
                {
                    string SelectCustomerSetup = string.Empty;
                    if (Record.Header[0].CaptureCustomerData == "True")
                    {
                        customerPanNumber = string.IsNullOrEmpty(Record.Header[0].Document) ? "N/A" : Record.Header[0].Document;
                        SelectCustomerSetup = $@"Select CUSTOMER_CODE as CustomerCode,CUSTOMER_ID as CustomerId  from SA_CUSTOMER_SETUP WHERE TPIN_VAT_NO='{customerPanNumber}' and deleted_flag='N'";

                        //ustomer_code = customerPanNumber;

                        if (Record.Header[0].CaptureCustomerData == "True")
                        {

                            Customer_Address = PayeeAddress + " " + PayeeCity + " " + PayeeState + " " + PayeePostalCode;
                            customer_phone = string.IsNullOrEmpty(Record.Header[0].Number) ? "" : Record.Header[0].Number;
                            var customer_Name = string.IsNullOrEmpty(Record.Header[0].Name) ? "Customer" : Record.Header[0].Name;
                            CustomerName = Regex.Replace(customer_Name, "[^0-9a-zA-Z]+", " ");
                        }
                    }
                    else
                    {
                        SelectCustomerSetup = $@"Select CUSTOMER_CODE as CustomerCode,CUSTOMER_ID as CustomerId  from SA_CUSTOMER_SETUP WHERE CUSTOMER_ID='{customer_code}' and deleted_flag='N'";
                    }
                    var CustomerCodeList = this._dbContext.SqlQuery<CustomersDetail>(SelectCustomerSetup);
                    if (CustomerCodeList.FirstOrDefault() == null)
                    {
                        // if (CustomerCodeList.Count < 1)
                        // {
                        //masterCustomerCode = masterCustomerCode;
                        preCustomerCode = masterCustomerCode + ".00";
                        string CustomerCodeQuery = $@"select TO_CHAR(NVL(MAX(TO_NUMBER(C.CUSTOMER_CODE )+1),1)) AS C  from sa_customer_setup c";
                        CustomerCode = this._dbContext.SqlQuery<string>(CustomerCodeQuery).FirstOrDefault();
                        var subcode = "C" + CustomerCode;
                        string InsertSaCustomerSetupQuery = $@"INSERT INTO SA_CUSTOMER_SETUP(CUSTOMER_CODE,CUSTOMER_EDESC,CUSTOMER_NDESC,TEL_MOBILE_NO1,GROUP_SKU_FLAG,CREDIT_ACTION_FLAG,CASH_CUSTOMER_FLAG,MASTER_CUSTOMER_CODE,PRE_CUSTOMER_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE,PARTY_TYPE_CODE,APPROVED_FLAG,CUSTOMER_ID,REGD_OFFICE_EADDRESS,LINK_SUB_CODE,ACC_CODE,TPIN_VAT_NO)
                            VALUES('{CustomerCode}','{CustomerName}','{Guest_Name}','{customer_phone}','I','{""}','{""}','{preCustomerCode}','{masterCustomerCode}','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','{branchcode}','CP','Y','{customer_code}','{Customer_Address}','{subcode}','{Customer_Acc_Code}','{customerPanNumber}')";
                        var ItemCustomerCount = _dbContext.ExecuteSqlCommand(InsertSaCustomerSetupQuery);
                        //    #region INSERT INTO SUB-LEDGER MAP
                        ////    var SUB_LEDGER_MAP_QUERY = $@"INSERT INTO FA_SUB_LEDGER_MAP(SUB_CODE,ACC_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE)             
                        ////VALUES('{subcode}','16','01','ADMIN',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}', 'MM/dd/yyyy'))";
                        ////    this._dbContext.ExecuteSqlCommand(SUB_LEDGER_MAP_QUERY);
                        //    #endregion
                        // }
                    }
                    else
                    {
                        if (CustomerCodeList != null)
                        {

                            CustomerCode = CustomerCodeList.FirstOrDefault().CustomerCode;

                        }

                    }

                }
                #endregion
                //#region Customer SKU_Group_Flag G
                //string countCustomerQuery = $@"Select count(CUSTOMER_CODE) AS CUSTOMER_CODE  from SA_CUSTOMER_SETUP WHERE GROUP_SKU_FLAG='G' and CUSTOMER_EDESC='OperaSymphonyCustomer'";
                //int customerCount = this._dbContext.SqlQuery<int>(countCustomerQuery).FirstOrDefault();
                //customer_sublink_code = "C" + CustomerCode;
                //#endregion

                #region INSERT INTO IP_CATEGORY_CODE
                string SELECT_IP_CATEGORY_CODE_QUERY = $@"SELECT CATEGORY_CODE FROM IP_CATEGORY_CODE";
                List<string> ItemCategoryCodeList = this._dbContext.SqlQuery<string>(SELECT_IP_CATEGORY_CODE_QUERY).ToList();
                if (!ItemCategoryCodeList.Contains("FF"))
                {
                    string InsertIpItemMasterSetupQuerey = $@"INSERT INTO IP_CATEGORY_CODE (CATEGORY_CODE,CATEGORY_EDESC,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,CATEGORY_TYPE)
                    VALUES('{"FF"}','{"Food"}','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','{"FF"}')";
                    _dbContext.ExecuteSqlCommand(InsertIpItemMasterSetupQuerey);
                }
                #endregion
                //Eliminating Voided Items and Items with less or zero total
                var negativeitemnamelist = Record.MenuItem.Where(x => Convert.ToDouble(x.Total) < 0 || Convert.ToDouble(x.Total) == 0).ToList();
                var positiveitemnamelist = Record.MenuItem.Where(x => Convert.ToDouble(x.Total) > 0).ToList();
                for (int i = 0; i < negativeitemnamelist.Count(); i++)
                {
                    var name = negativeitemnamelist[i].Name;
                    var PosItemName = positiveitemnamelist.Where(x => x.Name == name).FirstOrDefault();
                    if (PosItemName != null)
                    {
                        positiveitemnamelist.Remove(PosItemName);
                    }

                }
                Record.MenuItem = positiveitemnamelist;
                //Rejecting Rejectble Items

                if (rejectableItem.Count > 0)
                {
                    Record.MenuItem.RemoveAll(x => rejectableItem.Any(y => y.ITEM_NAME == x.Name));
                }
                int cnt = Record.MenuItem.Count;
                if (Record.MenuItem.Count > 0)
                {
                    //Animesh
                    string SquenceQuerey = $@"select MYSEQUENCE.nextval from duAL";
                    decimal ROWID = this._dbContext.SqlQuery<decimal>(SquenceQuerey).FirstOrDefault();
                    SYN_ROWID = ROWID.ToString();

                    foreach (var itemData in Record.MenuItem)
                    {
                        var payment_mode = String.Empty;

                        if (Convert.ToDouble(itemData.Total) < 0)
                        {
                            itemData.Name = Regex.Replace(itemData.Name, "[^0-9a-zA-Z]+", " ") + " (Voided)";
                        }
                        else
                        {
                            itemData.Name = Regex.Replace(itemData.Name, "[^0-9a-zA-Z]+", " ");
                        }
                        ++SERIAL_NO;
                        if (isParentItemCreated == "N")
                        {
                            //masterItemCode = "01";
                            //preItemCode = "00";
                            //string SelectItemCodeQuerey1 = $@"SELECT max(TO_NUMBER(item_code)+1) FROM IP_ITEM_MASTER_SETUP";
                            //var max_item_code1 = this._dbContext.SqlQuery<int>(SelectItemCodeQuerey1).FirstOrDefault();
                            string SelectItemCodeQuerey = $@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(item_code )+1),1))) AS item_code FROM IP_ITEM_MASTER_SETUP";
                            max_item_code = this._dbContext.SqlQuery<int>(SelectItemCodeQuerey).FirstOrDefault();

                            string InsertIpItemMasterSetupQuerey = $@"INSERT INTO IP_ITEM_MASTER_SETUP (ITEM_CODE,PRODUCT_CODE,ITEM_EDESC,CATEGORY_CODE,GROUP_SKU_FLAG,MASTER_ITEM_CODE,PRE_ITEM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE)
                    VALUES('{max_item_code}','{ItemId}','{"OperaSymphonyItem"}','{"FF"}','G','{masterItemCode}','{preItemCode}','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','{branchcode}')";
                            _dbContext.ExecuteSqlCommand(InsertIpItemMasterSetupQuerey);

                            string CustomerParentUpdateCodeQuery = $@"UPDATE OS_PREFERENCE_SETUP SET S_ISPARENTITEM_CREATED='{"Y"}' WHERE  STABLE_NAME='{"SA_SALES_INVOICE"}' AND SFORM_CODE='{formcode}'";
                            _dbContext.ExecuteSqlCommand(CustomerParentUpdateCodeQuery);
                        }

                        #region  INSERT INTO ITEM MASTER TABLE (ITEM)

                        string SelectIpItemMasterSetupQuerey = $@"SELECT ITEM_EDESC FROM IP_ITEM_MASTER_SETUP";
                        List<string> ItemCodeList = this._dbContext.SqlQuery<string>(SelectIpItemMasterSetupQuerey).ToList();
                        string SelectItemCodeQuery = $@"SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE ITEM_EDESC='{itemData.Name}'";
                        string MaxItemCode = this._dbContext.SqlQuery<string>(SelectItemCodeQuery).FirstOrDefault();
                        if (MaxItemCode == null || MaxItemCode == "")
                        {
                            string SelectItemCodeQuerey = $@"SELECT TO_NUMBER(TO_CHAR(NVL(MAX(TO_NUMBER(item_code )+1),1))) AS item_code FROM IP_ITEM_MASTER_SETUP";
                            max_item_code = this._dbContext.SqlQuery<int>(SelectItemCodeQuerey).FirstOrDefault();
                        }
                        else
                        {
                            max_item_code = Convert.ToInt32(MaxItemCode);
                        }
                        if (!ItemCodeList.Contains(itemData.Name))
                        {
                            //masterItemCode = "01";
                            //var pre_item_code = masterItemCode + ".00";
                            preItemCode = masterItemCode + ".00";

                            string InsertIpItemMasterSetupQuerey = $@"INSERT INTO IP_ITEM_MASTER_SETUP (ITEM_CODE,PRODUCT_CODE,ITEM_EDESC,CATEGORY_CODE,GROUP_SKU_FLAG,MASTER_ITEM_CODE,PRE_ITEM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,BRANCH_CODE,INDEX_MU_CODE)
                    VALUES('{max_item_code}','{ItemId}','{itemData.Name}','{"FF"}','I','{preItemCode}','{masterItemCode}','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','{branchcode}','UT')";
                            _dbContext.ExecuteSqlCommand(InsertIpItemMasterSetupQuerey);
                        }
                        #endregion
                        #region INSERT INTO ITEM INTEGRATION SETUP
                        //string SelectItemListQuerey = $@"SELECT ITEM_CODE  FROM IP_INTEGRATION_SETUP WHERE ITEM_CODE='{max_item_code}'AND ACC_CODE='{accode}' AND FORM_CODE='{formcode}' ";
                        string SelectItemListQuerey = $@"SELECT ITEM_CODE  FROM IP_INTEGRATION_SETUP WHERE ITEM_CODE='{max_item_code}'AND ACC_CODE='{accode}' AND FORM_CODE='{formcode}' ";
                        List<string> Response = this._dbContext.SqlQuery<string>(SelectItemListQuerey).ToList();

                        if (Response.Count() == 0)
                        {
                            string IntegrationQuery = $@"INSERT INTO IP_INTEGRATION_SETUP(ITEM_CODE,ACC_CODE,FORM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,INSERT_FLAG)
                            VALUES('{max_item_code}','{accode}','{formcode}','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','{"F"}')";
                            this._dbContext.ExecuteSqlCommand(IntegrationQuery);
                        }
                        #endregion
                        #region INSERT INTO CHARGE ITEM SETUP 
                        string ItemCodeQuerey = $@"SELECT ITEM_CODE FROM IP_ITEM_MASTER_SETUP WHERE PRODUCT_CODE='{ItemId}' ";
                        var ItemCode = this._dbContext.SqlQuery<string>(ItemCodeQuerey).FirstOrDefault();


                        #region   ITEM CHARGE
                        ItemChargeModel ItemChargeModel = new ItemChargeModel();
                        ItemChargeModel.ItemCode = ItemCode;
                        ItemChargeModelList.Add(ItemChargeModel);

                        #endregion
                        var ItemChargecount = ItemChargeModelList.Where(x => x.ItemCode == ItemCode).Count();

                        string SelectChargeCodeQuerey = $@"SELECT COUNT(CHARGE_CODE) FROM IP_ITEM_CHARGE_SETUP where CHARGE_CODE='{"VT"}' and FORM_CODE='{formcode}' and ITEM_CODE='{ItemCode}'   ";
                        var CHARGECODECOUNT = this._dbContext.SqlQuery<int>(SelectChargeCodeQuerey).FirstOrDefault();
                        if (ItemChargecount == 0)
                        {
                            if (CHARGECODECOUNT < 1)
                            {

                                string InsertIpItemChargeSetupQuerey = $@"INSERT INTO IP_ITEM_CHARGE_SETUP (ITEM_CODE,FORM_CODE,CHARGE_CODE,SERIAL_NO,VALUE_QUANTITY_BASED,VALUE_PERCENT_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                        VALUES('{max_item_code}','{formcode}','VT','{SERIAL_NO}','V','P','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}')";
                                _dbContext.ExecuteSqlCommand(InsertIpItemChargeSetupQuerey);
                            }
                        }


                        string SelectChargeCodeQuerey1 = $@"SELECT COUNT(CHARGE_CODE) FROM IP_ITEM_CHARGE_SETUP where CHARGE_CODE='{"SC"}' and FORM_CODE='{formcode}' and ITEM_CODE='{ItemCode}'   ";
                        var CHARGECODECOUNT1 = this._dbContext.SqlQuery<int>(SelectChargeCodeQuerey1).FirstOrDefault();

                        if (ItemChargecount == 0)
                        {
                            if (CHARGECODECOUNT1 < 1)
                            {
                                string InsertIpItemChargeSetupQuerey = $@"INSERT INTO IP_ITEM_CHARGE_SETUP (ITEM_CODE,FORM_CODE,CHARGE_CODE,SERIAL_NO,VALUE_QUANTITY_BASED,VALUE_PERCENT_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                     VALUES('{max_item_code}','{formcode}','SC','{SERIAL_NO}','V','P','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}')";
                                _dbContext.ExecuteSqlCommand(InsertIpItemChargeSetupQuerey);
                            }
                        }
                        string SelectChargeCodeQuerey2 = $@"SELECT COUNT(CHARGE_CODE) FROM IP_ITEM_CHARGE_SETUP where CHARGE_CODE='{"DC"}' and FORM_CODE='{formcode}' and ITEM_CODE='{ItemCode}'   ";
                        var CHARGECODECOUNT2 = this._dbContext.SqlQuery<int>(SelectChargeCodeQuerey1).FirstOrDefault();
                        if (ItemChargecount == 0)
                        {
                            if (CHARGECODECOUNT2 < 1)
                            {
                                string InsertIpItemChargeSetupQuerey = $@"INSERT INTO IP_ITEM_CHARGE_SETUP (ITEM_CODE,FORM_CODE,CHARGE_CODE,SERIAL_NO,VALUE_QUANTITY_BASED,VALUE_PERCENT_FLAG,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG)
                     VALUES('{max_item_code}','{formcode}','DC','{SERIAL_NO}','V','P','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}')";
                                _dbContext.ExecuteSqlCommand(InsertIpItemChargeSetupQuerey);
                            }

                        }
                        #endregion
                        #region INSERT INTO SALES INVOICE TABLE
                        //New Sales Number
                        var transactiondate = DateTime.Now.ToString("dd-MMM-yyyy");
                        var sales_tablename = "SA_SALES_INVOICE";
                        #region Insert into Sales Invoice
                        //commented later and left at front in order to take same rowid eveytime (Animesh)
                        //string SquenceQuerey = $@"select MYSEQUENCE.nextval from duAL";
                        //decimal ROWID = this._dbContext.SqlQuery<decimal>(SquenceQuerey).FirstOrDefault();
                        //SYN_ROWID = ROWID.ToString();

                        #region Insert into Sales Invoice
                        string ChecksalesManualNoQuery = $@"select count( MANUAL_NO) as MANUAL_NO  from SA_SALES_INVOICE WHERE MANUAL_NO='{InvoiceNumber}' and form_code='{formcode}'";
                        var checksalesCount = this._dbContext.SqlQuery<int>(ChecksalesManualNoQuery).FirstOrDefault();
                        if (checksalesCount < 1)
                        {
                            SALES_NO = New_Sales_No(companycode, formcode, transactiondate, sales_tablename);
                        }

                        else
                        {
                            string SalesQuery = $@"select SALES_NO from SA_SALES_INVOICE WHERE MANUAL_NO='{InvoiceNumber}'";
                            SALES_NO = this._dbContext.SqlQuery<string>(SalesQuery).FirstOrDefault();
                        }
                        // double Total = 0;
                        // var totalQuantity = Convert.ToDouble(itemData.SalesCount);
                        var totalQuantity = Convert.ToDouble(itemData.Quantity);
                        var Total = Convert.ToDouble(itemData.Total) / Convert.ToDouble(totalQuantity);

                        //new change
                        newTotal += (totalQuantity * Total);

                        #region Tax SC Discount Calculation

                        //AA


                        if (Record.TenderMedia.Count > 0)

                        {
                            for (int index = 0; index < Record.TenderMedia.Count; index++)
                            {
                                if (index == Record.TenderMedia.Count - 1)
                                {
                                    payment_mode += Record.TenderMedia[index].Name;
                                }
                                else
                                {
                                    payment_mode += Record.TenderMedia[index].Name + " & ";
                                }

                            }

                        }



                        //---------------------------
                        //MasterAmount = ((newTotal + total_tax_amount + cumulative_st_amount) - cumulative_discount_amount);
                        //totalSalesAmount = ((MasterAmount + cumulative_discount_amount) - (total_tax_amount + cumulative_st_amount));
                        //Shymphony.vatAmount = total_tax_amount;
                        //Shymphony.discountAmount = cumulative_discount_amount;
                        //Shymphony.serviceTaxAmount = cumulative_st_amount;

                        //Shymphony.totalSales = ((MasterAmount + cumulative_discount_amount) - (total_tax_amount + cumulative_st_amount));
                        //Shymphony.taxableSalesVat = ((totalSalesAmount - cumulative_discount_amount) + cumulative_st_amount);
                        //--------------------------------

                        string InsertSalesInvoiceQuery = $@"INSERT INTO SA_SALES_INVOICE(SALES_NO,MANUAL_NO,MISC_CODE,SALES_DATE,CUSTOMER_CODE,SERIAL_NO,ITEM_CODE,MU_CODE,QUANTITY,
                                        UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,FORM_CODE,COMPANY_CODE,BRANCH_CODE,TRACKING_NO,REMARKS,DESCRIPTION,CREATED_BY,CREATED_DATE,DELETED_FLAG,FROM_LOCATION_CODE,PARTY_TYPE_CODE,SESSION_ROWID,BUDGET_FLAG,BATCH_NO, PAYMENT_MODE)
                                        VALUES('{SALES_NO}','{InvoiceNumber}','{""}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{CustomerCode}','{SERIAL_NO}','{max_item_code}','NOS','{totalQuantity}',
                                        TO_NUMBER({Total}),TO_NUMBER({Total * totalQuantity}),'{totalQuantity}',TO_NUMBER({Total}),TO_NUMBER({Total * totalQuantity}),'{formcode}','{companycode}','{branchcode}','{""}','{""}','{""}','{"ADMIN"}',sysdate,'{deletedflag}','01.01','02','{SYN_ROWID}','L','{TBL}', '{payment_mode}')";

                        var rowCount = _dbContext.ExecuteSqlCommand(InsertSalesInvoiceQuery);
                        if (rowCount == 0)
                        {
                            throw new Exception("Error while inserting  in sa sales invoice");
                        }

                        #endregion
                        #endregion

                        #endregion
                    }


                    #region  Insert Into Master Transaction
                    string MASTERQUERY = $@"SELECT REFERENCE_NO FROM MASTER_TRANSACTION";
                    List<string> MASTERLIST = this._dbContext.SqlQuery<string>(MASTERQUERY).ToList();
                    Shymphony.invoice_number = SALES_NO;
                    if (Record.MenuItem.Count > 0)
                    {

                        commulative_sub_total_amount = newTotal - cumulative_discount_amount;
                        #region ServiceCharge

                        //Removed because service charge not needed.
                        //commulative_service_charge = commulative_sub_total_amount * 0.1;

                        //AA This was the previous vat amount calculation
                        //commulative_vat_amount = (commulative_sub_total_amount + commulative_service_charge) * 0.13;


                        //AA Now the vat is calcluated by checking whether the service charge is available or not 
                        //and also the total amount is calculated accordingly.
                        #endregion

                        //Inserting Luxery Tax
                        commulative_lt_amount = (commulative_sub_total_amount) * 0.02;
                        commulative_master_amount = (commulative_sub_total_amount + commulative_lt_amount);

                        if (bool.Parse(ConfigurationManager.AppSettings["IsRetailBusiness"]))
                        {
                            commulative_vat_amount = (commulative_sub_total_amount+ commulative_lt_amount) * 0.13;
                            //commulative_master_amount = Math.Round(commulative_sub_total_amount + commulative_vat_amount);
                            commulative_master_amount = (commulative_sub_total_amount + commulative_vat_amount+ commulative_lt_amount);

                        }
                        else
                        {
                            commulative_vat_amount = (commulative_sub_total_amount + commulative_service_charge+ commulative_lt_amount) * 0.13;
                            //commulative_master_amount = Math.Round(commulative_sub_total_amount + commulative_vat_amount + commulative_service_charge);
                            commulative_master_amount = (commulative_sub_total_amount + commulative_vat_amount + commulative_service_charge+ commulative_lt_amount);

                        }

                        //AA This was how the total amount calculated previously
                        //commulative_master_amount = commulative_sub_total_amount * 1.243;



                        Shymphony.totalSales = commulative_sub_total_amount;
                        Shymphony.discountAmount = cumulative_discount_amount;
                        Shymphony.serviceTaxAmount = commulative_service_charge;
                        Shymphony.vatAmount = commulative_vat_amount;
                        Shymphony.taxableSalesVat = commulative_sub_total_amount + commulative_service_charge;

                        if (!MASTERLIST.Contains(InvoiceNumber))
                        {

                            string InsertMasterTransaction = $@"INSERT INTO MASTER_TRANSACTION(VOUCHER_NO,REFERENCE_NO,VOUCHER_AMOUNT,VOUCHER_DATE,CURRENCY_CODE,EXCHANGE_RATE,
                                                                    FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SESSION_ROWID,SYN_ROWID,IS_SYNC_WITH_IRD,IS_REAL_TIME, WORKSTATION_ID)
                                                                    VALUES('{SALES_NO}','{InvoiceNumber}','{commulative_master_amount}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'NRS','1','{formcode}','{companycode}','{branchcode}','{Record.Header[0].EmployeeFirstName}',sysdate,'{deletedflag}','{SYN_ROWID}','{InvoiceNumber}','{iS_SYNC_WITH_IRD}','{iS_REAL_TIME}', '{Record.Header[0].WorkstationID}')";
                            var ROWSSS = _dbContext.ExecuteSqlCommand(InsertMasterTransaction);
                            if (ROWSSS == 0)
                            {
                                throw new Exception("Error while inserting master transaction");
                            }

                        }
                        //}

                        #endregion
                        #endregion

                        #region INSERT INTO CHARGE TRANSACTION TABLE
                        string chargetransactionmaxvaluequery = $@"select TO_CHAR(MAX(TO_NUMBER(TRIM(transaction_no)))) as transaction_no from charge_transaction";
                        string CTMAXVALUE = _dbContext.SqlQuery<string>(chargetransactionmaxvaluequery).FirstOrDefault();
                        int CTMAXVALUEe = Convert.ToInt32(CTMAXVALUE) + 1;
                        var charge_invoice_number = InvoiceNumber;
                        #region INSERT INTO IP_CHARGE_CODE
                        //VT
                        string SELECTCHARGE_CODEQUERY = $@"SELECT CHARGE_CODE FROM IP_CHARGE_CODE";
                        List<string> CHARGECODELIST = this._dbContext.SqlQuery<string>(SELECTCHARGE_CODEQUERY).ToList();

                        if (!CHARGECODELIST.Contains("VT"))
                        {
                            string InsertChargeCodeQuerey = $@"INSERT INTO IP_CHARGE_CODE (CHARGE_CODE,CHARGE_EDESC,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SPECIFIC_CHARGE_FLAG)
                          VALUES('VT','VAT Charge','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','V')";
                            _dbContext.ExecuteSqlCommand(InsertChargeCodeQuerey);
                        }

                        if (!CHARGECODELIST.Contains("SC"))
                        {
                            string InsertChargeCodeQuerey = $@"INSERT INTO IP_CHARGE_CODE (CHARGE_CODE,CHARGE_EDESC,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SPECIFIC_CHARGE_FLAG)
                         VALUES('SC', 'Service Charge','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','E')";
                            _dbContext.ExecuteSqlCommand(InsertChargeCodeQuerey);
                        }

                        if (!CHARGECODELIST.Contains("DC"))
                        {
                            string InsertChargeCodeQuerey = $@"INSERT INTO IP_CHARGE_CODE (CHARGE_CODE,CHARGE_EDESC,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,SPECIFIC_CHARGE_FLAG)
                         VALUES('DC', 'Discount','{companycode}','{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','D')";
                            _dbContext.ExecuteSqlCommand(InsertChargeCodeQuerey);
                        }
                        #endregion
                        #region  INSERT INTO CHARGE_SETUP
                        string SELECTFORM_CODEQUERY = $@"SELECT FORM_CODE FROM CHARGE_SETUP WHERE FORM_CODE={formcode}";
                        List<string> FORMCODELIST = this._dbContext.SqlQuery<string>(SELECTFORM_CODEQUERY).ToList();
                        var a = !FORMCODELIST.Contains(formcode);

                        //VT
                        string SELECTCHARGE_SETUPQUERY = $@"SELECT CHARGE_CODE FROM CHARGE_SETUP WHERE FORM_CODE = {formcode}";
                        List<string> VTCHARGECODELIST = this._dbContext.SqlQuery<string>(SELECTCHARGE_SETUPQUERY).ToList();
                        var b = !VTCHARGECODELIST.Contains("VT");

                        if (!VTCHARGECODELIST.Contains("VT") && !FORMCODELIST.Contains(formcode))
                        {
                            string InsertChargeSetupQuerey = $@"INSERT INTO CHARGE_SETUP (CHARGE_CODE,ACC_CODE,CHARGE_TYPE_FLAG,VALUE_PERCENT_FLAG,VALUE_PERCENT_AMOUNT,FORM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,GL_FLAG,APPLY_FROM_DATE,APPLY_TO_DATE,APPLY_ON)
                    VALUES('VT','{vat_charge_acc_code}','A','V','{"13"}',{formcode},{companycode},'{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','Y',sysdate-360,sysdate+360,'D')";
                            _dbContext.ExecuteSqlCommand(InsertChargeSetupQuerey);
                        }
                        //ST
                        string SCSELECTCHARGE_SETUPQUERY = $@"SELECT CHARGE_CODE FROM CHARGE_SETUP WHERE FORM_CODE = {formcode}";
                        List<string> SCCCHARGECODELIST = this._dbContext.SqlQuery<string>(SCSELECTCHARGE_SETUPQUERY).ToList();
                        if (!SCCCHARGECODELIST.Contains("SC") && !FORMCODELIST.Contains(formcode))
                        {
                            string InsertChargeSetupQuerey = $@"INSERT INTO CHARGE_SETUP (CHARGE_CODE,ACC_CODE,CHARGE_TYPE_FLAG,VALUE_PERCENT_FLAG,VALUE_PERCENT_AMOUNT,FORM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,GL_FLAG,APPLY_FROM_DATE,APPLY_TO_DATE,APPLY_ON)
                    VALUES('SC','{st_charge_acc_code}','A','V','{"10"}',{formcode},{companycode},'{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','Y',sysdate-360,sysdate+360,'D')";
                            _dbContext.ExecuteSqlCommand(InsertChargeSetupQuerey);
                        }
                        //DC
                        string DCSELECTCHARGE_SETUPQUERY = $@"SELECT CHARGE_CODE FROM CHARGE_SETUP WHERE FORM_CODE = {formcode}";
                        List<string> DCCCHARGECODELIST = this._dbContext.SqlQuery<string>(SCSELECTCHARGE_SETUPQUERY).ToList();
                        if (!SCCCHARGECODELIST.Contains("DC") && !FORMCODELIST.Contains(formcode))
                        {
                            string InsertChargeSetupQuerey = $@"INSERT INTO CHARGE_SETUP (CHARGE_CODE,ACC_CODE,CHARGE_TYPE_FLAG,VALUE_PERCENT_FLAG,VALUE_PERCENT_AMOUNT,FORM_CODE,COMPANY_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,GL_FLAG,APPLY_FROM_DATE,APPLY_TO_DATE,APPLY_ON)
                    VALUES('DC','','D','V','{"0"}',{formcode},{companycode},'{"ADMIN"}',TO_DATE('{DateTime.Now.ToString("MM/dd/yyyy")}','MM/dd/yyyy'),'{deletedflag}','N',sysdate-360,sysdate+360,'D')";
                            _dbContext.ExecuteSqlCommand(InsertChargeSetupQuerey);
                        }
                        #endregion
                        #region Insert Vat service and discount

                        var VTSALES_NO = "";

                        VTSALES_NO = SALES_NO;
                        //   #region Inserting vat amount
                        //   if (total_tax_amount > 0)
                        //   {
                        //       string InsertChargeTransactionQuery = $@"Insert into charge_transaction(transaction_no, TABLE_NAME, REFERENCE_NO, SERIAL_NO, APPLY_ON, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, GL_FLAG, ACC_CODE, APPORTION_FLAG, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE)
                        //VALUES(TO_CHAR('{CTMAXVALUEe}'), 'SA_SALES_INVOICE', '{VTSALES_NO}','{SERIAL_NO}', 'D', 'VT', 'A',{commulative_vat_amount}, 'Y', '{vat_charge_acc_code}', 'F', {formcode}, '{companycode}', '{branchcode}', 'ADMIN', SYSDATE, '{deletedflag}', '{"NRS"}', 1)";
                        //       _dbContext.ExecuteSqlCommand(InsertChargeTransactionQuery);
                        //   }
                        //   #endregion
                        //   #region Inserting service charge Amount
                        //   if (cumulative_st_amount > 0)
                        //   {
                        //       string InsertNextChargeTransactionQuery = $@"Insert into charge_transaction(transaction_no, TABLE_NAME, REFERENCE_NO,SERIAL_NO, APPLY_ON, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, GL_FLAG, ACC_CODE, APPORTION_FLAG, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE)
                        //    VALUES(TO_CHAR('{CTMAXVALUEe}'), 'SA_SALES_INVOICE', '{VTSALES_NO}','{SERIAL_NO}', 'D', 'SC', 'A',{commulative_service_charge}, 'Y', '{st_charge_acc_code}', 'F', {formcode}, '{companycode}', '{branchcode}', 'ADMIN', SYSDATE, '{deletedflag}', '{"NRS"}', 1)";
                        //       _dbContext.ExecuteSqlCommand(InsertNextChargeTransactionQuery);
                        //   }
                        //   #endregion

                        #region Inserting Discount
                        if (cumulative_discount_amount > 0)
                        {
                            string InsertChargeTransactionQuery = $@"Insert into charge_transaction(transaction_no, TABLE_NAME, REFERENCE_NO,SERIAL_NO, APPLY_ON, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, GL_FLAG, ACC_CODE, APPORTION_FLAG, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SESSION_ROWID)
                             VALUES(TO_CHAR('{CTMAXVALUEe}'), 'SA_SALES_INVOICE', '{VTSALES_NO}','1', 'D', 'DC', 'D',{cumulative_discount_amount}, 'N', '{discount_acc_code}', 'F', {formcode}, '{companycode}', '{branchcode}', 'ADMIN', SYSDATE, '{deletedflag}', '{"NRS"}', 1,'{SYN_ROWID}')";
                            var discountrow = _dbContext.ExecuteSqlCommand(InsertChargeTransactionQuery);
                            if (discountrow == 0)
                            {
                                throw new Exception("Error while inserting discount in charge transaction");
                            }
                        }
                        #endregion
                        #region Inserting service charge Amount
                        if (commulative_service_charge > 0)
                        {
                            string InsertNextChargeTransactionQuery = $@"Insert into charge_transaction(transaction_no, TABLE_NAME, REFERENCE_NO,SERIAL_NO, APPLY_ON, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, GL_FLAG, ACC_CODE, APPORTION_FLAG, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SESSION_ROWID)
                             VALUES(TO_CHAR('{CTMAXVALUEe}'), 'SA_SALES_INVOICE', '{VTSALES_NO}','2', 'D', 'SC', 'A',{commulative_service_charge}, 'Y', '{st_charge_acc_code}', 'F', {formcode}, '{companycode}', '{branchcode}', 'ADMIN', SYSDATE, '{deletedflag}', '{"NRS"}', 1,'{SYN_ROWID}')";
                            var serviucechargerow = _dbContext.ExecuteSqlCommand(InsertNextChargeTransactionQuery);
                            if (serviucechargerow == 0)
                            {
                                throw new Exception("Error while inserting service charge in charge transaction");
                            }
                        }
                        #endregion

                        #region Inserting luxury tax amount
                        if (commulative_lt_amount > 0)
                        {
                            string InsertChargeTransactionQuery = $@"Insert into charge_transaction(transaction_no, TABLE_NAME, REFERENCE_NO, SERIAL_NO, APPLY_ON, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, GL_FLAG, ACC_CODE, APPORTION_FLAG, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SESSION_ROWID)
                             VALUES(TO_CHAR('{CTMAXVALUEe}'), 'SA_SALES_INVOICE', '{VTSALES_NO}','3', 'D', 'LT', 'A',{commulative_lt_amount}, 'Y', '{lt_charge_acc_code}', 'F', {formcode}, '{companycode}', '{branchcode}', 'ADMIN', SYSDATE, '{deletedflag}', '{"NRS"}', 1,'{SYN_ROWID}')";
                            var ltrow = _dbContext.ExecuteSqlCommand(InsertChargeTransactionQuery);
                            if (ltrow == 0)
                            {
                                throw new Exception("Error while inserting Luxury Tax in charge transaction");
                            }
                        }
                        #endregion
                        #region Inserting vat amount
                        if (commulative_vat_amount > 0)
                        {
                            string InsertChargeTransactionQuery = $@"Insert into charge_transaction(transaction_no, TABLE_NAME, REFERENCE_NO, SERIAL_NO, APPLY_ON, CHARGE_CODE, CHARGE_TYPE_FLAG, CHARGE_AMOUNT, GL_FLAG, ACC_CODE, APPORTION_FLAG, FORM_CODE, COMPANY_CODE, BRANCH_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG, CURRENCY_CODE, EXCHANGE_RATE, SESSION_ROWID)
                             VALUES(TO_CHAR('{CTMAXVALUEe}'), 'SA_SALES_INVOICE', '{VTSALES_NO}','4', 'D', 'VT', 'A',{commulative_vat_amount}, 'Y', '{vat_charge_acc_code}', 'F', {formcode}, '{companycode}', '{branchcode}', 'ADMIN', SYSDATE, '{deletedflag}', '{"NRS"}', 1,'{SYN_ROWID}')";
                            var vatrow = _dbContext.ExecuteSqlCommand(InsertChargeTransactionQuery);
                            if (vatrow == 0)
                            {
                                throw new Exception("Error while inserting vat in charge transaction");
                            }
                        }
                        #endregion


                        #endregion
                        #endregion
                    }
                }
            }
            Record.Sales_No = SALES_NO;

        }
        public void ifIsRealTimeFalse(Shymphony Record)
        {
            using (TransactionScope scopeForUpdate = new TransactionScope())
            {
                try
                {
                    var updatMasterTransactionQuery = $@"UPDATE MASTER_TRANSACTION SET IS_SYNC_WITH_IRD='{"N"}', IS_REAL_TIME='{"N"}' WHERE VOUCHER_NO='{Shymphony.invoice_number}' AND FORM_CODE='{Shymphony.form_code}'";
                    _dbContext.ExecuteSqlCommand(updatMasterTransactionQuery);
                    scopeForUpdate.Complete();
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
        //public class ShymphonyInvoiceModel
        //{
        //    public string SYNERGY_INVOICE { get; set; }
        //    public string CHARGE_TRAN_INVOICE { get; set; }
        //}
        public class ItemChargeModel
        {
            public string ItemCode { get; set; }

        }

        public class CustomersDetail
        {
            public string CustomerName { get; set; }
            public string CustomerId { get; set; }
            public string CustomerCode { get; set; }
        }
    }
}

