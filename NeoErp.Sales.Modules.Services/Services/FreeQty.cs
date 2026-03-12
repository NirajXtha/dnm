

using NeoErp.Core;
using NeoErp.Core.Domain;
using NeoErp.Core.Helpers;
using NeoErp.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using NeoErp.Sales.Modules.Services.Models;
using System.IO;
using OfficeOpenXml;

namespace NeoErp.Sales.Modules.Services.Services
{
    public class FreeQty : IFreeQty
    {
        private readonly NeoErpCoreEntity _objectEntity;
        private readonly IWorkContext _workContext;

        public FreeQty(NeoErpCoreEntity objectEntity, IWorkContext workContext)
        {
            _objectEntity = objectEntity;
            _workContext = workContext;
            
            // Initialize table if it doesn't exist
            InitializeFreeQuantityTable();
        }

        private void InitializeFreeQuantityTable()
        {
            try
            {
                string checkTableQuery = @"
                    SELECT COUNT(*) 
                    FROM user_tables 
                    WHERE table_name = 'FREE_QUANTITY'";

                using (var command = _objectEntity.Database.Connection.CreateCommand())
                {
                    command.CommandText = checkTableQuery;
                    
                    if (_objectEntity.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        _objectEntity.Database.Connection.Open();
                    }
                    
                    var result = command.ExecuteScalar();
                    int tableCount = Convert.ToInt32(result);
                    
                    if (tableCount == 0)
                    {
                        // Table doesn't exist, create it
                        string createTableQuery = @"
                            CREATE TABLE FREE_QUANTITY
                            (
                              FREE_QTY_NO     NUMBER,
                              EFFECTIVE_DATE  DATE,
                              ITEM_CODE       VARCHAR2(50 BYTE),
                              QTY             NUMBER,
                              MAIN_UNIT       VARCHAR2(50 BYTE),
                              FREE_QTY        NUMBER,
                              FREE_UNIT       VARCHAR2(50 BYTE),
                              CUSTOMER_CODE   VARCHAR2(100 BYTE),
                              FORM_CODE       VARCHAR2(50 BYTE),
                              CREATED_BY      VARCHAR2(50 BYTE),
                              CREATED_DATE    DATE,
                              MODIFIED_DATE   DATE,
                              DELETED_FLAG    VARCHAR2(1 BYTE),
                              SERIAL_NO       NUMBER,
                              COMPANY_CODE    VARCHAR2(10 BYTE),
                              MAIN_SERIAL     NUMBER,
                              FREE_SERIAL     NUMBER
                            )";
                        
                        using (var createCmd = _objectEntity.Database.Connection.CreateCommand())
                        {
                            createCmd.CommandText = createTableQuery;
                            createCmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine("FREE_QUANTITY table created successfully");
                        }
                    }
                    
                    if (_objectEntity.Database.Connection.State == System.Data.ConnectionState.Open)
                    {
                        _objectEntity.Database.Connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error initializing FREE_QUANTITY table: {ex.Message}");
                // Don't throw - allow application to continue even if table creation fails
            }
        }

        public List<dynamic> AllCustomer(string searchTerm = "")
        {
            string query = @"
                SELECT CUSTOMER_CODE, CUSTOMER_EDESC
                FROM sa_customer_setup
                WHERE company_code = '01'";

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query += " AND (UPPER(CUSTOMER_CODE) LIKE :searchTerm OR UPPER(CUSTOMER_EDESC) LIKE :searchTerm)";
            }

            var customers = new List<dynamic>();
            
            using (var command = _objectEntity.Database.Connection.CreateCommand())
            {
                command.CommandText = query;
                
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    var param = command.CreateParameter();
                    param.ParameterName = "searchTerm";
                    param.Value = "%" + searchTerm.ToUpper() + "%";
                    command.Parameters.Add(param);
                }
                
                _objectEntity.Database.Connection.Open();
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var customer = new Dictionary<string, object>
                        {
                            { "CUSTOMER_CODE", reader["CUSTOMER_CODE"] },
                            { "CUSTOMER_EDESC", reader["CUSTOMER_EDESC"] }
                        };
                        customers.Add(customer);
                    }
                }
                
                _objectEntity.Database.Connection.Close();
            }
            
