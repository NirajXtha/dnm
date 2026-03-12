
using NeoErp.Core;
using NeoErp.Core.Infrastructure.DependencyManagement;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface;
using NeoERP.DocumentTemplate.Service.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace NeoERP.DocumentTemplate.Service.Services
{

    public class IRDDataSyncService : IIRDDataSyncService
    {

        private IDbContext _dbContext;
        private NeoErpCoreEntity _objectEntity;
        private IWorkContext _workContext;
        public IRDDataSyncService(IDbContext dbContext, NeoErpCoreEntity objEntity, IWorkContext workContext)
        {
            this._dbContext = dbContext;
            this._objectEntity = objEntity;
            this._workContext = workContext;
        }

        public string IRDSyncInvoice(string dataString)
        {
            return "success sync data";
        }

        public string FormatBSDate(string date)
        {
            if (string.IsNullOrEmpty(date))
                return string.Empty;

            return date.Replace("-", ".");
        }


        public void SaveIRDLog(IRDSyncDataModel Record, string Message)
        {
            try
            {
                var updatMasterTransactionQuery = $@"insert into IRD_LOG(VOUCHER_NO,MESSAGE,FORM_CODE,CREATED_DATE, AMOUNT, REQUEST_BY, REQUEST_JSON, RESPONSE_JSON, TAX_AMOUNT, VAT) values ('{Record.BILL_NO}','{Message}','{Record.FORM_CODE}',sysdate, '{Record.AMOUNT}','{Record.REQUESTED_BY}','{Record.REQUESTED_JSON}','{Record.RESPONSE_JSON}', '{Record.TAX_AMOUNT}', '{Record.VAT}')";

                _dbContext.ExecuteSqlCommand(updatMasterTransactionQuery);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateMasterTransactionIsRealTimeFalse(IRDSyncDataModel Record)
        {
            try
            {
                var updatMasterTransactionQuery = $@"UPDATE MASTER_TRANSACTION SET IS_SYNC_WITH_IRD='{"Y"}', IS_REAL_TIME='{"Y"}' WHERE VOUCHER_NO='{Record.BILL_NO}' AND FORM_CODE='{Record.FORM_CODE}'";
                _dbContext.ExecuteSqlCommand(updatMasterTransactionQuery);
            }
            catch (Exception)
            {
                throw;
            }
        }




        public string IRDSyncInvoice(FormDetails modelObj)
        {

            // Capture user info from the current request thread
            var currentUser = _workContext.CurrentUserinformation;
            var userid = currentUser.User_id;
            var company_code = currentUser.company_code;
            var branch_code = currentUser.branch_code;
            var requestBy = currentUser.login_code;

            // 1. Create HttpClient ONCE to prevent socket exhaustion
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(100); // 100 mins timeout
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialize model with basic data immediately so logging always works
                var model = new IRDSyncDataModel
                {
                    BILL_NO = modelObj.Order_No,  // Voucher_No
                    FORM_CODE = modelObj.Form_Code, // FormCode
                    COMPANY_CODE = company_code, // companh_c
                    BRANCH_CODE = branch_code,
                    AMOUNT = !string.IsNullOrEmpty(modelObj.Grand_Total) ? Convert.ToDouble(modelObj.Grand_Total) : 0,
                    REQUESTED_BY = requestBy,
                };

                try
                {
                    // Populate full model details
                    model.AMOUNT = !string.IsNullOrEmpty(modelObj.Grand_Total) ? Convert.ToDouble(modelObj.Grand_Total) : 0;
                    model.BILL_DATE = DateTime.Now.ToString();
                    model.FISCAL_YEAR = ConfigurationManager.AppSettings["FiscalYear"];
                    model.TAXABLE_AMOUNT = 0.00;
                    model.TAX_AMOUNT = 0.00;

                    // Fetch config
                    var username = ConfigurationManager.AppSettings["ird_username"];
                    var password = ConfigurationManager.AppSettings["ird_password"];
                    var seller_pan = ConfigurationManager.AppSettings["ird_seller_pan"];
                    var IRDUrl = ConfigurationManager.AppSettings["ird_url"];
                    var IRDUrlReturn = ConfigurationManager.AppSettings["ird_url_return"];
                    var fiscal_year = ConfigurationManager.AppSettings["FiscalYear"];

                    // IRD configuration using DB
                    var appSettingQuery = $@"select * from API_SETTING where Company_Code='{company_code}'";
                    var appSettingResult = _dbContext.SqlQuery<ApiSettingModel>(appSettingQuery).FirstOrDefault();

                    if (appSettingResult != null)
                    {
                        username = appSettingResult.USER_NAME;
                        password = appSettingResult.API_PWD;
                        seller_pan = appSettingResult.PAN_NO.ToString();
                        IRDUrl = appSettingResult.SALES_URL;
                        IRDUrlReturn = appSettingResult.SALES_RETURN_URL;
                    }


                    // Common Data Fetching
                    var sqlQuery = $@"select CUSTOMER_CODE from SA_SALES_INVOICE where SALES_NO='{modelObj.Order_No}'";
                    var resCode = _dbContext.SqlQuery<string>(sqlQuery).FirstOrDefault();
                    var cust_code = resCode != null ? resCode.ToString() : "";

                    var query_customer = $@"select CUSTOMER_EDESC,TPIN_VAT_NO from sa_customer_setup where customer_code = '{cust_code}' and Company_code='{company_code}'";
                    var cResult = _dbContext.SqlQuery<CustomerDetails>(query_customer).FirstOrDefault();

                    var buyer_pan = cResult?.TPIN_VAT_NO ?? "";
                    var buyer_name = cResult?.CUSTOMER_EDESC ?? "";
                    var invoice_date = "";

                    var sqlMitiQuery = $@"select MITI from MASTER_TRANSACTION where VOUCHER_NO='{modelObj.Order_No}'";
                    var mitiStringResult = _dbContext.SqlQuery<string>(sqlMitiQuery).FirstOrDefault();

                    if (!string.IsNullOrEmpty(mitiStringResult))
                        invoice_date = FormatBSDate(mitiStringResult);
                    else
                    {
                        var dateQuery = $@"select bs_date(trunc(sysdate)) from dual";
                        var bsDate = _dbContext.SqlQuery<string>(dateQuery).FirstOrDefault();
                        invoice_date = FormatBSDate(bsDate);
                    }

                    var vatQuery = $@"select SUM(nvl(CHARGE_AMOUNT,0)) from charge_transaction where charge_code = 'VT' and Company_code = '{company_code}' and Reference_No = '{modelObj.Order_No}' and Form_Code = '{modelObj.Form_Code}'";
                    double? vat = _dbContext.SqlQuery<double?>(vatQuery).FirstOrDefault();

                    //// === Return Logic ===
                    //if (_bgFormRepo.IsReturnType(item.VOUCHER_NO) == "TRUE")
                    //{
                    //    isProcessed = true;
                    //    var taxableVatQuery = $@"select sum(nvl(quantity,0) * nvl(net_taxable_rate, 0))  
                    //                                                from sa_sales_return 
                    //                                                where return_no='{item.VOUCHER_NO}' 
                    //                                                and company_code='{company_code}' 
                    //                                                and form_code='{item.Form_CODE}'";
                    //    var taxable_sales_vat = _bgDbContext.SqlQuery<double>(taxableVatQuery).FirstOrDefault();

                    //    var ref_noQuery = $@"select reference_no from reference_detail where
                    //                                                 voucher_no = '{item.VOUCHER_NO}'
                    //                                                 and company_code = '{company_code}'
                    //                                                 and form_code = '{item.Form_CODE}'
                    //                                                 group by
                    //                                                 voucher_no,reference_no";
                    //    var reference_no = _bgDbContext.SqlQuery<string>(ref_noQuery).FirstOrDefault();

                    //    var total_sales = Convert.ToDouble(item.VOUCHER_AMOUNT.ToString().Replace(",", ""));
                    //    var taxable_sales_vat1 = Convert.ToDouble(taxable_sales_vat.ToString().Replace(",", ""));

                    //    var billReturnViewModel = new BillReturnViewModel()
                    //    {
                    //        username = username,
                    //        password = password,
                    //        seller_pan = seller_pan,
                    //        buyer_pan = string.IsNullOrEmpty(buyer_pan) ? "0" : buyer_pan,
                    //        buyer_name = buyer_name,
                    //        fiscal_year = fiscal_year,
                    //        ref_invoice_number = string.IsNullOrEmpty(reference_no) ? "" : reference_no,
                    //        credit_note_date = invoice_date,
                    //        credit_note_number = item.VOUCHER_NO,
                    //        reason_for_return = string.IsNullOrEmpty(item.REMARKS) ? "" : item.REMARKS,
                    //        total_sales = total_sales,
                    //        taxable_sales_vat = taxable_sales_vat1,
                    //        vat = vat ?? 0,
                    //        excisable_amount = 0,
                    //        excise = 0,
                    //        taxable_sales_hst = 0,
                    //        hst = 0,
                    //        amount_for_esf = 0,
                    //        esf = 0,
                    //        export_sales = 0,
                    //        tax_exempted_sales = 0,
                    //        isrealtime = true,
                    //        datetimeclient = DateTime.Now
                    //    };

                    //    try
                    //    {
                    //        var response = client.PostAsJsonAsync(IRDUrlReturn, billReturnViewModel).Result;
                    //        if (response.IsSuccessStatusCode)
                    //        {
                    //            var result = response.Content.ReadAsStringAsync().Result;
                    //            if (result == "100") saveLog(model, "API credentials do not match");
                    //            else if (result == "101") { saveLog(model, "bill does not exists"); updateTrans(model); }
                    //            else if (result == "102") saveLog(model, "exception while saving bill details , Please check model fields and values\r\n");
                    //            else if (result == "103") saveLog(model, "Unknown exceptions, Please check API URL and model fields and values");
                    //            else if (result == "104") saveLog(model, "model invalid");
                    //            else if (result == "105") saveLog(model, "Bill does not exists (for Sales Return)");
                    //            else if (result == "200") { saveLog(model, "success"); updateTrans(model); }
                    //            else { saveLog(model, "Error Not Defined: " + result); updateTrans(model); }
                    //        }
                    //        else
                    //        {
                    //            var result = response.Content.ReadAsStringAsync().Result;
                    //            saveLog(model, "HTTP Error: " + response.StatusCode + " " + result);
                    //        }
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        saveLog(model, "Exception: " + ex.Message);
                    //    }
                    //}

                    // === Sales Invoice (Bill) Sync process ===

                    var taxableVatQuery = $@"select sum(nvl(quantity,0) * nvl(net_taxable_rate,0)) 
                                                                 from sa_sales_invoice 
                                                                 where sales_no='{modelObj.Order_No}' 
                                                                 and company_code='{company_code}' 
                                                                 and form_code='{modelObj.Form_Code}'";
                    var taxable_sales_vat = _dbContext.SqlQuery<double>(taxableVatQuery).FirstOrDefault();

                    var total_sales = modelObj.Grand_Total != null ? Convert.ToDouble(modelObj.Grand_Total.ToString().Replace(",", "")) : 0;
                    var taxable_sales_vat1 = taxable_sales_vat != null ? Convert.ToDouble(taxable_sales_vat.ToString().Replace(",", "")) : 0;


                    model.TAX_AMOUNT = taxable_sales_vat1;
                    model.VAT = vat;

                    var billViewModelIRD = new BillViewModelIRD()
                    {
                        username = username,
                        password = password,
                        seller_pan = seller_pan,
                        buyer_pan = string.IsNullOrEmpty(buyer_pan) ? "0" : buyer_pan,
                        buyer_name = buyer_name,
                        fiscal_year = fiscal_year,
                        invoice_number = modelObj.Order_No,
                        invoice_date = invoice_date,
                        total_sales = total_sales,
                        taxable_sales_vat = taxable_sales_vat1,
                        vat = vat ?? 0,
                        excisable_amount = 0,
                        excise = 0,
                        taxable_sales_hst = 0,
                        hst = 0,
                        amount_for_esf = 0,
                        esf = 0,
                        export_sales = 0,
                        tax_exempted_sales = 0,
                        isrealtime = false,
                        datetimeclient = DateTime.Now
                    };

                    try
                    {
                        // model.RequestJson = JsonConvert.SerializeObject(billViewModelIRD, Formatting.None);

                        var json = JsonConvert.SerializeObject(billViewModelIRD);
                        model.REQUESTED_JSON = json;
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(IRDUrl, content).Result;

                        // Capture response JSON / text
                        var responseContent = response.Content.ReadAsStringAsync().Result;
                        model.RESPONSE_JSON = responseContent;

                        if (response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            if (result == "100") SaveIRDLog(model, "API credentials do not match");
                            else if (result == "101") { SaveIRDLog(model, "bill already exists"); UpdateMasterTransactionIsRealTimeFalse(model); }
                            else if (result == "102") SaveIRDLog(model, "exception while saving bill details , Please check model fields and values");
                            else if (result == "103") SaveIRDLog(model, "Unknown exceptions");
                            else if (result == "104") SaveIRDLog(model, "Model invalid");
                            else if (result == "200") { SaveIRDLog(model, "Success"); UpdateMasterTransactionIsRealTimeFalse(model); }
                            else { SaveIRDLog(model, "Error Not Defined: " + result); UpdateMasterTransactionIsRealTimeFalse(model); }
                        }
                        else
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            SaveIRDLog(model, "HTTP Error: " + response.StatusCode + " " + result);
                        }
                    }
                    catch (Exception ex)
                    {
                        SaveIRDLog(model, "Exception : " + " " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    SaveIRDLog(model, "Local Exception: " + ex.Message);
                }

            }
            return "Sync done!";
        }

        public string IRDSyncSalesReturn(FormDetails modelObj)
        {
            // Capture user info from the current request thread
            var currentUser = _workContext.CurrentUserinformation;
            var userid = currentUser.User_id;
            var company_code = currentUser.company_code;
            var branch_code = currentUser.branch_code;
            var requestBy = currentUser.login_code;

            // 1. Create HttpClient ONCE to prevent socket exhaustion
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(100); // 100 mins timeout
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Initialize model with basic data immediately so logging always works
                var model = new IRDSyncDataModel
                {
                    BILL_NO = modelObj.Order_No,  // Voucher_No
                    FORM_CODE = modelObj.Form_Code, // FormCode
                    COMPANY_CODE = company_code, // companh_c
                    BRANCH_CODE = branch_code,
                    AMOUNT = !string.IsNullOrEmpty(modelObj.Grand_Total) ? Convert.ToDouble(modelObj.Grand_Total) : 0,
                    REQUESTED_BY = requestBy,
                };

                try
                {
                    // Populate full model details
                    model.AMOUNT = !string.IsNullOrEmpty(modelObj.Grand_Total) ? Convert.ToDouble(modelObj.Grand_Total) : 0;
                    model.BILL_DATE = DateTime.Now.ToString();
                    model.FISCAL_YEAR = ConfigurationManager.AppSettings["FiscalYear"];
                    model.TAXABLE_AMOUNT = 0.00;
                    model.TAX_AMOUNT = 0.00;

                    // Fetch config
                    var username = ConfigurationManager.AppSettings["ird_username"];
                    var password = ConfigurationManager.AppSettings["ird_password"];
                    var seller_pan = ConfigurationManager.AppSettings["ird_seller_pan"];
                    var IRDUrl = ConfigurationManager.AppSettings["ird_url"];
                    var IRDUrlReturn = ConfigurationManager.AppSettings["ird_url_return"];

                    var fiscal_year = ConfigurationManager.AppSettings["FiscalYear"];


                    // IRD configuration using DB
                    var appSettingQuery = $@"select * from API_SETTING where Company_Code='{company_code}'";
                    var appSettingResult = _dbContext.SqlQuery<ApiSettingModel>(appSettingQuery).FirstOrDefault();
                    if (appSettingResult != null)
                    {
                        username = appSettingResult.USER_NAME;
                        password = appSettingResult.API_PWD;
                        seller_pan = appSettingResult.PAN_NO.ToString();
                        IRDUrl = appSettingResult.SALES_URL;
                        IRDUrlReturn = appSettingResult.SALES_RETURN_URL;
                    }


                    // Common Data Fetching
                    var sqlQuery = $@"select CUSTOMER_CODE from SA_SALES_INVOICE where SALES_NO='{modelObj.Order_No}'";
                    var resCode = _dbContext.SqlQuery<string>(sqlQuery).FirstOrDefault();
                    var cust_code = resCode != null ? resCode.ToString() : "";

                    var query_customer = $@"select CUSTOMER_EDESC,TPIN_VAT_NO from sa_customer_setup where customer_code = '{cust_code}' and Company_code='{company_code}'";
                    var cResult = _dbContext.SqlQuery<CustomerDetails>(query_customer).FirstOrDefault();

                    var buyer_pan = cResult?.TPIN_VAT_NO ?? "";
                    var buyer_name = cResult?.CUSTOMER_EDESC ?? "";
                    var invoice_date = "";

                    var sqlMitiQuery = $@"select MITI from MASTER_TRANSACTION where VOUCHER_NO='{modelObj.Order_No}'";
                    var mitiStringResult = _dbContext.SqlQuery<string>(sqlMitiQuery).FirstOrDefault();

                    if (!string.IsNullOrEmpty(mitiStringResult))
                        invoice_date = FormatBSDate(mitiStringResult);
                    else
                    {
                        var dateQuery = $@"select bs_date(trunc(sysdate)) from dual";
                        var bsDate = _dbContext.SqlQuery<string>(dateQuery).FirstOrDefault();
                        invoice_date = FormatBSDate(bsDate);
                    }

                    var vatQuery = $@"select SUM(nvl(CHARGE_AMOUNT,0)) from charge_transaction where charge_code = 'VT' and Company_code = '{company_code}' and Reference_No = '{modelObj.Order_No}' and Form_Code = '{modelObj.Form_Code}'";
                    double? vat = _dbContext.SqlQuery<double?>(vatQuery).FirstOrDefault();

                    // === return logic ===
                    var taxableVatQuery = $@"select sum(nvl(quantity,0) * nvl(net_taxable_rate, 0))  
                                                                    from sa_sales_return 
                                                                    where return_no='{modelObj.Order_No}' 
                                                                    and company_code='{company_code}' 
                                                                    and form_code='{modelObj.Form_Code}'";
                    var taxable_sales_vat = _dbContext.SqlQuery<double>(taxableVatQuery).FirstOrDefault();

                    var ref_noQuery = $@"select reference_no from reference_detail where
                                                                     voucher_no = '{modelObj.Order_No}'
                                                                     and company_code = '{company_code}'
                                                                     and form_code = '{modelObj.Form_Code}'
                                                                     group by
                                                                     voucher_no,reference_no";
                    var reference_no = _dbContext.SqlQuery<string>(ref_noQuery).FirstOrDefault();


                    var total_sales = modelObj.Grand_Total != null ? Convert.ToDouble(modelObj.Grand_Total.ToString().Replace(",", "")) : 0;
                    var taxable_sales_vat1 = taxable_sales_vat != null ? Convert.ToDouble(taxable_sales_vat.ToString().Replace(",", "")) : 0;


                    model.TAX_AMOUNT = taxable_sales_vat1;
                    model.VAT = vat;

                    var billReturnViewModel = new BillReturnViewModel()
                    {
                        username = username,
                        password = password,
                        seller_pan = seller_pan,
                        buyer_pan = string.IsNullOrEmpty(buyer_pan) ? "0" : buyer_pan,
                        buyer_name = buyer_name,
                        fiscal_year = fiscal_year,
                        ref_invoice_number = string.IsNullOrEmpty(reference_no) ? "" : reference_no,
                        credit_note_date = invoice_date,
                        credit_note_number = modelObj.Order_No,
                        reason_for_return = string.IsNullOrEmpty(modelObj.REMARKS) ? "" : modelObj.REMARKS,
                        total_sales = total_sales,
                        taxable_sales_vat = taxable_sales_vat1,
                        vat = vat ?? 0,
                        excisable_amount = 0,
                        excise = 0,
                        taxable_sales_hst = 0,
                        hst = 0,
                        amount_for_esf = 0,
                        esf = 0,
                        export_sales = 0,
                        tax_exempted_sales = 0,
                        isrealtime = true,
                        datetimeclient = DateTime.Now
                    };

                    try
                    {

                        var json = JsonConvert.SerializeObject(billReturnViewModel);
                        model.REQUESTED_JSON = json;
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        var response = client.PostAsync(IRDUrlReturn, content).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            if (result == "100") SaveIRDLog(model, "API credentials do not match");
                            else if (result == "101") { SaveIRDLog(model, "bill does not exists"); UpdateMasterTransactionIsRealTimeFalse(model); }
                            else if (result == "102") SaveIRDLog(model, "exception while saving bill details , Please check model fields and values\r\n");
                            else if (result == "103") SaveIRDLog(model, "Unknown exceptions, Please check API URL and model fields and values");
                            else if (result == "104") SaveIRDLog(model, "model invalid");
                            else if (result == "105") SaveIRDLog(model, "Bill does not exists (for Sales Return)");
                            else if (result == "200") { SaveIRDLog(model, "success"); UpdateMasterTransactionIsRealTimeFalse(model); }
                            else { SaveIRDLog(model, "Error Not Defined: " + result); UpdateMasterTransactionIsRealTimeFalse(model); }
                        }
                        else
                        {
                            var result = response.Content.ReadAsStringAsync().Result;
                            SaveIRDLog(model, "HTTP Error: " + response.StatusCode + " " + result);
                        }
                    }
                    catch (Exception ex)
                    {
                        SaveIRDLog(model, "Exception: " + ex.Message);
                    }
                }
                catch (Exception ex)
                {
                    SaveIRDLog(model, "Local Exception: " + ex.Message);
                }

            }
            return "Sync done!";
        }




    }

}