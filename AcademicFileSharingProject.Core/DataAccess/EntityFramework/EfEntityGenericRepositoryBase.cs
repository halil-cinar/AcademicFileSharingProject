using AcademicFileSharingProject.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AcademicFileSharingProject.Core.DataAccess.EntityFramework
{
    public class EfEntityGenericRepositoryBase<TEntity, TContext> : IEntityRepository<TEntity>
        where TEntity : EntityBase, new()
        where TContext : DbContext, new()
    {
        public virtual TEntity Add(TEntity entity)
        {
            using(var context=new TContext())
            {
                var entry=context.Entry(entity);
                entry.State= EntityState.Added;
                context.SaveChanges();
                return entry.Entity;
            }
        }

        public virtual IQueryable<TEntity> BaseGetAll(TContext context)
        {
            
                return context.Set<TEntity>();
            
        }

        public long Count()
        {
            using (var context = new TContext())
            {
                return context.Set<TEntity>().LongCount();
            }
        }

        public virtual TEntity Delete(TEntity entity)
        {
            using (var context = new TContext())
            {
                var entry = context.Entry(entity);
                entry.State = EntityState.Deleted;
                context.SaveChanges();
                return entry.Entity;
            }
        }

        public virtual TEntity Delete(long id)
        {
            using (var context = new TContext())
            {
                var entity = context.Set<TEntity>().Find(id);
                if (entity != null)
                {
                    var entry = context.Entry(entity);
                    entry.State = EntityState.Added;
                    context.SaveChanges();
                    return entry.Entity;
                }
                return entity;

            }
        }

        public virtual TEntity Get(Expression<Func<TEntity, bool>> filter)
        {
            using (var context = new TContext())
            {
                var entity=BaseGetAll(context).FirstOrDefault(filter);
                return entity;
            }
        }

        public virtual TEntity Get(long id)
        {
            using (var context = new TContext())
            {
                var entity = BaseGetAll(context).FirstOrDefault(x => x.Id == id);
                return entity;
            }
        }

        public virtual List<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            using (var context = new TContext())
            {
                var entities = BaseGetAll(context);
                return (filter == null)
                    ? entities.ToList()
                    :entities.Where(filter).ToList();
            }
        }

        public virtual TEntity SoftDelete(TEntity entity)
        {
            using (var context = new TContext())
            {
                entity.IsDeleted = true;
                var entry = context.Entry(entity);
                entry.State = EntityState.Modified;
                context.SaveChanges();
                return entry.Entity;
            }
        }

        public virtual TEntity SoftDelete(long id)
        {
            using (var context = new TContext())
            {
                var entity = context.Set<TEntity>().Find(id);
                if(entity == null)
                {
                    return null;
                }
                entity.IsDeleted = true;
                var entry = context.Entry(entity);
                entry.State = EntityState.Modified;
                context.SaveChanges();
                return entry.Entity;
            }
        }

        public virtual TEntity Update(TEntity entity)
        {
            using (var context = new TContext())
            {
                var entry = context.Entry(entity);
                entry.State = EntityState.Modified;
                context.SaveChanges();
                return entry.Entity;
            }
        }
    }
}
