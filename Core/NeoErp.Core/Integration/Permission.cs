using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Models;
using System.Web.Mvc;
using System.Collections;
using NeoErp.Core.Helpers;

namespace NeoErp.Core.Integration
{
    #region Checking Permissions

    public enum PermissionType
    {
        FullAccess =1,
        AllowAdd,
        AllowEdit,
        AllowDelete,
        AllowView,
        AllowPrint
    }

    public static class PermissionContext
    {      
        public static void CheckPermission(this ControllerContext controller,PermissionType permissionType)
        {
            //Permissions p = new Permissions();
          
            //bool IsAllowed = false;

            //if (permissionType == PermissionType.FullAccess)
            //    IsAllowed = mp.FullAccess;
            //else if (permissionType == PermissionType.AllowAdd)
            //    IsAllowed = mp.AllowAdd;
            //else if (permissionType == PermissionType.AllowEdit)
            //    IsAllowed = mp.AllowEdit;
            //else if (permissionType == PermissionType.AllowDelete)
            //    IsAllowed = mp.AllowDelete;
            //else if (permissionType == PermissionType.AllowView)
            //    IsAllowed = mp.AllowView;
            //else if (permissionType == PermissionType.AllowPrint)
            //    IsAllowed = mp.AllowPrint;
            //else
            //    IsAllowed = false;

            //Permissions.CheckPermission(IsAllowed);
        }
        
        public static string Area(this ControllerContext htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("area"))
                return (string)routeValues["area"];

            return string.Empty;
        }

        public static string Controller(this ControllerContext htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("controller"))
                return (string)routeValues["controller"];

            return string.Empty;
        }

        public static string Action(this ControllerContext htmlHelper)
        {
            var routeValues = HttpContext.Current.Request.RequestContext.RouteData.Values;

            if (routeValues.ContainsKey("action"))
                return (string)routeValues["action"];

            return string.Empty;
        }
        
    }

    public class Permissions
    {

      
        
        //public static HRM.clsSecurity.UserPermission _get(string url)
        //{
        //    MenuPermission up = new HRM.clsSecurity(_Settings._S).GetWebPermission(url, AppSecurity.loginDetail.menu, AppSecurity.loginDetail);
        //    return up;
        //}

        //public static bool AllowView
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.AllowView;
        //    }
        //}

        //public static bool AllowReport
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.AllowReport;
        //    }
        //}

        //public static bool AllowEdit
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.AllowEdit;
        //    }
        //}

        //public static bool AllowAdd
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.AllowAdd;
        //    }
        //}

        //public static bool AllowDelete
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.AllowDelete;
        //    }
        //}

        //public static bool AllowFullAccess
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.FullAccess;
        //    }
        //}

        //public static bool AllowPowerAccess
        //{
        //    get
        //    {
        //        HRM.clsSecurity.UserPermission up = _get(System.Web.HttpContext.Current.Request.RawUrl);
        //        return up.AllowPowerAccess;
        //    }
        //}

        //public static bool IsValidIP
        //{
        //    get
        //    {
        //        if (_Settings._S.Attendance.AttendanceIPValidation == AppSettings.Enm.enmIPValidationOption.None)
        //            return true;

        //        string ip = new NeoErpUtilities().GetIpAddress();
        //        if (AppSecurity.loginDetail.ValidIPs.Contains(ip) == true)
        //            return true;
        //        else
        //            return false;
        //    }
        //}

    }

    #endregion
}