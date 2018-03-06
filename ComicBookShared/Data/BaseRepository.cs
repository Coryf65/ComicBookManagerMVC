using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace ComicBookShared.Data
{
    /// <summary>
    /// ~A Generic Base Repository Class~
    /// Make it public so other classes can access it 
    /// and Abstract so that this will be used as a base for other repository classes 
    /// and not used directly because it can't be instaniated
    /// </summary>
    public abstract class BaseRepository<TEntity>
        where TEntity : class // Genric Type Constraint 
    {

        protected Context Context { get; private set; }

        // BaseRepository(context)
        public BaseRepository(Context context)
        {
            Context = context;
        }

        // Get(id)
        // using a generic method
        public abstract TEntity Get(int id, bool includeRelatedEntities = true);

        // GetList()
        public abstract IList<TEntity> GetList();


        // Add(entity)
        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);

            Context.SaveChanges();
        }

        // Update(entity)
        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;

            Context.SaveChanges();
        }


        // Delete(id)
        public void Delete(int id)
        {
            //could have used a stub entity but would take more coding
            var set = Context.Set<TEntity>();
            var entity = set.Find(id);

            set.Remove(entity);
            Context.SaveChanges();
        }
    }
}
