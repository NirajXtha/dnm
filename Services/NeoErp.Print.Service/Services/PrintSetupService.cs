using NeoErp.Core;
using NeoErp.Core.Models;
using NeoErp.Print.Service.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NeoErp.Print.Service.Services
{
    public class PrintSetupService : IPrintSetupService
    {
        private NeoErpCoreEntity _objectEntity;

        public PrintSetupService(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }


        public List<ModuleSetupModel> GetModuleList()
        {
            string query = @"SELECT MODULE_CODE, MODULE_EDESC 
                           FROM MODULE_SETUP 
                           ORDER BY MODULE_CODE";
            
            var moduleList = _objectEntity.SqlQuery<ModuleSetupModel>(query).ToList();
            return moduleList;
        }

        public List<DocumentReportSetupModels> GetReportGroups(string moduleCode, string companyCode)
        {
            string query = string.Format(@"SELECT FORM_CODE, FORM_EDESC, 
                                          MASTER_FORM_CODE, PRE_FORM_CODE, 
                                          GROUP_SKU_FLAG, MODULE_CODE
                                         FROM FORM_SETUP 
                                         WHERE MODULE_CODE = '{0}' 
                                         AND DELETED_FLAG = 'N' 
                                         AND COMPANY_CODE = '{1}' 
                                         AND GROUP_SKU_FLAG = 'G'
                                         ORDER BY MASTER_FORM_CODE", 
                                         moduleCode, companyCode);
            
            var reportGroups = _objectEntity.SqlQuery<DocumentReportSetupModels>(query).ToList();
            var reportGrouped = _objectEntity.SqlQuery<DocumentReportSetupModels>(query).ToList();
            return reportGroups;
        }

        public List<DocumentReportSetupModels> GetReportItems(string moduleCode, string companyCode, string masterFormCode)
        {
            string query = string.Format(@"SELECT FORM_CODE, FORM_EDESC, FORM_TYPE, 
                                          MASTER_FORM_CODE, PRE_FORM_CODE, 
                                          GROUP_SKU_FLAG, MODULE_CODE
                                         FROM FORM_SETUP 
                                         WHERE DELETED_FLAG = 'N' 
                                         AND COMPANY_CODE = '{0}' 
                                         AND GROUP_SKU_FLAG = 'I'
                                         AND PRE_FORM_CODE = '{1}'
                                         ORDER BY FORM_CODE", 
                                         companyCode, masterFormCode);
            
            System.Diagnostics.Debug.WriteLine($"GetReportItems Query: {query}");
            
            var reportItems = _objectEntity.SqlQuery<DocumentReportSetupModels>(query).ToList();
            
            System.Diagnostics.Debug.WriteLine($"Query returned {reportItems.Count} items");
            if (reportItems.Count > 0)
            {
                var first = reportItems[0];
                System.Diagnostics.Debug.WriteLine($"First item - FORM_CODE: {first.FORM_CODE}, FORM_EDESC: {first.FORM_EDESC}");
            }
            
            return reportItems;
        }

        

        public PatternConfigurationResponse GetAllPatterns(string formCode, string companyCode, int? patternId = null)
        {
            var response = new PatternConfigurationResponse();

            // 1. Get all patterns for this form
            string patternListQuery = string.Format(@"SELECT PATTERN_ID, PATTERN_NAME 
                                                     FROM WEB_PRINT_PATTERN 
                                                     WHERE FORM_CODE = '{0}' 
                                                     AND COMPANY_CODE = '{1}'
                                                     ORDER BY PATTERN_ID", 
                                                     formCode, companyCode);
            
            response.Patterns = _objectEntity.SqlQuery<PatternListModel>(patternListQuery).ToList();

            // If no pattern ID specified, use first pattern
            if (!patternId.HasValue && response.Patterns.Count > 0)
            {
                patternId = response.Patterns[0].PATTERN_ID;
            }

            // If we have a pattern ID, get its details
            if (patternId.HasValue)
            {
                // 2. Get pattern detail
                string patternDetailQuery = string.Format(@"SELECT PATTERN_ID, PATTERN_NAME, FORM_CODE, COMPANY_CODE, 
                                                           CHARGE_EXIST, AUTO, SQL_QUERY, FILE_NAME, FORM_TYPE, ACTIVE, MAIN_FIELD, ITEM_COUNT, MENU_NO
                                                           FROM WEB_PRINT_PATTERN 
                                                           WHERE FORM_CODE = '{0}' 
                                                           AND COMPANY_CODE = '{1}' 
                                                           AND PATTERN_ID = {2} ", 
                                                           formCode, companyCode, patternId.Value);
                
                response.PatternDetail = _objectEntity.SqlQuery<PatternDetailModel>(patternDetailQuery).FirstOrDefault();

                // 3. Get head fields
                string headFieldsQuery = string.Format(@"SELECT PATTERN_ID, SN, LABEL, FIELD, DEFAULT_VAL
                                                        FROM WEB_PRINT_PATTERN_HEAD 
                                                        WHERE PATTERN_ID = {0}
                                                        ORDER BY SN", 
                                                        patternId.Value);
                
                response.HeadFields = _objectEntity.SqlQuery<PatternHeadFieldModel>(headFieldsQuery).ToList();

                // 3a. Get footer fields
                string footerFieldsQuery = string.Format(@"SELECT PATTERN_ID, SN, LABEL, FIELD, DEFAULT_VAL
                                                        FROM WEB_PRINT_PATTERN_FOOTER 
                                                        WHERE PATTERN_ID = {0}
                                                        ORDER BY SN", 
                                                        patternId.Value);
                
                response.FooterFields = _objectEntity.SqlQuery<PatternFooterFieldModel>(footerFieldsQuery).ToList();

                // 4. Get column fields
                string columnFieldsQuery = string.Format(@"SELECT PATTERN_ID, SN, MASTER_CHILD_FLAG, LABEL, FIELD, 
                                                          DEFAULT_VAL, WIDTH
                                                          FROM WEB_PRINT_PATTERN_COLUMN 
                                                          WHERE PATTERN_ID = {0}
                                                          ORDER BY SN, MASTER_CHILD_FLAG, PATTERN_ID", 
                                                          patternId.Value);
                
                response.ColumnFields = _objectEntity.SqlQuery<PatternColumnFieldModel>(columnFieldsQuery).ToList();
            }

            return response;
        }

        public int AddPattern(string patternName, string formCode, string companyCode)
        {
            try
            {
                // Get next pattern ID from sequence
                string getNextIdQuery = "SELECT NVL(MAX(PATTERN_ID), 0) + 1 FROM WEB_PRINT_PATTERN";
                int nextPatternId = _objectEntity.SqlQuery<int>(getNextIdQuery).FirstOrDefault();

                // Insert new pattern
        string insertQuery = string.Format(@"INSERT INTO WEB_PRINT_PATTERN 
                                            (PATTERN_ID, PATTERN_NAME, FORM_CODE, COMPANY_CODE, CHARGE_EXIST, AUTO, SQL_QUERY, FILE_NAME, FORM_TYPE, ACTIVE, MAIN_FIELD)
                                            VALUES ({0}, '{1}', '{2}', '{3}', 0, 0, '', '', '', 1, '')",
                                            nextPatternId,
                                            patternName.Replace("'", "''"), // Escape single quotes
                                            formCode,
                                            companyCode);

                int result = _objectEntity.ExecuteSqlCommand(insertQuery);

                if (result > 0)
                {
                    return nextPatternId;
                }

                return 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error adding pattern: " + ex.Message);
            }
        }

        public bool SavePattern(PatternDetailModel patternDetail, List<PatternHeadFieldModel> headFields, List<PatternFooterFieldModel> footerFields, List<PatternColumnFieldModel> columnFields)
        {
            try
            {
                // Update pattern details in WEB_PRINT_PATTERN
                //string updatePatternQuery = string.Format(@"UPDATE WEB_PRINT_PATTERN 
                //                                           SET PATTERN_NAME = '{0}',
                //                                               CHARGE_EXIST = {1},
                //                                               SQL_QUERY = '{2}',
                //                                               FILE_NAME = '{3}',
                //                                               FORM_TYPE = '{4}',
                //                                               ACTIVE = {5},
                //                                               MAIN_FIELD = '{6}',
                //                                               ITEM_COUNT = {7},
                //                                               MENU_NO = '{8}'
                //                                           WHERE PATTERN_ID = {9}
                //                                           AND FORM_CODE = '{10}'
                //                                           AND COMPANY_CODE = '{11}'",
                //                                           patternDetail.PATTERN_NAME.Replace("'", "''"),
                //                                           patternDetail.CHARGE_EXIST ?? 0,
                //                                           (patternDetail.SQL_QUERY ?? "").Replace("'", "''"),
                //                                           (patternDetail.FILE_NAME ?? "").Replace("'", "''"),
                //                                           (patternDetail.FORM_TYPE ?? "").Replace("'", "''"),
                //                                           patternDetail.ACTIVE ?? 1,
                //                                           (patternDetail.MAIN_FIELD ?? "").Replace("'", "''"),
                //                                           patternDetail.ITEM_COUNT ?? 0,
                //                                           (patternDetail.MENU_NO ?? "").Replace("'", "''"),
                //                                           patternDetail.PATTERN_ID,
                //                                           patternDetail.FORM_CODE,
                //                                           patternDetail.COMPANY_CODE);

                string updatePatternQuery = string.Format(@"UPDATE WEB_PRINT_PATTERN 
                                           SET PATTERN_NAME = '{0}',
                                               CHARGE_EXIST = {1},
                                               SQL_QUERY = '{2}',
                                               FILE_NAME = '{3}',
                                               FORM_TYPE = '{4}',
                                               ACTIVE = {5},
                                               MAIN_FIELD = '{6}',
                                               ITEM_COUNT = {7}
                                           WHERE PATTERN_ID = {8}
                                           AND FORM_CODE = '{9}'
                                           AND COMPANY_CODE = '{10}'",
                                           patternDetail.PATTERN_NAME.Replace("'", "''"),
                                           patternDetail.CHARGE_EXIST ?? 0,
                                           (patternDetail.SQL_QUERY ?? "").Replace("'", "''"),
                                           (patternDetail.FILE_NAME ?? "").Replace("'", "''"),
                                           (patternDetail.FORM_TYPE ?? "").Replace("'", "''"),
                                           patternDetail.ACTIVE ?? 1,
                                           (patternDetail.MAIN_FIELD ?? "").Replace("'", "''"),
                                           patternDetail.ITEM_COUNT ?? 0,
                                           patternDetail.PATTERN_ID,
                                           patternDetail.FORM_CODE,
                                           patternDetail.COMPANY_CODE);


                _objectEntity.ExecuteSqlCommand(updatePatternQuery);

                // Delete existing head fields
                string deleteHeadQuery = string.Format("DELETE FROM WEB_PRINT_PATTERN_HEAD WHERE PATTERN_ID = {0}", patternDetail.PATTERN_ID);
                _objectEntity.ExecuteSqlCommand(deleteHeadQuery);

                // Insert new head fields
                if (headFields != null && headFields.Count > 0)
                {
                    int sn = 1;
                    foreach (var field in headFields)
                    {
                        if (field == null) continue;
                        
                        string insertHeadQuery = string.Format(@"INSERT INTO WEB_PRINT_PATTERN_HEAD 
                                                                (PATTERN_ID, SN, LABEL, FIELD, DEFAULT_VAL)
                                                                VALUES ({0}, {1}, '{2}', '{3}', '{4}')",
                                                                patternDetail.PATTERN_ID,
                                                                sn,
                                                                (field.LABEL ?? "").Replace("'", "''"),
                                                                (field.FIELD ?? "").Replace("'", "''"),
                                                                (field.DEFAULT_VAL ?? "").Replace("'", "''"));
                        
                        _objectEntity.ExecuteSqlCommand(insertHeadQuery);
                        sn++;
                    }
                }

                // Delete existing footer fields
                string deleteFooterQuery = string.Format("DELETE FROM WEB_PRINT_PATTERN_FOOTER WHERE PATTERN_ID = {0}", patternDetail.PATTERN_ID);
                _objectEntity.ExecuteSqlCommand(deleteFooterQuery);

                // Insert new footer fields
                System.Diagnostics.Debug.WriteLine($"SavePattern - Footer Fields Count: {footerFields?.Count ?? 0}");
                if (footerFields != null && footerFields.Count > 0)
                {
                    int sn = 1;
                    foreach (var field in footerFields)
                    {
                        if (field == null) continue;
                        
                        System.Diagnostics.Debug.WriteLine($"Inserting footer field - Label: {field.LABEL}, Field: {field.FIELD}, Default: {field.DEFAULT_VAL}");
                        
                        string insertFooterQuery = string.Format(@"INSERT INTO WEB_PRINT_PATTERN_FOOTER 
                                                                (PATTERN_ID, SN, LABEL, FIELD, DEFAULT_VAL)
                                                                VALUES ({0}, {1}, '{2}', '{3}', '{4}')",
                                                                patternDetail.PATTERN_ID,
                                                                sn,
                                                                (field.LABEL ?? "").Replace("'", "''"),
                                                                (field.FIELD ?? "").Replace("'", "''"),
                                                                (field.DEFAULT_VAL ?? "").Replace("'", "''"));
                        
                        System.Diagnostics.Debug.WriteLine($"Footer Insert Query: {insertFooterQuery}");
                        _objectEntity.ExecuteSqlCommand(insertFooterQuery);
                        sn++;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SavePattern - No footer fields to insert");
                }

                // Delete existing column fields
                string deleteColumnQuery = string.Format("DELETE FROM WEB_PRINT_PATTERN_COLUMN WHERE PATTERN_ID = {0}", patternDetail.PATTERN_ID);
                _objectEntity.ExecuteSqlCommand(deleteColumnQuery);

                // Insert new column fields
                if (columnFields != null && columnFields.Count > 0)
                {
                    foreach (var field in columnFields)
                    {
                        if (field == null) continue;
                        
                        string insertColumnQuery = string.Format(@"INSERT INTO WEB_PRINT_PATTERN_COLUMN 
                                                                 (PATTERN_ID, SN, MASTER_CHILD_FLAG, LABEL, FIELD, DEFAULT_VAL, WIDTH)
                                                                 VALUES ({0}, {1}, '{2}', '{3}', '{4}', '{5}', {6})",
                                                                 patternDetail.PATTERN_ID,
                                                                 field.SN ?? 0,
                                                                 field.MASTER_CHILD_FLAG ?? "C",
                                                                 (field.LABEL ?? "").Replace("'", "''"),
                                                                 (field.FIELD ?? "").Replace("'", "''"),
                                                                 (field.DEFAULT_VAL ?? "").Replace("'", "''"),
                                                                 field.WIDTH ?? 0);
                        _objectEntity.ExecuteSqlCommand(insertColumnQuery);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving pattern: " + ex.Message);
            }
        }

        public PatternConfigurationResponse AutoFillPattern(string formCode, int patternId, string companyCode)
        {
            try
            {
                var response = new PatternConfigurationResponse();

                // Hardcoded head fields (not saved to database, just returned)
                response.HeadFields = new List<PatternHeadFieldModel>
                {
                    new PatternHeadFieldModel
                    {
                        PATTERN_ID = patternId,
                        LABEL = "Company_name",
                        FIELD = "COMPANY_CODE",
                        DEFAULT_VAL = "Xyz company"
                    },
                    new PatternHeadFieldModel
                    {
                        PATTERN_ID = patternId,
                        LABEL = "ADDRESS",
                        FIELD = "ADDRESS",
                        DEFAULT_VAL = "Dhapasi 5, Tokha, Kathmandu"
                    }
                };

                // Hardcoded footer fields (not saved to database, just returned)
                response.FooterFields = new List<PatternFooterFieldModel>
                {
                    new PatternFooterFieldModel
                    {
                        PATTERN_ID = patternId,
                        LABEL = "Amount In Words",
                        FIELD = "AMOUNT_IN_WORDS",
                        DEFAULT_VAL = "Thank you for your business"
                    },
                    new PatternFooterFieldModel
                    {
                        PATTERN_ID = patternId,
                        LABEL = "Terms",
                        FIELD = "TERMS_CONDITION",
                        DEFAULT_VAL = "Contact: +977-1-1234567"
                    }
                };

                // Query column fields from FORM_DETAIL_SETUP (not saved to database, just returned)
                string columnFieldsQuery = string.Format(@"SELECT SERIAL_NO as SN,
                                                          MASTER_CHILD_FLAG,
                                                          COLUMN_HEADER as LABEL,
                                                          COLUMN_NAME as FIELD,
                                                          DEFA_VALUE as DEFAULT_VAL,
                                                          30 as WIDTH
                                                          FROM FORM_DETAIL_SETUP 
                                                          WHERE FORM_CODE = '{0}' and company_code='01'",
                                                          formCode);

                response.ColumnFields = _objectEntity.SqlQuery<PatternColumnFieldModel>(columnFieldsQuery).ToList();

                // Fetch MAIN_FIELD from FORM_DETAIL_SETUP where SERIAL_NO = 1
                string mainFieldQuery = string.Format(@"SELECT COLUMN_NAME 
                                               FROM FORM_DETAIL_SETUP 
                                               WHERE SERIAL_NO = 1 
                                               AND COMPANY_CODE = '{0}' 
                                               AND FORM_CODE = '{1}'",
                                               companyCode, formCode);
                
                string mainField = _objectEntity.SqlQuery<string>(mainFieldQuery).FirstOrDefault() ?? "";

                // Fetch FORM_TYPE from FORM_SETUP
                string formTypeQuery = string.Format(@"SELECT FORM_TYPE 
                                              FROM FORM_SETUP 
                                              WHERE COMPANY_CODE = '{0}' 
                                              AND FORM_CODE = '{1}'",
                                              companyCode, formCode);
                
                string formType = _objectEntity.SqlQuery<string>(formTypeQuery).FirstOrDefault() ?? "";

                // Update the pattern with MAIN_FIELD and FORM_TYPE
                string updatePatternQuery = string.Format(@"UPDATE WEB_PRINT_PATTERN 
                                                   SET MAIN_FIELD = '{0}',
                                                       FORM_TYPE = '{1}'
                                                   WHERE PATTERN_ID = {2}
                                                   AND FORM_CODE = '{3}'
                                                   AND COMPANY_CODE = '{4}'",
                                                   mainField.Replace("'", "''"),
                                                   formType.Replace("'", "''"),
                                                   patternId,
                                                   formCode,
                                                   companyCode);

                _objectEntity.ExecuteSqlCommand(updatePatternQuery);

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error auto-filling pattern: " + ex.Message);
            }
        }

        public string GenerateSqlQuery(string formCode, int patternId, string companyCode)
        {
            try
            {
                // 1. Get table name from FORM_DETAIL_SETUP
                string tableNameQuery = string.Format(@"SELECT TABLE_NAME 
                                                       FROM FORM_DETAIL_SETUP 
                                                       WHERE FORM_CODE = '{0}' 
                                                       AND COMPANY_CODE = '{1}' 
                                                       AND ROWNUM = 1",
                                                       formCode, companyCode);
                
                string tableName = _objectEntity.SqlQuery<string>(tableNameQuery).FirstOrDefault();
                
                if (string.IsNullOrEmpty(tableName))
                {
                    throw new Exception("Table name not found for this form");
                }

                // 2. Get MAIN_FIELD for WHERE condition
                string mainFieldQuery = string.Format(@"SELECT MAIN_FIELD 
                                                       FROM WEB_PRINT_PATTERN 
                                                       WHERE PATTERN_ID = {0}",
                                                       patternId);
                
                string mainField = _objectEntity.SqlQuery<string>(mainFieldQuery).FirstOrDefault();

                // 3. Get head fields from WEB_PRINT_PATTERN_HEAD
                string headFieldsQuery = string.Format(@"SELECT FIELD 
                                                        FROM WEB_PRINT_PATTERN_HEAD 
                                                        WHERE PATTERN_ID = {0}",
                                                        patternId);
                
                var headFields = _objectEntity.SqlQuery<string>(headFieldsQuery).ToList();

                // 3a. Get footer fields from WEB_PRINT_PATTERN_FOOTER
                string footerFieldsQuery = string.Format(@"SELECT FIELD 
                                                          FROM WEB_PRINT_PATTERN_FOOTER 
                                                          WHERE PATTERN_ID = {0}
                                                          ORDER BY SN",
                                                          patternId);
                
                var footerFields = _objectEntity.SqlQuery<string>(footerFieldsQuery).ToList();
                
                // Filter out TERMS_CONDITION and AMOUNT_IN_WORDS from footer fields
                var filteredFooterFields = footerFields
                    .Where(f => !f.Equals("TERMS_CONDITION", StringComparison.OrdinalIgnoreCase) && 
                                !f.Equals("AMOUNT_IN_WORDS", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // 4. Get all column fields from WEB_PRINT_PATTERN_COLUMN
                string columnFieldsQuery = string.Format(@"SELECT FIELD 
                                                          FROM WEB_PRINT_PATTERN_COLUMN 
                                                          WHERE PATTERN_ID = {0} 
                                                          ORDER BY MASTER_CHILD_FLAG",
                                                          patternId);
                
                var columnFields = _objectEntity.SqlQuery<string>(columnFieldsQuery).ToList();

                // 5. Filter out columns containing 'MITI'
                var filteredColumns = columnFields.Where(col => !col.ToUpper().Contains("MITI")).ToList();

                if (filteredColumns.Count == 0 && headFields.Count == 0 && filteredFooterFields.Count == 0)
                {
                    throw new Exception("No valid columns, head fields, or footer fields found for this pattern");
                }

                // 6. Build SELECT and FROM clauses with joins
                var selectParts = new List<string>();
                var fromTables = new List<string>();
                var whereConditions = new List<string>();
                
                fromTables.Add(tableName.ToUpper() + " a");

                char tableAlias = 'b';
                
                // Process column fields
                foreach (var field in filteredColumns)
                {
                    string fieldUpper = field.ToUpper();
                    
                    // Skip 'd' if we have head fields (reserve 'd' for COMPANY_SETUP)
                    if (headFields.Count > 0 && tableAlias == 'd')
                    {
                        tableAlias++;
                    }
                    
                    // Check if this field needs a join
                    if (fieldUpper == "ITEM_CODE")
                    {
                        selectParts.Add(tableAlias + ".ITEM_EDESC ITEM_CODE");
                        fromTables.Add("IP_ITEM_MASTER_SETUP " + tableAlias);
                        whereConditions.Add("a.COMPANY_CODE = " + tableAlias + ".COMPANY_CODE(+)");
                        whereConditions.Add("a.ITEM_CODE = " + tableAlias + ".ITEM_CODE(+)");
                        tableAlias++;
                    }
                    else if (fieldUpper == "CUSTOMER_CODE")
                    {
                        selectParts.Add(tableAlias + ".CUSTOMER_EDESC CUSTOMER_CODE");
                        fromTables.Add("SA_CUSTOMER_SETUP " + tableAlias);
                        whereConditions.Add("a.COMPANY_CODE = " + tableAlias + ".COMPANY_CODE(+)");
                        whereConditions.Add("a.CUSTOMER_CODE = " + tableAlias + ".CUSTOMER_CODE(+)");
                        tableAlias++;
                    }
                    else if (fieldUpper == "ACC_CODE")
                    {
                        selectParts.Add(tableAlias + ".ACC_EDESC ACC_CODE");
                        fromTables.Add("FA_CHART_OF_ACCOUNTS_SETUP " + tableAlias);
                        whereConditions.Add("a.COMPANY_CODE = " + tableAlias + ".COMPANY_CODE(+)");
                        whereConditions.Add("a.ACC_CODE = " + tableAlias + ".ACC_CODE(+)");
                        tableAlias++;
                    }
                    else if (fieldUpper == "SUPPLIER_CODE")
                    {
                        selectParts.Add(tableAlias + ".SUPPLIER_EDESC SUPPLIER_CODE");
                        fromTables.Add("IP_SUPPLIER_SETUP " + tableAlias);
                        whereConditions.Add("a.COMPANY_CODE = " + tableAlias + ".COMPANY_CODE(+)");
                        whereConditions.Add("a.SUPPLIER_CODE = " + tableAlias + ".SUPPLIER_CODE(+)");
                        tableAlias++;
                    }
                    else if (fieldUpper == "EMPLOYEE_CODE")
                    {
                        selectParts.Add(tableAlias + ".EMPLOYEE_EDESC EMPLOYEE_CODE");
                        fromTables.Add("HR_EMPLOYEE_SETUP " + tableAlias);
                        whereConditions.Add("a.COMPANY_CODE = " + tableAlias + ".COMPANY_CODE(+)");
                        whereConditions.Add("a.EMPLOYEE_CODE = " + tableAlias + ".EMPLOYEE_CODE(+)");
                        tableAlias++;
                    }
                    else
                    {
                        selectParts.Add("a." + fieldUpper);
                    }
                }

                // Process head fields - all come from COMPANY_SETUP as 'd'
                if (headFields.Count > 0)
                {
                    // Add COMPANY_SETUP table as 'd' only once
                    if (!fromTables.Any(t => t.Contains("COMPANY_SETUP")))
                    {
                        fromTables.Add("COMPANY_SETUP d");
                        whereConditions.Add("a.COMPANY_CODE = d.COMPANY_CODE(+)");
                    }
                    
                    // Add all head fields with 'd.' prefix
                    foreach (var field in headFields)
                    {
                        string fieldUpper = field.ToUpper();
                        
                        // Special handling for COMPANY_CODE - select both EDESC and CODE like CUSTOMER_CODE pattern
                        if (fieldUpper == "COMPANY_CODE")
                        {
                            selectParts.Add("d.COMPANY_EDESC COMPANY_CODE");
                        }
                        else
                        {
                            selectParts.Add("d." + fieldUpper);
                        }
                    }
                }

                // Process footer fields - simple columns from table 'a' (no joins needed)
                if (filteredFooterFields.Count > 0)
                {
                    foreach (var field in filteredFooterFields)
                    {
                        string fieldUpper = field.ToUpper();
                        selectParts.Add("a." + fieldUpper);
                    }
                }

                // 7. Add WHERE conditions for company and main field
                whereConditions.Add("a.COMPANY_CODE = '" + companyCode + "'");

                if (!string.IsNullOrEmpty(formCode))
                {
                    whereConditions.Add("a.form_CODE = '" + formCode + "'");                    
                }

                //if (!string.IsNullOrEmpty(mainField))
                //{
                //    whereConditions.Add("a." + mainField.ToUpper() + " = ''");
                //}

                // 8. Build the final query
                string sqlQuery = "SELECT " + string.Join(",\n       ", selectParts) + 
                                  "\nFROM " + string.Join(",\n     ", fromTables);
                
                if (whereConditions.Count > 0)
                {
                    sqlQuery += "\nWHERE " + string.Join("\n  AND ", whereConditions);
                }

                // Return the query without validation - let user see and fix any errors
                return sqlQuery;
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating SQL query: " + ex.Message);
            }
        }

        public PreviewDataResponse GetPreviewData(string formCode, string mainFieldValue, string companyCode, int activePatternId, string filters = "")
        {
            try
            {
                var response = new PreviewDataResponse();

                int patternId =0;

                // 1. Get active pattern ID


                if (activePatternId != 0)
                {
                    patternId = activePatternId;                    
                }
                else {
                    string patternIdQuery = string.Format(@"SELECT PATTERN_ID 
                                                       FROM WEB_PRINT_PATTERN 
                                                       WHERE ACTIVE = 1 
                                                       AND COMPANY_CODE = '{0}' 
                                                       AND FORM_CODE = '{1}'
                                                       AND ROWNUM = 1",
                                                       companyCode, formCode);

                    var patternIdResult = _objectEntity.SqlQuery<int?>(patternIdQuery).FirstOrDefault();

                    if (patternIdResult == null || patternIdResult == 0)
                    {
                        throw new Exception("No active pattern found for this form");
                    }

                    patternId = patternIdResult.Value;
                }


                





                // 2. Get pattern details (FILE_NAME and SQL_QUERY)
                string patternQuery = string.Format(@"SELECT FILE_NAME, SQL_QUERY, FORM_TYPE,ITEM_COUNT,MAIN_FIELD
                                                     FROM WEB_PRINT_PATTERN 
                                                     WHERE PATTERN_ID = {0}", patternId);
                
                var patternDataList = _objectEntity.SqlQuery<PatternDetailModel>(patternQuery).FirstOrDefault();
                
                if (patternDataList == null)
                {
                    throw new Exception("Pattern not found");
                }

                response.FileName = patternDataList.FILE_NAME ?? "Invoice";
                response.FormType = patternDataList.FORM_TYPE;
                response.ITEM_COUNT = patternDataList.ITEM_COUNT;
                



                string sqlQuery = patternDataList.SQL_QUERY;

                // **NEW: Process conditional filter sections {PLACEHOLDER ...}**
                // Step 1: Extract all unique placeholders from the SQL query
                var allPlaceholders = new HashSet<string>();
                var placeholderRegex = new System.Text.RegularExpressions.Regex(@"\{([A-Z_]+)\s+[^}]+\}", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                foreach (System.Text.RegularExpressions.Match match in placeholderRegex.Matches(sqlQuery))
                {
                    allPlaceholders.Add(match.Groups[1].Value);
                }

                // Step 2: Parse selected filters
                var selectedFilters = new Dictionary<string, FilterValue>();
                if (!string.IsNullOrEmpty(filters))
                {
                    try
                    {
                        selectedFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, FilterValue>>(filters) ?? new Dictionary<string, FilterValue>();
                    }
                    catch
                    {
                        // If filter parsing fails, continue without filters
                    }
                }

                // Step 3: Process each placeholder
                foreach (var placeholder in allPlaceholders)
                {
                    string patternToFind = @"\{" + placeholder + @"\s+([^}]+)\}";
                    
                    if (selectedFilters.ContainsKey(placeholder))
                    {
                        // Filter IS selected - remove braces and replace #PLACEHOLDER# with actual values
                        var filterValue = selectedFilters[placeholder];
                        string replacement = "";
                        
                        if (filterValue.IsGroup && filterValue.Codes != null && filterValue.Codes.Count > 0)
                        {
                            // Group selection: use IN clause with multiple codes
                            var quotedCodes = filterValue.Codes.Select(c => "'" + c.Replace("'", "''") + "'");
                            replacement = "(" + string.Join(", ", quotedCodes) + ")";
                        }
                        else if (!string.IsNullOrEmpty(filterValue.Code))
                        {
                            // Individual selection: use single value
                            replacement = "'" + filterValue.Code.Replace("'", "''") + "'";
                        }
                        
                        // Replace all occurrences: remove braces and replace #PLACEHOLDER# with actual value
                        sqlQuery = System.Text.RegularExpressions.Regex.Replace(
                            sqlQuery,
                            patternToFind,
                            match => {
                                string content = match.Groups[1].Value;
                                // Replace #PLACEHOLDER# inside the content with actual values
                                return content.Replace("#" + placeholder + "#", replacement);
                            },
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );
                    }
                    else
                    {
                        // Filter NOT selected - remove entire {PLACEHOLDER ...} section(s)
                        sqlQuery = System.Text.RegularExpressions.Regex.Replace(
                            sqlQuery,
                            patternToFind,
                            "",
                            System.Text.RegularExpressions.RegexOptions.IgnoreCase
                        );
                    }
                }

                // 3. Get head fields
                string headFieldsQuery = string.Format(@"SELECT PATTERN_ID, SN, LABEL, FIELD, DEFAULT_VAL 
                                                        FROM WEB_PRINT_PATTERN_HEAD 
                                                        WHERE PATTERN_ID = {0}
                                                        ORDER BY SN", patternId);
                
                response.HeadFields = _objectEntity.SqlQuery<PatternHeadFieldModel>(headFieldsQuery).ToList();

                // 3a. Get footer fields
                string footerFieldsQuery = string.Format(@"SELECT PATTERN_ID, SN, LABEL, FIELD, DEFAULT_VAL 
                                                        FROM WEB_PRINT_PATTERN_FOOTER 
                                                        WHERE PATTERN_ID = {0}
                                                        ORDER BY SN", patternId);
                
                response.FooterFields = _objectEntity.SqlQuery<PatternFooterFieldModel>(footerFieldsQuery).ToList();

                // 3. Get column fields and separate Master/Child
                string columnFieldsQuery = string.Format(@"SELECT PATTERN_ID, SN, MASTER_CHILD_FLAG, LABEL, FIELD, DEFAULT_VAL, WIDTH
                                                          FROM WEB_PRINT_PATTERN_COLUMN 
                                                          WHERE PATTERN_ID = {0}
                                                          ORDER BY SN, MASTER_CHILD_FLAG", patternId);
                
                var allFields = _objectEntity.SqlQuery<PatternColumnFieldModel>(columnFieldsQuery).ToList();
                
                response.MasterFields = allFields.Where(f => f.MASTER_CHILD_FLAG == "M").ToList();
                response.ChildFields = allFields.Where(f => f.MASTER_CHILD_FLAG == "C").ToList();

                

                if (!string.IsNullOrEmpty(mainFieldValue)) {
                    
                    //sqlQuery += " AND a."+patternDataList.MAIN_FIELD+ " = '' ";

                    //sqlQuery = sqlQuery.Replace(" = ''", " like '%" + mainFieldValue.Replace("'", "''") + "%'");

                    sqlQuery += " AND a." + patternDataList.MAIN_FIELD + " like '%"+ mainFieldValue+"%' order by a.serial_no";

                    //sqlQuery = sqlQuery.Replace(" = ''", " like '%" + mainFieldValue.Replace("'", "''") + "%'");
                }

                // 4. Execute SQL query with main field value
                if (!string.IsNullOrEmpty(sqlQuery))
                {
                    
                    // Execute query using ADO.NET to properly read column names and values
                    response.QueryData = new List<Dictionary<string, object>>();

                    var connection = _objectEntity.Database.Connection;
                    var wasOpen = connection.State == System.Data.ConnectionState.Open;

                    try
                    {
                        if (!wasOpen)
                        {
                            connection.Open();
                        }

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = sqlQuery;

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var dict = new Dictionary<string, object>();

                                    // Get all column names and values
                                    for (int i = 0; i < reader.FieldCount; i++)
                                    {
                                        var columnName = reader.GetName(i).ToUpper();
                                        var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                        dict[columnName] = columnValue;
                                    }

                                    response.QueryData.Add(dict);
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (!wasOpen && connection.State == System.Data.ConnectionState.Open)
                        {
                            connection.Close();
                        }
                    }

                    // Post-process: Format date columns to dd-MMM-yyyy (e.g., 12-Jul-2025)
                    var keysToFormat = new List<string>();
                    foreach (var row in response.QueryData)
                    {
                        // Identify date keys only once per row (or if structure changes)
                        if (keysToFormat.Count == 0)
                        {
                            keysToFormat = row.Keys.Where(k => k.Contains("DATE")).ToList();
                        }
                        foreach (var dateKey in keysToFormat)
                        {
                            if (row[dateKey] != null)
                            {
                                try
                                {
                                    DateTime dateValue;

                                    // Try to parse the date value
                                    if (row[dateKey] is DateTime)
                                    {
                                        dateValue = (DateTime)row[dateKey];
                                    }
                                    else if (DateTime.TryParse(row[dateKey].ToString(), out dateValue))
                                    {
                                        // Successfully parsed
                                    }
                                    else
                                    {
                                        // Skip if not a valid date
                                        continue;
                                    }
                                    // Format as dd-MMM-yyyy (e.g., 12-Jul-2025)
                                    row[dateKey] = dateValue.ToString("dd-MMM-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                    // If formatting fails, keep original value
                                }
                            }
                        }
                    }
                }                
                else
                {
                    response.QueryData = new List<Dictionary<string, object>>();
                }

                // 5. Fetch charge data
                response.ChargeData = new List<Dictionary<string, object>>();
                
                if (!string.IsNullOrEmpty(mainFieldValue))
                {
                    string chargeQuery = string.Format(@"
                        SELECT * FROM 
                        (
                            SELECT b.CHARGE_EDESC, a.CHARGE_TYPE_FLAG, NVL(c.AMOUNT, 0) AS AMOUNT, a.PRIORITY_INDEX_NO ,a.CHARGE_CODE
                            FROM CHARGE_SETUP a, 
                                 IP_CHARGE_CODE b,
                                 (
                                     SELECT CHARGE_CODE, SUM(CHARGE_AMOUNT) AS AMOUNT  
                                     FROM CHARGE_TRANSACTION 
                                     WHERE REFERENCE_NO = '{0}' 
                                       AND COMPANY_CODE = '{1}' 
                                       AND FORM_CODE = '{2}'  
                                     GROUP BY CHARGE_CODE
                                 ) c 
                            WHERE a.COMPANY_CODE = '{1}' 
                              AND a.FORM_CODE = '{2}'  
                              AND a.DELETED_FLAG = 'N'  
                              AND a.COMPANY_CODE = b.COMPANY_CODE(+) 
                              AND a.CHARGE_CODE = b.CHARGE_CODE(+)  
                              AND a.CHARGE_CODE = c.CHARGE_CODE(+)
                            ORDER BY a.PRIORITY_INDEX_NO
                        ) WHERE 1= 1 ",
                        mainFieldValue.Replace("'", "''"),
                        companyCode,
                        formCode);
                    
                    try
                    {
                        var connection = _objectEntity.Database.Connection;
                        var wasOpen = connection.State == System.Data.ConnectionState.Open;
                        
                        try
                        {
                            if (!wasOpen)
                            {
                                connection.Open();
                            }
                            
                            using (var command = connection.CreateCommand())
                            {
                                command.CommandText = chargeQuery;
                                
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        var dict = new Dictionary<string, object>();
                                        
                                        for (int i = 0; i < reader.FieldCount; i++)
                                        {
                                            var columnName = reader.GetName(i).ToUpper();
                                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i);
                                            dict[columnName] = columnValue;
                                        }
                                        
                                        response.ChargeData.Add(dict);
                                    }
                                }
                            }
                        }
                        finally
                        {
                            if (!wasOpen && connection.State == System.Data.ConnectionState.Open)
                            {
                                connection.Close();
                            }
                        }
                    }
                    catch
                    {
                        // If charge query fails, keep empty charge data
                        response.ChargeData = new List<Dictionary<string, object>>();
                    }
                }

                // 6. Fetch sub-ledger data
                response.SubLedgerData = new List<SubLedgerGroup>();
                
                if (!string.IsNullOrEmpty(mainFieldValue))
                {
                    string subLedgerQuery = string.Format(@"
                        SELECT a.serial_no, b.sub_edesc,
                               CASE WHEN a.transaction_type='DR' THEN a.dr_amount
                                    WHEN a.transaction_type='CR' THEN a.cr_amount 
                               END as amount    
                        FROM FA_VOUCHER_SUB_DETAIL a, fa_sub_ledger_setup b 
                        WHERE a.sub_code = b.sub_code(+) 
                          AND a.company_code = b.company_code(+) 
                          AND a.company_code = '{0}' 
                          AND a.form_code = '{1}' 
                          AND a.voucher_no = '{2}'",
                        companyCode,
                        formCode,
                        mainFieldValue.Replace("'", "''"));
                    
                    try
                    {
                        var subLedgerConnection = _objectEntity.Database.Connection;
                        var subLedgerWasOpen = subLedgerConnection.State == System.Data.ConnectionState.Open;
                        
                        // Temporary dictionary to group by serial_no
                        var tempDict = new Dictionary<int, List<SubLedgerItem>>();
                        
                        try
                        {
                            if (!subLedgerWasOpen)
                            {
                                subLedgerConnection.Open();
                            }
                            
                            using (var command = subLedgerConnection.CreateCommand())
                            {
                                command.CommandText = subLedgerQuery;
                                
                                using (var reader = command.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int serialNo = reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0));
                                        string subEdesc = reader.IsDBNull(1) ? "" : reader.GetString(1);
                                        object amount = reader.IsDBNull(2) ? null : reader.GetValue(2);
                                        
                                        // Create entry for this serial_no if it doesn't exist
                                        if (!tempDict.ContainsKey(serialNo))
                                        {
                                            tempDict[serialNo] = new List<SubLedgerItem>();
                                        }
                                        
                                        // Add the sub-ledger item
                                        tempDict[serialNo].Add(new SubLedgerItem
                                        {
                                            sub_edesc = subEdesc,
                                            amount = amount
                                        });
                                    }
                                }
                            }                            
                            // Convert dictionary to list
                            foreach (var kvp in tempDict)
                            {
                                response.SubLedgerData.Add(new SubLedgerGroup
                                {
                                    SerialNo = kvp.Key,
                                    Items = kvp.Value
                                });
                            }
                        }
                        finally
                        {
                            if (!subLedgerWasOpen && subLedgerConnection.State == System.Data.ConnectionState.Open)
                            {
                                subLedgerConnection.Close();
                            }
                        }
                    }
                    catch
                    {
                        // If sub-ledger query fails, keep empty sub-ledger data
                        response.SubLedgerData = new List<SubLedgerGroup>();
                    }
                }

                // 7. Update and fetch print count from master_transaction
                if (!string.IsNullOrEmpty(mainFieldValue))
                {
                    try
                    {
                        // First check if this form should track print count
                        string checkPrintCountEligibilityQuery = string.Format(@"
                            SELECT DISTINCT FORM_CODE 
                            FROM FORM_DETAIL_SETUP 
                            WHERE COMPANY_CODE = '{0}' 
                              AND TABLE_NAME IN ('SA_SALES_INVOICE') 
                              AND FORM_CODE = '{1}'",
                            companyCode,
                            formCode);

                        var eligibleFormCode = _objectEntity.SqlQuery<string>(checkPrintCountEligibilityQuery).FirstOrDefault();

                        if (!string.IsNullOrEmpty(eligibleFormCode))
                        {
                            // Form is eligible for print count tracking
                            response.PRINT_COUNT_FLAG = true;

                            // Fetch current print count
                            string fetchPrintCountQuery = string.Format(@"
                                SELECT PRINT_COUNT 
                                FROM MASTER_TRANSACTION 
                                WHERE VOUCHER_NO LIKE '%{0}%' 
                                  AND FORM_CODE = '{1}' 
                                  AND COMPANY_CODE = '{2}' 
                                  AND ROWNUM = 1",
                                mainFieldValue.Replace("'", "''"),
                                formCode,
                                companyCode);

                            var currentPrintCount = _objectEntity.SqlQuery<int?>(fetchPrintCountQuery).FirstOrDefault();

                            int newPrintCount;
                            if (currentPrintCount == null || currentPrintCount == 0)
                            {
                                // First print - set to 1
                                newPrintCount = 1;
                            }
                            else
                            {
                                // Increment the count
                                newPrintCount = currentPrintCount.Value +1 ;
                            };

                            // Update the print count in database
                            //string updatePrintCountQuery = string.Format(@"
                            //    UPDATE MASTER_TRANSACTION 
                            //    SET PRINT_COUNT = {0} 
                            //    WHERE VOUCHER_NO LIKE '%{1}%' 
                            //      AND FORM_CODE = '{2}' 
                            //      AND COMPANY_CODE = '{3}'",
                            //    newPrintCount,
                            //    mainFieldValue.Replace("'", "''"),
                            //    formCode,
                            //    companyCode);

                            //_objectEntity.ExecuteSqlCommand(updatePrintCountQuery);

                            // Set the print count in response
                            response.PRINT_COUNT = newPrintCount;

                        }
                        else
                        {
                            // Form is not eligible for print count tracking
                            response.PRINT_COUNT_FLAG = false;
                            response.PRINT_COUNT = null;
                        }
                    }
                    catch
                    {
                        // If print count update fails, continue without it
                        response.PRINT_COUNT_FLAG = false;
                        response.PRINT_COUNT = null;
                    }
                }
                else
                {
                    // No main field value, no print count tracking
                    response.PRINT_COUNT_FLAG = false;
                    response.PRINT_COUNT = null;
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting preview data: " + ex.Message);
            }
        }

        public string GetPreviewFileName(string formCode, string companyCode, int activePatternId)
        {
            try
            {
                int patternId = 0;

                // 1. Get active pattern ID
                if (activePatternId != 0)
                {
                    patternId = activePatternId;
                }
                else
                {
                    string patternIdQuery = string.Format(@"SELECT PATTERN_ID 
                                                       FROM WEB_PRINT_PATTERN 
                                                       WHERE ACTIVE = 1 
                                                       AND COMPANY_CODE = '{0}' 
                                                       AND FORM_CODE = '{1}'
                                                       AND ROWNUM = 1",
                                                       companyCode, formCode);

                    var patternIdResult = _objectEntity.SqlQuery<int?>(patternIdQuery).FirstOrDefault();

                    if (patternIdResult == null || patternIdResult == 0)
                    {
                        throw new Exception("No active pattern found for this form");
                    }

                    patternId = patternIdResult.Value;
                }

                // 2. Get FILE_NAME from pattern
                string fileNameQuery = string.Format(@"SELECT FILE_NAME
                                                      FROM WEB_PRINT_PATTERN 
                                                      WHERE PATTERN_ID = {0}", patternId);

                var fileName = _objectEntity.SqlQuery<string>(fileNameQuery).FirstOrDefault();

                return fileName ?? "Invoice";
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting preview file name: " + ex.Message);
            }
        }

        // ===== MENU MANAGEMENT METHODS =====

        public List<MenuModuleModel> GetMenuModules()
        {
            string query = @"SELECT MODULE_CODE, MODULE_EDESC 
                           FROM WEB_MODULE_SETUP 
                           ORDER BY MODULE_CODE";
            
            var modules = _objectEntity.SqlQuery<MenuModuleModel>(query).ToList();
            return modules;
        }

        public List<MenuGroupModel> GetMenuGroups(string moduleCode)
        {
            string query = string.Format(@"SELECT menu_no, menu_edesc 
                                         FROM web_menu_management 
                                         WHERE module_code = '{0}' 
                                         AND group_sku_flag = 'G' 
                                         ORDER BY menu_no", 
                                         moduleCode);
            
            var groups = _objectEntity.SqlQuery<MenuGroupModel>(query).ToList();
            return groups;
        }

        public string GetNextMenuCode(string moduleCode, string groupMenuNo)
        {
            try
            {
                // Query to get max menu_no for the group
                string query = string.Format(@"SELECT MAX(menu_no) 
                                              FROM web_menu_management 
                                              WHERE module_code = '{0}' 
                                              AND menu_no LIKE '{1}%' 
                                              AND group_sku_flag = 'I'", 
                                              moduleCode, groupMenuNo);
                
                var maxMenuNo = _objectEntity.SqlQuery<string>(query).FirstOrDefault();
                
                if (string.IsNullOrEmpty(maxMenuNo))
                {
                    // No existing menus, return first code
                    return groupMenuNo + ".01";
                }
                
                // Parse and increment
                // Example: "02.11" -> split on '.' -> ["02", "11"] -> increment "11" to "12"
                var parts = maxMenuNo.Split('.');
                if (parts.Length == 2)
                {
                    int itemNumber;
                    if (int.TryParse(parts[1], out itemNumber))
                    {
                        itemNumber++;
                        return string.Format("{0}.{1:D2}", parts[0], itemNumber);
                    }
                }
                
                // Fallback: just append .01
                return groupMenuNo + ".01";
            }
            catch (Exception ex)
            {
                throw new Exception("Error generating next menu code: " + ex.Message);
            }
        }

        public bool SaveNewMenu(SaveMenuRequest request, string companyCode)
        {
            try
            {
                string existingMenuNo = null;
                
                // First check: Does this pattern already have a linked menu?
                if (!string.IsNullOrEmpty(request.PatternId))
                {
                    int patternId;
                    if (int.TryParse(request.PatternId, out patternId))
                    {
                        string checkQuery = string.Format(@"SELECT MENU_NO 
                                                           FROM WEB_PRINT_PATTERN 
                                                           WHERE PATTERN_ID = {0}", 
                                                           patternId);
                        
                        existingMenuNo = _objectEntity.SqlQuery<string>(checkQuery).FirstOrDefault();
                    }
                }
                
                string menuNo;
                int result;
                
                // Decide based on database state
                if (!string.IsNullOrEmpty(existingMenuNo))
                {
                    // === EDIT CASE: Pattern has menu_no - UPDATE existing menu ===
                    menuNo = existingMenuNo;
                    
                    string updateQuery = string.Format(@"UPDATE WEB_MENU_MANAGEMENT
                                                         SET MENU_EDESC = '{0}',
                                                             MENU_OBJECT_NAME = '{1}',
                                                             FULL_PATH = '{2}',
                                                             VIRTUAL_PATH = '{3}'
                                                         WHERE MENU_NO = '{4}'
                                                         AND COMPANY_CODE = '{5}'",
                                                         request.MenuEdesc.Replace("'", "''"),
                                                         request.MenuObjectName.Replace("'", "''"),
                                                         request.FullPath.Replace("'", "''"),
                                                         request.VirtualPath.Replace("'", "''"),
                                                         menuNo,
                                                         companyCode);
                    
                    result = _objectEntity.ExecuteSqlCommand(updateQuery);
                }
                else
                {
                    // === INSERT CASE: Pattern has no menu_no - CREATE new menu ===
                    
                    // Generate new menu code
                    menuNo = GetNextMenuCode(request.ModuleCode, request.GroupMenuNo);
                    
                    // Default values
                    string iconPath = "fa fa-bar-chart";
                    string groupSkuFlag = "I";
                    string createdBy = "01";
                    string moduleAbbr = "DB";
                    string color = "#808080";
                    
                    // Build INSERT query for WEB_MENU_MANAGEMENT
                    string insertQuery = string.Format(@"INSERT INTO WEB_MENU_MANAGEMENT
                       (MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, MODULE_CODE, FULL_PATH, 
                        VIRTUAL_PATH, ICON_PATH, GROUP_SKU_FLAG, PRE_MENU_NO, COMPANY_CODE, 
                        CREATED_BY, CREATED_DATE, MODULE_ABBR, COLOR)
                     VALUES
                       ('{0}', '{1}', '{2}', '{3}', '{4}', 
                        '{5}', '{6}', '{7}', '{8}', '{9}', 
                        '{10}', SYSDATE, '{11}', '{12}')",
                        menuNo,
                        request.MenuEdesc.Replace("'", "''"),
                        request.MenuObjectName.Replace("'", "''"),
                        request.ModuleCode,
                        request.FullPath.Replace("'", "''"),
                        request.VirtualPath.Replace("'", "''"),
                        iconPath,
                        groupSkuFlag,
                        request.GroupMenuNo,
                        companyCode,
                        createdBy,
                        moduleAbbr,
                        color);
                    
                    result = _objectEntity.ExecuteSqlCommand(insertQuery);
                    
                    // If menu inserted successfully, link it to the pattern
                    if (result > 0 && !string.IsNullOrEmpty(request.PatternId))
                    {
                        int patternId;
                        if (int.TryParse(request.PatternId, out patternId))
                        {
                            // Update WEB_PRINT_PATTERN with the new menu_no
                            string updatePatternQuery = string.Format(@"UPDATE WEB_PRINT_PATTERN 
                                                                       SET MENU_NO = '{0}' 
                                                                       WHERE PATTERN_ID = {1}",
                                                                       menuNo,
                                                                       patternId);
                            
                            _objectEntity.ExecuteSqlCommand(updatePatternQuery);
                        }
                    }
                }
                
                return result > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving menu: " + ex.Message);
            }
        }

        public MenuGroupModel GetMenuInfo(string menuNo)
        {
            try
            {
                if (string.IsNullOrEmpty(menuNo))
                {
                    return null;
                }

                string query = string.Format(@"SELECT MENU_NO, MENU_EDESC, MENU_OBJECT_NAME, 
                                              MODULE_CODE, FULL_PATH, VIRTUAL_PATH, PRE_MENU_NO
                                              FROM web_menu_management 
                                              WHERE menu_no = '{0}'", 
                                              menuNo);
                
                var menuInfo = _objectEntity.SqlQuery<MenuGroupModel>(query).FirstOrDefault();
                return menuInfo;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting menu info: " + ex.Message);
            }
        }

        // ===== DYNAMIC FILTER METHODS =====

        public List<QueryPlaceholder> ExtractPlaceholders(string sqlQuery)
        {
            var placeholders = new List<QueryPlaceholder>();
            
            if (string.IsNullOrEmpty(sqlQuery))
            {
                return placeholders;
            }

            // Regex to find placeholders like #CUSTOMER_CODE# or #customer_code#
            var regex = new System.Text.RegularExpressions.Regex(@"#(\w+)#", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var matches = regex.Matches(sqlQuery);

            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                string field = match.Groups[1].Value.ToUpper();
                
                // Map field to table and entity type
                string table = "";
                string entityType = "";

                if (field.Contains("CUSTOMER"))
                {
                    table = "SA_CUSTOMER_SETUP";
                    entityType = "customer";
                }
                else if (field.Contains("SUPPLIER"))
                {
                    table = "IP_SUPPLIER_SETUP";
                    entityType = "supplier";
                }
                else if (field.Contains("EMPLOYEE"))
                {
                    table = "HR_EMPLOYEE_SETUP";
                    entityType = "employee";
                }

                if (!string.IsNullOrEmpty(entityType))
                {
                    // Check if placeholder already added
                    if (!placeholders.Any(p => p.Field.Equals(field, StringComparison.OrdinalIgnoreCase)))
                    {
                        placeholders.Add(new QueryPlaceholder
                        {
                            Field = field,
                            Table = table,
                            EntityType = entityType
                        });
                    }
                }
            }

            return placeholders;
        }

        public EntitySearchResult SearchEntity(string entityType, string searchTerm, string companyCode, int pageNumber, int pageSize)
        {
            try
            {
                var result = new EntitySearchResult
                {
                    Data = new List<EntityItem>(),
                    TotalCount = 0,
                    HasMore = false
                };

                string tableName = GetTableName(entityType);
                string codeField = GetCodeField(entityType);
                string descField = GetDescField(entityType);
                string masterCodeField = GetMasterCodeField(entityType);

                // Calculate offset for pagination
                int offset = (pageNumber - 1) * pageSize;

                // Build search query
                string searchCondition = "";
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchCondition = string.Format(@" AND ({0} LIKE '%{1}%' OR {2} LIKE '%{3}%')",
                        codeField, searchTerm.Replace("'", "''"),
                        descField, searchTerm.Replace("'", "''"));
                }

                // Get total count
                string countQuery = string.Format(@"SELECT COUNT(*) 
                    FROM {0} 
                    WHERE COMPANY_CODE = '{1}'{2}",
                    tableName, companyCode, searchCondition);

                result.TotalCount = _objectEntity.SqlQuery<int>(countQuery).FirstOrDefault();

                // Get paginated data - order by groups first, then individuals
                // Oracle 11g compatible pagination using ROWNUM
                int startRow = offset + 1;
                int endRow = offset + pageSize;
                
                string dataQuery = string.Format(@"
                    SELECT * FROM (
                        SELECT CODE, DESCRIPTION, MASTER_CODE, GROUP_SKU_FLAG, ROWNUM rnum FROM (
                            SELECT {0} as CODE, {1} as DESCRIPTION, {2} as MASTER_CODE, GROUP_SKU_FLAG
                            FROM {3}
                            WHERE COMPANY_CODE = '{4}'{5}
                            ORDER BY CASE WHEN GROUP_SKU_FLAG = 'G' THEN 0 ELSE 1 END, {2}
                        )
                        WHERE ROWNUM <= {6}
                    )
                    WHERE rnum >= {7}",
                    codeField, descField, masterCodeField, tableName, companyCode, 
                    searchCondition, endRow, startRow);

                var connection = _objectEntity.Database.Connection;
                var wasOpen = connection.State == System.Data.ConnectionState.Open;

                try
                {
                    if (!wasOpen)
                    {
                        connection.Open();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = dataQuery;

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Data.Add(new EntityItem
                                {
                                    Code = reader["CODE"]?.ToString() ?? "",
                                    Description = reader["DESCRIPTION"]?.ToString() ?? "",
                                    MasterCode = reader["MASTER_CODE"]?.ToString() ?? "",
                                    IsGroup = reader["GROUP_SKU_FLAG"]?.ToString() == "G"
                                });
                            }
                        }
                    }
                }
                finally
                {
                    if (!wasOpen && connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }

                result.HasMore = (offset + pageSize) < result.TotalCount;

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("Error searching entity: " + ex.Message);
            }
        }

        public List<string> GetEntityChildren(string entityType, string masterCode, string companyCode)
        {
            try
            {
                string tableName = GetTableName(entityType);
                string codeField = GetCodeField(entityType);
                string masterCodeField = GetMasterCodeField(entityType);

                string query = string.Format(@"SELECT {0}
                    FROM {1}
                    WHERE GROUP_SKU_FLAG = 'I'
                      AND COMPANY_CODE = '{2}'
                      AND {3} LIKE '{4}%'
                    ORDER BY {3}",
                    codeField, tableName, companyCode, 
                    masterCodeField, masterCode.Replace("'", "''"));

                var codes = _objectEntity.SqlQuery<string>(query).ToList();
                return codes;
            }
            catch (Exception ex)
            {
                throw new Exception("Error getting entity children: " + ex.Message);
            }
        }

        // Helper methods for entity mapping
        private string GetTableName(string entityType)
        {
            switch (entityType.ToLower())
            {
                case "customer":
                    return "SA_CUSTOMER_SETUP";
                case "supplier":
                    return "IP_SUPPLIER_SETUP";
                case "employee":
                    return "HR_EMPLOYEE_SETUP";
                default:
                    throw new Exception("Unknown entity type: " + entityType);
            }
        }

        private string GetCodeField(string entityType)
        {
            switch (entityType.ToLower())
            {
                case "customer":
                    return "CUSTOMER_CODE";
                case "supplier":
                    return "SUPPLIER_CODE";
                case "employee":
                    return "EMPLOYEE_CODE";
                default:
                    throw new Exception("Unknown entity type: " + entityType);
            }
        }

        private string GetDescField(string entityType)
        {
            switch (entityType.ToLower())
            {
                case "customer":
                    return "CUSTOMER_EDESC";
                case "supplier":
                    return "SUPPLIER_EDESC";
                case "employee":
                    return "EMPLOYEE_EDESC";
                default:
                    throw new Exception("Unknown entity type: " + entityType);
            }
        }

        private string GetMasterCodeField(string entityType)
        {
            switch (entityType.ToLower())
            {
                case "customer":
                    return "MASTER_CUSTOMER_CODE";
                case "supplier":
                    return "MASTER_SUPPLIER_CODE";
                case "employee":
                    return "MASTER_EMPLOYEE_CODE";
                default:
                    throw new Exception("Unknown entity type: " + entityType);
            }
        }

        public string PrintCount(string formCode, string mainFieldValue, string companyCode)
        {
            try
            {
                // Fetch current print count
                string fetchPrintCountQuery = string.Format(@"
                    SELECT PRINT_COUNT 
                    FROM MASTER_TRANSACTION 
                    WHERE VOUCHER_NO LIKE '%{0}%' 
                      AND FORM_CODE = '{1}' 
                      AND COMPANY_CODE = '{2}' 
                      AND ROWNUM = 1",
                    mainFieldValue.Replace("'", "''"),
                    formCode,
                    companyCode);

                var currentPrintCount = _objectEntity.SqlQuery<int?>(fetchPrintCountQuery).FirstOrDefault();

                int newPrintCount;
                if (currentPrintCount == null || currentPrintCount == 0)
                {
                    // First print - set to 1
                    newPrintCount = 1;
                }
                else
                {
                    // Increment the count
                    newPrintCount = currentPrintCount.Value + 1;
                }

                // Update the print count in database
                string updatePrintCountQuery = string.Format(@"
                    UPDATE MASTER_TRANSACTION 
                    SET PRINT_COUNT = {0} 
                    WHERE VOUCHER_NO LIKE '%{1}%' 
                      AND FORM_CODE = '{2}' 
                      AND COMPANY_CODE = '{3}'",
                    newPrintCount,
                    mainFieldValue.Replace("'", "''"),
                    formCode,
                    companyCode);

                _objectEntity.ExecuteSqlCommand(updatePrintCountQuery);

                // Return the new print count as string
                return newPrintCount.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating print count: " + ex.Message);
            }
        }
    }
}
