using Microsoft.Office.Interop.Excel;
using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Distribution.Service.Model;
using NeoErp.Distribution.Service.Service;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace NeoErp.Distribution.Controllers.Api
{
    public class SetupController : ApiController
    {

        public ISetupService _setupService;
        private IWorkContext _workContext;

        public SetupController(ISetupService setupService, IWorkContext workContext)
        {
            this._setupService = setupService;
            this._workContext = workContext;
        }

        [HttpPost]
        public IEnumerable<DistributorListModel> GetDistributorList(filterOption model)
        {
            model = model ?? new filterOption();
            IEnumerable<DistributorListModel> reportData = this._setupService.GetDistributorList(model.ReportFilters, _workContext.CurrentUserinformation);
            return reportData;
        }
        
        [HttpPost]
        public IEnumerable<FarmListModel> GetFarmList(filterOption model)
        {
            model = model ?? new filterOption();
            IEnumerable<FarmListModel> reportData = this._setupService.GetFarmList(model.ReportFilters, _workContext.CurrentUserinformation);
            return reportData;
        }

        [HttpPost]
        public string UpdateDistributorOrder(OrderModel model)
        {
            return this._setupService.UpdateDistributorOrder(model);
        }


        [HttpPost]
        public HttpResponseMessage deleteDistributor(DistributorListModel model)
        {
            var result = _setupService.deleteDistributor(model, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Delete successfully", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong ! Please try again", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            }
        }
        [HttpGet]
        public IHttpActionResult all_customer(string searchTerm = "")
        {
            var js = _setupService.AllCustomer(searchTerm);
            return Ok(js);
        }
        [HttpGet]
        public IHttpActionResult get_form_setup()
        {
            var js = _setupService.GetFormSetup();
            return Ok(js);
        }
        [HttpGet]
        public IHttpActionResult get_item_tree()
        {
            var js = _setupService.GetItemTreeView();
            return Ok(js);
        }
        public class SaveFreeQuantityRequest
        {
            public List<FreeQuantityModel> Data { get; set; }
            public string CustomerCode { get; set; }
            public List<string> FormCodes { get; set; }
        }
        [HttpPost]
        public IHttpActionResult save_free_quantity([FromBody] SaveFreeQuantityRequest request)
        {
            try
            {
                if (request == null || request.FormCodes == null || !request.FormCodes.Any())
                {
                    return Json(new { success = false, message = "No form codes provided" });
                }

                // Get current user from session or context
                string createdBy = User?.Identity?.Name ?? "SYSTEM";

                int successCount = 0;
                int failCount = 0;
                string lastError = "";

                // Loop through each form code and save data
                foreach (var formCode in request.FormCodes)
                {
                    try
                    {
                        var result = _setupService.SaveFreeQuantity(request.Data, request.CustomerCode, formCode, createdBy);

                        // Check if result has success property
                        var resultDict = result as IDictionary<string, object>;
                        if (resultDict != null && resultDict.ContainsKey("success"))
                        {
                            bool success = (bool)resultDict["success"];
                            if (success)
                            {
                                successCount++;
                            }
                            else
                            {
                                failCount++;
                                lastError = resultDict.ContainsKey("message") ? resultDict["message"].ToString() : "Unknown error";
                            }
                        }
                        else
                        {
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        failCount++;
                        lastError = ex.Message;
                    }
                }

                if (successCount == request.FormCodes.Count)
                {
                    return Json(new { success = true, message = $"Data saved successfully to {successCount} form(s)" });
                }
                else if (successCount > 0)
                {
                    return Json(new { success = true, message = $"Data saved to {successCount} form(s). Failed: {failCount}. Last error: {lastError}" });
                }
                else
                {
                    return Json(new { success = false, message = $"Failed to save data. Error: {lastError}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IHttpActionResult load_free_quantity_data(string customerCode, string formCode)
        {
            var js = _setupService.LoadFreeQuantityData(customerCode, formCode);
            return Ok(js);
        }
        [HttpPost]
        public IHttpActionResult UploadExcel()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                if (httpRequest.Files.Count == 0)
                {
                    return Json(new { success = false, message = "No file uploaded" });
                }

                var file = httpRequest.Files[0];
                if (file == null || file.ContentLength == 0)
                {
                    return Json(new { success = false, message = "No file uploaded" });
                }

                // Validate file extension
                var extension = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (extension != ".xlsx" && extension != ".xls")
                {
                    return Json(new { success = false, message = "Invalid file format. Please upload .xlsx or .xls file" });
                }

                // Get current user from session or context
                string createdBy = User.Identity.Name ?? "SYSTEM";

                // Process the Excel file
                var result = _setupService.ProcessExcelUpload(file.InputStream, createdBy);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpPost]
        public HttpResponseMessage AddDistributor(DistributorListModel model)

        {
            var message = this._setupService.AddDistributor(model, _workContext.CurrentUserinformation);
            if (message == "409")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Distributor Already Exist!!", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            else if (message == "500")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            else if (message == "220")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "New Synergy Customer Added and Distributor Created successfully.", STATUS_CODE = (int)HttpStatusCode.OK });
            else if (message == "444")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Cannot Create Distributor !! Select Create Synergy Customer Button To add New Entry", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Distributor Created Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }


        [HttpPost]
        public HttpResponseMessage UpdateDistributor(DistributorListModel model)
        {
            var message = this._setupService.UpdateDistributor(model, _workContext.CurrentUserinformation);
            if (message == "500")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Distributor Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }



        [HttpPost]
        public HttpResponseMessage GetResellerList(filterOption model, string Source = "B",string status="")
        {
            var reportData = this._setupService.GetResellerList(Source, model.ReportFilters, _workContext.CurrentUserinformation,status);
            var inactive = _setupService.InactiveResellers(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, new { data = reportData, inActive = inactive });

        }

        [HttpPost]
        public IEnumerable<OtherEntity> getAllEntityList(filterOption model)
        {
            IEnumerable<OtherEntity> Entity = _setupService.getAllEntityList(model.ReportFilters, _workContext.CurrentUserinformation);
            return Entity;
        }


        [HttpPost]
        public HttpResponseMessage AddReseller(ResellerListModel model)
        {
            var message = this._setupService.AddReseller(model, _workContext.CurrentUserinformation);
            if (message == "409")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Reseller Already Exist!!", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            else if (message == "500")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Reseller Created Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpPost]
        public HttpResponseMessage AddOtherEntity(OtherEntity model)
        {
            var message = this._setupService.AddOtherEntity(model, _workContext.CurrentUserinformation);
            if (message == "409")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Entity Already Exist!!", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            else if (message == "500")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Entity Created Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }






        [HttpPost]
        public HttpResponseMessage UpdateReseller(ResellerListModel model)
        {
            var message = this._setupService.UpdateReseller(model, _workContext.CurrentUserinformation);
            if (message == "500")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Reseller Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpPost]
        public string UpdateEntity(OtherEntity model)
        {
            var data = _setupService.UpdateEntity(model, _workContext.CurrentUserinformation);
            return data;
        }


        [HttpPost]
        public HttpResponseMessage deleteReseller(ResellerListModel model)
        {
            var result = this._setupService.deleteReseller(model, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Delete Successfully", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong !! Please try again", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            }
        }
        [HttpPost]
        public string DeleteOtherEntity(OtherEntity model)
        {
            var data = _setupService.DeleteOtherEntity(model, _workContext.CurrentUserinformation);
            return data;
        }

        [HttpPost]
        public HttpResponseMessage deleteArea(ResellerListModel model)
        {
            var message = this._setupService.deleteArea(model, _workContext.CurrentUserinformation);
            if (message == "success")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Delete Successfully", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong ! Please try again", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpGet]
        public HttpResponseMessage GetReseller(string ResellerId)
        {
            var data = _setupService.GetReseller(ResellerId, _workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });
        }

        [HttpGet]
        public List<OtherEntity> GetIndividualEntity(string Code)
        {
            var data = _setupService.GetIndividualEntity(Code, _workContext.CurrentUserinformation);
            return data;
        }

        ////***************** Route Setup
        [HttpPost]
        public IEnumerable<RouteListModel> GetRouteList(filterOption model)
        {
            IEnumerable<RouteListModel> reportData = this._setupService.GetRouteList(model.ReportFilters, _workContext.CurrentUserinformation);
            return reportData;
        }

        [HttpPost]
        public IEnumerable<RouteListModel> GetBrandingRoutes(filterOption model)
        {
            var reportData = this._setupService.GetBrandingRouteList(model.ReportFilters, _workContext.CurrentUserinformation);
            return reportData;
        }

        [HttpPost]

        public HttpResponseMessage AddRoute(RouteListModel model)
        {
            var a = model;
            var message = this._setupService.AddRoute(model, _workContext.CurrentUserinformation);
            if (message == "200")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Route Created Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });

        }

        [HttpPost]
        public HttpResponseMessage UpdateRoute(RouteListModel model)
        {
            var message = this._setupService.UpdateRoute(model, _workContext.CurrentUserinformation);
            if (message == "200")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Route Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });

        }
        [HttpPost]
        public HttpResponseMessage deleteRoute(RouteListModel model)
        {
            var message = this._setupService.deleteRoute(model, _workContext.CurrentUserinformation);
            if (message == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Delete successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong ! Please try again", STATUS_CODE = (int)HttpStatusCode.OK });
            }
        }

        //Dealer part goes here....

        [HttpPost]
        public IEnumerable<DealerModal> GetDealerList(filterOption modal)
        {
            IEnumerable<DealerModal> reportData = this._setupService.GetDealerList(modal.ReportFilters, _workContext.CurrentUserinformation);
            return reportData;
        }

        [HttpPost]
        public HttpResponseMessage AddDealer(DealerModal modal)
        {
            var message = this._setupService.AddDealer(modal, _workContext.CurrentUserinformation);
            if (message == "Already Exist")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Dealer Already Exist!!", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            else if (message == "Error")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Dealer Created Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        [HttpPost]
        public HttpResponseMessage deleteDealer(DealerModal model)
        {
            var result = _setupService.deleteDealer(model, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Delete successfully", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong ! Please try again", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            }
        }

        [HttpPost]
        public HttpResponseMessage UpdateDealer(DealerModal modal)
        {
            var message = this._setupService.UpdateDealer(modal, _workContext.CurrentUserinformation);
            if (message == "500")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Dealer Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
        }

        //Area setup start from here..

        public IList<AreaModel> GetDistrictList()
        {
            return this._setupService.GetDistrictList();

        }
        public IList<AreaModel> GetvdcList(string DISTRICT_CODE)
        {
            return this._setupService.GetvdcList(DISTRICT_CODE);

        }
        public IList<AreaModel> GetZoneList(string DISTRICT_CODE)
        {
            return this._setupService.GetZoneList(DISTRICT_CODE);
        }
        public IList<AreaModel> GetRegionList(string DISTRICT_CODE)
        {
            return this._setupService.GetRegionList(DISTRICT_CODE);
        }

        public List<AreaModel> GetAllAreaList()
        {
            return this._setupService.GetAllAreaList();
        }

        [HttpPost]
        public HttpResponseMessage AddArea(AreaModel modal)
        {
            var message = this._setupService.AddArea(modal, _workContext.CurrentUserinformation);
            if (message == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Dealer Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            }

        }

        [HttpPost]
        public HttpResponseMessage UpdateArea(AreaModel modal)
        {
            var message = this._setupService.UpdateArea(modal, _workContext.CurrentUserinformation);
            if (message == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Area Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            }
        }

        //Question setup
        [HttpPost]
        public HttpResponseMessage AddGeneralQuestions(QuestionSetupModel model)
        {
            var result = this._setupService.AddGeneralQuestions(model, _workContext.CurrentUserinformation);
            if (result == "success")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Questions saved successfully", TYPE = "success", STATUS_CODE = (int)HttpStatusCode.OK });
            else if (result == "Answered")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Answered Questions cannot be updated/deleted", TYPE = "warning", STATUS_CODE = (int)HttpStatusCode.OK });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error !! Try again Later", TYPE = "error", STATUS_CODE = (int)HttpStatusCode.BadRequest });
        }

        [HttpPost]
        public HttpResponseMessage AddTabularQuestions(List<TabularModel> model)
        {
            var result = this._setupService.AddTabularQuestions(model, _workContext.CurrentUserinformation);
            if (result == "success")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Questions saved successfully", TYPE = "success", STATUS_CODE = (int)HttpStatusCode.OK });
            else if (result == "Answered")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Answered Questions cannot be updated/deleted", TYPE = "warning", STATUS_CODE = (int)HttpStatusCode.OK });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error !!! Try again Later", TYPE = "error", STATUS_CODE = (int)HttpStatusCode.BadRequest });
        }

        [HttpGet]
        public HttpResponseMessage GetAllquestions()
        {
            var data = _setupService.GetAllQuestionSets(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpPost]
        public HttpResponseMessage SaveSurvey(SurveyModel model)
        {
            var result = _setupService.SaveSurvey(model, _workContext.CurrentUserinformation);
            var message = model.SURVEY_CODE > 0 ? "Survey updated successfully" : "Survey created successfully";
            if (result == "200")
                return Request.CreateResponse(HttpStatusCode.OK, new { TYPE = "success", MESSAGE = message });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { TYPE = "error", MESSAGE = "Something went wrong. Please try again" });
        }

        [HttpGet]
        public HttpResponseMessage GetSurveyList()
        {
            var data = _setupService.GetSurveyList(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        public List<OutletModel> getAllOutletList()
        {
            var data = _setupService.getAllOutletList();
            return data;
        }
        [HttpPost]

        public HttpResponseMessage AddOutlet(OutletModel modal)
        {
            var message = this._setupService.AddOutlet(modal, _workContext.CurrentUserinformation);
            if (message == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Insert Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }

            else if (message == "update")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Updated Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.BadRequest });
            }

        }
        [HttpPost]
        public HttpResponseMessage deleteItem(OutletModel modal)
        {
            var message = this._setupService.deleteItem(modal, _workContext.CurrentUserinformation);
            if (message == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something Wrong !! Try again Later", STATUS_CODE = (int)HttpStatusCode.OK });
            }
        }

        [HttpGet]
        public List<OutletSubtypeModel> getAllSubOutletList(int TYPE_ID)
        {
            var data = _setupService.getAllSubOutletList(TYPE_ID, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpGet]
        public List<OutletSubtypeModel> getAllSubOutletList()
        {
            var data = _setupService.getAllSubOutletList(_workContext.CurrentUserinformation);
            return data;
        }
        public HttpResponseMessage AddResellerGroup(ResellerGroupModel model)
        {
            try
            {
                var result = _setupService.AddResellerGroup(model, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Group added successfully", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
            }
        }

        [HttpPost]
        public HttpResponseMessage deleteGroup(ResellerGroupModel modal)
        {
            var result = _setupService.deleteGroup(modal, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "SomeThing went wrong", STATUS_CODE = (int)HttpStatusCode.OK });
            }
        }

        public HttpResponseMessage DeleteResellerGroup(int GroupId)
        {
            try
            {
                var result = _setupService.DeleteResellerGroup(GroupId, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Group deleted successfully", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
            }
        }
        public List<ResellerGroupModel> getAllResellerGroups()
        {
            var data = _setupService.GetResellerGroups(_workContext.CurrentUserinformation);
            return data;
        }

        public List<DistUserEmployeeModel> getLoginEmployee()
        {
            var data = _setupService.getLoginEmployee(_workContext.CurrentUserinformation);
            return data;
        }
        public List<DistUserEmployeeModel> getDistLoginEmployee()
        {
            var data = _setupService.getDistLoginEmployee(_workContext.CurrentUserinformation);
            return data;
        }
        public QuestionListModel GetQuestions()
        {
            var data = this._setupService.GetAllQuestions(this._workContext.CurrentUserinformation);
            return data;
        }
        [HttpGet]
        public QuestionSetupModel GeneralBySetId(string setId)
        {
            var data = this._setupService.GetGeneralBySetID(setId, this._workContext.CurrentUserinformation);
            return data;
        }
        [HttpGet]
        public TabularModel TabularBySetId(string setId)
        {
            var data = this._setupService.GetTabularBySetID(setId, this._workContext.CurrentUserinformation);
            return data;
        }
        [HttpPost]
        public HttpResponseMessage AddCategoryImage(ImageCategoryModel model)
        {
            try
            {
                var result = _setupService.AddImageCategory(model, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Image added successfully", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage deleteImage(ImageCategoryModel model)
        {
            var result = _setupService.deleteImage(model, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong! Please try again", STATUS_CODE = (int)HttpStatusCode.OK });
            }
        }

        //[HttpPost]
        //public HttpResponseMessage CreateClosingStock(ClosingStock model)
        //{
        //    var result = _setupService.CreateClosingStock(model, _workContext.CurrentUserinformation);
        //    if (result == "200")
        //    {
        //        return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Saved Successfully", Type = "success" });
        //    }
        //    else
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
        //    }
        //}
        //public HttpResponseMessage DeleteCategoryImage(int ImageId)
        //{
        //    try
        //    {
        //        var result = _setupService.DeleteCategoryImage(ImageId, _workContext.CurrentUserinformation);
        //        return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Image deleted successfully", TYPE = "success" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
        //    }
        //}
        public List<ImageCategoryModel> getAllCategoryImage()
        {
            var data = _setupService.GetCategoryImage(_workContext.CurrentUserinformation);
            return data;
        }


        public List<UserSetupTreeModel> GetUserSetupTreeList()
        {
            var data = _setupService.GetUserSetupTreeList(_workContext.CurrentUserinformation);
            return data;
        }



        public HttpResponseMessage SaveUserTree(UserSetupTreeModel model)
        {
            var Status = _setupService.SaveUserTree(model, _workContext.CurrentUserinformation);
            if (Status.Trim().Equals("200"))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("User Created Sucessfully."), STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else if (Status == "EXISTS")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Employee is already a user."), STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = Status, STATUS_CODE = (int)HttpStatusCode.ExpectationFailed });
        }

        public string UpdateUserTree(UserSetupTreeModel model)
        {
            return _setupService.UpdateUserTree(model, _workContext.CurrentUserinformation);
        }

        [HttpPost]
        public string UpdateUserTreeOrder(UserSetupTreeModel model)
        {
            return _setupService.UpdateUserTreeOrder(model);
        }
        [HttpGet]
        public string DeleteUserTree(string Code)
        {
            return _setupService.DeleteUserTree(Code, _workContext.CurrentUserinformation);
        }

        [HttpGet]
        public List<UserRoleModel> GetDistUserRole()
        {
            return _setupService.GetDistUserRole(_workContext.CurrentUserinformation);
        }


        [HttpPost]
        public HttpResponseMessage CreateClosingStock(ClosingStock model)
        {
            var result = _setupService.CreateClosingStock(model, _workContext.CurrentUserinformation);
            if (result == "200")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Saved Successfully", Type = "success" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage UpdateClosingStock(ClosingStock model)
        {
            var result = _setupService.UpdateClosingStock(model, _workContext.CurrentUserinformation);
            if (result == "200")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "Updated Successfully", Type = "success" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, new { MESSAGE = "Error processing request", TYPE = "error" });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetClosingStock(string DistId = "")
        {
            try
            {
                var data = _setupService.GetClosingStock(_workContext.CurrentUserinformation, DistId);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage SaveOpeningStock(OpeningDetailModel model)
        {
            var status = _setupService.SaveOpeningStockSetup(model, _workContext.CurrentUserinformation);
            if (status == "ADDED")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Opening stock successfully saved", TYPE = "success" });
            else if (status == "UPDATED")
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Opening stock successfully Updated", TYPE = "success" });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
        }
        [HttpGet]
        public HttpResponseMessage GetOpeningStock()
        {
            try
            {
                var data = _setupService.GetOpeningStock(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
            }
        }
        [HttpPost]
        public List<itemList> GetDistChildItems(string distCode)
        {
            var data = _setupService.GetDistChildItems(distCode, _workContext.CurrentUserinformation);
            return data;
        }
        [HttpGet]
        public List<ResellerItemRate> GetResellerItems()
        {
            return _setupService.GetResellerItems(_workContext.CurrentUserinformation);
        }
        [HttpPost]
        public HttpResponseMessage UpdateResellerItemRate(ResellerItemRate model)
        {
            var data = _setupService.UpdateResellerItemRate(model, _workContext.CurrentUserinformation);
            if (data == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Rate Saved Successfully", TYPE = "success" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
            }
        }

        public List<DistQueryBuilderModel> GetDistributorList()
        {
            var data = _setupService.GetDistributorList();
            return data;
        }

        [HttpPost]
        public HttpResponseMessage SaveWidgets(CreateDistWidgetsModel modal)
        {
            var data = _setupService.SaveWidgets(modal, _workContext.CurrentUserinformation);
            if (data == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Widgets Saved Successfully", TYPE = "success" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong. Please try again.", TYPE = "error" });
            }
        }

        public List<DistTableList> GetDistTableList()
        {
            var data = _setupService.GetDistTableList();
            return data;
        }

        public List<DistTableColumn> GetColumnNameList(string tablesName)
        {
            var data = _setupService.GetColumnNameList(tablesName);
            return data;
        }

        [HttpGet]
        public HttpResponseMessage GetNotifications()
        {
            try
            {
                var data = _setupService.GetAllNotifications(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage SaveNotification(NotificationModel model)
        {
            try
            {
                var data = _setupService.SaveNotification(model, _workContext.CurrentUserinformation);

                if (data == "200")
                {
                    var message = "Successful";
                    var result = FireBaseMessage(model);
                    if (result.Successful)
                        message += " and notification sent";
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = message, TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }
        [HttpGet]
        public HttpResponseMessage DeleteNotification(string Id)
        {
            try
            {
                var data = _setupService.DeleteNotification(Id);
                if (data > 0)
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted successfully", TYPE = "success" });
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }
        [NonAction]
        private FCMResult FireBaseMessage(NotificationModel model)
        {
            var senderId = "53456668796";
            var serverApiKey = "BBy41I3uQkS5hUaZPniQSlsSvjpYNnCACbElTB2mbkzlcb6lwufg7VOXTjcwexfpTxMPHHUAF5Qbv--OJfTsPo8";
            //var senderId = "272419356545";
            //var serverApiKey = "AAAAP211f4E:APA91bEJx1ChuXST92pTMkppvdlX8EtJNX6VENrWt4rdBvhPGevZVPhBjZ9TrcHZRsi1xg1UpqJB5Tg_wPXyOqXlw7Umjqrn8_7DlJJ9tqy-PUeBLCQZ4uCFs0qjJNJLA0MP5BSoQDZh";

            var result = new FCMResult();
            try
            {
                var deviceIds = _setupService.GetAllFCMDevices(model.SP_CODES); //get the deviceIds from sp_codes
                result.Successful = false;
                result.Error = null;

                //data according to the format provided by firebase
                var data = new
                {
                    registration_ids = deviceIds[0], //multiple device ids as array
                    data = new
                    {
                        body = model.NOTIFICATION_TEXT,
                        title = model.NOTIFICATION_TITLE,
                        id = model.NOTIFICATION_ID
                    }
                };

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(data);
                Byte[] byteArray = Encoding.UTF8.GetBytes(json);
                tRequest.Headers.Add(string.Format("Authorization: key={0}", serverApiKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);

                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                String sResponseFromServer = tReader.ReadToEnd();
                                result.Response = sResponseFromServer;
                                var token = Newtonsoft.Json.Linq.JToken.Parse(result.Response);
                                if (token["success"].ToString() != "0")
                                    result.Successful = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                result.Successful = false;
                result.Response = null;
                result.Error = ex;
            }
            return result;
        }

        //[NonAction]
        //private FCMResult FireBaseMessage(NotificationModel model)
        //{
        //    var result = new FCMResult();
        //    var unsentTokens = new List<string>();
        //        var deviceIds = _setupService.GetAllFCMDevices(model.SP_CODES);
        //        result.Successful = false;
        //        result.Error = null;
        //        string UploadPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"Areas\NeoErp.Distribution\Content";
        //        string fileUrl = System.IO.Path.Combine(UploadPath, "FirebaseConfig.json");

        //        // Check if file exists
        //        if (!System.IO.File.Exists(fileUrl))
        //        {
        //            throw new FileNotFoundException("Firebase configuration file not found at: " + fileUrl);
        //        }

        //        if (FirebaseApp.DefaultInstance == null)
        //        {
        //            FirebaseApp.Create(new AppOptions
        //            {
        //                Credential = GoogleCredential.FromFile(fileUrl)
        //            });
        //        }
        //        if (model.SP_CODES.Count == 0)
        //        {
        //            // Batch processing for tokens (100 per batch)
        //            const int batchSize = 100;
        //            for (int i = 0; i < deviceIds.Count; i += batchSize)
        //            {
        //                var batchTokens = deviceIds.Skip(i).Take(batchSize).ToList();

        //                foreach (var token in batchTokens)
        //                {
        //                    try
        //                    {
        //                        // Create a message for each token
        //                        var message = new Message()
        //                        {
        //                            Token = token,
        //                            Notification = new Notification
        //                            {
        //                                Title = model.NOTIFICATION_TITLE,
        //                                Body = model.NOTIFICATION_TEXT
        //                            }
        //                        };

        //                        // Synchronous send
        //                        var response = FirebaseMessaging.DefaultInstance.SendAsync(message).Result;
        //                        result.Successful = false;
        //                    result.Response = "Broadcast completed.";
        //                }
        //                catch (Exception ex)
        //                    {
        //                        unsentTokens.Add(token);
        //                        result.Successful = false;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (var token in deviceIds)
        //            {
        //                try
        //                {
        //                    var message = new Message()
        //                    {
        //                        Token = token,
        //                        Notification = new Notification
        //                        {
        //                            Title = model.NOTIFICATION_TITLE,
        //                            Body = model.NOTIFICATION_TEXT
        //                        }
        //                    };

        //                    var response = FirebaseMessaging.DefaultInstance.SendAsync(message);
        //                result.Successful = true;
        //            }
        //            catch (Exception ex)
        //                {
        //                    unsentTokens.Add(token);
        //                    result.Successful = false;
        //                }
        //            }
        //        }
        //    // Log unsent tokens
        //    if (unsentTokens.Any())
        //    {
        //        SaveUnsentMessagesLog(unsentTokens, model);
        //    }
        //    return result;
        //}
        //private void SaveUnsentMessagesLog(List<string> unsentMessages, NotificationModel model)
        //{
        //    try
        //    {
        //        string logFilePath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + @"Log\NotificationMessagesLog.txt";
        //        if (!System.IO.Directory.Exists(System.IO.Path.GetDirectoryName(logFilePath)))
        //        {
        //            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(logFilePath));
        //        }

        //        using (var writer = new StreamWriter(logFilePath, true))
        //        {
        //            writer.WriteLine($"Date: {DateTime.Now}");
        //            writer.WriteLine($"Notification Title: {model.NOTIFICATION_TITLE}");
        //            writer.WriteLine($"Notification Body: {model.NOTIFICATION_TEXT}");
        //            writer.WriteLine("Unsent Tokens:");
        //            foreach (var token in unsentMessages)
        //            {
        //                writer.WriteLine(token);
        //            }
        //            writer.WriteLine(new string('-', 50));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error while logging unsent messages: {ex.Message}");
        //    }
        //}
        //For EmployeeSetup

        //public HttpResponseMessage GetEmployeeSetup()
        //{
        //    var data = _setupService.GetEmployeeSetup(_workContext.CurrentUserinformation);
        //    return Request.CreateResponse(HttpStatusCode.OK, data);
        //}



        //Get
        [HttpGet]
        public HttpResponseMessage GetCompItem()
        {
            var data = _setupService.GetCompItem(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [HttpGet]
        public List<UserSetupModel> GetUserList()
        {
            List<UserSetupModel> data = this._setupService.GetUserList(_workContext.CurrentUserinformation);
            return data;
        }

        [HttpGet]
        public List<UserAccessModel2> GetUserAcccessList(string id)
        {
            List<UserAccessModel2> data = this._setupService.GetUserAcccessList(_workContext.CurrentUserinformation, id);
            return data;
        }
        [HttpPost]
        public HttpResponseMessage setUserAccess(List<UserAccessModel2> models)
        {
            var result = _setupService.setUserAccess(_workContext.CurrentUserinformation, models);
            if (result)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Updated successfully", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong ! Please try again", STATUS_CODE = (int)HttpStatusCode.Ambiguous });
            }
        }
        //EmployeeSetup
        [HttpGet]
        public HttpResponseMessage GetCompany()
        {
            try
            {
                var data = _setupService.GetCompany(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<string>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetBranch(string COMPANY_CODE = "")
        {
            try
            {
                var data = _setupService.GetBranch(COMPANY_CODE, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<string>());
            }
        }

        //for grid display
        [HttpGet]
        public HttpResponseMessage GetEmployee()
        {
            try
            {
                var data = _setupService.GetEmployee(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<string>());
            }
        }

        //Create Employee
        [HttpPost]
        public HttpResponseMessage CreateEmployee(EmployeeSetupModel model)
        {
            try
            {
                var data = _setupService.CreateEmployee(model, _workContext.CurrentUserinformation);

                if (data == "200")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Successfully created", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }

        }

        //Delete Employee
        [HttpGet]
        public HttpResponseMessage DeleteEmployee(int Id)
        {
            var result = _setupService.DeleteEmployee(Id, _workContext.CurrentUserinformation);
            if (result == "200")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted Successfully", TYPE = "success" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong! Please try again", TYPE = "success" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCompCategories()
        {
            try
            {
                var data = _setupService.GetCompCategories(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<string>());
            }
        }

        //Create
        [HttpPost]
        public HttpResponseMessage CreateCompItem(CompItemSetupModel model)
        {
            try
            {
                var data = _setupService.CreateCompItem(model, _workContext.CurrentUserinformation);

                if (data == "200")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Successfully created", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }

        }

        [HttpGet]
        public HttpResponseMessage DeleteCompItem(int Id)
        {
            var result = _setupService.DeleteCompItem(Id, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted Successfully", TYPE = "success" });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong! Please try again", TYPE = "success" });
            }
        }
        [HttpGet]
        public HttpResponseMessage GetCompItems()
        {
            try
            {
                var data = _setupService.GetAllCompItems(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<ItemModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCompMap()
        {
            try
            {
                var data = _setupService.GetCompItemMap(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveCompItemMap(List<CompItemModel> model)
        {
            try
            {
                var data = _setupService.SaveCompItemMap(model, _workContext.CurrentUserinformation);
                if (data == "200")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Saved successfully", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCompFields()
        {
            try
            {
                var data = _setupService.GetCompFields(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveCompField(CompetitorItemFields model)
        {
            try
            {
                var data = _setupService.SaveCompFields(model, _workContext.CurrentUserinformation);
                if (data == "200")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Saved successfully", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }

        [HttpGet]
        public HttpResponseMessage DefaultCompFileds()
        {
            try
            {
                var data = _setupService.DefaultCompFileds(_workContext.CurrentUserinformation);
                if (data == "200")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Saved successfully", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }

        public HttpResponseMessage GetGroupMap()
        {
            try
            {
                var data = _setupService.GetGroupMap(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }
        public HttpResponseMessage GetDistUserMap()
        {
            try
            {
                var data = _setupService.GetDistUserMap(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage SaveGroupMap(List<GroupMapModel> model)
        {
            try
            {
                var data = _setupService.SaveGroupMap(model, _workContext.CurrentUserinformation);
                if (data == "200")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Saved successfully", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
            }
        }

        [HttpPost]
        public HttpResponseMessage SaveDistUserMap(List<UserMapModel> model)
        {
            try
            {
                var data = _setupService.SaveDistUserMap(model, _workContext.CurrentUserinformation);
                if (data == "Success")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Saved successfully", TYPE = "success" });
                }
                else
                    throw new Exception();
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = ex.Message.ToString(), TYPE = "error" });
            }
        }
        [HttpPost]
        public HttpResponseMessage deleteUserMap(UserMapModel modal)
        {
            var result = _setupService.deleteUser(modal, _workContext.CurrentUserinformation);
            if (result == "success")
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Deleted Successfully", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "SomeThing went wrong", STATUS_CODE = (int)HttpStatusCode.OK });
            }
        }

        [HttpGet]
        public HttpResponseMessage GetHeaderNotifications()
        {
            var data = new Dictionary<string, string>();
            var inactive = _setupService.InactiveResellers(_workContext.CurrentUserinformation);
            if (inactive > 0)
                data.Add("/Distribution/Home/Dashboard#!Distribution/ResellerSetup?status=inactive", inactive + " new Outlets are waiting for activation");
            return Request.CreateResponse(HttpStatusCode.OK, data.ToList());
        }

        [HttpGet]
        public HttpResponseMessage GetMuList()
        {
            try
            {
                var data = _setupService.GetMuCodes();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<CustomerModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetCategoryList()
        {
            try
            {
                var data = _setupService.GetCategoryCodes();
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<CustomerModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetItems()
        {
            var data = _setupService.GetItemsList(_workContext.CurrentUserinformation);
            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        /*[HttpPost]
        public HttpResponseMessage SaveItem(ItemModel model)
        {
            var data = _setupService.SaveItem(model, _workContext.CurrentUserinformation);
            if (data == 200)
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Item successfully created", TYPE = "success" });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
        }*/

        [HttpPost]
        public HttpResponseMessage SaveItem()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            HttpPostedFile file = files["file"];
            string uniqueFileName = "";

            if (file != null)
            {
                string originalFileName = Path.GetFileName(file.FileName);
                string extension = Path.GetExtension(originalFileName).ToLowerInvariant();

                if (!new[] { ".jpg", ".jpeg", ".png" }.Contains(extension))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Invalid file format.", TYPE = "error" });
                }

                uniqueFileName = $"{Guid.NewGuid()}{extension}";
                string folderPath = System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath + "Areas\\NeoErp.Distribution\\Images\\ItemImages";

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string fullFilePath = Path.Combine(folderPath, uniqueFileName);
                file.SaveAs(fullFilePath);

                // Parse model from Request.Form
                
            }
                var modelJson = HttpContext.Current.Request.Form["model"];
                if (string.IsNullOrWhiteSpace(modelJson))
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Invalid data", TYPE = "error" });
                }

                var model = JsonConvert.DeserializeObject<ItemModel>(modelJson);
                model.IMAGE_FILE_NAME = uniqueFileName;
            

            var data = _setupService.SaveItem(model, _workContext.CurrentUserinformation);
            if (data == 200)
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Item successfully created", TYPE = "success" });
            else
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Something went wrong", TYPE = "error" });
        }


        [HttpGet]
        public List<WEB_QUESTION_ASNWER> GetWebQAList()
        {
            return _setupService.GetWebQuestionAnswerList(_workContext.CurrentUserinformation);
        }

        [HttpPost]
        public List<ResellerDistributorModel> GetDistributorResellerList()
        {
            try
            {
                var result = this._setupService.GetDistributorResellerList(_workContext.CurrentUserinformation);
                return result;
            }
            catch (Exception ex)
            {
                throw ex; 
            }

        }
        [HttpPost]
        public List<DDL_TEMPLATE> GetMessageTemplateList()
        {
            try
            {
                var result = this._setupService.GetMessageTemplateList(_workContext.CurrentUserinformation);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        [HttpPost]
        public string GetTemplates(string name)
        {
            var template = string.Empty;
            try
            {
                var path = HttpContext.Current.Server.MapPath($@"~/App_Data/SchedularMailTemplae/{name}");
                if (System.IO.File.Exists(path))
                {
                    using (System.IO.StreamReader readtext = new System.IO.StreamReader(path))
                    {
                        template = readtext.ReadLine();
                        template = EncodeDecodeHtml.HtmDecode(template);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return template;
        }
        [HttpGet]
        public HttpResponseMessage GetCustomerByDealer()
        {
            try
            {
                var data = _setupService.GetCustomerByDealer(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<CustomerModel>());
            }
        }

        [HttpPost]
        public string AddScheme(SchemeModel model)
        {
            try
            {
                var response = _setupService.AddScheme(model, _workContext.CurrentUserinformation);
                return response;
             }
            catch(Exception ex)
            {
                return "Error";
            }
        }

        [HttpGet]
        public HttpResponseMessage GetAllScheme()
        {
            try
            {
                var data = _setupService.GetAllScheme(_workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, data);
            }
            catch(Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetSchemeItem(string SchemeID)
        {
            try
            {
                var data = _setupService.GetSchemeItem(SchemeID, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetGiftItem(string SchemeID)
        {
            try
            {
                var data = _setupService.GetGiftItem(SchemeID, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOtherItem(string SchemeID)
        {
            try
            {
                var data = _setupService.GetOtherItem(SchemeID, _workContext.CurrentUserinformation);
                return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }

        [HttpGet]
        public HttpResponseMessage DeleteScheme(string SchemeID)
        {
            try
            {
                var data = _setupService.DeleteScheme(SchemeID);
                return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }

        //Animesh
        [HttpGet]
        public List<EmployeeAreaModel> GetEmployeesandRoute()
        {
            try
            {
                var data = _setupService.GetEmployeesandRoute(_workContext.CurrentUserinformation);
                return data;
                //return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });

            }
            catch (Exception ex)
            {
                return new List<EmployeeAreaModel>();
               // return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }
        
        [HttpGet]
        public List<AreaModel> GetSchemeArea(string SchemeID)
        {
            try
            {
                var data = _setupService.GetSchemeArea(SchemeID);
                return data;
                //return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });

            }
            catch (Exception ex)
            {
                return new List<AreaModel>();
               // return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }
        
        
        [HttpPost]
        public bool ApproveScheme(string SchemeID, string Action)
        {
            try
            {
                var data = _setupService.ApproveScheme(SchemeID, Action);
                return data;
                //return Request.CreateResponse(HttpStatusCode.OK, new { DATA = data, MESSAGE = "Success" });

            }
            catch (Exception ex)
            {
                return false;
               // return Request.CreateResponse(HttpStatusCode.OK, new List<SchemeModel>());
            }
        }


    }
}