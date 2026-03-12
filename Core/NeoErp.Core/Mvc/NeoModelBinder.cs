using System.Web.Mvc;

namespace NeoErp.Core.Mvc
{
    public class NeoModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BaseNeoModel)
            {
                ((BaseNeoModel)model).BindModel(controllerContext, bindingContext);
            }
            return model;
        }
    }
}
