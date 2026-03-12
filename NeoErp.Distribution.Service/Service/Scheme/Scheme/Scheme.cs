using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Service.Scheme.models;
using System.Linq;

namespace NeoErp.Distribution.Service.Service.Scheme.Scheme
{
    public class Scheme : IScheme
    {
        public object ClaimOffer(RequestModel request, NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"SELECT *
                                    FROM SCHEME_OFFERS
                                    WHERE SYSDATE BETWEEN VALID_FROM AND VALID_TO;";
            var result = dbContext.SqlQuery<OffersModel>(offersQuery).ToList();
            if (result.Count > 0)
            {
                return new
                {
                    Success = true,
                    Message = "Scheme offers fetched successfully",
                    Data = result,

                };
            }
            else
            {
                return new
                {
                    Success = true,
                    Message = "No offers are active at this moment",
                };

            }
        }

        public object CreateOffer(RequestModel request, NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"INSERT INTO OFFER (
                            OFFER_ID,
                            OFFER_NAME,
                            OFFER_DESC,
                            VALID_FROM,
                            VALID_TO,
                            REQUIRED_POINTS,
                            OFFER_IMAGE
                        )
                        VALUES (
                            , 
                            {request.offer.OFFER_NAME},
                            {request.offer.OFFER_DESC},
                           {request.offer.VALID_FROM},
                            {request.offer.VALID_TO},
                            {request.offer.REQUIRED_POINTS},
                            'newyear.png'
                        );";
            var result = dbContext.SqlQuery<OffersModel>(offersQuery).ToList();
            if (result.Count > 0)
            {
                return new
                {
                    Success = true,
                    Message = "Scheme offers fetched successfully",
                    Data = result,

                };
            }
            else
            {
                return new
                {
                    Success = true,
                    Message = "No offers are active at this moment",
                };

            }
        }

        public object UpdateOffer(RequestModel request, NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"UPDATE SCHEME_OFFERS
                                SET 
                                    OFFER_NAME = {request.offer.OFFER_NAME},
                                    OFFER_DESC = {request.offer.OFFER_DESC},
                                    VALID_FROM = {request.offer.VALID_FROM},
                                    VALID_TO = {request.offer.VALID_TO},
                                    REQUIRED_POINTS ={request.offer.REQUIRED_POINTS},
                                    OFFER_IMAGE ={request.offer.OFFER_IMAGE},
                                WHERE 
                                    OFFER_ID = {request.offer.OFFER_ID}";
            var result = dbContext.SqlQuery<OffersModel>(offersQuery).ToList();
            if (result.Count > 0)
            {
                return new
                {
                    Success = true,
                    Message = "Scheme offers fetched successfully",
                    Data = result,

                };
            }
            else
            {
                return new
                {
                    Success = true,
                    Message = "No offers are active at this moment",
                };

            }
        }


    }
}
