using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    public interface IMobileResellerService
    {
        List<ResellerRegistration> GetResellerRegisteredList(string Id, out string message, out bool status);
        void RegisterReseller(ResellerRegistration model, out string message, out bool status);
        ResellerRegistration ValidateLogin(string userName, string password, out string message, out bool status);
        void ApproveRegisteredReseller(string Id, out string message, out bool status);
        void ChangePassword(string password, string userName, out string message, out bool status);
        void DeleteReseller(string id, out string message, out bool status);
        List<ResellerRegistration> GetResellerImgLst(string resellerCode, out string message, out bool status);
    }
}
