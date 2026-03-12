using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Transaction.Service.Models;

namespace NeoErp.Transaction.Service.Services
{
    public interface ITargetServiceRepository
    {
        List<Target> GetAllTargets();
        void AddNewTarget(Target target);
        void UpdateTarget(Target target);
        void DeleteTarget(Target target);
    }
}
