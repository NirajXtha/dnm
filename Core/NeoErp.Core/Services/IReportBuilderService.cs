using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace NeoErp.Core.Services
{
    public interface IReportBuilderService
    {
        DataTable GetData(string reportName, string queryProvider, string settings = "");
    }
}
