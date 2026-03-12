using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model.Mobile;
using Newtonsoft.Json.Linq;
using OfficeOpenXml.FormulaParsing.LexicalAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public class ActionSelector : IActionSelector
    {
        private IMobileService _mobileService;
        private IMobileOfflineService _offlineService;
        private string CompanyCode;

        public ActionSelector(IMobileService mobileService, IMobileOfflineService offlineService)
        {
            _mobileService = mobileService;
            _offlineService = offlineService;
        }
        public object SelectAction(JToken token, NeoErpCoreEntity dbContext)
        {
            object Output = new object();

            string Action = (string)token.SelectToken("action");
            if (Action == null)
                throw new Exception("Invalid action.");

            //online operations
            if (!Action.Equals("login", StringComparison.OrdinalIgnoreCase))
            {
                CompanyCode = (string)token.SelectToken("COMPANY_CODE");
                string Branch = (string)token.SelectToken("BRANCH_CODE");
                if (string.IsNullOrWhiteSpace(CompanyCode))
                    throw new Exception("Company Code Not Found.");
                if (string.IsNullOrWhiteSpace(Branch))
                    throw new Exception("Branch Code Not Found.");
            }

            #region Actions

            #region Fetching
            if (Action.Equals("login", StringComparison.OrdinalIgnoreCase))
            {
                LoginModel model = token.ToObject<LoginModel>();
                var data = _mobileService.Login(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("logout", StringComparison.OrdinalIgnoreCase))
            {
                LogoutRequestModel model = token.ToObject<LogoutRequestModel>();
                var data = _mobileService.Logout(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchVisitPlan", StringComparison.OrdinalIgnoreCase))
            {
                VisitPlanRequestModel model = token.ToObject<VisitPlanRequestModel>();
                var data = _mobileService.GetVisitPlan(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchEntity", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchEntity(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchItems", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchItems(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchAllQuestion", StringComparison.OrdinalIgnoreCase))
            {
                QuestionRequestModel model = token.ToObject<QuestionRequestModel>();
                var data = _mobileService.FetchAllQuestions(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchArea", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchArea(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchOutlets", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchOutlets(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("getEntityItemByBrand", StringComparison.OrdinalIgnoreCase))
            {
                ClosingStockRequestModel model = token.ToObject<ClosingStockRequestModel>();
                var data = _mobileService.GetEntityItemByBrand(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchMu", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchMU(model, dbContext);
                if (data["Unit"].Count <= 0)
                    throw new Exception("No records found");
                Output = data;
            }
            //else if (Action.Equals("fetchTransactions", StringComparison.OrdinalIgnoreCase))
            //{
            //    TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
            //    var data = _mobileService.FetchTransactions(model, dbContext);
            //    Output = data;
            //}
            else if (Action.Equals("fetchTransactions", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.FetchSubLedgers(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("ageingTransactions", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.ageingTransactions(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("ageingTransactionsPdf", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.ageingTransactionsPdf(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("FetchVoucher", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.FetchVoucher(model, dbContext, token);
                Output = data;
            }
            else if (Action.Equals("FetchVoucherPdf", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.FetchVoucherPdf(model, dbContext, token);
                Output = data;
            }
            else if (Action.Equals("transactionsPdf", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.fetchTransactionData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchTransactionsMoveAnislis", StringComparison.OrdinalIgnoreCase))
            {
                TransactionRequestModel model = token.ToObject<TransactionRequestModel>();
                var data = _mobileService.FetchMovementTransactions(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchPurchaseOrder", StringComparison.OrdinalIgnoreCase))
            {
                PurchaseOrderRequestModel model = token.ToObject<PurchaseOrderRequestModel>();
                var data = _mobileService.FetchPurchaseOrder(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchPurchaseOrderStatus", StringComparison.OrdinalIgnoreCase))
            {
                PurchaseOrderRequestModel model = token.ToObject<PurchaseOrderRequestModel>();
                var data = _mobileService.FetchPOStatus(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("getSalesAndAgeReport", StringComparison.OrdinalIgnoreCase))
            {
                ReportRequestModel model = token.ToObject<ReportRequestModel>();
                var data = _mobileService.SalesAgingReport(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("getAgingReport", StringComparison.OrdinalIgnoreCase))
            {
                ReportRequestModel model = token.ToObject<ReportRequestModel>();
                var data = _mobileService.AgingReportGroup(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("agingReport", StringComparison.OrdinalIgnoreCase))
            {
                ReportRequestModel model = token.ToObject<ReportRequestModel>();
                var data = _mobileService.AgingReport(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("monthWiseSales", StringComparison.OrdinalIgnoreCase))
            {
                ReportRequestModel model = token.ToObject<ReportRequestModel>();
                var data = _mobileService.MonthWiseSales(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchEntityOrderBookingData", StringComparison.OrdinalIgnoreCase))
            {
                EntityRequestModel model = token.ToObject<EntityRequestModel>();
                var data = _mobileService.FetchEntityPartyTypeAndMu(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchPartyTypeBillingEntity", StringComparison.OrdinalIgnoreCase))
            {
                EntityRequestModel model = token.ToObject<EntityRequestModel>();
                var data = _mobileService.FetchPartyTypeBillingEntity(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchEntityById", StringComparison.OrdinalIgnoreCase))
            {
                string code = (string)token.SelectToken("code");
                string type = (string)token.SelectToken("type");
                EntityRequestModel model = token.ToObject<EntityRequestModel>();
                model.entity_type = type;
                model.entity_code = code;
                var data = _mobileService.FetchEntityById(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchDistributorWithConstraint", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchDistributorWithConstraint(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchSpPartyType", StringComparison.OrdinalIgnoreCase))
            {
                VisitPlanRequestModel model = token.ToObject<VisitPlanRequestModel>();
                var data = _mobileService.FetchSpPartyType(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchSpCustomer", StringComparison.OrdinalIgnoreCase))
            {
                VisitPlanRequestModel model = token.ToObject<VisitPlanRequestModel>();
                var data = _mobileService.FetchSpCustomer(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchImageCategory", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.FetchImageCategory(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchDistributorItems", StringComparison.OrdinalIgnoreCase))
            {
                EntityRequestModel model = token.ToObject<EntityRequestModel>();
                var data = _mobileService.FetchDistributorItems(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchResellerEntity", StringComparison.OrdinalIgnoreCase))
            {
                EntityRequestModel model = token.ToObject<EntityRequestModel>();
                var data = _mobileService.FetchResellerEntity(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchResellerGroups", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.GetResellerGroups(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchContracts", StringComparison.OrdinalIgnoreCase))
            {
                CommonRequestModel model = token.ToObject<CommonRequestModel>();
                var data = _mobileService.GetContracts(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchAchievementReport", StringComparison.OrdinalIgnoreCase))
            {
                AchievementReportRequestModel model = token.ToObject<AchievementReportRequestModel>();
                var data = _mobileService.GetAchievementData(model, dbContext);
                Output = data;

            }
            else if (Action.Equals("fetchAchievementReportMonthWise", StringComparison.OrdinalIgnoreCase))
            {
                AchievementReportRequestModel model = token.ToObject<AchievementReportRequestModel>();
                var data = _mobileService.fetchAchievementReportMonthWise(model, dbContext);
                Output = data;

            }
            //else if (Action.Equals("profile", StringComparison.OrdinalIgnoreCase))
            //{
            //    ProfileDetails model = token.ToObject<ProfileDetails>();
            //    var data = new Dictionary<string, object>();
            //    data = _mobileService.fetchProfileDetails(model, dbContext);
            //    Output = data;
            //}
            else if (Action.Equals("CalenderData", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetailsModel model = token.ToObject<ProfileDetailsModel>();
                var data = new Dictionary<string, object>();
                data = _mobileService.synCalenderData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("AttendanceData", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetailsModel model = token.ToObject<ProfileDetailsModel>();
                var data = _mobileService.AttendanceCountData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("profileDetails", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetailsModel model = token.ToObject<ProfileDetailsModel>();
                var data = new Dictionary<string, object>();
                data = _mobileService.SynProfileData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("AreaCustomerWise", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetailsModel model = token.ToObject<ProfileDetailsModel>();
                var data = new Dictionary<string, object>();
                data = _mobileService.SynAreaCustomerData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("ProductQuantityWise", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetailsModel model = token.ToObject<ProfileDetailsModel>();
                var data = new Dictionary<string, object>();
                data = _mobileService.SynProductQuantityData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("SalesVsCollection", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetailsModel model = token.ToObject<ProfileDetailsModel>();
                var data = _mobileService.fetchSalesVsCollectionData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("MonthlySalesVsCollection", StringComparison.OrdinalIgnoreCase))
            {
                UserDetailsModel model = token.ToObject<UserDetailsModel>();
                var data = _mobileService.fetchMonthlySalesCollectionData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("LatestClosingStock", StringComparison.OrdinalIgnoreCase))
            {
                ClosingStockModel model = token.ToObject<ClosingStockModel>();
                var data = _mobileService.fetchLatestClosingStock(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchSchemeReportData", StringComparison.OrdinalIgnoreCase))
            {
                SchemeReportRequestModel model = token.ToObject<SchemeReportRequestModel>();
                var data = _mobileService.fetchSchemeReportData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchNotificationData", StringComparison.OrdinalIgnoreCase))
            {
                ProfileDetails model = token.ToObject<ProfileDetails>();
                var data = _mobileService.fetchNotificationData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchPoStatus", StringComparison.OrdinalIgnoreCase))
            {
                PurchaseOrderStatus model = token.ToObject<PurchaseOrderStatus>();
                var data = _mobileService.fetchPoStatus(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchPoStatusDate", StringComparison.OrdinalIgnoreCase))
            {
                PurchaseOrderStatus model = token.ToObject<PurchaseOrderStatus>();
                var data = _mobileService.fetchPoStatusDate(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchPoTimeline", StringComparison.OrdinalIgnoreCase))
            {
                PurchaseOrderStatus model = token.ToObject<PurchaseOrderStatus>();
                var data = _mobileService.GetTimeline(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchOrder", StringComparison.OrdinalIgnoreCase))
            {
                PurchaseOrderStatus model = token.ToObject<PurchaseOrderStatus>();
                var data = _mobileService.GetPurchaseOrderDetailItems(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchCurrentStock", StringComparison.OrdinalIgnoreCase))
            {
                OrderStocksModel model = token.ToObject<OrderStocksModel>();
                var data = _mobileService.GetCurrentOrderStocks(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchCustomerLastRate", StringComparison.OrdinalIgnoreCase))
            {
                LastRateModel model = token.ToObject<LastRateModel>();
                var data = _mobileService.GetCustomerLastRate(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchOdoMeterModeType", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString(),
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetOdoMeterModeType(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchOdoMeterClaimType", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString(),
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetOdoMeterClaimType(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchVehicleSetup", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString(),
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetVehicleSetup(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchClaimData", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString(),
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetClaimId(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchOdoMeterClaimReport", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString(),
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString(),
                    ["CLAIM_CODE"] = token["CLAIM_CODE"]?.ToString() ?? "VA",
                    //["MODE_CODE"] = token["MODE_CODE"]?.ToString() ?? "",
                    ["FROM_DATE"] = token["FROM_DATE"]?.ToString() ?? "",
                    ["TO_DATE"] = token["TO_DATE"]?.ToString() ?? ""
                };
                var data = _mobileService.GetOdoMeterReport(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchBillwiseAgeingReport", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString(),
                    ["cus_code"] = token["CUSTOMER_CODE"]?.ToString() ?? "",
                    ["FROM_DATE"] = token["FROM_DATE"]?.ToString() ?? "",
                    ["TO_DATE"] = token["TO_DATE"]?.ToString() ?? ""
                };
                var data = _mobileService.billwiseAgeingTransactions(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchBillwiseAgeingReportPdf", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString(),
                    ["cus_code"] = token["CUSTOMER_CODE"]?.ToString() ?? "",
                    ["FROM_DATE"] = token["FROM_DATE"]?.ToString() ?? "",
                    ["TO_DATE"] = token["TO_DATE"]?.ToString() ?? ""
                };
                var data = _mobileService.billwiseAgeingTransactionsPdf(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchOrderList", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.billwiseAgeingTransactions(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchCustomerProductWiseSalesData", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetCustomerProductWiseSales(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchFreeQty", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["CUSTOMER_CODE"] = token["CUSTOMER_CODE"]?.ToString() ?? "",
                    ["FORM_CODE"] = token["FORM_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetFreeQtyData(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("fetchFarmerProblems", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.GetFarmerProblems(model, dbContext);
                Output = data;
            }
            #endregion Fetching

            #region Inserting
            else if (Action.Equals("updatePurchaseOrder", StringComparison.OrdinalIgnoreCase))
            {
                var model = new Dictionary<string, string>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["ORDER_NO"] = token["ORDER_NO"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString(),
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString()
                };
                var data = _mobileService.updatePurchaseOrder(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("saveFarmProblems", StringComparison.OrdinalIgnoreCase))
            {
                var farmProblems = token["farmProblems"] as JArray;
                var farmProblemsList = new List<Dictionary<string, object>>();
                if (farmProblems != null)
                {
                    foreach (var farm in farmProblems)
                    {
                        var problemsList = new List<Dictionary<string, object>>();
                        var problems = farm["problems"] as JArray;

                        if (problems != null)
                        {
                            foreach (var problem in problems)
                            {
                                problemsList.Add(new Dictionary<string, object>
                                {
                                    ["ID"] = problem["problemId"]?.ToString() ?? "",
                                    ["NAME"] = problem["problemName"]?.ToString() ?? "",
                                    ["description"] = problem["description"]?.ToString() ?? "",
                                    ["recommendation"] = problem["recommendation"]?.ToString() ?? "",
                                });
                            }
                        }

                        farmProblemsList.Add(new Dictionary<string, object>
                        {
                            ["cropName"] = farm["cropName"]?.ToString() ?? "",
                            ["problems"] = problemsList
                        });
                    }
                }

                var model = new Dictionary<string, dynamic>
                {
                    ["SP_CODE"] = token["SP_CODE"]?.ToString() ?? "",
                    ["COMPANY_CODE"] = token["COMPANY_CODE"]?.ToString() ?? "",
                    ["BRANCH_CODE"] = token["BRANCH_CODE"]?.ToString() ?? "",
                    ["farmerId"] = token["farmerId"]?.ToString() ?? "",
                    ["altitude"] = token["altitude"]?.ToString() ?? "",
                    ["farmProblems"] = farmProblemsList
                };

                var data = _mobileService.saveFarmProblems(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("updateMyLocation", StringComparison.OrdinalIgnoreCase))
            {
                UpdateRequestModel model = token.ToObject<UpdateRequestModel>();
                var data = _mobileService.UpdateMyLocation(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("UpdateCurrentLocation", StringComparison.OrdinalIgnoreCase))
            {
                UpdateRequestModel model = token.ToObject<UpdateRequestModel>();
                var data = _mobileService.UpdateCurrentLocation(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("saveExtraActivity", StringComparison.OrdinalIgnoreCase))
            {
                UpdateRequestModel model = token.ToObject<UpdateRequestModel>();
                var data = _mobileService.SaveExtraActivity(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("updateCustomerLocation", StringComparison.OrdinalIgnoreCase))
            {
                UpdateCustomerRequestModel model = token.ToObject<UpdateCustomerRequestModel>();
                var data = _mobileService.UpdateCustomerLocation(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("newMarketingInformation", StringComparison.OrdinalIgnoreCase))
            {
                InformationSaveModel model = token.ToObject<InformationSaveModel>();
                var data = _mobileService.NewMarketingInformation(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("newCompetitorInformation", StringComparison.OrdinalIgnoreCase))
            {
                InformationSaveModel model = token.ToObject<InformationSaveModel>();
                var data = _mobileService.NewCompetitorInformation(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("saveQuestionaire", StringComparison.OrdinalIgnoreCase))
            {
                QuestionaireSaveModel model = token.ToObject<QuestionaireSaveModel>();
                var data = _mobileService.SaveQuestionaire(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("updateDealerStock", StringComparison.OrdinalIgnoreCase))
            {
                UpdateEntityRequestModel model = token.ToObject<UpdateEntityRequestModel>();
                var data = _mobileService.UpdateDealerStock(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("updateDistributorStock", StringComparison.OrdinalIgnoreCase))
            {
                UpdateEntityRequestModel model = token.ToObject<UpdateEntityRequestModel>();
                var data = _mobileService.UpdateDistributorStock(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("updateResellerStock", StringComparison.OrdinalIgnoreCase))
            {
                UpdateEntityRequestModel model = token.ToObject<UpdateEntityRequestModel>();
                var data = _mobileService.UpdateResellerStock(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("saveScheme", StringComparison.OrdinalIgnoreCase))
            {
                SchemeModel model = token.ToObject<SchemeModel>();
                var data = _mobileService.SaveScheme(model, dbContext);
                Output = data;
            }
            else if (Action.Equals("editOutlet", StringComparison.OrdinalIgnoreCase))
            {
                CreateResellerModel model = token.ToObject<CreateResellerModel>();
                var data = _mobileService.UpdateReseller(model, dbContext);
                Output = data;
            }
           
            else if (Action.Equals("eodUpload", StringComparison.OrdinalIgnoreCase))
            {
                var attenModel = new UpdateEodUpdate();
                var dataToken = token["data"];
                //var remarks = token["remarks"].ToString();
                var remarks = (string)token.SelectToken("remarks");
                var sp_code = (string)token.SelectToken("SP_CODE");
                var company_code = (string)token.SelectToken("COMPANY_CODE");
                var listModel = dataToken.ToObject<List<UpdateEodUpdate>>();
                var result = new Dictionary<string, string>();
                foreach (var model in listModel)
                {
                    UpdateRequestModel modelEod = new UpdateRequestModel()
                    {
                        sp_code = model.sp_code,
                        user_id = model.user_id,
                        latitude = model.latitude,
                        longitude = model.longitude,
                        COMPANY_CODE = model.COMPANY_CODE,
                        BRANCH_CODE = model.BRANCH_CODE,
                        Track_Type = "EOD",
                        PO_DCOUNT = model.PO_D_COUNT,
                        PO_RCOUNT = model.PO_R_COUNT,
                        RES_CONTACT_PHOTO = model.reseller_contact_photo,
                        RES_DETAIL = model.reseller_detail,
                        RES_ENTITY = model.reseller_entity,
                        RES_MASTER = model.reseller_master,
                        RES_PHOTO = model.reseller_photo,
                        remarks = remarks,
                        Time_Eod = model.Time_Eod ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                        Saved_Date = model.Saved_Date ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                    };
                    result = this._mobileService.UpdateCurrentLocation(modelEod, dbContext);
                }
                if (listModel.Count > 0)
                {
                    listModel[0].Remarks = remarks;
                }
                else
                {
                    var branch_code = (string)token.SelectToken("BRANCH_CODE");
                    var user_id = (string)token.SelectToken("USER_ID");
                    var latitude = (string)token.SelectToken("latitude");
                    var longitude = (string)token.SelectToken("longitude");
                    var Time_Eod = (string)token.SelectToken("TIME_EOD");
                    var Saved_Date = (string)token.SelectToken("SAVED_DATE");
                    UpdateRequestModel modelEod = new UpdateRequestModel()
                    {
                        sp_code = sp_code,
                        user_id = user_id,
                        latitude = latitude,
                        longitude = longitude,
                        COMPANY_CODE = company_code,
                        BRANCH_CODE = branch_code,
                        Track_Type = "EOD",
                        PO_DCOUNT = 0,
                        PO_RCOUNT = 0,
                        RES_CONTACT_PHOTO = 0,
                        RES_DETAIL = 0,
                        RES_ENTITY = 0,
                        RES_MASTER = 0,
                        RES_PHOTO = 0,
                        remarks = remarks,
                        Time_Eod = Time_Eod ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                        Saved_Date = Saved_Date ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                    };
                    result = this._mobileService.UpdateCurrentLocation(modelEod, dbContext);
                    //listModel.Add(new UpdateEodUpdate { Remarks = remarks });
                    //result = new Dictionary<string, string> { { "message", "EOD update successfully" } };
                }
                var prefs = _mobileService.FetchPreferences(company_code, dbContext);
                if (prefs.MO_EOD_MAIL == "Y")
                {
                    var mailStatus = _mobileService.SendEODMail(listModel, sp_code, company_code, dbContext);
                }
                Output = result;
            }
            #endregion Inserting
            #endregion Actions

            else
                throw new Exception("Invalid action.");

            return Output;
        }
        public object SelectAction(NameValueCollection Form, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            object Output = new object();
            var Action = Form["action"];
            if (Action == null)
                throw new Exception("Invalid action");
            string Company = Form["COMPANY_CODE"];
            string Branch = Form["BRANCH_CODE"];
            if (string.IsNullOrWhiteSpace(Company))
                throw new Exception("Company Code Not Found.");
            if (string.IsNullOrWhiteSpace(Branch))
                throw new Exception("Branch Code Not Found.");

            if (Action.Equals("createReseller", StringComparison.OrdinalIgnoreCase))
            {
                var model = new CreateResellerModel()
                {
                    user_id = Form["user_id"],
                    reseller_name = Form["reseller_name"],
                    area_code = Form["area_code"],
                    address = Form["address"],
                    pan = Form["pan"],
                    wholeseller = Form["wholeseller"],
                    type_id = Form["type_id"],
                    subtype_id = Form["subtype_id"],
                    Group_id = Form["group_Id"],
                    distributor_code = Form["distributor_code"],
                    wholeseller_code = Form["wholeseller_code"],
                    Reseller_contact = Form["Reseller_contact"],
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    latitude = Form["latitude"],
                    longitude = Form["longitude"],
                    email = Form["email"] ?? "",
                    ROUTE_CODE = Form["ROUTE_CODE"] ?? "",
                };
                for (int i = 0; ; i++)
                {
                    if (string.IsNullOrWhiteSpace(Form["contact[" + i + "][name]"]))
                        break;
                    var con = new ContactModel
                    {
                        contact_suffix = Form["contact[" + i + "][contact_suffix]"],
                        designation = Form["contact[" + i + "][designation]"],
                        name = Form["contact[" + i + "][name]"],
                        number = Form["contact[" + i + "][number]"],
                        primary = Form["contact[" + i + "][primary]"],
                    };
                    model.contact.Add(con);
                }
                var coll = new Dictionary<string, string>();
                foreach (string tagName in Files)
                {
                    coll.Add(tagName, Form[tagName + "[description]"]);
                }
                Output = this._mobileService.CreateReseller(model, Files, coll, dbContext);
            }
            else if (Action.Equals("createDistributor", StringComparison.OrdinalIgnoreCase))
            {
                var model = new CreateDistributorModel()
                {
                    user_id = Form["user_id"],
                    distributor_name = Form["distributor_name"],
                    area_code = Form["area_code"],
                    address = Form["address"],
                    pan = Form["pan_no"],
                    //wholeseller = Form["wholeseller"],
                    //type_id = Form["type_id"],
                    //subtype_id = Form["subtype_id"],
                    Group_id = Form["group_Id"],
                    distributor_code = Form["distributor_code"],
                    //wholeseller_code = Form["wholeseller_code"],
                    Distributor_contact = Form["Distributor_contact"],
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    DISTRIBUTOR_TYPE_ID = Form["DISTRIBUTOR_TYPE_ID"],
                    DISTRIBUTOR_SUBTYPE_ID = Form["DISTRIBUTOR_SUBTYPE_ID"],
                    latitude = Form["latitude"],
                    longitude = Form["longitude"],
                    email = Form["email"] ?? "",
                    ROUTE_CODE = Form["ROUTE_CODE"] ?? "",
                };
                for (int i = 0; ; i++)
                {
                    if (string.IsNullOrWhiteSpace(Form["contact[" + i + "][name]"]))
                        break;
                    var con = new ContactModel
                    {
                        contact_suffix = Form["contact[" + i + "][contact_suffix]"],
                        designation = Form["contact[" + i + "][designation]"],
                        name = Form["contact[" + i + "][name]"],
                        number = Form["contact[" + i + "][number]"],
                        primary = Form["contact[" + i + "][primary]"],
                    };
                    model.contact.Add(con);
                }
                var coll = new Dictionary<string, string>();
                foreach (string tagName in Files)
                {

                    //String data = tagName + "[description]";
                    //System.Diagnostics.Debug.WriteLine(tagName + "[description]");
                    coll.Add(tagName, Form[tagName + "[description]"]);
                }
                Output = this._mobileService.CreateDistributor(model, Files, coll, dbContext);
            }
            else if (Action.Equals("newCollection", StringComparison.OrdinalIgnoreCase))
            {


                //VALUES('{model.sp_code}', '{model.entity_code}', '{model.entity_type}', '{model.bill_no}', '{model.cheque_no}', '{model.bank_name}', '{model.amount}', '{model.payment_mode}', TO_DATE('{model.cheque_clearance_date}', 'dd-mm-yyyy'),
                // '{model.cheque_deposit_bank}', '{model.latitude}', '{model.longitude}', '{model.remarks}', '{model.created_by}', 'N', '{model.COMPANY_CODE}', '{model.BRANCH_CODE}')";

                var model = new CollectionRequestModel()
                {
                    sp_code = Form["sp_code"],
                    entity_code = Form["entity_code"],
                    entity_type = Form["entity_type"],
                    bill_no = Form["bill_no"],
                    cheque_no = Form["cheque_no"],
                    bank_name = Form["bank_name"],
                    amount = Form["amount"],
                    payment_mode = Form["payment_mode"],
                    cheque_clearance_date = Form["cheque_clearance_date"],
                    cheque_deposit_bank = Form["cheque_deposit_bank"],
                    latitude = Form["latitude"],
                    longitude = Form["longitude"],
                    remarks = Form["remarks"],
                    created_by = Form["created_by"],
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    otp_code = Form["OTP_CODE"]
                };

                //for (int i = 0; ; i++)
                //{
                //    if (string.IsNullOrWhiteSpace(Form["contact[" + i + "][name]"]))
                //        break;
                //    var con = new ContactModel
                //    {
                //        contact_suffix = Form["contact[" + i + "][contact_suffix]"],
                //        designation = Form["contact[" + i + "][designation]"],
                //        name = Form["contact[" + i + "][name]"],
                //        number = Form["contact[" + i + "][number]"],
                //        primary = Form["contact[" + i + "][primary]"],
                //    };
                //    model.contact.Add(con);
                //}
                //var coll = new Dictionary<string, string>();
                //foreach (string tagName in Files)
                //{
                //    coll.Add(tagName, Form[tagName + "[description]"]);
                //}
                Output = this._mobileService.NewCollection(model, Files, dbContext);
            }
            else if (Action.Equals("odoMeterClaim", StringComparison.OrdinalIgnoreCase))
            {
                var formData = new Dictionary<string, string>
                {
                    ["SP_CODE"] = Form["SP_CODE"] ?? "",
                    ["CLAIM_CODE"] = Form["CLAIM_CODE"] ?? "",
                    ["MODE_CODE"] = Form["MODE_CODE"] ?? "",
                    ["TRAVEL_PURPOSE"] = Form["TRAVEL_PURPOSE"] ?? "",
                    ["MILEAGE"] = Form["MILEAGE"] ?? "",
                    ["FUEL_PRICE"] = Form["FUEL_PRICE"] ?? "",
                    ["START_KM_READING"] = Form["START_KM_READING"] ?? "",
                    ["END_KM_READING"] = Form["END_KM_READING"] ?? "",
                    ["TOTAL_EXPENSE"] = Form["TOTAL_EXPENSE"] ?? "",
                    ["IN_REMARKS"] = Form["START_REMARKS"] ?? "",
                    ["OUT_REMARKS"] = Form["END_REMARKS"] ?? "",
                    ["COMPANY_CODE"] = Form["COMPANY_CODE"],
                    ["BRANCH_CODE"] = Form["BRANCH_CODE"],
                    ["CHECK_IN_TIME"] = Form["CHECK_IN_TIME"],
                    ["CHECK_OUT_TIME"] = Form["CHECK_OUT_TIME"]
                };

                Output = this._mobileService.OdoMeterClaim(formData, Files, dbContext);
            }
            else if (Action.Equals("odoMeterUpdateClaim", StringComparison.OrdinalIgnoreCase))
            {
                var formData = new Dictionary<string, string>
                {
                    ["SP_CODE"] = Form["SP_CODE"] ?? "",
                    ["CLAIM_ID"] = Form["CLAIM_ID"] ?? "",
                    ["CLAIM_CODE"] = Form["CLAIM_CODE"] ?? "",
                    ["MODE_CODE"] = Form["MODE_CODE"] ?? "",
                    ["TRAVEL_PURPOSE"] = Form["TRAVEL_PURPOSE"] ?? "",
                    ["MILEAGE"] = Form["MILEAGE"] ?? "",
                    ["FUEL_PRICE"] = Form["FUEL_PRICE"] ?? "",
                    ["START_KM_READING"] = Form["START_KM_READING"] ?? "",
                    ["END_KM_READING"] = Form["END_KM_READING"] ?? "",
                    ["TOTAL_EXPENSE"] = Form["TOTAL_EXPENSE"] ?? "",
                    ["IN_REMARKS"] = Form["START_REMARKS"] ?? "",
                    ["OUT_REMARKS"] = Form["END_REMARKS"] ?? "",
                    ["COMPANY_CODE"] = Form["COMPANY_CODE"],
                    ["BRANCH_CODE"] = Form["BRANCH_CODE"],
                    ["CHECK_IN_TIME"] = Form["CHECK_IN_TIME"],
                    ["CHECK_OUT_TIME"] = Form["CHECK_OUT_TIME"]
                };

                Output = this._mobileService.UpdateOdoMeterClaim(formData, Files, dbContext);
            }
            else if (Action.Equals("vehicleSetup", StringComparison.OrdinalIgnoreCase))
            {
                var formData = new Dictionary<string, string>
                {
                    ["SP_CODE"] = Form["SP_CODE"],
                    ["MODE_CODE"] = Form["MODE_CODE"] ?? "",
                    ["VEHICLE_NO"] = Form["VEHICLE_NO"] ?? "",
                    ["MODEL"] = Form["MODEL"] ?? "",
                    ["MILEAGE"] = Form["MILEAGE"] ?? "",
                    ["YEAR_MADE"] = Form["YEAR_MADE"] ?? "",
                    ["FUEL_PRICE"] = Form["FUEL_PRICE"] ?? "",
                    ["COMPANY_CODE"] = Form["COMPANY_CODE"],
                    ["BRANCH_CODE"] = Form["BRANCH_CODE"]
                };
                Output = this._mobileService.OdoMeterVehicleSetup(formData, Files, dbContext);
            }
            else if (Action.Equals("updateVehicleSetup", StringComparison.OrdinalIgnoreCase))
            {
                var formData = new Dictionary<string, string>
                {
                    ["SP_CODE"] = Form["SP_CODE"],
                    ["MODE_CODE"] = Form["MODE_CODE"] ?? "",
                    ["VEHICLE_NO"] = Form["VEHICLE_NO"] ?? "",
                    ["MODEL"] = Form["MODEL"] ?? "",
                    ["ID"] = Form["ID"] ?? "",
                    ["MILEAGE"] = Form["MILEAGE"] ?? "",
                    ["YEAR_MADE"] = Form["YEAR_MADE"] ?? "",
                    ["FUEL_PRICE"] = Form["FUEL_PRICE"] ?? "",
                    ["COMPANY_CODE"] = Form["COMPANY_CODE"],
                    ["BRANCH_CODE"] = Form["BRANCH_CODE"]
                };
                Output = this._mobileService.OdoMeterUpdateVehicleSetup(formData, Files, dbContext);
            }

            //else if (Action.Equals("ApproveDelivery", StringComparison.OrdinalIgnoreCase))
            //{
            //    try
            //    {
            //        // Check if the request contains multipart/form-data
            //        if (!Request.Content.IsMimeMultipartContent())
            //        {
            //            Output.response = false;
            //            Output.error = "Unsupported media type";
            //            return Output;
            //        }

            //        var provider = new MultipartMemoryStreamProvider();
            //        Request.Content.ReadAsMultipartAsync(provider);

            //        var number = token["NUMBER"].ToString();
            //        var file = provider.Contents.FirstOrDefault();

            //        if (file == null)
            //        {
            //            Output.response = false;
            //            Output.error = "No file was uploaded";
            //            return Output;
            //        }

            //        // Generate a unique filename
            //        var originalFileName = file.Headers.ContentDisposition.FileName.Trim('"');
            //        var fileExt = Path.GetExtension(originalFileName);
            //        var newFileName = $"DELIVERY_{number}_{Guid.NewGuid()}{fileExt}";

            //        // Define storage path (create if doesn't exist)
            //        var uploadPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"Areas\NeoErp.Distribution\Images\EntityImages";
            //        Directory.CreateDirectory(uploadPath);

            //        // Save the file
            //        var filePath = Path.Combine(uploadPath, newFileName);
            //        using (var stream = new FileStream(filePath, FileMode.Create))
            //        {
            //            file.CopyToAsync(stream);
            //        }

            //        // Save to database or return filename
            //        Output.result = newFileName;
            //        Output.response = true;
            //        Output.error = "";
            //    }
            //    catch (Exception ex)
            //    {
            //        // Log the exception if needed
            //        Output.response = false;
            //        Output.error = "An error occurred while uploading the image";
            //        // Output.error = ex.Message; // Only for debugging
            //    }

            //    return Output;
            //}

            else if (Action.Equals("newPurchaseOrder", StringComparison.OrdinalIgnoreCase))
            {
                //var locationInfo = new UpdateRequestModel();
                /*{
                    sp_code = Form["LocationInfo.sp_code"],
                    customer_code = Form["LocationInfo.customer_code"],
                    customer_type = Form["LocationInfo.customer_type"],
                    remarks = Form["LocationInfo.remarks"],
                    is_visited = Form["LocationInfo.is_visited"],
                    destination = Form["LocationInfo.destination"],
                    Track_Type = string.IsNullOrEmpty(Form["LocationInfo.Track_Type"]) ? "TRK" : Form["LocationInfo.Track_Type"],
                    PO_DCOUNT = int.TryParse(Form["LocationInfo.PO_DCOUNT"], out int dcount) ? dcount : 0,
                    PO_RCOUNT = int.TryParse(Form["LocationInfo.PO_RCOUNT"], out int rcount) ? rcount : 0,
                    RES_DETAIL = int.TryParse(Form["LocationInfo.RES_DETAIL"], out int resDetail) ? resDetail : 0,
                    RES_MASTER = int.TryParse(Form["LocationInfo.RES_MASTER"], out int resMaster) ? resMaster : 0,
                    RES_ENTITY = int.TryParse(Form["LocationInfo.RES_ENTITY"], out int resEntity) ? resEntity : 0,
                    RES_PHOTO = int.TryParse(Form["LocationInfo.RES_PHOTO"], out int resPhoto) ? resPhoto : 0,
                    RES_CONTACT_PHOTO = int.TryParse(Form["LocationInfo.RES_CONTACT_PHOTO"], out int resContactPhoto) ? resContactPhoto : 0,
                    Time_Eod = string.IsNullOrEmpty(Form["LocationInfo.Time_Eod"])
                        ? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")
                        : Form["LocationInfo.Time_Eod"]
                };*/

                var model = new PurchaseOrderModel
                {
                    reseller_code = Form["reseller_code"],
                    distributor_code = string.IsNullOrWhiteSpace(Form["distributor_code"]) ? Form["customer_code"] : Form["distributor_code"] ,
                    type = Form["type"],
                    login_code = Form["login_code"],
                    Order_No = Form["Order_No"],
                    form_code = Form["form_code"],
                    Order_Date = Form["Order_Date"],
                    Dispatch_From = Form["Dispatch_From"],
                    WholeSeller_Code = Form["WholeSeller_Code"],

                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    user_id = Form["user_id"],
                    latitude = Form["latitude"],
                    longitude = Form["longitude"],
                    Sync_Id = Form["Sync_Id"],
                    products = new List<ProductModel>()
                    //LocationInfo = locationInfo
                };

                for (int i = 0; ; i++)
                {
                    var itemCode = Form[$"products[{i}][ITEM_CODE]"];
                    if (string.IsNullOrEmpty(itemCode)) break;

                    model.products.Add(new ProductModel
                    {
                        item_code = itemCode,
                        mu_code = Form[$"products[{i}][MU_CODE]"],
                        quantity = Convert.ToDecimal(Form[$"products[{i}][QUANTITY]"]),
                        reject_flag = Form[$"products[{i}][REJECT_FLAG]"],
                        rate = Convert.ToDecimal(Form[$"products[{i}][RATE]"]),
                        calc_rate = Convert.ToDecimal(Form[$"products[{i}][CALC_RATE]"]),
                        discountRate = Convert.ToDecimal(Form[$"products[{i}][DISCOUNT_RATE_AMOUNT]"]),
                        discountPercentage = Convert.ToDecimal(Form[$"products[{i}][DISCOUNT_PERCENTAGE]"]),
                        discount = Convert.ToDecimal(Form[$"products[{i}][DISCOUNT]"]),
                        billing_name = Form[$"products[{i}][BILLING_NAME]"],
                        remarks = Form[$"products[{i}][REMARKS]"],
                        party_type_code = Form[$"products[{i}][PARTY_TYPE_CODE]"],
                        Sync_Id = Form[$"products[{i}][SYNC_ID]"],
                        PRIORITY_STATUS_CODE = Form[$"products[{i}][PRIORITY_STATUS_CODE]"],
                        CITY_CODE = Form[$"products[{i}][CITY_CODE]"],
                        SHIPPING_CONTACT = Form[$"products[{i}][SHIPPING_CONTACT]"],
                        RESELLER_SHIPPING_ADDRESS = Form[$"products[{i}][RESELLER_SHIPPING_ADDRESS]"],
                        SALES_TYPE_CODE = Form[$"products[{i}][SALES_TYPE_CODE]"],
                        FREE_QTY = Form[$"products[{i}][FREE_QUANTITY]"],
                        //SECOND_QUANTITY = Form[$"products[{i}][SECOND_QUANTITY]"],
                        SECONDARY_UNIT = Form[$"products[{i}][secondary_unit]"]
                    });
                }

                Output = this._mobileService.NewPurchaseOrder(model, Files, dbContext);
            }
            else if (Action.Equals("uploadEntityMedia", StringComparison.OrdinalIgnoreCase))
            {
                var model = new EntityRequestModel()
                {
                    ACC_CODE = Form["SP_CODE"],
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    entity_code = Form["ENTITY_CODE"],
                    entity_type = Form["ENTITY_TYPE"]
                };
                var coll = new Dictionary<string, ImageSaveModel>();
                int i = 0;
                foreach (string tagName in Files)
                {
                    coll.Add(tagName, new ImageSaveModel { Description = Form[$"description[{i}]"], CategoryId = Form[$"CategoryId[{i}]"] });
                    i++;
                }
                Output = this._mobileService.UploadEntityMedia(model, Files, coll, dbContext);
            }
            else if (Action.Equals("AddFarmer", StringComparison.OrdinalIgnoreCase))
            {
                var model = new FarmerModel()
                {
                    FARMER_EDESC = Form["FARMER_EDESC"],
                    FARM_EDESC = Form["FARM_EDESC"],
                    ADDRESS = Form["ADDRESS"],
                    CONTACT_NO = Form["CONTACT_NO"],
                    AREA_CODE = Form["AREA_CODE"],
                    FARM_LONGITUDE = Form["FARM_LONGITUDE"],
                    FARM_LATITUDE = Form["FARM_LATITUDE"],
                    FARM_AREA = Form["FARM_AREA"],
                    FARM_CROPS = Form["FARM_CROPS"],
                    EXPERIENCE = Form["EXPERIENCE"],
                    REMARKS = Form["REMARKS"],
                    DEALERS = new List<DealersList>(),
                    SUB_DEALERS = new List<DealersList>()

                };

               

                int dealerIndex = 0;
                while (true)
                {
                    var dealerId = Form[$"DEALERS[{dealerIndex}][DealerId]"];
                    var dealerName = Form[$"DEALERS[{dealerIndex}][DealerName]"];

                    if (string.IsNullOrEmpty(dealerId) || string.IsNullOrEmpty(dealerName))
                    {
                        break; 
                    }
                    model.DEALERS.Add(new DealersList
                    {
                        DEALER_CODE = dealerId,
                        DEALER_NAME = dealerName
                    });
                    dealerIndex++;
                }
                int subDealerIndex = 0;
                while (true)
                {
                    var subDealerId = Form[$"SUB_DEALERS[{dealerIndex}][DealerId]"];
                    var subDealerName = Form[$"SUB_DEALERS[{dealerIndex}][DealerName]"];

                    if (string.IsNullOrEmpty(subDealerId) || string.IsNullOrEmpty(subDealerName))
                    {
                        break; 
                    }
                    model.SUB_DEALERS.Add(new DealersList
                    {
                        DEALER_CODE = subDealerId,
                        DEALER_NAME = subDealerName
                    });
                    subDealerIndex++;
                }
                Output = this._mobileService.AddFarmer(model, Files, dbContext);
            }
            else if (Action.Equals("DeliveryApprove", StringComparison.OrdinalIgnoreCase))
            {
                var model = new DeliveryApproveModel()
                {
                    DRIVER_MOBILE_NO = Form["DRIVER_MOBILE_NO"] ?? "",
                    REMARKS = Form["REMARKS"] ?? "",
                    REFERENCE_NO = Form["REFERENCE_NO"] ?? "",
                    CHALAN_NO = Form["CHALAN_NO"] ?? "",
                    FORM_CODE = Form["FORM_CODE"] ?? "",
                    TRANSACTION_NO = Form["TRANSACTION_NO"] ?? "",
                    COMPANY_CODE = Form["COMPANY_CODE"] ?? "",
                    BRANCH_CODE = Form["BRANCH_CODE"] ?? "",
                    LATITUDE = Form["LATITUDE"] ?? "",
                    LONGTITUDE = Form["LONGITUDE"] ?? "",
                    RECEIVER_NUMBER = Form["RECEIVER_NUMBER"] ?? "",
                    RECEIVER_NAME = Form["RECEIVER_NAME"] ?? "",
                    TYPE = Form["TYPE"] ?? "",
                    TRANSPORTER_CODE = Form["TRANSPORTER_CODE"] ?? "",
                    BILTY_NUMBER = Form["BILTY_NUMBER"] ?? ""
                };
                Output = this._mobileService.DeliveryApprove(model, Files, dbContext);
            }


            else if (Action.Equals("attendanceUpload", StringComparison.OrdinalIgnoreCase))
            {
                var time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                var attenModel = new EntityRequestModel()
                {
                    ACC_CODE = Form["SP_CODE"],
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    entity_code = Form["SP_CODE"],
                    user_id = Form["user_id"],
                    Saved_Date = time
                };
                //attenModel.entity_type = Form["ENTITY_TYPE"];

                //for location update(LM_LOCATION_TRACKING)
                var LocationModel = new UpdateRequestModel()
                {
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    longitude = Form["LONGITUDE"],
                    latitude = Form["LATITUDE"],
                    sp_code = Form["SP_CODE"],
                    Track_Type = "ATN",
                    Saved_Date = attenModel.Saved_Date ?? time
                };

                //descriptions
                var coll = new Dictionary<string, string>();
                int i = 0;
                foreach (string tagName in Files)
                {
                    coll.Add(tagName, Form[$"description"]);
                    i++;
                }
                Output = this._mobileService.UploadAttendencePic(attenModel, LocationModel, Files, coll, dbContext);
                Output = this._mobileService.UpdateCurrentLocation(LocationModel, dbContext);
            }
            else if (Action.Equals("FSyncUploadEntity", StringComparison.OrdinalIgnoreCase))
            {
                var data = new List<EntityRequestModelOffline>();
                for (int i = 0; ; i++)
                {
                    if (string.IsNullOrWhiteSpace(Form[$"ENTITY_CODE[{i}]"]))
                        break;
                    var model = new EntityRequestModelOffline()
                    {
                        ACC_CODE = Form[$"SP_CODE"],
                        COMPANY_CODE = Form[$"COMPANY_CODE"],
                        BRANCH_CODE = Form[$"BRANCH_CODE"],
                        entity_code = Form[$"ENTITY_CODE[{i}]"],
                        entity_type = Form[$"ENTITY_TYPE[{i}]"],
                        Categoryid = Form[$"categoryid[{i}]"],
                        File_name = Form[$"filename[{i}]"],
                        Title = Form[$"Title[{i}]"],
                        Saved_Date = Form[$"Saved_Date[{i}]"] ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                        Description = Form[$"description[{i}]"],
                        Sync_Id = Form[$"syncId[{i}]"],
                        latitude = Form[$"latitude[{i}]"],
                        longitude = Form[$"longitude[{i}]"],
                        Index = i
                    };
                    data.Add(model);
                }

                var response = this._offlineService.UploadEntityMedia(data, Files, dbContext);
                Output = response.Select(x => new { Key = x.Key, Value = x.Value }).ToList();
            }
            else if (Action.Equals("SyncUploadReseller", StringComparison.OrdinalIgnoreCase))
            {
                var data = new List<EntityRequestModelOffline>();

                for (int i = 0; ; i++)
                {
                    if (string.IsNullOrWhiteSpace(Form[$"ENTITY_CODE[{i}]"]))
                        break;
                    var model = new EntityRequestModelOffline()
                    {
                        ACC_CODE = Form[$"USER_ID"],// why this Acc_code is here
                        user_id = Form[$"USER_ID"],
                        COMPANY_CODE = Form[$"COMPANY_CODE"],
                        BRANCH_CODE = Form[$"BRANCH_CODE"],
                        entity_code = Form[$"ENTITY_CODE[{i}]"],
                        entity_type = Form[$"ENTITY_TYPE[{i}]"],
                        Media_Type = Form[$"MediaType[{i}]"],
                        Saved_Date = Form[$"Saved_Date[{i}]"] ?? DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss"),
                        //model.Categoryid = Form[$"categoryid[{i}]"];
                        //model.File_name = Form[$"filename[{i}]"];
                        Title = Form[$"Title[{i}]"],
                        Description = Form[$"description[{i}]"],
                        Sync_Id = Form[$"syncId[{i}]"],
                        Index = i
                    };
                    data.Add(model);
                }

                var response = this._offlineService.UploadResellerEntityMedia(data, Files, dbContext);
                Output = response.Select(x => new { Key = x.Key, Value = x.Value }).ToList();
            }
            else if (Action.Equals("distanceTracking", StringComparison.OrdinalIgnoreCase))
            {
                var time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");

                var trackingData = new DistanceTrackingModel()
                {
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    Type = Form["Type"],
                    Km_Run = Form["Km_Run"],
                    sp_code = Form["SP_CODE"],
                };

                ////descriptions
                //var coll = new Dictionary<string, string>();
                //int i = 0;
                //foreach (string tagName in Files)
                //{
                //    coll.Add(tagName, Form[$"description"]);
                //    i++;
                //}
            }
            

            else if (Action.Equals("uploadSalesReturnMedia", StringComparison.OrdinalIgnoreCase))
            {
                //var remarks = Form.ContainsKey("REMARKS") ? Form["REMARKS"] : "";
                string condition = Form["CONDITION"] == null ? Form["CONDITION"] : "";
                string complain_type = Form["COMPLAIN_TYPE"] == null ? Form["COMPLAIN_TYPE"] : "";
                string seriousness = Form["SERIOUSNESS"] == null ? Form["SERIOUSNESS"] : "";

                var time = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss");
                var coll = new Dictionary<string, string>();
                var model = new DistributionSalesReturnModel()
                {
                    ORDER_NO = Form["ORDER_NO"],
                    COMPANY_CODE = Form["COMPANY_CODE"],
                    BRANCH_CODE = Form["BRANCH_CODE"],
                    CUSTOMER_CODE = Form["customer_code"],
                    user_id = Form["USER_ID"],
                    P_KEY = Form["P_KEY"],
                    ORDER_DATE = DateTime.Parse(Form["ORDER_DATE"]),
                    ENTITY_TYPE = Form["ENTITY_TYPE"],
                    RESELLER_CODE = Form["RESELLER_CODE"],
                    CONDITION = condition,
                    COMPLAIN_TYPE = complain_type,
                    SERIOUSNESS = seriousness,
                    REMARKS_DIST = Form["REMARKS_DIST"],
                    REMARKS_ASM = Form["REMARKS_ASM"],
                    CREATED_BY = Form["SP_CODE"],
                    CREATED_DATE = DateTime.Parse(Form["CREATED_DATE"]),
                    SAVED_DATE = DateTime.Parse(Form["SAVED_DATE"]),
                    CURRENCY_CODE = Form["CURRENCY_CODE"],
                    EXCHANGE_RATE = Form["EXCHANGE_RATE"],
                    APPROVED_FLAG = Form["APPROVED_FLAG"],
                    DISPATCH_FLAG = Form["DISPATCH_FLAG"],
                    WHOLESELLER_CODE = Form["WHOLESELLER_CODE"],
                    ACKNOWLEDGE_FLAG = Form["ACKNOWLEDGE_FLAG"],
                    REJECT_FLAG = Form["REJECT_FLAG"],
                    DELETED_FLAG = Form["DELETED_FLAG"],
                    SYN_ROWID = Form["SYN_ROWID"],
                    MODIFY_DATE = DateTime.Parse(Form["MODIFY_DATE"]),
                    MODIFY_BY = Form["MODIFY_BY"],
                    BILLING_NAME = Form["BILLING_NAME"],
                    DISPATCH_FROM = Form["DISPATCH_FROM"],
                };
                for (int i = 0; ; i++)
                {
                    if (string.IsNullOrEmpty(Form["products[" + i + "][SYNC_ID]"]))
                    {
                        break;
                    }
                    var con = new SalesReturnProductInfo
                    {
                        SYNC_ID = Form["products[" + i + "][SYNC_ID]"],
                        MU_CODE = Form["products[" + i + "][MU_CODE]"],
                        ITEM_CODE = Form["products[" + i + "][ITEM_CODE]"],
                        MBF_DATA = Form["products[" + i + "][MBF_DATA]"],
                        EXP_DATE = Form["products[" + i + "][EXP_DATE]"],
                        BATCH_NO = Form["products[" + i + "][BATCH_NO]"],
                        QUANTITY = Form["products[" + i + "][QUANTITY]"],
                        SHIPPING_CONTACT = Form["products[" + i + "][SHIPPING_CONTACT]"],
                        BILLING_NAME = Form["products[" + i + "][BILLING_NAME]"],
                        PARTY_TYPE_CODE = Form["products[" + i + "][PARTY_TYPE_CODE]"],
                        REMARKS = Form["products[" + i + "][REMARKS]"]
                    };
                    string descriptionValue = Form[$"products[{i}][DESCRIPTION]"];
                    if (!string.IsNullOrEmpty(descriptionValue))
                    {
                        var descriptions = descriptionValue.Split(',');

                        for (int j = 0; j < descriptions.Length; j++)
                        {
                            string imageKey = $"products[{i}][IMAGES][{j}]";
                            string value = descriptions[j];
                            if (string.IsNullOrEmpty(value))
                            {
                                break;
                            }
                            coll.Add(imageKey, value);
                        }
                    }
                    //for (int k = 0; ; k++)
                    //{
                    //    string imageKey = $"products[{i}][IMAGES][{k}]";
                    //    string imageValue = Form["products[" + i + "][DESCRIPTION][" + k + "]"];
                    //    if (string.IsNullOrEmpty(imageValue))
                    //    {
                    //        break;
                    //    }
                    //}
                    model.products.Add(con);
                }

                Output = this._mobileService.UploadDistSalesReturnPic(model, Files, coll, dbContext);
            }
            else
                throw new Exception("Invalid Action");
            return Output;
        }
    }
}