using NeoERP.DocumentTemplate.Service.Interface.ThirdPartyApi;
using NeoERP.DocumentTemplate.Service.Models.ThirdPartyApi;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;
namespace NeoERP.DocumentTemplate.Controllers.ThirdPartyApi
{
    public class OperaApiController : ApiController
    {
        private IOperaService _operaService;
        public OperaApiController(IOperaService operaService)
        {
            this._operaService = operaService;
        }
        [HttpPost]
        public IHttpActionResult Read(string formCode)
        {
            var httpRequest = HttpContext.Current.Request;
            Opera opera = new Opera();
            foreach (string file in httpRequest.Files)
            {
                //var postedFile = httpRequest.Files[file];
                //var filePath = HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Opera/" + postedFile.FileName);
                //string xmlFile = filePath;
                //string xmlString = System.IO.File.ReadAllText(xmlFile);
                //List<string> splitXmlString = xmlString.Split(new string[] { "<head>" }, StringSplitOptions.None).ToList<string>();
                //string concatHeadTag = "<head>" + splitXmlString[1];
                //string optimizedXML = "<root>" + concatHeadTag + "</root>";
                //var xml = XDocument.Parse(optimizedXML);

                var postedFile = httpRequest.Files[file];
                postedFile.SaveAs(HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Opera/" + postedFile.FileName));
                var filePath = HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Opera/" + postedFile.FileName);
                string xmlFile = filePath;
                string xmlstring = System.IO.File.ReadAllText(xmlFile);
                var xml = XDocument.Parse(xmlstring);
                //var newXmlFileName = xml.Descendants("head").Descendants("folio_print_task").Descendants("FOLIOPRINTTASK").FirstOrDefault().Value + "_" + xml.Descendants("head").Descendants("document_info").Descendants("FOLIOTYPE").FirstOrDefault().Value + "_" + xml.Descendants("head").Descendants("document_info").Descendants("BILLNUMBER").FirstOrDefault().Value + ".xml";
                //postedFile.SaveAs(HttpContext.Current.Server.MapPath(@"~/Areas/NeoERP.DocumentTemplate/xmlfile/Opera/" + newXmlFileName));
                var bodyXmlList = xml.Descendants("body").ToList();
                #region DocumentInfo 
                var documentInfoXmlList = xml.Descendants("head").Descendants("document_info").ToList();
                opera.DocumentInfo = new DocumentInfo();
                if (documentInfoXmlList.Count > 0)
                {
                    //opera.DocumentInfo.QUEUENAME = documentInfoXmlList.Descendants("QUEUENAME").FirstOrDefault().Value;
                    opera.DocumentInfo.BILLNUMBER = documentInfoXmlList.Descendants("BILLNUMBER").FirstOrDefault().Value;
                    opera.DocumentInfo.ASSOCIATED_BILL_NO = documentInfoXmlList.Descendants("ASSOCIATED_BILL_NO").FirstOrDefault().Value;
                    opera.DocumentInfo.WINDOW = documentInfoXmlList.Descendants("WINDOW").FirstOrDefault().Value;
                    opera.DocumentInfo.CASHIERNUMBER = documentInfoXmlList.Descendants("CASHIERNUMBER").FirstOrDefault().Value;
                }
                #endregion
                #region General
                var generalXmlList = xml.Descendants("head").Descendants("general").ToList();
                opera.General = new General();
                if (generalXmlList.Count > 0)
                {
                    //opera.General.HOTELNAME = generalXmlList.Descendants("HOTELNAME").FirstOrDefault().Value;
                    opera.General.ARRIVALDATE = generalXmlList.Descendants("ARRIVALDATE").FirstOrDefault().Value;
                    //opera.General.NUMBEROFNIGHTS = generalXmlList.Descendants("NUMBEROFNIGHTS").FirstOrDefault().Value;
                    opera.General.DEPARTUREDATE = generalXmlList.Descendants("DEPARTUREDATE").FirstOrDefault().Value;
                    //opera.General.USDPEXCHRATE = generalXmlList.Descendants("USDPEXCHRATE").FirstOrDefault().Value;
                    opera.General.ROOMNUMBER = generalXmlList.Descendants("ROOMNUMBER").FirstOrDefault().Value;
                }
                #endregion
                #region Guest
                var guestXmlList = xml.Descendants("head").Descendants("guest_info").ToList();
                opera.Guest = new Guest();
                if (guestXmlList.Count > 0)
                {
                    opera.Guest.GUESTNAMEID = guestXmlList.Descendants("GUESTNAMEID").FirstOrDefault().Value;
                    opera.Guest.FIRSTNAME = guestXmlList.Descendants("FIRSTNAME").FirstOrDefault().Value;
                    opera.Guest.LASTNAME = guestXmlList.Descendants("LASTNAME").FirstOrDefault().Value;
                }
                #endregion
                #region PositiveCharge
                var positiveChargeXmlList = xml.Descendants("body").Descendants("positive_charge").ToList();
                opera.PositiveCharge = new List<PositiveCharge>();
                if (positiveChargeXmlList.Count > 0)
                {
                    foreach (var pdata in positiveChargeXmlList)
                    {
                        PositiveCharge positiveCharge = new PositiveCharge();
                        positiveCharge.REFERENCE = pdata.Descendants("REFERENCE").FirstOrDefault().Value;
                        positiveCharge.DESCRIPTION = pdata.Descendants("DESCRIPTION").FirstOrDefault().Value;
                        positiveCharge.TYPE = pdata.Descendants("TYPE").FirstOrDefault().Value;
                        positiveCharge.CODE = pdata.Descendants("CODE").FirstOrDefault().Value;
                        positiveCharge.CURRENCY = pdata.Descendants("REFERENCE").FirstOrDefault().Value;
                        positiveCharge.EXCHANGE_RATE = pdata.Descendants("EXCHANGE_RATE").FirstOrDefault().Value;
                        positiveCharge.UNITPRICE = pdata.Descendants("UNITPRICE").FirstOrDefault().Value;
                        positiveCharge.NETAMOUNT = pdata.Descendants("NETAMOUNT").FirstOrDefault().Value;
                        positiveCharge.GROSSAMOUNT = pdata.Descendants("GROSSAMOUNT").FirstOrDefault().Value;
                        positiveCharge.TAX1AMOUNT = pdata.Descendants("TAX1AMOUNT").FirstOrDefault().Value;
                        positiveCharge.CODETYPE = pdata.Descendants("CODETYPE").FirstOrDefault().Value;
                        opera.PositiveCharge.Add(positiveCharge);
                    }
                }
                #endregion
                #region NegativeCharge
                var negativeChargeXmlList = xml.Descendants("body").Descendants("negative_charge").ToList();
                opera.NegativeCharge = new List<NegativeCharge>();
                if (negativeChargeXmlList.Count > 0)
                {
                    foreach (var negativeChargeData in negativeChargeXmlList)
                    {
                        opera.NegativeCharge.Add(new NegativeCharge
                        {
                            DESCRIPTION = negativeChargeData.Descendants("DESCRIPTION").FirstOrDefault().Value,
                            TYPE = negativeChargeData.Descendants("TYPE").FirstOrDefault().Value,
                            CODE = negativeChargeData.Descendants("CODE").FirstOrDefault().Value,
                            REFERENCE = negativeChargeData.Descendants("REFERENCE").FirstOrDefault().Value,
                            CURRENCY = negativeChargeData.Descendants("CURRENCY").FirstOrDefault().Value,
                            EXCHANGE_RATE = negativeChargeData.Descendants("EXCHANGE_RATE").FirstOrDefault().Value,
                            UNITPRICE = negativeChargeData.Descendants("UNITPRICE").FirstOrDefault().Value,
                            NETAMOUNT = negativeChargeData.Descendants("NETAMOUNT").FirstOrDefault().Value,
                            GROSSAMOUNT = negativeChargeData.Descendants("GROSSAMOUNT").FirstOrDefault().Value,
                            CODETYPE = negativeChargeData.Descendants("CODETYPE").FirstOrDefault().Value
                        });
                        //NegativeCharge negativeCharge = new NegativeCharge();
                        //negativeCharge.DESCRIPTION = negativeChargeData.Descendants("DESCRIPTION").FirstOrDefault().Value;
                        //negativeCharge.TYPE = negativeChargeData.Descendants("TYPE").FirstOrDefault().Value;
                        //negativeCharge.REFERENCE = negativeChargeData.Descendants("REFERENCE").FirstOrDefault().Value;
                        //negativeCharge.CURRENCY = negativeChargeData.Descendants("CURRENCY").FirstOrDefault().Value;
                        //negativeCharge.EXCHANGE_RATE = negativeChargeData.Descendants("EXCHANGE_RATE").FirstOrDefault().Value;
                        //negativeCharge.UNITPRICE = negativeChargeData.Descendants("UNITPRICE").FirstOrDefault().Value;
                        //negativeCharge.NETAMOUNT = negativeChargeData.Descendants("NETAMOUNT").FirstOrDefault().Value;
                        //negativeCharge.GROSSAMOUNT = negativeChargeData.Descendants("GROSSAMOUNT").FirstOrDefault().Value;
                        //opera.NegativeCharge.Add(negativeCharge);
                    }
                }
                #endregion
                #region Total
                var totalInfoXmlList = xml.Descendants("body").Descendants("total_info").ToList();
                opera.Total = new Total();
                if (totalInfoXmlList.Count > 0)
                {
                    opera.Total.TOTALNETAMOUNT = totalInfoXmlList.Descendants("TOTALNETAMOUNT").FirstOrDefault().Value;
                    opera.Total.TOTALGROSSAMOUNT = totalInfoXmlList.Descendants("TOTALGROSSAMOUNT").FirstOrDefault().Value;
                }
                #endregion
                #region Payment
                var paymentXmlList = xml.Descendants("body").Descendants("payment").ToList();
                opera.Payment = new List<Payment>();
                if (paymentXmlList.Count > 0)
                {
                    foreach (var paymentDate in paymentXmlList)
                    {
                        opera.Payment.Add(new Payment
                        {
                            DATE = paymentDate.Descendants("DATE").FirstOrDefault().Value,
                            CODE = paymentDate.Descendants("CODE").FirstOrDefault().Value,
                            TYPE = paymentDate.Descendants("TYPE").FirstOrDefault().Value,
                            CODETYPE = paymentDate.Descendants("CODETYPE").FirstOrDefault().Value,
                            DESCRIPTION = paymentDate.Descendants("DESCRIPTION").FirstOrDefault().Value,
                            REFERENCE = paymentDate.Descendants("REFERENCE").FirstOrDefault().Value,
                            CURRENCY = paymentDate.Descendants("CURRENCY").FirstOrDefault().Value,
                            EXCHANGE_RATE = paymentDate.Descendants("EXCHANGE_RATE").FirstOrDefault().Value,
                            QUANTITY = paymentDate.Descendants("QUANTITY").FirstOrDefault().Value,
                            UNITPRICE = paymentDate.Descendants("UNITPRICE").FirstOrDefault().Value,
                            TAXINCLUSIVEYN = paymentDate.Descendants("TAXINCLUSIVEYN").FirstOrDefault().Value,
                            //FISCALTRXCODETYPE = paymentDate.Descendants("FISCALTRXCODETYPE").FirstOrDefault().Value,
                            //CREDIT_CARD_NO = paymentDate.Descendants("CREDIT_CARD_NO").FirstOrDefault().Value,
                            NETAMOUNT = paymentDate.Descendants("NETAMOUNT").FirstOrDefault().Value,
                            GROSSAMOUNT = paymentDate.Descendants("GROSSAMOUNT").FirstOrDefault().Value,
                            //ARRANGEMENT_CODE = paymentDate.Descendants("ARRANGEMENT_CODE").FirstOrDefault().Value,
                            //ARRANGEMENT_DESCRIPTION = paymentDate.Descendants("ARRANGEMENT_DESCRIPTION").FirstOrDefault().Value,
                            //COUPON_NO = paymentDate.Descendants("COUPON_NO").FirstOrDefault().Value
                        });
                    }
                }
                #endregion
            }
            using (TransactionScope scope = new TransactionScope())
            {
                this._operaService.InsertInvoiceRecord(opera, formCode);
                scope.Complete();
            }
            return Ok(opera);
        }
    }
}