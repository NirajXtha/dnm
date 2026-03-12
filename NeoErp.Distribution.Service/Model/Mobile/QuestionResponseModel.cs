using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class QuestionResponseModel
    {
        public QuestionResponseModel()
        {
            general = new List<GeneralModel>();
            tabular = new Dictionary<string, TabularModel>();
        }
        public List<GeneralModel> general { get; set; }
        public Dictionary<string, TabularModel> tabular { get; set; }
    }
    public class GeneralModel
    {
        public GeneralModel()
        {
            ANSWERS = new List<string>();
        }
        public string QA_CODE { get; set; }
        public string QA_TYPE { get; set; }
        public string QUESTION { get; set; }
        public string SET_TYPE { get; set; }
        public string SURVEY_EDESC { get; set; }
        public string AREA_CODES { get; set; }
        public string GROUP_CODES { get; set; }
        public List<string> ANSWERS { get; set; }
    }
    public class TabularModel
    {
        public TabularModel()
        {
            CELL_DATA = new List<List<TabularCellModel>>();
        }
        public string TABLE_ID { get; set; }
        public string TABLE_TITLE { get; set; }
        public string CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public string SET_TYPE { get; set; }
        public string SURVEY_EDESC { get; set; }
        public string AREA_CODES { get; set; }
        public string GROUP_CODES { get; set; }
        public List<List<TabularCellModel>> CELL_DATA { get; set; }
    }
    public class TabularCellModel
    {
        public string CELL_ID { get; set; }
        public string ROW_NO { get; set; }
        public string CELL_NO { get; set; }
        public string CELL_TYPE { get; set; }
        public string CELL_LABEL { get; set; }
    }
    public class TabularTemp
    {
        public int TABLE_ID { get; set; }
        public string TABLE_TITLE { get; set; }
        public DateTime? CREATED_DATE { get; set; }
        public string DELETED_FLAG { get; set; }
        public int CELL_ID { get; set; }
        public int ROW_NO { get; set; }
        public int CELL_NO { get; set; }
        public string CELL_TYPE { get; set; }
        public string CELL_LABEL { get; set; }
        public string SET_TYPE { get; set; }
        public string SURVEY_EDESC { get; set; }
        public string AREA_CODES { get; set; }
        public string GROUP_CODES { get; set; }
    }
}