using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using System.Data;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using NeoErp.Core.Integration;
using OfficeOpenXml;
using System.Collections;
using OfficeOpenXml.Style;
using System.Web.Http;

namespace NeoErp.Core.Helpers
{
    public class ExcelHelper : Controller
    {
        public Stream GetFileContent(HttpPostedFileBase uploadFile)
        {
            if (uploadFile != null)
            {
                Stream fileStream = uploadFile.InputStream;
                return fileStream;
            }
            return null;
        }

        //error: skip cells having null value
        #region Old Read
        public static DataTable ReadAsDataTable(Stream fileName)
        {
            DataTable dataTable = new DataTable();
            using (SpreadsheetDocument spreadSheetDocument = SpreadsheetDocument.Open(fileName, false))
            {
                WorkbookPart workbookPart = spreadSheetDocument.WorkbookPart;
                IEnumerable<Sheet> sheets = spreadSheetDocument.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>();
                string relationshipId = sheets.First().Id.Value;
                WorksheetPart worksheetPart = (WorksheetPart)spreadSheetDocument.WorkbookPart.GetPartById(relationshipId);
                Worksheet workSheet = worksheetPart.Worksheet;
                SheetData sheetData = workSheet.GetFirstChild<SheetData>();
                IEnumerable<Row> rows = sheetData.Descendants<Row>();

                foreach (Cell cell in rows.ElementAt(0))
                {
                    dataTable.Columns.Add(GetCellValue(spreadSheetDocument, cell));
                }

                foreach (Row row in rows)
                {
                    DataRow dataRow = dataTable.NewRow();
                    for (int i = 0; i < row.Descendants<Cell>().Count(); i++)
                    {
                        dataRow[i] = GetCellValue(spreadSheetDocument, row.Descendants<Cell>().ElementAt(i));
                    }

                    dataTable.Rows.Add(dataRow);
                }

            }
            dataTable.Rows.RemoveAt(0);

            return dataTable;
        }

        private static string GetCellValue(SpreadsheetDocument document, Cell cell)
        {
            SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;

            if (cell.CellValue == null)
            {
                return string.Empty;
            }

            string value = cell.CellValue.InnerXml;

            if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            {
                return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            }
            else
            {
                return value;
            }
        }

        #endregion 

