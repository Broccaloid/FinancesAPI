using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancesApi.Models
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext context;

        public Repository(DbContext context)
        {
            this.context = context;
        }
        public void Add(T entity)
        {
            context.Set<T>().Add(entity);
        }

        public void AddRange(IEnumerable<T> entities)
        {
            context.Set<T>().AddRange(entities); ;
        }

        public IEnumerable<T> GetAll()
        {
            return context.Set<T>().ToList();
        }

        public T GetById(int? id)
        {
            return context.Set<T>().Find(id);
        }

        public void Remove(T entity)
        {
            context.Set<T>().Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            context.Set<T>().RemoveRange(entities);
        }
    }
}
