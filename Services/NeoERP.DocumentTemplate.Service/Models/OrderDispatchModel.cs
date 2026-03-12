using System.Collections.Generic;
using System;
using Microsoft.SqlServer.Server;

namespace NeoERP.DocumentTemplate.Service.Models
{
    public class OrderDispatchModel
    {
        
        public DateTime VOUCHER_DATE_TYPE { get; set; }
        public decimal? LOADING_SLIP_NO { get; set; }
        public string DISPATCH_NO { get; set; }
        public string ORDER_NO { get; set; }
        public string VOUCHER_NO { get; set; }
        public string ORDER_DATE { get; set; }
        public string VOUCHER_DATE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_CODE { get; set; }
        public string ITEM_CODE { get; set; }
        public decimal QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public decimal UNIT_PRICE { get; set; }
        public decimal SERIAL_NO { get; set; }
        public string MANUAL_NO { get; set; }
        public string CUSTOMER_CODE { get; set; }
        public string FROM_LOCATION { get; set; }
        public string TO_LOCATION { get; set; }
        public string DISPATCH_FLAG { get; set; }
        public string COMPANY_CODE { get; set; }
        public string BRANCH_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string CREATED_BY { get; set; }
        public string CREATED_DATE { get; set; }
        public string MODIFY_DATE { get; set; }
        public string MITI { get; set; }
        public decimal DUE_QTY { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string PARTY_TYPE_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string ADDRESS { get; set; }
        public string AGENT_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string PLANNING_QTY { get; set; }
        public decimal PENDING_TO_DISPATCH { get; set; }
        public string PLANNING_AMOUNT { get; set; }
        public string EXCISE_AMOUNT { get; set; }
        public string VAT_AMOUNT { get; set; }
        public decimal TRANS_NO { get; set; }
        public string ACKNOWLEDGE_FLAG { get; set; }
        public decimal? PendingToPlanning { get; set; }
        public decimal? PendingToDispatch { get; set; }
        public decimal? CONVERSION_FACTOR { get; set; }
        public decimal? FRACTION { get; set; }
        public decimal? STOCK { get; set; }
        public decimal? STOCK_2ND { get; set; }
        public decimal? QUANTITY_2ND { get; set; }
        public decimal? PendingToPlanning2ND { get; set; }
        public decimal? PLANNING_QTY_2ND { get; set; }
        public decimal? PendingToDispatch2ND { get; set; }
        public decimal? LS_PENDING_QTY { get; set; }
    }

    public class DispatchDocument
    {
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
    }

    public class SearchParameter
    {
        public List<CompBranch> CompBrnhList { get; set; }
        public string DocumentCode { get; set; }
        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string PlanningDate { get; set; }
        public string BranchCodeList { get; set; }
    }

    public class DispatchSalesMain
    {
        public decimal? TotalQty { get; set; }
        public decimal? TotalAmount { get; set; }

    }

    public class CompBranch
    {
        public string Company { get; set; }
        public string Branch { get; set; }
    }

    public class DispatchDealer
    {
        public string PARTY_TYPE_CODE { get; set; }
        public string PARTY_TYPE_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
        public string ACC_CODE { get; set; }
        public int? CREDIT_LIMIT { get; set; }
        public int? CREDIT_DAYS { get; set; }
        public string id { get; set; }
        public string label { get; set; }
        public string ADDRESS { get; set; }
    }
}
