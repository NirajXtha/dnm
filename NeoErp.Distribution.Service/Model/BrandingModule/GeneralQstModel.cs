using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.BrandingModule
{

    public class GeneralQstModel
    {
        public GeneralQstModel()
        {
            ANSWERS = new List<string>();
        }
        public string CONTRACT_CODE { get; set; }
        public string QA_CODE { get; set; }
        public string QA_TYPE { get; set; }
        public string QUESTION { get; set; }
        public List<string> ANSWERS { get; set; }
        public string ANSWER { get; set; }
    }

    public class GeneralSaveModel
    {
        public string CONTRACT_CODE { get; set; }
        public List<GeneralQstModel> ANSWERS { get; set; }
    }

    public class QstReportModel
    {
        public string CONTRACT_CODE { get; set; }
        public string CONTRACT_EDESC { get; set; }
        public string QUESTION { get; set; }
        public string ANSWER { get; set; }
        public string ANSWERED_BY { get; set; }
        public string SOURCE { get; set; }
        public DateTime CREATED_DATE { get; set; }
    }
}
