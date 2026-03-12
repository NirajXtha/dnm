using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.Planning.Models
{
    public enum EnumPlanType
    {
        SALES,
        PRODUCTION,
        PROCUREMENT,
        CONSUMPTION,
        DISPATCH,
        OTHER
    }

    public enum EnumPlanFor
    {
        QUANTITY,
        AMOUNT
    }
    public enum EnumPlanForProcurement
    {
        QUANTITY
    }
    public enum EnumPlanForCollection
    {
        AMOUNT
    }
    public enum EnumPlanForProduction
    {
        QUANTITY
    }
    public enum EnumSubgroup
    {
        CUSTOMER,
        DIVISION,
        EMPLOYEE
    }
}