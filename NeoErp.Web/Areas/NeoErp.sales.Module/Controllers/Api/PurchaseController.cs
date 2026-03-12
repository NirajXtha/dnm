using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core;
using NeoErp.Core.Domain;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class PurchaseController : ApiController
    {
        public IPurchaseService _purchaseRegister { get; set; }
        private IWorkContext _workContext;
        public PurchaseController(IPurchaseService purchaseRegister, IWorkContext workContext)
        {
            this._purchaseRegister = purchaseRegister;
            this._workContext = workContext;
        }
        [HttpPost]
        public PurchaseRegisterViewModel GetPurchaseRegister(filterOption model, string formDate, string toDate)
        {
            User userInfo = this._workContext.CurrentUserinformation;
            var purchaseRegisterViewModel = new PurchaseRegisterViewModel();
            var purchaseRegistrations = _purchaseRegister.GetPurchaseRegister(formDate, toDate, userInfo);
            purchaseRegisterViewModel.RegisterDetails = purchaseRegistrations.ToList();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchaseRegisterDetail>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchaseRegisterDetail>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                purchaseRegisterViewModel.RegisterDetails = purchaseRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                purchaseRegisterViewModel.total = purchaseRegisterViewModel.RegisterDetails.Count;
            }
            else
            {
                purchaseRegisterViewModel.total = purchaseRegistrations.Count;

            }
            purchaseRegisterViewModel.AggregationResult = KendoGridHelper.GetAggregation(purchaseRegisterViewModel.RegisterDetails);
            purchaseRegisterViewModel.RegisterDetails = purchaseRegisterViewModel.RegisterDetails.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    purchaseRegisterViewModel.RegisterDetails = purchaseRegisterViewModel.RegisterDetails.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }


            return purchaseRegisterViewModel;
        }


        [HttpPost]
        public PurchaseRegisterViewModel GetPurchaseRegister(filterOption model)
        {
            User userInfo = this._workContext.CurrentUserinformation;
            var purchaseRegisterViewModel = new PurchaseRegisterViewModel();
            var purchaseRegistrations = _purchaseRegister.GetPurchaseRegister(model.ReportFilters, userInfo);
            purchaseRegisterViewModel.RegisterDetails = purchaseRegistrations.ToList();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchaseRegisterDetail>(i, model.filter.Logic, filters[i], parameters));
                    else
                    //whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchaseRegisterDetail>(i, model.filter.Logic, filters[i], parameters));
                    {
                        var checkValue = KendoGridHelper.BuildWhereClause<PurchaseRegisterDetail>(i, model.filter.Logic, filters[i], parameters);
                        if (checkValue != "" & checkValue != null)
                            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                    }
                }
                // Where
                purchaseRegisterViewModel.RegisterDetails = purchaseRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                purchaseRegisterViewModel.total = purchaseRegisterViewModel.RegisterDetails.Count;
            }
            else
            {
                purchaseRegisterViewModel.total = purchaseRegistrations.Count;

            }
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    purchaseRegisterViewModel.RegisterDetails = purchaseRegisterViewModel.RegisterDetails.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            purchaseRegisterViewModel.AggregationResult = KendoGridHelper.GetAggregation(purchaseRegisterViewModel.RegisterDetails, model.aggregate);
            purchaseRegisterViewModel.RegisterDetails = purchaseRegisterViewModel.RegisterDetails.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();



            return purchaseRegisterViewModel;
        }
        [HttpPost]
        public PurchaseVatRegistersViewModel GetPurchaseVatRegister(filterOption model, string formDate, string toDate)
        {
            var userInfo = this._workContext.CurrentUserinformation;
            var purchaseRegisterViewModel = new PurchaseVatRegistersViewModel();
            var purchaseRegistrations = _purchaseRegister.GetPurchaseVatRegisters(formDate, toDate, userInfo);

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchaseVatRegisters>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchaseVatRegisters>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                purchaseRegistrations = purchaseRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                purchaseRegisterViewModel.total = purchaseRegistrations.Count;
            }
            else
            {
                purchaseRegisterViewModel.total = purchaseRegistrations.Count;

            }

            var purchaseVatRegisters = new List<PurchaseVatRegistersView>();
            foreach (var purchaseVat in purchaseRegistrations)
            {
                var purchaseReg = new PurchaseVatRegistersView();
                purchaseReg.InvoiceDate = purchaseVat.InvoiceDate;
                purchaseReg.Miti = purchaseVat.Miti;
                purchaseReg.InvoiceNo = purchaseVat.InvoiceNo;
                purchaseReg.ManualNo = purchaseVat.ManualNo;
                purchaseReg.PartyName = purchaseVat.PartyName;
                purchaseReg.VatNo = purchaseVat.VatNo;
                purchaseReg.PurchaseTaxExemp = 0;
                if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "LOC".ToLowerInvariant().Trim())
                {
                    purchaseReg.localTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.localVatAmount = purchaseVat.VatAmount;
                    purchaseReg.TotalPurchaseLocalAmount = purchaseReg.localTaxAmount;
                    purchaseReg.TotalVatLocalAmount = purchaseReg.localVatAmount;
                }
                else if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "IMP".ToLowerInvariant().Trim())
                {
                    purchaseReg.ImportTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.ImportVatAmount = purchaseVat.VatAmount;
                    purchaseReg.TotalPurchaseImport = purchaseReg.ImportTaxAmount;
                    purchaseReg.TotalVatImportAmount = purchaseReg.ImportVatAmount;
                }
                else if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "ADM".ToLowerInvariant().Trim())
                {
                    purchaseReg.AdminTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.AdminVatAmount = purchaseVat.VatAmount;

                }
                else if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "CPE".ToLowerInvariant().Trim())
                {
                    purchaseReg.CaptialTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.CapitalVatAmount = purchaseVat.VatAmount;

                }
                purchaseVatRegisters.Add(purchaseReg);
            }

            purchaseRegisterViewModel.AggregationResult = KendoGridHelper.GetAggregation(purchaseVatRegisters, model.aggregate);
            var purchaseVatsPaging = purchaseVatRegisters.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();


            purchaseRegisterViewModel.VatRegisters = purchaseVatsPaging;
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    purchaseRegisterViewModel.VatRegisters = purchaseRegisterViewModel.VatRegisters.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            return purchaseRegisterViewModel;
        }

        [HttpPost]
        public PurchaseVatRegistersViewModel GetPurchaseVatRegister(filterOption model)
        {
            var userInfo = this._workContext.CurrentUserinformation;
            var purchaseRegisterViewModel = new PurchaseVatRegistersViewModel();
            var purchaseRegistrations = _purchaseRegister.GetPurchaseVatRegisters(model.ReportFilters, userInfo);

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<PurchaseVatRegisters>(i, model.filter.Logic, filters[i], parameters));
                    else
                    //whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<PurchaseVatRegisters>(i, model.filter.Logic, filters[i], parameters));
                    {
                        if (model.filter.Filters[i].Field != "PurchaseTaxExemp")
                        {
                            var checkValue = KendoGridHelper.BuildWhereClause<PurchaseVatRegisters>(i, model.filter.Logic, filters[i], parameters);
                            if (checkValue != "" & checkValue != null)
                                whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), checkValue);
                        }
                    }
                }
                // Where
                purchaseRegistrations = purchaseRegistrations.Where(whereClause, parameters.ToArray()).ToList();
                purchaseRegisterViewModel.total = purchaseRegistrations.Count;
            }
            else
            {
                purchaseRegisterViewModel.total = purchaseRegistrations.Count;

            }
            var purchaseVatRegisters = new List<PurchaseVatRegistersView>();
            foreach (var purchaseVat in purchaseRegistrations)
            {
                var purchaseReg = new PurchaseVatRegistersView();
                purchaseReg.InvoiceDate = purchaseVat.InvoiceDate;
                purchaseReg.Miti = purchaseVat.Miti;
                purchaseReg.InvoiceNo = purchaseVat.InvoiceNo;
                purchaseReg.ManualNo = purchaseVat.ManualNo;
                purchaseReg.PartyName = purchaseVat.PartyName;
                purchaseReg.VatNo = purchaseVat.VatNo;
                purchaseReg.PurchaseTaxExemp = 0;

                if (purchaseVat.Ptype == null || purchaseVat.Ptype.ToLowerInvariant().Trim() == "LOC".ToLowerInvariant().Trim())
                {
                    purchaseReg.localTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.localVatAmount = purchaseVat.VatAmount;
                    purchaseReg.TotalPurchaseLocalAmount = purchaseReg.localTaxAmount;
                    purchaseReg.TotalVatLocalAmount = purchaseReg.localVatAmount;
                }
                else if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "IMP".ToLowerInvariant().Trim())
                {
                    purchaseReg.ImportTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.ImportVatAmount = purchaseVat.VatAmount;
                    purchaseReg.TotalPurchaseImport = purchaseReg.ImportTaxAmount;
                    purchaseReg.TotalVatImportAmount = purchaseReg.ImportVatAmount;
                }
                else if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "ADM".ToLowerInvariant().Trim())
                {
                    purchaseReg.AdminTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.AdminVatAmount = purchaseVat.VatAmount;

                }
                else if (purchaseVat.Ptype.ToLowerInvariant().Trim() == "CPE".ToLowerInvariant().Trim())
                {
                    purchaseReg.CaptialTaxAmount = purchaseVat.TaxablePurchase;
                    purchaseReg.CapitalVatAmount = purchaseVat.VatAmount;

                }

                purchaseVatRegisters.Add(purchaseReg);
            }
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    purchaseRegisterViewModel.VatRegisters = purchaseRegisterViewModel.VatRegisters.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            else
                purchaseRegisterViewModel.VatRegisters = purchaseRegisterViewModel.VatRegisters.OrderByDescending(x => x.InvoiceDate).ThenByDescending(x => x.InvoiceNo).ToList();

            purchaseRegisterViewModel.AggregationResult = KendoGridHelper.GetAggregation(purchaseVatRegisters);
            var purchaseVatsPaging = purchaseVatRegisters.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();


            purchaseRegisterViewModel.VatRegisters = purchaseVatsPaging;

            return purchaseRegisterViewModel;
        }


        [HttpPost]
        public PurchaseItemsLandingReportModel GetPurchaseLandingCost(filterOption model, string formDate, string toDate)
        {
            var purchaseItemsModel = new PurchaseItemsLandingReportModel();
            var itemsDetail = _purchaseRegister.GetPurchaseItemsSummary(formDate, toDate);

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<ItemsLandingCostViewModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<ItemsLandingCostViewModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                purchaseItemsModel.items = itemsDetail.Where(whereClause, parameters.ToArray()).ToList();
                purchaseItemsModel.total = purchaseItemsModel.items.Count;
            }
            else
            {
                purchaseItemsModel.items = itemsDetail.ToList();
                purchaseItemsModel.total = itemsDetail.Count;

            }
            var purchaseVatsPaging = purchaseItemsModel.items.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    purchaseItemsModel.items = purchaseItemsModel.items.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            var allCharges = _purchaseRegister.GetPurchaseCharges(formDate, toDate);
            foreach (var a in purchaseItemsModel.items)
            {
                //a.RatePerUnit = a.GrossAmount / a.Quantity;
                if (purchaseItemsModel.items.Any(x => x.charges.Any(y => y.ITEM_CODE == a.ItemCode)))
                    continue;
                var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ItemCode).ToList();
                var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A").Sum(x => x.CHARGE_AMOUNT);
                a.NetAmount = (a.GrossAmount - sumOfSubtractCharges) + sumOfAddCharges;
                a.AvgLandingCost = a.NetAmount / a.Quantity;
                a.TotalInCharges = sumOfAddCharges - sumOfSubtractCharges;
                a.charges = itemCharges;

            }
            return purchaseItemsModel;
        }

        [HttpPost]
        public PurchaseItemsLandingReportModel GetPurchaseLandingCost(filterOption model)
        {
            User userinfo = this._workContext.CurrentUserinformation;
            var purchaseItemsModel = new PurchaseItemsLandingReportModel();
            var itemsDetail = _purchaseRegister.GetPurchaseItemsSummary(model.ReportFilters, userinfo);

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<ItemsLandingCostViewModel>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<ItemsLandingCostViewModel>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                purchaseItemsModel.items = itemsDetail.Where(whereClause, parameters.ToArray()).ToList();
                purchaseItemsModel.total = purchaseItemsModel.items.Count;
            }
            else
            {
                purchaseItemsModel.items = itemsDetail.ToList();
                purchaseItemsModel.total = itemsDetail.Count;

            }

            var allCharges = _purchaseRegister.GetPurchaseCharges(model.ReportFilters, _workContext.CurrentUserinformation);
            #region Original Code
            //foreach (var a in purchaseItemsModel.items)
            //{
            //    if (purchaseItemsModel.items.Any(x => x.charges.Any(y => y.ITEM_CODE == a.ItemCode)))
            //        continue;
            //    var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ItemCode).ToList();
            //    var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
            //    var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A").Sum(x => x.CHARGE_AMOUNT);
            //    a.NetAmount = (a.GrossAmount - sumOfSubtractCharges) + sumOfAddCharges;
            //    a.AvgLandingCost = a.NetAmount / a.Quantity;
            //    a.TotalInCharges = sumOfAddCharges - sumOfSubtractCharges;
            //    a.charges = itemCharges;
            //}
            #endregion

            #region EditedCode

            foreach (var a in purchaseItemsModel.items)
            {
                if (purchaseItemsModel.items.Any(x => x.charges.Any(y => y.ITEM_CODE == a.ItemCode)))
                {
                    a.NetAmount = a.GrossAmount;
                    a.TotalInCharges = a.TotalInCharges.Equals(DBNull.Value) ? 0 : Convert.ToInt32(a.TotalInCharges);
                    a.AvgLandingCost = a.AvgLandingCost.Equals(DBNull.Value) ? 0 : Convert.ToInt32(a.AvgLandingCost);
                }
                else
                {
                    var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ItemCode).ToList();
                    var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                    var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A").Sum(x => x.CHARGE_AMOUNT);
                    a.NetAmount = (a.GrossAmount - sumOfSubtractCharges) + sumOfAddCharges;
                    a.AvgLandingCost = a.NetAmount / a.Quantity;
                    a.TotalInCharges = sumOfAddCharges - sumOfSubtractCharges;
                    a.charges = itemCharges;
                }
            }

            #endregion
            purchaseItemsModel.AggregationResult = KendoGridHelper.GetAggregation(purchaseItemsModel.items, model.aggregate);


            if (purchaseItemsModel.items.Count() > 0)
            {
                var groupData = allCharges.GroupBy(q => q.CHARGE_CODE).Select(group => new
                {
                    Code = group.Key,
                    Sum = group.Sum(q => q.CHARGE_AMOUNT),
                    Min = group.Min(q => q.CHARGE_AMOUNT),
                    Max = group.Max(q => q.CHARGE_AMOUNT)
                });


                foreach (var item in groupData)
                {
                    purchaseItemsModel.AggregationResult.Add(item.Code, new AggregationModel()
                    {
                        max = item.Max ?? 0,
                        min = item.Min ?? 0,
                        sum = item.Sum ?? 0
                    });
                }
            }

            foreach (var item in model.aggregate)
            {
                if (purchaseItemsModel.AggregationResult.ContainsKey(item.field))
                    continue;

                purchaseItemsModel.AggregationResult.Add(item.field, new AggregationModel()
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
                    purchaseItemsModel.items = purchaseItemsModel.items.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            var purchaseVatsPaging = purchaseItemsModel.items.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();


            return purchaseItemsModel;
        }
        [HttpPost]
        public PurchaseInvoiceLandingReportModel GetPurchaseInvoiceLandingCost(filterOption model, string formDate, string toDate)
        {
            var purchaseItemsModel = new PurchaseInvoiceLandingReportModel();
            var itemsDetail = _purchaseRegister.GetPurchaseInvoiceSummary(formDate, toDate);

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<InvoiceLandedCostSummary>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<InvoiceLandedCostSummary>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                purchaseItemsModel.items = itemsDetail.Where(whereClause, parameters.ToArray()).ToList();
                purchaseItemsModel.total = purchaseItemsModel.items.Count;
            }
            else
            {
                purchaseItemsModel.items = itemsDetail.ToList();
                purchaseItemsModel.total = itemsDetail.Count;

            }
            var purchaseVatsPaging = purchaseItemsModel.items.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();
            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    purchaseItemsModel.items = purchaseItemsModel.items.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            var allCharges = _purchaseRegister.GetPurchaseInvoiceCharges(formDate, toDate);
            foreach (var a in purchaseItemsModel.items)
            {

                if (purchaseItemsModel.items.Any(x => x.charges.Any(y => y.ITEM_CODE == a.ItemCode && y.InvoiceNo == a.InvoiceNo)))
                    continue;
                var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ItemCode && x.InvoiceNo == a.InvoiceNo).ToList();
                var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE != "VT").Sum(x => x.CHARGE_AMOUNT);
                a.TotalCharges = sumOfAddCharges - sumOfSubtractCharges;
                a.NetAmount = (a.TotalPrice + sumOfAddCharges) - sumOfSubtractCharges;
                a.AvgLandedCost = a.NetAmount / a.Quantity;
                a.charges = itemCharges;

            }
            return purchaseItemsModel;
        }

        [HttpPost]
        public PurchaseInvoiceLandingReportModel GetPurchaseInvoiceLandingCost(filterOption model)
        {
            User userinfo = this._workContext.CurrentUserinformation;
            var purchaseItemsModel = new PurchaseInvoiceLandingReportModel();
            var itemsDetail = _purchaseRegister.GetPurchaseInvoiceSummary(model.ReportFilters, userinfo);

            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<InvoiceLandedCostSummary>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<InvoiceLandedCostSummary>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                purchaseItemsModel.items = itemsDetail.Where(whereClause, parameters.ToArray()).ToList();
                purchaseItemsModel.total = purchaseItemsModel.items.Count;
            }
            else
            {
                purchaseItemsModel.items = itemsDetail.ToList();
                purchaseItemsModel.total = itemsDetail.Count;

            }

            var allCharges = _purchaseRegister.GetPurchaseInvoiceCharges(model.ReportFilters, _workContext.CurrentUserinformation);
            #region Original Code
            //foreach (var a in purchaseItemsModel.items)
            //{

            //    if (purchaseItemsModel.items.Any(x => x.charges.Any(y => y.ITEM_CODE == a.ItemCode && y.InvoiceNo == a.InvoiceNo)))
            //        continue;
            //    var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ItemCode && x.InvoiceNo == a.InvoiceNo).ToList();
            //    var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
            //    var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE != "VT").Sum(x => x.CHARGE_AMOUNT);
            //    a.TotalCharges = sumOfAddCharges - sumOfSubtractCharges;
            //    a.NetAmount = (a.TotalPrice + sumOfAddCharges) - sumOfSubtractCharges;
            //    a.AvgLandedCost = a.NetAmount / a.Quantity;
            //    a.charges = itemCharges;
            //}
            #endregion

            #region EditedCode
            foreach (var a in purchaseItemsModel.items)
            {

                if (purchaseItemsModel.items.Any(x => x.charges.Any(y => y.ITEM_CODE == a.ItemCode && y.InvoiceNo == a.InvoiceNo)))
                {
                    a.NetAmount = a.TotalPrice;
                    a.TotalCharges = a.TotalCharges.Equals(DBNull.Value) ? 0 : Convert.ToInt32(a.TotalCharges);
                    a.AvgLandedCost = a.AvgLandedCost.Equals(DBNull.Value) ? 0 : Convert.ToInt32(a.AvgLandedCost);
                }
                else
                {
                    var itemCharges = allCharges.Where(x => x.ITEM_CODE == a.ItemCode && x.InvoiceNo == a.InvoiceNo).ToList();
                    var sumOfSubtractCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "D").Sum(x => x.CHARGE_AMOUNT);
                    var sumOfAddCharges = itemCharges.Where(x => x.CHARGE_TYPE_FLAG == "A" && x.CHARGE_CODE != "VT").Sum(x => x.CHARGE_AMOUNT);
                    a.TotalCharges = sumOfAddCharges - sumOfSubtractCharges;
                    a.NetAmount = (a.TotalPrice + sumOfAddCharges) - sumOfSubtractCharges;
                    a.AvgLandedCost = a.NetAmount / a.Quantity;
                    a.charges = itemCharges;
                }
            }

            #endregion

            purchaseItemsModel.AggregationResult = KendoGridHelper.GetAggregation(purchaseItemsModel.items, model.aggregate);
            if (purchaseItemsModel.items.Count() > 0)
            {
                var groupData = allCharges.GroupBy(q => q.CHARGE_CODE).Select(group => new
                {
                    Code = group.Key,
                    Sum = group.Sum(q => q.CHARGE_AMOUNT),
                    Min = group.Min(q => q.CHARGE_AMOUNT),
                    Max = group.Max(q => q.CHARGE_AMOUNT)
                });


                foreach (var item in groupData)
                {
                    purchaseItemsModel.AggregationResult.Add(item.Code, new AggregationModel()
                    {
                        max = item.Max ?? 0,
                        min = item.Min ?? 0,
                        sum = item.Sum ?? 0
                    });
                }
            }

            foreach (var item in model.aggregate)
            {
                if (purchaseItemsModel.AggregationResult.ContainsKey(item.field))
                    continue;

                purchaseItemsModel.AggregationResult.Add(item.field, new AggregationModel()
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
                    purchaseItemsModel.items = purchaseItemsModel.items.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }
            var purchaseVatsPaging = purchaseItemsModel.items.Skip((model.page - 1) * model.pageSize)
                      .Take(model.pageSize).ToList();

            return purchaseItemsModel;
        }


        [HttpGet]
        public List<VoucherModel> GetPurchaseRegisterVouchers(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var voucherRegister = _purchaseRegister.GetPurchaseRegisterVouchers().ToList();
            var voucherRegister = _purchaseRegister.GetPurchaseRegisterVouchers().ToList();
            return voucherRegister;
        }

        [HttpGet]
        public List<DocumentTree> GetAllVoucherNodes()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var AllVoucherList = _purchaseRegister.GetAllVoucherNodes().ToList();
            var AllVoucherList = _purchaseRegister.GetAllVoucherNodes().ToList();

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
            var userinfo = this._workContext.CurrentUserinformation;
            //var AllVoucherList = _purchaseRegister.GetVoucherListByFormCode(level, mastercode).ToList();
            var AllVoucherList = _purchaseRegister.GetVoucherListByFormCode(level, mastercode).ToList();
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

        //Storage Location
        [HttpGet]
        public List<LocationModel> GetStorageLocations(GridFilters filter)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            //var locationRegister = _purchaseRegister.GetStorageLocations().ToList();
            var locationRegister = _purchaseRegister.GetStorageLocations().ToList();
            return locationRegister;
        }

        [HttpGet]
        public List<LocationTree> GetAllStorageLocations()
        {
            var AllLocationsList = _purchaseRegister.GetAllStorageLocationNodes().ToList();
            var locationNodes = new List<LocationTree>();
            foreach (var loc in AllLocationsList)
            {
                locationNodes.Add(new LocationTree()
                {
                    Level = loc.LEVEL,
                    LocationName = loc.LocationName,
                    LocationCode = loc.LocationCode,
                    MasterLocationCode = loc.MasterLocationCode,
                    PreLocationCode = loc.PreLocationCode,
                    hasLocation = loc.GroupFlag == "G" ? true : false
                });
            }
            return locationNodes;
        }

        [HttpGet]
        public List<LocationTree> GetAllStorageLocationsByLocationId(string locationId, string level, string mastercode)
        {
            var AllLocationsList = _purchaseRegister.GetLocationListByLocationId(level, mastercode).ToList();
            var locationNodes = new List<LocationTree>();
            foreach (var loc in AllLocationsList)
            {
                locationNodes.Add(new LocationTree()
                {
                    Level = loc.LEVEL,
                    LocationName = loc.LocationName,
                    LocationCode = loc.LocationCode,
                    MasterLocationCode = loc.MasterLocationCode,
                    PreLocationCode = loc.PreLocationCode,
                    hasLocation = loc.GroupFlag == "G" ? true : false
                });
            }
            return locationNodes;
        }


        //// Company
        [HttpGet]
        public List<CompanyModel> GetCompanyList()
        {
            var companyList = _purchaseRegister.GetCompaniesModelList().ToList();
            return companyList;
        }

        //// Accounts
        [HttpGet]
        public List<AccountsModel> GetAccountsList()
        {
            var accountList = _purchaseRegister.GetAccountsList().ToList();
            return accountList;
        }

        [HttpPost]
        public PurchaseRegisterPivot GetPurchaseRegisterPrivot(filterOption model)
        {

            var salesRegistersViewModel = new PurchaseRegisterPivot();
            var userinfo = _workContext.CurrentUserinformation;
            var salesRegister = _purchaseRegister.GetPurchaseRegisterPivort(model.ReportFilters, userinfo).AsQueryable();
            salesRegistersViewModel.PurchaseRegisters = salesRegister.ToList();
            salesRegistersViewModel.total = salesRegistersViewModel.PurchaseRegisters.Count;


            return salesRegistersViewModel;
        }

        public List<ItemCode> GetItemList()
        {
            var itemList = _purchaseRegister.GetItemList().ToList();
            return itemList;
        }
        [HttpPost]
        public string InsertItem(ItemSetupModal modal)
        {
            var message = _purchaseRegister.InsertItem(modal);
            return message;
        }
        [HttpPost]
        public List<ItemCode> GetItemCode(string ITEM_GROUP_CODE)
        {
            var itemCodeList = _purchaseRegister.GetItemCode(ITEM_GROUP_CODE).ToList();
            return itemCodeList;
        }
        [HttpPost]
        public string DeleteItemCode(string ITEM_GROUP_CODE)
        {
            var message = _purchaseRegister.DeleteItemCode(ITEM_GROUP_CODE);
            return message;
        }

    }
}
