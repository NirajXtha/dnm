using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using NeoErp.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data;
using NeoErp.Core;

namespace NeoErp.Core.Models
{

    public partial class NeoErpCoreEntity : DbContext, IDbContext
    {
        private readonly string _selectedDatabaseName;
        //public NeoErpCoreEntity()
        //    : base(ConnectionManager.BuildConnectionString("NeoErpCoreEntity", "NeoErp.Core.Models"))
        //{
        //}

        public NeoErpCoreEntity()
            : base(GetDynamicConnectionString())
        {
            _selectedDatabaseName = ConnectionManager.CurrentSelectedDatabase;
            System.Diagnostics.Debug.WriteLine($"NeoErpCoreEntity constructor: Created with database: {_selectedDatabaseName}");
            System.Diagnostics.Debug.WriteLine($"NeoErpCoreEntity constructor: Connection string: {Database.Connection.ConnectionString}");
        }

        public NeoErpCoreEntity(string customerDbKey)
            : base(ConnectionManager.BuildConnectionString("NeoErp.Core.Models", "NeoErp.Core.Models", customerDbKey))
        {
            _selectedDatabaseName = customerDbKey;
            System.Diagnostics.Debug.WriteLine($"NeoErpCoreEntity constructor(string): Created with database: {_selectedDatabaseName}");
            System.Diagnostics.Debug.WriteLine($"NeoErpCoreEntity constructor(string): Connection string: {Database.Connection.ConnectionString}");
        }
        public string SelectedDatabaseName => _selectedDatabaseName;

        /// <summary>
        /// Creates a new NeoErpCoreEntity instance using the currently selected database from UI
        /// </summary>
        /// <returns>NeoErpCoreEntity instance connected to the selected database</returns>
        public static NeoErpCoreEntity CreateForSelectedDatabase()
        {
            string selectedDb = ConnectionManager.CurrentSelectedDatabase;
            return new NeoErpCoreEntity(selectedDb);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LINKAQDEntities"/> class.
        /// </summary>
        /// <param name="entityConnection">The entity connection.</param>
        public NeoErpCoreEntity(EntityConnection entityConnection) :
            base(entityConnection, false)
        {

        }

        private static string GetDynamicConnectionString()
        {
            string selectedDb = ConnectionManager.CurrentSelectedDatabase;
            System.Diagnostics.Debug.WriteLine($"GetDynamicConnectionString called. Selected DB: {selectedDb}");

            var connectionString = ConnectionManager.BuildConnectionString("NeoErp.Core.Models", "NeoErp.Core.Models", selectedDb);
            System.Diagnostics.Debug.WriteLine($"Built connection string for database: {selectedDb}");
            return connectionString;
        }
        private static string GetSelectedDatabaseName(string selectedDbFromUi = null)
        {
            if (!string.IsNullOrWhiteSpace(selectedDbFromUi))
            {
                return selectedDbFromUi;
            }
            return ConnectionManager.CurrentSelectedDatabase;
        }


        /// <summary>
        /// Attach an entity to the context or return an already attached entity (if it was already attached)
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <returns>Attached entity</returns>
        protected virtual TEntity AttachEntityToContext<TEntity>(TEntity entity) where TEntity : BaseEntity, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            var alreadyAttached = Set<TEntity>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (alreadyAttached == null)
            {
                //attach new entity
                Set<TEntity>().Attach(entity);
                return entity;
            }

            //entity is already loaded
            return alreadyAttached;
        }
        /// <summary>
        /// Create database script
        /// </summary>
        /// <returns>SQL to generate database</returns>
        public string CreateDatabaseScript()
        {
            return ((IObjectContextAdapter)this).ObjectContext.CreateDatabaseScript();
        }

        /// <summary>
        /// Get DbSet
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>DbSet</returns>
        public new IDbSet<TEntity> Set<TEntity>() where TEntity : BaseEntity
        {
            return base.Set<TEntity>();
        }

