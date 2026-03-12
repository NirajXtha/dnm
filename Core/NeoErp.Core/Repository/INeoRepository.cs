using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoErp.Data
{
    public interface INeoErpRepository<T> where T:class
    {
        IEnumerable<T> GetAll();
        T GetById(object Id);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
