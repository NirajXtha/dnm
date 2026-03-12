using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Core.Models
{
    public class ReportFilterSettingsModel
    {
        public bool ShowQuantityFilter { get; set; }

        public bool ShowQuantityFigureFilter { get; set; }

        public bool ShowQuantityRoundUpFilter { get; set; }

        public bool ShowAmountFilter { get; set; }

        public bool ShowAmountFigureFilter { get; set; }

        public bool ShowAmountRoundUpFilter { get; set; }

        public bool ShowRangeAmountFilter { get; set; }

        public bool ShowRangeQuantityFilter { get; set; }

        public bool ShowRangeRateFilter { get; set; }

        public bool ShowInPoup { get; set; } = true;
        public string ActionPageId { get; set; }

        public bool ShowApplyToAllChartOption { get; set; } = false;

        public bool ShowBranchFilter { get; set; } = false;
        public bool ShowChartResetButton { get; set; } = false;

        public bool ShowContractStatusType { get; set; } = false;

        public bool ShowClassWise { get; set; } = false;

      
    }
}