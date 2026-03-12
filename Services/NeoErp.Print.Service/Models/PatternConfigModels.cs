using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Print.Service.Models
{
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
    }

    // Pattern head field model
    public class PatternHeadFieldModel
    {
        public int PATTERN_ID { get; set; }
        public string LABEL { get; set; }
        public string FIELD { get; set; }
        public string DEFAULT_VAL { get; set; }
        public List<PatternColumnFieldModel> ColumnFields { get; set; }
    }
}
