using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NeoERP.DocumentTemplate
{
    public class UserSecurity
    {
        private NeoErpCoreEntity _coreEntity;
        // private IDbContext _dbContext;
        public UserSecurity(NeoErpCoreEntity coreEntity)
        {
            _coreEntity = coreEntity;
        }
        public static bool Login(string username, string password)
        {
            using (NeoErpCoreEntity _coreEntity = new NeoErpCoreEntity())
            //using (NeoErpCoreEntity _coreEntity = NeoErpCoreEntity.CreateForSelectedDatabase())

            {
                //var query = $@"select LOGIN_CODE,PASSWORD from sc_application_users";
                //var entity = _coreEntity.SqlQuery<UserModel>(query).FirstOrDefault(usr => usr.LOGIN_CODE == username);
                //var decryptPasswordQuery = $@"select  FN_DECRYPT_PASSWORD('{entity.PASSWORD}') from dual";
                //var pass = _coreEntity.SqlQuery<string>(decryptPasswordQuery).FirstOrDefault();
                //return entity.LOGIN_CODE.Equals(username, StringComparison.OrdinalIgnoreCase) && pass == password;

                var query = $@"select LOGIN_CODE,PASSWORD from sc_application_users";
                var entity = _coreEntity.SqlQuery<UserModel>(query).ToList().Where(usr => usr.LOGIN_CODE == username);
                List<string> pass = new List<string>();
                List<string> use = new List<string>();
                foreach (var decryptpassword in entity)
                {
                    var decryptPasswordQuery = $@"select  FN_DECRYPT_PASSWORD('{decryptpassword.PASSWORD}') from dual";
                    var pass2 = _coreEntity.SqlQuery<string>(decryptPasswordQuery).FirstOrDefault();
                    pass.Add(pass2);
                    use.Add(decryptpassword.LOGIN_CODE);

                }
            
                return entity.Any(y=>y.LOGIN_CODE.Equals(username, StringComparison.OrdinalIgnoreCase) && pass.Contains(password));
                //var abc = pass.Contains(password);
                //return v;
            }
        }
        public class UserModel
        {
            public string LOGIN_CODE { get; set; }
            public string PASSWORD { get; set; }
        }
    }
}