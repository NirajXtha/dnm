using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NeoERP.LOC.Models
{
    public class EnumList
    {
        public enum ShipmentType
        {
            SEA,
            AIR,
            ROAD,
            TRAIN
        }
        
        public enum LogisticPlan_ShipmentType
        {
            SEA,
            ROAD,
            TRAIN
        }
        public enum ShipmentLoadType
        {
            LCL,
            FCL
        }

        public enum DeliveryPlaceType
        {
            GODOWN,
            PORT,
            FACTORY,
            OTHER
        }

        public enum DocumentActionType
        {
            PREPARE,
            RECIEVED,
            SUBMIT
        }

        public enum CalenderDaysType
        {
            DAY,
            WEEK,
            MONTH
        };
    }
}