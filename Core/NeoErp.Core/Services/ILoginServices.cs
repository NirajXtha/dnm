using NeoErp.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Core.Services
{
 public   interface ILoginServices
    {

        User GetUser(User user);
        User GetUserByUserId(string userId);
        
    }
}
