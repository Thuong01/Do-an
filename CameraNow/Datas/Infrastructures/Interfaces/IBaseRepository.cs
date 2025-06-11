
using Datas.ViewModels;
using Models.Models;
using System.Linq.Expressions;

namespace Datas.Infrastructures.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        /// <summary>
        /// Marks an entity as new
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Add(T entity);
        void AddRange(IEnumerable<T> entities);
        object AddQuery(string query, params object[] parameters);
        /// <summary>
        /// Marks an entity as modified
        /// </summary>
        /// <param name="entity"></param>
        T Update(T entity);
        /// <summary>
        /// Marks an entity to be removed
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        T Delete(T entity);
        T Delete(Guid id);
        Task<T> DeleteAsync(Guid id);
        /// <summary>
        /// Delete multi records
        /// </summary>
        /// <param name="where"></param>
        void DeleteMulti(Expression<Func<T, bool>> where);
        /// <summary>
        /// Get an entity by int id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetSingleById(Guid id, string[] includes = null);
        Task<T> GetSingleByIdAsync(Guid id, string[] includes = null);
        Task<T> GetSingleByIdAsync(int id, string[] includes = null);
        IEnumerable<T> GetMany(Expression<Func<T, bool>> where, string includes);
        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where, string[] includes = null);
        T GetSingleByCondition(Expression<Func<T, bool>> expression, string[] includes = null);
        /// <summary>
        /// get all record
        /// </summary>
        /// <param name="includes"></param>
        /// <returns></returns>
        IQueryable<T> GetAll(string[] includes = null);
        Task<IEnumerable<T>> GetAllAsync(string[] includes = null);
        IEnumerable<T> GetMulti(Expression<Func<T, bool>> predicate, string[] includes = null);
        IEnumerable<T> GetMultiPaging(Expression<Func<T, bool>> filter, out int total, int index = 0, int size = 50, string[] includes = null);
        int Count(Expression<Func<T, bool>> where);
        bool CheckContains(Expression<Func<T, bool>> predicate);
    }
}
