﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HackerPlatform.Core.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        //Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null);

        Task AddAsync(T entity);
        void Remove(T entity);
        void Update(T entity);

        Task<T?> GetAsync(Expression<Func<T, bool>> predicate);
    }
}
