using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Service.Models;
using NeoERP.Planning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.OutputCache.V2;

namespace NeoERP.Planning.Controllers.Api
{
    public class PlanApiController : ApiController
    {
        IPlan _plan;
        private IWorkContext _workContext;
        public PlanApiController(IPlan plan, IWorkContext iworkContext)
        {
            _plan = plan;
            this._workContext = iworkContext;
        }

        [HttpGet]
        public List<FrequencyModels> GetAllFrequencyByFilters(string filter = null)
        {
            return _plan.getFrequencyByFilter(filter);
        }
        [HttpGet]
        public List<AggregatePlanModel> GetAggregatePlanType(string filter = null)
        {
            return _plan.GetAggregatePlanType(filter);
        }
        [HttpGet]
        public List<ProductCategory> GetAllCategory(string filter = null)
        {
            return _plan.GetAllCategory(filter);
        }
        [HttpPost]
        public string SaveConfigSetup(ConfigSetupModel model)
        {
            var result = _plan.SaveConfigSetup(model);
            return "";
        }
        [HttpGet]
        public List<string> GetPlanType()
        {
            var plantype = Enum.GetNames(typeof(EnumPlanType)).ToList();

            return plantype;
        }

        [HttpGet]
        public List<string> GetPlanFor()
        {
            var plantype = Enum.GetNames(typeof(EnumPlanFor)).ToList();
            return plantype;
        }

        [HttpGet]
        public List<PlanModels> GetPlanList()
        {
            return _plan.getAllPlans();

        }

        [HttpPost]
        public List<SalesPlan> GetSalesPlanList(filterOption model)
        {

            return _plan.getAllSalesPlans(model.ReportFilters);

        }

        [HttpGet]
        public List<SalesPlans_CustomersEmployees> GetDateWiseCustomerEmployeeDivisions(string startdate, string enddate)
        {
            return this._plan.GetDateWiseCustomerEmployeeDivisions(startdate, enddate);
        }
        [HttpGet]
        public List<SalesPlans_CustomersEmployees> GetDateWise_SalesPlan_Branch(string startdate, string enddate)
        {
            return this._plan.GetDateWiseCustomerEmployeeDivisions(startdate, enddate, "branch");
        }
        [HttpGet]
        public List<SalesPlans_CustomersEmployees> GetDateWise_SalesPlan_Division(string startdate, string enddate)
        {
            return this._plan.GetDateWiseCustomerEmployeeDivisions(startdate, enddate, "division");
        }
        [HttpGet]
        public List<SalesPlans_CustomersEmployees> GetDateWise_SalesPlan_Employee(string startdate, string enddate)
        {
            return this._plan.GetDateWiseCustomerEmployeeDivisions(startdate, enddate, "employee");
        }
        [HttpGet]
        public List<SalesPlans_CustomersEmployees> GetDateWise_SalesPlan_Customer(string startdate, string enddate)
        {
            return this._plan.GetDateWiseCustomerEmployeeDivisions(startdate, enddate, "customer");
        }
        [HttpGet]
        public List<SalesPlan> GetSalesPlanListMaster(string customercode, string employeecode, string divisioncode, string branchcode, string startdate, string enddate)
        {
            List<SalesPlan> model = new List<SalesPlan>();
            model = _plan.getAllSalesPlans(customercode, employeecode, divisioncode, branchcode, startdate, enddate);
            //return _plan.getAllSalesPlans(model.ReportFilters);
            return model;

        }
        //[HttpGet]
        //public List<ConsolidateTree> GetAllConsolidateNodes()
        //{
        //    var userinfo = this._workContext.CurrentUserinformation;
        //    var result =  _plan.CompanyListAllNodes(userinfo).ToList();
        //    var consolidateNodes = new List<ConsolidateTree>();
        //    var productNode = generateBrancTree(result, consolidateNodes, "00");
        //    return productNode;

        //}

        //private List<ConsolidateTree> generateBrancTree(List<ConsolidateSetupModel> model, List<ConsolidateTree> consolidateNodes, string preBranchCode)
        //{
        //    foreach (var item in model.Where(x => x.PRE_BRANCH_CODE == preBranchCode))
        //    {
        //        var productNodesChild = new List<ConsolidateTree>();
        //        consolidateNodes.Add(new ConsolidateTree()
        //        {
        //            branch_Code = item.BRANCH_CODE,
        //            branch_edesc = item.BRANCH_EDESC,
        //            GroupSkuFlag = item.GroupFlag,

        //            pre_branch_code = item.PRE_BRANCH_CODE,
        //            hasBranch = item.GroupFlag == "G" ? true : false,
        //            Items = item.GroupFlag == "G" ? generateBrancTree(model, productNodesChild, item.BRANCH_CODE) : null,
        //        });

