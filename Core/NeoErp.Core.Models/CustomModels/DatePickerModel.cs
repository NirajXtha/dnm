using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    public class DatePickerModel
    {
        public bool AppendText { get; set; }
        public string TextToAppend { get; set; }
        public string ActionPageId { get; set; }
        public bool IsPopUp { get; set; } = true;
    }
}
