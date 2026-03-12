using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class QuestionRequestModel:CommonRequestModel
    {
        public string sp_code { get; set; }
        public string SetType { get; set; }
    }
}