        //    }
        //    return consolidateNodes;
        //}

        [HttpGet]

        public List<ProductTree> GetAllProductsWithChildItem(string pageNameId)
        {
            var result = _plan.getAllProductsWithChildItem(pageNameId);
            var productNodes = new List<ProductTree>();
            var productNode = generateProductTree(result, productNodes, "00");
            return productNode;
        }


        private List<ProductTree> generateProductTree(List<ProductSetupModel> model, List<ProductTree> productNodes, string preItemCode)
        {
            foreach (var item in model.Where(x => x.PreItemCode == preItemCode))
            {
                var productNodesChild = new List<ProductTree>();
                productNodes.Add(new ProductTree()
                {
                    Level = item.LEVEL,
                    ItemName = item.ItemName,
                    ItemCode = item.ItemCode,
                    Rate = item.Rate,
                    GroupSkuFlag = item.GroupFlag,
                    MasterItemCode = item.MasterItemCode,
                    PreItemCode = item.PreItemCode,
                    hasProducts = item.GroupFlag == "G" ? true : false,
                    Items = item.GroupFlag == "G" ? generateProductTree(model, productNodesChild, item.MasterItemCode) : null,
                });

            }
            return productNodes;
        }

        public HttpResponseMessage GetPlanDetailByPlanCode(string planCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            try
            {
                var planDetail = _plan.GetPlanDetailByPlanCode(planCode, userinfo).ToList();
                if (planDetail.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Plan Detail Successfully retrived", DATA = planDetail.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Plan Detail not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }

        }

        public HttpResponseMessage GetProductListWithChildByPreCode(string preItemCode)
        {
            var userinfo = this._workContext.CurrentUserinformation;
            try
            {
                var allProductList = _plan.GetProductListWithChildByPreCode(preItemCode, userinfo).ToList();
                var productNodes = new List<PlanProductTree>();
                foreach (var prod in allProductList)
                {
                    productNodes.Add(new PlanProductTree()
                    {



                        GROUP_SKU_FLAG = "",
                        IS_CHILD_SELECTED = "",
                        ITEM_CODE = prod.ItemCode,
                        ITEM_EDESC = prod.ItemName,
                        MASTER_ITEM_CODE = prod.MasterItemCode,
                        PRE_ITEM_CODE = prod.PreItemCode,


                        //LEVEL = prod.LEVEL,
                        //ITEM_EDESC = prod.ItemName,
                        //ITEM_CODE = prod.ItemCode,
                        //MASTER_ITEM_CODE = prod.MasterItemCode,
                        //PRE_ITEM_CODE = prod.PreItemCode,
                        //hasProducts = prod.GroupFlag == "G" ? true : false

                    });
                }
                if (productNodes.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Product list Successfully retrived", DATA = productNodes.ToList(), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Product list not found", STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });

            }

            //return productNodes;

        }

        [HttpGet]
        public List<AggregatePlanModel> GetAllPlansWithType()
        {
            var result = new List<AggregatePlanModel>();
            try
            {
                result = _plan.GetAllPlansWithType();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        [HttpGet]
        public List<EmployeeHandoverModel> getEmployeeHandoverList()
        {
            var result = new List<EmployeeHandoverModel>();
            try
            {
                result = _plan.getEmployeeHandoverList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
        public List<PlanListModel> getEmployeeHandoverListWithPlan(string fromEmpCode, string toEmpCode) {
            var result = new List<PlanListModel>();
            try
            {
                result = _plan.getEmployeeHandoverListWithPlan(fromEmpCode,toEmpCode);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }

        [HttpPost]
        public HttpResponseMessage CreatePlan(PlanModels plandetail)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //if (plandetail.END_DATE < plandetail.START_DATE)
                    //{
                    //    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "dateValidation", STATUS_CODE = (int)HttpStatusCode.OK });
                    //}
                    //else
                    //{
                    var message = _plan.generatePlan(plandetail);
                    //var message = "";
                    if (message == "validation")
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Validation", STATUS_CODE = (int)HttpStatusCode.OK });
                    }
                    else
                    {
                        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", PLAN_CODE = message, STATUS_CODE = (int)HttpStatusCode.OK });
                    }
                    //}
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "fieldValidation", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "DbError", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateSalesPlan(SalesPlan sp)
        {
            bool isUpdated = this._plan.UpdateSalesPlan(sp);
            if (isUpdated)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "success", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "fieldValidation", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpPost]
        public HttpResponseMessage ApprovedPlan(int planCode, bool isChecked)
        {
            try
            {
                var deleteStatus = this._plan.approvedSalesPlan(planCode, isChecked);
                if (deleteStatus)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        [HttpPost]
        public HttpResponseMessage DeletePlane(int planCode)
        {
            try
            {
                var deleteStatus = this._plan.deleteSalesPlan(planCode);
                if (deleteStatus)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpPost]
        public HttpResponseMessage CopyPlane(int planCode)
        {
            try
            {
                var cloneStatus = this._plan.cloneSalesPlan(planCode);
                if (cloneStatus)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetMasterSalesPlanList()
        {
            List<MasterSalesPlan> masterSalesPlanList = new List<MasterSalesPlan>();
            masterSalesPlanList = this._plan.getMasterSalesPlanList();
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = masterSalesPlanList, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpGet]
        public HttpResponseMessage GetMasterSalesPlanDetail(string masterplancode)
        {
            MasterSalesPlan msp = new MasterSalesPlan();
            msp = this._plan.getMasterSalesPlanDetail(masterplancode);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = msp, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpGet]
        public HttpResponseMessage GetMasterSalesPlanDetailList(string masterplancode)
        {
            List<SalesPlanMap> msp = new List<SalesPlanMap>();
            msp = this._plan.getMasterSalesPlanDetailList(masterplancode);
            //return Request.CreateResponse(HttpStatusCode.OK, new { msp, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
            return Request.CreateResponse(msp);
        }

        [HttpGet]
        public HttpResponseMessage DeleteMasterSalesPlan(string code)
        {
            var result = this._plan.DeleteMasterSalesPlan(code);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = result, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpGet]
        public HttpResponseMessage getEmployeeByUserId(string userId)
        {
            var result = this._plan.GetEmployeeByID(userId);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = result, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpGet]
        public HttpResponseMessage SaveMasterPlan(string masterPlanCode, string masterPlanName, string startDate,
            string endDate, string customer, string employee, string division, string branch, string selectedPlans)
        {
            try
            {
                string isSaved = this._plan.SaveMasterPlan(masterPlanCode, masterPlanName, startDate, endDate, customer, employee, division, branch, selectedPlans);
                if (string.Equals(isSaved, "success"))
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = isSaved, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = isSaved, MESSAGE = isSaved, STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetParentEmployeeTreeList()
        {
            List<HREmployeeTreeModel> hrEmployeeTree = new List<HREmployeeTreeModel>();
            hrEmployeeTree = this._plan.GetParentEmployeeTreeList();
            return Ok(hrEmployeeTree);
        }
        [HttpGet]
        public IHttpActionResult SaveParentEmployeeTreeList(string parentEmployeeCode, string selectedEmployees)
        {
            string saveResult = this._plan.SaveParentEmployeeTreeList(parentEmployeeCode, selectedEmployees);
            return Ok(saveResult);
        }

        [HttpGet]
        public IHttpActionResult DeleteEmployeeFromTree(string parent_employee_code, string employee_code)
        {
            string saveResult = this._plan.DeleteEmployeeFromTree(parent_employee_code, employee_code);
            return Ok(saveResult);
        }
        public List<Employee> GetEmployeeList()
        {
            return this._plan.GetEmployeesList();
        }

        [HttpGet]
        public List<User> GetAllUsers(string filter = null)
        {
            return this._plan.getAllUsers(filter);
        }

        #region Preference status

        #region   #region Preference Dvision Flag
        public HttpResponseMessage GetDivisionFlag()
        {
            try
            {
                var flag = this._plan.GetDivisionFlag();
                if (flag != "")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag, MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        #endregion

        #region Preference Flag
        //sales plan  ok
        public HttpResponseMessage GetPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetPreferenceSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //ledger plan no need
        public HttpResponseMessage GetledgerPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GeLedgerPlanSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        public HttpResponseMessage GetPlanPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetSalesPlanSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //Budget plan copy of planning (check)
        public HttpResponseMessage GetbudgetPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetBudgetPlanSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //production plan ok 
        public HttpResponseMessage GetproductionPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetProductionPlanSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //branding no need
        public HttpResponseMessage GetbrandingPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetBrandingSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //procurement ok
        public HttpResponseMessage GetProcuremntPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetProcuremntSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        //procurement ok
        public HttpResponseMessage GetCollectionPreferenceSetupFlag()
        {
            try
            {
                var flag = this._plan.GetCollectionSetupFlag();
                if (flag.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { DATA = flag.ToList(), MESSAGE = "Success", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error", STATUS_CODE = (int)HttpStatusCode.OK });
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex);
            }
        }
        #endregion


        #endregion
    }
}
