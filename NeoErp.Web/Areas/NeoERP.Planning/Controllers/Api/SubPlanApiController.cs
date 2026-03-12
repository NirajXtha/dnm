using NeoErp.Core;
using NeoErp.Core.Caching;
using NeoErp.Core.Helpers;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using NeoERP.Planning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoERP.Planning.Controllers.Api
{
    public class SubPlanApiController : ApiController
    {
        ISubPlanRepo _subplan;
        IPlanSetup _planSetup;
        IWorkContext _workContext { get; set; }
        private ICacheManager _cacheManager { get; set; }
        public SubPlanApiController(ISubPlanRepo subplan, IPlanSetup planSetup, IWorkContext iworkContext)
        {
            _subplan = subplan;
            _planSetup = planSetup;
            this._workContext = iworkContext;
        }

        [HttpPost]
        public List<SubPlanModels> ViewSubPlanReport(SubPlanModels viewdetail)
        {
            return _subplan.ViewSubPlanReport(viewdetail);
        }

        [HttpGet]
        public List<SubPlanModels> ViewSubPlans(string plancode, int? take, int? skip, int? page, int? pageSize)
        {
            return this._subplan.getSubPlans(plancode);
        }

        [HttpPost]
        public PlanModels GetPlan(string plancode)
        {
            return _subplan.getPlan(plancode);
        }

        public List<MyColumnSettings> GetDynamicMultiHeader(string plancode)
        {
            var FrequencyTitle = _planSetup.GetFrequencyTitle(plancode);
            return FrequencyTitle;
            //return _subplan.getAllDynamicColumnHeader(plancode);

        }

        [HttpGet]
        public string GetFrequencyColumnByPlanCode(string plancode)
        {
            return _subplan.GetFrequencyColumnByPlanCode(plancode);
        }

        [HttpPost]
        public List<SubPlanModels> GetTargetValue(SubPlanModels details)
        {
            return _subplan.getTargetValue(details);
        }

        public List<PlanModels> GetAllPlanNames(string filter)
        {
            return _subplan.getPlanList(filter);
        }

        [HttpGet]
        public List<string> GetSubGroup()
        {
            var subgroup = Enum.GetNames(typeof(EnumSubgroup)).ToList();
            return subgroup;
        }

        [HttpGet]
        public List<DateFilterModel> GetNextFiscalYearhDateFilters(string nFiscalYear, string textToAppend = "", bool appendText = false)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._subplan.GetFiscalYearDateFilters(FincalYear,nFiscalYear, textToAppend, appendText).OrderBy(q => q.SortOrder).ToList();
            return result;
        }
        [HttpGet]
        public List<DateFilterModel> GetEnglishDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false)
        {
            var FincalYear = System.Configuration.ConfigurationManager.AppSettings["FiscalYear"].ToString();
            var result = this._subplan.GetEnglishDateFilters(FincalYear, textToAppend, appendText).OrderByDescending(q => q.SortOrder).ToList();
            return result;
        }
        [HttpGet]
        public List<MyColumnSettings> GetTimeFrameForSubPlan(string planCode)
        {
            var FrequencyTitle = _planSetup.GetFrequencyTitle(planCode);
            return FrequencyTitle;
        }

        [HttpGet]
        public List<PlanRegisterProductModel> GetSalesRegisterProducts(filterOption filter,string pageNameId, bool individual = false)
        {
            var productRegister = new List<PlanRegisterProductModel>();
            if (individual == false)
            {
                var userinfo = this._workContext.CurrentUserinformation;

                //if (this._cacheManager.IsSet("PlanRegisterProduct"))
                //{
                //    productRegister = this._cacheManager.Get<List<PlanRegisterProductModel>>("PlanRegisterProduct");
                //}
                //else
                //{
                    productRegister = _subplan.PlanRegisterProducts(userinfo,pageNameId).ToList();
                    //this._cacheManager.Set("PlanRegisterProduct", productRegister, 1);
                //}
            }
            else
                productRegister = _subplan.PlanRegisterProductsIndividual().ToList();
            return productRegister;
        }

        public List<EmployeeTree> GetEmployees()
        {
            var result = _subplan.getAllEmployees();
            var employeeNodes = new List<EmployeeTree>();
            var employeeNode = generateEmployeeTree(result, employeeNodes, "00");
            return employeeNode;
        }

        private List<EmployeeTree> generateEmployeeTree(List<EmployeeModels> model, List<EmployeeTree> employeeNodes, string preEmployeeCode)
        {
            foreach (var employee in model.Where(x => x.PRE_EMPLOYEE_CODE == preEmployeeCode))
            {
                var employeeNodesChild = new List<EmployeeTree>();
                employeeNodes.Add(new EmployeeTree()
                {
                    Level = employee.LEVEL,
                    employeeName = employee.EMPLOYEE_EDESC,
                    employeeId = employee.EMPLOYEE_CODE,
                    masterEmployeeCode = employee.MASTER_EMPLOYEE_CODE,
                    groupSkuFlag = employee.GROUP_SKU_FLAG,
                    preEmployeeCode = employee.PRE_EMPLOYEE_CODE,
                    hasEmployees = employee.GROUP_SKU_FLAG == "G" ? true : false,
                    Items = employee.GROUP_SKU_FLAG == "G" ? generateEmployeeTree(model, employeeNodesChild, employee.MASTER_EMPLOYEE_CODE) : null,
                });

            }
            return employeeNodes;
        }

        public List<AccountSetup> getAllAccountSetup(string filter)
        {
            var result = _subplan.getAllAccounts();
            return result;
        }

        public List<CustomersTree> GetCustomers()
        {
            var result = _subplan.getAllCustomer();
            var customerNodes = new List<CustomersTree>();
            var customerNode = generateCustomerTree(result, customerNodes, "00");
            return customerNode;
        }

        public List<CustomerModels> GetChiledLevelCustomers(string filter)
        {
            List<CustomerModels> result = _subplan.getAllCustomer(filter);
            return result;
        }
        public List<CustomerModels> GetCustomersForPlan(string filter)
        {
            if (string.IsNullOrEmpty(filter))
                return new List<CustomerModels>();

            List<CustomerModels> result = _subplan.getAllCustomerForPlan(filter);
            return result;
        }
        public List<AgentModels> GetChiledLevelAgent(string filter)
        {
            List<AgentModels> result = _subplan.getAllAgent(filter);
            return result;
        }
        public List<BranchModels> GetChiledLevelBranch(string filter)
        {
            List<BranchModels> result = _subplan.getAllBranch(filter);
            return result;
        }
        public List<DivisionModels> GetChiledLevelDivision(string filter)
        {
            List<DivisionModels> result = _subplan.getAllDivision(filter);
            return result;
        }
        public List<EmployeeModels> GetChiledLevelEmployee(string filter)
        {
            List<EmployeeModels> result = _subplan.getAllEmployee(filter);
            return result;
        }
        public List<PARTY_TYPE_MODELS> GetChiledLevelPartyType(string filter)
        {
            var result = _subplan.GetChiledLevelPartyType(filter);
            return result;
        }


        public List<EmployeeModels> GetChiledLevelEmployeeForTree(string filter)
        {
            List<EmployeeModels> result = _subplan.getAllEmployeeForHrEmployeeTree(filter);
            return result;
        }

        private List<CustomersTree> generateCustomerTree(List<CustomerModels> model, List<CustomersTree> customerNodes, string preItemCode)
        {
            foreach (var customer in model.Where(x => x.PRE_CUSTOMER_CODE == preItemCode))
            {
                var customerNodesChild = new List<CustomersTree>();
                customerNodes.Add(new CustomersTree()
                {
                    Level = customer.LEVEL,
                    customerName = customer.CUSTOMER_EDESC,
                    customerId = customer.CUSTOMER_CODE,
                    masterCustomerCode = customer.MASTER_CUSTOMER_CODE,
                    preCustomerCode = customer.PRE_CUSTOMER_CODE,
                    groupSkuFlag = customer.GROUP_SKU_FLAG,
                    hasCustomers = customer.GROUP_SKU_FLAG == "G" ? true : false,
                    Items = customer.GROUP_SKU_FLAG == "G" ? generateCustomerTree(model, customerNodesChild, customer.MASTER_CUSTOMER_CODE) : null,
                });

            }
            return customerNodes;
        }


        public List<DivisionTree> GetDivisions()
        {
            var result = _subplan.getAllDivisions();
            var divisionNodes = new List<DivisionTree>();
            var divisionNode = generateDivisionTree(result, divisionNodes, "00");
            return divisionNode;
        }


        private List<DivisionTree> generateDivisionTree(List<DivisionModels> model, List<DivisionTree> divisionNodes, string preDivisionCode)
        {
            foreach (var division in model.Where(x => x.PRE_DIVISION_CODE == preDivisionCode))
            {
                var divisionNodesChild = new List<DivisionTree>();
                divisionNodes.Add(new DivisionTree()
                {
                    Level = division.LEVEL,
                    divisionName = division.DIVISION_EDESC,
                    divisionId = division.DIVISION_CODE,
                    masterDivisionCode = division.DIVISION_CODE,
                    groupSkuFlag = division.GROUP_SKU_FLAG,
                    preDivisionCode = division.PRE_DIVISION_CODE,
                    hasDivisions = division.GROUP_SKU_FLAG == "G" ? true : false,
                    Items = division.GROUP_SKU_FLAG == "G" ? generateDivisionTree(model, divisionNodesChild, division.DIVISION_CODE) : null,
                });

            }
            return divisionNodes;
        }
        [HttpGet]
        public HttpResponseMessage CreateEntities(string systemName)
        {
            try
            {
                var Count = _subplan.createEntities(systemName);
                return Request.CreateResponse(HttpStatusCode.OK, new { COUNT=Math.Abs(Count), MESSAGE = $"{Math.Abs(Count)} Entities successfully created", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }

    }

    public class filter
    {
        public string field { get; set; }
        public string condition { get; set; }
        public string value { get; set; }
        public int take { get; set; }
        public int skip { get; set; }
        public int page { get; set; }
        public int pageSize { get; set; }
        //filter[logic]:and
        //filter[filters][0]
        //        [field]:PLAN_CODE
        //filter[filters][0]
        //        [condition]:eq
        //filter[filters][0]
        //        [value]:382
    }

}
