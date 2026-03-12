using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoERP.DocumentTemplate.Service.Models;

namespace NeoERP.DocumentTemplate.Service.Interface
{
    public interface ITestTemplateRepo
    {
        void testfunction();
        List<FormDetailSetup> GetAllFORMDETAILSETUP();
    }
}
