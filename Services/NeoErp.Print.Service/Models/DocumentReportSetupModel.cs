using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Print.Service.Models
{
    public class DocumentReportSetupModels
    {
        public string REPORT_NO { get; set; }
        public string REPORT_EDESC { get; set; }
        public string REPORT_NDESC { get; set; }
        public string MASTER_REPORT_CODE { get; set; }
        public string PRE_REPORT_CODE { get; set; }
        public string GROUP_SKU_FLAG { get; set; }
        public string MODULE_CODE { get; set; }
        
        // Form Setup fields
        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string FORM_TYPE { get; set; }
        public string MASTER_FORM_CODE { get; set; }
        public string PRE_FORM_CODE { get; set; }

        // Form Detail Setup fields
        public int SERIAL_NO { get; set; }
        public string TABLE_NAME { get; set; }
        public string COLUMN_NAME { get; set; }
        public string COLUMN_HEADER { get; set; }
        public string MASTER_CHILD_FLAG { get; set; }
    }

    // Pattern list model
    public class PatternListModel
    {
        public int PATTERN_ID { get; set; }
        public string PATTERN_NAME { get; set; }
    }

    // Pattern detail model
    public class PatternDetailModel
    {
        public int PATTERN_ID { get; set; }
        public string PATTERN_NAME { get; set; }
        public string FORM_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public int? CHARGE_EXIST { get; set; }
        public int? AUTO { get; set; }
        public string SQL_QUERY { get; set; }
        public string FILE_NAME { get; set; }
        public string FORM_TYPE { get; set; }
        public int? ACTIVE { get; set; }
        public string MAIN_FIELD { get; set; }
        public int? ITEM_COUNT { get; set; }
        public string MENU_NO { get; set; }
    }

    // Pattern head field model
    public class PatternHeadFieldModel
    {
        public int PATTERN_ID { get; set; }
        public int? SN { get; set; }
        public string LABEL { get; set; }
        public string FIELD { get; set; }
        public string DEFAULT_VAL { get; set; }
    }

    // Pattern footer field model
    public class PatternFooterFieldModel
    {
        public int PATTERN_ID { get; set; }
        public int? SN { get; set; }
        public string LABEL { get; set; }
        public string FIELD { get; set; }
        public string DEFAULT_VAL { get; set; }
    }

    // Pattern column field model
    public class PatternColumnFieldModel
    {
        public int PATTERN_ID { get; set; }
        public int? SN { get; set; }
        public string MASTER_CHILD_FLAG { get; set; }  // M or C
        public string LABEL { get; set; }
        public string FIELD { get; set; }

        
        public string DEFAULT_VAL { get; set; }
        public int? WIDTH { get; set; }

    }

    // Complete pattern configuration response
    public class PatternConfigurationResponse
    {
        public List<PatternListModel> Patterns { get; set; }
        public PatternDetailModel PatternDetail { get; set; }
        public List<PatternHeadFieldModel> HeadFields { get; set; }
        public List<PatternFooterFieldModel> FooterFields { get; set; }
        public List<PatternColumnFieldModel> ColumnFields { get; set; }
    }

    // Preview data response model
    public class PreviewDataResponse
    {
        public string FileName { get; set; }
        public List<PatternHeadFieldModel> HeadFields { get; set; }
        public List<PatternFooterFieldModel> FooterFields { get; set; }
        public List<PatternColumnFieldModel> MasterFields { get; set; }
        public List<PatternColumnFieldModel> ChildFields { get; set; }
        public List<Dictionary<string, object>> QueryData { get; set; }
        public List<Dictionary<string, object>> ChargeData { get; set; }
        public string FormType { get; set; }

        public int? ITEM_COUNT { get; set; }

        
        public List<SubLedgerGroup> SubLedgerData { get; set; }
        
        public int? PRINT_COUNT { get; set; }
        public bool PRINT_COUNT_FLAG { get; set; }
    }

    // Sub-ledger group model
    public class SubLedgerGroup
    {
        public int SerialNo { get; set; }
        public List<SubLedgerItem> Items { get; set; }
    }

    // Sub-ledger item model
    public class SubLedgerItem
    {
        public string sub_edesc { get; set; }
        public object amount { get; set; }
    }

    // Menu module model
    public class MenuModuleModel
    {
        public string MODULE_CODE { get; set; }
        public string MODULE_EDESC { get; set; }
    }

    // Menu group model
    public class MenuGroupModel
    {
        public string MENU_NO { get; set; }
        public string MENU_EDESC { get; set; }
        public string MENU_OBJECT_NAME { get; set; }
        public string MODULE_CODE { get; set; }
        public string FULL_PATH { get; set; }
        public string VIRTUAL_PATH { get; set; }
        public string PRE_MENU_NO { get; set; }
    }

    // Save menu request model
    public class SaveMenuRequest
    {
        public string ExistingMenuNo { get; set; }  // If provided, UPDATE instead of INSERT
        public string ModuleCode { get; set; }
        public string GroupMenuNo { get; set; }
        public string MenuEdesc { get; set; }
        public string MenuObjectName { get; set; }
        public string FullPath { get; set; }
        public string VirtualPath { get; set; }
        public string PatternId { get; set; }  // To link menu to pattern
    }

    // ===== DYNAMIC FILTER MODELS =====

    // Query placeholder model
    public class QueryPlaceholder
    {
        public string Field { get; set; }
        public string Table { get; set; }
        public string EntityType { get; set; }
    }

    // Entity search result model
    public class EntitySearchResult
    {
        public List<EntityItem> Data { get; set; }
        public int TotalCount { get; set; }
        public bool HasMore { get; set; }
    }

    // Entity item model
    public class EntityItem
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string MasterCode { get; set; }
        public bool IsGroup { get; set; }
    }

    // Filter value model for deserializing filter parameters
    public class FilterValue
    {
        public string Code { get; set; }
        public List<string> Codes { get; set; }
        public bool IsGroup { get; set; }
    }
}
