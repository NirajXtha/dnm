using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models.Contract;
using NeoErp.Sales.Modules.Services.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Linq.Dynamic;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class ContractController : ApiController
    {
        private IContractService _contractService;
        private IWorkContext _workContext;

        public ContractController(IContractService contractService)
        {
            this._contractService = contractService;
        }

        [HttpPost]
        public List<ContractViewModel> GetContractDetailMonthWise(filterOption model)
        {
            var returnviewModel = new ContractLandingReportModel();
           
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var contracts = _contractService.GetAllContractInfo(model,FincalYear);
            //if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            //{
            //    string whereClause = null;
            //    var parameters = new System.Collections.ArrayList();
            //    var filters = model.filter.Filters;
            //    for (var i = 0; i < filters.Count; i++)
            //    {
            //        if (i == 0)
            //            whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<ContractViewModel>(i, model.filter.Logic, filters[i], parameters));
            //        else
            //            whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<ContractViewModel>(i, model.filter.Logic, filters[i], parameters));
            //    }
            //    // Where
            //    returnviewModel.items = contracts.Where(whereClause, parameters.ToArray()).ToList();
            //    returnviewModel.total = contracts.Count;
            //}
            //else
            //{
            //    returnviewModel.items = contracts.ToList();
            //    returnviewModel.total = contracts.Count;
            //}

         
            //returnviewModel.items = returnviewModel.items.Skip((model.page - 1) * model.pageSize)
            //          .Take(model.pageSize).ToList();
            //if (model.sort != null && model.sort.Count > 0)
            //{
            //    foreach (var s in model.sort)
            //    {
            //        // s.Field =  s.Field;
            //        returnviewModel.items = returnviewModel.items.OrderBy(s.Field + " " + s.Dir).ToList();
            //    }
            //}

            foreach (var contract in contracts)
            { 
                try
                {
                    contract.TotalAmount = 0;
                    if (contract.service_start_date == null)
                        contract.service_start_date = DateTime.Now.AddYears(1);
                    var servicestartdate = EnglishToNepaliDateConveter.EngToNep(contract.service_start_date ?? DateTime.Now);
                    var serviceExpireddate = EnglishToNepaliDateConveter.EngToNep(contract.EXPIRY_DATE ?? DateTime.Now);
                    var NepTodayDate = EnglishToNepaliDateConveter.EngToNep(DateTime.Now);
                    #region bymonthly
                    if (contract.PAYMENT_BASIS.Trim().ToLower().Equals("MONTHLY".Trim().ToLower()))
                    {
                        var currentYear = DateTime.Now.Year;
                        var startMonth = 4;
                        if (servicestartdate.Year < NepTodayDate.Year)
                        {
                            startMonth = 4;
                        }
                        else if (servicestartdate.Year == NepTodayDate.Year)
                        {
                            if (servicestartdate.Month >=3)
                            {
                                var monthid = servicestartdate.Month;
                                startMonth = monthid;
                            }
                            else
                            {
                                startMonth = 4;
                            }
                             
                          
                        }
                        else
                        {
                            var monthid = servicestartdate.Month+12;
                            startMonth = monthid;
                        }
                        var endMOnth = 16;
                        if (serviceExpireddate.Year == NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month > 3)
                                endMOnth = serviceExpireddate.Month;
                            else
                            {
                                endMOnth = 4;
                            }
                        }
                        else if (serviceExpireddate.Year > NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month <= 3)
                                endMOnth = serviceExpireddate.Month + 12;
                        }


                        for (int i = startMonth; i <= 15; i++)
                        {
                            if (i >= endMOnth)
                                break;
                            switch (i)
                            {
                                case 4:
                                    contract.Shrawan = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.Shrawan;
                                    break;
                                case 5:
                                    contract.Bhadra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 6:
                                    contract.Ashwin = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 7:
                                    contract.Kartik = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 8:
                                    contract.Mangsir = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 9:
                                    contract.Poush = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 10:
                                    contract.Magh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 11:
                                    contract.Falgun = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 12:
                                    contract.Chaitra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 13:
                                    contract.Baisakh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 14:
                                    contract.Jestha = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 15:
                                    contract.Ashadh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                            }

                        }

                    }
                    #endregion

                    #region byQuaterly
                    else if (contract.PAYMENT_BASIS.Trim().ToLower().Equals("QUATERLY".Trim().ToLower()))
                    {
                        var currentYear = DateTime.Now.Year;
                        var startMonth = 4;
                        if (servicestartdate.Year < NepTodayDate.Year)
                        {
                            startMonth = 4;
                        }
                        else if (servicestartdate.Year == NepTodayDate.Year)
                        {
                            if (servicestartdate.Month >= 3)
                            {
                                var monthid = servicestartdate.Month;
                                startMonth = monthid;
                            }
                            else
                            {
                                startMonth = 4;
                            }


                        }
                        else
                        {
                            var monthid = servicestartdate.Month + 12;
                            startMonth = monthid;
                        }
                        var endMOnth = 16;
                        if (serviceExpireddate.Year == NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month > 3)
                                endMOnth = serviceExpireddate.Month;
                            else
                            {
                                endMOnth = 4;
                            }
                        }
                        else if (serviceExpireddate.Year > NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month <= 3)
                                endMOnth = serviceExpireddate.Month + 12;
                        }



                        for (int i = startMonth; i <= 15; i = i + 3)
                        {
                            if (i >= endMOnth)
                                break;
                            switch (i)
                            {
                                case 4:
                                    contract.Shrawan = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.Shrawan;
                                    break;
                                case 5:
                                    contract.Bhadra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 6:
                                    contract.Ashwin = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 7:
                                    contract.Kartik = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 8:
                                    contract.Mangsir = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 9:
                                    contract.Poush = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 10:
                                    contract.Magh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 11:
                                    contract.Falgun = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 12:
                                    contract.Chaitra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 13:
                                    contract.Baisakh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 14:
                                    contract.Jestha = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 15:
                                    contract.Ashadh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                            }

                        }


                    }
                    #endregion
                    #region by Half Year
                    else if (contract.PAYMENT_BASIS.Trim().ToLower().Contains("Half".Trim().ToLower()))
                    {
                        var currentYear = DateTime.Now.Year;
                        var startMonth = 4;
                        if (servicestartdate.Year < NepTodayDate.Year)
                        {
                            startMonth = 4;
                        }
                        else if (servicestartdate.Year == NepTodayDate.Year)
                        {
                            if (servicestartdate.Month >= 3)
                            {
                                var monthid = servicestartdate.Month;
                                startMonth = monthid;
                            }
                            else
                            {
                                startMonth = 4;
                            }


                        }
                        else
                        {
                            var monthid = servicestartdate.Month + 12;
                            startMonth = monthid;
                        }
                        var endMOnth = 16;
                        if (serviceExpireddate.Year == NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month > 3)
                                endMOnth = serviceExpireddate.Month;
                            else
                            {
                                endMOnth = 4;
                            }
                        }
                        else if (serviceExpireddate.Year > NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month <= 3)
                                endMOnth = serviceExpireddate.Month + 12;
                        }


                        for (int i = startMonth; i <= 15; i = i + 6)
                        {
                            if (i >= endMOnth)
                                break;
                            switch (i)
                            {
                                case 4:
                                    contract.Shrawan = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.Shrawan;
                                    break;
                                case 5:
                                    contract.Bhadra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 6:
                                    contract.Ashwin = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 7:
                                    contract.Kartik = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 8:
                                    contract.Mangsir = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 9:
                                    contract.Poush = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 10:
                                    contract.Magh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 11:
                                    contract.Falgun = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 12:
                                    contract.Chaitra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 13:
                                    contract.Baisakh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 14:
                                    contract.Jestha = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 15:
                                    contract.Ashadh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                            }

                        }


                    }
                    #endregion

                    #region by  Year
                    else if (contract.PAYMENT_BASIS.Trim().ToLower().Equals("YEARLY".Trim().ToLower()))
                    {
                        var currentYear = DateTime.Now.Year;
                        var startMonth = 4;
                        if (servicestartdate.Year < NepTodayDate.Year)
                        {
                            startMonth = 4;
                        }
                        else if (servicestartdate.Year == NepTodayDate.Year)
                        {
                            if (servicestartdate.Month >= 3)
                            {
                                var monthid = servicestartdate.Month;
                                startMonth = monthid;
                            }
                            else
                            {
                                startMonth = 4;
                            }


                        }
                        else
                        {
                            var monthid = servicestartdate.Month + 12;
                            startMonth = monthid;
                        }
                        var endMOnth = 16;
                        if (serviceExpireddate.Year == NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month > 3)
                                endMOnth = serviceExpireddate.Month;
                            else
                            {
                                endMOnth = 4;
                            }
                        }
                        else if (serviceExpireddate.Year > NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month <= 3)
                                endMOnth = serviceExpireddate.Month + 12;
                        }


                        for (int i = startMonth; i <= 15; i = i + 12)
                        {
                            if (i >= endMOnth)
                                break;
                            switch (i)
                            {
                                case 4:
                                    contract.Shrawan = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.Shrawan;
                                    break;
                                case 5:
                                    contract.Bhadra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 6:
                                    contract.Ashwin = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 7:
                                    contract.Kartik = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 8:
                                    contract.Mangsir = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 9:
                                    contract.Poush = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 10:
                                    contract.Magh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 11:
                                    contract.Falgun = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 12:
                                    contract.Chaitra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 13:
                                    contract.Baisakh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 14:
                                    contract.Jestha = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 15:
                                    contract.Ashadh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                            }

                        }


                    }
                    #endregion
                    #region by  biMonth
                    else if (contract.PAYMENT_BASIS.Trim().ToLower().Equals("BIMONTHLY".Trim().ToLower()))
                    {
                        var currentYear = DateTime.Now.Year;
                        var startMonth = 4;
                        if (servicestartdate.Year < NepTodayDate.Year)
                        {
                            startMonth = 4;
                        }
                        else if (servicestartdate.Year == NepTodayDate.Year)
                        {
                            if (servicestartdate.Month >= 3)
                            {
                                var monthid = servicestartdate.Month;
                                startMonth = monthid;
                            }
                            else
                            {
                                startMonth = 4;
                            }


                        }
                        else
                        {
                            var monthid = servicestartdate.Month + 12;
                            startMonth = monthid;
                        }
                        var endMOnth = 16;
                        if (serviceExpireddate.Year == NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month > 3)
                                endMOnth = serviceExpireddate.Month;
                            else
                            {
                                endMOnth = 4;
                            }
                        }
                        else if (serviceExpireddate.Year > NepTodayDate.Year)
                        {
                            if (serviceExpireddate.Month <= 3)
                                endMOnth = serviceExpireddate.Month + 12;
                        }

                        for (int i = startMonth; i <= 15; i = i + 2)
                        {
                            if (i >= endMOnth)
                                break;
                            switch (i)
                            {
                                case 4:
                                    contract.Shrawan = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.Shrawan;
                                    break;
                                case 5:
                                    contract.Bhadra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 6:
                                    contract.Ashwin = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 7:
                                    contract.Kartik = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 8:
                                    contract.Mangsir = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 9:
                                    contract.Poush = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 10:
                                    contract.Magh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 11:
                                    contract.Falgun = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 12:
                                    contract.Chaitra = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 13:
                                    contract.Baisakh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 14:
                                    contract.Jestha = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                                case 15:
                                    contract.Ashadh = contract.RE_PAYMENT_AMOUNT ?? 0;
                                    contract.TotalAmount = contract.TotalAmount + contract.RE_PAYMENT_AMOUNT ?? 0;
                                    break;
                            }

                        }


                    }
                    #endregion
                }
                catch(Exception ex)
                {
                    
                    continue;
                }
            }
         //   returnviewModel.AggregationResult = KendoGridHelper.GetAggregation(returnviewModel.items);
            return contracts;
        }



    }
}
