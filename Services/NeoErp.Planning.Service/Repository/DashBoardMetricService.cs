using NeoErp.Core;
using NeoErp.Data;
using NeoErp.Planning.Service.Interface;
using NeoErp.Planning.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Planning.Service.Repository
{
    public class DashBoardMetricService : IDashBoardMetricService
    {
        //private NeoErpCoreEntity _objectEntity;
        //private IWorkContext _workContext;
        private IDbContext _objectEntity;
        private IWorkContext _workContext;

        public DashBoardMetricService(IDbContext objectEntity, IWorkContext _workContext)
        {
            this._objectEntity = objectEntity;
            this._workContext = _workContext;
        }
        public List<MetricWidgetsModel> GetMetricList()
        {
            var returnList = new List<MetricWidgetsModel>();

            var sqlQuery = @"select id as ReportId,USERPERMISSION, isactive as IsActive,orderno as Orderno, quickcap_edesc as widgetsName,
            quickcap_bgcolor as WidgetsBGColor,quickcap_title as widgetsTitle,sql_statement as sqlQuery,quickcap_fontcolor as widgetsColor
            ,QUICKTYPE ,MIDBGCOLOR,MIDFONTCOLOR,MIDISBLINK,MAXBGCOLOR,MAXFONTCOLOR,MAXISBLINK,MAXVALUES,MINVALUES,MINISBLINK,LABELPOSITION,SPEEDOMETERMAXVALUE,MAXVALUEQUERY,MINVALUEQUERY from web_quickcap
            where deleted_flag = 'N'";
            var userID = this._workContext.CurrentUserinformation.User_id;
            var Widgets = _objectEntity.SqlQuery<MetricWidgetsModel>(sqlQuery).ToList().Where(x => x.USERPERMISSION == null ? false : x.USERPERMISSION.Contains(userID.ToString())).ToList();



            var orderItemQuery = @"SELECT regexp_replace(ORDER_NO, '[^0-9,]', '')  from dashboard_widgets where MODULE_NAME = 'DashBoard_Widgets_PLAN'";
            string orderlist = _objectEntity.SqlQuery<string>(orderItemQuery).FirstOrDefault();
            if (orderlist != null)
            {
                var ids = orderlist.Split(',');
                for (int i = 0; i < ids.Length; i++)
                {
                    var wid = Widgets.Where(x => x.ReportId == int.Parse(ids[i])).FirstOrDefault();

                    if (wid != null)
                    {
                        wid.orderIndex = i;

                        returnList.Add(wid);
                    }

                }
                if (returnList.Count() == 0)
                    returnList = Widgets;
            }
            else
            {
                returnList = Widgets;
            }
            return returnList;

        }
        public List<string> GetDashboard(string reportname)
        {
            var query = "select distinct QuickCap from dashboard_widgets  where module_name='" + reportname + "'";
            return _objectEntity.SqlQuery<string>(query).ToList();
        }
    }
}
