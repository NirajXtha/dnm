//using NeoErp.sales.Module.Models;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Dynamic;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
//using static NeoErp.sales.Module.Models.GridFilters;
using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Caching;
using System.Web.Http.ModelBinding;
using NeoErp.Core.Controllers;
using NeoErp.Core.Services.CommonSetting;
using NeoErp.Sales.Modules.Services.Models.Settings;
using System.Diagnostics;
using System.Collections;
using NeoErp.Sales.Modules.Services;
using NeoErp.Sales.Modules.Services.Models.NetSalesReports;
using NeoErp.Sales.Modules.Services.Models.BillwisePurchaseSummary;
using NeoErp.Sales.Modules.Services.Models.NetSalesReport;
using NeoErp.Sales.Modules.Services.Models.NetPurchaseModels;
using NeoErp.Sales.Modules.Services.Models.PurchaseSummaryReport;
using NeoErp.Sales.Modules.Services.Models.Audit;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class SalesHomeController : ApiController
    {
        public ISalesRegister _salesRegister { get; set; }
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private ISettingService _setting;
        bool isBusy = false;


        public SalesHomeController(ISalesRegister salesRegister, ICacheManager cacheManger, IWorkContext workContext, ISettingService service)
        {
            this._salesRegister = salesRegister;
            this._workContext = workContext;
            this._cacheManager = cacheManger;
            this._setting = service;
        }
        // GET: SalesReprt
        public gridViewSalesReg GetSalesRegistertable(string formDate, string toDate, int take = 100, int skip = 0, int page = 1, int pageSize = 100)
        {


            var gridViewSalesModel = new gridViewSalesReg();
            var salesRegistrations = _salesRegister.SaleRegistersDateWiseFilter(formDate, toDate);
            gridViewSalesModel.total = salesRegistrations.Count;
            gridViewSalesModel.gridSalesRegReport = salesRegistrations.Skip((page - 1) * pageSize)
                       .Take(pageSize).ToList();
            var allCharges = _salesRegister.GetSalesCharges();
            foreach (var a in gridViewSalesModel.gridSalesRegReport)
            {
                if (gridViewSalesModel.gridSalesRegReport.Any(x => x.charges.Any(y => y.REFERENCE_NO == a.SALES_NO)))
                    continue;
                var abc = allCharges.Where(x => x.REFERENCE_NO == a.SALES_NO).ToList();

                a.charges = abc;
            }


            return gridViewSalesModel;


        }
        [HttpPost]
        public VatRegisterViewModel GetVatRegisters(filterOption model, string formDate, string toDate)
        {

            var vatRegisterViewModel = new VatRegisterViewModel();
            var VatRegistrations = _salesRegister.GetVatRegisterDateWiseFilter(formDate, toDate);
            vatRegisterViewModel.vatregisterModel = VatRegistrations.ToList();
            //checking
            Debug.WriteLine("sadasd");

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                vatRegisterViewModel.vatregisterModel = VatRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                // vatRegisterViewModel.total = VatRegistrations.Count;
            }


            vatRegisterViewModel.vatregisterModel = vatRegisterViewModel.vatregisterModel.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    vatRegisterViewModel.vatregisterModel = vatRegisterViewModel.vatregisterModel.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            vatRegisterViewModel.total = vatRegisterViewModel.vatregisterModel.Count;

            return vatRegisterViewModel;
        }

        [HttpPost]
        public VatRegisterViewModel GetVatRegisters(filterOption model)
        {
            var vatRegisterViewModel = new VatRegisterViewModel();

            var VatRegistrations = _salesRegister.GetVatRegister(model.ReportFilters, _workContext.CurrentUserinformation);
            vatRegisterViewModel.vatregisterModel = VatRegistrations.ToList();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                    {
                        var checkValue = KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters);
                        if (checkValue != "" & checkValue != null)
                            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                    }
                    //whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                vatRegisterViewModel.vatregisterModel = VatRegistrations.Where(whereClause, parameters.ToArray()).ToList();

                // vatRegisterViewModel.total = VatRegistrations.Count;
            }
            vatRegisterViewModel.total = VatRegistrations.Count;
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    vatRegisterViewModel.vatregisterModel = vatRegisterViewModel.vatregisterModel.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            vatRegisterViewModel.total = vatRegisterViewModel.vatregisterModel.Count;
            vatRegisterViewModel.AggregationResult = KendoGridHelper.GetAggregation(vatRegisterViewModel.vatregisterModel, model.aggregate);
            vatRegisterViewModel.vatregisterModel = vatRegisterViewModel.vatregisterModel.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();

            return vatRegisterViewModel;
        }

        [HttpPost]
        public ProductWiseSalesSummaryViewModel GetProductWiseSalesSummary(filterOption model)
        {
            var viewModel = new ProductWiseSalesSummaryViewModel();

            var productSales = _salesRegister.GetProductWiseSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = productSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Any() == true)
            {
                var parameters = new ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<ProductWiseSalesSummaryModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                if (whereClauses.Any())
                {
                    var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                    filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
                }
            }

            // Set total before pagination
            viewModel.Total = filteredSales.Count();

            // Sorting
            if (model.sort?.Any() == true)
            {
                var sortingString = string.Join(", ", model.sort.Select(s => $"{s.Field} {s.Dir}"));
                filteredSales = filteredSales.OrderBy(sortingString);
            }

            // Aggregation (before pagination)
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Pagination
            if (model.pageSize > 0)
            {
                filteredSales = filteredSales
                    .Skip((model.page - 1) * model.pageSize)
                    .Take(model.pageSize);
            }

            viewModel.ProductWiseSalesModel = filteredSales.ToList();

            return viewModel;
        }


        [HttpPost]
        public DateWiseSalesDetailsViewModel GetDateWiseSalesDetails(filterOption model)
        {
            var viewModel = new DateWiseSalesDetailsViewModel();

            var productSales = _salesRegister.GetDateWiseSalesDetails(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = productSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DateWiseSalesDetailsModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWiseSalesModelData = filteredSales.ToList();

            return viewModel;
        }

        //datewise sales return details 
        [HttpPost]
        public DateWiseSalesReturnDetailsViewModel GetDateWiseSalesReturnDetails(filterOption model)
        {
            var viewModel = new DateWiseSalesReturnDetailsViewModel();

            var dateSales = _salesRegister.GetDateWiseSalesReturnDetails(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DateWiseSalesDetailsModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWiseSalesReturnModel = filteredSales.ToList();

            return viewModel;
        }
        // bill wise sales summary
        [HttpPost]
        public BillwiseSalesSummaryViewModel GetBillwiseSalesSummary(filterOption model)
        {
            var viewModel = new BillwiseSalesSummaryViewModel();

            var billwiseSales = _salesRegister.GetBillwiseSalesSummary(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwiseSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DateWiseSalesDetailsModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BillwiseSalesSummary = filteredSales.ToList();

            return viewModel;
        }
        // bill wise sales return summary
        [HttpPost]
        public BillwiseSalesReturnSummaryViewModel GetBillwiseSalesReturnSummary(filterOption model)
        {
            var viewModel = new BillwiseSalesReturnSummaryViewModel();

            var billwiseReturnSales = _salesRegister.GetBillwiseSalesReturnSummary(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwiseReturnSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DateWiseSalesDetailsModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BillwiseSalesReturnSummary = filteredSales.ToList();

            return viewModel;
        }
        //purchase landed cost
        [HttpPost]
        public BillwisePurchaseSummaryModel GetPurchaseLandedCost(filterOption model)
        {
            var viewModel = new BillwisePurchaseSummaryModel();

            var billwisePurchase = _salesRegister.GetPurchaseLandedCost(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwisePurchase.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<BillwisePurchaseSummaryViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BillwisePurchaseSummary = filteredSales.ToList();

            return viewModel;
        }
        //datewise purchase details
        [HttpPost]
        public BillwisePurchaseSummaryModel GetDatewisePurchaseDetails(filterOption model)
        {
            var viewModel = new BillwisePurchaseSummaryModel();

            var billwisePurchase = _salesRegister.GetDatewisePurchaseDetails(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwisePurchase.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<BillwisePurchaseSummaryViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BillwisePurchaseSummary = filteredSales.ToList();

            return viewModel;
        }
        //Billwise purchase sumarry 
        [HttpPost]
        public BillwisePurchaseSummaryModel GetBillwisePurchaseSummary(filterOption model)
        {
            var viewModel = new BillwisePurchaseSummaryModel();

            var billwisePurchase = _salesRegister.GetBillwisePurchaseSummary(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwisePurchase.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<BillwisePurchaseSummaryViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BillwisePurchaseSummary = filteredSales.ToList();

            return viewModel;
        }
        //Billwise purchase Return sumarry 
        [HttpPost]
        public BillwisePurchaseSummaryModel GetBillwisePurchaseReturnSummary(filterOption model)
        {
            var viewModel = new BillwisePurchaseSummaryModel();

            var billwisePurchase = _salesRegister.GetBillwisePurchaseReturnSummary(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwisePurchase.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<BillwisePurchaseSummaryViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BillwisePurchaseSummary = filteredSales.ToList();

            return viewModel;
        }

        //
        [HttpPost]
        public ProductwisePurchaseSummaryModel GetProductwisePurchaseSummary(filterOption model)
        {
            var viewModel = new ProductwisePurchaseSummaryModel();

            var billwisePurchase = _salesRegister.GetProductwisePurchaseSummary(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = billwisePurchase.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<ProductwisePurchaseSummaryViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.ProductwisePurchaseSummary = filteredSales.ToList();

            return viewModel;
        }

        [HttpPost]
        public GridViewSalesVatSummary GetSalesVatRegisterSummaryReport(filterOption model)
        {
            var CustomerSalesSummary = new GridViewSalesVatSummary();
            var customerSales = _salesRegister.GetSalesVatWiseSummary(model.ReportFilters).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesVatWiseSummaryModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesVatWiseSummaryModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                CustomerSalesSummary.gridSalesVatReport = customerSales.Where(whereClause, parameters.ToArray()).ToList();
                CustomerSalesSummary.total = CustomerSalesSummary.gridSalesVatReport.Count;
            }
            else
            {
                CustomerSalesSummary.gridSalesVatReport = customerSales.ToList();
                CustomerSalesSummary.total = CustomerSalesSummary.gridSalesVatReport.Count;
            }
            var allCharges = new List<Charges>();
            var filterCharge = new List<Charges>();
            if (CustomerSalesSummary.gridSalesVatReport.Count > 0)
                allCharges = _salesRegister.GetSumCharges(model.ReportFilters);

            foreach (var a in CustomerSalesSummary.gridSalesVatReport)
            {
                if (CustomerSalesSummary.gridSalesVatReport.Any(x => x.charges.Any(y => y.CustomerId == a.CustomerId)))
                    continue;

                var itemCharges = allCharges.Where(x => x.CustomerId == a.CustomerId).ToList();
                var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                var sumOfAddChargesWithoutVat = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE != "VTD").Sum(x => x.CHARGE_AMOUNT);
                a.NetAmount = a.GrossAmount - sumOfSubtractCharges + sumOfAddChargesWithoutVat;
                a.InvoiceAmount = a.NetAmount + itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE == "VTD").Sum(x => x.CHARGE_AMOUNT);
                a.charges = itemCharges;
                filterCharge.AddRange(itemCharges);
            }

            var aggregationDictionary = KendoGridHelper.GetAggregation(CustomerSalesSummary.gridSalesVatReport, model.aggregate);

            var groupData = filterCharge.GroupBy(q => q.CHARGE_CODE).Select(group => new
            {
                Code = group.Key,
                Sum = group.Sum(q => q.CHARGE_AMOUNT),
                Min = group.Min(q => q.CHARGE_AMOUNT),
                Max = group.Max(q => q.CHARGE_AMOUNT),
                Average = group.Average(q => q.CHARGE_AMOUNT)
            });

            foreach (var item in groupData)
            {
                aggregationDictionary.Add(item.Code, new AggregationModel()
                {
                    average = item.Average ?? 0,
                    max = item.Max ?? 0,
                    min = item.Min ?? 0,
                    sum = item.Sum ?? 0
                });
            }

            foreach (var item in model.aggregate)
            {
                if (aggregationDictionary.ContainsKey(item.field))
                    continue;

                aggregationDictionary.Add(item.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    CustomerSalesSummary.gridSalesVatReport = CustomerSalesSummary.gridSalesVatReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                CustomerSalesSummary.gridSalesVatReport = CustomerSalesSummary.gridSalesVatReport.OrderBy(x => x.CustomerName).ToList();
            CustomerSalesSummary.AggregationResult = aggregationDictionary;

            CustomerSalesSummary.gridSalesVatReport = CustomerSalesSummary.gridSalesVatReport.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();


            return CustomerSalesSummary;
        }


        // branch wise emplpyee net sales 
        [HttpPost]
        public BranchwiseEmployeeNetSalesViewModel GetBranchwiseEmployeeNetSales(filterOption model)
        {
            var viewModel = new BranchwiseEmployeeNetSalesViewModel();

            var branchwiseSales = _salesRegister.BranchwiseEmployeeNetSales(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = branchwiseSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<BranchwiseEmployeeNetSalesModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.BranchwiseEmployeeSales = filteredSales.ToList();

            return viewModel;
        }


        [HttpPost]
        public dynamic GetGroupwiseStockSummaryReport([FromBody] dynamic data)
        {
            var groupWise = this._salesRegister.GetGroupwiseStockSummary(data, _workContext.CurrentUserinformation);

            return groupWise;
        }
        [HttpPost]
        public dynamic GetBranchwiseNetSalesReport([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetBranchwiseNetSalesReport(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        //Branch wise customer product net sales 
        [HttpPost]
        public dynamic GetBranchwiseCustomerProductNetSales([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetBranchwiseCustomerProductNetSales(data, _workContext.CurrentUserinformation);

            return branchWise;
        }

        [HttpPost]
        public dynamic GetMonthwiseProductNetSales([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetMonthwiseProductNetSales(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        //Monthwise Customer Sales Collection
        [HttpPost]
        public dynamic GetMonthwiseCustomerSalesCollection([FromBody] dynamic data)
        {
            var collection = this._salesRegister.GetMonthwiseCustomerSalesCollection(data, _workContext.CurrentUserinformation);
            return collection;
        }

        //Datewise Net Sales Report 
        [HttpPost]
        public DatewiseNetSalesModelsViewModel GetDatewiseNetSalesReport(filterOption model)
        {
            var viewModel = new DatewiseNetSalesModelsViewModel();

            var dateSales = _salesRegister.GetDatewiseNetSalesReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DatewiseNetSalesModels>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWiseNetSales = filteredSales.ToList();

            return viewModel;
        }

        //Datewise Net Sales Report 



        //Sales Above OneLakh Report 
        [HttpPost]
        public SalesAboveOneLakhViewModel GetSalesAboveOneLakh(filterOption model)
        {
            var viewModel = new SalesAboveOneLakhViewModel();

            var Sales = _salesRegister.GetSalesAboveOneLakh(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = Sales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<SalesAboveOneLakhModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.SalesAboveOneLakh = filteredSales.ToList();

            return viewModel;
        }

        //Product Groupwise NetSales
        [HttpPost]
        public dynamic GetProductGroupwiseNetSales([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetProductGroupwiseNetSales(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        //Customer wise NetSales
        [HttpPost]
        public dynamic GetCustomerwiseNetSalesRep([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetCustomerwiseNetSalesRep(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        //Product Customerwise NetSales Report
        [HttpPost]
        public dynamic GetProductCustomerwiseNetSalesReport([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetProductCustomerwiseNetSalesReport(data, _workContext.CurrentUserinformation);

            return branchWise;
        }

        //Customer Brandwise NetSales Report
        [HttpPost]
        public dynamic GetCustomerBrandwiseNetSalesReport([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetCustomerBrandwiseNetSalesReport(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        //Employee Customer  NetSales Report
        [HttpPost]
        public dynamic GetEmployeeCustomerwiseNetSalesReport([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetEmployeeCustomerwiseNetSalesReport(data, _workContext.CurrentUserinformation);

            return branchWise;
        }

        //Employee Productwise NetSales Report
        [HttpPost]
        public dynamic GetEmployeeProductwiseNetSalesReport([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetEmployeeProductwiseNetSalesReport(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        ////Productwise Net Sales Report 
        [HttpPost]
        public ProductwiseNetSalesModelsViewModel GetProductwiseNetSalesReport(filterOption model)
        {
            var viewModel = new ProductwiseNetSalesModelsViewModel();

            var dateSales = _salesRegister.GetProductwiseNetSalesReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<ProductwiseNetSalesModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.ProductwiseNetSales = filteredSales.ToList();

            return viewModel;
        }
        //
        ////Productwise Net Sales Report 
        [HttpPost]
        public ProductwiseNetSalesModelsViewModel GetDealerNetSalesReport(filterOption model)
        {
            var viewModel = new ProductwiseNetSalesModelsViewModel();

            var dateSales = _salesRegister.GetDealerNetSalesReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<ProductwiseNetSalesModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.ProductwiseNetSales = filteredSales.ToList();

            return viewModel;
        }

        [HttpPost]
        public dynamic GetAllItems([FromBody] dynamic data)
        {
            var groupWise = this._salesRegister.GetAllItems(data, _workContext.CurrentUserinformation);

            return groupWise;
        }
        [HttpPost]
        public dynamic GetMonthwiseCustomerProductNetSales(dynamic data)
        {

            var netSales = _salesRegister.GetMonthwiseCustomerProductNetSales(data, _workContext.CurrentUserinformation);
            return netSales;

        }
        //monthwise Sales Type Customer Net Sales Report
        [HttpPost]
        public dynamic GetMonthwiseSalesTypeCustomerNetSales(dynamic data)
        {

            var netSales = _salesRegister.GetMonthwiseSalesTypeCustomerNetSales(data, _workContext.CurrentUserinformation);
            return netSales;

        }
        //Monthwise Employee Customer NetSales Report
        [HttpPost]
        public dynamic GetMonthwiseEmployeeCustomerNetSales(dynamic data)
        {

            var netSales = _salesRegister.GetMonthwiseEmployeeCustomerNetSales(data, _workContext.CurrentUserinformation);
            return netSales;

        }
        //Monthwise Employee Prodcut Net Sales Report
        [HttpPost]
        public dynamic GetMonthwiseEmployeeProductNetSales(dynamic data)
        {

            var netSales = _salesRegister.GetMonthwiseEmployeeProductNetSales(data, _workContext.CurrentUserinformation);
            return netSales;

        }
        // Monthwise Region Epmloyee Net Sales Report
        [HttpPost]
        public dynamic GetMonthwiseRegionEmployeeNetSales(dynamic data)
        {

            var netSales = _salesRegister.GetMonthwiseRegionEmployeeNetSales(data, _workContext.CurrentUserinformation);
            return netSales;

        }
        [HttpPost]
        public dynamic GetMonthwiseRegionProductNetSales(dynamic data)
        {

            var regionSales = _salesRegister.GetMonthwiseRegionProductSales(data, _workContext.CurrentUserinformation);
            return regionSales;

        }
        //Brand wise Employee Customer net sales
        [HttpPost]
        public dynamic GetBrandwiseEmployeeCustomerNetSales(dynamic data)
        {

            var regionSales = _salesRegister.GetBrandwiseEmployeeCustomerNetSales(data, _workContext.CurrentUserinformation);
            return regionSales;

        }
        //Customerwise order tracking 
        [HttpPost]
        public dynamic GetCustomerwiseOrderTracking(dynamic data)
        {

            var tracking = _salesRegister.GetCustomerwiseOrderTracking(data, _workContext.CurrentUserinformation);
            return tracking;

        }
        [HttpPost]
        public dynamic GetGroupProductwiseOrderPending(dynamic data)
        {

            var tracking = _salesRegister.GetGroupProductwiseOrderPending(data, _workContext.CurrentUserinformation);
            return tracking;

        }
        //Datewise  Sale detail Report for jewelary 
        [HttpPost]
        public DateWiseSalesDetailsViewModel GetDateWiseSalesDetailsJewellery(filterOption model)
        {
            var viewModel = new DateWiseSalesDetailsViewModel();

            var productSales = _salesRegister.GetDateWiseSalesDetailsJewellery(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = productSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DateWiseSalesDetailsModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWiseSalesModelData = filteredSales.ToList();

            return viewModel;
        }
        [HttpPost]
        public VatSalesRegisterNewViewModel GetVatSalesRegisterNew(filterOption model)
        {
            var viewModel = new VatSalesRegisterNewViewModel();

            var vatRegisterNew = _salesRegister.GetVatSalesRegisterNew(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = vatRegisterNew.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<VatSalesRegisterNewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.VatSalesRegisterNew = filteredSales.ToList();

            return viewModel;
        }


        [HttpPost]
        public VatSalesRegisterNewViewModel GetVatSalesReturnRegisterNew(filterOption model)
        {
            var viewModel = new VatSalesRegisterNewViewModel();

            var vatRegisterNew = _salesRegister.GetVatSalesReturnRegisterNew(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = vatRegisterNew.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<VatSalesRegisterNewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.VatSalesRegisterNew = filteredSales.ToList();

            return viewModel;
        }
        //Vat Pujrchase Register New 
        [HttpPost]
        public VatPurchaseRegisterNewViewModel GetVatPurchaseRegisterNew(filterOption model)
        {
            var viewModel = new VatPurchaseRegisterNewViewModel();

            var vatPurchaserNew = _salesRegister.GetVatPurchaseRegisterNew(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = vatPurchaserNew.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<VatPurchaseRegisterNewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.VatPurchaseNew = filteredSales.ToList();

            return viewModel;
        }
        //Vat Purchase Returnregister New 
        [HttpPost]
        public VatPurchaseRegisterNewViewModel GetVatPurchaseReturnRegisterNew(filterOption model)
        {
            var viewModel = new VatPurchaseRegisterNewViewModel();

            var vatPurchaseReturn = _salesRegister.GetVatPurchaseReturnRegisterNew(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = vatPurchaseReturn.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<VatPurchaseRegisterNewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.VatPurchaseNew = filteredSales.ToList();

            return viewModel;
        }
        [HttpPost]
        public gridViewSalesReg GetSalesRegister(filterOption model, string formDate, string toDate)
        {

            var gridViewSalesModel = new gridViewSalesReg();
            var salesRegistrations = _salesRegister.SaleRegistersDateWiseFilter(formDate, toDate).AsQueryable();
            gridViewSalesModel.total = salesRegistrations.ToList().Count;

            gridViewSalesModel.gridSalesRegReport = salesRegistrations.ToList();

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
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }

            gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            if (gridViewSalesModel.gridSalesRegReport.Count > 0)
            {
                gridViewSalesModel.Aggrationresult.CALC_QUANTITY.sum = gridViewSalesModel.gridSalesRegReport.Sum(x => x.CALC_QUANTITY);
            }
            else
            {
                gridViewSalesModel.Aggrationresult.CALC_QUANTITY.sum = 0;
            }
            //  var test=Json({"test","'2 },{ })

            return gridViewSalesModel;
        }
        [HttpPost]
        public gridViewSalesReg GetSalesRegister(filterOption model)
        {

            var gridViewSalesModel = new gridViewSalesReg();
            var salesRegistrations = _salesRegister.GetSaleRegisters(model.ReportFilters).AsQueryable();
            gridViewSalesModel.total = salesRegistrations.ToList().Count;

            gridViewSalesModel.gridSalesRegReport = salesRegistrations.ToList();
            //  KendoGridHelper.ProcessFilters<SalesRegisterModel>(model,ref salesRegistrations);

            // gridViewSalesModel.gridSalesRegReport= salesRegistrations.Skip((model.page - 1) * model.pageSize)
            //           .Take(model.pageSize).ToList();
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
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }

            gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            if (gridViewSalesModel.gridSalesRegReport.Count > 0)
            {
                gridViewSalesModel.Aggrationresult.CALC_QUANTITY.sum = gridViewSalesModel.gridSalesRegReport.Sum(x => x.CALC_QUANTITY);
            }
            else
            {
                gridViewSalesModel.Aggrationresult.CALC_QUANTITY.sum = 0;
            }
            //  var test=Json({"test","'2 },{ })

            return gridViewSalesModel;
        }

        [HttpPost]
        public SalesRegisterDetialReport GetSalesRegisterMasterReport(filterOption model)
        {
            var gridViewSalesModel = new SalesRegisterDetialReport();
            var salesRegistrations = _salesRegister.SaleRegistersMasterDynamic(model.ReportFilters).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {


                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                    //whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters));
                    {
                        if (model.filter.Filters[i].Field != "VTD" & model.filter.Filters[i].Field != "EDD" & model.filter.Filters[i].Field != "SPDD" & model.filter.Filters[i].Field != "DCD")
                        {
                            var checkValue = KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters);
                            if (checkValue != "" & checkValue != null)
                                whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                        }
                    }
                }
                // Where
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }
            else
            {
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }
            //gridViewSalesModel.gridSalesRegReport = salesRegistrations.ToList();
            //gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            var allCharges = _salesRegister.GetSalesCharges(model.ReportFilters);
            var filterCharge = new List<Charges>();
            foreach (var a in gridViewSalesModel.gridSalesRegReport)
            {
                a.NetAmount = a.GrossAmount;
                a.InvoiceAmount = 0;
                if (allCharges.Count > 0)
                {
                    if (gridViewSalesModel.gridSalesRegReport.Any(x => x.charges.Any(y => y.REFERENCE_NO == a.InvoiceNumber)))
                        continue;

                    var itemCharges = allCharges.Where(x => x.REFERENCE_NO == a.InvoiceNumber && x.APPLY_ON == "D").ToList();
                    var IndivisualCharges = allCharges.Where(x => x.REFERENCE_NO == a.InvoiceNumber && x.APPLY_ON == "I").ToList();
                    var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                    var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE != "VTD").Sum(x => x.CHARGE_AMOUNT);
                    var sumofVatCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE == "VTD").Sum(x => x.CHARGE_AMOUNT);
                    foreach (var indv in IndivisualCharges)
                    {
                        if (itemCharges.Any(x => x.CHARGE_CODE == indv.CHARGE_CODE && x.APPLY_ON == "I"))
                            continue;
                        var charge = new Charges();
                        charge.CHARGE_CODE = indv.CHARGE_CODE;
                        charge.CHARGE_EDESC = indv.CHARGE_EDESC + "(I)";
                        charge.APPLY_ON = indv.APPLY_ON;
                        charge.CHARGE_TYPE_FLAG = indv.CHARGE_TYPE_FLAG;
                        charge.CHARGE_AMOUNT = IndivisualCharges.Where(x => x.CHARGE_CODE == indv.CHARGE_CODE).Sum(x => x.CHARGE_AMOUNT);
                        charge.REFERENCE_NO = a.InvoiceNumber;
                        itemCharges.Add(charge);
                    }
                    var sumOfItemSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D" && x.APPLY_ON == "I").Sum(x => x.CHARGE_AMOUNT);
                    var sumOfItemAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.APPLY_ON == "I" && x.CHARGE_CODE != "VTD").Sum(x => x.CHARGE_AMOUNT);
                    var sumOfItemAddVatCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.APPLY_ON == "I" && x.CHARGE_CODE == "VTD").Sum(x => x.CHARGE_AMOUNT);
                    var extraAddional = sumOfItemAddCharges + sumOfAddCharges;
                    var vatamount = sumOfItemAddVatCharges + sumofVatCharges;
                    a.NetAmount = a.GrossAmount - sumOfSubtractCharges - sumOfItemSubtractCharges;
                    a.NetAmount = a.NetAmount + extraAddional;
                    a.InvoiceAmount = a.NetAmount + vatamount;
                    a.charges = itemCharges;

                    filterCharge.AddRange(itemCharges);
                }
            }
            var aggregationDictionary = KendoGridHelper.GetAggregation(gridViewSalesModel.gridSalesRegReport, model.aggregate);

            var groupData = filterCharge.GroupBy(q => q.CHARGE_CODE).Select(group => new
            {
                Code = group.Key,
                Sum = group.Sum(q => q.CHARGE_AMOUNT),
                Min = group.Min(q => q.CHARGE_AMOUNT),
                Max = group.Max(q => q.CHARGE_AMOUNT),
                Average = group.Average(q => q.CHARGE_AMOUNT)
            });

            foreach (var item in groupData)
            {
                aggregationDictionary.Add(item.Code, new AggregationModel()
                {
                    average = item.Average ?? 0,
                    max = item.Max ?? 0,
                    min = item.Min ?? 0,
                    sum = item.Sum ?? 0
                });
            }
            // For Charges verify is present or not
            foreach (var aggkey in model.aggregate)
            {
                if (aggregationDictionary.Any(x => x.Key == aggkey.field))
                    continue;
                aggregationDictionary.Add(aggkey.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            foreach (var item in model.aggregate)
            {
                if (aggregationDictionary.ContainsKey(item.field))
                    continue;

                aggregationDictionary.Add(item.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            gridViewSalesModel.AggregationResult = aggregationDictionary;
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(x => x.SalesDate).ThenByDescending(x => x.InvoiceNumber).ToList();

            gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.Skip((model.page - 1) * model.pageSize)
                     .Take(model.pageSize).ToList();


            return gridViewSalesModel;
        }

        [HttpPost]
        public List<SalesChildModel> GetSalesRegisterChilds(filterOption model, string SalesNo, string CompanyCode = "01")
        {
            if (string.IsNullOrEmpty(SalesNo))
                throw new Exception();
            var salesItems = _salesRegister.GetSalesItemBySalesId(model, SalesNo, CompanyCode).ToList();

            var allCharges = _salesRegister.GetSalesItemCharges(model.ReportFilters, SalesNo);
            var filterCharge = new List<Charges>();

            foreach (var a in salesItems)
            {
                a.NetAmount = a.GrossAmount;

                if (allCharges.Count > 0)
                {

                    var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ITEM_CODE).ToList();
                    var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                    var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A").Sum(x => x.CHARGE_AMOUNT);
                    a.NetAmount = a.GrossAmount - sumOfSubtractCharges;
                    //  a. = a.GrossAmount - sumOfSubtractCharges + sumOfAddCharges;
                    a.charges = itemCharges;
                    filterCharge.AddRange(itemCharges);
                }
            }
            return salesItems;
        }

        [HttpPost]
        public GridViewSalesVatSummary GetGroupwiseStockSummary(filterOption model)
        {
            var CustomerSalesSummary = new GridViewSalesVatSummary();
            var customerSales = _salesRegister.GetSalesVatWiseSummary(model.ReportFilters).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesVatWiseSummaryModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesVatWiseSummaryModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                CustomerSalesSummary.gridSalesVatReport = customerSales.Where(whereClause, parameters.ToArray()).ToList();
                CustomerSalesSummary.total = CustomerSalesSummary.gridSalesVatReport.Count;
            }
            else
            {
                CustomerSalesSummary.gridSalesVatReport = customerSales.ToList();
                CustomerSalesSummary.total = CustomerSalesSummary.gridSalesVatReport.Count;
            }
            var allCharges = new List<Charges>();
            var filterCharge = new List<Charges>();
            if (CustomerSalesSummary.gridSalesVatReport.Count > 0)
                allCharges = _salesRegister.GetSumCharges(model.ReportFilters);

            foreach (var a in CustomerSalesSummary.gridSalesVatReport)
            {
                if (CustomerSalesSummary.gridSalesVatReport.Any(x => x.charges.Any(y => y.CustomerId == a.CustomerId)))
                    continue;

                var itemCharges = allCharges.Where(x => x.CustomerId == a.CustomerId).ToList();
                var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                var sumOfAddChargesWithoutVat = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE != "VTD").Sum(x => x.CHARGE_AMOUNT);
                a.NetAmount = a.GrossAmount - sumOfSubtractCharges + sumOfAddChargesWithoutVat;
                a.InvoiceAmount = a.NetAmount + itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE == "VTD").Sum(x => x.CHARGE_AMOUNT);
                a.charges = itemCharges;
                filterCharge.AddRange(itemCharges);
            }

            var aggregationDictionary = KendoGridHelper.GetAggregation(CustomerSalesSummary.gridSalesVatReport, model.aggregate);

            var groupData = filterCharge.GroupBy(q => q.CHARGE_CODE).Select(group => new
            {
                Code = group.Key,
                Sum = group.Sum(q => q.CHARGE_AMOUNT),
                Min = group.Min(q => q.CHARGE_AMOUNT),
                Max = group.Max(q => q.CHARGE_AMOUNT),
                Average = group.Average(q => q.CHARGE_AMOUNT)
            });

            foreach (var item in groupData)
            {
                aggregationDictionary.Add(item.Code, new AggregationModel()
                {
                    average = item.Average ?? 0,
                    max = item.Max ?? 0,
                    min = item.Min ?? 0,
                    sum = item.Sum ?? 0
                });
            }

            foreach (var item in model.aggregate)
            {
                if (aggregationDictionary.ContainsKey(item.field))
                    continue;

                aggregationDictionary.Add(item.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    CustomerSalesSummary.gridSalesVatReport = CustomerSalesSummary.gridSalesVatReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                CustomerSalesSummary.gridSalesVatReport = CustomerSalesSummary.gridSalesVatReport.OrderBy(x => x.CustomerName).ToList();
            CustomerSalesSummary.AggregationResult = aggregationDictionary;

            CustomerSalesSummary.gridSalesVatReport = CustomerSalesSummary.gridSalesVatReport.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();


            return CustomerSalesSummary;
        }
        [HttpPost]
        public SalesRegisterViewModel GetSalesRegistersReport(filterOption model)
        {

            var salesRegistersViewModel = new SalesRegisterViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetSalesRegister(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
                    else
                    {
                        //whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
                        var checkValue = KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters);
                        if (checkValue != "" & checkValue != null)
                            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                    }
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
            //salesRegistersViewModel.SalesRegisters = salesRegister.ToList();
            //salesRegistersViewModel.total = salesRegistersViewModel.SalesRegisters.Count;

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
        public List<SalesRegisterCustomerModel> GetSalesRegisterCustomers(filterOption model)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var salesRegister = new List<SalesRegisterCustomerModel>();
            if (this._cacheManager.IsSet("SalesRegisterCustomer"))
            {
                salesRegister = this._cacheManager.Get<List<SalesRegisterCustomerModel>>("SalesRegisterCustomer");
            }
            else
            {
                salesRegister = _salesRegister.SaleRegisterCustomers().ToList();
                this._cacheManager.Set("SalesRegisterCustomer", salesRegister, 10);
            }

            return salesRegister;
        }

        [HttpGet]
        public List<SalesRegisterCustomerModel> GetSalesRegisterGroupCustomers(GridFilters filter)
        {
            var salesRegister = _salesRegister.SaleRegisterGroupCustomers().ToList();
            return salesRegister;
        }
        [HttpGet]
        public List<SalesRegisterProductModel> GetSalesRegisterProducts(filterOption filter, bool individual = false)
        {
            var productRegister = new List<SalesRegisterProductModel>();
            if (individual == false)
            {
                var userinfo = this._workContext.CurrentUserinformation;
                //var productRegister = _salesRegister.SalesRegisterProducts().ToList();

                if (this._cacheManager.IsSet("SalesRegisterProduct"))
                {
                    productRegister = this._cacheManager.Get<List<SalesRegisterProductModel>>("SalesRegisterProduct");
                }
                else
                {
                    productRegister = _salesRegister.SalesRegisterProducts(userinfo).ToList();
                    this._cacheManager.Set("SalesRegisterProduct", productRegister, 1);
                }
            }
            else
                productRegister = _salesRegister.SalesRegisterProductsIndividual().ToList();
            return productRegister;
        }

        //[HttpGet]
        //public List<SalesRegisterProductModel> GetDistributorItems ()
        //{
        //    var item = _salesRegister.GetDistributorItems(_workContext.CurrentUserinformation);
        //    return item;
        //}

        [HttpGet]
        public List<SalesRegisterProductModel> GetSalesRegisterProductsByCategory(GridFilters filter, string category)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var productRegister = _salesRegister.SalesRegisterProducts().ToList();
            var productRegister = _salesRegister.SalesRegisterProductsByCategory(userinfo, category).ToList();
            return productRegister;
        }

        // *** Supplier 
        [HttpGet]
        public List<SalesRegisterSupplierModel> GetSalesRegisterSupplier(GridFilters filter)
        {
            //var SupplierRegister = _salesRegister.SalesRegisterSuppliers().ToList();
            var userinfo = this._workContext.CurrentUserinformation;
            var SupplierRegister = _salesRegister.SalesRegisterSuppliers(userinfo).ToList();
            return SupplierRegister;
        }
        [HttpGet]
        public List<SalesRegisterSupplierModel> GetSalesRegisterDealer(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var SupplierRegister = _salesRegister.SalesRegisterDealer(userinfo).ToList();
            return SupplierRegister;
        }

        [HttpGet]
        public List<SalesRegisterSupplierModel> GetSalesRegisterGroupSupplier(GridFilters filter)
        {
            //var SupplierRegister = _salesRegister.SalesRegisterGroupSuppliers().ToList();
            var SupplierRegister = _salesRegister.SalesRegisterGroupSuppliers(this._workContext.CurrentUserinformation).ToList();
            return SupplierRegister;
        }

        [HttpGet]
        public List<VoucherModel> GetSalesRegisterVouchers(GridFilters filter)
        {
            var voucherRegister = _salesRegister.SalesRegisterVouchers().ToList();
            return voucherRegister;
        }

        [HttpGet]
        public List<PartyTypeModel> GetSalesRegisterPartyTypes(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var partyTypeRegister = _salesRegister.GetSalesRegisterPartyTypes().ToList();
            var partyTypeRegister = _salesRegister.GetSalesRegisterPartyTypes(userinfo).ToList();
            return partyTypeRegister;
        }

        [HttpGet]
        public List<AreaTypeModel> GetAreaTypes(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var partyTypeRegister = _salesRegister.GetSalesRegisterPartyTypes().ToList();
            var areaTypes = _salesRegister.GetAreaTypes(userinfo).ToList();
            return areaTypes;
        }

        //Branch code start from 
        public List<BranchModel> GetSalesRegisterBranch(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var BranchTypeRegister = _salesRegister.getSalesRegisterBranch().ToList();
            var BranchTypeRegister = _salesRegister.getSalesRegisterBranch(userinfo).ToList();
            return BranchTypeRegister;
        }
        [HttpGet]
        public List<CategoryModel> GetSalesRegisterItemCategories(GridFilter filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var categoryRegister = _salesRegister.GetSalesRegisterItemCategory().ToList();
            var categoryRegister = new List<CategoryModel>();
            if (this._cacheManager.IsSet("SalesRegisterItemCategory"))
            {
                categoryRegister = this._cacheManager.Get<List<CategoryModel>>("SalesRegisterItemCategory");
            }
            else
            {
                categoryRegister = _salesRegister.GetSalesRegisterItemCategory(userinfo).ToList();
                this._cacheManager.Set("SalesRegisterItemCategory", categoryRegister, 1);
            }
            return categoryRegister;
        }

        [HttpGet]
        public List<CustomerTree> GetAllCustomerNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allCustomerList = _salesRegister.CustomerListAllNodes().ToList();
            var allCustomerList = _salesRegister.CustomerListAllNodes(userinfo).ToList();
            var customerNodes = new List<CustomerTree>();

            foreach (var cust in allCustomerList)
            {
                customerNodes.Add(new CustomerTree()
                {
                    Level = cust.LEVEL,
                    customerName = cust.CUSTOMER_EDESC,
                    customerId = cust.CUSTOMER_CODE,
                    masterCustomerCode = cust.MASTER_CUSTOMER_CODE,
                    preCustomerCode = cust.PRE_CUSTOMER_CODE,
                    hasCustomers = cust.GROUP_SKU_FLAG == "G" ? true : false
                });
            }

            return customerNodes;
        }
        [HttpGet]
        public List<ConsolidateTree> GetAllConsolidateNodes(int? userNo = null)
        {
            string uno = null;
            uno = userNo == null ? uno : userNo.Value.ToString();
            var userinfo = this._workContext.CurrentUserinformation;
            //var branchNodes = new List<ConsolidateTree>();
            //if (false)//this._cacheManager.IsSet("consolidateNodes"))
            //{
            //    branchNodes = this._cacheManager.Get<List<ConsolidateTree>>("consolidateNodes");
            //}
            //else
            //{
            //    var allCompanyList = 
            //    foreach (var cust in allCompanyList)
            //    {
            //        branchNodes.Add(new ConsolidateTree()
            //        {

            //            branch_edesc = cust.branch_edesc,
            //            branch_Code = cust.branch_Code,
            //            pre_branch_code = cust.pre_branch_code,
            //            Abbr_Code = cust.Abbr_Code,
            //            hasBranch = true
            //        });
            //    }
            //    this._cacheManager.Set("consolidateNodes", branchNodes, 60);
            //}

            //return branchNodes;
            return _salesRegister.CompanyListAllNodes(userinfo, uno).ToList();

        }
        [HttpGet]
        public List<ConsolidateTree> GetAllConsolidateNodes(string company_code)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var branchNodes = new List<ConsolidateTree>();
            if (false)//this._cacheManager.IsSet("consolidateNodesBy" + company_code))
            {
                branchNodes = this._cacheManager.Get<List<ConsolidateTree>>("consolidateNodesBy" + company_code);
            }
            else
            {
                var allCompanyList = _salesRegister.branchListByCompanyCode(userinfo, company_code).ToList();
                foreach (var cust in allCompanyList)
                {
                    branchNodes.Add(new ConsolidateTree()
                    {

                        branch_edesc = cust.branch_edesc,
                        branch_Code = cust.branch_Code,
                        pre_branch_code = cust.pre_branch_code,
                        Abbr_Code = cust.Abbr_Code,
                        hasBranch = false
                    });
                }
                this._cacheManager.Set("consolidateNodesBy" + company_code, branchNodes, 60);
            }
            return branchNodes;
        }
        [HttpGet]
        public List<CustomerTree> GetAllGroupCustomerNodes()
        {
            var allCustomerList = _salesRegister.CustomerGroupListAllNodes().ToList();
            var customerNodes = new List<CustomerTree>();

            foreach (var cust in allCustomerList)
            {
                customerNodes.Add(new CustomerTree()
                {
                    Level = cust.LEVEL,
                    customerName = cust.CUSTOMER_EDESC,
                    customerId = cust.CUSTOMER_CODE,
                    masterCustomerCode = cust.MASTER_CUSTOMER_CODE,
                    preCustomerCode = cust.PRE_CUSTOMER_CODE,
                    hasCustomers = cust.Childrens > 0 ? true : false
                });
            }

            return customerNodes;
        }
        [HttpGet]
        public List<ProductTree> GetAllProductNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.ProductListAllNodes().ToList();
            var allProductList = _salesRegister.ProductListAllNodes(userinfo).ToList();
            var productNodes = new List<ProductTree>();

            foreach (var prod in allProductList)
            {
                productNodes.Add(new ProductTree()
                {
                    Level = prod.LEVEL,
                    ItemName = prod.ItemName,
                    ItemCode = prod.ItemCode,
                    MasterItemCode = prod.MasterItemCode,
                    PreItemCode = prod.PreItemCode,
                    hasProducts = prod.GroupFlag == "G" ? true : false
                });
            }

            return productNodes;
        }
        [HttpGet]
        public List<ProductTree> GetAllProductsByProdId(string prodId, string level, string masterCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var allProductList = _salesRegister.GetProductsListByProductCode(level, masterCode).ToList();
            var allProductList = _salesRegister.GetProductsListByProductCode(level, masterCode, userinfo).ToList();
            var productNodes = new List<ProductTree>();

            foreach (var prod in allProductList)
            {
                productNodes.Add(new ProductTree()
                {
                    Level = prod.LEVEL,
                    ItemName = prod.ItemName,
                    ItemCode = prod.ItemCode,
                    MasterItemCode = prod.MasterItemCode,
                    PreItemCode = prod.PreItemCode,
                    hasProducts = prod.GroupFlag == "G" ? true : false
                });
            }

            return productNodes;
        }
        [HttpGet]
        public List<CustomerTree> GetAllCustomersByCustId(string custId, string level, string masterCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;

            //var preCustId = _salesRegister.GetPreCustCodeByCustomerId(custId).ToString();
            //var allCustomerList = _salesRegister.GetCustomerListByCustomerCode(level,masterCode).ToList();
            var allCustomerList = _salesRegister.GetCustomerListByCustomerCode(level, masterCode, userinfo).ToList();
            var customerNodes = new List<CustomerTree>();

            foreach (var cust in allCustomerList)
            {
                customerNodes.Add(new CustomerTree()
                {
                    Level = cust.LEVEL,
                    customerName = cust.CUSTOMER_EDESC,
                    customerId = cust.CUSTOMER_CODE,
                    masterCustomerCode = cust.MASTER_CUSTOMER_CODE,
                    preCustomerCode = cust.PRE_CUSTOMER_CODE,
                    hasCustomers = cust.GROUP_SKU_FLAG == "G" ? true : false
                });
            }

            return customerNodes;
        }

        [HttpGet]
        public List<CustomerTree> GetAllGroupCustomersByCustId(string custId, string level, string masterCode)
        {
            //var preCustId = _salesRegister.GetPreCustCodeByCustomerId(custId).ToString();
            var allCustomerList = _salesRegister.GetGroupCustomerListByCustomerCode(level, masterCode).ToList();
            var customerNodes = new List<CustomerTree>();

            foreach (var cust in allCustomerList)
            {
                customerNodes.Add(new CustomerTree()
                {
                    Level = cust.LEVEL,
                    customerName = cust.CUSTOMER_EDESC,
                    customerId = cust.CUSTOMER_CODE,
                    masterCustomerCode = cust.MASTER_CUSTOMER_CODE,
                    preCustomerCode = cust.PRE_CUSTOMER_CODE,
                    hasCustomers = cust.Childrens > 0 ? true : false
                });
            }

            return customerNodes;
        }

        [HttpGet]
        public List<SupplierTree> GetAllSupplierNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var AllSupplierList = _salesRegister.SupplierAllNodes(userinfo).ToList();
            var suplierNodes = new List<SupplierTree>();
            foreach (var sup in AllSupplierList)
            {
                suplierNodes.Add(new SupplierTree()
                {
                    Level = sup.LEVEL,
                    SupplierName = sup.SupplierName,
                    SupplierCode = sup.SupplierCode,
                    MasterSupplierCode = sup.MasterSupplierCode,
                    PreSupplierCode = sup.PreSupplierCode,
                    hasSuppliers = sup.GroupFlag == "G" ? true : false
                });
            }
            return suplierNodes;
        }
        [HttpGet]
        public List<SupplierTree> GetAllDealerNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var AllSupplierList = _salesRegister.DealerAllNodes(userinfo).ToList();
            var suplierNodes = new List<SupplierTree>();
            foreach (var sup in AllSupplierList)
            {
                suplierNodes.Add(new SupplierTree()
                {
                    Level = sup.LEVEL,
                    SupplierName = sup.SupplierName,
                    SupplierCode = sup.SupplierCode,
                    MasterSupplierCode = sup.MasterSupplierCode,
                    PreSupplierCode = sup.PreSupplierCode,
                    hasSuppliers = sup.Childrens > 0 ? true : false

                });
            }
            return suplierNodes;
        }

        [HttpGet]
        public List<SupplierTree> GetAllSupplierNodesGroup()
        {
            //var AllSupplierList = _salesRegister.SupplierAllNodesGroup().ToList();
            var AllSupplierList = _salesRegister.SupplierAllNodesGroup(this._workContext.CurrentUserinformation).ToList();
            var suplierNodes = new List<SupplierTree>();
            foreach (var sup in AllSupplierList)
            {
                suplierNodes.Add(new SupplierTree()
                {
                    Level = sup.LEVEL,
                    SupplierName = sup.SupplierName,
                    SupplierCode = sup.SupplierCode,
                    MasterSupplierCode = sup.MasterSupplierCode,
                    PreSupplierCode = sup.PreSupplierCode,
                    hasSuppliers = sup.Childrens > 0 ? true : false

                });
            }
            return suplierNodes;
        }
        [HttpGet]
        public List<SupplierTree> GetAllSupplierBySupId(string Supid, string level, string mastercode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var AllSupplierList = _salesRegister.GetSupplierListBySupplierCode(level, mastercode, userinfo).ToList();
            var SupplierNodes = new List<SupplierTree>();
            foreach (var sup in AllSupplierList)
            {
                SupplierNodes.Add(new SupplierTree()
                {
                    Level = sup.LEVEL,
                    SupplierName = sup.SupplierName,
                    SupplierCode = sup.SupplierCode,
                    MasterSupplierCode = sup.MasterSupplierCode,
                    PreSupplierCode = sup.PreSupplierCode,
                    hasSuppliers = sup.GroupFlag == "G" ? true : false
                });
            }
            return SupplierNodes;
        }
        [HttpGet]
        public List<SupplierTree> GetAllDealerBySupId(string Supid, string level, string mastercode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var AllSupplierList = _salesRegister.GetDealerListBySupplierCode(level, mastercode, userinfo).ToList();
            var SupplierNodes = new List<SupplierTree>();
            foreach (var sup in AllSupplierList)
            {
                SupplierNodes.Add(new SupplierTree()
                {
                    Level = sup.LEVEL,
                    SupplierName = sup.SupplierName,
                    SupplierCode = sup.SupplierCode,
                    MasterSupplierCode = sup.MasterSupplierCode,
                    PreSupplierCode = sup.PreSupplierCode,
                    hasSuppliers = sup.GroupFlag == "G" ? true : false
                });
            }
            return SupplierNodes;
        }
        [HttpGet]
        public List<SupplierTree> GetAllSupplierBySupIdGroup(string Supid, string mastercode)
        {
            //var AllSupplierList = _salesRegister.GetSupplierListBySupplierCodeGroup(mastercode).ToList();
            var AllSupplierList = _salesRegister.GetSupplierListBySupplierCodeGroup(mastercode, this._workContext.CurrentUserinformation).ToList();
            var SupplierNodes = new List<SupplierTree>();
            foreach (var sup in AllSupplierList)
            {
                SupplierNodes.Add(new SupplierTree()
                {
                    Level = sup.LEVEL,
                    SupplierName = sup.SupplierName,
                    SupplierCode = sup.SupplierCode,
                    MasterSupplierCode = sup.MasterSupplierCode,
                    PreSupplierCode = sup.PreSupplierCode,
                    hasSuppliers = sup.Childrens > 0 ? true : false,
                });
            }
            return SupplierNodes;
        }

        [HttpGet]
        public List<DocumentTree> GetAllVoucherNodes()
        {
            var AllVoucherList = _salesRegister.GetAllVoucherNodes().ToList();
            var voucherNodes = new List<DocumentTree>();
            foreach (var doc in AllVoucherList)
            {
                voucherNodes.Add(new DocumentTree()
                {
                    Level = doc.LEVEL,
                    VoucherName = doc.VoucherName,
                    VoucherCode = doc.VoucherCode,
                    MasterFormCode = doc.MasterFormCode,
                    PreFormCode = doc.PreFormCode,
                    hasChildren = doc.GroupFlag == "G" ? true : false

                });
            }
            return voucherNodes;
        }

        [HttpGet]
        public List<DocumentTree> GetAllVouchersByvoucherId(string voucherId, string level, string mastercode)
        {
            var AllVoucherList = _salesRegister.GetVoucherListByFormCode(level, mastercode).ToList();
            var voucherNodes = new List<DocumentTree>();
            foreach (var doc in AllVoucherList)
            {
                voucherNodes.Add(new DocumentTree()
                {
                    Level = doc.LEVEL,
                    VoucherName = doc.VoucherName,
                    VoucherCode = doc.VoucherCode,
                    MasterFormCode = doc.MasterFormCode,
                    PreFormCode = doc.PreFormCode,
                    hasChildren = doc.GroupFlag == "G" ? true : false

                });
            }
            return voucherNodes;
        }

        public List<ChartSalesModel> GetCategorySales()
        {
            return this._salesRegister.GetCategorySales();
        }
        public List<ChartSalesModel> GetAreaSales()
        {
            return this._salesRegister.GetAreasSales();
        }

        [HttpPost]
        public List<ChartSalesModel> GetCategorySales(filterOption model)
        {
            return this._salesRegister.GetCategorySales(model.ReportFilters, _workContext.CurrentUserinformation);
        }



        //[HttpPost]
        //public List<ChartSalesModel> GetCategorySales(filterOption model, string customerCode, string itemCode, string categoryCode, string companyCode, string branchCode, string partyTypeCode, string formCode)
        //{
        //    return this._salesRegister.GetCategorySales(model.ReportFilters,_workContext.CurrentUserinformation,  customerCode,  itemCode,  categoryCode,  companyCode,  branchCode,  partyTypeCode,  formCode);
        //}
        //[HttpPost]
        //public List<ChartSalesModel> GetProductSalesByCategory(filterOption model,string categoryCode)
        //{
        //    return this._salesRegister.GetProductSalesByCategory(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode);
        //}

        //[HttpPost]
        //public List<ChartSalesModel> GetProductSalesByCategory(filterOption model, string categoryCode,  string customerCode, string itemCode, string categoryCode2, string companyCode, string branchCode, string partyTypeCode, string formCode)
        //{
        //    return this._salesRegister.GetProductSalesByCategory(model.ReportFilters, _workContext.CurrentUserinformation, categoryCode,  customerCode,  itemCode,  categoryCode2,  companyCode,  branchCode,  partyTypeCode,  formCode);
        //}


        [HttpPost]
        public List<CustomerWisePriceListModel> GetCustomerWisePriceList(filterOption model)
        {
            return this._salesRegister.GetCustomerWisePriceList(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpPost]
        public List<ProductWisePriceListModel> GetProductWisePriceList(filterOption model)
        {
            return this._salesRegister.GetProductWisePriceList(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }


        [HttpPost]
        public CustomerWiseProfileAnalysisViewModel GetCustomerWiseProfileAnalysisReport(filterOption model)
        {
            CustomerWiseProfileAnalysisViewModel reportData = new CustomerWiseProfileAnalysisViewModel();
            reportData.CustomerWiseProfileAnalysisModel = this._salesRegister.GetCustomerWiseProfitAnalysis(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.CustomerWiseProfileAnalysisModel.Count();
            reportData.AggregationResult = KendoGridHelper.GetAggregation(reportData.CustomerWiseProfileAnalysisModel);
            return reportData;
        }

        [HttpGet]
        public List<DynamicMenu> GetDynamicMenu(string ModuleCode)
        {
            var menuList = new List<DynamicMenu>();
            try
            {
                var level = 1;
                var userId = _workContext.CurrentUserinformation.User_id;
                if (!ModuleCode.Contains("0"))
                    ModuleCode = ModuleCode.PadLeft(2, '0');

                menuList = _salesRegister.GetDynamicMenu(userId, level, ModuleCode);
                foreach (var item in menuList)
                {
                    var itemList = new List<DynamicMenu>();
                    itemList = _salesRegister.GetChlidMenu(item.MENU_NO, userId, ModuleCode);
                    //item.Items.AddRange(itemList);
                    item.Items = itemList;
                }

            }
            catch (Exception e)
            {
                //throw e;
            }
            return menuList;
        }

        [HttpPost]
        public SalesRegisterDetailGridModel GetSalesRegisterPrivot(filterOption model)
        {
            var reportData = new SalesRegisterDetailGridModel();
            reportData.salesRegisterDetailModel = this._salesRegister.GetSalesRegisterModelPrivot(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
            reportData.total = reportData.salesRegisterDetailModel.Count();
            return reportData;
        }

        [HttpPost]
        public List<DailySalesTreeList> GetSalesRegisterDailyReport(filterOption model)
        {

            return this._salesRegister.GetSalesRegisterDailyReport(model.ReportFilters, _workContext.CurrentUserinformation).ToList();
        }

        [HttpGet]
        public List<DynamicColumnForNCR> GetDynamicColumnsForNCR()
        {
            return this._salesRegister.GetDynamiColumns();
        }


        [HttpPost]
        public GoodsReceiptNotesGridModel GoodsReceiptNotesReport(filterOption model, bool liveData = false)
        {

            var reportData = new GoodsReceiptNotesGridModel();
            reportData.GoodsReceiptNotesDetailModel = this._salesRegister.GetGoodsReceiptNotesData(model.ReportFilters, _workContext.CurrentUserinformation, liveData).ToList();
            reportData.total = reportData.GoodsReceiptNotesDetailModel.Count();
            return reportData;
        }


        public string GetJsonReportLastUpdatedDate(string reportName)
        {
            string file = System.Web.HttpContext.Current.Server.MapPath("~/App_Files/json/" + reportName + ".json");
            var message = System.IO.File.GetLastWriteTime(file).ToString("yyyy-MMM-dd hh:mm tt");
            return message;
        }

        [HttpPost]
        public MaterializeViewModel GetMaterializeReport(filterOption model, bool SynData = false)
        {
            User userInfo = this._workContext.CurrentUserinformation;
            var materializeModel = new MaterializeViewModel();
            var materializeReport = _salesRegister.GetMaterializeReprot(model.ReportFilters, userInfo, SynData);
            materializeModel.RegisterDetails = materializeReport.ToList();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<MaterializeModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                    //whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<MaterializeModel>(i, model.filter.Logic, filters[i], parameters));
                    {
                        var checkValue = KendoGridHelper.BuildWhereClause<MaterializeModel>(i, model.filter.Logic, filters[i], parameters);
                        if (checkValue != "" & checkValue != null)
                            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                    }
                }
                // Where
                materializeModel.RegisterDetails = materializeReport.Where(whereClause, parameters.ToArray()).ToList();
                materializeModel.total = materializeModel.RegisterDetails.Count;
            }
            else
            {
                materializeModel.total = materializeReport.Count;

            }
            //materializeModel.total = materializeModel.RegisterDetails.Count;
            //materializeModel.total = materializeReport.Count;

            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    materializeModel.RegisterDetails = materializeModel.RegisterDetails.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            materializeModel.AggregationResult = KendoGridHelper.GetAggregation(materializeModel.RegisterDetails, model.aggregate);
            materializeModel.RegisterDetails = materializeModel.RegisterDetails.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            return materializeModel;
        }

        [HttpPost]
        public string SynMaterialize(filterOption model)
        {
            User userInfo = this._workContext.CurrentUserinformation;
            var materializeModel = new MaterializeViewModel();
            var materializeReport = _salesRegister.GetMaterializeReprot(model.ReportFilters, userInfo);
            return "mESSAGE";
        }
        [HttpPost]
        public MaterializedViewDetailReport GetMaterilizedViewReport(filterOption model)
        {
            var gridViewSalesModel = new MaterializedViewDetailReport();//MaterializedViewMasterModel
            var salesRegistrations = _salesRegister.MaterializedViewReport(model.ReportFilters).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }
            else
            {
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }
            var aggregationDictionary = KendoGridHelper.GetAggregation(gridViewSalesModel.gridSalesRegReport, model.aggregate);


            foreach (var aggkey in model.aggregate)
            {
                if (aggregationDictionary.Any(x => x.Key == aggkey.field))
                    continue;
                aggregationDictionary.Add(aggkey.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            foreach (var item in model.aggregate)
            {
                if (aggregationDictionary.ContainsKey(item.field))
                    continue;

                aggregationDictionary.Add(item.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            gridViewSalesModel.AggregationResult = aggregationDictionary;
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(x => x.BILL_DATE).ThenByDescending(x => x.BILL_NO).ToList();

            gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.Skip((model.page - 1) * model.pageSize)
                     .Take(model.pageSize).ToList();


            return gridViewSalesModel;
        }
        [HttpPost]
        public VatRegistrationIRDDetailReport GetVatRegistrationIRDReport(filterOption model)
        {
            var gridViewSalesModel = new VatRegistrationIRDDetailReport();//MaterializedViewMasterModel
            var salesRegistrations = _salesRegister.VatRegisterIRDReport(model.ReportFilters).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegisterMasterModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }
            else
            {
                gridViewSalesModel.gridSalesRegReport = salesRegistrations.ToList();
                gridViewSalesModel.total = gridViewSalesModel.gridSalesRegReport.Count;
            }
            var aggregationDictionary = KendoGridHelper.GetAggregation(gridViewSalesModel.gridSalesRegReport, model.aggregate);


            foreach (var aggkey in model.aggregate)
            {
                if (aggregationDictionary.Any(x => x.Key == aggkey.field))
                    continue;
                aggregationDictionary.Add(aggkey.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            foreach (var item in model.aggregate)
            {
                if (aggregationDictionary.ContainsKey(item.field))
                    continue;

                aggregationDictionary.Add(item.field, new AggregationModel()
                {
                    average = 0,
                    max = 0,
                    min = 0,
                    sum = 0
                });
            }

            gridViewSalesModel.AggregationResult = aggregationDictionary;
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.OrderBy(x => x.MITI).ThenByDescending(x => x.INVOICE_NO).ToList();

            gridViewSalesModel.gridSalesRegReport = gridViewSalesModel.gridSalesRegReport.Skip((model.page - 1) * model.pageSize)
                     .Take(model.pageSize).ToList();


            return gridViewSalesModel;
        }
        [HttpPost]
        public PurchaseReturnRegisterViewModel GetPurchaseReturnRegistersReport(filterOption model)
        {

            var salesRegistersViewModel = new PurchaseReturnRegisterViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetPurchaseReturnRegister(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
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
                salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderByDescending(x => x.RETURNDATE).ThenByDescending(x => x.INVOICENUMBER).ToList();
            return salesRegistersViewModel;
        }
        [HttpPost]
        public SalesRegisterViewModel GetAgentWiseSalesRegistersReport(filterOption model)
        {

            var salesRegistersViewModel = new SalesRegisterViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetAgentWiseSalesRegister(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
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
        [HttpPost]
        public PurchaseReturnRegisterViewModel GetPurchaseRegistersReport(filterOption model)
        {

            var salesRegistersViewModel = new PurchaseReturnRegisterViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetPurchaseRegister(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<SalesRegistersDetail>(i, model.filter.Logic, filters[i], parameters));
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
                salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderByDescending(x => x.RETURNDATE).ThenByDescending(x => x.INVOICENUMBER).ToList();
            return salesRegistersViewModel;
        }
        [HttpPost]
        public PurchasePendingViewModel GetPurchasePendingReport(filterOption model)
        {

            var salesRegistersViewModel = new PurchasePendingViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetPurchasePendingReport(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchasePendingDetailModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchasePendingDetailModel>(i, model.filter.Logic, filters[i], parameters));
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
                salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderByDescending(x => x.ORDER_DATE).ThenByDescending(x => x.ORDER_NO).ToList();
            return salesRegistersViewModel;
        }
        //GetPurchaseOrderReport
        [HttpPost]
        public PurchasePendingViewModel GetPurchaseOrderReport(filterOption model)
        {
            var salesRegistersViewModel = new PurchasePendingViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetPurchaseOrderReport(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchasePendingDetailModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchasePendingDetailModel>(i, model.filter.Logic, filters[i], parameters));
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
                salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderByDescending(x => x.ORDER_DATE).ThenByDescending(x => x.ORDER_NO).ToList();
            return salesRegistersViewModel;
        }
        [HttpPost]
        public PurchaseVatRegistrationViewModel GetPurchaseVatRegisterReport(filterOption model)
        {
            var salesRegistersViewModel = new PurchaseVatRegistrationViewModel();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _salesRegister.GetPurchaseVatRegisterReport(model.ReportFilters, userinfo).AsQueryable();
            //  var salesRegister = _salesRegister.GetSalesRegisterDateWisePaging(formDate, toDate, model.pageSize, model.page);
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchasePendingDetailModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchasePendingDetailModel>(i, model.filter.Logic, filters[i], parameters));
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
                salesRegistersViewModel.SalesRegisters = salesRegistersViewModel.SalesRegisters.OrderByDescending(x => x.INVOICE_DATE).ThenByDescending(x => x.INVOICE_NO).ToList();
            return salesRegistersViewModel;
        }

        ////Productwise Net Purchase Report 
        [HttpPost]
        public ProductwiseNetPurchaseModel GetProductwiseNetPurchaseReport(filterOption model)
        {
            var viewModel = new ProductwiseNetPurchaseModel();

            var dateSales = _salesRegister.GetProductwiseNetPurchaseReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<ProductwiseNetPurchaseViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.ProductwiseNetPurchase = filteredSales.ToList();

            return viewModel;
        }

        //Audit transaction report 
        [HttpPost]
        public AuditTransactionViewModel GetAuditTrasactionReport(filterOption model)
        {
            var viewModel = new AuditTransactionViewModel();

            var dateSales = _salesRegister.GetAuditTrasactionReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<AuditTransactionModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.AuditTransaction = filteredSales.ToList();

            return viewModel;
        }
        ////Datewise Net Purchase Report 
        [HttpPost]
        public ProductwiseNetPurchaseModel GetDatewiseNetPurchaseReport(filterOption model)
        {
            var viewModel = new ProductwiseNetPurchaseModel();

            var dateSales = _salesRegister.GetDatewiseNetPurchaseReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<ProductwiseNetPurchaseViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.ProductwiseNetPurchase = filteredSales.ToList();

            return viewModel;
        }

        //Sales Above OneLakh Report 
        [HttpPost]
        public SalesAboveOneLakhViewModel GetPurchaseAboveOneLakh(filterOption model)
        {
            var viewModel = new SalesAboveOneLakhViewModel();

            var Sales = _salesRegister.GetPurchaseAboveOneLakh(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = Sales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<SalesAboveOneLakhModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.SalesAboveOneLakh = filteredSales.ToList();

            return viewModel;
        }
        //Supplierwise Net Purchase Report
        [HttpPost]
        public dynamic GetSupplierwiseNetPurchaseReport([FromBody] dynamic data)
        {
            var branchWise = this._salesRegister.GetSupplierwiseNetPurchaseReport(data, _workContext.CurrentUserinformation);

            return branchWise;
        }
        //Datewise Purchase Detail  Report 
        [HttpPost]
        public DatewisePurchaseDetailModel GetDatewisePurchaseDetailReport(filterOption model)
        {
            var viewModel = new DatewisePurchaseDetailModel();

            var dateSales = _salesRegister.GetDatewisePurchaseDetailReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DatewisePurchaseDetailViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWisePurchase = filteredSales.ToList();

            return viewModel;
        }


        //Billwise Purchase Detail  Report 
        [HttpPost]
        public DatewisePurchaseDetailModel GetBillwisePurchaseSummaryReport(filterOption model)
        {
            var viewModel = new DatewisePurchaseDetailModel();

            var dateSales = _salesRegister.GetBillwisePurchaseSummaryReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DatewisePurchaseDetailViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWisePurchase = filteredSales.ToList();

            return viewModel;
        }



        //Productwise Purchase summary  Report 
        [HttpPost]
        public DatewisePurchaseDetailModel GetProductwisePurchaseSummaryReport(filterOption model)
        {
            var viewModel = new DatewisePurchaseDetailModel();

            var dateSales = _salesRegister.GetProductwisePurchaseSummaryReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DatewisePurchaseDetailViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWisePurchase = filteredSales.ToList();

            return viewModel;
        }

        //Billwise Purchase Return summary  Report 
        [HttpPost]
        public DatewisePurchaseDetailModel GetBillwisePurchaseReturnSummaryReport(filterOption model)
        {
            var viewModel = new DatewisePurchaseDetailModel();

            var dateSales = _salesRegister.GetBillwisePurchaseReturnSummaryReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DatewisePurchaseDetailViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWisePurchase = filteredSales.ToList();

            return viewModel;
        }
        //Productwise Purchase summary  Report 
        [HttpPost]
        public DatewisePurchaseDetailModel GetPurchaseLandedCostSummaryReport(filterOption model)
        {
            var viewModel = new DatewisePurchaseDetailModel();

            var dateSales = _salesRegister.GetPurchaseLandedCostSummaryReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = dateSales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<DatewisePurchaseDetailViewModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.DateWisePurchase = filteredSales.ToList();

            return viewModel;
        }
        [HttpGet]
        public HttpResponseMessage GetFormCodeByCompany(filterOption model)
        {
            if (model == null)
            {
                model = new filterOption();
            }
            var filters = model.ReportFilters ?? new ReportFiltersModel();
            if (_salesRegister == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Sales Register service not initialized.");
            }
            var userInfo = _workContext.CurrentUserinformation;
            if (userInfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User context information is missing.");
            }
            var data = _salesRegister.GetFormCodeByCompany(filters, userInfo);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });
        }
        // Puirchase Report Above one lakh 
        [HttpPost]
        public SalesAboveOneLakhViewModel PurchaseAboveOneLakhReport(filterOption model)
        {
            var viewModel = new SalesAboveOneLakhViewModel();

            var Sales = _salesRegister.PurchaseAboveOneLakhReport(model.ReportFilters, _workContext.CurrentUserinformation);
            var filteredSales = Sales.AsQueryable();

            // Apply filtering
            if (model.filter?.Filters?.Count > 0)
            {
                var parameters = new System.Collections.ArrayList();
                var whereClauses = new List<string>();

                for (int i = 0; i < model.filter.Filters.Count; i++)
                {
                    var clause = KendoGridHelper.BuildWhereClause<SalesAboveOneLakhModel>(i, model.filter.Logic, model.filter.Filters[i], parameters);
                    if (!string.IsNullOrEmpty(clause))
                    {
                        whereClauses.Add(clause);
                    }
                }

                var fullWhereClause = string.Join($" {KendoGridHelper.ToLinqOperator(model.filter.Logic)} ", whereClauses);
                filteredSales = filteredSales.Where(fullWhereClause, parameters.ToArray());
            }

            // Update total count after filtering
            viewModel.Total = filteredSales.Count();

            // Apply sorting
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    filteredSales = filteredSales.OrderBy($"{s.Field} {s.Dir}");
                }
            }

            // Get aggregation before pagination
            viewModel.AggregationResult = KendoGridHelper.GetAggregation(filteredSales.ToList(), model.aggregate);

            // Apply pagination
            filteredSales = filteredSales
                .Skip((model.page - 1) * model.pageSize)
                .Take(model.pageSize);

            viewModel.SalesAboveOneLakh = filteredSales.ToList();

            return viewModel;
        }

        [HttpPost]
        public dynamic GetGroupstockSummaryReport([FromBody] dynamic data)
        {
            var groupWise = _salesRegister.GetGroupstockSummaryReport(data, _workContext.CurrentUserinformation);

            return groupWise;
        }

        #region NewReports
        [HttpPost]
        public SalesExciseRegisterViewModel SalesExciseRegister(filterOption model)
        {
            var salesExciseViewModel = new SalesExciseRegisterViewModel();

            var SalesExciseRegister = _salesRegister.SalesExciseRegister(model.ReportFilters, _workContext.CurrentUserinformation);
            salesExciseViewModel.salesExciseRegisterModel = SalesExciseRegister.ToList();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    SalesExciseRegisterModel salesExciseRegisterModel = new SalesExciseRegisterModel();
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<SalesExciseRegisterModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                    {
                        var checkValue = KendoGridHelper.BuildWhereClause<SalesExciseRegisterModel>(i, model.filter.Logic, filters[i], parameters);
                        if (checkValue != "" & checkValue != null)
                            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                    }

                }
                salesExciseViewModel.salesExciseRegisterModel = SalesExciseRegister.Where(whereClause, parameters.ToArray()).ToList();
            }
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    salesExciseViewModel.salesExciseRegisterModel = salesExciseViewModel.salesExciseRegisterModel.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            salesExciseViewModel.total = salesExciseViewModel.salesExciseRegisterModel.Count;
            salesExciseViewModel.AggregationResult = KendoGridHelper.GetAggregation(salesExciseViewModel.salesExciseRegisterModel, model.aggregate);
            salesExciseViewModel.salesExciseRegisterModel = salesExciseViewModel.salesExciseRegisterModel.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();

            return salesExciseViewModel;
        }
        [HttpPost]
        public AuditTrailViewModel AuditTrailReport(filterOption model)
        {
            var auditTrailViewModel = new AuditTrailViewModel();

            var AuditTrailReport = _salesRegister.AuditTrailReport(model.ReportFilters, _workContext.CurrentUserinformation);
            auditTrailViewModel.auditTrialReportModel = AuditTrailReport.ToList();
            //if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            //{
            //    string whereClause = null;
            //    var parameters = new System.Collections.ArrayList();
            //    var filters = model.filter.Filters;
            //    for (var i = 0; i < filters.Count; i++)
            //    {
            //        if (i == 0)
            //            whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters));
            //        else
            //            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<VatRegisterModel>(i, model.filter.Logic, filters[i], parameters));
            //    }
            //    salesExciseViewModel.salesExciseRegisterModel = SalesExciseRegister.Where(whereClause, parameters.ToArray()).ToList();
            //}
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    auditTrailViewModel.auditTrialReportModel = auditTrailViewModel.auditTrialReportModel.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            auditTrailViewModel.total = auditTrailViewModel.auditTrialReportModel.Count;
            auditTrailViewModel.AggregationResult = KendoGridHelper.GetAggregation(auditTrailViewModel.auditTrialReportModel, model.aggregate);
            auditTrailViewModel.auditTrialReportModel = auditTrailViewModel.auditTrialReportModel.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();

            return auditTrailViewModel;
        }
        [HttpGet]
        public HttpResponseMessage GetAllItemByGroup(filterOption model)
        {
            if (model == null)
            {
                model = new filterOption();
            }
            var filters = model.ReportFilters ?? new ReportFiltersModel();
            if (_salesRegister == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Sales Register service not initialized.");
            }
            var userInfo = _workContext.CurrentUserinformation;
            if (userInfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User context information is missing.");
            }
            var data = _salesRegister.GetAllItemByGroup(filters, userInfo);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });
        }

        [HttpGet]
        public HttpResponseMessage GetAllLocation(filterOption model)
        {
            if (model == null)
            {
                model = new filterOption();
            }
            var filters = model.ReportFilters ?? new ReportFiltersModel();
            if (_salesRegister == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Sales Register service not initialized.");
            }
            var userInfo = _workContext.CurrentUserinformation;
            if (userInfo == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "User context information is missing.");
            }
            var data = _salesRegister.GetAllLocation(filters, userInfo);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });
        }



        #endregion
    }
}

