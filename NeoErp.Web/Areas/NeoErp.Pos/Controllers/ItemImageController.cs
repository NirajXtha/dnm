
using NeoErp.Pos.Services;
using NeoErp.Pos.Services.Model;
using NeoErp.Pos.Services.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NeoErp.Pos.Controllers
{
    public class ItemImageController : Controller
    {
        private IItemImageService _itemimage;
        public ItemImageController(IItemImageService itemimage)
        {
            this._itemimage = itemimage;

        }

        // GET: ItemImg
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ItemImageUpload(ItemImageModel itemdetail)
        {
            if (itemdetail.ItemCode == null)
            {
                return Json("Warning-item");
            }
            else
            {
                if (itemdetail.file != null)
                {
                    string picture = System.IO.Path.GetFileName(itemdetail.file.FileName);
                    string underscore = "_";
                    string pic = string.Format("{0}{1}", itemdetail.ItemCode, underscore) + picture.PadLeft(0, '0');
                    string ItemImages = "ItemImages";
                    string strMappath = "~/Pictures/" + ItemImages + "/" + itemdetail.ItemCode + "/";
                    string path = System.IO.Path.Combine(
                                           Server.MapPath(strMappath), pic);
                    string ext = Path.GetExtension(path);
                    if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
                    {
                        return Json("Warning-jp");
                    }
                    else
                    {
                        string dbstrMappath = "\\Pictures\\" + ItemImages + "\\" + itemdetail.ItemCode + "\\";
                        if (!Directory.Exists(strMappath))
                        {
                            Directory.CreateDirectory(Server.MapPath(strMappath));
                        }


                        itemdetail.file.SaveAs(path);
                        itemdetail.Path = dbstrMappath + pic;
                        var imagepath = _itemimage.insert(itemdetail);
                        return Json(imagepath);
                    }
                }
                else
                {
                    return Json("Warning-up");
                }
            }
        }


        public JsonResult ItemImageDisplay(ItemImageModel itemdetail)
        {
            var path = _itemimage.GetImagesByItemCode(itemdetail);
            return Json(path, JsonRequestBehavior.AllowGet);
        }
    }
}