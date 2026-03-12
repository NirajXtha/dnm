using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using MongoDB.Driver;

namespace NeoErp.Core.MongoDBRepository
{
    internal static class Util<U>
    {
        private const string DefaultConnectionString = "MongoDbConnection";

        public static string  GetDefaultConnectionString()
        {
           //var _client = new MongoClient();
           //var _database = _client.GetDatabase("test");
            return ConfigurationManager.ConnectionStrings[DefaultConnectionString].ConnectionString;
        }

        private static IMongoDatabase GetDatabaseFromUrl(MongoUrl url)
        {
            var client = new MongoClient(url);
            var server = client.GetDatabase(url.DatabaseName);
            return server;
        }

        public static IMongoCollection<T> GetCollectionFromConnectionString<T>(string connectionString) where T:IEntity<U>
        {
            return Util<U>.GetCollectionFromConnectionString<T>(connectionString, GetCollectionName<T>());
        }

        public static IMongoCollection<T> GetCollectionFromConnectionString<T>(string connectionString, string collectionName)
         where T : IEntity<U>
        {
            return Util<U>.GetDatabaseFromUrl(new MongoUrl(connectionString))
                .GetCollection<T>(collectionName);
            
        }

        //public static  bool DropCollection(string connectionString, string collectionName)
        // where T : IEntity<U>
        //{
        //    return Util<U>.GetDatabaseFromUrl(new MongoUrl(connectionString))
        //        .GetCollection<T>(collectionName);

        //}
        public static IMongoCollection<T> GetCollectionFromUrl<T>(MongoUrl url) where T:IEntity<U>
        {
            return Util<U>.GetCollectionFromUrl<T>(url,GetCollectionName<T>());
        }

        public static IMongoCollection<T> GetCollectionFromUrl<T>(MongoUrl url,string collectionName)
        {
            return Util<U>.GetDatabaseFromUrl(url).GetCollection<T>(collectionName);
        }
        private static string GetCollectionName<T>() where T:IEntity<U>
        {
            string collectionName;
            if(typeof(T).BaseType.Equals(typeof(object)))
            {
                collectionName = GetCollectionNameFromInterface<T>();

            }
            else
            {
                collectionName = GetCollectionNameFromType(typeof(T));
            }
            if(string.IsNullOrEmpty(collectionName))
            {
                throw new ArgumentException("Collection name cannot be empty for this entity");
            }
            return collectionName;
        }

        private static string GetCollectionNameFromInterface<T>()
        {
            string collectionname;
            var att = Attribute.GetCustomAttribute(typeof(T), typeof(CollectionName));
            if(att!=null)
            {
                collectionname = ((CollectionName)att).Name;
            }
            else
            {
                collectionname = typeof(T).Name;
            }

            return collectionname;
        }

        /// <summary>
        /// Determines the collectionname from the specified type.
        /// </summary>
        /// <param name="entitytype">The type of the entity to get the collectionname from.</param>
        /// <returns>Returns the collectionname from the specified type.</returns>
        private static string GetCollectionNameFromType(Type entitytype)
        {
            string collectionname;

            // Check to see if the object (inherited from Entity) has a CollectionName attribute
            var att = Attribute.GetCustomAttribute(entitytype, typeof(CollectionName));
            if (att != null)
            {
                // It does! Return the value specified by the CollectionName attribute
                collectionname = ((CollectionName)att).Name;
            }
            else
            {
                if (typeof(Entity).IsAssignableFrom(entitytype))
                {
                    // No attribute found, get the basetype
                    while (!entitytype.BaseType.Equals(typeof(Entity)))
                    {
                        entitytype = entitytype.BaseType;
                    }
                }
                collectionname = entitytype.Name;
            }

            return collectionname;
        }
    }
}