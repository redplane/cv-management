using System.Collections.Generic;
using System.Linq;
using DbEntity.Interfaces;
using DbEntity.Models.Entities.Context;
using Microsoft.EntityFrameworkCore;

namespace DbEntity.Services
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Properties

        /// <summary>
        ///     Database set.
        /// </summary>
        private readonly DbSet<T> _dbSet;
        
        #endregion

        #region Constructors

        /// <summary>
        ///     Initiate repository with database context wrapper.
        /// </summary>
        /// <param name="dbContext"></param>
        public Repository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<T>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Search all data from the specific table.
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> Search()
        {
            return _dbSet;
        }

        /// <summary>
        ///     Insert a record into data table.
        /// </summary>
        /// <param name="entity"></param>
        public T Insert(T entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        /// <summary>
        ///     Remove a list of entities from database.
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public void Remove(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }

        /// <summary>
        ///     Remove an entity from database.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        #endregion
    }
}