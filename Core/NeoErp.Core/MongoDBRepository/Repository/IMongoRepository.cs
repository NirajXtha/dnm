using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace NeoErp.Core.MongoDBRepository.Repository
{
    public interface IMongoRepository<T,TKey>:IQueryable<T> where T:IEntity<TKey>
    {
       IMongoCollection<T> collection { get; set; }
        T GetById(TKey id);
        T Add(T entity);
        T Add(IEnumerable<T> entities);
        void AddMany(IEnumerable<T> entities);
        T Update(T entity);
        T Update(IEnumerable<T> Entities);
        void Delete(TKey id);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        void DeleteAll();
        long Count();

        bool Exists(Expression<Func<T, bool>> predicate);
        IDisposable RequestStart();

        /// <summary>
        /// Lets the server know that this thread is done with a series of related operations.
        /// </summary>
        /// <remarks>
        /// Instead of calling this method it is better to put the return value of RequestStart in a using statement.
        /// </remarks>
        void RequestDone();
        void Drop();

    }
    public interface IMongoRepository<T> : IQueryable<T>, IMongoRepository<T, string>
       where T : IEntity<string>
    { }
}
