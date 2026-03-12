using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoERP.ProjectManagement.Service.Models;

namespace NeoERP.ProjectManagement.Service.Interface
{
    public interface ITestTemplateRepo
    {
        void testfunction();
        List<FormDetailSetup> GetAllFORMDETAILSETUP();
    }
}
