using NeoERP.ProjectManagement.Service.Models;
using NeoERP.ProjectManagement.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using NeoErp.Core;
using NeoErp.Core.Caching;
using System.Data.SqlClient;
using NeoErp.Core.Models;
using NeoErp.Core.Models.Log4NetLoggin;
using System.Web.Http;
using NeoErp.Data;

namespace NeoERP.ProjectManagement.Controllers.Api
{
    public class ProjectApiController : ApiController
    {
        private IFormProjectRepo _formProjectRepo;
        private IDbContext _dbContext;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private readonly ILogErp _logErp;
        private DefaultValueForLog _defaultValueForLog;
        public ProjectApiController(IFormProjectRepo _iformProjectRepo, IDbContext dbContext, IWorkContext workContext, ICacheManager cacheManager)
        {
            this._formProjectRepo = _iformProjectRepo;
            this._dbContext = dbContext;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._defaultValueForLog = new DefaultValueForLog(this._workContext);
            this._logErp = new LogErp(this, _defaultValueForLog.LogUser, _defaultValueForLog.LogCompany, _defaultValueForLog.LogBranch, _defaultValueForLog.LogTypeCode, _defaultValueForLog.LogModule, _defaultValueForLog.FormCode);
        }


        [HttpPost]
        public HttpResponseMessage SaveProjectFormData(Project projectData)
        {
            try
            {
                bool isPosted = _formProjectRepo.InsertProjectData(projectData);
                if (isPosted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Project created successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to create project!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        public List<ProjectCount> ListAllProjects()
        {
            List<ProjectCount> response = new List<ProjectCount>();
            try
            {
                response = _formProjectRepo.ListAllProjects();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        public List<Project> GetProjectById(int id)
        {
            List<Project> response = new List<Project>();
            try
            {
                response = _formProjectRepo.GetProjectById(id);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpPost]
        public HttpResponseMessage DeleteProjectById(int id)
        {
            try
            {
                bool isDeleted = _formProjectRepo.DeleteProjects(id);
                if (isDeleted)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = "Project deleted successfully!!", STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    // Handle case where project was not posted successfully
                    return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Failed to delete project!!", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occurred during the operation
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
        [HttpGet]

        public List<ProjectCount> GetProjectCount()
        {
            List<ProjectCount> response = new List<ProjectCount>();
            try
            {
                response = _formProjectRepo.GetProjectCount();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]

        public List<ProjectCount> CountBarGraph()
        {
            List<ProjectCount> response = new List<ProjectCount>();
            try
            {
                response = _formProjectRepo.CountBarGraph();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }

        [HttpGet]
        public List<SubProjectData> GetCompletedProject()
        {
            List<SubProjectData> response = new List<SubProjectData>();
            try
            {
                response = _formProjectRepo.GetCompletedProject();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        [HttpGet]
        public List<SubProjectData> GetStartedProject()
        {
            List<SubProjectData> response = new List<SubProjectData>();
            try
            {
                response = _formProjectRepo.GetStartedProject();
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }
        }
        //[HttpGet]
        //public List<SupplierModel> GetAllSupplier()
        //{
        //    try
        //    {
        //        var supplier = _formProjectRepo.GetAllSupplier();
        //        return supplier;
        //    }
        //    catch (Exception ex)
        //    {

        //        throw new Exception(ex.StackTrace);
        //    }
        //}
        //[HttpGet]
        //public List<PriceSetupModel> GetAllItemWithName()
        //{
        //    List<PriceSetupModel> itemModel = new List<PriceSetupModel>();
        //    try
        //    {
        //        itemModel = _formProjectRepo.ListAllItemWithName();
        //        return itemModel;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.StackTrace);
        //    }
        //}
        [HttpGet]
        public List<EntryReport> GetConsumptionDetails(string formCode, string docVer = "All")
        {
            List<EntryReport> response = new List<EntryReport>();
            try
            {
                response = _formProjectRepo.GetConsumptionTransDetailByFormCode(formCode, docVer);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }

        }

        [HttpGet]
        public List<EntryReport> GetRequisitionDetails(string formCode, string docVer = "All")
        {
            List<EntryReport> response = new List<EntryReport>();
            try
            {
                response = _formProjectRepo.GetRequisitionTransDetailByFormCode(formCode, docVer);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }

        }

        [HttpGet]
        public List<EntryReport> GetRequisitionPendingDetails(string formCode, string docVer = "All")
        {
            List<EntryReport> response = new List<EntryReport>();
            try
            {
                response = _formProjectRepo.GetRequisitionPendingTransByFormCode(formCode, docVer);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }

        }
        public List<EntryReport> GetPurchaseDetails(string formCode, string docVer = "All")
        {
            List<EntryReport> response = new List<EntryReport>();
            try
            {
                response = _formProjectRepo.GetPurchaseTransByFormCode(formCode, docVer);
                return response;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.StackTrace);
            }

        }
    }
}