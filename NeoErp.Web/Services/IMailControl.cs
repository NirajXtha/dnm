using NeoErp.Core.Models.CustomModels;
using NeoErp.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Services
{
    public interface IMailControl
    {
        List<MailListModel> AllMailList();
    }
}
