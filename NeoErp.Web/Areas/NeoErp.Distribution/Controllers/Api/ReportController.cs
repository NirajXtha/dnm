using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Distribution.Service;
using NeoErp.Distribution.Service.Model;
using NeoErp.Distribution.Service.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using LinqToExcel;
using System.Diagnostics;
using System.Web.Services.Description;
using Remotion.Utilities;

namespace NeoErp.Distribution.Controllers.Api
{
    public class ReportController : ApiController
    {
        public IDistributionService _distributionService { get; set; }
        private IWorkContext _workContext;

        public ReportController(IDistributionService distributionService, IWorkContext workContext)
        {
            this._distributionService = distributionService;
            this._workContext = workContext;
        }

        [HttpPost]
        public IEnumerable<QuestionaireCustomerModel> GetQuestionaireCustomerReport(filterOption model, string surveyCode = "")
        {

            IEnumerable<QuestionaireCustomerModel> reportData = this._distributionService.GetQuestionaireReport(model.ReportFilters, _workContext.CurrentUserinformation, surveyCode);
            return reportData;
        }

        [HttpGet]
        public List<SurveyDDl> GetSurveyList()
        {
            var data = _distributionService.GetSurveys(_workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public dynamic GetClaimReport(filterOption model)
        {
            return _distributionService.GetClaimReport(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public IEnumerable<QuestionaireModel> GetQuestionaireReport(filterOption model, string CustomerCode, string CustomerType, string surveyCode = "")
        {
            IEnumerable<QuestionaireModel> reportData = this._distributionService.GetQuestionaire(model.ReportFilters, CustomerCode, CustomerType, surveyCode, _workContext.CurrentUserinformation);
            return reportData;
        }

        public List<RouteModel> GetAllRouteByFilters(string filter, string empCode)
        {
            var result = _distributionService.getAllRoutesByFilter(filter, empCode);
            return result;
        }

        [HttpGet]
        public List<DIST_PLAN_ROUTE> GetRouteList(string plancode)
        {
            return _distributionService.getAllPlanRoutes(plancode, _workContext.CurrentUserinformation);

        }
        [HttpPost]
        public List<SURVEY_COLUMN_MODEL> GetSurveyReportCol()
        {
            return _distributionService.GetSurveyReportCol();
        }
        [HttpPost]
        public List<SURVEY_COLUMN_MODEL> GetWebSurveyReportQUE()
        {
            return _distributionService.GetWebSurveyReportQUE(_workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<SURVEY_REPORT_MODEL> GetSurveyReport(filterOption model)
        {
            return _distributionService.GetSurveyReport(model);
        }

        [HttpPost]
        public List<SURVEY_REPORT_AATA_MODEL> GetSurveyReportAata(filterOption model)
        {
            return _distributionService.GetSurveyReportAata(model);
        }

        [HttpPost]
        public List<SURVEY_REPORT_AATA_MODEL> GetAataSurveyReportTab(filterOption model)
        {
            return _distributionService.GetSurveyReportAataTab(model);
        }
        [HttpPost]
        public List<SURVEY_REPORT_MODEL> GetDynamicSurveyReportTab(filterOption model)
        {
            return _distributionService.GetSurveyReportDynamic(model);
        }
        [HttpPost]
        public List<SURVEY_REPORT_MODEL> GetDynamicWebSurveyReport(filterOption model)
        {
            return _distributionService.GetDynamicWebSurveyReport(model, _workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<SURVEY_REPORT_MODEL> GetSurveyReport_JGI(filterOption model)
        {
            return _distributionService.getSurveyReport_JGI(model);
        }

        [HttpPost]
        public List<SURVEY_REPORT_BRANDING_MODEL> GetBrandingSurveyReportTab(filterOption model)
        {
            return _distributionService.GetBrandingSurveyReportTab(model);
        }
        [HttpPost]
        public List<MerchandisingStockModel> GetMerchandisingStockReport(filterOption model)
        {
            return _distributionService.GetMerchandisingStockReport(model, _workContext.CurrentUserinformation);
        }
        [HttpGet]
        public List<RouteModelPlan> GetRouteByPlanCode(string PLAN_ROUTE_CODE)
        {
            return _distributionService.GetRouteByPlanCode(PLAN_ROUTE_CODE, _workContext.CurrentUserinformation);

        }

        [HttpGet]
        public List<TargetModelPLan> GetTargetPlan( string subTargetType, string filterType, string dateFilter)
        {
            return _distributionService.GetTargetPlan(subTargetType, filterType, dateFilter);
        }


        [HttpGet]
        public List<ModelEmployee> GetEmployees(string filter, string empGroup)
        {
            var employees = this._distributionService.getEmployees(filter, empGroup, _workContext.CurrentUserinformation);
            return employees;
        }

        [HttpPost]
        public List<PerformanceModel> GetPerformanceReport(filterOption dateFilter)
        {
            var data = _distributionService.GetPerformanceReportList(dateFilter.ReportFilters);
            return data;
        }
        [HttpPost]
        public List<SumOutletModel> GetSumOutletReport(filterOption dateFilter)
        {
            var data = _distributionService.GetSumOutletReportList(dateFilter.ReportFilters);
            return data;
        }
        [HttpPost]
        public List<OutletSummaryModel> GetOutletSummaryReport(filterOption dateFilter)
        {

            var data = _distributionService.GetOutletSummaryReportList(dateFilter.ReportFilters);
            return data;
        }
        [HttpPost]
        public List<TopEffectiveModel> GetTopEffectiveCallsReport(string percentEffectiveCalls, filterOption dateFilter)
        {
            var data = _distributionService.GetTopEffectiveCallsReportList(percentEffectiveCalls, dateFilter.ReportFilters);
            return data;
        }

        [HttpPost]
        public List<TopEffectiveModel> GetALLPerformanceReport(filterOption model)
        {
            var data = _distributionService.GetALLPerformanceReport(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public List<DetailTopEffective> GetPresentASMBeat(filterOption model)
        {
            var data = _distributionService.GetPresentASMBeat(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public List<DetailTopEffective> GetASMBeat(filterOption model)
        {
            var data = _distributionService.GetASMBeat(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public List<TopEffectiveModel> GetALLEmployeeReport(filterOption model)
        {
            var data = _distributionService.GetALLEmployeeReport(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public List<TopEffectiveModel> GetALLEmployeeReportNew(filterOption model)
        {
            var data = _distributionService.GetALLEmployeeReportNew(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public List<DetailTopEffective> DetailEmpReport(filterOption model)
        {
            var data = _distributionService.GetDetailEmployeeReport(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public List<DetailTopEffective> DetailEmpReportNew(filterOption model)
        {
            var data = _distributionService.GetDetailEmployeeReportDetail(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public List<DetailTopEffective> DetailEmpReportIndivisual(filterOption model)
        {
            var data = _distributionService.GetDetailEmpReportIndivisual(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public List<DetailTopEffective> DetailBrandingEmpReport(filterOption model)
        {
            var data = _distributionService.GetDetailBrandingEmployeeReport(model.ReportFilters, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public List<EmployeeWisePerformance> GetEmployeeProductive(filterOption model, string SP_CODE)
        {
            var data = _distributionService.GetEmployeeProductive(model.ReportFilters, _workContext.CurrentUserinformation, SP_CODE);
            return data;
        }
        [HttpPost]
        public List<EmployeeWisePerformance> GetBrandingEmployeeProductive(filterOption model, string SP_CODE)
        {
            var data = _distributionService.GetBrandingEmployeeProductive(model.ReportFilters, _workContext.CurrentUserinformation, SP_CODE);
            return data;
        }
        [HttpPost]
        public List<EmpBrandWiseModel> GetEmployeeBrandSales(filterOption model, string SP_CODE)
        {
            var data = _distributionService.GetBrandwiseEmpData(model.ReportFilters, _workContext.CurrentUserinformation, SP_CODE);
            return data;
        }

        [HttpPost]
        public List<EmpBrandWiseModel> GetEmployeeBrandSalesConversion(filterOption model, string SP_CODE)
        {
            var data = _distributionService.GetBrandwiseEmpDataConversion(model.ReportFilters, _workContext.CurrentUserinformation, SP_CODE);
            return data;
        }
        [HttpPost]
        public List<EmpBrandWiseModel> GetEmployeeItemSalesConversion(filterOption model, string SP_CODE)
        {
            var data = _distributionService.GetItemwiseEmpDataConversion(model.ReportFilters, _workContext.CurrentUserinformation, SP_CODE);
            return data;
        }
        [HttpPost]
        public List<AttendanceModel> GetAttendanceReportEmployeeWise(filterOption model, string SP_CODE)
        {
            var data = _distributionService.GetAttendanceReportEmployeeWise(model.ReportFilters, _workContext.CurrentUserinformation, SP_CODE);
            return data;
        }

        [HttpPost]
        public List<SalesPersonPoModel> GetSalesPersonList(filterOption dateFilter, string requestStatus = "Pending")

        {
            var data = _distributionService.GetSalesPersonList(dateFilter.ReportFilters, requestStatus, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public List<SalesPersonPoModel> GetItemCumulativeReport(filterOption dateFilter, string dateFlag)
        {
            var data = _distributionService.GetItemCumulativeReport(dateFilter.ReportFilters, _workContext.CurrentUserinformation, dateFlag);
            return data;
        }
        [HttpPost]
        public List<AreaWiseDistributorModel> GetAllDistributerReseller([ModelBinder] filterOption modelPar, string type = "")
        {
            var modelrequest = Request.Content.ReadAsAsync<filterOption>().Result;
            var data = _distributionService.GetAreaWiseDistributor(modelrequest, _workContext.CurrentUserinformation, type);
            return data;
        }
        [HttpPost]
        public List<AreaWiseDistributorModel> GetAllOutleet([ModelBinder] filterOption modelPar, string type = "R")
        {
            var modelrequest = Request.Content.ReadAsAsync<filterOption>().Result;
            var data = _distributionService.GetUserWiseOUtlet(modelrequest, _workContext.CurrentUserinformation, type);
            return data;
        }

        [HttpPost]
        public List<EODModel> GetEODList(filterOption model)
        {
            return _distributionService.GetEODList(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<DailyActivityModel> GetDailyAcivityList(filterOption model)
        {
            return _distributionService.GetDailyAcivityList(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<EODModel> GetEODDetail(filterOption model, string SP_CODE, string type)
        {
            return _distributionService.GetEODDetail(model.ReportFilters, SP_CODE, type, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<ItemWiseMinMaxReport> GetItemsMinMaxList(filterOption model)
        {
            return _distributionService.GetItemsMinMaxList(model.ReportFilters, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<AttendanceModel> GetAttendanceReport(filterOption model)
        {
            return _distributionService.GetAttendanceReport(model.ReportFilters, _workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<DeliveryModel> GetDeliveryReport(filterOption model, string Type, string status)
        {
            return _distributionService.GetDeliveryReport(Type, status, model.ReportFilters, _workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<AttendanceModel> GetBrandingAttendanceReport(filterOption model)
        {
            return _distributionService.GetBrandingAttendanceReport(model.ReportFilters, _workContext.CurrentUserinformation);
        }
        [HttpPost]
        public SalesRegisterViewModel GetSalesRegistersReport(filterOption model)
        {

            var salesRegistersViewModel = new SalesRegisterViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _distributionService.GetSalesRegister(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                salesRegistersViewModel.SalesRegisters = salesRegister.Where(whereClause, parameters.ToArray()).ToList();
                salesRegistersViewModel.total = salesRegistersViewModel.SalesRegisters.Count;
            }
            else
            {
                salesRegistersViewModel.SalesRegisters = salesRegister.ToList();
                salesRegistersViewModel.total = salesRegistersViewModel.SalesRegisters.Count;
                // salesRegistersViewModel.total = _salesRegister.TotalSalesRegister(formDate, toDate);
            }

            salesRegistersViewModel.AggregationResult = KendoGridHelper.GetAggregation(salesRegistersViewModel.SalesRegisters, model.aggregate);

            salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderByDescending(x => x.SalesDate).ThenByDescending(x => x.InvoiceNumber).ToList();
            return salesRegistersViewModel;
        }
        [HttpGet]
        public dynamic GetAllUnitFilter()
        {
            return this._distributionService.GetAllUnitFilter(_workContext.CurrentUserinformation);
        }
        [HttpGet]
        public dynamic GetAllDeliveryStatusFilter()
        {
            return this._distributionService.GetAllDeliveryStatusFilter(_workContext.CurrentUserinformation);
        }

        [HttpPost]
        public dynamic GetOutletClosing(filterOption model, string unitFilter)
        {
            var result = _distributionService.GetOutletClosingReport(model.ReportFilters, unitFilter, _workContext.CurrentUserinformation);
            return result;
        }
        [HttpPost]
        public dynamic GetResellerClosingStock(filterOption model, string unitFilter)
        {
            var result = _distributionService.GetResellerClosingStock(model.ReportFilters, unitFilter, _workContext.CurrentUserinformation);
            return result;
        }
        [HttpPost]
        public dynamic GetFarmerProblemRecommendation(filterOption model)
        {
            var result = _distributionService.GetFarmerProblemRecommendation(model.ReportFilters, _workContext.CurrentUserinformation);
            return result;
        }

        //[HttpPost]
        //public List<ClosingReportDetailModel> GetOutletClosingDetails(filterOption model, ClosingReportModel detail, string unitFilter)
        //{
        //    var result = _distributionService.GetOutletClosingReportDetail(model, detail, unitFilter, _workContext.CurrentUserinformation);
        //    return result;
        //}
        [HttpPost]
        public List<VisitImageModel> GetVisiterList(filterOption filter, string Distributor = "", string Reseller = "")
        {
            var result = _distributionService.GetVisiterList(filter.ReportFilters, _workContext.CurrentUserinformation, Distributor, Reseller);
            return result;
        }

        [HttpPost]
        public List<NotVisitedCustomerModel> GetNotVisitedList(filterOption filter, string Distributor = "", string Reseller = "")
        {
            var result = _distributionService.GetNotVisitedCustomer(filter.ReportFilters, _workContext.CurrentUserinformation, Distributor, Reseller);
            return result;
        }

        [HttpPost]
        public List<VisitImageModel> GetBrandingVisiterList(filterOption filter, string Distributor = "", string Reseller = "")
        {
            var result = _distributionService.GetBrandingVisiterList(filter.ReportFilters, _workContext.CurrentUserinformation, Distributor, Reseller);
            return result;
        }
        [HttpPost]
        public List<VisitImageModel> GetVisiterListCondition(filterOption filter, string Distributor = "", string Reseller = "")
        {
            var result = _distributionService.GetVisiterListCondition(filter.ReportFilters, _workContext.CurrentUserinformation, Distributor, Reseller);
            return result;
        }

        [HttpPost]
        public HttpResponseMessage GetCompetitorReport(filterOption filter, string Item_code = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Item_code))
                    throw new Exception();
                var data = _distributionService.GetCompReport(filter.ReportFilters, _workContext.CurrentUserinformation, Item_code);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<CompReportModel>());
            }
        }
        [HttpPost]
        public HttpResponseMessage GetCompetitorReportMonthly(filterOption filter, string Item_code = "", string category = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Item_code))
                    throw new Exception();
                var data = _distributionService.GetCompReportMonthly(filter.ReportFilters, _workContext.CurrentUserinformation, Item_code, category);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<CompReportModel>());
            }
        }

        [HttpPost]
        public HttpResponseMessage GetSPDistanceReport(filterOption filter)
        {
            try
            {
                var data = _distributionService.GetSPTravelReport(filter.ReportFilters, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SPDistanceModel>());
            }
        }

        [HttpPost]
        public HttpResponseMessage GetSPRouteReport(filterOption filter, string source = "V")
        {
            try
            {
                var data = _distributionService.GetSPRouteReport(filter.ReportFilters, _workContext.CurrentUserinformation, source);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SPDistanceModel>());
            }
        }

        [HttpPost]
        public HttpResponseMessage GetResellerDetailReport(filterOption filter, string source = "V")
        {
            try
            {
                var data = _distributionService.GetResellerDetailReport(filter.ReportFilters, _workContext.CurrentUserinformation, source);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<ResellerListModel>());
            }
        }

        [HttpPost]
        public HttpResponseMessage GetDeviceLog(filterOption model)
        {
            try
            {
                var data = _distributionService.GetDeviceLog(model, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<MobileLogModel>());
            }
        }
        [HttpPost]
        public List<SummaryReport> GetAllSummaryReport(filterOption model)
        {
            try
            {
                var data = _distributionService.GetAllSummaryReport(model, _workContext.CurrentUserinformation);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public List<AttendanceReportCalendarModel> GetAttendanceForCalendar(filterOption model)
        {
            try
            {
                var data = _distributionService.GetAttendanceForCalendar(model);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public List<DistResellerStockModel> GetDistResellerStockList(filterOption model, string Distributor = "", string Reseller = "")
        {
            try
            {
                var data = _distributionService.GetDistResellerStockList(model, _workContext.CurrentUserinformation, Distributor, Reseller);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public List<DistResellerStockConversionModel> GetDistResellerBrandItemStockList(filterOption model, string Distributor = "", string Reseller = "")
        {
            try
            {
                var data = _distributionService.GetDistResellerBrandItemStockList(model, _workContext.CurrentUserinformation, Distributor, Reseller);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public List<DistdistributorStockModel> GetDistDistributorBrandItemStockList(filterOption model, string Distributor = "", string Reseller = "")
        {
            try
            {
                var data = _distributionService.GetDistDistributorBrandItemStockList(model, _workContext.CurrentUserinformation, Distributor, Reseller);
                return data;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPost]
        public List<DistAttendanceReportSummary> GetAttendanceSummaryGroupWise(filterOption model)
        {
            return _distributionService.GetAttendanceSummaryGroup(model.ReportFilters, _workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<DistAttendanceReportSummary> GetAttendanceSummaryEmployeeWise(filterOption model, string GroupWise)
        {
            return _distributionService.GetAttendanceSummaryEmployeeWise(model.ReportFilters, _workContext.CurrentUserinformation, GroupWise);
        }
        [HttpPost]
        public List<StockSummary> GetZoneWiseStockReport(filterOption model)
        {
            return _distributionService.GetStockGroupSummary(model.ReportFilters, _workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<StockSummary> GetAreaWiseStockReport(filterOption model, int GroupWise)
        {
            return _distributionService.GetAreaGroupSummary(model.ReportFilters, _workContext.CurrentUserinformation, GroupWise.ToString());
        }

        [HttpPost]
        public List<StockSummary> GetResellerStockReport(filterOption model, string areawise)
        {
            return _distributionService.GetResellerCodeStock(model.ReportFilters, _workContext.CurrentUserinformation, areawise.ToString());
        }
        [HttpPost]
        public List<StockDetailReort> GetResellerStockReportDetail(filterOption model, string areawise)
        {
            return _distributionService.GetResellerCodeStockDetail(model.ReportFilters, _workContext.CurrentUserinformation, areawise.ToString());
        }
        [HttpPost]
        public List<AttendanceModel> GetRoutename(filterOption model, string spcode)
        {
            return _distributionService.GetEmployeeRoute(model.ReportFilters, _workContext.CurrentUserinformation, spcode.ToString());
        }
        [HttpGet]
        public List<SchemeModel> GetSchemeName()
        {
            return _distributionService.GetSchemeName(_workContext.CurrentUserinformation);
        }
        [HttpPost]
        public List<SchemeReportModel> GetSchemeSalesPersonList(string SchemeID)
        {
            return _distributionService.GetSchemeSalesPersonList(_workContext.CurrentUserinformation, SchemeID);
        }

        [HttpGet]
        public List<SchemeModel> GetSchemeandDetails()
        {
            return _distributionService.GetSchemeAndDetails(_workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<SalesPersonPoModel> GetSchemeReport(string Id, string MinVal, string MaxVal, string fromDate, string toDate)
        {
            return _distributionService.GetSchemeReport(_workContext.CurrentUserinformation, Id, MinVal, MaxVal, fromDate, toDate);

        }

        [HttpPost]
        public FinalDataModel FinalData(string employeeCode, string companyCode)
        {
            return _distributionService.FinalData(employeeCode, companyCode);
        }
        
        [HttpPost]
        public bool SaveData(List<CustomerData> dataToSave)
        {
            _distributionService.SaveData(_workContext.CurrentUserinformation, dataToSave);
            return true;
        }
        
        [HttpPost]
        public bool AccessData( List<ManageAccessModal> dataToSave)
        {
            _distributionService.AccessData(_workContext.CurrentUserinformation, dataToSave);
            return true;
        }

        [HttpPost]
        public List<ManageAccessModal> StoredAccessData()
        {
            return _distributionService.StoredAccessData(_workContext.CurrentUserinformation);
        }

        [HttpPost]
        public FinalDataModel UploadExcel()
        {

            var file = HttpContext.Current.Request.Files[0];
            if (file == null || file.ContentLength == 0)
            {
                Debug.WriteLine("no file uploaded");
            }

            var customerDataList = new List<CustomerData>();

            // Ensure temp folder exists
            string tempFolder = Path.Combine(Path.GetTempPath(), "MyAppUploads");
            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            // Unique temp file path
            string tempFilePath = Path.Combine(tempFolder, Guid.NewGuid().ToString() + ".xlsx");

            file.SaveAs(tempFilePath);

            if (!File.Exists(tempFilePath))
            {
                Debug.WriteLine("file not found after saving");
            }

            try
            {
                var excelFile = new ExcelQueryFactory(tempFilePath);
                var rows = excelFile.Worksheet("Sheet1");
                foreach (var row in rows)
                {
                    var name = row["Name"].ToString();
                    var balanceStr = row["Balance"].ToString();
                    decimal balance = decimal.Parse(balanceStr);
                    balance = Math.Round(balance, 2);

                    customerDataList.Add(new CustomerData
                    {
                        Name = name,
                        Balance = Math.Abs(balance),
                        Type = balance < 0 ? "CR" : "DR",
                        CustomerCode = null
                    });
                }

                //Debug.WriteLine(customerDataList[0].Name);
            }
            finally
            {
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                    //Debug.WriteLine("file exists");
                }
            }

            // Now call the service method with the customer data list

            //var result = _distributionService.ExcelService.ProcessCustomerData(customerDataList);
            var result = _distributionService.ProcessCustomerData(_workContext.CurrentUserinformation, customerDataList);
            return result; 
        }

       
    }
}

