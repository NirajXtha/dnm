using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Pos.Services.Model
{
    public class ItemImageModel
    {
        public string ItemCode { get; set; }

        public HttpPostedFileBase file { get; set; }

        public string Path { get; set; }
    }
}