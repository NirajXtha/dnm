using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Models.Ledger;
using NeoErp.Sales.Modules.Services.Models.Voucher;
using NeoErp.Sales.Modules.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class SubsidiaryLedgerController : ApiController
    {
        public IVoucherService _voucherRegister { get; set; }
        public ITrialBalanceService _trialBalanceRegister { get; set; }
        private ISubsidiaryLedger _subsidiaryLedger { get; set; }
        private ICacheManager _cacheManager;
        private IWorkContext _workContext;
        public SubsidiaryLedgerController(IVoucherService voucherRegister, ITrialBalanceService trialBalanceRegister, ICacheManager cacheManager, IWorkContext workContext, ISubsidiaryLedger subsidiaryLedger)
        {
            this._voucherRegister = voucherRegister;
            this._trialBalanceRegister = trialBalanceRegister;
            this._subsidiaryLedger = subsidiaryLedger;
            this._cacheManager = cacheManager;
            this._workContext = workContext;
        }


        [HttpGet]
        public List<TreeModels> GetAllParentCustomerNodes()
        {
            var allCustomerList = _subsidiaryLedger.CustomerListAllParentNodes(_workContext.CurrentUserinformation).ToList();
            foreach (var cust in allCustomerList)
            {
                cust.HasChildren = cust.Child_rec > 0 ? true : false;
                //if (cust.Level == 1)
                //    cust.HasChildren = false;
            }
            return allCustomerList;
        }

        [HttpGet]
        public List<TreeModels> GetAllCustomersByCustId(string custId)
        {
            var allCustomerList = _subsidiaryLedger.GetCustomerListByCustCode("1", custId, _workContext.CurrentUserinformation).ToList();
            foreach (var cust in allCustomerList)
            {
                cust.HasChildren = cust.Child_rec > 0 ? true : false;
            }
            return allCustomerList;

        }


        [HttpGet]
        public List<TreeModels> GetAllParentMiscSubLedgerNodes()
        {
            var allMiscSubLedgerList = _subsidiaryLedger.MisSubLedgerListAllParentNodes(_workContext.CurrentUserinformation).ToList();
            foreach (var cust in allMiscSubLedgerList)
            {
                cust.HasChildren = cust.Child_rec > 0 ? true : false;

            }
            return allMiscSubLedgerList;
        }

        [HttpGet]
        public List<TreeModels> GetAllMiscSubLedgerBySubId(string subId)
        {
            var allMiscSubLedgerList = _subsidiaryLedger.GetCustomerListByCustCode("1", subId, _workContext.CurrentUserinformation).ToList();
            foreach (var cust in allMiscSubLedgerList)
            {
                cust.HasChildren = cust.Child_rec > 0 ? true : false;
            }
            return allMiscSubLedgerList;

        }

        [HttpGet]
        public List<TreeModels> GetAllParentEmployeeNodes()
        {
            var allEmployeeList = _subsidiaryLedger.EmployeeListAllParentNodes(_workContext.CurrentUserinformation).ToList();
            foreach (var emp in allEmployeeList)
            {
                emp.HasChildren = emp.Child_rec > 0 ? true : false;
            }
            return allEmployeeList;
        }

        [HttpGet]
        public List<TreeModels> GetAllEmployeeByEmpId(string empId)
        {
            var allEmployeeList = _subsidiaryLedger.GetEmployeeListByEmpCode("1", empId, _workContext.CurrentUserinformation).ToList();
            foreach (var emp in allEmployeeList)
            {
                emp.HasChildren = emp.Child_rec > 0 ? true : false;
            }
            return allEmployeeList;

        }

        [HttpGet]
        public List<TreeModels> GetAllParentSupplierNodes()
        {
            var allSupplierList = _subsidiaryLedger.SupplierListAllParentNodes(_workContext.CurrentUserinformation).ToList();
            foreach (var sup in allSupplierList)
            {
                sup.HasChildren = sup.Child_rec > 0 ? true : false;
            }
            return allSupplierList;
        }

        [HttpGet]
        public List<TreeModels> GetAllSupplierBySupId(string supId)
        {
            var allSupplierList = _subsidiaryLedger.GetSupplierListBySupCode("1", supId, _workContext.CurrentUserinformation).ToList();
            foreach (var sup in allSupplierList)
            {
                sup.HasChildren = sup.Child_rec > 0 ? true : false;
            }
            return allSupplierList;

        }

        [HttpGet]
        public List<MultiSelectModels> GetSubsidiaryCustomers()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var customerList = new List<MultiSelectModels>();
            //if (this._cacheManager.IsSet("SubsidiaryCustomer"))
            //{
            //    customerList = this._cacheManager.Get<List<MultiSelectModels>>("SubsidiaryCustomer");
            //}
            //else
            //{
            customerList = _subsidiaryLedger.GetSubsidiaryCustomers(_workContext.CurrentUserinformation).ToList();
            this._cacheManager.Set("SubsidiaryCustomer", customerList, 10);
            //}

            return customerList;
        }

        [HttpGet]
        public List<MultiSelectModels> GetSubsidiaryEmployees()
        {
            var userinfo = this._workContext.CurrentUserinformation;
            var employeeList = new List<MultiSelectModels>();
            //if (this._cacheManager.IsSet("SubsidiaryEmployees"))
            //{
            //    employeeList = this._cacheManager.Get<List<MultiSelectModels>>("SubsidiaryEmployees");
            //}
            //else
            //{
            employeeList = _subsidiaryLedger.GetSubsidiaryEmployees(_workContext.CurrentUserinformation).ToList();
            this._cacheManager.Set("SubsidiaryEmployees", employeeList, 10);
            //}

            return employeeList;
        }

        [HttpGet]
        public List<MultiSelectModels> GetSubsidiarySuppliers()
        {

            var userinfo = this._workContext.CurrentUserinformation;
            var supplierList = new List<MultiSelectModels>();
            //if (this._cacheManager.IsSet("SubsidiarySuppliers"))
            //{
            //    supplierList = this._cacheManager.Get<List<MultiSelectModels>>("SubsidiarySuppliers");
            //}
            //else
            //{
            supplierList = _subsidiaryLedger.GetSubsidiarySuppliers(_workContext.CurrentUserinformation).ToList();
            this._cacheManager.Set("SubsidiarySuppliers", supplierList, 10);
            //}

            return supplierList;
        }

        [HttpGet]
        public List<MultiSelectModels> GetSubsidiaryMSubLedger()
        {

            var userinfo = this._workContext.CurrentUserinformation;
            var subsidiaryMSubLedger = new List<MultiSelectModels>();
            //if (this._cacheManager.IsSet("SubsidiaryMSubLedger"))
            //{
            //    subsidiaryMSubLedger = this._cacheManager.Get<List<MultiSelectModels>>("SubsidiaryMSubLedger");
            //}
            //else
            //{
            subsidiaryMSubLedger = _subsidiaryLedger.GetSubsidiaryMSubLedger(_workContext.CurrentUserinformation).ToList();
            this._cacheManager.Set("SubsidiaryMSubLedger", subsidiaryMSubLedger, 10);
            //}

            return subsidiaryMSubLedger;
        }

        [HttpGet]
        public List<MultiSelectModels> GetSubLedgerTransactions(string accountCode, string listType)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            if (string.IsNullOrEmpty(accountCode))
            {
                return new List<MultiSelectModels>();
            }
            var list = _subsidiaryLedger.GetListByHierarchy(accountCode, userinfo, listType).Select(x => new MultiSelectModels { Code = x.Code, LinkSubCode = x.LinkSubCode }).ToList();
            return list;
        }

        [HttpPost]
        public gridVoucherModel GetSubsidiaryVouchersDetails(filterOption model, string formDate, string toDate, string AccoundCode, string LinkSubCode, string branchCode = null, string groupSkuFlag = "", string listType = "",string MasterCode = "",string actionName = "")
        {
            var modelVoucher = new gridVoucherModel();
            try
            {
                // model.take = 10;
                // model.pageSize = 10;
                var voucherList = _subsidiaryLedger.GetSubsidiaryVoucherDetails(model.ReportFilters, formDate, toDate, AccoundCode, _workContext.CurrentUserinformation, LinkSubCode, branchCode, groupSkuFlag, listType,MasterCode,actionName).OrderBy(q => q.voucher_date).ToList();
                modelVoucher.total = voucherList.Count;
                voucherList = voucherList.ToList();
                modelVoucher.data = voucherList;
                return modelVoucher;
            }
            catch (Exception ex)
            {
                VoucherDetailModel data = new VoucherDetailModel();
                data.Voucher_no = "MaximumData";
                modelVoucher.data.Add(data);
                return modelVoucher;
            }
        }

        [HttpGet]
        public List<MultiSelectModels> GetLedgerAC(string ledgerMultiSelects)
        {
            var salesRegister = _subsidiaryLedger.GetLedgerAC(ledgerMultiSelects).ToList();
            return salesRegister;
        }
    }
}
