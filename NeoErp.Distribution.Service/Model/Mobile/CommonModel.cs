using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Model.Mobile
{
    public class CommonModel<T>
    {
        public T result { get; set; }
        public bool response { get; set; }
        public string error { get; set; }
    }
}
