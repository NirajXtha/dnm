using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class EntityRequestModel:CommonRequestModel
    {
        public string ACC_CODE { get; set; }
        public string entity_code { get; set; }
        public string entity_type { get; set; }
    }
    public class ImageSaveModel
    {
        public string Description { get; set; }
        public string CategoryId { get; set; }
    }
    public class EntityRequestModelOffline : CommonRequestModel
    {
        public string ACC_CODE { get; set; }
        public string entity_code { get; set; }
        public string entity_type { get; set; }
        public string File_name { get; set; }
        public string Media_Type { get; set; }
        public string Description { get; set; }
        public string Categoryid { get; set; }
        public string Title { get; set; }
        public int Index { get; set; }
    }
}
