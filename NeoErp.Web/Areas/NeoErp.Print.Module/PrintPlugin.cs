using NeoErp.Core.Models;
using NeoErp.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NeoErp.Print.Module
{
    public class PrintPlugin : BasePlugin
    {
        private NeoErpCoreEntity _objectEntity;
        
        public PrintPlugin(NeoErpCoreEntity objectEntity)
        {
            this._objectEntity = objectEntity;
        }

        public override void Install()
        {
            var rowaffected = CreateTables();
            if (rowaffected <= 0)
            {
                //DeleteTables();
                return;
            }
            base.Install();
        }

        public override void Uninstall()
        {
            //DeleteTables();
            base.Uninstall();
        }

        public int CreateTables()
        {
            try
            {
                int totalRows = 0;

                // Check if WEB_PRINT_PATTERN table exists
                string checkTableQuery = @"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'WEB_PRINT_PATTERN'";
                var tableExists = _objectEntity.SqlQuery<int>(checkTableQuery).FirstOrDefault();

                if (tableExists == 0)
                {
                    // Create WEB_PRINT_PATTERN table
                    string createPatternTable = @"
                        CREATE TABLE WEB_PRINT_PATTERN
                        (
                          PATTERN_ID    NUMBER,
                          PATTERN_NAME  VARCHAR2(200 BYTE),
                          FORM_CODE     VARCHAR2(50 BYTE),
                          COMPANY_CODE  VARCHAR2(50 BYTE),
                          CHARGE_EXIST  NUMBER,
                          AUTO          NUMBER,
                          SQL_QUERY     CLOB,
                          FILE_NAME     VARCHAR2(50 BYTE),
                          FORM_TYPE     VARCHAR2(50 BYTE),
                          ACTIVE        NUMBER(1),
                          MAIN_FIELD    VARCHAR2(50 BYTE),
                          ITEM_COUNT    NUMBER,
                          MENU_NO       VARCHAR2(100 BYTE)
                        )";
                    totalRows += _objectEntity.ExecuteSqlCommand(createPatternTable);

                    // Add primary key
                    string addPrimaryKey = @"ALTER TABLE WEB_PRINT_PATTERN ADD PRIMARY KEY (PATTERN_ID)";
                    _objectEntity.ExecuteSqlCommand(addPrimaryKey);
                }

                // Check if WEB_PRINT_PATTERN_COLUMN table exists
                checkTableQuery = @"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'WEB_PRINT_PATTERN_COLUMN'";
                tableExists = _objectEntity.SqlQuery<int>(checkTableQuery).FirstOrDefault();

                if (tableExists == 0)
                {
                    // Create WEB_PRINT_PATTERN_COLUMN table
                    string createColumnTable = @"
                        CREATE TABLE WEB_PRINT_PATTERN_COLUMN
                        (
                          PATTERN_ID         NUMBER,
                          MASTER_CHILD_FLAG  VARCHAR2(1 BYTE),
                          LABEL              VARCHAR2(200 BYTE),
                          FIELD              VARCHAR2(200 BYTE),
                          DEFAULT_VAL        VARCHAR2(500 BYTE),
                          WIDTH              NUMBER,
                          SN                 NUMBER(10)
                        )";
                    totalRows += _objectEntity.ExecuteSqlCommand(createColumnTable);
                }

                // Check if WEB_PRINT_PATTERN_FOOTER table exists
                checkTableQuery = @"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'WEB_PRINT_PATTERN_FOOTER'";
                tableExists = _objectEntity.SqlQuery<int>(checkTableQuery).FirstOrDefault();

                if (tableExists == 0)
                {
                    // Create WEB_PRINT_PATTERN_FOOTER table
                    string createFooterTable = @"
                        CREATE TABLE WEB_PRINT_PATTERN_FOOTER
                        (
                          PATTERN_ID   NUMBER,
                          LABEL        VARCHAR2(200 BYTE),
                          FIELD        VARCHAR2(200 BYTE),
                          DEFAULT_VAL  VARCHAR2(500 BYTE),
                          SN           NUMBER
                        )";
                    totalRows += _objectEntity.ExecuteSqlCommand(createFooterTable);
                }

                // Check if WEB_PRINT_PATTERN_HEAD table exists
                checkTableQuery = @"SELECT COUNT(*) FROM USER_TABLES WHERE TABLE_NAME = 'WEB_PRINT_PATTERN_HEAD'";
                tableExists = _objectEntity.SqlQuery<int>(checkTableQuery).FirstOrDefault();

                if (tableExists == 0)
                {
                    // Create WEB_PRINT_PATTERN_HEAD table
                    string createHeadTable = @"
                        CREATE TABLE WEB_PRINT_PATTERN_HEAD
                        (
                          PATTERN_ID   NUMBER,
                          LABEL        VARCHAR2(200 BYTE),
                          FIELD        VARCHAR2(200 BYTE),
                          DEFAULT_VAL  VARCHAR2(500 BYTE),
                          SN           NUMBER
                        )";
                    totalRows += _objectEntity.ExecuteSqlCommand(createHeadTable);
                }

                return totalRows > 0 ? 1 : 0;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public int DeleteTables()
        {
            try
            {
                int totalRows = 0;

                // Drop tables in reverse order
                string dropHead = @"DROP TABLE WEB_PRINT_PATTERN_HEAD";
                try { totalRows += _objectEntity.ExecuteSqlCommand(dropHead); } catch { }

                string dropFooter = @"DROP TABLE WEB_PRINT_PATTERN_FOOTER";
                try { totalRows += _objectEntity.ExecuteSqlCommand(dropFooter); } catch { }

                string dropColumn = @"DROP TABLE WEB_PRINT_PATTERN_COLUMN";
                try { totalRows += _objectEntity.ExecuteSqlCommand(dropColumn); } catch { }

                string dropPattern = @"DROP TABLE WEB_PRINT_PATTERN";
                try { totalRows += _objectEntity.ExecuteSqlCommand(dropPattern); } catch { }

                return totalRows;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}