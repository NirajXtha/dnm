using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class CompetitorItemModel
    {
        public int ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
    }
    public class CompetitorItemMapModel
    {
        public int ITEM_CODE { get; set; }
        public string ITEM_EDESC { get; set; }
        public string COMP_ITEM_CODES { get; set; }
    }

    public class CompetitorItemFields
    {
        public int QUESTION_ID { get; set; }
        public int ITEM_CODE { get; set; }
        public string COL_NAME { get; set; }
        public string COL_DATA_TYPE { get; set; }
    }
    public class CompAnsItemModel : CommonRequestModel
    {
        public string COMP_ITEM_CODE { get; set; }
        public string QUESTION_ID { get; set; }
        public string ANSWER { get; set; }
    }
    public class CompAnsModel : CommonRequestModel
    {
        public CompAnsModel()
        {
            ANSWERS = new List<CompAnsItemModel>();
        }
        public string ITEM_CODE { get; set; }
        public string ENTITY_CODE { get; set; }
        public string ENTITY_TYPE { get; set; }
        public List<CompAnsItemModel> ANSWERS { get; set; }
    }
}
