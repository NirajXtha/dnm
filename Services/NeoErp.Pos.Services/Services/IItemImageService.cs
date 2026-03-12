using NeoErp.Pos.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Pos.Services.Services
{
    public interface IItemImageService
    {
        string insert(ItemImageModel itemdetail);

        string GetImagesByItemCode(ItemImageModel itemdetail);
    }
}