        #region New Read
        public DataTable ReadFirstCell(Stream fileName)
        {
            DataTable dataTable = new DataTable();
            string value = string.Empty;
            try
            {
                using (SpreadsheetDocument myWorkbook = SpreadsheetDocument.Open(fileName, false))
                {
                    var sheet = myWorkbook.WorkbookPart.Workbook.GetFirstChild<Sheets>().Elements<Sheet>().First();
                    var worksheetPart = (WorksheetPart)myWorkbook.WorkbookPart.GetPartById(sheet.Id.Value);
                    IEnumerable<Row> rows = worksheetPart.Worksheet.GetFirstChild<SheetData>().Elements<Row>();
                    int RowIndex = 0;

                    foreach (var row in rows)
                    {
                        //Create the data table header row ie columns using first excel row.
                        if (RowIndex == 0)
                        {
                            foreach (Cell cell in rows.ElementAt(0))
                            {
                                value = GetCellValue(cell);
                                if (value.ToString().Trim() == string.Empty) continue;
                                dataTable.Columns.Add(value);
                            }

                            // CreateColumnsFromHeaderRow(row, dataTable);

                            RowIndex++;

                        }
                        else
                        {
                            //From second row of excel onwards, add data rows to data table.
                            IEnumerable<Cell> cells = GetCellsFromRowIncludingEmptyCells(row);
                            DataRow newDataRow = dataTable.NewRow();
                            int columnCount = 0;
                            foreach (Cell currentCell in cells)
                            {
                                value = GetCellValue(currentCell);
                                //There are empty headers which are not added to data table columns. So avoid those.
                                if (columnCount < dataTable.Columns.Count)
                                {
                                    newDataRow[columnCount++] = value;
                                }
                            }
                            dataTable.Rows.Add(newDataRow);
                        }
                    }
                }
                return dataTable;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        //Below are the helper methods used

        private static IEnumerable<Cell> GetCellsFromRowIncludingEmptyCells(Row row)
        {
            int currentCount = 0;
            // row is a class level variable representing the current
            foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in
                row.Descendants<DocumentFormat.OpenXml.Spreadsheet.Cell>())
            {
                string columnName = GetColumnName(cell.CellReference);
                int currentColumnIndex = ConvertColumnNameToNumber(columnName);
                //Return null for empty cells
                for (; currentCount < currentColumnIndex; currentCount++)
                {
                    yield return null;
                }
                yield return cell;
                currentCount++;
            }
        }

        public static string GetColumnName(string cellReference)
        {
            // Match the column name portion of the cell name.
            Regex regex = new Regex("[A-Za-z]+");
            Match match = regex.Match(cellReference);

            return match.Value;
        }

        public static int ConvertColumnNameToNumber(string columnName)
        {
            Regex alpha = new Regex("^[A-Z]+$");
            if (!alpha.IsMatch(columnName)) throw new ArgumentException();

            char[] colLetters = columnName.ToCharArray();
            Array.Reverse(colLetters);

            int convertedValue = 0;
            for (int i = 0; i < colLetters.Length; i++)
            {
                char letter = colLetters[i];
                int current = i == 0 ? letter - 65 : letter - 64; // ASCII 'A' = 65
                convertedValue += current * (int)Math.Pow(26, i);
            }

            return convertedValue;
        }

        private string GetCellValue(Cell excelCell)
        {
            if (excelCell == null)
                return null;
            if (excelCell.DataType == null)
                return excelCell.InnerText;

            string value = excelCell.InnerText;
            switch (excelCell.DataType.Value)
            {
                case CellValues.SharedString:
                    value = ReadFromSharedString(excelCell);
                    break;
                case CellValues.Boolean:
                    switch (value)
                    {
                        case "0":
                            value = "false";
                            break;
                        default:
                            value = "true";
                            break;
                    }
                    break;
            }
            return value;
        }


        private string ReadFromSharedString(Cell cell)
        {
            string value = cell.InnerText;
            SharedStringTablePart sstPart = GetSharedTablePart(cell);

            //string value = cell.CellValue.InnerXml;

            //if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
            //{
            //    return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
            //}
            //else
            //{
            //    return value;
            //}

            // lookup value in shared string table
            if (sstPart != null && sstPart.SharedStringTable != null)
            {
                int index = int.Parse(value);
                if (sstPart.SharedStringTable.ElementAt(index).ChildElements.Count != 0)
                {
                    value = sstPart.SharedStringTable.ElementAt(index).InnerText;
                }
                else
                {
                    //ChildElements is collection of Run
                    value = GetStringValueFromRuns(sstPart.SharedStringTable.ElementAt(index).ChildElements);
                }
            }
            return value;
        }


        private SharedStringTablePart GetSharedTablePart(Cell cell)
        {
            Worksheet workSheet = cell.Ancestors<Worksheet>().FirstOrDefault();
            SpreadsheetDocument doc = workSheet.WorksheetPart.OpenXmlPackage as SpreadsheetDocument;
            SharedStringTablePart sharedStringTablePart = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
            return sharedStringTablePart;
        }


        private string GetStringValueFromRuns(OpenXmlElementList openXmlElementList)
        {
            StringBuilder text = new StringBuilder();
            foreach (Run run in openXmlElementList)
            {
                //    //t ADD OPENNING TAGS HERE, if any
                if (run.RunProperties != null)
                {
                    foreach (OpenXmlLeafElement element in run.RunProperties.OrderBy(e => e.LocalName))
                    {
                        //StartTagWrite is the extension method.
                        element.StartTagWrite(text);
                    }
                }
                text.Append(run.Text.Text.Replace("\n", "<br/>"));
                if (run.RunProperties != null)
                {
                    foreach (OpenXmlLeafElement element in run.RunProperties.OrderByDescending(e => e.LocalName))
                    {
                        element.EndTagWrite(text);
                    }
                }
            }
            return text.ToString();
        }


        #endregion


        #region Export to Excel ActionResults
               
        public class ConditionalFormatingModel
        {           
            public ConditionalFormattingOperatorValues ConditionType { get; set; }
            public string CheckForValue { get; set; }
            public string HtmlColor { get; set; }

        }

        /// <summary>
        /// Get Controller and Data and return Excel File
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult Export<T>(Controller controller, IQueryable<T> data, string ReportTitle = "")
        {
            controller.HttpContext.Response.AddHeader("Content-Disposition", "attachment; filename=" + controller.RouteData.Values["controller"].ToString() + ".xls");
            controller.HttpContext.Response.ContentType = "application/ms-excel";

            int perPage = Display.ItemPerPage;
            Display.ItemPerPage = 100000;

            var model = data;
            string result = NeoErp.Core.Helpers.ViewContextExtensions.RenderPartialView(controller, "_AGrid", model);

            if (ReportTitle == "")
                ReportTitle = controller.RouteData.Values["controller"].ToString();

            result = @"<STRONG>" + "_Settings._S.Client.CompanyName" + "</STRONG><br/>" +
                      "_Settings._S.Client.NPVDCname" + ", " + "_Settings._S.Client.District" + "<br/>&nbsp;<br/>" +
                       "<strong>" + ReportTitle + "</strong><br/>&nbsp; <br/>" + result;

            Display.ItemPerPage = perPage;
            return PartialView("Export", result);

        }


        /// <summary>
        /// Get Controller and Data and return Excel File
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult ExportToExcel<T>(Controller controller, IEnumerable<T> data, string ReportTitle = "", Stream stream = null, List<ConditionalFormatingModel> conditionalFormat = null)
        {
            string controllername = controller.RouteData.Values["controller"].ToString();
            if (ReportTitle == "") ReportTitle = controllername;

            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Worksheets.Add(ReportTitle);
                var workSheet = excelPackage.Workbook.Worksheets[1];

                workSheet.DefaultColWidth = 20;
                workSheet.DefaultRowHeight = 18;
                // workSheet.Cells.Style.WrapText = true;


                workSheet.Cells[1, 1].Value = "_Settings._S.Client.CompanyName";
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 15;


                workSheet.Cells[2, 1].Value = "_Settings._S.Client.ClientOneLineAddress";
                workSheet.Cells[2, 1].Style.Font.Size = 14;

                workSheet.Cells[4, 1].Value = ReportTitle;
                workSheet.Cells[4, 1].Style.Font.Bold = true;
                workSheet.Cells[4, 1].Style.Font.Size = 14;

                workSheet.Cells[1, 1, 4, 1].Style.WrapText = false;


                workSheet.Cells[5, 1].LoadFromCollection<T>(data, true, OfficeOpenXml.Table.TableStyles.Light18);

                workSheet = DoConditionalFormatting(workSheet, conditionalFormat);

                excelPackage.Save();
                var memoryStream = excelPackage.Stream as MemoryStream;

                controller.HttpContext.Response.ClearContent();
                controller.HttpContext.Response.ClearHeaders();
                controller.HttpContext.Response.Buffer = true;
                controller.HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=" + controllername + ".xlsx");
                controller.HttpContext.Response.ContentType = "application/ms-excel";
                controller.HttpContext.Response.AppendHeader("Accept-Header", memoryStream.Length.ToString());
                controller.HttpContext.Response.BinaryWrite(memoryStream.ToArray());
                controller.HttpContext.Response.Flush();
                controller.HttpContext.Response.End();

            }

            return Json("Done");


        }



