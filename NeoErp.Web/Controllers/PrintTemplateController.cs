using NeoErp.Core.Helpers;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Models.CustomModels.SettingsEntities;
using NeoErp.Core.Services.TemplateMappingServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Controllers
{
    public class PrintTemplateController : Controller
    {
        private ITemplateMappingService _templateMappingService;

        public PrintTemplateController(ITemplateMappingService templateMappingService)
        {
            this._templateMappingService = templateMappingService;
        }

        // GET: PrintTemplate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetAllMappedFormTemplate(filterOption filterOptions)
        {
            var mappedTemplate = _templateMappingService.GetAllMappedFormTemplate(filterOptions.ReportFilters).ToList();
            var jsonResult = Json(mappedTemplate, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult GetAllFormWithCode()
        {
            var formWithCode = _templateMappingService.GetAllFormWithCode();
            var formResponse = Json(formWithCode, JsonRequestBehavior.AllowGet);
            return formResponse;
        }

        public ActionResult GetAllTemplateWithCode()
        {
            var availableTemplate = _templateMappingService.GetAllTemplateWithCode();
            var templateResponse = Json(availableTemplate, JsonRequestBehavior.AllowGet);
            return templateResponse;
        }

        public ActionResult GetAllUserWithCompanyCode()
        {
            var allUserWithCompany = _templateMappingService.GetUsersWithCompany();
            var userResponse = Json(allUserWithCompany, JsonRequestBehavior.AllowGet);
            return userResponse;
        }

        public ActionResult AddMappedTemplate(MappedFormTemplateModel controlEntities)
        {
            var mapToAdd = _templateMappingService.AddSingleMappedTemplate(controlEntities);
            var addedResponse = Json(mapToAdd, JsonRequestBehavior.AllowGet);
            return addedResponse;
        }

        public ActionResult AddBulkMappedTemplate(MappedFormTemplateModel mappedTemplate)
        {
            var bulkToAdd = _templateMappingService.AddBulkMappedTemplate(mappedTemplate);
            var bulkResponse = Json(bulkToAdd, JsonRequestBehavior.AllowGet);
            return bulkResponse;
        }

        public ActionResult DeleteTemplateMapping(string formCode,string templateName)
        {
            var deletedRecord = _templateMappingService.DeleteTemplateMapping(formCode,templateName);
            var deleteResponse = Json(deletedRecord, JsonRequestBehavior.AllowGet);
            return deleteResponse;
        }

        public ActionResult GetMappedTemplateForEdit(string formCode,string templateName)
        {
            var recordToEdit = _templateMappingService.GetMappedTemplateForEdit(formCode, templateName);
            var editResponse = Json(recordToEdit, JsonRequestBehavior.AllowGet);
            return editResponse;
        }

        public ActionResult UpdateMappedTemplate(MappedFormTemplateModel updateTemplateModel)
        {
            var editedRow = _templateMappingService.UpdateMappedTemplate(updateTemplateModel);
            var editedResponse = Json(editedRow, JsonRequestBehavior.AllowGet);
            return editedResponse;
        }
    }
}