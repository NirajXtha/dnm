using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class QuestionSetupModel
    {
        public QuestionSetupModel()
        {
            Questions = new List<GeneralQuestionModel>();
        }
        public List<GeneralQuestionModel> Questions { get; set; }
        public string SetId { get; set; }
        public string SetTitle { get; set; }
        public string SetType { get; set; }
        public string IsActive { get; set; }
        public bool Editable { get; set; }
    }
    public class GeneralQuestionModel
    {
        public string Type { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
    }
    public class TabularModel
    {
        public TabularModel()
        {
            Cells = new List<List<CellModel>>();
        }
        public string SetId { get; set; }
        public string SetTitle { get; set; }
        public string SetType { get; set; }
        public string IsActive { get; set; }
        public List<List<CellModel>> Cells { get; set; }
        public bool Editable { get; set; }
    }
    public class CellModel
    {
        public string Type { get; set; }
        public string Label { get; set; }
    }
}