        /// <summary>
        /// Get Controller and Data and return Excel File
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="controller"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ActionResult ExportToExcel(Controller controller, DataTable data, string ReportTitle = "", List<DBFieldProperties> exportFields = null, bool CalculateSummary = false, bool IsDisplaySrNo = true, bool IsRepeatGroupHeader = false, bool IsDisplayGroupNo = true, List<ConditionalFormatingModel> conditionalFormat = null, string GroupByColumn = "")
        {

            var memoryStream = GenerateExcelSheet(data, ReportTitle, exportFields, CalculateSummary, IsDisplaySrNo, IsRepeatGroupHeader, IsDisplayGroupNo,conditionalFormat,GroupByColumn) as MemoryStream;

            controller.HttpContext.Response.ClearContent();
            controller.HttpContext.Response.ClearHeaders();
            controller.HttpContext.Response.Buffer = true;
            controller.HttpContext.Response.AddHeader("content-disposition", "attachment;  filename=" + controller.RouteData.Values["controller"].ToString() + ".xlsx");
            controller.HttpContext.Response.ContentType = "application/ms-excel";
            controller.HttpContext.Response.AppendHeader("Accept-Header", memoryStream.Length.ToString());
            controller.HttpContext.Response.BinaryWrite(memoryStream.ToArray());
            controller.HttpContext.Response.Flush();
            controller.HttpContext.Response.End();

            return Json("Done");

        }

        #endregion

        #region Generate Excel Sheet

        /// <summary>
        /// Get DataTable and Return Excel as MemoryStream
        /// </summary>

        /// <param name="data">DataTable</param>
        /// <returns></returns>
        public MemoryStream GenerateExcelSheet(DataTable data, string ReportTitle = "", List<DBFieldProperties> exportFields = null, bool CalculateSummary = false, bool IsDisplaySrNo = true, bool IsRepeatGroupHeader = true, bool IsDisplayGroupNo = true,List<ConditionalFormatingModel> conditionalFormat=null, string GroupByColumn = "")
        {
            Stream stream = null;
            using (var excelPackage = new ExcelPackage(stream ?? new MemoryStream()))
            {
                excelPackage.Workbook.Worksheets.Add(ReportTitle);
                ExcelWorksheet workSheet = excelPackage.Workbook.Worksheets[1];

                //workSheet.DefaultColWidth = 20;
                workSheet.DefaultRowHeight = 22;
                
               
                // workSheet.Cells.Style.WrapText = true;
                workSheet.PrinterSettings.FitToPage = true;
                workSheet.PrinterSettings.FitToWidth = 1;
                workSheet.PrinterSettings.LeftMargin = (decimal)0.25;
                workSheet.PrinterSettings.RightMargin = (decimal)0.25;
                workSheet.PrinterSettings.Orientation = eOrientation.Landscape;

                workSheet.Cells[1, 1].Value = "_Settings._S.Client.CompanyName";
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 15;


                workSheet.Cells[2, 1].Value = "_Settings._S.Client.ClientOneLineAddress";
                workSheet.Cells[2, 1].Style.Font.Size = 14;

                workSheet.Cells[4, 1].Value = ReportTitle;
                workSheet.Cells[4, 1].Style.Font.Bold = true;
                workSheet.Cells[4, 1].Style.Font.Size = 14;

                workSheet.Cells[1, 1, 4, 1].Style.WrapText = false;

                if (exportFields == null && GroupByColumn=="" )
                {
                    workSheet.Cells[5, 1].LoadFromDataTable(data, true, OfficeOpenXml.Table.TableStyles.Light18);
                }
                else
                {
                    if (exportFields == null)
                    {
                        exportFields = new List<DBFieldProperties>();
                        foreach (DataColumn col in data.Columns)
                        {
                            exportFields.Add(new ExcelHelper.DBFieldProperties() { FieldName = col.ColumnName, FieldCaption = col.ColumnName, Summary = "", GroupBy = col.ColumnName == GroupByColumn ? true : false });
                        }
                    }
                    workSheet = ExportNonTemplate(data, workSheet, exportFields, 5, 1, CalculateSummary, IsDisplaySrNo, IsRepeatGroupHeader, IsDisplayGroupNo);
                }


                workSheet = DoConditionalFormatting(workSheet, conditionalFormat);
                

                

                excelPackage.Save();

                var memoryStream = excelPackage.Stream as MemoryStream;
                return memoryStream;

            }



        }

