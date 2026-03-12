using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NeoErp.Core.Models;

namespace NeoErp.Data
{
    public class NeoErpRepository<T>:INeoErpRepository<T> where T:class
    {
       
     //   public NeoCoreEntity context { get; set; }
        public T GetById(object Id)
        {           
            throw new NotImplementedException();
        }

        public void Create(T entity)
        {
            throw new NotImplementedException();
        }

        public void Update(T entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(T entity)
        {
            throw new NotImplementedException();
        }

      
        public T GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetAll()
        {
            throw new NotImplementedException();
        }
    }
}