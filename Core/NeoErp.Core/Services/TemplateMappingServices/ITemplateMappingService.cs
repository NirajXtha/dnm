using NeoErp.Core.Models;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Models.CustomModels.SettingsEntities;
using System.Collections.Generic;

namespace NeoErp.Core.Services.TemplateMappingServices
{
    public interface ITemplateMappingService
    {
        List<MappedFormTemplateModel> GetAllMappedFormTemplate(ReportFiltersModel reportFilter);

        List<FormModelToMap> GetAllFormWithCode();

        List<TemplateModelToMap> GetAllTemplateWithCode();

        List<MenuControlEntities> GetUsersWithCompany();

        string AddSingleMappedTemplate(MappedFormTemplateModel mappedFormTemplateModel);

        string AddBulkMappedTemplate(MappedFormTemplateModel mappedFormTemplateList);

        string DeleteTemplateMapping(string formCode,string templateName);

        List<TemplateModelToMap> GetMappedTemplateForEdit(string formCode, string templateName);

        string UpdateMappedTemplate(MappedFormTemplateModel mapppedModelToUpdate);
    }
}
