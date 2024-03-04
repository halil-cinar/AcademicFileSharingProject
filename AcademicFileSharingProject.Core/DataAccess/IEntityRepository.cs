using AcademicFileSharingProject.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Core.DataAccess
{
    public interface IEntityRepository<TEntity>
        where TEntity : EntityBase,new()

    {
        public TEntity Add(TEntity entity);
        public TEntity Update(TEntity entity);
        public TEntity Delete(TEntity entity);
        public TEntity SoftDelete(TEntity entity);
        public TEntity Delete(long id);
        public TEntity SoftDelete(long id);
        
        public List<TEntity> GetAll(Expression<Func<TEntity,bool>> filter=null);
        public TEntity Get(Expression<Func<TEntity, bool>> filter);
        public TEntity Get(long id);

        public long Count();





    }
}
