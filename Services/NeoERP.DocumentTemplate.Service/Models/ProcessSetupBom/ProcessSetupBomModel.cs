using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoERP.DocumentTemplate.Service.Models.ProcessSetupBom
{
    public class ProcessSetupBomModel
    {
        public int LEVEL { get; set; }
        public string PROCESS_CODE { get; set; }
        public string PROCESS_EDESC { get; set; }
        public string PROCESS_TYPE_CODE { get; set; }
        public string PROCESS_FLAG { get; set; }
        public string PRE_PROCESS_CODE { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

        public bool HAS_BRANCH { get; set; }
        public string LOCATION_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string PRIORITY_ORDER_NO { get; set; }



        public List<ProcessSetupBomModel> ITEMS { get; set; }

    }
    public class ProcessTypeCodeModel
    {
        public string PROCESS_TYPE_CODE { get; set; }
        public string PROCESS_TYPE_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime CREATED_DATE { get; set; }

    }
    public class BillAndOutputMaterialModel
    {
        public string PROCESS_CODE { get; set; }
        public string PROCESS_EDESC { get; set; }
        public string MODEL_INFO { get; set; }
        public string MODAL_INFO { get; set; }
        public string SKU_ITEM { get; set; }
        public string QUANTITY { get; set; }
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
        public string UNIT_CODE { get; set; }
        public string REMARKS { get; set; }
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string VALUATION_FLAG { get; set; }
        public string PERIOD_CODE { get; set; }
        public string PERIOD_EDESC { get; set; }
        public decimal? INDEX_CAPACITY { get; set; }
        public decimal? INDEX_TIME_REQUIRED { get; set; }
        public decimal? OUTPUT_PERCENT { get; set; }
        public decimal? OUT_PUT { get; set; }
        public string INPUT_INDEX_ITEM_CODE { get; set; }

        public string OUTPUT_INDEX_ITEM_CODE { get; set; }
    }
    public class BomRoutineModel
    {
        public string SHORT_CUT { get; set; }
        public string PROCESS_DESCRIPTION { get; set; }
        public string ITEM_DESCRIPTION { get; set; }
        public double CAPACITY { get; set; }
        public string MU_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }
    }
    public class ProcessCatRoutineForDDL
    {
        public string PRE_PROCESS_CODE { get; set; }
        public string PROCESS_CODE { get; set; }
        public string PROCESS_EDESC { get; set; }
        public string PROCESS_TYPE_CODE { get; set; }
        public string PROCESS_FLAG { get; set; }
    }
    public class ProcessCategoryRoutineSaveModel
    {
        public string ROOT_UNDER { get; set; }
        public string PROCESS_CODE { get; set; }
        public string IN_ENGLISH { get; set; }
        public string IN_NEPALI { get; set; }
        public string PRIORITY_NUMBER { get; set; }
        public string PROCESS_FLAG { get; set; }
        public string PROCESS_TYPE { get; set; }
        public string LOCATION { get; set; }
        public string REMARK { get; set; }
        public bool IS_EDIT { get; set; } = false;

    }
    public class InputOutMaterialSaveModel
    {
        public string MODAL_INFO { get; set; }
        public string PROCESS { get; set; }
        public string PROCESS_CODE { get; set; }
        public string ITEM_NAME { get; set; }
        public string ITEM_CODE { get; set; }
        public string CAPACITY { get; set; }
        public string QUANTITY { get; set; }
        public string UNIT { get; set; }
        public string UNIT_CODE { get; set; }
        public string REMARKS { get; set; }
        public string OUTPUT { get; set; }
        public decimal? OUT_PUT { get; set; }
        public decimal? OUTPUT_PERCENT { get; set; }
        public string VALUATION_FLAG { get; set; }
    }
    public class TempRoutineDetail
    {
        public string ROUTINE_NAME { get; set; }
        public string ROUTINE_CODE { get; set; }
        public string BELONGS { get; set; }
        public string BELONGS_CODE { get; set; }
        public string INPUT_INDEX_ITEM { get; set; }
        public string OUTPUT_INDEX_ITEM { get; set; }
        public string INPUT_CAPACITY { get; set; }
        public string INPUT_UNIT { get; set; }
        public string INPUT_IN_PERIOD { get; set; }
        public string OUTPUT_CAPACITY { get; set; }
        public string OUTPUT_UNIT { get; set; }
        public string OUTPUT_IN_PERIOD { get; set; }
        public string PROCESS_FLAG { get; set; }

    }
    public class RoutineDetailSaveModel
    {
        public TempRoutineDetail RoutineDetail { get; set; }
        public List<InputOutMaterialSaveModel> InputModel { get; set; } = new List<InputOutMaterialSaveModel>();
        public List<InputOutMaterialSaveModel> OutputModel { get; set; } = new List<InputOutMaterialSaveModel>();

        public bool? IsShowMsgOnly { get; set; }
    }


    public class RoutineDetailForValidationModel
    {
        public TempRoutineDetail RoutineDetail { get; set; }
    }


    public class ProcessMuCodeModel
    {
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
    }
    public class ProcessLocationModal
    {
        public string LOCATION_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }

    }
    public class ProcessItemModal
    {
        public string ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string MU_CODE { get; set; }
        public string MU_EDESC { get; set; }
    }
    public class ProcessResposponseForRoutine
    {
        public string MESSAGE { get; set; }
        public string PROCESS_FLAG { get; set; }
        public string ROOT_UNDER { get; set; }
        public ProcessCategoryRoutineSaveModel SAVED_MODAL { get; set; }
    }
    public class ProcessRoutineDetail
    {
        public string PROCESS_CODE { get; set; }
        public string PROCESS_EDESC { get; set; }
        public string PROCESS_TYPE_CODE { get; set; }
        public string PROCESS_TYPE_EDESC { get; set; }
        public string PROCESS_FLAG { get; set; }
        public string LOCATION_CODE { get; set; }

        public string PRE_LOCATION_CODE { get; set; }
        public string LOCATION_EDESC { get; set; }
        public string REMARKS { get; set; }
        public string PRIORITY_ORDER_NO { get; set; }

    }

    public class ProcessPeriodModal
    {
        public string PERIOD_CODE { get; set; }
        public string PERIOD_EDESC { get; set; }
        public decimal YEARLY_PERIOD_NO { get; set; }
        public decimal YEARLY_DAYS_NO { get; set; }

    }


    public class AssignedRoutineResourceSetupModel
    {
        public string RESOURCE_CODE { get; set; }         // a.RESOURCE_CODE
        public string RESOURCE_EDESC { get; set; }        // a.RESOURCE_EDESC
        public string QUANTITY { get; set; }             // TO_CHAR(b.QUANTITY)
        public string MU_CODE { get; set; }               // b.MU_CODE
        public string REMARKS { get; set; }              // b.REMARKS
        public decimal? STANDARD_OUTPUT_QTY { get; set; }
        public decimal? STANDARD_INPUT_QTY { get; set; }
        public decimal? REQUIRED_INPUT_QTY { get; set; }
        public decimal? ACTUAL_QTY { get; set; }
        public string HAS_SERIAL { get; set; }
        public string SERIAL_NO { get; set; }
    }


    public class ResourceInfoModel
    {
        public string RESOURCE_EDESC { get; set; }   // RESOURCE_EDESC
        public string REMARKS { get; set; }         // REMARKS
        public string RESOURCE_CODE { get; set; }    // RESOURCE_CODE
    }

    public class MuCodeModel
    {
        public string MU_EDESC { get; set; }
        public string REMARKS { get; set; }
        public string MU_CODE { get; set; }
    }


    public class MappingRoutineResourceModel
    {
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceCode { get; set; }
        public decimal Quantity { get; set; }  // nullable number field
        public string Unit { get; set; }
        public string UnitCode { get; set; }
        public string Remarks { get; set; }
        public string CategoryType { get; set; }
        public decimal StandardInputQty { get; set; }
        public decimal StandardOutputQty { get; set; }
    }


    public class MappingRoutineResourceModelTest1
    {
        public string ProcessCode { get; set; }
        public string ProcessName { get; set; }
        public string ResourceName { get; set; }
        public string ResourceCode { get; set; }
        public decimal Quantity { get; set; }  // nullable number field
        public string Unit { get; set; }
        public string UnitCode { get; set; }
        public string Remarks { get; set; }
    }

}
