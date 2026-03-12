using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Division;
using NeoErp.Sales.Modules.Services.Models.SalesDashBoard;
using NeoErp.Sales.Modules.Services.Models.Stock;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class SalesDashboardController : ApiController
    {
        private ISalesDashboardService _salesService;
        private ISalesRegister _salesRegister;
        public IWorkContext _workContext;
        private ICacheManager _cacheManager;

        public SalesDashboardController(ISalesDashboardService salesService, ISalesRegister salesRegister, IWorkContext workContext,ICacheManager cacheManager)
        {
            this._salesService = salesService;
            this._salesRegister = salesRegister;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
        }

        [HttpPost]
        public List<ChartSalesModel> GetCategorySales(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetCategorySales(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode);
        }
        [HttpPost]
        public List<CategoryWiseSalesModel> GetCategoryStockLevel(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetCategoryStockLevel(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode);
        }
        [HttpPost]
        public List<ChartSalesModel> GetAreaSales(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetAreaSales(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode);
        }
        [HttpPost]
        public List<ChartSalesModel> GetNoOfSales(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            var data = this._salesRegister.GetNoOfbills(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode).Where(x => x.Code != "01.01" && x.Code != "01.05").ToList();
            return data;
        }
        [HttpPost]
        public List<ChartSalesModel> GetProductSalesByCategory(filterOption model, string categoryCode)
        {
            return this._salesRegister.GetProductSalesByCategory(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode);
        }
        [HttpPost]
        public List<ChartSalesModel> GetProductSalesByCategory(filterOption model, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetProductSalesByCategory(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode, customerCode, itemCode, categoryCode2, companyCode, branchCode, partyTypeCode, formCode);
        }

        [HttpPost]
        public List<ChartSalesModel> GetStocklevelByCategory(filterOption model, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetStockLevelByCategory(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode, customerCode, itemCode, categoryCode2, companyCode, branchCode, partyTypeCode, formCode);
        }

        [HttpPost]
        public List<ChartSalesModel> GetDealerSalesByArea(filterOption model, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetProductSalesByArea(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode, customerCode, itemCode, categoryCode2, companyCode, branchCode, partyTypeCode, formCode);
        }
        [HttpPost]
        public List<ChartSalesModel> GetEmployeeSalesByArea(filterOption model, string categoryCode, string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesRegister.GetProductSalesByAreaEmployee(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode, customerCode, itemCode, categoryCode2, companyCode, branchCode, partyTypeCode, formCode);
        }
        [HttpPost]
        public List<MonthlySalesGraph> GetSalesMonthlyReport(filterOption model, bool salesReturn, string DateFormat)
        {
            return this._salesService.GetSalesMonthSummanry(model.ReportFilters, DateFormat, _workContext.CurrentUserinformation, salesReturn);
        }


        [HttpPost]
        public List<MonthlySalesFiscalYearGraph> GetSalesMonthlyFiscalYearReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetSalesMonthFiscal(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public List<MonthlySalesFiscalYearGraph> AgingCategoryWise(filterOption model, string DateFormat)
        {
            return this._salesService.AgingCategoryWise(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public List<MonthlySalesFiscalYearGraph> GetCollectionMonthlyFiscalYearReport(filterOption model, string DateFormat)
        {
            var data = new List<MonthlySalesFiscalYearGraph>();
            try
            {
                data = this._salesService.GetCollectionMonthFiscal(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);

            }
            catch (Exception ex)
            {

            }
            return data;
        }




        [HttpPost]
        public List<SalesCollectionDivisionDayWise> GetSalesCollectionDivisionWiseMonthlyReport(filterOption model, string DateFormat, string divisionCode)
        {
            return this._salesService.GetSalesCollectionDivisionWise(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, divisionCode);
        }



        [HttpPost]
        public List<SalesCollectionDivisionDayWise> GetSalesCollectionDivisionWiseDailyReport(filterOption model, string DateFormat, string month)
        {
            return this._salesService.GetSalesCollectionDivisionWiseDaily(model.ReportFilters, _workContext.CurrentUserinformation, month, DateFormat);
        }


        [HttpPost]
        public List<SalesProductRateFiscalYearGraph> GetSalesProductRateFiscalYearReport(filterOption model, string DateFormat)
        {
            model.ReportFilters.CompanyFilter = model.ReportFilters.CompanyFilter.Count() == 0 ? new List<string> { _workContext.CurrentUserinformation.company_code } : model.ReportFilters.CompanyFilter;
            return this._salesService.GetSalesProductRateFiscalYear(model.ReportFilters, DateFormat);
        }


        [HttpPost]
        public List<SalesProductRateFiscalYearGraph> GetAvgPurchaseRateFiscalYearReport(filterOption model, string duration)
        {
            return this._salesService.GetAvgPurchaseRateFiscalYear(model.ReportFilters, duration, _workContext.CurrentUserinformation);
        }


        [HttpPost]
        public List<SaudaModel> GetSaudaQuantityReport(filterOption model, string duration)
        {
            return this._salesService.GetSaudaQuantityReport(model.ReportFilters, duration, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<SaudaModel> GetSaudaAveragePurchaseRateReport(filterOption model, string duration)
        {
            return this._salesService.GetSaudaAveragePurchaseRateReport(model.ReportFilters, duration, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<BranchWiseSalesFiscalYearGraph> GetSalesBranchWiseFiscalYearReport(filterOption model)
        {
            return this._salesService.GetSalesBranchWiseFiscalYear(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<MonthlySalesFiscalYearGraph> GetSalesDailyFiscalYearReport(filterOption model, string month, string dbName, string DateFormat)
        {
            return this._salesService.GetSalesDailyFiscal(model.ReportFilters, _workContext.CurrentUserinformation, month, dbName, DateFormat);
        }

        [HttpPost]
        public List<DaysSalesGraph> GetSalesDailyReport(filterOption model, string monthName, string customerCode,
            string itemCode, string categoryCode, string companyCode, string branchCode,
            string partyTypeCode, string formCode, string DateFormat)
        {
            return this._salesService.GetDailySalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, DateFormat, false).ToList();
        }


        [HttpPost]
        public List<DaysSalesGraph> GetSalesDailyReport(filterOption model, string DateFormat, string monthName, bool salesReturn)
        {
            return this._salesService.GetDailySalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, DateFormat, salesReturn).ToList();
        }

        [HttpPost]
        public List<ChartSalesModel> GetProductSalesByMonth(filterOption model, string DateFormat, string monthName)
        {
            return this._salesRegister.GetProductSalesByMonth(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, monthName).ToList();
        }



        [HttpPost]
        public List<MonthlyBranchSalesGraph> GetSalesBranchMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType = "GrossAmount")
        {
            return this._salesService.GetMonthlyBranchSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, salesReturn, AmountType);
        }

        [HttpPost]
        public List<MonthlyBranchSalesGraph> GetPurchaseitemGroupMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType = "GrossAmount")
        {
            return this._salesService.GetMonthlyBranchPurchaseGroupsummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, salesReturn, AmountType);
        }
        [HttpPost]
        public List<MonthlyBranchSalesGraph> GetSalesAreaMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType = "GrossAmount")
        {
            return this._salesService.GetMonthlyAreaalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, salesReturn, AmountType);
        }

        [HttpPost]
        public List<MonthlyBranchSalesBill> GetSalesBillBranchMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType = "GrossAmount")
        {
            return this._salesService.GetMonthlyBranchBillCount(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat).Where(x => x.Branch_Code != "01.01" && x.Branch_Code != "01.05").ToList();
        }

        [HttpPost]
        public List<BranchDaysSalesGraph> GetSalesBranchDailyReport(filterOption model, string monthName, string branchName, string DateFormat, bool salesReturn, string AmountType = "GrossAmount")
        {
            var resultData = this._salesService.GetDailyBranchSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, branchName, "", "", "", "01", "", "", "", DateFormat, salesReturn, AmountType).ToList();
            return resultData;

        }
        [HttpPost]
        public List<ChartSalesModel> GetSalesBranchItemWiseReport(filterOption model, string monthCode, string branchCode, string companyCode)
        {
            var resultData = this._salesService.GetItemWiseBranchSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthCode, branchCode, companyCode).ToList();
            return resultData;

        }
        [HttpPost]
        public List<ChartSalesModel> GetPurchaseGroupItemWiseReport(filterOption model, string monthCode, string branchCode, string companyCode)
        {
            var resultData = this._salesService.GetItemWiseBranchPurchaseGroupSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthCode, branchCode, companyCode).ToList();
            return resultData;

        }


        [HttpPost]
        public List<BranchDaysSalesGraph> GetSalesBranchDailyReport(filterOption model, string monthName, string branchName, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat, bool salesReturn, string AmountType)
        {
            var resultData = this._salesService.GetDailyBranchSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, branchName, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, DateFormat, salesReturn, AmountType).ToList();

            return resultData;

        }


        [HttpPost]
        public List<MonthlyDivisionSalesGraph> GetSalesDivisionMonthlyReport(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string DateFormat, string AmountType = "GrossAmount")
        {
            return this._salesService.GetMonthlyDivisionSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, DateFormat, AmountType);
        }

        [HttpPost]
        public List<ChartSalesModel> GetDivisionItemWiseReport(filterOption model, string monthCode, string divisionCode, string companyCode, string dateFormat)
        {
            var userinfo = _workContext.CurrentUserinformation;
            var result = this._salesService.GetDivisionItemWiseReport(model.ReportFilters, userinfo, monthCode, divisionCode, companyCode, dateFormat);
            return result;
        }
        [HttpPost]
        public List<DivisionDaysSalesGraph> GetSalesDivisionDailyReport(filterOption model, string monthName, string divisionName,
            string DateFormat, string AmountType = "GrossAmount")
        {
            return this._salesService.GetDailyDivisionSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, "", "", "", "01", "", divisionName, "", "", DateFormat, AmountType)
                .Where(x => x.MonthEnglish.ToLower().Trim().Equals(monthName.ToLower().Trim())).ToList();
        }

        [HttpPost]
        public List<DivisionDaysSalesGraph> GetSalesDivisionDailyReport(filterOption model, string monthName, string divisionName,
            string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode,
            string formCode, string DateFormat, string AmountType = "GrossAmount")
        {
            return this._salesService.GetDailyDivisionSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, divisionName, partyTypeCode, formCode, DateFormat, AmountType)
                .Where(x => x.MonthEnglish.ToLower().Trim().Equals(monthName.ToLower().Trim())).ToList();
        }



        //start monthly sales target
        [HttpPost]
        public List<SalesTargetGraph> GetSalesTargetMonthlyReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetSalesTargetMonthlyReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }

        [HttpPost]
        public List<SalesTargetGraphDayWise> GetSalesTargetDailyReport(filterOption model, string monthName, string DateFormat, string checkedValue,string monthInt)
        {
            if (checkedValue == "Day")
            {
                return this._salesService.GetSalesTargetDailyReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, monthName, monthInt).ToList();
            }
            else if (checkedValue == "Item")
            {
                return this._salesService.GetSalesTargetItemWiseReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, monthName, monthInt).ToList();
            }
            else
            {
                return this._salesService.GetSalesTargetDailyReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, monthName, monthInt).ToList();
            }
        }

        //end monthly sales target


        //start monthly collection vs target
        [HttpPost]
        public List<TargetCollectionGraph> GetTargetCollectionMonthlyReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetTargetCollectionMonthlyReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }

        [HttpPost]
        public List<TargetCollectionGraphDayWise> GetTargetCollectionDailyReport(filterOption model, string monthName, string DateFormat)
        {

            return this._salesService.GetTargetCollectionDailyReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, monthName).ToList();
        }

        //end monthly collection vs target

        [HttpPost]
        public List<SalesCollectionGraph> GetSalesCollectionMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetSalesCollectionMonthlyReport(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode);
        }

        [HttpPost]
        public List<SalesCollectionGraphDayWise> GetSalesCollectionDailyReport(filterOption model, string monthName, string DateFormat)
        {

            return this._salesService.GetSalesCollectionDailyReport(model.ReportFilters, _workContext.CurrentUserinformation, monthName, DateFormat).ToList();
        }

        [HttpPost]
        public List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(filterOption model, string monthName)
        {
            return this._salesService.GetSalesCollectionBranchWiseReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<BranchWiseSalesCollection> GetSalesCollectionDivisionWiseReport(filterOption model)
        {
            return this._salesService.GetSalesCollectionDivisionWiseReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(filterOption model)
        {
            return this._salesService.GetSalesCollectionBranchWiseReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<BranchWiseSalesCollection> GetSalesCollectionBranchWiseReport(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetSalesCollectionBranchWiseReport(model.ReportFilters, _workContext.CurrentUserinformation, branchCode).ToList();
        }
        [HttpPost]
        public List<BranchWiseSalesCollection> GetSalesCollectionAreaWiseReport(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetSalesCollectionAreaWiseReport(model.ReportFilters, _workContext.CurrentUserinformation, branchCode).ToList();
        }
        [HttpPost]
        public List<BranchWiseTargetCollection> GetTargetCollectionBranchWiseReport(filterOption model)
        {
            return this._salesService.GetTargetCollectionBranchWiseReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<BranchWiseSalesTarget> GetSalesTargetBranchWiseReport(filterOption model)
        {
            return this._salesService.GetSalesTargetBranchWiseReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }


        [HttpPost]
        public List<BranchWiseSalesTarget> GetSalesTargetSegmentWiseReport(filterOption model)
        {
            return this._salesService.GetSalesTargetSegmentReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<MonthlySalesFiscalYearGraph> AgingCategoryWise1(filterOption Model, string DateFormat)
        {
            return this._salesService.AgingCategoryWise(Model.ReportFilters, _workContext.CurrentUserinformation, DateFormat).ToList();
        }
        [HttpPost]
        public List<MonthlySalesFiscalYearGraph> CustomerWiseaging(filterOption Model, string DateFormat)
        {
            return this._salesService.customerWiseaging(Model.ReportFilters, _workContext.CurrentUserinformation, DateFormat).ToList();
        }
        [HttpPost]
        public List<BranchWiseSalesTarget> GetSalesTargetBranchWiseMonthlyReport(filterOption model,string branchcode,string category)
        {
            if(category =="Month")
            { 
            return this._salesService.GetSalesTargetBranchWiseMonthlyReport(model.ReportFilters, _workContext.CurrentUserinformation,branchcode).ToList();
            }
            else if(category == "Item")
            {
                return this._salesService.GetSalesTargetBranchWiseItemsReport(model.ReportFilters, _workContext.CurrentUserinformation, branchcode).ToList();
            }
            else
            {
                return this._salesService.GetSalesTargetBranchWiseMonthlyReport(model.ReportFilters, _workContext.CurrentUserinformation, branchcode).ToList();
            }
        }

        public List<Employee> GetEmployeeList()
        {
            return this._salesService.GetEmployeesList(_workContext.CurrentUserinformation).ToList();
        }

        public List<Employee> GetEmployeeListForScheduler()
        {
            return this._salesService.GetEmployeesListForScheduler(_workContext.CurrentUserinformation).ToList();
        }

        public List<Agent> GetAgentList()
        {
            return this._salesService.GetAgentList(_workContext.CurrentUserinformation).ToList();
        }
        public List<Division> GetDivisionList()
        {
            return this._salesService.GetDivisionList(_workContext.CurrentUserinformation).ToList();
        }

        public List<DistArea> GetDistAreaList()
        {
            return this._salesService.GetDistAreaList(_workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<EmployeeWiseReport> GetEmployeeWiseReport(filterOption model)
        {
            return this._salesService.GetEmployeesSalesReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }


        [HttpPost]
        public List<EmployeeWiseReport> GetEmployeeWiseReport(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string employeeCode)
        {
            return this._salesService.GetEmployeesSalesReport(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, employeeCode).ToList();
        }


        [HttpGet]
        public string GetDashboardWidgets(string type)
        {
            return this._salesService.GetDashboardWidgets(User.Identity.Name, type);
        }

        [HttpGet]
        public bool SaveDashboardWidgets(string order, string type)
        {
            return this._salesService.SaveDashboardWidgets(User.Identity.Name, order, type);
        }

        [HttpGet]
        public bool ResetDashboardWidgets(string type)
        {
            return this._salesService.ResetDashboardWidgets(User.Identity.Name, type);
        }


        [HttpPost]
        public IList<UserwiseChartList> GetUserwiseChartList(string Module_Code)
        {
            return this._salesService.GetUserwiseChartList(Module_Code);
        }

        [HttpPost]
        public IList<UserwiseChartList> GetUserWiseMenuPermission(string Module_Code)
        {
            return this._salesService.GetUserWiseMenuPermission(Module_Code, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public IList<UserwiseChartList> GetUserwiseChartById(UserwiseChartList model)
        {
            return this._salesService.GetUserwiseChartById(model);
        }

        [HttpPost]
        public bool DeleteUserwiseChartList(UserwiseChartList model)
        {
            return this._salesService.DeleteUserwiseChartList(model);
        }



        [HttpPost]
        public List<WeekWiseCollectionReport> GetWeekWiseCollectionReport(filterOption model)
        {
            return this._salesService.GetWeekWiseCollectionReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<WeekWiseCollectionReport> GetWeekWiseCollectionReport(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetWeekWiseCollectionReport(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode).ToList();
        }

        [HttpPost]
        public List<WeekWiseCollectionReport> GetWeekWiseCustomerCollectionReport(filterOption model, string week)
        {

            return this._salesService.GetWeekWiseCustomerCollectionReport(model.ReportFilters, week, _workContext.CurrentUserinformation).ToList();
        }




        //[HttpPost]
        //public List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(filterOption model)
        //{
        //    return this._salesService.GetExpensesTrendMonthlyReport(model.ReportFilters, this._workContext.CurrentUserinformation, "", "", "", "", "", "", "", "").ToList();
        //}

        //[HttpGet]
        //public List<ExpensesTrendReport> GetExpensesTrendAccountWiseReport(filterOption model, string acc_group)
        //{
        //    return this._salesService.GetExpensesTrendAccountWiseReport(model.ReportFilters, acc_group, _workContext.CurrentUserinformation).ToList();

        //}

        // for filter            
        public List<ExpensesTrendReport> GetExpensesTrendAccount()
        {
            return this._salesService.GetExpensesAccount(_workContext.CurrentUserinformation).ToList();
        }

        // First hit
        [HttpPost]
        public List<ExpensesTrendReport> GetExpensesTrendMonthlyReport(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode, string accountCode)
        {
            return this._salesService.GetExpensesTrendMonthlyReport(model.ReportFilters, this._workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, accountCode).ToList();
        }

        //first series click
        [HttpPost]
        public List<ExpensesTrendReport> GetExpensesTrendAccountWiseMonthlyReport(filterOption model, string master_acc_code, string DateFormat)
        {
            return this._salesService.GetExpensesTrendAccountWiseReport(model.ReportFilters, master_acc_code, _workContext.CurrentUserinformation, DateFormat).ToList();
        }

        // second series click
        [HttpPost]
        public List<ExpensesTrendReport> GetExpensesTrendAccountWiseDailyReport(filterOption model, string master_acc_code, string month, string DateFormat)
        {
            return this._salesService.GetExpensesTrendAccountWiseDailyReport(model.ReportFilters, _workContext.CurrentUserinformation, master_acc_code, month, DateFormat).ToList();
        }

        [HttpPost]
        public List<GrossProfitReport> GetGPMonthWiseReport(filterOption model, string customerCode, string itemCode,
            string categoryCode, string companyCode, string branchCode, string partyTypeCode,
            string formCode, string DateFormat)
        {
            if (string.IsNullOrEmpty(DateFormat))
            {
                DateFormat = "AD";
            }
            return this._salesService.GetGPReportMonthWise(model.ReportFilters, _workContext.CurrentUserinformation
                , customerCode, itemCode,
                    categoryCode, companyCode, branchCode, partyTypeCode, formCode, DateFormat).ToList();
        }

        [HttpPost]
        public List<GrossProfitReport> GetGPDayWiseReport(filterOption model, string customerCode, string itemCode,
            string categoryCode, string companyCode, string branchCode, string partyTypeCode,
            string formCode, string month, string DateFormat)
        {
            if (string.IsNullOrEmpty(DateFormat))
            {
                DateFormat = "AD";
            }
            List<GrossProfitReport> result = new List<GrossProfitReport>();
            result = this._salesService.GetGPReportDayWise(model.ReportFilters, _workContext.CurrentUserinformation, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode, month, DateFormat)
                .ToList();
            return result;
        }

        [HttpPost]
        public List<GrossProfitReport> GetGPMonthWiseCategoryReport(filterOption model, string format)
        {
            var data = this._salesService.GetGPReportMonthCategoryWise(model.ReportFilters, _workContext.CurrentUserinformation, format).ToList();
            return data;
        }


        [HttpPost]
        public List<GrossProfitReportDayWise> GetGPDayWiseItemReport(filterOption model, string categoryCode, string month)
        {
            return this._salesService.GetGPDayWiseItemReport(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode, month).ToList();
        }

        [HttpPost]
        public List<PendingVoucherModel> GetPendingVoucher()
        {
            return this._salesService.GetPendingVoucher();
        }

        [HttpPost]
        public List<CompanySalesModel> GetCompanySalesMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode, bool salesReturn, string AmountType = "GrossAmount")
        {
            return this._salesService.GetCompanySalesMonthlyReport(model, DateFormat, salesReturn, AmountType);
        }
        [HttpPost]
        public List<CompanySalesModel> GetCompanyPurchaseMonthlyReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetCompanyPurchaseMonthlyReport(model, DateFormat, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<MonthlyBranchSalesGraph> GetPurchaseBranchMonthlyReport(filterOption Model, string DateFormat, string companyCode)
        {
            Model.ReportFilters.CompanyFilter.Clear();
            Model.ReportFilters.CompanyFilter.Add(companyCode);

            return _salesService.GetMonthlyPurchaseSalesSummary(Model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public List<CompanyProductionModel> GetCompanyPurchaseDailyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetCompanyProductionMonthlyReport(model, DateFormat);
        }
        [HttpPost]
        public List<CompanyProductionModel> GetCompanyProductionMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetCompanyProductionMonthlyReport(model, DateFormat);
        }

        [HttpPost]
        public List<CompanyExpenseModel> GetCompanyExpenseMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetCompanyExpenseMonthlyReport(model, DateFormat);
        }


        [HttpPost]
        public List<MonthlyBranchProductionGraph> GetProductionBranchMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetMonthlyBranchProductionSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode);
        }

        [HttpPost]
        public List<MonthlyBranchExpenseGraph> GetExpenseBranchMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetMonthlyBranchExpenseSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat, customerCode, itemCode, categoryCode, companyCode, branchCode, partyTypeCode, formCode);
        }

        [HttpPost]
        public List<BranchDaysProductionGraph> GetProductionBranchDailyReport(filterOption model, string monthName, string branchName, string DateFormat)
        {
            var resultData = this._salesService.GetDailyBranchProductionSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, branchName, "", "", "", "01", "", "", "", DateFormat).ToList();
            return resultData;

        }

        [HttpPost]
        public List<BranchDaysExpenseGraph> GetExpenseBranchDailyReport(filterOption model, string monthName, string branchName, string companyCode, string DateFormat)
        {
            var resultData = this._salesService.GetDailyBranchExpenseSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, branchName, "", "", "", companyCode, "", "", "", DateFormat).ToList();
            return resultData;

        }

        [HttpPost]
        public List<BranchDaysSalesGraph> GetPurchaseBranchDailyReport(filterOption model, string monthName, string branchName, string DateFormat)
        {

            var resultData = this._salesService.GetDailyBranchPurchaseSummary(model.ReportFilters, _workContext.CurrentUserinformation, monthName, branchName, DateFormat).ToList();

            return resultData;

        }
        [HttpPost]
        public List<StockChartModel> GetCompanyStockMonthlyReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetCompanyStockMonthlyReport(model, DateFormat);
        }


        [HttpPost]
        public List<BalanceChartModel> GetCompanyBankBalanceReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetCompanyBankBalanceReport(model, DateFormat);
        }

        [HttpPost]
        public List<BalanceChartModel> GetCompanyCashBalanceReport(filterOption model, string DateFormat, string customerCode, string itemCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        {
            return this._salesService.GetCompanyCashBalanceReport(model, DateFormat);
        }
        [HttpPost]
        public List<StockChartModel> GetStockBranchMonthlyReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetMonthlyBranchStockSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }


        [HttpPost]
        public List<BalanceChartModel> GetBranchBankBalanceReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetBranchBankBalanceSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public List<MonthlyLoan> GetMonthlyShortTeamLoanReport(filterOption Model, string DateFormat)
        {
            return _salesService.GetMonthlyShortteamLoan(Model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public List<MonthlyLoan> GetMonthlyLongTeamLoanReport(filterOption Model, string DateFormat)
        {
            return _salesService.GetMonthlyLongteamLoan(Model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public List<BalanceChartModel> GetBranchCashBalanceReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetBranchCashBalanceSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }
        [HttpPost]
        public HttpResponseMessage RemoveDashboardcache(string reportname = "")
        {

            try
            {
                var MainCancheName = $"neoCache_{reportname}";
                _cacheManager.Remove(MainCancheName);

                var QueryCacheName = $"neocacheQuery_{reportname}";
                _cacheManager.Remove(QueryCacheName);
                var cacheNamagerKey = $"neoreopder_{reportname}";
                _cacheManager.Remove(cacheNamagerKey);
                var cacheChartPermissionKey = $"neochartpersmission_{reportname}";
                _cacheManager.Remove(cacheChartPermissionKey);
                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpResponseMessage(HttpStatusCode.NotAcceptable);
            }
        }

        [HttpPost]
        public List<BalanceChartModel> GetCashBalanceDetailReport(filterOption model)
        {
            return this._salesService.GetCashBalanceDetailSummary(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<BalanceChartModel> GetBankBalanceDetailReport(filterOption model)
        {
            return this._salesService.GetBankBalanceDetailSummary(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<BalanceChartModel> GetLoanBalanceDetailReport(filterOption model, string duration)
        {
            return this._salesService.GetLoanBalanceDetailSummary(model.ReportFilters, duration, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<BalanceChartModel> GetExpenseBalanceReport(filterOption model)
        {
            return this._salesService.GetExpenseBalanceSummary(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<BalanceChartModel> GetExpenseBalanceDetailReport(filterOption model, string accCode)
        {
            return this._salesService.GetExpenseBalanceDetailSummary(model.ReportFilters, accCode, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<BalanceChartMonthlyModel> GetBankBalanceMonthlyReport(filterOption model, string bankName)
        {
            return this._salesService.GetBankBalanceMonthlySummary(model.ReportFilters, _workContext.CurrentUserinformation, bankName);
        }


        [HttpPost]
        public List<BalanceChartMonthlyModel> GetCashBalanceMonthlyReport(filterOption model, string bankName)
        {
            return this._salesService.GetCashBalanceMonthlySummary(model.ReportFilters, _workContext.CurrentUserinformation, bankName);
        }

        [HttpPost]
        public List<StockChartModel> GetStockCategoryWiseReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetStockCategorySummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }

        [HttpPost]
        public List<StockChartModel> GetStockItemWiseReport(filterOption model, string DateFormat)
        {
            return this._salesService.GetStockItemSummary(model.ReportFilters, _workContext.CurrentUserinformation, DateFormat);
        }

        [HttpPost]
        public List<StockValutionViewModel> GetStockValutions(filterOption model)
        {
            var data = this._salesService.GetStockValutionSummary(model, _workContext.CurrentUserinformation);
            return data.ToList();
        }
        [HttpPost]
        public List<StockValutionViewModel> GetProductStockValutions(filterOption model, string catagoryCode)
        {
            var data = this._salesService.GetProductStockValutions(model, _workContext.CurrentUserinformation, catagoryCode);
            return data.ToList();
        }
        [HttpPost]
        public List<string> GetAllDashboardWidgets()
        {
            List<string> widgets = new List<string>();
            string temp = _salesService.GetDashboardWidgets(User.Identity.Name, "Sales-Main");
            if (temp != null)
                widgets = temp.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
            return widgets;
        }


        [HttpPost]
        public List<string> GetDashboardWidgets()
        {
            List<string> widgets = new List<string>();
            string temp = _salesService.GetDashboardWidgets(User.Identity.Name, "Sales");
            if (temp != null)
                widgets = temp.Split(',').Where(s => !string.IsNullOrEmpty(s)).ToList();
            return widgets;
        }

        [HttpPost]
        public List<NCRChartModel> TreewiseSalesReport(filterOption model)
        {
            var treeWiseSalesReport = new List<NCRChartModel>();
            var cachekey = $"treewisesales-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<NCRChartModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetTreewiseSalesReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }
        [HttpPost]
        public List<NCRChartModel> TreewiseCustomerSalesPlanReport(filterOption model)
        {
            var treeWiseSalesReport = new List<NCRChartModel>();
            var cachekey = $"treewiseCustomesalesPlan-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<NCRChartModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetTreewiseCustomerSalesPlanReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }
        [HttpPost]
        public List<NCRChartModel> TreewiseProductSalesPlanReport(filterOption model)
        {
            var treeWiseSalesReport = new List<NCRChartModel>();
            var cachekey = $"treewiseProductsalesPlan-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<NCRChartModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetTreewiseProductSalesPlanReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }

        [HttpPost]
        public List<ItemCustomerTreeModel> TreewiseCustomerSalesReport(filterOption model)
        {
            var treeWiseSalesReport = new List<ItemCustomerTreeModel>();
            var cachekey = $"treewisesalesKiran-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<ItemCustomerTreeModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetTreewiseKiranshoesSalesReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }

        [HttpPost]
        public List<NCRChartModel> TreewiseCollectionReport(filterOption model)
        {
            var treeWiseSalesReport = new List<NCRChartModel>();
            var cachekey = $"treewiseCollection-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}{_workContext.CurrentUserinformation}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<NCRChartModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetYearTreewiseCollectonReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }
        [HttpPost]
        public List<NCRChartModel> TreewiseThisMonthCollectionReport(filterOption model)
        {
            var treeWiseSalesReport = new List<NCRChartModel>();
            var cachekey = $"treewiseCollectionThisMonth-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}{_workContext.CurrentUserinformation}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<NCRChartModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetThisMonthTreewiseCollectonReport(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }
        [HttpPost]
        public List<NCRChartModel> TreewiseSalesDealerReport(filterOption model)
        {
            var treeWiseSalesReport = new List<NCRChartModel>();
            var cachekey = $"treewisesalesdealer-{model.ReportFilters.FromDate}-{model.ReportFilters.ToDate}";
            if (this._cacheManager.IsSet(cachekey))
            {
                treeWiseSalesReport = this._cacheManager.Get<List<NCRChartModel>>(cachekey);
            }
            else
            {
                treeWiseSalesReport = _salesService.GetTreewiseSalesReportDealer(model.ReportFilters, _workContext.CurrentUserinformation);
                this._cacheManager.Set(cachekey, treeWiseSalesReport, 10);
            }
            return treeWiseSalesReport;
        }
        #region Division Wise Outstanding

        [HttpPost]
        public List<DivisionOutStanding> GetDivisionOutStanding(filterOption model)
        {
            return _salesService.GetDivisionWiseOutStanding(model, _workContext.CurrentUserinformation);
        }

        #endregion

        #region Lc Tragert
        [HttpPost]
        public List<LcChartViewModel> GetLCOutStanding(filterOption model)
        {
            List<LcChartViewModel> result = new List<LcChartViewModel>();
            var response = _salesService.GetLcOutStanding(model, _workContext.CurrentUserinformation);
            foreach (var item in response)
            {
                result.Add(new LcChartViewModel { Type = "Used Balance", Amount = item.Target - item.Balance });
                result.Add(new LcChartViewModel { Type = "Balance", Amount = item.Balance });
            }
            return result;
        }
        [HttpPost]
        public List<LcChartModel> GetLCOutStandingDetails(filterOption model)
        {
            return _salesService.GetLcOutStandingDetails(model, _workContext.CurrentUserinformation);
        }

        #endregion
        [HttpPost]
        public List<DashboardSalesReport> DashboardSalesOverAll()
        {
            return _salesService.SalesDashboardOverAll();
        }
    }
}
