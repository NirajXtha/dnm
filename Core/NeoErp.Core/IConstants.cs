using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core
{
  public interface IConstants
    {
        string CalendarReportMonthlyPartyWiseSalesQuery();
        string GetCalendarReportDetailsQuery { get; }
        string GetAdVersusBsCalendarQuery { get; }
        string CalendarTypeADVsBSWeeksQuery { get; }
    }
}
