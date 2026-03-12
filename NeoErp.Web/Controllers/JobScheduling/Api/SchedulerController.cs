using NeoErp.Core;
using NeoErp.Core.Helpers;
using NeoErp.Core.Services;
using NeoErp.Data;
using NeoErp.Models.Common;
using NeoErp.Models.QueryBuilder;
using NeoErp.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using static System.Net.WebRequestMethods;

namespace NeoErp.Controllers.JobScheduling.Api
{
    public class SchedulerController : ApiController
    {
        private IDbContext _dbContext;
        private IMessageService _messageService;
        private IUserChartPermission _userChartPermission;

        public SchedulerController(IDbContext dbContext, IMessageService messageService, IUserChartPermission userChartPermission)
        {
            this._dbContext = dbContext;
            this._messageService = messageService;
            this._userChartPermission = userChartPermission;
        }

        [HttpPost]
        public HttpResponseMessage CreateSchedular(SchedularMail Model)
        {
            try
            {
                var sqlquery = "select max(MESSAGE_QUEUE_CODE)+1 max  from WEB_SCHEDULARMAIL_QUEUE";
                var maxvalue = 0;
                var rowfound = _dbContext.SqlQuery<int?>(sqlquery).FirstOrDefault();
                if (rowfound == null)
                    maxvalue = 1;
                else
                    maxvalue = rowfound ?? 1;
                if (Model.sqltype != "inline")
                    Model.ATTACHMENT_FLAG = "Y";
                var newQry = Model.sqlQuery;

                if (Model.isTemplate)
                {
                    var isSuccess = generateTemplateAsFile(Model.schedularName, Model.Template);
                    if (isSuccess)
                        Model.Template = $@"{Model.schedularName }.html";
                    else
                        return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = new Exception($@"File name: {Model.schedularName} already exists."), STATUS_CODE = (int)HttpStatusCode.InternalServerError });
                }

                Model.Template = EncodeDecodeHtml.HtmEncode(Model.Template);

                //var tempLen = Model.Template.Length;
                //var tempArr = Split(Model.Template, 4000);
                //var templateChunk = new List<string>();
                //foreach (var temp in tempArr)
                //{
                //    string chunk = $@"to_clob( '{temp}' )";
                //    templateChunk.Add(chunk);
                //}
                //Model.Template = string.Join("||", templateChunk);

