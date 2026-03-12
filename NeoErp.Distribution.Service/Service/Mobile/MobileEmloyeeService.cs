using NeoErp.Core.Domain;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Distribution.Service.Service.Mobile
{
    
    public class MobileEmloyeeService : IMobileEmployee
    {
        private NeoErpCoreEntity _objectEntity;
        public MobileEmloyeeService()
        {
            _objectEntity = new NeoErpCoreEntity();
        }
        public List<SalesPersonModel> getSalesPersonList()
        {
            User userInfo = new User();
            userInfo.company_code = "06";
            var filter = "";
            //if (!string.IsNullOrWhiteSpace(userInfo.sp_codes))
            //    filter = $" AND DLU.SP_CODE IN ({userInfo.sp_codes})";

            string query = $@"SELECT DLU.SP_CODE,HES.EMPLOYEE_EDESC,DLU.GROUPID,WM_CONCAT(DISTINCT DUA.AREA_CODE) AREA_CODE
                FROM DIST_LOGIN_USER DLU
                INNER JOIN DIST_USER_AREAS DUA ON DLU.SP_CODE = DUA.SP_CODE AND DLU.COMPANY_CODE = DUA.COMPANY_CODE
                INNER JOIN HR_EMPLOYEE_SETUP HES ON DLU.SP_CODE = HES.EMPLOYEE_CODE AND DLU.COMPANY_CODE = HES.COMPANY_CODE
                WHERE DLU.COMPANY_CODE = '{userInfo.company_code}'
                            AND DLU.ACTIVE = 'Y'
                            AND DLU.USER_TYPE = 'S' 
                            AND HES.DELETED_FLAG = 'N'
                GROUP BY DLU.SP_CODE,HES.EMPLOYEE_EDESC,DLU.GROUPID";
            var data = _objectEntity.SqlQuery<SalesPersonModel>(query).ToList();
            data = data == null ? new List<SalesPersonModel>() : data;
            return data;
        }
    }
}
