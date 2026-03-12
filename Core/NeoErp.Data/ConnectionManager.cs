using Microsoft.Win32;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace NeoErp.Data
{
    internal class DatabaseConfig
    {
        public List<DatabaseInfo> databases { get; set; }
    }

    public class DatabaseInfoPub
    {
        public string dbUserName { get; set; }
        public string dbPassword { get; set; }
        public string service { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string name { get; set; }
        public string active { get; set; }
    }


    internal class DatabaseInfo
    {
        public string dbUserName { get; set; }
        public string dbPassword { get; set; }
        public string service { get; set; }
        public string host { get; set; }
        public int port { get; set; }
        public string name { get; set; }
        public string active { get; set; }
    }
    public static class ConnectionManager
    {
        private static readonly string _defaultDatabase = "NeoErpCoreEntity";
        private static string _currentSelectedDatabase;
        public static string CurrentSelectedDatabase
        {
            get
            {
                return _currentSelectedDatabase;
            }
            set
            {
                _currentSelectedDatabase = value;
            }
        }
        public static void SetSelectedDatabase(string databaseName)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
                throw new ArgumentException("Database name cannot be null or empty");
            CurrentSelectedDatabase = databaseName;
            if (HttpContext.Current?.Session != null)
            {
                HttpContext.Current.Session["SelectedDbKey"] = databaseName;
            }
        }

        public static DatabaseInfoPub GetDatabaseInfoFromJsonPub(string databaseName)
        {
            try
            {
                //string jsonPath = HttpContext.Current.Server.MapPath("~/App_Data/Json/DbInfoSettings.json");
                string jsonPath = HostingEnvironment.MapPath("~/App_Data/Json/DbInfoSettings.json");

                if (!System.IO.File.Exists(jsonPath))
                {
                    return null;
                }

                var json = System.IO.File.ReadAllText(jsonPath);
                var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);

                var dbInfo = config?.databases?.FirstOrDefault(db =>
                    db.name == databaseName &&
                    db.active?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true);
                DatabaseInfoPub databaseInfoPub = null;
                if (dbInfo != null)
                {
                    databaseInfoPub = new DatabaseInfoPub();
                    databaseInfoPub.port = dbInfo.port;
                    databaseInfoPub.dbUserName = dbInfo.dbUserName;
                    databaseInfoPub.dbPassword = dbInfo.dbPassword;

                }
                return databaseInfoPub;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static ConCredential GetConInfo()
        {
            SqlConnectionStringBuilder connBuilder = new SqlConnectionStringBuilder();
            connBuilder.ConnectionString = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ToString();
            ConCredential ConInfo = new ConCredential();
            ConInfo.Database = connBuilder.InitialCatalog;
            ConInfo.Server = connBuilder.DataSource;
            ConInfo.UserName = connBuilder.UserID;
            ConInfo.Password = connBuilder.Password;
            ConInfo.ConType = ConnectionType.SqlServer;
            return ConInfo;
        }
        private static string GetDecryptedPassword(string encryptedPassword)
        {
            string decryptedPassword = string.Empty;
            try
            {
                string defaultConnectionString = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ConnectionString;
                var entityBuilder = new EntityConnectionStringBuilder(defaultConnectionString);
                string providerConnectionString = entityBuilder.ProviderConnectionString;
                using (var connection = new OracleConnection(providerConnectionString))
                {
                    connection.Open();
                    using (var command = new OracleCommand($"select fn_decrypt_password('{encryptedPassword}') from dual", connection))
                    {
                        var result = command.ExecuteScalar();
                        decryptedPassword = result?.ToString() ?? string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to decrypt password: {ex.Message}", ex);
            }
            return decryptedPassword;
        }
        private static DatabaseInfo GetDatabaseInfoFromJson(string databaseName)
        {
            try
            {

                //string jsonPath = HttpContext.Current.Server.MapPath("~/App_Data/Json/DbInfoSettings.json");
                string jsonPath = HostingEnvironment.MapPath("~/App_Data/Json/DbInfoSettings.json");

                if (!System.IO.File.Exists(jsonPath))
                {
                    return null;
                }

                var json = System.IO.File.ReadAllText(jsonPath);
                var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);

                var dbInfo = config?.databases?.FirstOrDefault(db =>
                    db.name == databaseName &&
                    db.active?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true);
                return dbInfo;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        public static String BuildConnectionString(string modelName, string AssemblyName = "*", string selectedDatabaseName = "")
        {
            var database = ConfigurationManager.AppSettings["databasetype"].ToString();
            if (database == "oracle")
            {
                string databaseName;
                if (!string.IsNullOrWhiteSpace(selectedDatabaseName))
                {
                    CurrentSelectedDatabase = selectedDatabaseName;
                    databaseName = selectedDatabaseName;
                }
                else
                {
                    databaseName = "NeoErpCoreEntity";
                }
                if (databaseName != _defaultDatabase)
                {
                    try
                    {
                        var dbInfo = GetDatabaseInfoFromJson(databaseName);
                        if (dbInfo != null)
                        {
                            string user = dbInfo.dbUserName;
                            string encryptedPwd = dbInfo.dbPassword;
                            string host = dbInfo.host ?? ConfigurationManager.AppSettings["DefaultDatabaseHost"];
                            string service = dbInfo.service;

                            if (!string.IsNullOrWhiteSpace(user) && !string.IsNullOrWhiteSpace(encryptedPwd) &&
                                !string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(service))
                            {
                                string password = GetDecryptedPassword(encryptedPwd);
                                var customConnectionString = BuildEntityConnectionString(user, password, host, service);
                                return customConnectionString;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }

                var defaultConnectionString = ConfigurationManager.ConnectionStrings["NeoErpCoreEntity"].ConnectionString;
                return defaultConnectionString;
            }
            else
            {
                var esb1 = new EntityConnectionStringBuilder(ConfigurationManager.ConnectionStrings["NeoErpSqlEntities"].ToString());
                return esb1.ToString();
            }
        }

        public static ObjectContext GetDbContext(string dbName = "")
        {
            string databaseToUse = !string.IsNullOrWhiteSpace(dbName) ? dbName : CurrentSelectedDatabase;

            if (databaseToUse == _defaultDatabase)
            {
                var defaultConn = ConfigurationManager.ConnectionStrings[_defaultDatabase];
                if (defaultConn == null)
                    throw new Exception($"Default connection string '{_defaultDatabase}' not found in web.config");

                return new ObjectContext(new EntityConnection(defaultConn.ConnectionString));
            }
            var dbInfo = GetDatabaseInfoFromJson(databaseToUse);

            if (dbInfo == null)
                throw new Exception($"Database configuration not found in JSON for '{databaseToUse}'");
            string user = dbInfo.dbUserName;
            string encryptedPwd = dbInfo.dbPassword;
            string host = dbInfo.host ?? ConfigurationManager.AppSettings["DefaultDatabaseHost"];
            string service = dbInfo.service;

            if (string.IsNullOrWhiteSpace(user) || string.IsNullOrWhiteSpace(encryptedPwd) ||
                string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(service))
            {
                throw new Exception("Incomplete database information in JSON");
            }

            string password = GetDecryptedPassword(encryptedPwd);
            string entityConnStr = BuildEntityConnectionString(user, password, host, service);
            return new ObjectContext(new EntityConnection(entityConnStr));
        }
        public static string GetCurrentConnectionString()
        {
            var connectionString = BuildConnectionString("NeoErp.Core.Models", "NeoErp.Core.Models", CurrentSelectedDatabase);
            return connectionString;
        }

        public static string GetConnectionStringDetails()
        {
            try
            {
                var detailsBuilder = new System.Text.StringBuilder();
                detailsBuilder.AppendLine("Connection String Details:");
                detailsBuilder.AppendLine($"  Current Database: {CurrentSelectedDatabase}");
                detailsBuilder.AppendLine($"  Is Custom Database: {CurrentSelectedDatabase != _defaultDatabase}");
                detailsBuilder.AppendLine($"  Connection String: {GetCurrentConnectionString()}");

                if (CurrentSelectedDatabase != _defaultDatabase)
                {
                    var dbInfo = GetDatabaseInfoFromJson(CurrentSelectedDatabase);
                    if (dbInfo != null)
                    {
                        detailsBuilder.AppendLine($"  JSON Configuration:");
                        detailsBuilder.AppendLine($"    Database Name: {dbInfo.name}");
                        detailsBuilder.AppendLine($"    User: {dbInfo.dbUserName}");
                        detailsBuilder.AppendLine($"    Host: {dbInfo.host}");
                        detailsBuilder.AppendLine($"    Service: {dbInfo.service}");
                        detailsBuilder.AppendLine($"    Port: {dbInfo.port}");
                        detailsBuilder.AppendLine($"    Active: {dbInfo.active}");
                        detailsBuilder.AppendLine($"    Password: ***ENCRYPTED***");
                    }
                    else
                    {
                        detailsBuilder.AppendLine($"  Database Not Found in JSON: {CurrentSelectedDatabase}");
                    }
                }

                return detailsBuilder.ToString();
            }
            catch (Exception ex)
            {
                return $"Error getting connection string details: {ex.Message}";
            }
        }

        public static string BuildEntityConnectionString(string dbUser, string dbPassword, string host, string serviceName)
        {
            try
            {
                string providerConn =
                    $"DATA SOURCE=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={host})(PORT=1521))" +
                    $"(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME={serviceName})));USER ID={dbUser};PASSWORD={dbPassword};";
                var entityBuilder = new EntityConnectionStringBuilder
                {
                    Provider = "Oracle.ManagedDataAccess.Client",
                    ProviderConnectionString = providerConn,
                    Metadata = "res://*/NeoErpCommon.csdl|res://*/NeoErpCommon.ssdl|res://*/NeoErpCommon.msl"
                };

                var result = entityBuilder.ToString();
                return result;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public static List<Dictionary<string, string>> GetDatabasesFromJson()
        {
            var databases = new List<Dictionary<string, string>>();

            try
            {
                string jsonPath = HttpContext.Current.Server.MapPath("~/App_Data/Json/DbInfoSettings.json");

                if (!System.IO.File.Exists(jsonPath))
                {
                    return databases;
                }

                var json = System.IO.File.ReadAllText(jsonPath);
                var config = JsonConvert.DeserializeObject<DatabaseConfig>(json);

                foreach (var db in config?.databases?.Where(d => d.active?.Equals("Y", StringComparison.OrdinalIgnoreCase) == true))
                {
                    var dbInfo = new Dictionary<string, string>();
                    dbInfo["Name"] = db.name;
                    dbInfo["dbUserName"] = db.dbUserName;
                    dbInfo["dbPassword"] = db.dbPassword;
                    dbInfo["host"] = db.host;
                    dbInfo["service"] = db.service;
                    dbInfo["port"] = db.port.ToString();
                    dbInfo["active"] = db.active;
                    databases.Add(dbInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetDatabasesFromJson: Error reading JSON: {ex.Message}");
            }
            return databases;
        }
    }
}
