using NeoErp.Core.Caching;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Core.Plugins;
using NeoErp.Core.Services;
using NeoErp.Data;
using NeoErp.Models.QueryBuilder;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Sales.Modules.Services.Services;
using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace NeoErp.Controllers.Api
{
    public class QueryBuilderController : ApiController
    {
        public IQueryBuilder _queryBuilder { get; set; }
        private IDbContext _dbContext;
        private IDashBoardMetricService _dashBoardMetricservice { get; set; }
        private readonly IPluginFinder _pluginFinder;
        private ICacheManager _cacheManager { get; set; }

        public QueryBuilderController(IQueryBuilder queryBuilder, IDbContext dbContext, IDashBoardMetricService dashBoardMetricService,IPluginFinder pluginFinder,ICacheManager cacheManager)
        {
            this._queryBuilder = queryBuilder;
            this._dbContext = dbContext;
            this._dashBoardMetricservice = dashBoardMetricService;
            this._pluginFinder = pluginFinder;
            this._cacheManager = cacheManager;

        }

        public HttpResponseMessage GetModuleList()
        {
            try
            {
                var installPlugins = _pluginFinder.GetPluginDescriptors(LoadPluginsMode.InstalledOnly, 0);
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "find modulename", data = installPlugins, statusCode = (int)HttpStatusCode.OK });
            }
            catch(Exception e)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { Message = "find modulename", data = "error", statusCode = (int)HttpStatusCode.NotFound });
            }
        }
        public HttpResponseMessage GetTablesList()
        {

            try
            {
                var tables = this._queryBuilder.GetTransactionTable();
                if (tables != null && tables.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Table is Found"), DATA = tables, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = string.Format("No Found"), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        public HttpResponseMessage GetColumsListByTableName(string tablesName)
        {
            try
            {
               var tables = this._queryBuilder.GetTablesCoumnsByTableName(tablesName);
                var datecolumn = string.Empty;
                //var tablesContainVoucherDate = tables.Where(x => x.columnsName.ToLower().Equals("VOUCHER_DATE".ToLower())).FirstOrDefault();
                if (tables.Where(x => x.columnsName.ToLower().Equals("VOUCHER_DATE".ToLower())).FirstOrDefault() != null)
                {
                    datecolumn = tables.Where(x => x.columnsName.ToLower().Equals("VOUCHER_DATE".ToLower())).FirstOrDefault().columnsName;
                }
                else if (tables.Where(x => x.columnsName.ToLower().Equals("ORDER_DATE".ToLower())).FirstOrDefault() != null)
                {
                    datecolumn = tables.Where(x => x.columnsName.ToLower().Equals("ORDER_DATE".ToLower())).FirstOrDefault().columnsName;
                }
                else
                {
                    datecolumn = "CREATED_BY";
                }

                if (tables != null && tables.Count() > 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Columns are Found"), DATA = tables, datecolumn = datecolumn, STATUS_CODE = (int)HttpStatusCode.OK });
                }
                return Request.CreateResponse(HttpStatusCode.NotFound, new { MESSAGE = string.Format("No Found"), STATUS_CODE = (int)HttpStatusCode.NotFound });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "Error occured while processing the request - " + ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage CheckQuery([FromUri] string query)
        {
            try
            {
                var queryChk = query.Replace("\"", "'");

                var queryChecker = queryChk.Replace("''", "'");


                if (query.ToLower().Trim().Contains("count"))
                {
                    var rowCounts = _dbContext.SqlQuery<int?>(queryChecker).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                else
                {
                    var rowCounts = _dbContext.SqlQuery(queryChecker);
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK });
                }
                // var rowCounts = _dbContext.SqlQuery<int>(query).ToList();

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "aggeration Function is not Choosen", STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }


        [HttpPost]
        public HttpResponseMessage GetHighetDataFromSql(highntQuery query)
        {
            try
            {
                var QueryVerify = query.query;
               var queryChecker = QueryVerify.Replace("''", "'");

                if (QueryVerify.ToLower().Trim().Contains("count"))
                {
                    var rowCounts = _dbContext.SqlQuery<int?>(queryChecker).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK, value = rowCounts.FirstOrDefault() });
                }
                else
                {
                    var rowCounts = _dbContext.SqlQuery<decimal?>(queryChecker).ToList();
                    return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK, value = rowCounts.FirstOrDefault() });
                }
                // var rowCounts = _dbContext.SqlQuery<int>(query).ToList();

                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = "aggeration Function is not Choosen", STATUS_CODE = (int)HttpStatusCode.InternalServerError, value = "0" });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError, value = "0" });
            }

        }


        [HttpPost]
        public HttpResponseMessage CreateWidzed(CreateMetricwidgets Model)
        {
            try
            {
                Model.sqlQuery=Model.sqlQuery.Replace("'", "\"");
                var order = 0;
                int.TryParse(Model.OrderNo, out order);
                var maxValue = 0;
                int.TryParse(Model.MaxValue, out maxValue);
                var minValue = 0;
                int.TryParse(Model.MinValue, out minValue);
                var isactive = Model.Isactive ? "Y" : "N";
                var midIsblink = Model.IsMidBlink ? "Y" : "N";
                var maxIsblink = Model.IsMaxBlink ? "Y" : "N";
                var Isblink = Model.IsBlink ? "Y" : "N";
                var insertQuery = string.Format(@"INSERT INTO WEB_QUICKCAP
(Id, QUICKCAP_NO, QUICKCAP_EDESC, MODULE_CODE, SQL_STATEMENT, QUICKCAP_TITLE, QUICKCAP_BGCOLOR, QUICKCAP_FONTCOLOR, QUICKCAP_ICON,DELETED_FLAG,ISACTIVE,ORDERNO,QUICKTYPE,MIDBGCOLOR,
MIDFONTCOLOR,MIDISBLINK,MAXBGCOLOR,MAXFONTCOLOR,MAXISBLINK,MAXVALUES,MINVALUES,MINISBLINK,LABELPOSITION,SPEEDOMETERMAXVALUE,MAXVALUEQUERY,MINVALUEQUERY,calculationBase,secondaryTitle,tableSize,sparklineoption, xaxislabel,yaxislabel,redirect_link )
VALUES
({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','N','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}','{26}','" + Model.TableSize+"','"+Model.sparklineoption+"','"+Model.Xaxis+"','"+Model.Zaxis+ "','{27}')", "QuickCap_sequence.nextval", 01, Model.widgetsName, Model.ModuleCode, Model.sqlQuery, Model.widgetsTitle, Model.WidgetsBGColor, Model.widgetsColor, Model.widgetFontIcon, isactive, order
, Model.ChartType, Model.MidBGColor, Model.MidFontColor, midIsblink, Model.MaxBGColor, Model.MaxFontColor, maxIsblink, maxValue, minValue, Isblink, Model.LABELPOSITION, Model.SPEEDOMETERMAXVALUE, Model.MAXVALUEQUERY, Model.MINVALUEQUERY,Model.CaculationMethod, Model.SecondaryTitle, Model.widgetLink);

                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                //var rowCounts = _dbContext.SqlQuery<int>(query).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpPost]
        public HttpResponseMessage CreateWidzedConfig(CreateMetricwidgets Model)
        {
            try
            {
                Model.sqlQuery = Model.sqlQuery.Replace("#customercode#",Model.customerCode);
                Model.sqlQuery = Model.sqlQuery.Replace("#itemcode#", Model.itemsCode);
                Model.sqlQuery = Model.sqlQuery.Replace("#suppliercode#", Model.suppliersCode);
                Model.sqlQuery = Model.sqlQuery.Replace("#divisioncode#", Model.divisionCode);
                Model.sqlQuery = Model.sqlQuery.Replace("#ledgercode#", Model.ledgerCode);
                Model.WidgetsBGColor = "#db3456";
                Model.MidFontColor = "#fff";
                Model.widgetFontIcon = "icon-pie-chart";
                Model.ModuleCode = "01";
                Model.widgetsTitle = Model.widgetsName;
                Model.widgetsName = Model.widgetsName;
                Model.WidgetsBGColor="#db3456";
                 Model.widgetsColor= "#fff";
                Model.widgetFontIcon= "icon-pie-chart";
                Model.ChartType = "M"; Model.MidBGColor= "#db3456";  Model.MidFontColor = "#fff"; Model.MaxBGColor= "#db3456";
                Model.MaxFontColor = "#fff";
                Model.LABELPOSITION = "inside";
                Model.SPEEDOMETERMAXVALUE = "180";
                var order = 0;
                int.TryParse(Model.OrderNo, out order);
                var maxValue = 0;
                int.TryParse(Model.MaxValue, out maxValue);
                var minValue = 0;
                int.TryParse(Model.MinValue, out minValue);
                var isactive = Model.Isactive ? "Y" : "Y";
                var isHorizontal = Model.Horizontal ? "Y" : "N";
                var midIsblink = Model.IsMidBlink ? "Y" : "N";
                var maxIsblink = Model.IsMaxBlink ? "Y" : "N";
                var Isblink = Model.IsBlink ? "Y" : "N";
                var insertQuery = string.Format(@"INSERT INTO WEB_QUICKCAP
(Id, QUICKCAP_NO, QUICKCAP_EDESC, MODULE_CODE, SQL_STATEMENT, QUICKCAP_TITLE, QUICKCAP_BGCOLOR, QUICKCAP_FONTCOLOR, QUICKCAP_ICON,DELETED_FLAG,ISACTIVE,ORDERNO,QUICKTYPE,MIDBGCOLOR,
MIDFONTCOLOR,MIDISBLINK,MAXBGCOLOR,MAXFONTCOLOR,MAXISBLINK,MAXVALUES,MINVALUES,MINISBLINK,LABELPOSITION,SPEEDOMETERMAXVALUE,MAXVALUEQUERY,MINVALUEQUERY,IS_HORIZONTAL)
VALUES
({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','N','{9}','{10}','{11}','{12}','{13}','{14}','{15}','{16}','{17}','{18}','{19}','{20}','{21}','{22}','{23}','{24}','{25}')", "QuickCap_sequence.nextval", 01, Model.widgetsName, Model.ModuleCode, Model.sqlQuery, Model.widgetsTitle, Model.WidgetsBGColor, Model.widgetsColor, Model.widgetFontIcon, isactive, order
, Model.ChartType, Model.MidBGColor, Model.MidFontColor, midIsblink, Model.MaxBGColor, Model.MaxFontColor, maxIsblink, maxValue, minValue, Isblink, Model.LABELPOSITION, Model.SPEEDOMETERMAXVALUE, Model.MAXVALUEQUERY, Model.MINVALUEQUERY, isHorizontal);

                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                //var rowCounts = _dbContext.SqlQuery<int>(query).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        [HttpPost]
        public HttpResponseMessage UpdateWidzed(CreateMetricwidgets Model)
        {
            try
            {
                var order = 0;
                int.TryParse(Model.OrderNo, out order);
                var isactive = Model.Isactive ? "Y" : "N";
                var isHorizontal= Model.Horizontal ? "Y" : "N";

                //while updating the query, the deleted flag must be in double quote in input otherwise it will give error
                var updateQuery = string.Format(@"UPDATE WEB_QUICKCAP
SET SQL_STATEMENT = '{0}',
    QUICKCAP_TITLE = '{1}',
    QUICKCAP_BGCOLOR = '{2}',
 QUICKCAP_FONTCOLOR = '{3}',
 QUICKCAP_ICON = '{4}',
ISACTIVE = '{5}',
ORDERNO = '{6}' ,
MODULE_CODE='{8}',
IS_HORIZONTAL = '{9}'
where Id ='{7}'", Model.sqlQuery, Model.widgetsTitle, Model.WidgetsBGColor, Model.widgetsColor, Model.widgetFontIcon, isactive, order, Model.WidgetsId, Model.ModuleCode, isHorizontal);
                //                var insertQuery = string.Format(@"INSERT INTO WEB_QUICKCAP
                //(Id, QUICKCAP_NO, QUICKCAP_EDESC, MODULE_CODE, SQL_STATEMENT, QUICKCAP_TITLE, QUICKCAP_BGCOLOR, QUICKCAP_FONTCOLOR, QUICKCAP_ICON,DELETED_FLAG,ISACTIVE,ORDERNO)
                //VALUES
                //({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}','N','{9}','{10}')", "QuickCap_sequence.nextval", 01, Model.widgetsName, 01, Model.sqlQuery, Model.widgetsTitle, Model.WidgetsBGColor, Model.widgetsColor, Model.widgetFontIcon, isactive, order);
                if (!string.IsNullOrWhiteSpace(Model.WidgetsId))
                {
                    var rowCount = _dbContext.ExecuteSqlCommand(updateQuery);
                }
                //var rowCounts = _dbContext.SqlQuery<int>(query).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK });

            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }

        [HttpGet]
        public List<MetricWidgetsModel> QueryWidzedList()
        {

            var queryWizeds = _dashBoardMetricservice.GetMetricList();
            return queryWizeds;

        }

        [HttpPost]
        public HttpResponseMessage BuildNotification(NotificationBuilderModel model)
        {
            try
            {
                string Query = string.Empty;
                if (model.NotificationId == 0)//insert new row
                    Query = $@"INSERT INTO NOTIFICATIONS(ID,NOTIFICATION_EDESC,NOTIFICATION_RESULT,NOTIFICATION_TYPE,NOTIFICATION_TEMPLATE,APPEND_TEXT,APPEND_POSITION,SQL_STATEMENT,MAXVALUES,MINVALUES,MODULE_CODE,NOTIFICATION_COLOR,NOTIFICATION_ICON,USERPERMISSION,DELETED_FLAG,ISACTIVE)
                                VALUES(NOTIFICATION_SEQUENCE.NEXTVAL,'{model.NotificationName}','{model.NotificationResult}','{model.NotificationType}','{model.NotificationTemplate}','{model.AppendText}','{model.AppendPosition}','{model.SqlQuery}','{model.MaxResult}','{model.MinResult}','{model.ModuleCode}','{model.Color}','{model.Icon}','{model.Users}','N','Y')";
                else //update
                    Query = $@"UPDATE NOTIFICATIONS
                            SET NOTIFICATION_EDESC='{model.NotificationName}',NOTIFICATION_RESULT='{model.NotificationResult}',NOTIFICATION_TYPE='{model.NotificationType}',NOTIFICATION_TEMPLATE='{model.NotificationTemplate}',APPEND_TEXT='{model.AppendText}',
                            APPEND_POSITION='{model.AppendPosition}',SQL_STATEMENT='{model.SqlQuery}',MAXVALUES='{model.MaxResult}',MINVALUES='{model.MinResult}',MODULE_CODE='{model.ModuleCode}', NOTIFICATION_COLOR='{model.Color}',NOTIFICATION_ICON='{model.Icon}',USERPERMISSION='{model.Users}'
                            WHERE ID='{model.NotificationId}'";
                
                var row = _dbContext.ExecuteSqlCommand(Query);
                
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format(model.NotificationType, " successfully created"), STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }

        [HttpGet]
        public List<NotificationBuilderModel> NotificationList()
        {
            var data = _queryBuilder.NotificationList();
            return data;
        }
  

        [HttpPost]
        public HttpResponseMessage SaveReordingWidgets(List<ReordingReport> models)
        {
            
            try
            {
                if (models.Count > 0)
                {
                    var cacheNamagerKey = $"neoreopder_{models.FirstOrDefault().ReportName}";
                    this._cacheManager.Remove(cacheNamagerKey);
                    var QueryCacheName = $"neocacheQuery_{models.FirstOrDefault().ReportName}";
                    //var cacheNamagerKey = $"neoreopder_{models.FirstOrDefault().ReportName}";
                    this._cacheManager.Remove(QueryCacheName);
                    var delete = _dbContext.ExecuteSqlCommand($"delete from Web_widgets_reporder where ReportName='{models.FirstOrDefault().ReportName}'");
                    foreach (var model in models)
                    {
                        string Query = string.Empty;


                        var query = $@"insert into Web_widgets_reporder (ReportName,WidgetsId,xaxis,yaxis,height,width) values('{model.ReportName}','{model.widgetsId}','{model.XAxis}','{model.YAxis}','{model.Height}','{model.Width}')";
                        var row = _dbContext.ExecuteSqlCommand(query);
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format(" successfully created"), STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }
        }
    }
}