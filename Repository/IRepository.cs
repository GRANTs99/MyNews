using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNews.Repository
{
    public interface IRepository<T>
    {
        T Get(int id);
        void Add(T entity);
        void Remove(T entity);
        IEnumerable<T> GetAll();
        void Save();
        void Update(T item);
    }
}
