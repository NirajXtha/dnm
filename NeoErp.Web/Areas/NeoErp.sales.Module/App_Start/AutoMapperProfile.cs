using AutoMapper;
using NeoErp.Sales.Modules.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoErp.sales.Module.App_Start
{
    public class AutoMapperProfile:Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<GoodsReceiptNotesDetailModelMongo, GoodsReceiptNotesDetailModel>().ForMember(x=>x.Id,y=>y.MapFrom(abc=>abc.Id));
            CreateMap<GoodsReceiptNotesDetailModel, GoodsReceiptNotesDetailModelMongo>().ForMember(x => x.Id, y => y.MapFrom(abc => abc.Id)); ;
          //  CreateMap<List<GoodsReceiptNotesDetailModelMongo>,List<GoodsReceiptNotesDetailModel>>().ForMember(x => x.Id, y => y.MapFrom(abc => abc.Id)); ;
          //  CreateMap<List<GoodsReceiptNotesDetailModel>, List<GoodsReceiptNotesDetailModelMongo>>().ForMember(x => x.Id, y => y.MapFrom(abc => abc.Id)); ;
        }
       
    }
}