using NeoErp.Core.Helpers;
using NeoErp.Core.Models.CustomModels;
using NeoErp.Sales.Modules.Services.Services;
using System;
//using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NeoErp.Sales.Modules.Services.Models;
using NeoErp.Core;

namespace NeoErp.sales.Module.Controllers.Api
{
    public class JournalVoucherController : ApiController
    {
        private IJournalVoucherService _journalVoucherService;
        public IWorkContext _workContext;
        public JournalVoucherController(IJournalVoucherService journalVoucherService, IWorkContext workContext)
        {
            this._journalVoucherService = journalVoucherService;
            this._workContext = workContext;
        }

        [HttpPost]
        public GridViewJournalVoucher GetDayVoucher(filterOption model)
        {
            var gridViewJournalModel = new GridViewJournalVoucher();
            var userinfo = this._workContext.CurrentUserinformation;           
            var journalVoucher = this._journalVoucherService.GetJournalVoucher(model, userinfo).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<GridViewJournalVoucher>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<GridViewJournalVoucher>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                gridViewJournalModel.gridJournalVoucherReport = journalVoucher.Where(whereClause, parameters.ToArray()).ToList();
                gridViewJournalModel.total = gridViewJournalModel.gridJournalVoucherReport.Count;
            }
            else
            {
                gridViewJournalModel.gridJournalVoucherReport = journalVoucher.OrderBy(q => q.VOUCHER_NO).ToList();
                gridViewJournalModel.total = gridViewJournalModel.gridJournalVoucherReport.Count;
            }

            //gridViewJournalModel.gridJournalVoucherReport = gridViewJournalModel.gridJournalVoucherReport.Skip((model.page - 1) * model.pageSize)
            //         .Take(model.pageSize).ToList();

            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewJournalModel.gridJournalVoucherReport = gridViewJournalModel.gridJournalVoucherReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }

            return gridViewJournalModel;
        }

        [HttpPost]
        public GridViewJournalVoucher GetDaySubledgerVoucher(filterOption model)
        {
            var gridViewJournalModel = new GridViewJournalVoucher();
            var userinfo = this._workContext.CurrentUserinformation;
            var journalVoucher = this._journalVoucherService.GetJournalsUBLEDGERVoucher(model, userinfo).AsQueryable();
            if (model.filter != null && (model.filter.Filters != null && model.filter.Filters.Count > 0))
            {
                string whereClause = null;
                var parameters = new System.Collections.ArrayList();
                var filters = model.filter.Filters;
                for (var i = 0; i < filters.Count; i++)
                {
                    if (i == 0)
                        whereClause += string.Format("{0}", KendoGridHelper.BuildWhereClause<GridViewJournalVoucher>(i, model.filter.Logic, filters[i], parameters));
                    else
                        whereClause += string.Format(" {0} {1}", KendoGridHelper.ToLinqOperator(model.filter.Logic), KendoGridHelper.BuildWhereClause<GridViewJournalVoucher>(i, model.filter.Logic, filters[i], parameters));
                }
                // Where
                gridViewJournalModel.gridJournalVoucherReport = journalVoucher.Where(whereClause, parameters.ToArray()).ToList();
                gridViewJournalModel.total = gridViewJournalModel.gridJournalVoucherReport.Count;
            }
            else
            {
                gridViewJournalModel.gridJournalVoucherReport = journalVoucher.OrderBy(q => q.VOUCHER_NO).ToList();
                gridViewJournalModel.total = gridViewJournalModel.gridJournalVoucherReport.Count;
            }

            //gridViewJournalModel.gridJournalVoucherReport = gridViewJournalModel.gridJournalVoucherReport.Skip((model.page - 1) * model.pageSize)
            //         .Take(model.pageSize).ToList();

            if (model.sort != null && model.sort.Count > 0)
            {
                foreach (var s in model.sort)
                {
                    // s.Field =  s.Field;
                    gridViewJournalModel.gridJournalVoucherReport = gridViewJournalModel.gridJournalVoucherReport.OrderBy(s.Field + " " + s.Dir).ToList();
                }
            }

            return gridViewJournalModel;
        }
    }
}
