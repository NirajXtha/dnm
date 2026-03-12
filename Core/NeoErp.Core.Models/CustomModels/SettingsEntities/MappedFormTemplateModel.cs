using System;
using System.Collections.Generic;

namespace NeoErp.Core.Models.CustomModels.SettingsEntities
{
    public  class MappedFormTemplateModel
    {
        public int FORM_TEMPLATE_ID { get; set; }

        public string USER_NO { get; set; }

        public string COMPANY_EDESC { get; set; }

        public string FORM_NAME { get; set; }

        public string FORM_CODE { get; set; }
        public string FORM_EDESC { get; set; }
        public string TEMPLATE_NAME { get; set; }

        public string TEMPLATE_PATH { get; set; }
        public string USER_ID { get; set; }
        public string LOGIN_EDESC { get; set; }
        public string COMPANY_CODE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string MODIFIED_BY { get; set; }
        public DateTime? MODIFIED_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime? CREATED_DATE { get; set; }


        // Property For getting multiple value
        public List<string> FORM_LIST { get; set; }

        public List<string> TEMPLATE_LIST { get; set; }

        public List<string> USER_LIST { get; set; }


    }

    public class FormModelToMap
    {

        public string id { get; set; }

        public string label { get; set; }
        public string FORM_CODE { get; set; }

        public string FORM_EDESC { get; set; }
    }

    public class TemplateModelToMap
    {

        public string id { get; set; }

        public string label { get; set; }
        public string TEMPLATE_NAME { get; set; }

        public string PARTIAL_VIEW_NAME { get; set; }

        public string TEMPLATE_PATH { get; set; }
    }
}
