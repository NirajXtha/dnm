using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Model.Mobile;
using NeoErp.Distribution.Service.Service.Scheme.models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace NeoErp.Distribution.Service.Service.Scheme.Master
{
    public class MasterService : IMasterService
    {
        private readonly IWorkContext _workContext;

        public MasterService(IWorkContext workContext)
        {
            _workContext = workContext;
        }


        public PreferenceModel FetchPreferences(string comp_code, NeoErpCoreEntity dbContext)
        {
            var PreferenceQuery = $"SELECT * FROM DIST_PREFERENCE_SETUP WHERE COMPANY_CODE='{comp_code}'";
            var preferences = dbContext.SqlQuery<PreferenceModel>(PreferenceQuery).FirstOrDefault();
            string tableCheckQuery = @"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'DIST_AUTO_TIME'";
            int tableCount = dbContext.SqlQuery<int>(tableCheckQuery).FirstOrDefault();
            if (tableCount <= 0)
            {
                string createTableQuery = @"
                CREATE TABLE dist_auto_time (
                id           NUMBER PRIMARY KEY,
                time         VARCHAR2(255),
                created_date DATE DEFAULT SYSDATE,
                Deleted_flag char(1),
                company_code VARCHAR2(255)
                )";
                dbContext.Database.ExecuteSqlCommand(createTableQuery);
            }
            var Query = $"SELECT time FROM dist_auto_time WHERE Deleted_flag='N'";
            var time = dbContext.SqlQuery<AutoTimeModel>(Query).FirstOrDefault();
            if (time != null)
            {
                preferences.AUTO_TIME = time.TIME;
            }
            return preferences;
        }

        public object FetchItems(RequestModel model, NeoErpCoreEntity dbContext)
        {
            var pref = FetchPreferences(model.COMPANY_CODE, dbContext);
            string ItemsQuery = string.Empty;
            ItemsQuery = $@"SELECT * FROM IP_ITEM_MASTER_SETUP";

            //AND IM.CATEGORY_CODE = '{CATEGORY_CODE}' AND IM.GROUP_SKU_FLAG = '{GROUP_SKU_FLAG}'

            var Items = dbContext.SqlQuery<ItemModel>(ItemsQuery).ToList();
            if (Items.Count <= 0)
                throw new Exception("No records found");
            return new
            {
                Success = true,
                Message = "Items Fetched",
                Data = Items
            };
        }
        public object FetchSchemeUsers(RequestModel model, NeoErpCoreEntity dbContext)
        {
            string sql = string.Empty;
            sql = $@"select * from scheme_users";

            var users = dbContext.SqlQuery<SchemeUser>(sql).ToList();
            if (users.Count <= 0)
                throw new Exception("No records found");
            return new
            {
                Success = true,
                Message = "Users Fetched Successfully",
                Data = users
            };
        }
        public object FetchEntity(RequestModel model, NeoErpCoreEntity dbContext)
        {
            var result = new Dictionary<string, List<EntityResponseModel>>();
            string EntityQuery = string.Empty;
            EntityQuery = $@"SELECT * FROM (
                  (SELECT
                     DM.DEALER_CODE AS CODE,
                     PT.PARTY_TYPE_EDESC AS NAME,
                     PT.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, DM.REG_OFFICE_ADDRESS AS ADDRESS,DM.EMAIL,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'dealer' AS TYPE,
                     'N' AS WHOLESELLER,
                     '' DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE,'' TYPE_EDESC,'' SUBTYPE_EDESC
                   FROM
                     DIST_DEALER_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DEALER_CODE AND RE.ENTITY_TYPE = 'P' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN IP_PARTY_TYPE_CODE PT ON PT.PARTY_TYPE_CODE = DM.DEALER_CODE AND PT.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN IP_PARTY_TYPE_CODE B ON B.PARTY_TYPE_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'P'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DEALER_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'
                  )
                UNION
                  (SELECT
                     DM.DISTRIBUTOR_CODE AS CODE,
                     CS.CUSTOMER_EDESC AS NAME,
                     CS.ACC_CODE,
                     DM.CONTACT_NO AS P_CONTACT_NO, '' AS P_CONTACT_NAME, NVL(DM.REG_OFFICE_ADDRESS, CS.REGD_OFFICE_EADDRESS) AS ADDRESS,DM.EMAIL,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,
                     NVL(DM.LATITUDE,0) LATITUDE, NVL(DM.LONGITUDE,0) LONGITUDE,
                     'distributor' AS TYPE,
                     'N' AS WHOLESELLER,
                     CS.PARTY_TYPE_CODE AS DEFAULT_PARTY_TYPE_CODE,
                     '' PARENT_DISTRIBUTOR_CODE,
                     '' PARENT_DISTRIBUTOR_NAME,
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     DM.COMPANY_CODE, DM.BRANCH_CODE,'' TYPE_EDESC,'' SUBTYPE_EDESC
                   FROM
                     DIST_DISTRIBUTOR_MASTER DM
                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = DM.DISTRIBUTOR_CODE AND RE.ENTITY_TYPE = 'D' AND RE.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = DM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = DM.AREA_CODE AND AM.COMPANY_CODE = DM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ) IS_VISITED, 
                                  A.REMARKS,
                                  TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN SA_CUSTOMER_SETUP B ON B.CUSTOMER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'D'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = DM.DISTRIBUTOR_CODE AND LT.COMPANY_CODE = DM.COMPANY_CODE
                   WHERE 1 = 1
                         AND DM.COMPANY_CODE = '{model.COMPANY_CODE}'
                  )
                  UNION
                  (SELECT
                     REM.RESELLER_CODE AS CODE,
                     REM.RESELLER_NAME AS NAME,
                     '' AS ACC_CODE,
                     REM.CONTACT_NO AS P_CONTACT_NO, REM.CONTACT_NAME AS P_CONTACT_NAME, REM.REG_OFFICE_ADDRESS AS ADDRESS,REM.EMAIL,
                     RM.ROUTE_CODE, RM.ROUTE_NAME,
                     AM.AREA_CODE, AM.AREA_NAME,   
                     NVL(REM.LATITUDE,0) LATITUDE, NVL(REM.LONGITUDE,0) LONGITUDE,
                     'reseller' AS TYPE,
                     REM.WHOLESELLER AS WHOLESELLER,
                     '' AS DEFAULT_PARTY_TYPE_CODE,
                     DISTRIBUTOR_CODE AS PARENT_DISTRIBUTOR_CODE,
                     CS.CUSTOMER_EDESC AS PARENT_DISTRIBUTOR_NAME,   s
                     LT.LAST_VISIT_DATE, LT.LAST_VISIT_BY, LT.LAST_VISIT_STATUS, NVL(LT.IS_VISITED, 'X') AS IS_VISITED, LT.REMARKS,
                     REM.COMPANY_CODE, REM.BRANCH_CODE,
                     DOT.TYPE_EDESC,DOS.SUBTYPE_EDESC
                   FROM DIST_RESELLER_MASTER REM

                     LEFT JOIN DIST_ROUTE_ENTITY RE ON RE.ENTITY_CODE = REM.RESELLER_CODE AND RE.ENTITY_TYPE = 'R' AND RE.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN DIST_ROUTE_MASTER RM ON RM.ROUTE_CODE = RE.ROUTE_CODE AND RM.COMPANY_CODE = REM.COMPANY_CODE
                     INNER JOIN DIST_AREA_MASTER AM ON AM.AREA_CODE = REM.AREA_CODE AND AM.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN SA_CUSTOMER_SETUP CS ON CS.CUSTOMER_CODE = REM.DISTRIBUTOR_CODE AND CS.COMPANY_CODE = REM.COMPANY_CODE
                     LEFT JOIN (SELECT A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC AS LAST_VISIT_BY, A.IS_VISITED AS LAST_VISIT_STATUS,
                                 (CASE 
                                        WHEN A.IS_VISITED IS NULL THEN 'X' 
                                          ELSE
                                            CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                       END
                                      ) IS_VISITED, 
                                 A.REMARKS,
                                 TO_CHAR(MAX(A.UPDATE_DATE), 'RRRR-MM-DD HH24:MI A.M.') LAST_VISIT_DATE
                                FROM DIST_LOCATION_TRACK A
                                  INNER JOIN DIST_RESELLER_MASTER B ON B.RESELLER_CODE = A.CUSTOMER_CODE AND B.COMPANY_CODE = A.COMPANY_CODE
                                  INNER JOIN HR_EMPLOYEE_SETUP C ON C.EMPLOYEE_CODE = A.SP_CODE AND C.COMPANY_CODE = A.COMPANY_CODE
                                WHERE 1 = 1
                                      AND A.CUSTOMER_TYPE = 'R'
                                      AND A.UPDATE_DATE = (SELECT MAX(UPDATE_DATE) FROM DIST_LOCATION_TRACK WHERE CUSTOMER_CODE = A.CUSTOMER_CODE AND CUSTOMER_TYPE = A.CUSTOMER_TYPE)
                                GROUP BY A.SP_CODE, A.CUSTOMER_CODE, A.CUSTOMER_TYPE, A.COMPANY_CODE, C.EMPLOYEE_EDESC, A.IS_VISITED, 
                                  (CASE 
                                    WHEN A.IS_VISITED IS NULL THEN 'X' 
                                      ELSE
                                        CASE WHEN TO_CHAR(SYSDATE, 'DD-MON-RRRR') = TO_CHAR(A.UPDATE_DATE, 'DD-MON-RRRR') THEN A.IS_VISITED ELSE 'X' END
                                   END
                                  ), 
                                  A.REMARKS
                               ) LT
                       ON LT.CUSTOMER_CODE = REM.RESELLER_CODE AND LT.COMPANY_CODE = REM.COMPANY_CODE
               LEFT JOIN DIST_OUTLET_TYPE DOT ON REM.OUTLET_TYPE_ID=DOT.TYPE_ID AND REM.COMPANY_CODE=DOT.COMPANY_CODE
               LEFT JOIN DIST_OUTLET_SUBTYPE DOS ON REM.OUTLET_SUBTYPE_ID=DOS.SUBTYPE_ID AND REM.COMPANY_CODE=DOS.COMPANY_CODE
                   WHERE 1 = 1
                         AND REM.COMPANY_CODE = '{model.COMPANY_CODE}'
                  )
                )
                ORDER BY UPPER(NAME)";
            var Entities = dbContext.SqlQuery<EntityResponseModel>(EntityQuery).GroupBy(x => x.TYPE);
            if (Entities.Count() <= 0)
                throw new Exception("No records found");
            foreach (var EntGroup in Entities)
            {
                result.Add(EntGroup.Key, EntGroup.ToList());
            }

            return (result);
        }
        public object FetchSchemes(NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"SELECT *
                                    FROM SCHEME_OFFERS
                                    WHERE SYSDATE BETWEEN VALID_FROM AND VALID_TO";
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
        public object FetchArea(RequestModel model, NeoErpCoreEntity dbContext)
        {
            var AreaQuery = $@"SELECT A.AREA_CODE,A.AREA_NAME,A.ZONE_CODE,A.DISTRICT_CODE,A.VDC_CODE,A.REG_CODE,
                       B.ZONE_NAME,B.DISTRICT_NAME,B.VDC_NAME,B.REG_NAME
                FROM DIST_AREA_MASTER A,DIST_ADDRESS_MASTER B
                WHERE (A.DISTRICT_CODE = B.DISTRICT_CODE
                AND  A.REG_CODE = B.REG_CODE
                AND  A.ZONE_CODE = B.ZONE_CODE
                AND  A.VDC_CODE = B.VDC_CODE) AND A.COMPANY_CODE = '{model.COMPANY_CODE}' ORDER BY UPPER(A.AREA_NAME) ASC";

            var result = dbContext.SqlQuery<AreaResponseModel>(AreaQuery).ToList();
            if (result.Count <= 0)
                throw new Exception("No records found");
            return new
            {
                Success = true,
                Message = "Area fetched successfully",
                Data = result
            };
        }

        public object GenerateQr(RequestModel models, NeoErpCoreEntity dbContext)
        {
            if (models == null || models.data.count == 0)
                throw new Exception("No data provided");

            var item = models.data;

            // Get the starting QR_ID once
            var getMaxIdQuery = "SELECT NVL(MAX(QR_ID), 0) FROM SCHEME_QR";
            var maxId = dbContext.SqlQuery<int>(getMaxIdQuery).FirstOrDefault();
            var startingId = maxId + 1;

            // Build INSERT ALL statement with correct incremental IDs
            var queryBuilder = new StringBuilder();
            queryBuilder.AppendLine("INSERT ALL");

            for (int i = 0; i < models.data.count; i++)
            {
                var qrId = startingId + i;
                queryBuilder.AppendLine(
                    "INTO SCHEME_QR " +
                    "(QR_ID, QR_DATA, REDEMABLE_POINTS, VALID_FROM, VALID_TO, IS_CLAIMED, ITEM_CODE, CLAIMED_BY, CLAIMED_DATE, IS_PRINTED) " +
                    "VALUES (" +
                    $"{qrId}, " +
                    "'" + item.item_code + "qr', " +
                    item.points + ", " +
                    "TO_DATE('" + item.from_date.Value.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD'), " +
                    "TO_DATE('" + item.to_date.Value.ToString("yyyy-MM-dd") + "', 'YYYY-MM-DD'), " +
                    "'N', " +
                    "'" + item.item_code + "', " +
                    "NULL, NULL, 'Y')"
                );
            }

            queryBuilder.AppendLine("SELECT * FROM dual");

            var rowsAffected = dbContext.Database.ExecuteSqlCommand(queryBuilder.ToString());

            if (rowsAffected <= 0)
                throw new Exception("QR generation failed");

            // Fetch the newly inserted QR codes using ROWNUM (Oracle syntax)
            var fetchQuery = $@"SELECT * FROM (
                    SELECT *
                    FROM scheme_qr
                    WHERE ITEM_CODE = '{item.item_code}'
                        AND VALID_FROM = TO_DATE('{item.from_date.Value.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD')
                        AND VALID_TO = TO_DATE('{item.to_date.Value.ToString("yyyy-MM-dd")}', 'YYYY-MM-DD')
                        AND REDEMABLE_POINTS = {item.points}
                        --AND IS_PRINTED = 'N'
                    ORDER BY QR_ID DESC
                ) WHERE ROWNUM <= {models.data.count}";

            var insertedQrCodes = dbContext.SqlQuery<SchemeQR>(fetchQuery).ToList();

            var resultData = insertedQrCodes.Select(x => new
            {
                x.QR_ID,
                x.QR_DATA,
                x.REDEMABLE_POINTS,
                x.VALID_FROM,
                x.VALID_TO,
                x.IS_CLAIMED,
                x.ITEM_CODE,
                x.CLAIMED_BY,
                x.CLAIMED_DATE,
                x.IS_PRINTED,
                // Encoded QR code data for display
                ENCODED_DATA = EncodeToBase64(new
                {
                    key = "JANTA",
                    id = x.QR_ID
                })
            });

            return new
            {
                Success = true,
                Message = $"{models.data.count} QR codes generated successfully",
                Data = resultData.ToArray()
            };
        }

        public object UpdateQr(RequestModel models, NeoErpCoreEntity dbContext)
        {
            if (models == null || models.data.count == 0)
                throw new Exception("No data provided");

            var item = models.data;

            var query = $@"UPDATE SCHEME_QR SET REDEMABLE_POINTS = {item.points}" +
                                            (item.from_date != null ? $@",VALID_FROM = TO_DATE('{item.from_date}', 'YYYY-MM-DD')" : string.Empty) +
                                            (item.to_date != null ? $@",VALID_TO = TO_DATE('{item.to_date}', 'YYYY-MM-DD')" : string.Empty) +
                                                $" WHERE ITEM_CODE = '{item.item_code}' AND IS_CLAIMED = 'N'";

            var rowsAffected = dbContext.Database.ExecuteSqlCommand(query);

            if (rowsAffected <= 0)
                throw new Exception("QR generation failed");

            return new
            {
                Success = true,
                Message = $"{models.data.count} QR codes generated successfully"
            };
        }

        public object GetUnprintedQr(RequestModel models, NeoErpCoreEntity dbContext)
        {
            string ItemsQuery = string.Empty;
            ItemsQuery = $@"SELECT *
                    FROM scheme_qr
                    WHERE SYSDATE < VALID_TO
                        AND is_printed = 'N'";

            var Items = dbContext.SqlQuery<SchemeQR>(ItemsQuery).ToList();
            if (Items.Count <= 0)
                throw new Exception("No records found");
            var result = Items.Select(x => new
            {
                x.QR_ID,
                x.QR_DATA,
                x.REDEMABLE_POINTS,
                x.VALID_FROM,
                x.VALID_TO,
                x.IS_CLAIMED,
                x.ITEM_CODE,
                x.CLAIMED_BY,
                x.CLAIMED_DATE,
                x.IS_PRINTED,

                // New encoded field
                ENCODED_DATA = EncodeToBase64(new
                {
                    key = "JANTA",
                    id = x.QR_ID
                })
            });

            return new
            {
                Success = true,
                Message = "Items Fetched",
                Data = result.ToArray()
            };
        }


        public object CreateOffer(RequestModel request, NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"INSERT INTO SCHEME_OFFERS (
                            OFFER_ID,
                            OFFER_NAME,
                            OFFER_DESC,
                            VALID_FROM,
                            VALID_TO,
                            REQUIRED_POINTS,
                            OFFER_IMAGE
                        )
                        VALUES (
                            (SELECT COALESCE(MAX(OFFER_ID), 0) + 1 FROM SCHEME_OFFERS),
                            '{request.offer.OFFER_NAME}',
                            '{request.offer.OFFER_DESC}',
                            TO_DATE('{request.offer.VALID_FROM:yyyy-MM-dd}', 'YYYY-MM-DD'),
                            TO_DATE('{request.offer.VALID_TO:yyyy-MM-dd}', 'YYYY-MM-DD'),
                            {request.offer.REQUIRED_POINTS},
                            ''
                        )";
            var result = dbContext.ExecuteSqlCommand(offersQuery);
            if (result != 0)
            {
                return new
                {
                    Success = true,
                    Message = "Scheme offers inserted successfully",
                    Data = result,

                };
            }
            else
            {
                return new
                {
                    Success = true,
                    Message = "No offers are inserted at this moment",
                };

            }
        }


        public object LinkKhaltiNumber(LinkKhaltiRequestModel request, NeoErpCoreEntity dbContext)
        {
            var offersQuery = $@"UPDATE SCHEME_USERS
                    SET KHALTI_ACCOUNT = '{request.khalti_account}'
                    WHERE ID = '{request.user_id}'";
            var result = dbContext.ExecuteSqlCommand(offersQuery);
            if (result != 0)
            {
                return new
                {
                    Success = true,
                    Message = "Khalti Account Linked Successfully",
                    Data = result,

                };
            }
            else
            {
                return new
                {
                    Success = true,
                    Message = "Error Linking Khalti Account",
                };

            }
        }
        public static string EncodeToBase64(object data)
        {
            string json = JsonConvert.SerializeObject(data);
            return Encrypt(json);
        }



        public object UpdateQrPrinted(RequestModel model, NeoErpCoreEntity dbContext)
        {
            var qrIds = model.qrList;
            if (qrIds == null)
                throw new Exception("No QR IDs provided");

            // Convert list to comma-separated string for SQL IN clause
            string idList = string.Join(",", qrIds);

            string updateQuery = $@"
                    UPDATE scheme_qr
                    SET is_printed = 'Y'
                    WHERE qr_id IN ({idList})";

            int rowsAffected = dbContext.ExecuteSqlCommand(updateQuery);

            return new
            {
                Success = true,
                Message = $"{rowsAffected} QRs updated as printed",
                UpdatedCount = rowsAffected
            };
        }


        #region
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("12345678901234567890123456789012");
        // Must be exactly 32 characters!

        public static string Encrypt(string plainText)
        {
            if (plainText == null) return null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                aes.GenerateIV(); // new IV for every encryption
                byte[] iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                using (var ms = new MemoryStream())
                {
                    // Write IV first
                    ms.Write(iv, 0, iv.Length);

                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);
                    }

                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }

        #endregion



    }
}



