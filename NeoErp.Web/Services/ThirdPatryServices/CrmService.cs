using NeoErp.Core.Models;
using NeoErp.Models.ThirdPartyModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Services.ThirdPatryServices
{
    public class CrmService : ICrmService
    {
        private NeoErpCoreEntity _dbContext;
        public CrmService(NeoErpCoreEntity dbContext)
        {
            this._dbContext = dbContext;
        }
        public List<BranchModel> GetBranchList()
        {
            string Query = string.Empty;
            Query = @"SELECT BRANCH_CODE AS BranchId,BRANCH_EDESC AS BranchName,ADDRESS AS Address FROM FA_BRANCH_SETUP WHERE DELETED_FLAG='N'";
            var result = this._dbContext.SqlQuery<BranchModel>(Query).ToList();
            return result;
        }

        public List<CategoryModel> GetCategoryList()
        {
            string Query = string.Empty;
            Query = @"SELECT CATEGORY_CODE AS CatId,CATEGORY_EDESC AS CatName FROM IP_CATEGORY_CODE WHERE DELETED_FLAG='N'";
            var result = this._dbContext.SqlQuery<CategoryModel>(Query).ToList();
            return result;
        }

        public List<ItemModel> GetItemList()
        {
            string Query = string.Empty;
            Query = @"SELECT ITEM_CODE AS ItemId,ITEM_EDESC AS ItemName,CATEGORY_CODE AS CategoryId,INDEX_MU_CODE AS UnitId FROM IP_ITEM_MASTER_SETUP WHERE DELETED_FLAG='N'";
            var result = this._dbContext.SqlQuery<ItemModel>(Query).ToList();
            return result;
        }

        public List<StockModel> GetStockList()
        {
            string Query = string.Empty;
            Query = @"SELECT * FROM(
                    SELECT SL.ITEM_CODE ItemNumber,
                                     IMS.ITEM_EDESC ItemName,
                                     IMS.INDEX_MU_CODE UnitId,
                                     MC.MU_EDESC UnitName,
                                     IMS.CATEGORY_CODE CatId,
                                     CC.CATEGORY_EDESC CatName,
                                     SL.LOCATION_CODE HubId,
                                     SL.LOCATION_EDESC HubName,
                                     SL.BRANCH_CODE BranchId,
                                     BS.BRANCH_EDESC BranchName,
                                     SUM (SL.IN_QUANTITY - SL.OUT_QUANTITY) StockBalQty
                                FROM V$VIRTUAL_STOCK_WIP_LEDGER1 SL,
                                     FA_BRANCH_SETUP BS,
                                     IP_ITEM_MASTER_SETUP IMS,
                                     IP_MU_CODE MC,
                                     IP_CATEGORY_CODE CC
                               WHERE     1 = 1
                                     AND SL.ITEM_CODE = IMS.ITEM_CODE
                                     AND SL.COMPANY_CODE = BS.COMPANY_CODE
                                     AND SL.COMPANY_CODE = IMS.COMPANY_CODE
                                     AND IMS.INDEX_MU_CODE = MC.MU_CODE
                                     AND IMS.CATEGORY_CODE = CC.CATEGORY_CODE
                                     AND SL.DELETED_FLAG = 'N'
                                     AND BS.DELETED_FLAG = 'N'
                                     AND IMS.DELETED_FLAG = 'N'
                            GROUP BY SL.ITEM_CODE,
                                     IMS.ITEM_EDESC,
                                     IMS.INDEX_MU_CODE,
                                     MC.MU_EDESC,
                                     IMS.CATEGORY_CODE,
                                     CC.CATEGORY_EDESC,
                                     SL.LOCATION_CODE,
                                     SL.LOCATION_EDESC,
                                     SL.BRANCH_CODE,
                                     BS.BRANCH_EDESC
                            ORDER BY BS.BRANCH_EDESC)
                    WHERE StockBalQty > 0";
            var result = this._dbContext.SqlQuery<StockModel>(Query).ToList();
            return result;
        }

        public List<UnitModel> GetUnitList()
        {
            string Query = string.Empty;
            Query = @"SELECT MU_CODE AS UnitId,MU_EDESC AS UnitName FROM IP_MU_CODE WHERE DELETED_FLAG='N'";
            var result = this._dbContext.SqlQuery<UnitModel>(Query).ToList();
            return result;
        }

        public string SaveSalesOrder(SalesOrderModel model)
        {
            string result = string.Empty;
            using (var trans = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    var formCode = "463";
                    var OrderNumber = _dbContext.SqlQuery<string>($"SELECT  FN_NEW_VOUCHER_NO('{model.CompanyCode}','{formCode}',TRUNC(SYSDATE),'SA_SALES_ORDER') FROM DUAL").FirstOrDefault();
                    model.OrderDate = model.OrderDate ?? DateTime.Now.ToString("MM/dd/yyyy");
                    if (string.IsNullOrWhiteSpace(OrderNumber))
                        throw new Exception("Cannot generate Order Number. Please try again.");
                    int serialNum = 1;
                    decimal TotalAmount = 0;
                    int row = 0;
                    foreach (var item in model.Items)
                    {
                        item.UnitPrice = item.UnitPrice == null ? 0 : item.UnitPrice;
                        var total = item.UnitPrice * item.Quantity;
                        string InsertQuery = $@"INSERT INTO SA_SALES_ORDER (ORDER_NO,ORDER_DATE,CUSTOMER_CODE,SERIAL_NO,ITEM_CODE,MU_CODE,QUANTITY,UNIT_PRICE,TOTAL_PRICE,CALC_QUANTITY,CALC_UNIT_PRICE,CALC_TOTAL_PRICE,FORM_CODE,REMARKS,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,DELIVERY_DATE,CURRENCY_CODE,EXCHANGE_RATE,TRACKING_NO,STOCK_BLOCK_FLAG)
                                            VALUES('{OrderNumber}',TO_DATE('{model.OrderDate}','MM/dd/yyyy'),'{model.CustomerCode}','{serialNum}','{item.ItemId}','{item.UnitId}','{item.Quantity}','{item.UnitPrice}','{total}','{item.Quantity}','{item.UnitPrice}','{total}','{formCode}','{model.Remarks}','{model.CompanyCode}','{model.BranchCode}','{model.UserId}',SYSDATE,'N',TO_DATE(SYSDATE),'NRS','1','{model.TransactionRefNo}','N')";
                        row += _dbContext.ExecuteSqlCommand(InsertQuery);
                        serialNum++;
                        TotalAmount += total.Value;
                    }

                    if (row > 0)
                    {
                        //master transaction table
                        string masterQuery = $@"INSERT INTO MASTER_TRANSACTION (VOUCHER_NO,VOUCHER_AMOUNT,FORM_CODE,COMPANY_CODE,BRANCH_CODE,CREATED_BY,CREATED_DATE,DELETED_FLAG,VOUCHER_DATE,CURRENCY_CODE,EXCHANGE_RATE,PRINT_COUNT,PRINT_FLAG)
                                            VALUES('{OrderNumber}','{TotalAmount}','{formCode}','{model.CompanyCode}','{model.BranchCode}','{model.UserId}',TO_DATE(SYSDATE),'N',TO_DATE('{model.OrderDate}','MM/dd/yyyy'),'NRS','1','0','N')";
                        row = _dbContext.ExecuteSqlCommand(masterQuery);
                    }
                    trans.Commit();
                    result = OrderNumber;
                }
                catch (Exception ex)
                {
                    result = "Error";
                    trans.Rollback();
                }
            }
            return result;
        }

        public List<SalesInvoiceModel> FetchSODetails(string SalesNumber, string TransNo)
        {
            string InvoiceQuery = $@"SELECT SALES_NO SalesNumber,TO_CHAR(SALES_DATE,'DD-MON-RRRR') SalesDate,TRACKING_NO TransactionRefNo,REMARKS,
                    ITEM_CODE ItemId,MU_CODE UnitId,QUANTITY,REMARKS,COMPANY_CODE CompanyCode,BRANCH_CODE BranchCode
            FROM SA_SALES_INVOICE WHERE SALES_NO='{SalesNumber}' AND DELETED_FLAG='N'";
            if (!string.IsNullOrWhiteSpace(TransNo))
                InvoiceQuery += $" AND TRACKING_NO='{TransNo}'";
            var result = _dbContext.SqlQuery<SalesInvoiceModel>(InvoiceQuery).ToList();
            return result;
        }
    }
}