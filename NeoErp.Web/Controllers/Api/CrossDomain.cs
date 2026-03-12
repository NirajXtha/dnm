using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace NeoErp.Controllers.Api
{
    public class CrossDomain : DelegatingHandler
    {
        const string Origin = "Origin";
        const string AccessControlRequestMethod = "Access-Control-Request-Method";
        const string AccessControlRequestHeaders = "Access-Control-Request-Headers";
        const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            bool isCorsRequest = request.Headers.Contains(Origin);
            bool isPreflightRequest = request.Method == HttpMethod.Options;
            if (isCorsRequest)
            {
                if (isPreflightRequest)
                {
                    return Task.Factory.StartNew(() =>
                    {
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        response.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());

                        string accessControlRequestMethod = request.Headers.GetValues(AccessControlRequestMethod).FirstOrDefault();
                        if (accessControlRequestMethod != null)
                        {
                            response.Headers.Add(AccessControlAllowMethods, accessControlRequestMethod);
                        }

                        string requestedHeaders = string.Join(", ", request.Headers.GetValues(AccessControlRequestHeaders));
                        if (!string.IsNullOrEmpty(requestedHeaders))
                        {
                            response.Headers.Add(AccessControlAllowHeaders, requestedHeaders);
                        }

                        return response;
                    }, cancellationToken);
                }
                else
                {
                    var data= base.SendAsync(request, cancellationToken).ContinueWith(t =>
                    {

                        HttpResponseMessage resp = t.Result;
                        resp.Headers.Add(AccessControlAllowOrigin, request.Headers.GetValues(Origin).First());
                        return resp;
                    });

                    try
                    {
                        if (data.Result.StatusCode != HttpStatusCode.OK)
                        {
                            var errormessage =  data.Result.Content.ReadAsStringAsync();

                            System.IO.FileStream fs = new System.IO.FileStream(System.Web.Hosting.HostingEnvironment.MapPath($"~/Log/Api/errorLOGApi{DateTime.Now.ToString("ddMMMyyyy")}.txt"), System.IO.FileMode.OpenOrCreate, System.IO.FileAccess.Write, System.IO.FileShare.ReadWrite);
                            System.IO.StreamWriter s = new System.IO.StreamWriter(fs);
                            s.BaseStream.Seek(0, System.IO.SeekOrigin.End);
                            s.WriteLine(@"ERROR DATE: {0} Source :{4}
                        ERROR Source: {1}
                        Erorr MEssage:{2}
                        Type: {3}",
                                     System.DateTime.Now.ToString(System.Globalization.CultureInfo.InvariantCulture),
                                    data.Result,
                                    errormessage.Result,
                                    System.Diagnostics.EventLogEntryType.Error,data.Result.RequestMessage.RequestUri);
                            s.WriteLine("-------------------------------------------------------------------------------------------------------------");
                            s.Close();
                        }
                    }
                    catch(Exception ex)
                    {
                        return data;
                    }


                    return data;
                }
            }
            else
            {
                return base.SendAsync(request, cancellationToken);
            }
            // return base.SendAsync(request, cancellationToken);
        }

    }
}