//SELECT IM.ITEM_CODE, IM.ITEM_EDESC, ISS.BRAND_NAME, IM.INDEX_MU_CODE AS UNIT, MC.MU_EDESC, IUS.MU_CODE CONVERSION_UNIT,
//                TO_CHAR(IUS.CONVERSION_FACTOR) AS CONVERSION_FACTOR, TO_CHAR(NVL(IR.SALES_RATE, 0)) SALES_RATE, TO_CHAR(IR.APPLY_DATE) AS APPLY_DATE
//                FROM IP_ITEM_MASTER_SETUP IM
//                  INNER JOIN IP_MU_CODE MC ON MC.MU_CODE = IM.INDEX_MU_CODE AND MC.COMPANY_CODE = IM.COMPANY_CODE
//                  INNER JOIN IP_ITEM_SPEC_SETUP ISS ON ISS.ITEM_CODE = IM.ITEM_CODE AND ISS.COMPANY_CODE = IM.COMPANY_CODE AND TRIM(ISS.BRAND_NAME) IS NOT NULL
//                  LEFT JOIN IP_ITEM_UNIT_SETUP IUS ON IUS.ITEM_CODE = ISS.ITEM_CODE AND IUS.COMPANY_CODE = ISS.COMPANY_CODE
//                  LEFT JOIN (SELECT A.ITEM_CODE, A.APPLY_DATE, B.SALES_RATE, B.COMPANY_CODE
//                              FROM (SELECT ITEM_CODE, COMPANY_CODE, MAX(APP_DATE) APPLY_DATE 
//                                FROM IP_ITEM_RATE_APPLICAT_SETUP
//                                WHERE COMPANY_CODE = '{model.COMPANY_CODE}' 
//                                AND BRANCH_CODE = '{model.BRANCH_CODE}'
//                                GROUP BY ITEM_CODE, COMPANY_CODE) A
//                              INNER JOIN IP_ITEM_RATE_APPLICAT_SETUP B
//                                ON B.ITEM_CODE = A.ITEM_CODE
//                                AND B.APP_DATE = A.APPLY_DATE
//                                AND B.COMPANY_CODE = '{model.COMPANY_CODE}'
//                                AND B.BRANCH_CODE = '{model.BRANCH_CODE}') IR 
//                    ON IR.ITEM_CODE = IM.ITEM_CODE AND IR.COMPANY_CODE = IM.COMPANY_CODE
//                WHERE IM.COMPANY_CODE = '{model.COMPANY_CODE}' AND IM.DELETED_FLAG = 'N'
//                {salesClause}
//                { conversionClause}
//ORDER BY UPPER(IM.ITEM_EDESC) ASC