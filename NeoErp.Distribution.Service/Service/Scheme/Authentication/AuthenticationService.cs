using NeoErp.Core.Models;
using NeoErp.Distribution.Service.Service.Scheme.authentication;
using NeoErp.Distribution.Service.Service.Scheme.models;
using System;
using System.IO;
using System.Linq;
using System.Web;

namespace NeoErp.Distribution.Service.Service.Scheme
{
    public class AuthenticationService : IAuthenticationService
    {
        public object Register(RegisterRequestModel model, HttpFileCollection files, NeoErpCoreEntity dbContext)
        {

            string imagePath = null;
            if (files != null && files.Count > 0)
            {
                HttpPostedFile file = files[0]; // Assuming single file upload

                if (file != null && file.ContentLength > 0)
                {
                    // Create directory if not exists
                    string uploadFolder = HttpContext.Current.Server.MapPath("~/Uploads/ProfilePictures/");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    // Generate a unique filename
                    string fileName = $"{model.FirstName}{model.MobileNo}{Path.GetExtension(file.FileName)}";
                    string fullPath = Path.Combine(uploadFolder, fileName);

                    // Save file to server
                    file.SaveAs(fullPath);

                    // Store relative path (for database)
                    imagePath = $@"/Uploads/ProfilePictures/{fileName}";
                }
            }

            String sql = $@"INSERT INTO SCHEME_USERS (
                                        id,
                                        first_name,
                                        middle_name,
                                        last_name,
                                        address,
                                        mobile_no,
                                        date_of_birth,
                                        profession,
                                        email_id,
                                        password,
                                        profile_picture
                                    ) VALUES (
                                        (SELECT COALESCE(MAX(ID), 0) + 1 FROM SCHEME_USERS),
                                        '{model.FirstName}',
                                        '{model.MiddleName}',
                                        '{model.LastName}',
                                        '{model.Address}',
                                        '{model.MobileNo}',
                                        TO_DATE('{model.DateOfBirth:MM/dd/yyyy}', 'MM/DD/YYYY'),
                                        '{model.Profession}',
                                        '{model.EmailId}',
                                        (select fn_encrypt_password('{model.Password}') from dual),
                                        '{imagePath}'
                                    )";
            try
            {
                var row = dbContext.SqlQuery(sql);
                //if (row > 0)
                //{
                //    return true;
                //}
                //else
                //{
                //    return false;
                //}

                return "User registered successfully";
            }
            catch (Exception ex)
            {

                if (ex.Message.Contains("UNIQUE_NUMBER"))
                {
                    return "Mobile Number is already being used";
                }
                return ex.Message;
            }
        }


        public object Login(LoginRequestModel model, NeoErpCoreEntity dbContext)
        {

            String sql = $@"select * from scheme_users where mobile_no = '{model.PhoneNumber}' and '{model.Password}' = fn_decrypt_password(password) AND ROWNUM = 1";
            try
            {
                var user = dbContext.SqlQuery<SchemeUser>(sql).FirstOrDefault();


                if (user != null)
                {

                    return new
                    {
                        Success = true,
                        Message = "Login successful",
                        Data = user
                    };
                }
                else
                {
                    return new
                    {
                        Success = false,
                        Message = "Invalid mobile number or password"
                    };
                }
            }
            catch (Exception ex)
            {

                return new
                {
                    Success = false,
                    Message = ex.Message.ToString(),
                };


            }
        }
    }
}
