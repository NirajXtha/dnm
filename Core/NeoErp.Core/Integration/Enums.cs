using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using NeoErp.Core.Helpers;

namespace NeoErp.Core.Integration
{
    public class Enums : IEnums
    {

        #region App Enums
        public enum UserGroupTypes
        {
            SuperAdmin = 1,
            Admin = 2,
            User = 3,
            Supervisor = 4,
            Employee = 5,
            Guest = 6
        }

        public enum PrimaryDateType
        {
            NepaliDate = 0,
            EnglishDate
        }

        public enum EditModes
        {
            Add = 1,
            Modify,
            Delete,
            View
        }

        public enum SelectedDay
        {
            Sunday = 1,
            Monday,
            Tuesday,
            Wednesday,
            Thursday,
            Friday,
            Saturday
        }

        public enum SqlTypes
        {
            TypeInt,
            TypeFloat,
            TypeString,
            TypeDateTime
        }

        public enum NumberRoundUpOption
        {
            _Default = 0,
            Upper,
            Lower
        }

        public enum RTOptions
        {
            Input = 3,
            Output = 4
        }

        public enum DateSystem
        {
            NepaliDate = 0,
            EnglishDate = 1
        }

        public enum RecordGroupType
        {
            Group = 0,
            Group1 = 1,
            Group2 = 2,
            Group3 = 3
        }

        #endregion

        #region HR Enms

        public enum WorkingStatus
        {
            Working = 1,
            Pension,
            Retired,
            Suspended,
            Resigned,
            Terminated,
            Transferred,
            Study
        }

        public enum ApprovalActions
        {
            None = 0,
            Pending = 1,
            Recommended = 2,
            Reject = 3,
            ReDraft = 4,
            Approved = 5,
            Plan = 6
        }

        public enum AppointmentType
        {
            RegularEmployee=0,
            Consultant =1
        }

        public enum EmploymentTypeGroup
        {
            None = 0,
            Permanent,
            Contract,
            Probation,
            Trainee,
            NonEmployee
        }



        #endregion

     

        /// Get all values
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// Get all the names
        public static IEnumerable<T> GetNames<T>()
        {
            return Enum.GetNames(typeof(T)).Cast<T>();
        }

        /// Get the name for the enum value
        public static string GetName<T>(T enumValue)
        {
            return Enum.GetName(typeof(T), enumValue);
        }

        /// Get the underlying value for the Enum string
        public static int GetValue<T>(string enumString)
        {
            return (int)Enum.Parse(typeof(T), enumString.Trim());
        }

        public static string GetEnumDescription<T>(string value)
        {
            Type type = typeof(T);
            var name = Enum.GetNames(type).Where(f => f.Equals(value, StringComparison.CurrentCultureIgnoreCase)).Select(d => d).FirstOrDefault();

            if (name == null)
            {
                return string.Empty;
            }
            var field = type.GetField(name);
            var customAttribute = field.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return customAttribute.Length > 0 ? ((DescriptionAttribute)customAttribute[0]).Description : name;
        }
    }
}