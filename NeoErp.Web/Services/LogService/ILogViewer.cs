using NeoErp.Models.LogServiceModel;
using System;
using System.Collections.Generic;

namespace NeoErp.Services.LogService
{
    public interface ILogViewer
    {
        List<LogServiceViewModel> GetLogForCurrentUserAndCompany();
        void DeleteLogUsingFilter(DateTime fromDate, DateTime toDate, string module, string subModule);

        void DeleteAllLog();
        List<LogServiceViewModel> GetAllLog();
    }
}