                if (!string.IsNullOrEmpty(newQry))
                    newQry = newQry.Replace("'", "''").Replace(@"""", "\"");
                var fequency = ((char)FequencyTypeEnum.daily).ToString();
                if (Model.fequencytype == "onetime")
                    fequency = ((char)FequencyTypeEnum.onetime).ToString();
                else if (Model.fequencytype == "fequencyrange")
                    fequency = ((char)FequencyTypeEnum.fequencyrange).ToString();
                if (string.IsNullOrEmpty(Model.startdate))
                    Model.startdate = DateTime.Now.ToString();
                DateTime startDateTime = DateTime.Now;
                DateTime.TryParse(Model.startdate, out startDateTime);
                if (Model.fequencytype != "fequencyrange")
                    Model.Days = 0;
                int Daysint = 0;
                int.TryParse(Model.Days.ToString(), out Daysint);
                var employeeCode = "";
                if (Model.employeeCode.Count() > 0)
                    employeeCode = string.Join(",", Model.employeeCode);

                string insertQuery = string.Format(@"INSERT INTO WEB_SCHEDULARMAIL_QUEUE 
(MESSAGE_QUEUE_CODE,GUID, EMAIL_TO,EMAIL_BCC, EMAIL_CC,ATTACHMENT_FLAG,MESSAGE_FLAG,DELETE_FLAG,CREATED_BY,CREATED_DATE,MESSAGE,SUBJECT,SCHEDULAR_NAME,SCHEDULAR_TYPE,FREQUENCY_FLAG,SCHEDULAR_TEMPLATE,TIME_FRAME,EMPLOYEE_CODE,REPORT_NAME,REPORT_TYPE)
                            VALUES('{13}','{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',TO_DATE('{8}', 'yyyy/mm/dd hh24:mi:ss'),'{9}','{10}','{11}','{14}','" + fequency + "','{12}','" + Daysint + "','{15}','{16}','{17}')",
                           Model.UniqueKey, Model.email, Model.EMAIL_BCC, Model.EMAIL_CC, Model.ATTACHMENT_FLAG, ((char)MessageProcessStatus.Draft).ToString(),
                           YesNo.NO.ToString().FirstOrDefault(),
                           "bikalptest",
                            startDateTime.ToString("yyyy/MM/dd/ HH:mm:ss"), newQry, Model.Subject, Model.schedularName, Model.Template, maxvalue, Model.sqltype, employeeCode, Model.reportName, Model.reportType);
                var rowCount = _dbContext.ExecuteSqlCommand(insertQuery);
                //var rowCounts = _dbContext.SqlQuery<int>(query).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, new { MESSAGE = string.Format("Succesfully "), STATUS_CODE = (int)HttpStatusCode.OK });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, new { MESSAGE = ex.Message, STATUS_CODE = (int)HttpStatusCode.InternalServerError });
            }

        }
        public bool generateTemplateAsFile(string filename, string template)
        {
            var fileGenerated = false;
            var path = HttpContext.Current.Server.MapPath($@"~/App_Data/SchedularMailTemplae/{filename}.html");
            if (!System.IO.File.Exists(path))
            {
                using (var tw = new System.IO.StreamWriter(path, true))
                {
                    tw.WriteLine(template);
                    tw.Flush();
                    fileGenerated = true;
                }
            }
            else
            {
                using (var tw = new System.IO.StreamWriter(path))
                {
                    tw.WriteLine(template);
                    tw.Flush();
                    fileGenerated = true;
                }
            }
            return fileGenerated;
        }
        static IEnumerable<string> Split(string str, int chunkSize)
        {
            return Enumerable.Range(0, str.Length / chunkSize)
                .Select(i => str.Substring(i * chunkSize, chunkSize));
        }
        public List<EmployeeSchedular> GetEmployeeListForScheduler()
        {
            return this._userChartPermission.GetEmployeesListForScheduler().ToList();
        }
        public List<EmployeeSchedular> GetEmployeeList()
        {
            return this._userChartPermission.GetEmployeesList().ToList();
        }
        public List<Customers> GetAllCustomers()
        {
            return this._userChartPermission.GetAllCustomers().ToList();
        }
        public List<Customers> GetAllCustomerswithoutlet()
        {
            return this._userChartPermission.GetAllCustomersWithOutlet().ToList();

            //List<Customers> clist= this._userChartPermission.GetAllCustomersWithOutlet().ToList();
            //Customers c = new Customers();
            //c.CustomerCode = "0";
            //c.CustomerName = "ALL";
            //c.EMAIL = "";
            //clist.Add(c);
            //return clist.OrderBy(x => x.CustomerCode).ToList(); 
        }
        public List<Dropdownsmodel> GetAllItems()
        {
            return this._userChartPermission.GetAllItems().ToList();
        }
        public List<Dropdownsmodel> GetAllSuppliers()
        {
            return this._userChartPermission.GetAllSuppliers().ToList();
        }
        public List<Dropdownsmodel> GetAllDivisoin()
        {
            return this._userChartPermission.GetAllDivision().ToList();
        }
        public List<Dropdownsmodel> GetAllLEDGERS()
        {
            return this._userChartPermission.GetAllLedgers().ToList();
        }
        [HttpGet]
        public void SendMail()
        {
            this._messageService.SendSchedularMail();
        }
        [HttpPost]
        public HttpResponseMessage FileUpload()
        {
            HttpResponseMessage result = null;
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var docfiles = new List<string>();
                foreach (string file in httpRequest.Files)
                {
                    var postedFile = httpRequest.Files[file];
                    int hasheddate = DateTime.Now.GetHashCode();
                    //Good to use an updated name always, since many can use the same file name to upload.
                    string changed_name = hasheddate.ToString() + "_" + postedFile.FileName;

                    var filePath = HttpContext.Current.Server.MapPath("~/SchedularTemplateImage/" + changed_name);
                    postedFile.SaveAs(filePath); // save the file to a folder "Images" in the root of your app
                    generateImageNameList(changed_name);
                    changed_name = @"~\SchedularTemplateImage\" + changed_name; //store this complete path to database
                    docfiles.Add(changed_name);


                }
                result = Request.CreateResponse(HttpStatusCode.Created, docfiles);
            }
            else
            {
                result = Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            return result;
        }
        public void generateImageNameList(string imageName)
        {
            var path = HttpContext.Current.Server.MapPath(@"~/SchedularTemplateImage/ImagesNameList.txt");
            if (!System.IO.File.Exists(path))
            {
                using (var tw = new System.IO.StreamWriter(path, true))
                {
                    tw.WriteLine(imageName);
                    tw.Flush();
                }
            }
            else
            {
                using (var tw = new System.IO.StreamWriter(path, true))
                {
                    tw.WriteLine(imageName);
                    tw.Flush();
                }
            }
        }
        [HttpPost]
        public async Task<HttpResponseMessage> UploadFile(string id)
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "The request doesn't contain valid content!");
            }

            try
            {
                var provider = new MultipartMemoryStreamProvider();
                await Request.Content.ReadAsMultipartAsync(provider);
                var response = Request.CreateResponse(HttpStatusCode.OK);
                foreach (var file in provider.Contents)
                {
                    var dataStream = await file.ReadAsStreamAsync();
                    // use the data stream to persist the data to the server (file system etc)


                    response.Content = new StringContent("Successful upload", Encoding.UTF8, "text/plain");
                    response.Content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(@"text/html");
                }
                return response;
            }
            catch (Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e.Message);
            }
        }

        [HttpGet]
        public List<string> GetImages()
        {
            var imageList = new List<string>();
            var path = HttpContext.Current.Server.MapPath(@"~/SchedularTemplateImage/ImagesNameList.txt");
            if (System.IO.File.Exists(path))
            {
                using (System.IO.StreamReader readtext = new System.IO.StreamReader(path))
                {
                    var line = string.Empty;
                    while ((line = readtext.ReadLine()) != null)
                    {
                        imageList.Add(line);
                    }
                }
            }
            return imageList;
        }
        [HttpPost]
        public string DeleteImage(string name)
        {
            var result = "Failed";
            try
            {
                var path = HttpContext.Current.Server.MapPath(@"~/SchedularTemplateImage/ImagesNameList.txt");
                string[] readText = System.IO.File.ReadAllLines(path);

                using (StreamWriter writer = new StreamWriter(path))
                {
                    foreach (string s in readText)
                    {
                        if (!s.Equals(name))
                        {
                            writer.WriteLine(s);
                        }
                    }
                }
                result = "Success";
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return result;
        }
    }
}