        ExcelWorksheet DoConditionalFormatting(ExcelWorksheet workSheet, List<ConditionalFormatingModel> conditionalFormat = null)
        {
            if (conditionalFormat != null)
            {
                foreach (var item in conditionalFormat)
                {
                    if (item.HtmlColor == "") continue;
                    string _statement = "";
                    if (item.ConditionType == ConditionalFormattingOperatorValues.Equal)
                        _statement = string.Format("{0}{1}\"{2}\"", "A1", "=", item.CheckForValue);
                    else
                        continue;

                    var _cond = workSheet.ConditionalFormatting.AddExpression(new ExcelAddress(workSheet.Dimension.Address));
                    _cond.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    _cond.Style.Fill.BackgroundColor.Color = System.Drawing.ColorTranslator.FromHtml(item.HtmlColor);
                    _cond.Formula = _statement;

                    //_statement = "A1=\"L\"";
                    //var _cond = workSheet.ConditionalFormatting.AddExpression(new ExcelAddress(workSheet.Dimension.Address));
                    //_cond.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    //_cond.Style.Fill.BackgroundColor.Color = System.Drawing.ColorTranslator.FromHtml("#FFFFEB84");
                    //_cond.Formula = _statement;
                }
            }
            return workSheet;
        }


        #region Export Helper Functions

        public enum ExcelCellFormats
        {
            Account,
            Currency,
            Text,
            Dt,
            DtText,
            General,
            GeneralNumber,
            Int
        }

        // Number Format String Equivalents
        private string _acct = "_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)";
        private string _int = "#";
        private string _currency = "$#,##0.00_);($#,##0.00)";
        private string _txt = "@";
        private string _date = "dd/mm/yyyy";
        private string _date_text = "dd/mm/yyyy;@";
        private string _general = "General";
        private string _generalNumber = "#,##0";

        public string ChangeExcelCellFormat(ExcelCellFormats FormatType)
        {
            string _numFormat = string.Empty;
            switch (FormatType)
            {
                case ExcelCellFormats.Account:
                    _numFormat = _acct;
                    break;
                case ExcelCellFormats.Currency:
                    _numFormat = _currency;
                    break;
                case ExcelCellFormats.Dt:
                    _numFormat = _date;
                    break;
                case ExcelCellFormats.General:
                    _numFormat = _general;
                    break;
                case ExcelCellFormats.Text:
                    _numFormat = _txt;
                    break;
                case ExcelCellFormats.GeneralNumber:
                    _numFormat = _generalNumber;
                    break;
                case ExcelCellFormats.Int:
                    _numFormat = _int;
                    break;
                case ExcelCellFormats.DtText:
                    _numFormat = _date_text;
                    break;
                default:
                    _numFormat = string.Empty;
                    break;
            }
            return _numFormat;
        }

        public struct DBFieldProperties
        {
            public string FieldName;
            public string FieldCaption;
            public ConvertOptions ConvertTo;
            public bool IsNoRepeat;
            public string Summary;
            public bool GroupBy;
        }

        public enum ConvertOptions
        {
            Text = 1,
            NonDecimal,
            Decimal,
            Date,
            NoExport,
            None
        }

        public enum ReportGroupTypes
        {
            GroupInTop =1,
            GroupInColumn=2
        }

        public ExcelCellFormats ConvertExcel(string Type)
        {
            if (Type == "Text")
            {
                return ExcelCellFormats.Text;
            }
            else if (Type == "Integer")
            {
                return ExcelCellFormats.Int;
            }
            else if (Type == "Decimal")
            {
                return ExcelCellFormats.Account;
            }
            else if (Type == "Date")
            {
                return ExcelCellFormats.Dt;
            }
            else
            {
                return ExcelCellFormats.Text;
            }
        }

