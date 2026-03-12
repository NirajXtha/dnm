using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Models.CustomModels
{
    public class AggregationModel
    {
        public object min { get; set; }

        public object max { get; set; }

        public object sum { get; set; }

        public object average { get; set; }
    }
}
