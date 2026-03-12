using NeoErp.Print.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Print.Service.Services
{
    public interface IPrintSetupService
    {
        List<ModuleSetupModel> GetModuleList();
        List<DocumentReportSetupModels> GetReportGroups(string moduleCode, string companyCode);
        List<DocumentReportSetupModels> GetReportItems(string moduleCode, string companyCode, string masterFormCode);        
        PatternConfigurationResponse GetAllPatterns(string formCode, string companyCode, int? patternId = null);
        int AddPattern(string patternName, string formCode, string companyCode);
        bool SavePattern(PatternDetailModel patternDetail, List<PatternHeadFieldModel> headFields, List<PatternFooterFieldModel> footerFields, List<PatternColumnFieldModel> columnFields);
        PatternConfigurationResponse AutoFillPattern(string formCode, int patternId, string companyCode);
        string GenerateSqlQuery(string formCode, int patternId, string companyCode);
        PreviewDataResponse GetPreviewData(string formCode, string mainFieldValue, string companyCode, int activePatternId, string filters = "");
        string GetPreviewFileName(string formCode, string companyCode, int activePatternId);

        // Menu management methods
        List<MenuModuleModel> GetMenuModules();
        List<MenuGroupModel> GetMenuGroups(string moduleCode);
        string GetNextMenuCode(string moduleCode, string groupMenuNo);
        bool SaveNewMenu(SaveMenuRequest request, string companyCode);
        MenuGroupModel GetMenuInfo(string menuNo);

        // Dynamic filter methods
        List<QueryPlaceholder> ExtractPlaceholders(string sqlQuery);
        EntitySearchResult SearchEntity(string entityType, string searchTerm, string companyCode, int pageNumber, int pageSize);
        List<string> GetEntityChildren(string entityType, string masterCode, string companyCode);
        string PrintCount(string formCode, string mainFieldValue ,string companyCode);
        
    }
}
