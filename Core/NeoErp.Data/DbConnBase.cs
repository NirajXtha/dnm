using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Xml.Linq;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Data.Entity.Core.EntityClient;


namespace NeoErp.Data
{
    
    public class DbConnBase:IDisposable
    {
        #region Private Variables
        private SqlConnection con;
        private SqlCommand com;
        #endregion

        public string getConStrSQL()
        {
            ConCredential conInfo = ConnectionManager.GetConInfo();
            string connectionString = new SqlConnectionStringBuilder
            {
                InitialCatalog = conInfo.Database,
                DataSource = conInfo.Server,
                IntegratedSecurity = false,
                UserID = conInfo.UserName,
                Password = conInfo.Password
            }.ConnectionString;
            return connectionString;
        }

        public  string ConnStr
        {
            get {
                return getConStrSQL(); // System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connString"].ToString();
            }
        }

     


        public DbConnBase()
        {
            string connstr = ConnStr; // System.Web.Configuration.WebConfigurationManager.ConnectionStrings["connStr"].ToString();
            con = new SqlConnection(connstr);
            com = new SqlCommand();
            com.Connection = con;
        }

        public void Dispose()
        {
            CloseConnection();
            GC.SuppressFinalize(this);
        }
        

        //Opens Database Connection
        private void OpenConnection()
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //Closes Database Connection
        private void CloseConnection()
        {
            try
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable ExecuteDataTable(string sCommand)
        {
            return ExecuteDataset(sCommand).Tables[0];
        }

        //Returns DataSet
        public DataSet ExecuteDataset(string sCommand)
        {
            return ExecuteDataset(sCommand, CommandType.Text);
        }

        public DataSet ExecuteDataset(string sCommand, CommandType comType)
        {
            DataSet ds = new DataSet();
            try
            {
                com.CommandText = sCommand;
                com.CommandType = comType;
                com.CommandTimeout = 0;
                OpenConnection();
                SqlDataAdapter adap = new SqlDataAdapter(com);
                adap.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally 
           {
               // Dispose();
            }
        }

        /// <summary>
        /// Query Executing Method for Insert, Update And Delete Operation
        /// </summary>
        /// <param name="sCommand">SqlQuery Or StoredProcedure</param>
        /// <returns>Returns no of rows Affected</returns>
        public int ExecuteNonQuery(string sCommand)
        {
            return ExecuteNonQuery(sCommand, CommandType.Text);
        }

        public int ExecuteNonQuery(string sCommand, CommandType comType)
        {
            com.CommandText = sCommand;
            com.CommandType = comType;
            com.CommandTimeout = 0;
            int resRetn = 0;
            OpenConnection();
            try
            {
                resRetn = com.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
               // Dispose();
            }
            return resRetn;
        }

        //Execute Scalar Method
        public object ExecuteScalar(string sCommand)
        {
          return  ExecuteScalar(sCommand, CommandType.Text);
        }
        public object ExecuteScalar(string sCommand, CommandType comType)
        {
            com.CommandText = sCommand;
            com.CommandType = comType;
            com.CommandTimeout = 0;
            object objReturn = null;
            OpenConnection();
            try
            {
                objReturn = com.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
               // Dispose();
            }
            return objReturn;
        }

        //Clears Parameters attached with command
        public void ClearParameters()
        {
            com.Parameters.Clear();
        }

        //Returns Value of o.p Parameter
        public object GetParameter(string strName)
        {
            return (com.Parameters[strName].Value);
        }

        public void AddParameter(string strParamName, SqlDbType objType, object objValue)
        {
            AddParameter(strParamName, objType, ParameterDirection.Input, objValue, 0);
        }


        public void AddParameter(string strParamName, SqlDbType objType,ParameterDirection objDirection, object objValue,int paramSize)
        {
            SqlParameter param = new SqlParameter();
            param.ParameterName = strParamName;
            param.SqlDbType = objType;
            param.Direction = objDirection;
            param.Size = paramSize;
            com.Parameters.Add(param);
            if (com.Parameters[strParamName].SqlDbType == SqlDbType.UniqueIdentifier)
            {
                if (objValue.GetType() == typeof(System.String))
                {
                    objValue = new System.Guid(objValue.ToString());
                }
            }
            com.Parameters[strParamName].Value = objValue;
        }


    }
}
