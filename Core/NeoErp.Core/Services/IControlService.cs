using NeoErp.Core.Models.CustomModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
    public interface IControlService
    {
        IEnumerable<DateFilterModel> GetDateFilters(string fiscalYear, string textToAppend="", bool appendText=false,int substractYear = 0);
        IEnumerable<DateFilterModel> GetDateFiltersWithWeek(string fiscalYear, string textToAppend = "", bool appendText = false, int substractYear = 0);
        IEnumerable<DateFilterModel> GetEnglishDateFilters(string fiscalYear, string textToAppend = "", bool appendText = false);
        IEnumerable<DateFilterModel> GetNepaliDateFilters(string FiscalYear);
    }
}
