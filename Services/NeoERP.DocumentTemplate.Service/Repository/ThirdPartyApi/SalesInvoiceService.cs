using NeoErp.Core;
using NeoErp.Data;
using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Repository.ThirdPartyApi
{
    public class SalesInvoiceService : ISalesInvoice
    {
        private IDbContext _dbContext;

        public SalesInvoiceService(IDbContext dbContext)
        {
            _dbContext = dbContext;

        }
        public List<SalesInvoice> GetSalesInvoice(SalesInvoiceApiParameter salesInvoiceApiParameter)
        {
            DateTime? date1 = salesInvoiceApiParameter.dateTime;
            var date = "";
            //var date2 = "";
            if (date1.HasValue)
            {
                date = date1.Value.ToShortDateString();
                //date2 = date1.Value.ToShortDateString();
                //DateTime startDateTime = DateTime.Parse(date2, CultureInfo.InvariantCulture);
                //DateTime startDateTime1 = DateTime.ParseExact(date2, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
            }
            var invoice = salesInvoiceApiParameter.salesInvoiceNumber;
            SalesInvoice salesInvoice = new SalesInvoice();
            List<SalesInvoice> salesInvoice1 = new List<SalesInvoice>();
            List<MasterTransaction> mTransaction = new List<MasterTransaction>();
            List<string> response = new List<string>();
            if (!string.IsNullOrEmpty(invoice) && date == "")
            {
                string totalSalesInvoiceNumberInDateRange = $@"SELECT SI.SALES_NO AS salesNumber
                                                                        FROM SA_SALES_INVOICE SI
                                                                        INNER JOIN MASTER_TRANSACTION MT ON MT.VOUCHER_NO = SI.SALES_NO
	                                                                        AND MT.FORM_CODE = SI.FORM_CODE
	                                                                        AND MT.COMPANY_CODE = SI.COMPANY_CODE
	                                                                        AND MT.DELETED_FLAG = 'N'
                                                                        WHERE SALES_NO='{invoice}'";
                mTransaction = _dbContext.SqlQuery<MasterTransaction>(totalSalesInvoiceNumberInDateRange).ToList();

                if (mTransaction.Count == 0)
                {
                    salesInvoice.ResponseMessage = "Invalid Invoice Number";
                }
            }
            if ((date != "") && string.IsNullOrEmpty(invoice))
            {
                string totalSalesInvoiceNumberInDateRange = $@"SELECT SI.SALES_NO AS salesNumber
                                                                FROM SA_SALES_INVOICE SI
                                                                INNER JOIN MASTER_TRANSACTION MT ON MT.VOUCHER_NO = SI.SALES_NO
	                                                                AND MT.FORM_CODE = SI.FORM_CODE
	                                                                AND MT.COMPANY_CODE = SI.COMPANY_CODE
	                                                                AND MT.DELETED_FLAG = 'N'
                                                                WHERE SALES_DATE=to_date('{date}','mm/dd/yy')";
                mTransaction = _dbContext.SqlQuery<MasterTransaction>(totalSalesInvoiceNumberInDateRange).ToList();
                if (mTransaction.Count == 0)
                {
                    salesInvoice.ResponseMessage = "Invalid Date";
                }
            }
            if (!string.IsNullOrEmpty(invoice) && date != "")
            {
                string totalSalesInvoiceNumberInDateRange = $@"SELECT SI.SALES_NO AS salesNumber
                                                                        FROM SA_SALES_INVOICE SI
                                                                        INNER JOIN MASTER_TRANSACTION MT ON MT.VOUCHER_NO = SI.SALES_NO
	                                                                        AND MT.FORM_CODE = SI.FORM_CODE
	                                                                        AND MT.COMPANY_CODE = SI.COMPANY_CODE
	                                                                        AND MT.DELETED_FLAG = 'N' 
                                                                        WHERE SALES_DATE=TO_DATE('{date}','mm/dd/yy') and SALES_NO='{invoice}'";
                mTransaction = _dbContext.SqlQuery<MasterTransaction>(totalSalesInvoiceNumberInDateRange).ToList();
                if (mTransaction.Count == 0)
                {
                    salesInvoice.ResponseMessage = "Invalid Date and Invoice";
                }
            }
            salesInvoice.Transactions = new List<MasterTransaction>();
            foreach (var invoiceNumber in mTransaction)
            {
                var salesInvoiceNumber = invoiceNumber.salesNumber;
                string masterTransactionQuery1 = $@"SELECT SI.SALES_NO AS salesNumber
	                                                        ,TO_CHAR(TO_DATE(SI.SALES_DATE) ,'MM/DD/YYYY') AS salesDate
	                                                        ,SI.CURRENCY_CODE AS currencyCode
	                                                        ,TO_CHAR(SI.EXCHANGE_RATE) AS exchangeRate
	                                                        ,PTC.PARTY_TYPE_EDESC AS dealer
	                                                        ,SST.SALES_TYPE_EDESC AS salesType
	                                                        ,AST.AREA_EDESC AS area
	                                                        ,CC.CITY_EDESC AS city
	                                                        ,HES.EMPLOYEE_EDESC AS marketingPerson
                                                        FROM SA_SALES_INVOICE SI
                                                        INNER JOIN MASTER_TRANSACTION MT ON MT.VOUCHER_NO = SI.SALES_NO
	                                                        AND MT.FORM_CODE = SI.FORM_CODE
	                                                        AND MT.COMPANY_CODE = SI.COMPANY_CODE
	                                                        AND MT.DELETED_FLAG = 'N'
                                                        LEFT JOIN IP_PARTY_TYPE_CODE PTC ON PTC.PARTY_TYPE_CODE = SI.PARTY_TYPE_CODE
	                                                        AND PTC.COMPANY_CODE = SI.COMPANY_CODE
                                                        LEFT JOIN AREA_SETUP AST ON AST.AREA_CODE = SI.AREA_CODE
	                                                        AND AST.COMPANY_CODE = SI.COMPANY_CODE
                                                        LEFT JOIN CITY_CODE CC ON CC.CITY_CODE = SI.SHIPPING_ADDRESS
                                                        LEFT JOIN SA_SALES_TYPE SST ON SST.SALES_TYPE_CODE = SI.SALES_TYPE_CODE
	                                                        AND SST.COMPANY_CODE = SI.COMPANY_CODE
                                                        LEFT JOIN HR_EMPLOYEE_SETUP HES ON HES.EMPLOYEE_CODE = SI.EMPLOYEE_CODE
	                                                        AND HES.COMPANY_CODE = SI.COMPANY_CODE
                                                        WHERE SI.SALES_NO = '{salesInvoiceNumber}'";
                //string masterTransactionQuery1 = $@"SELECT SI.SALES_NO AS salesNumber
                //                                         ,TO_CHAR(SI.SALES_DATE) AS salesDate
                //                                         ,SI.CURRENCY_CODE AS currencyCode
                //                                         ,TO_CHAR(SI.EXCHANGE_RATE) AS exchangeRate
                //                                        FROM SA_SALES_INVOICE SI
                //                                        INNER JOIN MASTER_TRANSACTION MT ON MT.VOUCHER_NO = SI.SALES_NO
                //                                                         AND MT.FORM_CODE = SI.FORM_CODE
                //                                                         AND MT.COMPANY_CODE = SI.COMPANY_CODE
                //                                                         AND MT.DELETED_FLAG = 'N'
                //                                        WHERE SALES_NO = '{salesInvoiceNumber}'";
                var transactionList1 = _dbContext.SqlQuery<MasterTransaction>(masterTransactionQuery1).FirstOrDefault();
                MasterTransaction masterTransaction = new MasterTransaction();
                masterTransaction.CustomerInformation = new CustomerInformation();
                masterTransaction.LineItem = new List<LineItem>();
                masterTransaction.ChargeTransactions = new List<ChargeTransactions>();
                CustomerInformation customer = new CustomerInformation();
                string customerInfoListQuery = $@"SELECT CS.CUSTOMER_EDESC AS customerName
	                                                            ,CS.TEL_MOBILE_NO1 AS telMobileNo1
	                                                            ,CS.TEL_MOBILE_NO2 AS telMobileNo2
	                                                            ,CS.REGD_OFFICE_EADDRESS AS address
	                                                            ,CS.TPIN_VAT_NO AS vatNumber
	                                                            ,FN_FETCH_PRE_DESC(CS.COMPANY_CODE,                 'SA_CUSTOMER_SETUP',CS.PRE_CUSTOMER_CODE)  AS groupName
                                                            FROM SA_CUSTOMER_SETUP CS
                                                            INNER JOIN SA_SALES_INVOICE SI ON SI.CUSTOMER_CODE = CS.CUSTOMER_CODE
	                                                            AND SI.COMPANY_CODE = CS.COMPANY_CODE
                                                            WHERE SI.SALES_NO = '{salesInvoiceNumber}'";
                customer = _dbContext.SqlQuery<CustomerInformation>(customerInfoListQuery).FirstOrDefault();
                if (customer != null)
                {
                    masterTransaction.CustomerInformation.customerName = customer.customerName;
                    masterTransaction.CustomerInformation.telMobileNo1 = customer.telMobileNo1;
                    masterTransaction.CustomerInformation.telMobileNo2 = customer.telMobileNo2;
                    masterTransaction.CustomerInformation.address = customer.address;
                    masterTransaction.CustomerInformation.vatNumber = customer.vatNumber;
                    masterTransaction.CustomerInformation.groupName = customer.groupName;
                }
                string lineItemsListQuery = $@"SELECT SI.ITEM_CODE AS itemCode
	                                                            ,IMS.ITEM_EDESC AS itemName
	                                                            ,SI.MU_CODE AS muCode
	                                                            ,SI.DESCRIPTION AS description
	                                                            ,TO_CHAR(SI.QUANTITY) AS quantity
	                                                            ,TO_CHAR(SI.UNIT_PRICE) AS unitPrice
	                                                            ,TO_CHAR(SI.TOTAL_PRICE) AS totalPrice
	                                                            ,TO_CHAR(SI.SERIAL_NO) AS serialNumber
	                                                            ,SI.remarks
                                                            FROM IP_ITEM_MASTER_SETUP IMS
                                                            INNER JOIN SA_SALES_INVOICE SI ON SI.ITEM_CODE = IMS.ITEM_CODE
	                                                            AND SI.COMPANY_CODE = IMS.COMPANY_CODE
                                                            WHERE SI.SALES_NO = '{salesInvoiceNumber}'
                                                            ORDER BY serialNumber";
                var lineItemsList = _dbContext.SqlQuery<LineItem>(lineItemsListQuery).ToList();
                if (lineItemsList.Count > 0)
                {
                    foreach (var item in lineItemsList)
                    {
                        masterTransaction.LineItem.Add(new LineItem
                        {
                            serialNumber = item.serialNumber,
                            itemCode = item.itemCode,
                            itemName = item.itemName,
                            description = item.description,
                            muCode = item.muCode,
                            quantity = item.quantity,
                            unitPrice = item.unitPrice,
                            totalPrice = item.totalPrice,
                            remarks = item.remarks
                        });

                    }
                }
                string chargeTransactionListQuery = $@"SELECT CT.CHARGE_CODE as chargeCode
	                                                                    ,TO_CHAR(CT.CHARGE_AMOUNT) AS chargeAmount
	                                                                    ,CT.APPLY_ON as applyOn
	                                                                    ,CC.CHARGE_EDESC as chargeName
	                                                                    ,CT.CHARGE_TYPE_FLAG as chargeTypeFlag
	                                                                    ,TO_CHAR(CS.PRIORITY_INDEX_NO) AS serialNumber 
                                                                    FROM CHARGE_TRANSACTION CT
                                                                    INNER JOIN CHARGE_SETUP CS ON CS.CHARGE_CODE=CT.CHARGE_CODE 
                                                                       AND CS.COMPANY_CODE = CT.COMPANY_CODE AND CT.FORM_CODE=CS.FORM_CODE
                                                                    INNER JOIN IP_CHARGE_CODE CC ON CC.CHARGE_CODE = CT.CHARGE_CODE
	                                                                    AND CC.COMPANY_CODE = CT.COMPANY_CODE
                                                                    WHERE reference_no = '{salesInvoiceNumber}'
                                                                    ORDER BY serialNumber";
                var chargeTransactionList = _dbContext.SqlQuery<ChargeTransactions>(chargeTransactionListQuery).ToList();
                if (chargeTransactionList.Count > 0)
                {
                    foreach (var charge in chargeTransactionList)
                    {
                        masterTransaction.ChargeTransactions.Add(new ChargeTransactions
                        {
                            chargeCode = charge.chargeCode,
                            chargeAmount = charge.chargeAmount,
                            chargeName = charge.chargeName,
                            applyOn = charge.applyOn,
                            chargeTypeFlag = charge.chargeTypeFlag,
                            serialNumber = charge.serialNumber
                        });

                    }
                }
                salesInvoice.Transactions.Add(new MasterTransaction
                {
                    salesNumber = transactionList1.salesNumber,
                    salesDate = transactionList1.salesDate,
                    //salesAmount = transactionList1.salesAmount,
                    currencyCode = transactionList1.currencyCode,
                    exchangeRate = transactionList1.exchangeRate,
                    dealer = transactionList1.dealer,
                    salesType = transactionList1.salesType,
                    area = transactionList1.area,
                    city = transactionList1.city,
                    marketingPerson = transactionList1.marketingPerson,
                    CustomerInformation = masterTransaction.CustomerInformation,
                    LineItem = masterTransaction.LineItem,
                    ChargeTransactions = masterTransaction.ChargeTransactions

                }
                   );

            }
            salesInvoice1.Add(salesInvoice);
            return salesInvoice1;
        }
    }
}
