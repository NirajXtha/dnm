using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Models;

namespace NeoErp.Core.Helpers
{
    public static class ReportFilterHelper
    {

        public static int FigureFilterValue(string value)
        {
            FigureFilter figureValue;
            if (!Enum.TryParse(value, out figureValue))
            {
                figureValue = FigureFilter.Actual;
            }
            return (int)figureValue;
        }

        public static int RoundUpFilterValue(string value)
        {
            if (value.IndexOf('.') != -1)
            {
                var precisionArrary =  value.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
                if (precisionArrary.Count() == 2)
                    return precisionArrary[1].Count();

                else
                {
                    return 2;
                }

            }
            return 0;
        }

        public static void RangeFilterValue(string value, out int minValue, out int maxValue)
        {
            decimal minVal = 0;
            decimal maxVal  = 0;
            
            if(value.IndexOf(';') != -1)
            {
                var rangeSplit = value.Split(new char[] { ';'}, StringSplitOptions.RemoveEmptyEntries);

                if(rangeSplit.Count() == 2)
                {
                    decimal.TryParse(rangeSplit[0], out minVal);
                    decimal.TryParse(rangeSplit[1], out maxVal);
                }
            }

            minValue = (int)minVal;
            maxValue = (int)maxVal;
            
        }
    }
}