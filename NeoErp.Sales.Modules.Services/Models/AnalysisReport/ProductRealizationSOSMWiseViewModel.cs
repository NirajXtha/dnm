using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Sales.Modules.Services.Models.AnalysisReport
{
    public class ProductRealizationSOSMWiseModel
    {
        public string SalesOfficer { get; set; }
        public string SalesManager { get; set; }
        public decimal? OPC { get; set; }
        public decimal? PPC { get; set; }
        public decimal? PSC { get; set; }
    }
    public class ProductRealizationSOSMWiseViewModel
    {
        public List<ProductRealizationSOSMWiseModel> ProductRealizationSOSMWiseModel { get; set; }
        public decimal total { get; set; }
        public ProductRealizationSOSMWiseViewModel()
        {
            ProductRealizationSOSMWiseModel = new List<ProductRealizationSOSMWiseModel>();
            this.AggregationResult = new Dictionary<string, AggregationModel>();
        }
        public Dictionary<string, AggregationModel> AggregationResult { get; set; }
    }
}
