using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Service.Scheme.authentication;
using NeoErp.Distribution.Service.Service.Scheme.Master;
using NeoErp.Distribution.Service.Service.Scheme.models;
using NeoErp.Distribution.Service.Service.Scheme.Points;
using NeoErp.Distribution.Service.Service.Scheme.Scheme;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.Web;

namespace NeoErp.Distribution.Service.Service.Scheme
{
    public class SchemeServiceProvider : ISchemeServiceProvider
    {
        private IAuthenticationService _authenticationService;
        private IMasterService _masterService;
        private IPointsService _pointsService;
        private IScheme _schemeService;
        public SchemeServiceProvider(IAuthenticationService authenticationService, IPointsService pointsService, IScheme schemeService, IMasterService masterService)
        {
            _authenticationService = authenticationService;
            _masterService = masterService;
            _pointsService = pointsService;
        }
        public object SelectAction(JToken token, NeoErpCoreEntity dbContext)
        {
            object Output = new object();
            string Action = (string)token.SelectToken("action");
            if (Action == null)
                throw new Exception("Invalid action.");
            //else if (Action == "register")
            //{
            //    RegisterRequestModel registerRequestModel = (token.SelectToken("data")).ToObject<RegisterRequestModel>();
            //    return _authenticationService.Register(registerRequestModel, dbContext);
            //}
            else if (Action == "login")
            {
                LoginRequestModel loginRequestModel = (token.SelectToken("data")).ToObject<LoginRequestModel>();
                return _authenticationService.Login(loginRequestModel, dbContext);
            }
            else if (Action == "getItems")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.FetchItems(requestModel, dbContext);
            }
            else if (Action == "getUsers")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.FetchSchemeUsers(requestModel, dbContext);
            }
            else if (Action == "getDistributors")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.FetchEntity(requestModel, dbContext);
            }
            else if (Action == "getSchemes")
            {
                return _masterService.FetchSchemes(dbContext);
            }
            else if (Action == "getArea")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.FetchArea(requestModel, dbContext);
            }
            else if (Action == "createOffer")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.CreateOffer(requestModel, dbContext);
            }
            else if (Action == "linkKhalti")
            {
                LinkKhaltiRequestModel requestModel = (token.SelectToken("data")).ToObject<LinkKhaltiRequestModel>();
                return _masterService.LinkKhaltiNumber(requestModel, dbContext);
            }
            else if (Action == "loadKhalti")
            {
                LoadKhaltiRequestModel requestModel = (token.SelectToken("data")).ToObject<LoadKhaltiRequestModel>();
                return _pointsService.LoadKhalti(requestModel, dbContext);
            }
            else if (Action == "getPointsHistory")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _pointsService.FetchPointsHistory(requestModel, dbContext);
            }
            else if (Action == "getAllPointsHistory")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _pointsService.FetchAllPointsHistory(requestModel, dbContext);
            }
            else if (Action == "getRedeemHistory")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _pointsService.FetchRedeemHistory(requestModel, dbContext);
            }
            else if (Action == "generateQr")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.GenerateQr(requestModel, dbContext);
            }
            else if (Action == "updateQr")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.UpdateQr(requestModel, dbContext);
            }
            else if (Action == "claimQR")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _pointsService.ClaimPoints(requestModel, dbContext);
            }
            else if (Action == "updatePrinted")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.UpdateQrPrinted(requestModel, dbContext);
            }
            else if (Action == "fetchQR")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _masterService.GetUnprintedQr(requestModel, dbContext);
            }
            else if (Action == "redeemPoints")
            {
                RequestModel requestModel = (token.SelectToken("data")).ToObject<RequestModel>();
                return _pointsService.RedeemPoints(requestModel, dbContext);
            }
            else
            {
                throw new Exception("Invalid Action");
            }
        }



        public object SelectAction(NameValueCollection Form, HttpFileCollection Files, NeoErpCoreEntity dbContext)
        {
            if (Form == null)
                throw new ArgumentNullException(nameof(Form), "Form data cannot be null.");

            string action = Form["action"];

            if (string.IsNullOrWhiteSpace(action))
                throw new Exception("Invalid action.");

            switch (action.ToLower())
            {
                case "register":
                    {
                        // Parse form data properly
                        var parsed = Form;

                        var dataObject = new
                        {
                            firstName = parsed["firstName"],
                            middleName = parsed["middleName"],
                            lastName = parsed["lastName"],
                            address = parsed["address"],
                            mobileNo = parsed["mobileNo"],
                            dateOfBirth = parsed["dateOfBirth"],
                            profession = parsed["profession"],
                            emailId = parsed["emailId"],
                            password = parsed["password"]
                        };
                        // Convert anonymous object to JSON
                        string jsonData = JsonConvert.SerializeObject(dataObject);

                        // Deserialize JSON into your model
                        var registerRequestModel = JsonConvert.DeserializeObject<RegisterRequestModel>(jsonData);

                        if (registerRequestModel == null)
                            throw new Exception("Missing or invalid registration data.");

                        // Example call (uncomment when ready)
                        return _authenticationService.Register(registerRequestModel, Files, dbContext);

                        //return new { status = "success", message = "Registration processed." };
                    }

                default:
                    throw new Exception("Invalid action.");
            }
        }
    }

}
