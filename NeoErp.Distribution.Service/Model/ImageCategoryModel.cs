using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model
{
    public class ImageCategoryModel
    {
        public string CATEGORY_CODE { get; set; }
        public string CATEGORY_EDESC { get; set; }
        public int CATEGORYID { get; set; }
        public int Max_Items { get; set; }
        public string selectedType { get; set; }
    }
}