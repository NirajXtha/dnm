using NeoErp.Data;
using NeoErp.Pos.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.Pos.Services.Services
{
    public class ItemImageService : IItemImageService
    {
        //private ICacheManager _cacheManager;
        private IDbContext _dbContext;
        public ItemImageService(IDbContext dbContext)
        {

            this._dbContext = dbContext;
        }

        public string GetImagesByItemCode(ItemImageModel itemdetail)
        {
            string query = string.Format(@"SELECT IMAGE_FILE_NAME FROM IP_ITEM_MASTER_SETUP WHERE ITEM_CODE IN ({0})",
             itemdetail.ItemCode);
            var result = _dbContext.SqlQuery<string>(query).FirstOrDefault();
            return result;
        }



        public string insert(ItemImageModel itemdetail)
        {
            string query = string.Format(@"UPDATE IP_ITEM_MASTER_SETUP SET IMAGE_FILE_NAME  = '{0}' WHERE ITEM_CODE IN ({1})",
            itemdetail.Path, itemdetail.ItemCode);
            var rowCount = _dbContext.ExecuteSqlCommand(query);
            var path = itemdetail.Path;
            return path;
        }
    }
}