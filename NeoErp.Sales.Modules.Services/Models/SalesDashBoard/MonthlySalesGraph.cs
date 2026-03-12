using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.SalesDashBoard
{
    public class MonthlySalesGraph
    {
        public string Month { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string MonthYear { get; set; }
        public string NepaliMonthInt { get; set; }
        public string NepaliMonth { get; set; }
    }

    public class MonthlySalesFiscalYearGraph
    {
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string DisplayMonth { get; set; }
        public string MonthInt { get; set; }
        public string FiscalYear { get; set; }
        public string DBName { get; set; }
        public string Day { get; set; }
    }


    public class SalesProductRateFiscalYearGraph
    {
        public string Item_Code { get; set; }
        public string Item_Desc { get; set; }
        public string APP_Date { get; set; }
        public string MonthInt { get; set; }
        public decimal Sales_Rate { get; set; }
        public string FiscalYear { get; set; }
        public string DBName { get; set; }

    }


    public class SaudaModel
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_DESC { get; set; }
        public decimal DIRECT_QUANTITY { get; set; }
        public decimal SAUDA_QUANTITY { get; set; }   
        public decimal DIRECT_PURCHASE_RATE { get; set; }
        public decimal SAUDA_PURCHASE_RATE { get; set; }
    }


    public class BranchWiseSalesFiscalYearGraph
    {
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Branch_Code { get; set; }
        public string BranchName { get; set; }
        public string FiscalYear { get; set; }
        public string DBName { get; set; }

    }
    public class MonthlyBranchSalesGraph
    {
        public string Month { get; set; }
        public string Nepalimonth { get; set; }
        public string NepaliMonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Branch_Code { get; set; }
        public string BranchName { get; set; }
    }
    public class MonthlyBranchSalesBill
    {
        public string Month { get; set; }
        public string Nepalimonth { get; set; }
        public string NepaliMonthInt { get; set; }
        public string MonthYear { get; set; }
      
        public decimal BillCount { get; set; }
        public string Branch_Code { get; set; }
        public string BranchName { get; set; }
    }
    public class MonthlyDivisionSalesGraph
    {
        public string Month { get; set; }
        public string DivisionCode { get; set; }
        public string MonthInt { get; set; }
        public decimal Quantity { get; set; }
        public string MonthYear { get; set; }
        public decimal Amount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string BranchName { get; set; }
        public string DivisionName { get; set; }
    }
    public class DaysSalesGraph
    {
        public string MonthEnglish { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public string day { get; set; }
        public string MonthNo { get; set; }
        public string Nepalimonth { get; set; }
    }
    public class BranchDaysSalesGraph
    {
        public string Month { get; set; }
        public string MonthNo { get; set; }
        public string day { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string BranchName { get; set; }
        public string BsYear_Month { get; set; }
    }


    public class TargetCollectionGraph
    {
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Target { get; set; }
        public decimal Collection { get; set; }
    }

    public class TargetCollectionGraphDayWise
    {
        public string Month { get; set; }
        public decimal Target { get; set; }
        public decimal Collection { get; set; }
        public string day { get; set; }
        public string MonthNo { get; set; }

    }

    public class SalesCollectionGraph
    {
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public string NepaliMonth { get; set; }
        public string NepaliMonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Sales { get; set; }
        public decimal Collection { get; set; }
    }

    public class SalesCollectionGraphDayWise
    {
        public string MonthEnglish { get; set; }
        public string Nepalimonth { get; set; }
        public decimal Sales { get; set; }
        public decimal Collection { get; set; }
        public string day { get; set; }
        public string MonthNo { get; set; }
        public string BranchName { get; set; }
    }

    public class SalesTargetGraphDayWise
    {
        public string Month { get; set; }
        public decimal Sales { get; set; }
        public decimal? Target { get; set; }
        public string day { get; set; }
        public string MonthNo { get; set; }
    }


    public class SalesCollectionDivisionDayWise
    {
        public string Month { get; set; }
        public decimal Sales { get; set; }
        public decimal Collection { get; set; }
        public string day { get; set; }
        public string MonthNo { get; set; }
        public string Division { get; set; }
    }


    public class SalesTargetGraph
    {
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Sales { get; set; }
        public decimal Target { get; set; }
    }

    public class BranchWiseSalesCollection
    {
        public decimal? Sales { get; set; }
        public decimal? Collection { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
    }
    public class BranchWiseTargetCollection
    {
        public decimal Target { get; set; }
        public decimal Collection { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
    }

    public class BranchWiseSalesTarget
    {
        public decimal? Sales { get; set; }
        public decimal? Target { get; set; }
        public string Branch { get; set; }
        public string BranchName { get; set; }
    }

    public class DivisionDaysSalesGraph
    {
        public string MonthEnglish { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string day { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string MonthNo { get; set; }
        public string BranchName { get; set; }
        public string DivisionName { get; set; }
    }

    public class Employee
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string PersonalEmail { get; set; }

    }
    public class Division
    {
        public string DivisionCode { get; set; }
        public string DivisionName { get; set; }

    }

    public class DistArea
    {
        public string AREA_CODE { get; set; }
        public string AREA_NAME { get; set; }

    }
    public class Agent
    {
        public string AgentCode { get; set; }
        public string AgentName { get; set; }

    }

    public class EmployeeWiseReport
    {
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Month { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class WeekWiseCollectionReport
    {
        public string week { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
    }


    public class ExpensesTrendReport
    {
        public string Month { get; set; }
        public string MonthInt { get; set; }
        public decimal Amount { get; set; }
        public string Account { get; set; }
        public string Master_Acc_Code { get; set; }
        public string acc_group { get; set; }
        public string Day { get; set; }
        public string MonthYear { get; set; }
    }

    public class GrossProfitReport
    {
        public string MonthsName { get; set; }
        public string MonthsDisplayName { get; set; }
        public string yearmonth { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalLandedCost { get; set; }
        public decimal TotalProfitPer { get; set; }
        public int Monthsint { get; set; }
        public string CategoryName { get; set; }
        public string CategoryCode { get; set; }
        public string day { get; set; }
    }


    public class GrossProfitReportDayWise
    {
        public string Day { get; set; }
        public string MonthsName { get; set; }
        public string MonthsDisplayName { get; set; }
        public string yearmonth { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal TotalLandedCost { get; set; }
        public decimal TotalProfitPer { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }

    }
    public class PendingVoucherModel
    {
        public string VoucherType { get; set; }
        public string BranchName { get; set; }
        public int PendingVoucherCount { get; set; }
    }

    public class CompanySalesModel
    {
        public decimal Amount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Quantity { get; set; }
        public string Yearmonth { get; set; }
        public string Month { get; set; }
        public string Companycode { get; set; }
        public string Companyname { get; set; }
    }

    public class CompanyProductionModel
    {
        public decimal Amount { get; set; }
        public decimal Quantity { get; set; }
        public string Yearmonth { get; set; }
        public string Month { get; set; }
        public string Companycode { get; set; }
        public string Companyname { get; set; }
    }


    public class StockChartModel
    {

        public decimal Quantity { get; set; }
        public string Companycode { get; set; }
        public string Companyname { get; set; }
        public string Branch_Code { get; set; }
        public string BranchName { get; set; }
        public string Category_Code { get; set; }
        public string CategoryName { get; set; }
        public string Item_Code { get; set; }
        public string ItemName { get; set; }

    }

    public class MonthlyLoan
    {
        public string MONTH { get; set; }
        public decimal? BALANCE_AMOUNT { get; set; }
        public string COMPANY_CODE { get; set; }
        public String DBName { get; set; }
    }


    public class BalanceChartModel
    {

        public decimal Amount { get; set; }
        public decimal Limit { get; set; }
        public string Companycode { get; set; }
        public string Companyname { get; set; }
        public string Branch_Code { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
        public decimal NetProfit { get; set; }


    }

    public class MonthlyLoanReportModel
    {
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_DATE { get; set; }
        public string MITI { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string MONTHINT { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }

        public string ITEM_SUBGROUP_EDESC { get; set; }
        public string ITEM_GROUP_EDESC { get; set; }


        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }

        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }

        public string BUDGET_CODE { get; set; }
        public string BUDGET_EDESC { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }

        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }

        public decimal? PER_DAY_QUANTITY { get; set; }
        public decimal? PER_DAY_AMOUNT { get; set; }

        public string COMPANY_CODE { get; set; }

        public string REMARKS { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class BalanceChartMonthlyModel
    {

        public decimal Deposit { get; set; }
        public decimal WithDrawn { get; set; }
        public string MonthYear { get; set; }
        public string BankName { get; set; }
        public string BranchName { get; set; }
    }



    public class CompanyExpenseModel
    {
        public decimal Amount { get; set; }
        public string YearMonth { get; set; }
        public string Month { get; set; }
        public string CompanyCode { get; set; }
        public string CompanyName { get; set; }
    }

    public class MonthlyBranchProductionGraph
    {
        public string Month { get; set; }
        public string Nepalimonth { get; set; }
        public string NepaliMonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string Branch_Code { get; set; }
        public string BranchName { get; set; }
    }





    public class MonthlyBranchExpenseGraph
    {
        public string Month { get; set; }
        public string Nepalimonth { get; set; }
        public string NepaliMonthInt { get; set; }
        public string MonthYear { get; set; }
        public decimal Amount { get; set; }
        public string Branch_Code { get; set; }
        public string BranchName { get; set; }
    }




    public class BranchDaysProductionGraph
    {
        public string Month { get; set; }
        public string MonthNo { get; set; }
        public string day { get; set; }
        public decimal Quantity { get; set; }
        public decimal Amount { get; set; }
        public string BranchName { get; set; }
        public string BsYear_Month { get; set; } // to get year-month 2073-04
    }




    public class BranchDaysExpenseGraph
    {
        public string Month { get; set; }
        public string MonthNo { get; set; }
        public string day { get; set; }
        public decimal Amount { get; set; }
        public string BranchName { get; set; }
        public string BsYear_Month { get; set; } // to get year-month 2073-04
        public string Acc_Edesc { get; set; }
    }


    public class UserwiseChartList
    {
        public string MENU_NO { get; set; }
        public string MENU_EDESC { get; set; }
        public string MENU_OBJECT_NAME { get; set; }
        public string USER_ID { get; set; }
        public string ORDER_NO { get; set; }
        public string MODULE_CODE { get; set; }
        public string MODULE_EDESC { get; set; }
        public string FULL_PATH { get; set; }
        public string MODULE_ABBR { get; set; }
        public string COLOR { get; set; }
        public string DESCRIPTION { get; set; }
        public string ICON_PATH { get; set; }
        public int? ORDERBY { get; set; }
        public string VIRTUAL_PATH { get; set; }
        public string QuickCap { get; set; }
    }

    public class NCRChartModel
    {
        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }
        public string MASTER_CUSTOMER_CODE { get; set; }
        public string PRE_CUSTOMER_CODE { get; set; }
        public decimal QUANTITY { get; set; }
        public decimal TOTAL_SALES { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
    }

    public class ItemCustomerTreeModel
    {
        public decimal? SERIAL_NO { get; set; }
        public decimal? PARENTNO { get; set; }
        public string PRODUCT { get; set; }

        public decimal? TD_QUANTITY { get; set; }

        public decimal? TD_TOTAL_VALUE { get; set; }
        public decimal? MTD_QUANTITY { get; set; }
        public decimal? MTD_TOTAL_VALUE { get; set; }

        public decimal? YTD_QUANTITY { get; set; }

        public decimal? YTD_TOTAL_VALUE { get; set; }

    }
    public class PlanReportModel
    {
        public string DESCRIPTION { get; set; }
        public string PLAN_CODE { get; set; }
        public string PLAN_EDESC { get; set; }
        public string PLAN_DATE { get; set; }
        public string MITI { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; }
        public string MONTHINT { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }

        public string ITEM_SUBGROUP_EDESC { get; set; }
        public string ITEM_GROUP_EDESC { get; set; }


        public string CUSTOMER_CODE { get; set; }
        public string CUSTOMER_EDESC { get; set; }

        public string BRANCH_CODE { get; set; }
        public string BRANCH_EDESC { get; set; }

        public string BUDGET_CODE { get; set; }
        public string BUDGET_EDESC { get; set; }
        public string DIVISION_CODE { get; set; }
        public string DIVISION_EDESC { get; set; }

        public string EMPLOYEE_CODE { get; set; }
        public string EMPLOYEE_EDESC { get; set; }

        public decimal? PER_DAY_QUANTITY { get; set; }
        public decimal? PER_DAY_AMOUNT { get; set; }
        public decimal? PER_DAY_SALES_QUANTITY { get; set; }
        public decimal? PER_DAY_SALES_AMOUNT { get; set; }

        public string COMPANY_CODE { get; set; }

        public string REMARKS { get; set; }

        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }

    public class Aging
    {
        public string Description { get; set; }
        public decimal? AMount { get; set; }
    }
    public class FiscalYearModel
    {
        public string FISCAL_YEAR_CODE { get; set; }
        public DateTime START_DATE { get; set; }
        public DateTime END_DATE { get; set; }
    }
}