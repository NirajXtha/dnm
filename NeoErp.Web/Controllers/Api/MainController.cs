using NeoErp.Models;
using NeoErp.Services;
using NeoErp.Services.UserService;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace NeoErp.Controllers.Api
{
    public class MainController : ApiController
    {

        private IUserChartPermission _userChartPermisson;
        private IUserService _userService;
        public MainController(IUserChartPermission userChartPermission, IUserService userService)
        {
            this._userChartPermisson = userChartPermission;
            this._userService = userService;
        }

        [HttpGet]
        public HttpResponseMessage CreateEntities(string systemName)
        {
            try
            {
                var Count = _userChartPermisson.createEntities(systemName);
                return Request.CreateResponse(HttpStatusCode.OK, new { COUNT = Math.Abs(Count), MESSAGE = $"{Math.Abs(Count)} Entities successfully created", TYPE = "success" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Error occured while processing the request - " + ex.Message, TYPE = "error" });
            }
        }
       
        public string ChangeUserPassword(ChangePasswordViewModel model)
        {
            return this._userService.ChangeUserPassword(model);
        }
        [HttpPost]
        public string InsertChangeUserPassword(UserViewModel model)
        {
            return this._userService.InsertChangeUserPassword(model);
        }

        [HttpGet]
        public List<UserViewModel> GetAllUserDetails(UserViewModel model)
        {
            return this._userService.GetAllUserDetails(model);
        }
        [HttpGet]
        public UserViewModel GetUserDetailsByUserNo(int userno)
        {
            return this._userService.GetUserDetailsByUserNo(userno);
        }
        [HttpGet]
        public List<UserViewModel> GetAllUserType()
        {
            return this._userService.GetAllUserType();
        }

        #region User Setup

        [HttpGet]
       
        public List<AddUserModel> GetUserList()
        {

            var result = _userService.GetUserList();
            return result;

        }


        [HttpGet]
       
        public List<CompanySetupModel> getAllCompany()
        {

            var result = _userService.getAllCompany();
            return result;

        }

        [HttpGet]
       
        public List<EmployeeModel1> getAllEmployeeList()
        {

            var result = _userService.getAllEmployeeList();
            return result;

        }

        [HttpPost]
       
        public HttpResponseMessage createUser(AddUserModel model)
        {
            var a = model;
            var message = this._userService.createUser(model);
            if (message == "200")
            {

                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "INSERTED", STATUS_CODE = (int)HttpStatusCode.OK });
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        //UPDATE VEHICLE
        [HttpPost]
        public HttpResponseMessage updateUser(AddUserModel model)
        {
            try
            {
                var result = this._userService.updateUser(model);
                if (result == "UPDATED")
                {

                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                { return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "ERROR", STATUS_CODE = (int)HttpStatusCode.InternalServerError }); }

            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }


        [HttpPost]
        public HttpResponseMessage DeleteUserFromDb(string usercode)
        {
            try
            {
                var result = _userService.DeleteUserFromDb(usercode);


                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = result, STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        #endregion



    }
}
