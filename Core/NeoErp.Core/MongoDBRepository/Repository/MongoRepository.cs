using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Linq.Expressions;

namespace NeoErp.Core.MongoDBRepository.Repository
{
    public class MongoRepository<T,TKey>: IMongoRepository<T,TKey> where T:IEntity<TKey>
    {
       public IMongoCollection<T> collection { get; set; }
        public MongoRepository():this(Util<TKey>.GetDefaultConnectionString())
        {

        }

        public MongoRepository(string connectionString)
        {
            this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString);

        }

        public MongoRepository(string connectionString,string collectionName)
        {
            this.collection = Util<TKey>.GetCollectionFromConnectionString<T>(connectionString,collectionName);
        }

        public MongoRepository(MongoUrl url)
        {
            this.collection = Util<TKey>.GetCollectionFromUrl<T>(url);
        }
        public MongoRepository(MongoUrl url,string collectionName)
        {
            this.collection = Util<TKey>.GetCollectionFromUrl<T>(url, collectionName);
        }

        public  IMongoCollection<T> Collection
        {
            get { return this.collection; }
        }

        public string CollectionName
        {
            get { return this.collection.CollectionNamespace.CollectionName; }
        }

        public virtual T GetById(TKey id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
            {
                return this.GetById(new ObjectId(id as string));
            }
         //   var query = Query<T>.EQ(e => e.Id, id);
          //  var speaker = this.Collection.<T>();
            //.GetCollection<Speaker>("speakers").FindOne(query);
            throw new NotImplementedException();
            //return this.collection.FindOneAndReplace<T>(a=> new ObjectId(a.Id as string)== new ObjectId(id as string));
        }
            public virtual T GetById(ObjectId id)
        {
            throw new NotImplementedException();
            //return this.collection.FindOneByIdAs<T>(id);
        }

        public virtual T Add(T entity)
        {
            this.collection.InsertOne(entity);

            return entity;
        }

        public virtual void AddMany(IEnumerable<T> entities)
        {
            this.collection.InsertMany(entities);
        }

        public virtual T Update(T entity)
        {
            throw new NotImplementedException();
        }

        public virtual void Update(IEnumerable<T> entities)
        {
            foreach (T entity in entities)
            {
                // this.collection.Save<T>(entity);
                throw new NotImplementedException();
            }
        }
        public virtual void Delete(TKey id)
        {
            if (typeof(T).IsSubclassOf(typeof(Entity)))
            {
               // this.collection.ReplaceOne(Query.EQ("_id", new ObjectId(id as string)));
            }
            else
            {
               // this.collection.Remove(Query.EQ("_id", BsonValue.Create(id)));
            }

        }
        public virtual void Delete(ObjectId id)
        {
            //this.collection.Remove(Query.EQ("_id", id));

        }
        public virtual void Delete(T entity)
        {
            this.Delete(entity.Id);
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            foreach (T entity in this.collection.AsQueryable<T>().Where(predicate))
            {
                this.Delete(entity.Id);
            }
        }
        public virtual void DeleteAll()
        {
          //  this.collection.RemoveAll();
        }
        public virtual long Count()
        {
            return this.collection.ToBsonDocument().Count();
        }
        public virtual bool Exists(Expression<Func<T, bool>> predicate)
        {
            return this.collection.AsQueryable<T>().Any(predicate);
        }

        public virtual IDisposable RequestStart()
        {
            //  return this.collection.Database.RequestStart();
            throw new NotImplementedException();
        }
        public virtual void RequestDone()
        {
           // this.collection.Database.RequestDone();
           throw new NotImplementedException();
        }
        public virtual IEnumerator<T> GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.AsQueryable<T>().GetEnumerator();
        }

        T IMongoRepository<T, TKey>.Add(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        T IMongoRepository<T, TKey>.Update(IEnumerable<T> Entities)
        {
            throw new NotImplementedException();
        }

        public virtual Type ElementType
        {
            get { return this.collection.AsQueryable<T>().ElementType; }
        }
        public virtual Expression Expression
        {
            get { return this.collection.AsQueryable<T>().Expression; }
        }
        public virtual IQueryProvider Provider
        {
            get { return this.collection.AsQueryable<T>().Provider; }
        }

        public virtual void Drop()
        {
            this.collection.Database.DropCollection(this.CollectionName);
        }


    }
    public class MongoRepository<T> : MongoRepository<T, string>, IMongoRepository<T>
      where T : IEntity<string>
    {
        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// Uses the Default App/Web.Config connectionstrings to fetch the connectionString and Database name.
        /// </summary>
        /// <remarks>Default constructor defaults to "MongoServerSettings" key for connectionstring.</remarks>
        public MongoRepository()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        public MongoRepository(MongoUrl url)
            : base(url) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="url">Url to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepository(MongoUrl url, string collectionName)
            : base(url, collectionName) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        public MongoRepository(string connectionString)
            : base(connectionString) { }

        /// <summary>
        /// Initializes a new instance of the MongoRepository class.
        /// </summary>
        /// <param name="connectionString">Connectionstring to use for connecting to MongoDB.</param>
        /// <param name="collectionName">The name of the collection to use.</param>
        public MongoRepository(string connectionString, string collectionName)
            : base(connectionString, collectionName) { }
    }
}