        /// <summary>
        /// Execute stores procedure and load a list of entities at the end
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="commandText">Command text</param>
        /// <param name="parameters">Parameters</param>
        /// <returns>Entities</returns>
        public IList<TEntity> ExecuteStoredProcedureList<TEntity>(string commandText, params object[] parameters) where TEntity : BaseEntity, new()
        {
            //add parameters to command
            if (parameters != null && parameters.Length > 0)
            {
                for (int i = 0; i <= parameters.Length - 1; i++)
                {
                    var p = parameters[i] as DbParameter;
                    if (p == null)
                        throw new Exception("Not support parameter type");

                    commandText += i == 0 ? " " : ", ";

                    commandText += "@" + p.ParameterName;
                    if (p.Direction == ParameterDirection.InputOutput || p.Direction == ParameterDirection.Output)
                    {
                        //output parameter
                        commandText += " output";
                    }
                }
            }

            var result = this.Database.SqlQuery<TEntity>(commandText, parameters).ToList();

            //performance hack applied as described here - http://www.nopcommerce.com/boards/t/25483/fix-very-important-speed-improvement.aspx
            bool acd = this.Configuration.AutoDetectChangesEnabled;
            try
            {
                this.Configuration.AutoDetectChangesEnabled = false;

                for (int i = 0; i < result.Count; i++)
                    result[i] = AttachEntityToContext(result[i]);
            }
            finally
            {
                this.Configuration.AutoDetectChangesEnabled = acd;
            }

            return result;
        }

        /// <summary>
        /// Creates a raw SQL query that will return elements of the given generic type.  The type can be any type that has properties that match the names of the columns returned from the query, or can be a simple primitive type. The type does not have to be an entity type. The results of this query are never tracked by the context even if the type of object returned is an entity type.
        /// </summary>
        /// <typeparam name="TElement">The type of object returned by the query.</typeparam>
        /// <param name="sql">The SQL query string.</param>
        /// <param name="parameters">The parameters to apply to the SQL query string.</param>
        /// <returns>Result</returns>
        public IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return this.Database.SqlQuery<TElement>(sql, parameters);
        }

        public DataTable SqlQuery(string sql, params object[] parameters)
        {
            DbConnection conn = this.Database.Connection;
            ConnectionState initialState = conn.State;
            var dataTable = new DataTable();
            try
            {
                if (initialState != ConnectionState.Open)
                    conn.Open();  // open connection if not already open
                using (DbCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = sql;
                    var datareader = cmd.ExecuteReader();
                    dataTable.Load(datareader);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (initialState != ConnectionState.Open)
                    conn.Close(); // only close connection if not initially open
            }

            return dataTable;
        }


        /// <summary>
        /// Executes the given DDL/DML command against the database.
        /// </summary>
        /// <param name="sql">The command string</param>
        /// <param name="doNotEnsureTransaction">false - the transaction creation is not ensured; true - the transaction creation is ensured.</param>
        /// <param name="timeout">Timeout value, in seconds. A null value indicates that the default value of the underlying provider will be used</param>
        /// <param name="parameters">The parameters to apply to the command string.</param>
        /// <returns>The result returned by the database after executing the command.</returns>
        public int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            try
            {
                int? previousTimeout = null;
                if (timeout.HasValue)
                {
                    //store previous timeout
                    previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                    ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
                }

                var transactionalBehavior = doNotEnsureTransaction
                    ? TransactionalBehavior.DoNotEnsureTransaction
                    : TransactionalBehavior.EnsureTransaction;
                var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

                if (timeout.HasValue)
                {
                    //Set previous timeout back
                    ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
                }

                //return result
                return result;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public class SqlExecuteResult
        {
            public bool Success { get; set; }
            public string Error { get; set; }
            public int RowsAffected { get; set; }
        }
        public SqlExecuteResult ExecuteSqlCommand2(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            var response = new SqlExecuteResult();

            try
            {
                int? previousTimeout = null;

                if (timeout.HasValue)
                {
                    previousTimeout = ((IObjectContextAdapter)this).ObjectContext.CommandTimeout;
                    ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = timeout;
                }

                var transactionalBehavior = doNotEnsureTransaction
                    ? TransactionalBehavior.DoNotEnsureTransaction
                    : TransactionalBehavior.EnsureTransaction;

                var result = this.Database.ExecuteSqlCommand(transactionalBehavior, sql, parameters);

                if (timeout.HasValue)
                {
                    ((IObjectContextAdapter)this).ObjectContext.CommandTimeout = previousTimeout;
                }

                response.Success = true;
                response.Error = null;
                response.RowsAffected = result;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Error = ex.Message;   // actual error message
                response.RowsAffected = 0;
            }

            return response;
        }





        /// <summary>
        /// Detach an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public void Detach(object entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }


        public void UpdateQuery<TEntity>(TEntity entity) where TEntity : BaseEntity
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            this.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Modified;


        }



        /// <summary>
        /// Gets or sets a value indicating whether proxy creation setting is enabled (used in EF)
        /// </summary>
        public virtual bool ProxyCreationEnabled
        {
            get
            {
                return this.Configuration.ProxyCreationEnabled;
            }
            set
            {
                this.Configuration.ProxyCreationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get
            {
                return this.Configuration.AutoDetectChangesEnabled;
            }
            set
            {
                this.Configuration.AutoDetectChangesEnabled = value;
            }
        }
    }

}