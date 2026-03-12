using System.Collections.Generic;
using System.Web.Mvc;

namespace NeoErp.Core.Mvc
{
    /// <summary>
    /// Base nopCommerce model
    /// </summary>
    [ModelBinder(typeof(NeoModelBinder))]
    public partial class BaseNeoModel
    {
        public BaseNeoModel()
        {            
            PostInitialize();
        }

        public virtual void BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
        }

        /// <summary>
        /// Developers can override this method in custom partial classes
        /// in order to add some custom initialization code to constructors
        /// </summary>
        protected virtual void PostInitialize()
        {
            
        }
                
    }

    /// <summary>
    /// Base Neo entity model
    /// </summary>
    public partial class BaseNopEntityModel : BaseNeoModel
    {
        public virtual int Id { get; set; }
    }
}
