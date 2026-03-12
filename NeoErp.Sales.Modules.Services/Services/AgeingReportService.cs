using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeoErp.Sales.Modules.Services.Models.AgeingReport;
using NeoErp.Core.Models;
using NeoErp.Data;
using NeoErp.Core.Caching;
using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Sales.Modules.Services.Models;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class AgeingReportService : IAgeingReportService
    {
        private IDbContext _dbContext;
        private ICacheManager _cacheManager;
        private IAgeingFactory _ageingFactory;
        public IWorkContext _workingContext;

        public AgeingReportService(IDbContext dbContext,ICacheManager cacheManger, IAgeingFactory ageingFactory,IWorkContext workingContext)
        {
            this._dbContext = dbContext;
            this._cacheManager = cacheManger;
            this._ageingFactory = ageingFactory;
            this._workingContext = workingContext;
        }

        public IEnumerable<AgeingColumnRange> GenerateColumns(int frequency, int fixedDay, string asOnDate)
        {
            List<AgeingColumnRange> ageingColumns = new List<AgeingColumnRange>();

            DateTime ageingDateFrom = new DateTime();

            if (!DateTime.TryParseExact(asOnDate, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out ageingDateFrom))
                return ageingColumns;

            var exitDate = 0;
            while (exitDate + frequency <= fixedDay)
            {
                var ageingColumn = new AgeingColumnRange();
                ageingColumn.ColumnRange = string.Format("{0}-{1}", exitDate == 0?  exitDate:exitDate+1, exitDate + frequency);
                ageingColumn.orderBy = exitDate;
                //ageingColumn.ToDate = exitDate == 0 ? ageingDateFrom : ageingDateFrom;
                ageingColumn.ToDate = exitDate == 0 ? ageingDateFrom : ageingDateFrom.AddDays(-1);
                ageingColumn.ToDateString = ageingColumn.ToDate.ToString("yyyy-MM-dd");
                ageingColumn.FromDate = ageingDateFrom = exitDate == 0 ? ageingDateFrom.AddDays(-frequency) : ageingDateFrom.AddDays(-frequency-1);
                ageingColumn.FromDateString = ageingColumn.FromDate.Value.ToString("yyyy-MM-dd");
                exitDate += frequency;
                if ( !(exitDate == fixedDay) && exitDate + frequency > fixedDay)
                {
                    frequency = frequency - (exitDate + frequency - fixedDay);
                }

                ageingColumns.Add(ageingColumn);
            }
            ageingColumns.Add(new AgeingColumnRange()
            {
                ColumnRange = string.Format("{0}{1}", fixedDay, "+"),
                orderBy = fixedDay,
                ToDateString = ageingDateFrom.AddDays(-1).ToString("yyyy-MM-dd"),
                ToDate = ageingDateFrom.AddDays(-1),
                FromDateString = string.Empty,
                FromDate = (DateTime?)null
                
            });
            return ageingColumns;
        }
        public IEnumerable<AgeingColumnRange> GenerateColumns(int frequency, int fixedDay, string asOnDate, IFormatProvider dateFormatProvider)
        {
            List<AgeingColumnRange> ageingColumns = new List<AgeingColumnRange>();

            DateTime ageingDateFrom = new DateTime();

            if (!DateTime.TryParse(asOnDate,dateFormatProvider,System.Globalization.DateTimeStyles.None, out ageingDateFrom))
                return ageingColumns;

            var exitDate = 0;
            while (exitDate + frequency <= fixedDay)
            {
                
                var ageingColumn = new AgeingColumnRange();
                ageingColumn.ColumnRange = string.Format("{0}-{1}",exitDate ==0 ? exitDate:exitDate+1, exitDate + frequency);
                ageingColumn.ToDate = exitDate == 0? ageingDateFrom: ageingDateFrom.AddDays(-1);
                ageingColumn.ToDateString = ageingColumn.ToDate.ToString(dateFormatProvider);
                ageingColumn.FromDate = ageingDateFrom = ageingDateFrom.AddDays(-frequency);
                ageingColumn.FromDateString = ageingColumn.FromDate.Value.ToString(dateFormatProvider);

                exitDate += frequency;
                if (!(exitDate == fixedDay) && exitDate + frequency > fixedDay)
                {
                    frequency = frequency - (exitDate + frequency - fixedDay);
                }

                ageingColumns.Add(ageingColumn);
            }
            ageingColumns.Add(new AgeingColumnRange()
            {
                ColumnRange = string.Format("{0}{1}", fixedDay, "+"),
                ToDateString = ageingDateFrom.AddDays(-1).ToString(dateFormatProvider),
                ToDate = ageingDateFrom.AddDays(-1),
                FromDateString = string.Empty,
                FromDate = (DateTime?)null
            });

            return ageingColumns;
        }

        public IEnumerable<AgeingDataViewModel> GetAgeingReport(AgeingFilterModel afModel)
        {
           
            var ageingData = new List<AgeingDataViewModel>();

            if (this._cacheManager.IsSet("ageing-report"))
            {
                ageingData = this._cacheManager.Get<List<AgeingDataViewModel>>("ageing-report");
            }
            else
            {
                ageingData = this.GetAgeingReportData(afModel).ToList();
                this._cacheManager.Set("ageing-report", ageingData, 20);
            }
           var data= afModel.ShowGroupWise == "True" ?  ageingData.ToList() : ageingData;
            return data;
         //   return ageingData.Where(q => q.parentId == afModel.Id).ToList();
        }

        protected IEnumerable<AgeingDataViewModel> GetAgeingReportData(AgeingFilterModel afModel)
        {
            User userinfo = this._workingContext.CurrentUserinformation;
            var ageingData = new List<AgeingDataViewModel>();

            // if (afModel.Codes.Count <= 0)
            if (afModel.Type == "Product")
                afModel.Codes = afModel.ReportFilters.ProductFilter;
            else
                afModel.Codes = afModel.ReportFilters.CustomerFilter;

            IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService((AgeingReportType)Enum.Parse(typeof(AgeingReportType), afModel.Type));

            var rawAgeingDebitData = dataService.GetAgeingDebitAmount(afModel.AsOnDate,afModel.Codes,afModel.ReportFilters.BranchFilter,afModel.BillWiseOrLedgerWise);
            var rawAgeingCreditData = dataService.GetAgeingCreditAmount(afModel.AsOnDate,afModel.Codes, afModel.ReportFilters.BranchFilter,afModel.BillWiseOrLedgerWise);
            var agingGroupData = new List<AgeingGroupData>();
            var groupData = afModel.ShowGroupWise=="True"? dataService.GetGroupData(afModel.ReportFilters.CompanyFilter): agingGroupData;

            var columnRangeData = this.GenerateColumns(afModel.FrequencyInDay.Value, afModel.FixedInDay.Value, afModel.AsOnDate);
            var distinctSubCode = rawAgeingCreditData.Select(q => q.SubCode).Union(rawAgeingDebitData.Select(q => q.SubCode)).Distinct();

            //var debitAmount = rawAgeingCreditData.GroupBy(q => new { q.SubCode, q.MasterCode, q.PreCode, q.Code, q.Description }, (key, group) => new AgeingDataViewModel() {
            //    SubCode = key.SubCode,
            //    Description = key.Description,
            //    id = key.Code,
            //    parentId = groupData.Where(q => q.MasterCode == key.PreCode).SingleOrDefault().Code,
            //    RangeColumnData = this.GetColumnRangeData(columnRangeData,rawAgeingDebitData,key.SubCode),
            //}).ToList();
            //debitAmount.ForEach(q => q.Total = q.RangeColumnData.Select(x => x.NetAmount).Sum());
            //ageingData.AddRange(debitAmount);
            if (afModel.Type == "Supplier")
            {
                foreach (var subCode in distinctSubCode)
                {

                    var itemData = rawAgeingCreditData.Where(q => q.SubCode == subCode).FirstOrDefault() != null ? rawAgeingCreditData.Where(q => q.SubCode == subCode).FirstOrDefault() : rawAgeingDebitData.Where(q => q.SubCode == subCode).FirstOrDefault();
                    var ageingItemByColumn = new AgeingDataViewModel()
                    {
                        SubCode = subCode,
                        Description = itemData.Description,
                        id = itemData.Code,
                        parentId = afModel.ShowGroupWise == "True"?groupData.Where(q => q.MasterCode == itemData.PreCode).SingleOrDefault().Code:0,
                        AccCode = itemData.AccCode,
                    };

                    foreach (var columnItem in columnRangeData)
                    {
                        // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                        if (columnItem.FromDate == null)
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = (DateTime?)null,
                                ToDate = columnItem.ToDate
                            });
                        }
                        else
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                //NetAmount = rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode).Sum(y => y.Amount),
                                NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                // NetAmount=rawAgeingDebitData.Where(x=>x.VoucherDate.E)
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = columnItem.FromDate.Value,
                                ToDate = columnItem.ToDate
                            });
                        }
                    }
                    ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    ageingData.Add(ageingItemByColumn);
                }

                var crAmount = rawAgeingDebitData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in crAmount)
                {
                    var crReminder = crItem.NetCrAmount;
                    var debitAmounts = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault().RangeColumnData.OrderBy(q => q.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {
                            //if(debitAmounts.Any(x=>x.NetAmount==0))
                            //     {
                            //     debitItem.NetAmount = netAmoutWithCrDeduction;
                            //     break;
                            // }
                            //else
                            // {
                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                    var ageingDataForSubCode = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault();
                    ageingDataForSubCode.RangeColumnData = debitAmounts;
                    ageingDataForSubCode.Total = debitAmounts.Select(q => q.NetAmount.Value).Sum();
                }
            }
            else
            {
                foreach (var subCode in distinctSubCode)
                {

                    var itemData = rawAgeingDebitData.Where(q => q.SubCode == subCode).FirstOrDefault() != null ? rawAgeingDebitData.Where(q => q.SubCode == subCode).FirstOrDefault() : rawAgeingCreditData.Where(q => q.SubCode == subCode).FirstOrDefault();
                    var ageingItemByColumn = new AgeingDataViewModel()
                    {
                        SubCode = subCode,
                        Description = itemData.Description,
                        id = itemData.Code,
                        parentId = afModel.ShowGroupWise == "True"?groupData.Where(q => q.MasterCode == itemData.PreCode).SingleOrDefault().Code: (int?)null,
                        AccCode = itemData.AccCode,
                    };

                    foreach (var columnItem in columnRangeData)
                    {
                        // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                        if (columnItem.FromDate == null)
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = (DateTime?)null,
                                ToDate = columnItem.ToDate
                            });
                        }
                        else
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                //NetAmount = rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode).Sum(y => y.Amount),
                                NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                // NetAmount=rawAgeingDebitData.Where(x=>x.VoucherDate.E)
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = columnItem.FromDate.Value,
                                ToDate = columnItem.ToDate
                            });
                        }
                    }
                    ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    ageingData.Add(ageingItemByColumn);
                }

                var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in crAmount)
                {
                    var crReminder = crItem.NetCrAmount;
                    var debitAmounts = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault().RangeColumnData.OrderBy(q => q.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {
                            //if(debitAmounts.Any(x=>x.NetAmount==0))
                            //     {
                            //     debitItem.NetAmount = netAmoutWithCrDeduction;
                            //     break;
                            // }
                            //else
                            // {
                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                    var ageingDataForSubCode = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault();
                    ageingDataForSubCode.RangeColumnData = debitAmounts;
                    ageingDataForSubCode.Total = debitAmounts.Select(q => q.NetAmount.Value).Sum();
                }
            }
            


            foreach (var groupItem in groupData)
            {
                var groupSum = ageingData.Where(q => q.parentId == groupItem.Code).Select(q => q.RangeColumnData);
                if(groupSum.Count() > 0)
                {
                    var groupSubMerger = new List<AgeingDataViewModel.AgeingColumnRangeData>();
                    foreach (var item in groupSum)
                    {
                        groupSubMerger.AddRange(item);
                    }

                    var parentIdForGroup = groupData.Where(q => q.MasterCode == groupItem.PreCode).SingleOrDefault();
                    int? parentGroupId = null;
                    if (parentIdForGroup != null)
                    {
                        parentGroupId = parentIdForGroup.Code;
                    }
                    var groupAgeingData = new AgeingDataViewModel() { Description = groupItem.Description, parentId = parentGroupId, id = groupItem.Code, hasChildren = true };
                    foreach (var columnItem in columnRangeData)
                    {
                        var newGroupAgeingData = new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            ColumnRangeName = columnItem.ColumnRange,
                            FromDate = columnItem.FromDate,
                            ToDate = columnItem.ToDate,
                            NetAmount = groupSubMerger.Where(q => q.ColumnRangeName == columnItem.ColumnRange).Select(q => q.NetAmount).Sum(),
                        };
                        groupAgeingData.RangeColumnData.Add(newGroupAgeingData);
                    }

                    groupAgeingData.Total = groupAgeingData.RangeColumnData.Select(q => q.NetAmount).Sum();
                    ageingData.Add(groupAgeingData);
                }
                
            }
            return ageingData.Where( q => q.Total != decimal.Zero).ToList();
        }

        protected IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartData(AgeingFilterModel afModel)
        {
            User userinfo = this._workingContext.CurrentUserinformation;
            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();
            afModel.Codes = afModel.ReportFilters.AreaTypeFilter;
            IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService((AgeingReportType)Enum.Parse(typeof(AgeingReportType), afModel.Type));
            
            var rawAgeingDebitData =afModel.BillWiseOrLedgerWise.ToLower()== "LedgerWise".ToLower()? dataService.GetAgeingChartDebitAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter): dataService.GetAgeingBillWiseChartDebitAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter);
            var rawAgeingCreditData = afModel.BillWiseOrLedgerWise.ToLower() == "LedgerWise".ToLower() ? dataService.GetAgeingChartCreditAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter): dataService.GetAgeingBillWiseChartCreditAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter);
            //  var groupData = dataService.GetGroupData(afModel.ReportFilters.CompanyFilter);
            if (string.IsNullOrEmpty(afModel.AsOnDate))
                afModel.AsOnDate = DateTime.Now.ToString("yyyy-MM-dd");
            var columnRangeData = this.GenerateColumns(afModel.FrequencyInDay.Value, afModel.FixedInDay.Value, afModel.AsOnDate);
            #region customer wise
            if(afModel.Type.Trim().ToLower().Equals("customer"))
            {
                foreach (var columnItem in columnRangeData)
                {

                    // var ageingItemByColumn = new AgeingDataViewModel.AgeingColumnRangeData();
                    // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                    if (columnItem.FromDate == null)
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = (DateTime?)null,
                            ToDate = columnItem.ToDate
                        });
                    }
                    else
                    {
                        var test  = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0);

                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = columnItem.FromDate.Value,
                            ToDate = columnItem.ToDate
                        });
                    }
                    //     ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    //  ageingData.Add(ageingItemByColumn);
                }
                var crReminder = rawAgeingCreditData.Sum(X => X.Amount);
                // foreach (var crItem in rawAgeingCreditData)
                // {
               // var crReminder = crAmount ?? 0;
                    var debitAmounts = ageingData.OrderBy(x => x.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        

                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {
                        debitItem.NetAmount = 0;
                        crReminder = Math.Abs(netAmoutWithCrDeduction ?? 0);
                        continue;
                            //if (columnCount != debitAmounts.Count())
                            //{
                            //    var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                            //    if (netAmoutWithCrDeductionReapted < 0)
                            //    {
                            //        crReminder = crReminder - debitItem.NetAmount;
                            //        debitItem.NetAmount = 0;

                            //    }

                            //}
                            //else
                            //    debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            //columnCount += 1;

                            // }

                        }


                    }

               // }
                return ageingData.ToList();

            }
            #endregion
            #region supplier wise
            else
            {

                foreach (var columnItem in columnRangeData)
                {

                    // var ageingItemByColumn = new AgeingDataViewModel.AgeingColumnRangeData();
                    // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                    if (columnItem.FromDate == null)
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = (DateTime?)null,
                            ToDate = columnItem.ToDate
                        });
                    }
                    else
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = columnItem.FromDate.Value,
                            ToDate = columnItem.ToDate
                        });
                    }
                    //     ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    //  ageingData.Add(ageingItemByColumn);
                }
                //var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in rawAgeingDebitData)
                {
                    var crReminder = crItem.Amount;
                    var debitAmounts = ageingData.OrderBy(x => x.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {

                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                }
                return ageingData.ToList();
            }
            #endregion

        }

        protected IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingDivisionChartData(AgeingFilterModel afModel)
        {
            User userinfo = this._workingContext.CurrentUserinformation;
            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();

            IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService((AgeingReportType)Enum.Parse(typeof(AgeingReportType), afModel.Type));

            var rawAgeingDebitData = afModel.BillWiseOrLedgerWise.ToLower() == "LedgerWise".ToLower() ? dataService.GetAgeingChartDebitAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter) : dataService.GetAgeingBillWiseChartDebitAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter);
            var rawAgeingCreditData = afModel.BillWiseOrLedgerWise.ToLower() == "LedgerWise".ToLower() ? dataService.GetAgeingChartCreditAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter) : dataService.GetAgeingBillWiseChartCreditAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter);
            //  var groupData = dataService.GetGroupData(afModel.ReportFilters.CompanyFilter);
            if (string.IsNullOrEmpty(afModel.AsOnDate))
                afModel.AsOnDate = DateTime.Now.ToString("yyyy-MM-dd");
            var columnRangeData = this.GenerateColumns(afModel.FrequencyInDay.Value, afModel.FixedInDay.Value, afModel.AsOnDate);
            #region customer wise
            if (afModel.Type.Trim().ToLower().Equals("customer"))
            {
                foreach (var columnItem in columnRangeData)
                {

                    // var ageingItemByColumn = new AgeingDataViewModel.AgeingColumnRangeData();
                    // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                    if (columnItem.FromDate == null)
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = (DateTime?)null,
                            ToDate = columnItem.ToDate
                        });
                    }
                    else
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = columnItem.FromDate.Value,
                            ToDate = columnItem.ToDate
                        });
                    }
                    //     ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    //  ageingData.Add(ageingItemByColumn);
                }
                //var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in rawAgeingCreditData)
                {
                    var crReminder = crItem.Amount;
                    var debitAmounts = ageingData.OrderBy(x => x.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {

                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                }
                return ageingData.ToList();

            }
            #endregion
            #region supplier wise
            else
            {

                foreach (var columnItem in columnRangeData)
                {

                    // var ageingItemByColumn = new AgeingDataViewModel.AgeingColumnRangeData();
                    // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                    if (columnItem.FromDate == null)
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = (DateTime?)null,
                            ToDate = columnItem.ToDate
                        });
                    }
                    else
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            OrderBy = columnItem.orderBy,
                            FromDate = columnItem.FromDate.Value,
                            ToDate = columnItem.ToDate
                        });
                    }
                    //     ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    //  ageingData.Add(ageingItemByColumn);
                }
                //var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in rawAgeingDebitData)
                {
                    var crReminder = crItem.Amount;
                    var debitAmounts = ageingData.OrderBy(x => x.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {

                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                }
                return ageingData.ToList();
            }
            #endregion

        }

        protected List<AgeingDataViewModel.AgeingColumnRangeData> GetColumnRangeData(IEnumerable<AgeingColumnRange> columnRangeData, IEnumerable<AgeingDataModel> ageingDataModel, string subcode)
        {
            if((columnRangeData ==null || columnRangeData.Count() == 0) && (ageingDataModel == null || ageingDataModel.Count() == 0) && !string.IsNullOrEmpty(subcode))
            return new List<AgeingDataViewModel.AgeingColumnRangeData>();

            var columnRangeDate = new List<AgeingDataViewModel.AgeingColumnRangeData>();
            foreach (var columnItem in columnRangeData)
            {
                if (columnItem.FromDate == null)
                {
                    columnRangeDate.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                    {
                        NetAmount = ageingDataModel.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subcode).Select(q => q.Amount).Sum(),
                        ColumnRangeName = columnItem.ColumnRange,
                        FromDate = (DateTime?)null,
                        ToDate = columnItem.ToDate
                    });
                }
                else
                {
                    columnRangeDate.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                    {
                        NetAmount = ageingDataModel.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subcode).Select(q => q.Amount).Sum(),
                        ColumnRangeName = columnItem.ColumnRange,
                        FromDate = columnItem.FromDate.Value,
                        ToDate = columnItem.ToDate
                    });
                }
            }
            return new List<AgeingDataViewModel.AgeingColumnRangeData>();
        }

        public IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartReport(AgeingFilterModel afModel)
        {

            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();

            if (this._cacheManager.IsSet("ageingChart-report"))
            {
                ageingData = this._cacheManager.Get<List<AgeingDataViewModel.AgeingColumnRangeData>>("ageingChart-report");
            }
            else
            {
                ageingData = this.GetAgeingChartData(afModel).ToList();
                this._cacheManager.Set("ageingChart-report", ageingData, 20);
            }

            return ageingData.ToList();
        }
        public IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingChartReportBranchWise(AgeingFilterModel afModel)
        {
            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();

            if (this._cacheManager.IsSet("ageingChart-reportBranchwise"))
            {
                ageingData = this._cacheManager.Get<List<AgeingDataViewModel.AgeingColumnRangeData>>("ageingChart-reportBranchwise");
            }
            else
            {
                var branches = _dbContext.SqlQuery<BranchModel>($"SELECT BRANCH_CODE BranchCode, BRANCH_EDESC BranchName FROM FA_BRANCH_SETUP WHERE GROUP_SKU_FLAG = 'I' AND COMPANY_CODE = '{_workingContext.CurrentUserinformation.company_code}'").ToList();
                foreach(var branch in branches)
                {
                    afModel.ReportFilters.BranchFilter = new List<string> { branch.BranchCode };
                    afModel.ShowGroupWise = "true";
                    var CustomerWiseAgingdata = this.GetAgeingReportData(afModel).ToList();
                    var result =  new  List<AgeingDataViewModel.AgeingColumnRangeData>();
                    var RangeData0 = new AgeingDataViewModel.AgeingColumnRangeData();
                    var RangeData30 = new AgeingDataViewModel.AgeingColumnRangeData();
                    var RangeData60 = new AgeingDataViewModel.AgeingColumnRangeData();
                    var RangeData90 = new AgeingDataViewModel.AgeingColumnRangeData();
                    var RangeData120 = new AgeingDataViewModel.AgeingColumnRangeData();
                    RangeData0.NetAmount = 0;
                    RangeData0.ColumnRangeName = "0-30";
                    
                    RangeData30.NetAmount = 0;
                    RangeData30.ColumnRangeName = "31-60";
                    RangeData60.NetAmount = 0;
                    RangeData60.ColumnRangeName = "61-90";
                    RangeData90.NetAmount = 0;
                    RangeData90.ColumnRangeName = "91-120";
                    RangeData120.NetAmount = 0;
                    RangeData120.ColumnRangeName = "120+";
                    foreach (var customeraging in CustomerWiseAgingdata.Select(x => x.RangeColumnData))
                    {
                      
                        RangeData0.NetAmount = RangeData0.NetAmount+customeraging.Where(x => x.ColumnRangeName == "0-30").Sum(x => x.NetAmount);
                        RangeData0.Descriptions = "0-30";
                        RangeData30.NetAmount = RangeData30.NetAmount+customeraging.Where(x => x.ColumnRangeName == "31-60").Sum(x => x.NetAmount);
                        RangeData30.Descriptions = "31-60";
                        RangeData60.NetAmount = RangeData60.NetAmount+customeraging.Where(x => x.ColumnRangeName == "61-90").Sum(x => x.NetAmount);
                        RangeData60.Descriptions = "61-90";
                        RangeData90.NetAmount = RangeData90.NetAmount+customeraging.Where(x => x.ColumnRangeName == "91-120").Sum(x => x.NetAmount);
                        RangeData90.Descriptions = "91-120";
                        RangeData120.NetAmount = RangeData120.NetAmount+customeraging.Where(x => x.ColumnRangeName == "120+").Sum(x => x.NetAmount);
                        RangeData120.Descriptions = "120+";
                    }

                    result.Add(RangeData0);
                    result.Add(RangeData30);
                    result.Add(RangeData90);
                    result.Add(RangeData120);
                    result.Add(RangeData60);
                    //  var result = this.GetAgeingChartData(afModel).ToList();
                    var total = result.Sum(x => x.NetAmount);
                    //if (total <= 0)
                       // continue;
                    for(int i = 0; i < result.Count; i++)
                    {
                        if(result[i].ColumnRangeName=="0-30")
                        {
                            result[i].ColumnRangeName = "A: 0-30";
                            result[i].colorCode = "#28B463";
                            result[i].OrderBy = 0;
                        }
                        else if (result[i].ColumnRangeName == "31-60")
                        {
                            result[i].ColumnRangeName = "B: 31-60";
                            result[i].colorCode = "#82E0AA";
                            result[i].OrderBy = 2;
                        }
                        else if (result[i].ColumnRangeName == "61-90")
                        {
                            result[i].ColumnRangeName = "C: 61-90";
                            result[i].colorCode = "#85929E";
                            result[i].OrderBy = 3;
                        }
                        else if (result[i].ColumnRangeName == "91-120")
                        {
                            result[i].ColumnRangeName = "D: 91-120";
                            result[i].colorCode = "#E74C3C";
                            result[i].OrderBy = 4;
                        }
                        else
                        {
                            result[i].ColumnRangeName = "E: 120+";
                            result[i].colorCode = "#D7BDE2";
                            result[i].OrderBy = 5;
                        }
                        result[i].NetAmount = result[i].NetAmount;
                        //result[i].NetAmount = (result[i].NetAmount / total) * 100;  //calculating percentage
                        result[i].Branch = branch.BranchName;
                    }
                    ageingData.AddRange(result);
                }
                ageingData.OrderBy(x => x.OrderBy);
                this._cacheManager.Set("ageingChart-reportBranchwise", ageingData, 20);
            }

            return ageingData.ToList();
        }
        public IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetAgeingDivisionChartReport(AgeingFilterModel afModel)
        {

            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();

            //if (this._cacheManager.IsSet("ageingChart-report"))
            //{
            //    ageingData = this._cacheManager.Get<List<AgeingDataViewModel.AgeingColumnRangeData>>("ageingChart-report");
            //}
            //else
            //{
            ageingData = this.GetAgeingDivisionChartData(afModel).ToList();
            //  this._cacheManager.Set("ageingChart-report", ageingData, 20);
            //  }

            return ageingData.ToList();
        }

        
        public IEnumerable<AgeingDataViewModel> GetMobileAgeingReport(AgeingFilterModel afModel, string customerCode, string companyCode)
        {

            var ageingData = new List<AgeingDataViewModel>();

            IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService((AgeingReportType)Enum.Parse(typeof(AgeingReportType), afModel.Type));
            var rawAgeingDebitData = dataService.GetMobileAgeingDebitAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter, customerCode, companyCode);
            var rawAgeingCreditData = dataService.GetMobileAgeingCreditAmount(afModel.AsOnDate, afModel.Codes, afModel.ReportFilters.BranchFilter, customerCode, companyCode);
            var groupData = dataService.GetMobileGroupData(afModel.ReportFilters.CompanyFilter, customerCode, companyCode);

            var columnRangeData = this.GenerateColumns(afModel.FrequencyInDay.Value, afModel.FixedInDay.Value, afModel.AsOnDate);
            var distinctSubCode = rawAgeingCreditData.Select(q => q.SubCode).Union(rawAgeingDebitData.Select(q => q.SubCode)).Distinct();

            //var debitAmount = rawAgeingCreditData.GroupBy(q => new { q.SubCode, q.MasterCode, q.PreCode, q.Code, q.Description }, (key, group) => new AgeingDataViewModel() {
            //    SubCode = key.SubCode,
            //    Description = key.Description,
            //    id = key.Code,
            //    parentId = groupData.Where(q => q.MasterCode == key.PreCode).SingleOrDefault().Code,
            //    RangeColumnData = this.GetColumnRangeData(columnRangeData,rawAgeingDebitData,key.SubCode),
            //}).ToList();
            //debitAmount.ForEach(q => q.Total = q.RangeColumnData.Select(x => x.NetAmount).Sum());
            //ageingData.AddRange(debitAmount);
            if (afModel.Type == "Supplier")
            {
                foreach (var subCode in distinctSubCode)
                {

                    var itemData = rawAgeingCreditData.Where(q => q.SubCode == subCode).FirstOrDefault() != null ? rawAgeingCreditData.Where(q => q.SubCode == subCode).FirstOrDefault() : rawAgeingDebitData.Where(q => q.SubCode == subCode).FirstOrDefault();
                    var ageingItemByColumn = new AgeingDataViewModel()
                    {
                        SubCode = subCode,
                        Description = itemData.Description,
                        id = itemData.Code,
                        parentId = groupData.Where(q => q.MasterCode == itemData.PreCode).SingleOrDefault().Code,
                        AccCode = itemData.AccCode,
                    };

                    foreach (var columnItem in columnRangeData)
                    {
                        // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                        if (columnItem.FromDate == null)
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = (DateTime?)null,
                                ToDate = columnItem.ToDate
                            });
                        }
                        else
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                //NetAmount = rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode).Sum(y => y.Amount),
                                NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                // NetAmount=rawAgeingDebitData.Where(x=>x.VoucherDate.E)
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = columnItem.FromDate.Value,
                                ToDate = columnItem.ToDate
                            });
                        }
                    }
                    ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    ageingData.Add(ageingItemByColumn);
                }

                var crAmount = rawAgeingDebitData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in crAmount)
                {
                    var crReminder = crItem.NetCrAmount;
                    var debitAmounts = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault().RangeColumnData.OrderBy(q => q.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {
                            //if(debitAmounts.Any(x=>x.NetAmount==0))
                            //     {
                            //     debitItem.NetAmount = netAmoutWithCrDeduction;
                            //     break;
                            // }
                            //else
                            // {
                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                    var ageingDataForSubCode = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault();
                    ageingDataForSubCode.RangeColumnData = debitAmounts;
                    ageingDataForSubCode.Total = debitAmounts.Select(q => q.NetAmount.Value).Sum();
                }
            }
            else
            {
                foreach (var subCode in distinctSubCode)
                {

                    var itemData = rawAgeingDebitData.Where(q => q.SubCode == subCode).FirstOrDefault() != null ? rawAgeingDebitData.Where(q => q.SubCode == subCode).FirstOrDefault() : rawAgeingCreditData.Where(q => q.SubCode == subCode).FirstOrDefault();
                    var ageingItemByColumn = new AgeingDataViewModel()
                    {
                        SubCode = subCode,
                        Description = itemData.Description,
                        id = itemData.Code,
                        parentId = groupData.Where(q => q.MasterCode == itemData.PreCode).SingleOrDefault().Code,
                        AccCode = itemData.AccCode,
                    };

                    foreach (var columnItem in columnRangeData)
                    {
                        // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                        if (columnItem.FromDate == null)
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = (DateTime?)null,
                                ToDate = columnItem.ToDate
                            });
                        }
                        else
                        {
                            ageingItemByColumn.RangeColumnData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                //NetAmount = rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode).Sum(y => y.Amount),
                                NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0 && q.SubCode == subCode).Select(q => q.Amount).Sum(),
                                // NetAmount=rawAgeingDebitData.Where(x=>x.VoucherDate.E)
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = columnItem.FromDate.Value,
                                ToDate = columnItem.ToDate
                            });
                        }
                    }
                    ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    ageingData.Add(ageingItemByColumn);
                }

                var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in crAmount)
                {
                    var crReminder = crItem.NetCrAmount;
                    var debitAmounts = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault().RangeColumnData.OrderBy(q => q.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {
                            //if(debitAmounts.Any(x=>x.NetAmount==0))
                            //     {
                            //     debitItem.NetAmount = netAmoutWithCrDeduction;
                            //     break;
                            // }
                            //else
                            // {
                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                    var ageingDataForSubCode = ageingData.Where(q => q.SubCode == crItem.SubCode).SingleOrDefault();
                    ageingDataForSubCode.RangeColumnData = debitAmounts;
                    ageingDataForSubCode.Total = debitAmounts.Select(q => q.NetAmount.Value).Sum();
                }
            }



            foreach (var groupItem in groupData)
            {
                var groupSum = ageingData.Where(q => q.parentId == groupItem.Code).Select(q => q.RangeColumnData);
                if (groupSum.Count() > 0)
                {
                    var groupSubMerger = new List<AgeingDataViewModel.AgeingColumnRangeData>();
                    foreach (var item in groupSum)
                    {
                        groupSubMerger.AddRange(item);
                    }

                    var parentIdForGroup = groupData.Where(q => q.MasterCode == groupItem.PreCode).SingleOrDefault();
                    int? parentGroupId = null;
                    if (parentIdForGroup != null)
                    {
                        parentGroupId = parentIdForGroup.Code;
                    }
                    var groupAgeingData = new AgeingDataViewModel() { Description = groupItem.Description, parentId = parentGroupId, id = groupItem.Code, hasChildren = true };
                    foreach (var columnItem in columnRangeData)
                    {
                        var newGroupAgeingData = new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            ColumnRangeName = columnItem.ColumnRange,
                            FromDate = columnItem.FromDate,
                            ToDate = columnItem.ToDate,
                            NetAmount = groupSubMerger.Where(q => q.ColumnRangeName == columnItem.ColumnRange).Select(q => q.NetAmount).Sum(),
                        };
                        groupAgeingData.RangeColumnData.Add(newGroupAgeingData);
                    }

                    groupAgeingData.Total = groupAgeingData.RangeColumnData.Select(q => q.NetAmount).Sum();
                    ageingData.Add(groupAgeingData);
                }

            }
            return ageingData.Where(q => q.Total != decimal.Zero).ToList();
        }

        public IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetMobileAgeingChartReport(AgeingFilterModel afModel, List<string> customerCode, string companyCode)
        {

            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();
            ageingData = this.GetMobileAgeingChartData(afModel, customerCode, companyCode).ToList();

            return ageingData.ToList();
        }


        protected IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> GetMobileAgeingChartData(AgeingFilterModel afModel, List<string> customerCode, string companyCode)
        {
            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();

            IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService((AgeingReportType)Enum.Parse(typeof(AgeingReportType), afModel.Type));

            IEnumerable<AgeingDataModel> rawAgeingDebitData;
            IEnumerable<AgeingDataModel> rawAgeingCreditData;
            if (afModel.CustomerDealerWise== "Customer")
            {
                 rawAgeingDebitData = dataService.GetMobileAgeingChartDebitAmount(afModel.AsOnDate, customerCode, afModel.ReportFilters.BranchFilter);
                 rawAgeingCreditData = dataService.GetMobileAgeingChartCreditAmount(afModel.AsOnDate, customerCode, afModel.ReportFilters.BranchFilter);
            }
            else
            {
                 rawAgeingDebitData = dataService.GetMobileAgeingDealerDebitAmount(afModel.AsOnDate, customerCode, afModel.ReportFilters.BranchFilter,"0",afModel.DealerGroup);
                 rawAgeingCreditData = dataService.GetMobileAgeingDealerCreditAmount(afModel.AsOnDate, customerCode, afModel.ReportFilters.BranchFilter, "0", afModel.DealerGroup);
            }
           
            //  var groupData = dataService.GetGroupData(afModel.ReportFilters.CompanyFilter);
            if (string.IsNullOrEmpty(afModel.AsOnDate))
                afModel.AsOnDate = DateTime.Now.ToString("yyyy-MM-dd");
            var columnRangeData = this.GenerateColumns(afModel.FrequencyInDay.Value, afModel.FixedInDay.Value, afModel.AsOnDate);
            #region customer wise
            if (afModel.Type.Trim().ToLower().Equals("customer"))
            {
                foreach (var columnItem in columnRangeData)
                {

                    // var ageingItemByColumn = new AgeingDataViewModel.AgeingColumnRangeData();
                    // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                    if (columnItem.FromDate == null)
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            FromDate = (DateTime?)null,
                            ToDate = columnItem.ToDate
                        });
                    }
                    else
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingDebitData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            FromDate = columnItem.FromDate.Value,
                            ToDate = columnItem.ToDate
                        });
                    }
                    //     ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    //  ageingData.Add(ageingItemByColumn);
                }
                //var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in rawAgeingCreditData)
                {
                    var crReminder = crItem.Amount;
                    var debitAmounts = ageingData.OrderBy(x => x.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {

                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                }
                return ageingData.ToList();

            }
            #endregion
            #region supplier wise
            else
            {

                foreach (var columnItem in columnRangeData)
                {

                    // var ageingItemByColumn = new AgeingDataViewModel.AgeingColumnRangeData();
                    // var test= rawAgeingDebitData.Where(x => x.VoucherDate >= columnItem.FromDate && x.VoucherDate <= columnItem.ToDate && x.SubCode == subCode)
                    if (columnItem.FromDate == null)
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            FromDate = (DateTime?)null,
                            ToDate = columnItem.ToDate
                        });
                    }
                    else
                    {
                        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                        {
                            NetAmount = rawAgeingCreditData.Where(q => DateTime.Compare(q.VoucherDate, columnItem.FromDate.Value) >= 0 && DateTime.Compare(q.VoucherDate, columnItem.ToDate) <= 0).Select(q => q.Amount).Sum(),
                            ColumnRangeName = columnItem.ColumnRange,
                            FromDate = columnItem.FromDate.Value,
                            ToDate = columnItem.ToDate
                        });
                    }
                    //     ageingItemByColumn.Total = ageingItemByColumn.RangeColumnData.Select(q => q.NetAmount).Sum();
                    //  ageingData.Add(ageingItemByColumn);
                }
                //var crAmount = rawAgeingCreditData.GroupBy(q => q.SubCode).Select(x => new { NetCrAmount = x.Sum(p => p.Amount), SubCode = x.Key });
                foreach (var crItem in rawAgeingDebitData)
                {
                    var crReminder = crItem.Amount;
                    var debitAmounts = ageingData.OrderBy(x => x.FromDate).ToList();

                    var columnCount = 1;
                    foreach (var debitItem in debitAmounts)
                    {
                        var netAmoutWithCrDeduction = debitItem.NetAmount - crReminder;
                        if (netAmoutWithCrDeduction >= 0)
                        {
                            debitItem.NetAmount = netAmoutWithCrDeduction;
                            break;
                        }
                        //else if()
                        else
                        {

                            if (columnCount != debitAmounts.Count())
                            {
                                var netAmoutWithCrDeductionReapted = debitItem.NetAmount - crReminder;
                                if (netAmoutWithCrDeductionReapted < 0)
                                {
                                    crReminder = crReminder - debitItem.NetAmount;
                                    debitItem.NetAmount = 0;

                                }

                            }
                            else
                                debitItem.NetAmount = debitItem.NetAmount - crReminder;
                            columnCount += 1;

                            // }

                        }


                    }

                }
                return ageingData.ToList();
            }
            #endregion

        }

        public IEnumerable<AgeingDataViewModel.AgeingColumnRangeData> testAgeingData(AgeingFilterModel afModel, List<string> customerCode, string companyCode)
        {
            var ageingData = new List<AgeingDataViewModel.AgeingColumnRangeData>();

            IAgeingReportDataService dataService = this._ageingFactory.GetAgeingDataService((AgeingReportType)Enum.Parse(typeof(AgeingReportType), afModel.Type));

            var rawAgingData = dataService.testAgeingData(afModel.AsOnDate, customerCode, afModel.ReportFilters.BranchFilter);
            if (string.IsNullOrEmpty(afModel.AsOnDate))
                afModel.AsOnDate = DateTime.Now.ToString("yyyy-MM-dd");
            var columnRangeData = this.GenerateColumns(afModel.FrequencyInDay.Value, afModel.FixedInDay.Value, afModel.AsOnDate);
            #region customer wise
            if (afModel.Type.Trim().ToLower().Equals("customer"))
            {
                //foreach (var columnItem in columnRangeData)
                //{
                //    if (columnItem.FromDate == null)
                //    {
                //        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                //        {
                //            FromDate = (DateTime?)null,
                //        });
                //    }
                //    else
                //    {
                //        ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                //        {
                //            FromDate = columnItem.FromDate.Value,
                //        });
                //    }
                //}
                var To_Date1 = Convert.ToDateTime(afModel.AsOnDate).ToString("MM/dd/yyyy");
                var From_Date1 = Convert.ToDateTime(To_Date1).AddDays(-30).ToString("MM/dd/yyyy");
                var To_Date2 = Convert.ToDateTime(From_Date1).AddDays(-1).ToString("MM/dd/yyyy");
                var From_Date2 = Convert.ToDateTime(To_Date2).AddDays(-30).ToString("MM/dd/yyyy");
                var To_Date3 = Convert.ToDateTime(From_Date2).AddDays(-1).ToString("MM/dd/yyyy");
                var From_Date3 = Convert.ToDateTime(To_Date3).AddDays(-30).ToString("MM/dd/yyyy");

                var To_Date4 = Convert.ToDateTime(From_Date3).AddDays(-1).ToString("MM/dd/yyyy");
                var From_Date4 = Convert.ToDateTime(To_Date4).AddDays(-30).ToString("MM/dd/yyyy");

                var To_Date5 = Convert.ToDateTime(From_Date4).AddDays(-1).ToString("MM/dd/yyyy");
                var From_Date5 = Convert.ToDateTime(To_Date5).AddDays(-30).ToString("MM/dd/yyyy");
                foreach (var crItem in rawAgingData)
                {
                    var crReminder=0M;
                    var debitAmounts = crItem.DR_AMOUNT4;
                    foreach (var columnItem in columnRangeData.OrderByDescending(x=> x.orderBy))
                    {
                        if (columnItem.ColumnRange == "120+")
                        {
                            crReminder = Convert.ToDecimal(crItem.DR_AMOUNT4 + (-crItem.CR_AMOUNT));
                                ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                                {
                                    NetAmount = Convert.ToDecimal(crReminder),
                                    FromDate = Convert.ToDateTime(From_Date5),
                                    ColumnRangeName = columnItem.ColumnRange,
                                    ToDate = Convert.ToDateTime(To_Date5)
                                });
                            
                        }
                        else if (columnItem.ColumnRange == "91-120")
                        {
                            crReminder = Convert.ToDecimal(crItem.DR_AMOUNT3 + crReminder);
                            ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = Convert.ToDecimal(crReminder),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = Convert.ToDateTime(From_Date4),
                                ToDate = Convert.ToDateTime(To_Date4)
                            });
                        }
                        else if (columnItem.ColumnRange == "61-90")
                        {
                            crReminder = Convert.ToDecimal(Convert.ToDecimal(crItem.DR_AMOUNT2) + crReminder);
                            ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = Convert.ToDecimal(crReminder),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = Convert.ToDateTime(From_Date3),
                                ToDate = Convert.ToDateTime(To_Date3)
                            });
                        }
                        else if (columnItem.ColumnRange == "31-60")
                        {
                            crReminder = Convert.ToDecimal(Convert.ToDecimal(crItem.DR_AMOUNT2) + crReminder);
                            ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = Convert.ToDecimal(crReminder),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = Convert.ToDateTime(From_Date2),
                                ToDate = Convert.ToDateTime(To_Date2)
                            });
                        }
                        else if (columnItem.ColumnRange == "0-30")
                        {
                            crReminder = Convert.ToDecimal(Convert.ToDecimal(crItem.DR_AMOUNT2) + crReminder);
                            ageingData.Add(new AgeingDataViewModel.AgeingColumnRangeData()
                            {
                                NetAmount = Convert.ToDecimal(crReminder),
                                ColumnRangeName = columnItem.ColumnRange,
                                FromDate = Convert.ToDateTime(From_Date1),
                                ToDate = Convert.ToDateTime(To_Date1)
                            });
                        }
                    }
                }
            }
            #endregion
            return ageingData.ToList();
        }


    }
}
