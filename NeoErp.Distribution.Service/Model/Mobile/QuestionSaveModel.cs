using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class QuestionaireSaveModel : CommonRequestModel
    {
        public QuestionaireSaveModel()
        {
            general = new List<GeneralSaveModel>();
            tabular = new List<TabularSaveModel>();
        }
        public string sp_code { get; set; }
        public string entity_type { get; set; }
        public string entity_code { get; set; }
        public List<GeneralSaveModel> general { get; set; }
        public List<TabularSaveModel> tabular { get; set; }
    }
    public class GeneralSaveModel
    {
        public string qa_code { get; set; }
        public string answer { get; set; }
        public string Sync_Id { get; set; }
    }
    public class TabularSaveModel
    {
        public string cell_id { get; set; }
        public string answer { get; set; }
        public string Sync_Id { get; set; }
    }
}