            return customers;
        }

        public List<dynamic> GetFormSetup()
        {
            string query = @" SELECT form_code, form_edesc FROM form_setup WHERE company_code = '01' and module_code='04' and  GROUP_SKU_FLAG ='I' order by MASTER_FORM_CODE ";

            var forms = new List<dynamic>();
            
            using (var command = _objectEntity.Database.Connection.CreateCommand())
            {
                command.CommandText = query;
                _objectEntity.Database.Connection.Open();
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var form = new Dictionary<string, object>
                        {
                            { "form_code", reader["form_code"] },
                            { "form_edesc", reader["form_edesc"] }
                        };
                        forms.Add(form);
                    }
                }
                
                _objectEntity.Database.Connection.Close();
            }
            
            return forms;
        }

        public List<dynamic> GetItemTreeView()
        {
            string query = @"
                SELECT item_code, item_edesc, master_item_code child_code, 
                       pre_item_code parent_code, group_sku_flag, index_mu_code unit 
                FROM ip_item_master_setup 
                WHERE company_code = '01' AND deleted_flag = 'N'
                ORDER BY master_item_code, pre_item_code";

            var items = new List<dynamic>();
            
            using (var command = _objectEntity.Database.Connection.CreateCommand())
            {
                command.CommandText = query;
                _objectEntity.Database.Connection.Open();
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new Dictionary<string, object>
                        {
                            { "item_code", reader["item_code"] },
                            { "item_edesc", reader["item_edesc"] },
                            { "child_code", reader["child_code"] },
                            { "parent_code", reader["parent_code"] },
                            { "group_sku_flag", reader["group_sku_flag"] },
                            { "unit", reader["unit"] }
                        };
                        items.Add(item);
                    }
                }
                
                _objectEntity.Database.Connection.Close();
            }
            
            // Fetch available units for each item
            foreach (var item in items)
            {
                var itemDict = item as IDictionary<string, object>;
                if (itemDict != null && itemDict["group_sku_flag"].ToString() == "I")
                {
                    string itemCode = itemDict["item_code"].ToString();
                    itemDict["available_units"] = GetItemUnits(itemCode);
                }
                else
                {
                    itemDict["available_units"] = new List<dynamic>();
                }
            }
            
            return items;
        }


        //SELECT item_code,
        //                        mu_code,
        //                        serial_no,
        //                        company_code,
        //                        deleted_flag
        //                    FROM IP_ITEM_UNIT_SETUP
        //                                    UNION ALL
        private List<dynamic> GetItemUnits(string itemCode)
        {
            string query = @"
                SELECT mu_code, serial_no 
                FROM (                                
                                            SELECT item_code,
                                index_mu_code mu_code,
                                0 serial_no,
                                company_code,
                                deleted_flag
                            FROM ip_item_master_setup
                ) 
                WHERE company_code = '01' 
                AND item_code = :itemCode 
                AND deleted_flag = 'N'
                ORDER BY serial_no";

            var units = new List<dynamic>();
            
            using (var command = _objectEntity.Database.Connection.CreateCommand())
            {
                command.CommandText = query;
                
                var param = command.CreateParameter();
                param.ParameterName = "itemCode";
                param.Value = itemCode;
                command.Parameters.Add(param);
                
                if (_objectEntity.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    _objectEntity.Database.Connection.Open();
                }
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var unit = new Dictionary<string, object>
                        {
                            { "mu_code", reader["mu_code"] },
                            { "serial_no", Convert.ToInt32(reader["serial_no"]) }
                        };
                        units.Add(unit);
                    }
                }
            }
            
            return units;
        }

        public dynamic SaveFreeQuantity(List<FreeQuantityItem> data, string customerCode, string formCode, string createdBy)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"SaveFreeQuantity called with {data?.Count ?? 0} items");
                System.Diagnostics.Debug.WriteLine($"Customer: {customerCode}, Form: {formCode}, CreatedBy: {createdBy}");
                
                if (data == null || data.Count == 0)
                {
                    return new { success = false, message = "No data provided" };
                }
                
                // Don't manually open connection - let EF handle it with the transaction
                System.Diagnostics.Debug.WriteLine("Starting transaction");
                
                // Start transaction
                using (var transaction = _objectEntity.Database.BeginTransaction())
                {
                    try
                    {
                        // Delete existing records for this customer and form
                        var deleteQuery = @"
                            DELETE FROM FREE_QUANTITY 
                            WHERE CUSTOMER_CODE = :customerCode 
                            AND FORM_CODE = :formCode
                            AND COMPANY_CODE = :companyCode";

                        using (var deleteCmd = _objectEntity.Database.Connection.CreateCommand())
                        {
                            deleteCmd.CommandText = deleteQuery;
                            
                            var paramCustomer = deleteCmd.CreateParameter();
                            paramCustomer.ParameterName = "customerCode";
                            paramCustomer.Value = customerCode;
                            deleteCmd.Parameters.Add(paramCustomer);
                            
                            var paramForm = deleteCmd.CreateParameter();
                            paramForm.ParameterName = "formCode";
                            paramForm.Value = formCode;
                            deleteCmd.Parameters.Add(paramForm);
                            
                            var paramCompany = deleteCmd.CreateParameter();
                            paramCompany.ParameterName = "companyCode";
                            paramCompany.Value = "01";
                            deleteCmd.Parameters.Add(paramCompany);
                            
                            deleteCmd.ExecuteNonQuery();
                            System.Diagnostics.Debug.WriteLine("Existing records deleted");
                        }

                        // Get next FREE_QTY_NO
                        var seqQuery = "SELECT NVL(MAX(FREE_QTY_NO), 0) + 1 as NEXT_NO FROM FREE_QUANTITY";
                        int nextNo = 1;
                        
                        using (var seqCmd = _objectEntity.Database.Connection.CreateCommand())
                        {
                            seqCmd.CommandText = seqQuery;
                            var result = seqCmd.ExecuteScalar();
                            nextNo = Convert.ToInt32(result);
                            System.Diagnostics.Debug.WriteLine($"Next sequence number: {nextNo}");
                        }

                        // Build INSERT ALL query with string concatenation
                        var insertAllQuery = new System.Text.StringBuilder();
                        insertAllQuery.AppendLine("INSERT ALL");
                        
                        int recordCount = 0;
                        foreach (var item in data)
                        {
                            if (!string.IsNullOrEmpty(item.item_code))
                            {
                                // Escape single quotes in strings
                                string itemCode = item.item_code.Replace("'", "''");
                                string unit = (item.unit ?? "").Replace("'", "''");
                                string freeUnit = (item.free_unit ?? item.unit ?? "").Replace("'", "''");
                                string createdByEscaped = createdBy.Replace("'", "''");
                                
                                insertAllQuery.AppendLine(string.Format(
                                    "INTO FREE_QUANTITY (FREE_QTY_NO, EFFECTIVE_DATE, ITEM_CODE, QTY, MAIN_UNIT, FREE_QTY, FREE_UNIT, " +
                                    "MAIN_SERIAL, FREE_SERIAL, CUSTOMER_CODE, FORM_CODE, COMPANY_CODE, CREATED_BY, CREATED_DATE, DELETED_FLAG) " +
                                    "VALUES ({0}, SYSDATE, '{1}', {2}, '{3}', {4}, '{5}', {6}, {7}, '{8}', '{9}', '01', '{10}', SYSDATE, 'N')",
                                    nextNo,                    // FREE_QTY_NO (number)
                                    itemCode,                  // ITEM_CODE (varchar - quoted)
                                    item.qty,                  // QTY (number)
                                    unit,                      // MAIN_UNIT (varchar - quoted)
                                    item.free_qty,             // FREE_QTY (number)
                                    freeUnit,                  // FREE_UNIT (varchar - quoted)
                                    item.main_serial,          // MAIN_SERIAL (number)
                                    item.free_serial,          // FREE_SERIAL (number)
                                    customerCode,              // CUSTOMER_CODE (varchar - quoted)
                                    formCode,                  // FORM_CODE (varchar - quoted)
                                    createdByEscaped));        // CREATED_BY (varchar - quoted)
                                
                                nextNo++;
                                recordCount++;
                            }
                        }
                        
                        if (recordCount > 0)
                        {
                            insertAllQuery.AppendLine("SELECT 1 FROM DUAL");
                            
                            using (var insertCmd = _objectEntity.Database.Connection.CreateCommand())
                            {
                                insertCmd.CommandText = insertAllQuery.ToString();
                                
                                try
                                {
                                    var rowsAffected = insertCmd.ExecuteNonQuery();
                                    System.Diagnostics.Debug.WriteLine($"Inserted {recordCount} items, rows affected: {rowsAffected}");
                                }
                                catch (Exception insertEx)
                                {
                                    System.Diagnostics.Debug.WriteLine($"Error in INSERT ALL: {insertEx.Message}");
                                    System.Diagnostics.Debug.WriteLine($"Query: {insertAllQuery.ToString()}");
                                    throw;
                                }
                            }
                        }
                        else
                        {
                            return new { success = false, message = "No valid items to insert" };
                        }

                        transaction.Commit();
                        System.Diagnostics.Debug.WriteLine("Transaction committed successfully");
                        return new { success = true, message = "Free quantity saved successfully" };
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        System.Diagnostics.Debug.WriteLine($"Transaction rolled back: {ex.Message}");
                        throw new Exception("Error saving free quantity: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Outer exception: {ex.Message}");
                return new { success = false, message = ex.Message };
            }
        }

        public List<dynamic> LoadFreeQuantityData(string customerCode, string formCode)
        {
            string query = @"
                SELECT ITEM_CODE, QTY, MAIN_UNIT, FREE_QTY, FREE_UNIT, MAIN_SERIAL, FREE_SERIAL
                FROM FREE_QUANTITY
                WHERE CUSTOMER_CODE = :customerCode 
                AND FORM_CODE = :formCode
                AND COMPANY_CODE = :companyCode
                AND DELETED_FLAG = 'N'";

            var freeQtyData = new List<dynamic>();
            
            using (var command = _objectEntity.Database.Connection.CreateCommand())
            {
                command.CommandText = query;
                
                var paramCustomer = command.CreateParameter();
                paramCustomer.ParameterName = "customerCode";
                paramCustomer.Value = customerCode;
                command.Parameters.Add(paramCustomer);
                
                var paramForm = command.CreateParameter();
                paramForm.ParameterName = "formCode";
                paramForm.Value = formCode;
                command.Parameters.Add(paramForm);
                
                var paramCompany = command.CreateParameter();
                paramCompany.ParameterName = "companyCode";
                paramCompany.Value = "01";
                command.Parameters.Add(paramCompany);
                
                _objectEntity.Database.Connection.Open();
                
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var data = new Dictionary<string, object>
                        {
                            { "item_code", reader["ITEM_CODE"] },
                            { "qty", reader["QTY"] },
                            { "main_unit", reader["MAIN_UNIT"] },
                            { "free_qty", reader["FREE_QTY"] },
                            { "free_unit", reader["FREE_UNIT"] },
                            { "main_serial", reader["MAIN_SERIAL"] },
                            { "free_serial", reader["FREE_SERIAL"] }
                        };
                        freeQtyData.Add(data);
                    }
                }
                
                _objectEntity.Database.Connection.Close();
            }
            
            return freeQtyData;
        }

        private void AddParameter(System.Data.IDbCommand command, string name, object value)
        {
            var param = command.CreateParameter();
            param.ParameterName = name;
            param.Value = value ?? DBNull.Value;
            command.Parameters.Add(param);
        }


        public dynamic ProcessExcelUpload(Stream fileStream, string createdBy)
        {
            try
            {
                var errors = new List<string>();
                var successCount = 0;
                var errorCount = 0;

                using (var package = new ExcelPackage(fileStream))
                {
                    ExcelWorksheet worksheet = null;

                    try
                    {
                        // Check if workbook has any worksheets
                        if (package.Workbook == null)
                        {
                            return new
                            {
                                success = false,
                                message = "Invalid Excel file - Workbook is null",
                                errors = new List<string> { "The Excel file could not be read properly" },
                                successCount = 0,
                                errorCount = 0
                            };
                        }

                        if (package.Workbook.Worksheets == null || package.Workbook.Worksheets.Count == 0)
                        {
                            return new
                            {
                                success = false,
                                message = "Excel file has no worksheets",
                                errors = new List<string> { "The uploaded Excel file contains no worksheets" },
                                successCount = 0,
                                errorCount = 0
                            };
                        }

                        // Try to get the first worksheet - try by name first, then by index
                        worksheet = package.Workbook.Worksheets["Sheet1"] ?? package.Workbook.Worksheets.FirstOrDefault();
                        
                        if (worksheet == null)
                        {
                            return new
                            {
                                success = false,
                                message = "Could not access worksheet",
                                errors = new List<string> { $"Available worksheets: {string.Join(", ", package.Workbook.Worksheets.Select(w => w.Name))}" },
                                successCount = 0,
                                errorCount = 0
                            };
                        }
                    }
                    catch (Exception wsEx)
                    {
                        return new
                        {
                            success = false,
                            message = "Error accessing worksheet",
                            errors = new List<string> { $"Worksheet access error: {wsEx.Message}" },
                            successCount = 0,
                            errorCount = 0
                        };
                    }
                    
                    // Check if worksheet has data
                    if (worksheet.Dimension == null)
                    {
                        return new
                        {
                            success = false,
                            message = "Worksheet is empty",
                            errors = new List<string> { "The first worksheet in the Excel file has no data" },
                            successCount = 0,
                            errorCount = 0
                        };
                    }

                    int rowCount = worksheet.Dimension.Rows;

                    // Expected columns: CUSTOMER_CODE, ITEM_CODE, QTY, FREE_QTY, FORM_CODE
                    
                    using (var transaction = _objectEntity.Database.BeginTransaction())
                    {
                        try
                        {
                            // Get next FREE_QTY_NO
                            var seqQuery = "SELECT NVL(MAX(FREE_QTY_NO), 0) + 1 as NEXT_NO FROM FREE_QUANTITY";
                            int nextNo = 1;
                            
                            using (var seqCmd = _objectEntity.Database.Connection.CreateCommand())
                            {
                                seqCmd.CommandText = seqQuery;
                                var result = seqCmd.ExecuteScalar();
                                nextNo = Convert.ToInt32(result);
                            }

                            for (int row = 2; row <= rowCount; row++) // Start from row 2 (skip header)
                            {
                                try
                                {
                                    var customerCode = worksheet.Cells[row, 1].Value?.ToString()?.Trim();
                                    var itemCode = worksheet.Cells[row, 2].Value?.ToString()?.Trim();
                                    var qtyStr = worksheet.Cells[row, 3].Value?.ToString()?.Trim();
                                    var freeQtyStr = worksheet.Cells[row, 4].Value?.ToString()?.Trim();
                                    var formCode = worksheet.Cells[row, 5].Value?.ToString()?.Trim();

                                    if (string.IsNullOrEmpty(customerCode) || string.IsNullOrEmpty(itemCode))
                                    {
                                        continue; // Skip empty rows
                                    }

                                    // Parse quantities
                                    if (!decimal.TryParse(qtyStr, out decimal qty))
                                    {
                                        errors.Add($"Row {row}: Invalid QTY value '{qtyStr}'");
                                        errorCount++;
                                        continue;
                                    }

                                    if (!decimal.TryParse(freeQtyStr, out decimal freeQty))
                                    {
                                        errors.Add($"Row {row}: Invalid FREE_QTY value '{freeQtyStr}'");
                                        errorCount++;
                                        continue;
                                    }

                                    // Validate Customer Code or Name and get actual CUSTOMER_CODE
                                    var customerCheckQuery = @"
                                        SELECT CUSTOMER_CODE 
                                        FROM SA_CUSTOMER_SETUP 
                                        WHERE COMPANY_CODE = '01' 
                                        AND GROUP_SKU_FLAG = 'I'
                                        AND (CUSTOMER_CODE = :customerInput OR UPPER(CUSTOMER_EDESC) = UPPER(:customerInput))
                                        AND ROWNUM = 1";
                                    
                                    string actualCustomerCode = null;
                                    using (var custCmd = _objectEntity.Database.Connection.CreateCommand())
                                    {
                                        custCmd.CommandText = customerCheckQuery;
                                        AddParameter(custCmd, "customerInput", customerCode);
                                        var custResult = custCmd.ExecuteScalar();
                                        
                                        if (custResult != null && custResult != DBNull.Value)
                                        {
                                            actualCustomerCode = custResult.ToString();
                                        }
                                    }

                                    if (string.IsNullOrEmpty(actualCustomerCode))
                                    {
                                        errors.Add($"Row {row}: Customer '{customerCode}' does not exist in system");
                                        errorCount++;
                                        continue;
                                    }
                                    
                                    // Use the actual customer code from database
                                    customerCode = actualCustomerCode;

                                    // Validate Item Code or Name and get actual ITEM_CODE and INDEX_MU_CODE
                                    var itemCheckQuery = @"
                                        SELECT ITEM_CODE, INDEX_MU_CODE 
                                        FROM IP_ITEM_MASTER_SETUP 
                                        WHERE COMPANY_CODE = '01' 
                                        AND GROUP_SKU_FLAG = 'I'
                                        AND (ITEM_CODE = :itemInput OR UPPER(ITEM_EDESC) = UPPER(:itemInput))
                                        AND ROWNUM = 1";
                                    
                                    string actualItemCode = null;
                                    string indexMuCode = null;
                                    using (var itemCmd = _objectEntity.Database.Connection.CreateCommand())
                                    {
                                        itemCmd.CommandText = itemCheckQuery;
                                        AddParameter(itemCmd, "itemInput", itemCode);
                                        
                                        using (var reader = itemCmd.ExecuteReader())
                                        {
                                            if (reader.Read())
                                            {
                                                actualItemCode = reader["ITEM_CODE"]?.ToString();
                                                indexMuCode = reader["INDEX_MU_CODE"]?.ToString();
                                            }
                                        }
                                    }

                                    if (string.IsNullOrEmpty(actualItemCode) || string.IsNullOrEmpty(indexMuCode))
                                    {
                                        errors.Add($"Row {row}: Item '{itemCode}' does not exist in system");
                                        errorCount++;
                                        continue;
                                    }
                                    
                                    // Use the actual item code from database
                                    itemCode = actualItemCode;

                                    // Insert into FREE_QUANTITY
                                    var insertQuery = @"
                                        INSERT INTO FREE_QUANTITY 
                                        (FREE_QTY_NO, EFFECTIVE_DATE, ITEM_CODE, QTY, MAIN_UNIT, 
                                         FREE_QTY, FREE_UNIT, CUSTOMER_CODE, FORM_CODE, CREATED_BY, 
                                         CREATED_DATE, DELETED_FLAG, COMPANY_CODE, MAIN_SERIAL, FREE_SERIAL, EXCEL_ROWNUM)
                                        VALUES 
                                        (:freeQtyNo, SYSDATE, :itemCode, :qty, :mainUnit, 
                                         :freeQty, :freeUnit, :customerCode, :formCode, :createdBy, 
                                         SYSDATE, 'N', '01', 0, 0, :excelRowNum)";

                                    using (var insertCmd = _objectEntity.Database.Connection.CreateCommand())
                                    {
                                        insertCmd.CommandText = insertQuery;
                                        AddParameter(insertCmd, "freeQtyNo", nextNo);
                                        AddParameter(insertCmd, "itemCode", itemCode);
                                        AddParameter(insertCmd, "qty", qty);
                                        AddParameter(insertCmd, "mainUnit", indexMuCode);
                                        AddParameter(insertCmd, "freeQty", freeQty);
                                        AddParameter(insertCmd, "freeUnit", indexMuCode);
                                        AddParameter(insertCmd, "customerCode", customerCode);
                                        AddParameter(insertCmd, "formCode", formCode);
                                        AddParameter(insertCmd, "createdBy", createdBy);
                                        AddParameter(insertCmd, "excelRowNum", row);
                                        
                                        insertCmd.ExecuteNonQuery();
                                        nextNo++;
                                        successCount++;
                                    }
                                }
                                catch (Exception rowEx)
                                {
                                    errors.Add($"Row {row}: {rowEx.Message}");
                                    errorCount++;
                                }
                            }

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new Exception("Error processing Excel file: " + ex.Message);
                        }
                    }
                }

                // Return result based on success/error counts
                if (errorCount > 0 && successCount == 0)
                {
                    return new 
                    { 
                        success = false, 
                        message = $"Upload failed. {errorCount} error(s) found.",
                        errors = errors,
                        successCount = 0,
                        errorCount = errorCount
                    };
                }
                else if (errorCount > 0)
                {
                    return new 
                    { 
                        success = true, 
                        message = $"Upload completed with warnings. {successCount} record(s) uploaded, {errorCount} error(s).",
                        errors = errors,
                        successCount = successCount,
                        errorCount = errorCount
                    };
                }
                else
                {
                    return new 
                    { 
                        success = true, 
                        message = $"Upload completed successfully. {successCount} record(s) uploaded.",
                        errors = new List<string>(),
                        successCount = successCount,
                        errorCount = 0
                    };
                }
            }
            catch (Exception ex)
            {
                return new 
                { 
                    success = false, 
                    message = "Error processing Excel file: " + ex.Message,
                    errors = new List<string> { ex.Message },
                    successCount = 0,
                    errorCount = 0
                };
            }
        }
    }
}