        private ExcelWorksheet ExportNonTemplate(DataTable tbl, ExcelWorksheet wSheet, List<DBFieldProperties> exportFields, int StartRow, int StartCol, bool CalculateSummary = false, bool IsDisplaySrNo = true, bool IsRepeatGroupHeader = true, bool IsDisplayGroupNo = true, ReportGroupTypes GroupReportType= ReportGroupTypes.GroupInTop)
        {
            //*** Exporting Non Templete Begin ***' 
          //  ReportGroupTypes GroupReportType = ReportGroupTypes.GroupInColumn;

            try
            {

                int ColumnNumber = StartCol;
                int RowNumber = StartRow;

                int colIndex = ColumnNumber;
                int rowIndex = RowNumber;

                Dictionary<string, DBFieldProperties> DbFieldProp = new Dictionary<string, DBFieldProperties>();
                Dictionary<string, string> DBFieldFormula = new Dictionary<string, string>();
                Dictionary<string, string> DBFieldGroup = new Dictionary<string, string>();
                ArrayList DBFieldNoRepeat = new ArrayList();


                string GroupBy = "";

                #region Preparing field and group for export

                if (exportFields.Count == 0)
                {
                    foreach (DataColumn field in tbl.Columns)
                    {
                        DBFieldProperties FieldProp = new DBFieldProperties()
                        {
                            FieldName = field.ColumnName,
                            FieldCaption = field.ColumnName
                        };
                        DbFieldProp.Add(FieldProp.FieldName, FieldProp);
                    }
                }
                else
                {
                    foreach (DBFieldProperties field in exportFields)
                    {
                        if (field.ConvertTo == ConvertOptions.NoExport) continue;

                        DBFieldProperties FieldProp = new DBFieldProperties()
                        {
                            FieldName = field.FieldName,
                            FieldCaption = field.FieldCaption,
                            ConvertTo = field.ConvertTo,
                            GroupBy = field.GroupBy,
                            IsNoRepeat = field.IsNoRepeat,
                            Summary = field.Summary
                        };

                       // field.CopyPropertiesTo(FieldProp);

                        DbFieldProp.Add(FieldProp.FieldName, FieldProp);

                        if (FieldProp.IsNoRepeat == true)
                        {
                            DBFieldNoRepeat.Add(FieldProp.FieldName);
                        }

                        DBFieldFormula.Add(FieldProp.FieldName, FieldProp.Summary);

                        if (FieldProp.GroupBy == true)
                            GroupBy = FieldProp.FieldName;

                        //if (dt.Columns.Contains(_with2.FieldName) & tblCaption.DefaultView(K).Item("IsPresistColumnPosition") == false)
                        //{
                        //    dt.Columns(_with2.FieldName).SetOrdinal(DisplayIndex);
                        //    DisplayIndex += 1;
                        //}

                    }

                    //if (!string.IsNullOrEmpty(GroupByColumn) && (GroupByColumn != null) && tbl.Columns.Contains(GroupByColumn))
                    //    GroupBy = GroupByColumn;

                }




                if (!string.IsNullOrEmpty(GroupBy))
                {
                    for (int g = 0; g <= tbl.Rows.Count - 1; g++)
                    {
                        if (tbl.Rows[g][GroupBy] == null)
                        {
                            if (DBFieldGroup.ContainsValue("null") == false)
                                DBFieldGroup.Add(g.ToString(), "null");
                        }
                        else
                        {
                            if (DBFieldGroup.ContainsValue(tbl.Rows[g][GroupBy].ToString().Trim()) == false)
                                DBFieldGroup.Add(g.ToString(), tbl.Rows[g][GroupBy].ToString());
                        }

                    }
                }
                if (DBFieldGroup.Count == 0) DBFieldGroup.Add("0", "");

                #endregion


                int GroupColIndex = 0;
                int GroupRowIndex = 0;

                GroupColIndex = ColumnNumber;
                GroupRowIndex = RowNumber;

                rowIndex = GroupRowIndex + 1;
                int LastColumnIndex = 0;
                int GroupNo = 0;
                dynamic Rng1 = null;
                dynamic Rng2 = null;
                //*** Looping for Each Group ***'

                foreach (KeyValuePair<string, string> gf in DBFieldGroup)
                {
                    //*** Exporting Columns ***'
                    #region Exporting Group Columns

                    colIndex = ColumnNumber;
                    //rowIndex += 1

                    if (GroupNo == 0 || IsRepeatGroupHeader == true)
                    {
                        GroupNo += 1;
                                              

                        if(GroupReportType== ReportGroupTypes.GroupInColumn )
                        {
                           // wSheet.InsertRow(rowIndex + 1, 1);

                            if (IsDisplayGroupNo == true)
                            {
                                wSheet.Cells[rowIndex, colIndex].Value = "G.No";
                                wSheet.Cells[rowIndex, colIndex].Style.Font.Bold = true;
                                wSheet.Column(1).Width = 7;
                                colIndex += 1;
                            }
                            wSheet.Cells[rowIndex, colIndex].Value = GroupBy;
                            wSheet.Cells[rowIndex, colIndex].Style.Font.Bold = true;
                            colIndex += 1;
                        }
                        else
                        {
                           // wSheet.InsertRow(rowIndex + 1, 4);
                        }


                        if (IsDisplaySrNo == true)
                        {
                            wSheet.Cells[rowIndex, colIndex].Value = "Sr.No.";
                            wSheet.Cells[rowIndex, colIndex].Style.Font.Bold = true;
                          //  wSheet.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                         //   wSheet.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.Indexed = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.WhiteSmoke);

                            colIndex += 1;
                        }

                        foreach (DataColumn dc0 in tbl.Columns)
                        {                           
                            if (DbFieldProp.ContainsKey(dc0.ColumnName) == false)
                                continue;
                            if (dc0.ColumnName == "SNo")
                                continue;
                            if (dc0.ColumnName == GroupBy)
                                continue;

                            wSheet.Cells[rowIndex, colIndex].Value = DbFieldProp[dc0.ColumnName].FieldCaption;
                            wSheet.Cells[rowIndex, colIndex].Style.Font.Bold = true;
                          //  wSheet.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                           // wSheet.Cells[rowIndex, colIndex].Style.Fill.BackgroundColor.Indexed = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.WhiteSmoke);

                            colIndex = colIndex + 1;
                        }


                       // wSheet.Cells[rowIndex, 1, rowIndex, colIndex].Style.WrapText = true;

                    }

                    //*** Exporting Column End ***'

                    if (LastColumnIndex == 0)
                    {
                        //  MergeHeader(wSheet, Excel, colIndex);
                        LastColumnIndex = colIndex - 1;
                    }


                    #endregion

                    
                    //*** Exporting Data ***'
                    #region Exporting Group 
                    Rng1 = wSheet.Cells[rowIndex + 1, ColumnNumber].Address;

                    if (!string.IsNullOrEmpty(GroupBy))
                    {
                        if (gf.Value == "null")
                        {
                            tbl.DefaultView.RowFilter = string.Format("{0} is null", GroupBy);
                        }
                        else
                        {
                            tbl.DefaultView.RowFilter = string.Format("{0} ='{1}'", GroupBy, gf.Value.ToString()); //RT(enmRTOption.Input, gf.Value.ToString())
                        }

                        if (GroupReportType == ReportGroupTypes.GroupInTop)
                        {

                            if (GroupRowIndex > 0 && GroupColIndex > 0)
                            {
                                wSheet.Cells[GroupRowIndex, GroupColIndex].Value = string.Format("{0}: {1}", DbFieldProp[GroupBy].FieldCaption, gf.Value.ToString());
                                wSheet.SelectedRange[wSheet.Cells[GroupRowIndex, GroupColIndex].Address].Style.Font.Bold = true;
                                //rowIndex = GroupRowIndex;

                            }

                           // wSheet.Cells[GroupRowIndex, GroupColIndex, GroupRowIndex, LastColumnIndex].Merge = true;
                            // wSheet.Cells[GroupRowIndex, GroupColIndex, GroupRowIndex, LastColumnIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            //wSheet.Cells[GroupRowIndex, GroupColIndex, GroupRowIndex, LastColumnIndex].Style.Font.Size = 13;

                           // wSheet.Cells[GroupRowIndex, GroupColIndex, GroupRowIndex, LastColumnIndex].Style.WrapText = false;
                            //wSheet.Cells[GroupRowIndex, GroupColIndex, GroupRowIndex, LastColumnIndex].Style.Fill.BackgroundColor.Indexed = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.WhiteSmoke);
                        }
                    }
                    #endregion


                    int Count = 0;
                    DataRowView dr = default(DataRowView);
                    DataRowView drPrev = default(DataRowView);
                    int rowGroupBegin = rowIndex + 1;

                    for (int r = 0; r <= tbl.DefaultView.Count - 1; r++)
                    {
                        #region Exporting Data
                        /// Reading Previous Row value to check to avoid repeative data exporting for same column
                        if (r > 0)
                        {
                            drPrev = tbl.DefaultView[r - 1];
                        }

                        dr = tbl.DefaultView[r];
                        colIndex = ColumnNumber;
                        rowIndex = rowIndex + 1;
                        Count += 1;

                        if (GroupReportType == ReportGroupTypes.GroupInColumn)
                        {
                            if (IsDisplayGroupNo == true)
                            {
                               if(Count==1) wSheet.Cells[rowIndex, colIndex].Value = GroupNo;                               
                                colIndex += 1;
                            }
                            if (Count == 1) wSheet.Cells[rowIndex, colIndex].Value = dr[GroupBy];
                            colIndex += 1;
                        }


                        if (IsDisplaySrNo == true)
                        {
                            wSheet.Cells[rowIndex, colIndex].Value = Count;    
                            colIndex += 1;
                        }

                        foreach (DataColumn dc1 in tbl.Columns)
                        {

                            if (DbFieldProp.ContainsKey(dc1.ColumnName) == false)
                                continue;
                            if (dc1.ColumnName == "SNo")
                                continue;
                            if (dc1.ColumnName == GroupBy)
                                continue;

                            if (r > 0)
                            {

                                if (DbFieldProp[dc1.ColumnName].IsNoRepeat == true)
                                {
                                    bool IsRepeat = false;
                                    foreach (string item in DBFieldNoRepeat)
                                    {
                                        if (drPrev[item].ToString() != dr[item].ToString())
                                        {
                                            IsRepeat = true;
                                        }
                                    }

                                    if (IsRepeat == false)
                                    {                                       
                                      //  wSheet.Cells[rowIndex - 1, colIndex, rowIndex, colIndex].Merge = true;

                                        colIndex = colIndex + 1;
                                        continue;
                                    }
                                }

                            }


                            if (dr[dc1.ColumnName] != null)
                            {

                                if (tbl.Columns[dc1.ColumnName].DataType.ToString() == "System.DateTime")
                                {
                                    string dtIn = string.Format("{0:dd/MM/yyyy}", dr[dc1.ColumnName]);
                                    wSheet.Cells[rowIndex, colIndex].Value = dtIn;
                                }
                                else if (tbl.Columns[dc1.ColumnName].DataType.ToString() == "System.Int32")
                                {                                  
                                    wSheet.Cells[rowIndex, colIndex].Value = dr[dc1.ColumnName];
                                }
                                else if (tbl.Columns[dc1.ColumnName].DataType.ToString() == "System.Double")
                                {
                                    wSheet.Cells[rowIndex, colIndex].Style.Numberformat.Format = _acct;     
                                    wSheet.Cells[rowIndex, colIndex].Value = dr[dc1.ColumnName];
                                }
                                else
                                {
                                    wSheet.Cells[rowIndex, colIndex].Value = dr[dc1.ColumnName].ToString();
                                }

                               // wSheet.Cells[rowIndex, colIndex].Value = dr[dc1.ColumnName].ToString();
                               // wSheet.Cells[rowIndex, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);


                                colIndex = colIndex + 1;
                            }
                        }

                        wSheet.InsertRow(rowIndex + 1, 1);

                        #endregion

                    }
                
                
                    //*** Exporting Data End ***'


                    //*** Exporting Summary (Total) Formula if Calculate Summmary is True ***'
                    #region Exporting Summary

                    int SummaryRows = 0;

                    if (CalculateSummary == true && exportFields.Count > 0)
                    {
                        colIndex = ColumnNumber;

                        if (GroupReportType == ReportGroupTypes.GroupInColumn)
                        {
                            if (IsDisplayGroupNo == true)
                            {                                
                                colIndex += 1;
                            }
                           
                            colIndex += 1;
                        }


                        if (IsDisplaySrNo == true)
                            colIndex += 1;
                        wSheet.Row(rowIndex + 1).Style.Font.Bold = true;
                        foreach (DataColumn dcSum in tbl.Columns)
                        {
                            if (DBFieldFormula.ContainsKey(dcSum.ColumnName) == false)
                                continue;
                            if (dcSum.ColumnName == GroupBy)
                                continue;
                            if (dcSum.ColumnName == "SNo")
                            {
                                colIndex += 1;
                                continue;
                            }


                            if (DBFieldFormula[dcSum.ColumnName].ToString().Length > 0)
                            {
                                SummaryRows = 1;
                                dynamic C1 = null;
                                dynamic C2 = null;
                                int FormulaStartIndex = 0;
                                if (IsRepeatGroupHeader == true | GroupRowIndex <= 6)
                                {
                                    FormulaStartIndex = GroupRowIndex + 2;
                                }
                                else
                                {
                                    FormulaStartIndex = GroupRowIndex + 1;
                                }
                                C1 = wSheet.Cells[FormulaStartIndex, colIndex].Address;
                                C2 = wSheet.Cells[rowIndex, colIndex].Address;
                                C1 = C1.ToString().Replace("$", "");
                                C2 = C2.ToString().Replace("$", "");

                                wSheet.Cells[rowIndex + 1, colIndex].Style.Numberformat.Format = ChangeExcelCellFormat(ExcelCellFormats.General);
                                wSheet.Cells[rowIndex + 1, colIndex].FormulaR1C1 = DBFieldFormula[dcSum.ColumnName] + "(" + C1 + ": " + C2 + ")";
                               // wSheet.Cells[rowIndex + 1, colIndex].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                            }
                            colIndex = colIndex + 1;
                        }

                    }
                    #endregion

                    //*** Exporting Summary (Total) ***'

                    //*** Fortmatting Exported Data Range ***'

                    if (GroupReportType == ReportGroupTypes.GroupInColumn)
                    {
                        wSheet.Cells[rowGroupBegin, StartCol, rowIndex, StartCol].Merge = true;
                        wSheet.Cells[rowGroupBegin, StartCol, rowIndex, StartCol].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                        wSheet.Cells[rowGroupBegin, StartCol + 1, rowIndex, StartCol + 1].Merge = true;
                        wSheet.Cells[rowGroupBegin, StartCol + 1, rowIndex, StartCol + 1].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                        wSheet.Cells[rowGroupBegin, StartCol + 1, rowIndex, StartCol + 1].Style.WrapText = true;

                        var cells = wSheet.Cells[rowGroupBegin - 1, StartCol, rowIndex + 1, colIndex - 1];
                        //foreach(var cell in cells)
                        //{
                        //    cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        //}
                        //wSheet.Cells[rowGroupBegin - 2, StartCol+2, rowIndex + 1, colIndex - 1].AutoFitColumns(5, 100);


                        rowIndex += 2;
                    }
                    else
                    {
                        var cells = wSheet.Cells[rowGroupBegin - 1, StartCol, rowIndex + 1, colIndex - 1];
                        foreach (var cell in cells)
                        {
                            cell.Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        }

                        //var exTable = wSheet.Tables.Add(new ExcelAddressBase(rowGroupBegin - 1, StartCol, rowIndex+1, colIndex - 1), gf.Value.ToString()); // = true;  //(OfficeOpenXml.Style.ExcelBorderStyle.Thin);
                        //exTable.TableStyle = OfficeOpenXml.Table.TableStyles.Light18;
                        //exTable.ShowHeader = true;

                        rowIndex += 3;
                    }
                    
                    //*** Formating End ***'

                   
                    GroupRowIndex = rowIndex;

                    if (IsRepeatGroupHeader == true)
                        rowIndex += 1;

                }
                wSheet.DefaultRowHeight = 22;

                return wSheet;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }


        //public object InsertSpaceBeforeCapitalLetter(string txt)
        //{
        //    try
        //    {
        //        txt = txt.Trim();
        //        if (txt.Length == 0)
        //            return txt;
        //        var Result = txt.ToCharArray(); // (0);
        //        for (int i = 1; i <= txt.Length - 1; i++)
        //        {
        //            if (Strings.Asc(txt.Chars(i)) >= 65 & Strings.Asc(txt.Chars(i)) <= 90)
        //            {
        //                try
        //                {
        //                    if (txt.Chars(i - 1) != " " & txt.Chars(i - 1) != "(" & !(Strings.Asc(txt.Chars(i + 1)) >= 65 & Strings.Asc(txt.Chars(i + 1)) <= 90) & ((Strings.Asc(txt.Chars(i + 1)) >= 97 & Strings.Asc(txt.Chars(i + 1)) <= 122)))
        //                    {
        //                        Result += " " + txt.Chars(i);
        //                    }
        //                    else
        //                    {
        //                        Result += txt.Chars(i);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    Result += txt.Chars(i);
        //                }
        //            }
        //            else
        //            {
        //                Result += txt.Chars(i);
        //            }
        //        }
        //        return Result;
        //    }
        //    catch
        //    {
        //        return txt;
        //    }

        //}

        #endregion

        #endregion

    }

    static class Extension
    {
        public static void StartTagWrite(this OpenXmlLeafElement runElement, StringBuilder writer)
        {
            IWriteOperation x = new HTMLWriteOperation { Writer = writer };
            x.StartTagWrite((dynamic)runElement);
        }
        public static void EndTagWrite(this OpenXmlLeafElement runElement, StringBuilder writer)
        {
            IWriteOperation x = new HTMLWriteOperation { Writer = writer };
            x.EndTagWrite((dynamic)runElement);
        }

    }

    interface IWriteOperation
    {
        void StartTagWrite(Bold bold);
        void StartTagWrite(Italic it);
        void StartTagWrite(Underline underline);
        void StartTagWrite(Color c);
        void StartTagWrite(Font font);
        void StartTagWrite(FontSize fontSize);
        void StartTagWrite(FontFamily fontFamily);
        void StartTagWrite(FontScheme fontScheme);
        void StartTagWrite(RunFont runFont);
        void StartTagWrite(Strike strike);
        void StartTagWrite(VerticalTextAlignment vertialAlignment);
        void StartTagWrite(Shadow shadow);
        void StartTagWrite(Outline outline);
        void StartTagWrite(Condense condense);

        void EndTagWrite(Bold b);
        void EndTagWrite(Italic b);
        void EndTagWrite(Underline b);
        void EndTagWrite(Color u);
        void EndTagWrite(Font u);
        void EndTagWrite(FontSize u);
        void EndTagWrite(FontFamily u);
        void EndTagWrite(RunFont u);
        void EndTagWrite(FontScheme u);
        void EndTagWrite(Strike u);
        void EndTagWrite(VerticalTextAlignment u);
        void EndTagWrite(Shadow u);
        void EndTagWrite(Outline u);
        void EndTagWrite(Condense u);
    }

    class HTMLWriteOperation : IWriteOperation
    {
        public StringBuilder Writer { get; set; }

        void IWriteOperation.StartTagWrite(Bold iThing) { Writer.AppendLine("<B>"); }
        void IWriteOperation.EndTagWrite(Bold iThing) { Writer.AppendLine("</B>"); }

        void IWriteOperation.EndTagWrite(Underline aThing) { Writer.AppendLine("</U>"); }
        void IWriteOperation.StartTagWrite(Underline aThing) { Writer.AppendLine("<U>"); }

        void IWriteOperation.StartTagWrite(Italic aThing) { Writer.AppendFormat("<I>"); }
        void IWriteOperation.EndTagWrite(Italic aThing) { Writer.AppendFormat("</I>"); }

        void IWriteOperation.StartTagWrite(Strike u) { Writer.AppendLine("<del>"); }
        void IWriteOperation.EndTagWrite(Strike aThing) { Writer.AppendLine("</del>"); }

        void IWriteOperation.StartTagWrite(Color u)
        {
            if (u.Rgb != null)
            {
                string colorFormat = @"<span style=""color:#{0};"">";
                string span = string.Format(colorFormat, u.Rgb.Value.Substring(2, 6));
                Writer.AppendLine(span);
            }
        }
        void IWriteOperation.EndTagWrite(Color aThing)
        {
            Writer.AppendLine("</span>");
        }
        void IWriteOperation.StartTagWrite(RunFont u)
        {
            if (u.Val.HasValue)
            {
                string fontFormat = @"<span style=""font-family:'{0}';"">";
                string span = string.Format(fontFormat, u.Val.Value);
                Writer.AppendLine(span);
            }
        }
        void IWriteOperation.EndTagWrite(RunFont aThing)
        {
            Writer.AppendLine("</span>");
        }

        void IWriteOperation.EndTagWrite(FontSize aThing)
        {
            Writer.AppendLine("</span>");
        }
        void IWriteOperation.StartTagWrite(FontSize u)
        {
            string fontSizeFormat = @"<span style=""font-size:{0}px;"">";
            string span = string.Format(fontSizeFormat, u.Val);
            Writer.AppendLine(span);
        }

        void IWriteOperation.StartTagWrite(Font u) { }
        void IWriteOperation.EndTagWrite(Font aThing) { }

        void IWriteOperation.EndTagWrite(FontScheme aThing) { }
        void IWriteOperation.StartTagWrite(FontScheme u) { }

        void IWriteOperation.EndTagWrite(Condense aThing) { }
        void IWriteOperation.StartTagWrite(Condense u) { }

        void IWriteOperation.EndTagWrite(VerticalTextAlignment aThing) { }
        void IWriteOperation.StartTagWrite(VerticalTextAlignment u) { }

        void IWriteOperation.EndTagWrite(Shadow aThing) { }
        void IWriteOperation.StartTagWrite(Shadow u) { }

        void IWriteOperation.StartTagWrite(FontFamily u) { }
        void IWriteOperation.EndTagWrite(FontFamily aThing) { }

        void IWriteOperation.StartTagWrite(Outline u) { }
        void IWriteOperation.EndTagWrite(Outline aThing) { }
    }

}

