using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Npgsql;
using System.Linq.Expressions;

namespace BlueCardPortal.Infrastructure.Data.Common
{
    /// <summary>
    /// Implementation of repository access methods
    /// for Relational Database Engine
    /// </summary>
    /// <typeparam name="T">Type of the data table to which 
    /// current reposity is attached</typeparam>
    public class Repository : IRepository
    {
        private bool disposedValue;

        /// <summary>
        /// Entity framework DB context holding connection information and properties
        /// and tracking entity states 
        /// </summary>
        private DbContext Context { get; set; }

        public Repository(ApplicationDbContext dbContext)
        {
            Context = dbContext;
        }

        /// <summary>
        /// Representation of table in database
        /// </summary>
        protected DbSet<T> DbSet<T>() where T : class
        {
            return this.Context.Set<T>();
        }

        /// <summary>
        /// Executes stored procedure
        /// </summary>
        /// <typeparam name="T">Return type of stored procedure</typeparam>
        /// <param name="procedure">Procedure from enum</param>
        /// <param name="args">Procedure parameters</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> ExecuteProc<T>(ProcedureType procedure, params object[] args) where T : class
        {
            string procedureName = StoredProcedures.GetProcedureName(procedure);
            string query = $"/*NO LOAD BALANCE*/ select * from {procedureName}";

            NpgsqlParameter[] parameters = new NpgsqlParameter[args.Length];

            for (int i = 0; i < args.Length; i++)
            {
                parameters[i] = new NpgsqlParameter($"@p{i + 1}", args[i]);
            }

            return await Context.Database
                .SqlQueryRaw<T>(query, parameters)
                .ToListAsync();
        }

        /// <summary>
        /// Executes raw SQL query
        /// </summary>
        /// <typeparam name="T">Query return type</typeparam>
        /// <param name="query">Query string as interpolated string
        /// parameters will be sanitized</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">Thrown if unsafe query detected</exception>
        public IQueryable<T> ExecuteSQL<T>(FormattableString query) where T : class
        {
            return Context.Database.SqlQuery<T>(query);
        }

        /// <summary>
        /// Adds entity to the database
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public async Task AddAsync<T>(T entity) where T : class
        {
            await DbSet<T>().AddAsync(entity);
        }

        /// <summary>
        /// Ads collection of entities to the database
        /// </summary>
        /// <param name="entities">Enumerable list of entities</param>
        public async Task AddRangeAsync<T>(IEnumerable<T> entities) where T : class
        {
            await DbSet<T>().AddRangeAsync(entities);
        }

        /// <summary>
        /// All records in a table
        /// </summary>
        /// <returns>Queryable expression tree</returns>
        public IQueryable<T> All<T>() where T : class
        {
            return DbSet<T>().AsQueryable();
        }

        public IQueryable<T> All<T>(Expression<Func<T, bool>> search) where T : class
        {
            return this.DbSet<T>()
                .Where(search);
        }

        /// <summary>
        /// The result collection won't be tracked by the context
        /// </summary>
        /// <returns>Expression tree</returns>
        public IQueryable<T> AllReadonly<T>() where T : class
        {
            return this.DbSet<T>()
                .AsNoTracking();
        }
        public IQueryable<T> AllReadonly<T>(Expression<Func<T, bool>> search) where T : class
        {
            return this.DbSet<T>()
                .Where(search)
                .AsNoTracking();
        }

        /// <summary>
        /// Deletes a record from database
        /// </summary>
        /// <param name="id">Identificator of record to be deleted</param>
        public async Task DeleteAsync<T>(object id) where T : class
        {
            T entity = await GetByIdAsync<T>(id);

            Delete<T>(entity);
        }

        /// <summary>
        /// Deletes a record from database
        /// </summary>
        /// <param name="entity">Entity representing record to be deleted</param>
        public void Delete<T>(T entity) where T : class
        {
            EntityEntry entry = this.Context.Entry(entity);

            if (entry.State == EntityState.Detached)
            {
                this.DbSet<T>().Attach(entity);
            }

            if (entity.GetType().IsAssignableTo(typeof(ISoftDeletable)))
            {
                entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
                entry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = DateTime.UtcNow;
                entry.State = EntityState.Modified;
            }
            else
            {
                entry.State = EntityState.Deleted;
            }
        }

        /// <summary>
        /// Detaches given entity from the context
        /// </summary>
        /// <param name="entity">Entity to be detached</param>
        public void Detach<T>(T entity) where T : class
        {
            EntityEntry entry = this.Context.Entry(entity);

            entry.State = EntityState.Detached;
        }   

        /// <summary>
        /// Gets specific record from database by primary key
        /// </summary>
        /// <param name="id">record identificator</param>
        /// <returns>Single record or null</returns>
        public async Task<T?> GetByIdAsync<T>(object id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        /// <summary>
        /// Gets specifi record from database by primary key
        /// </summary>
        /// <param name="id">Composite key</param>
        /// <returns>Single record or null</returns>
        public async Task<T?> GetByIdsAsync<T>(object[] id) where T : class
        {
            return await DbSet<T>().FindAsync(id);
        }

        /// <summary>
        /// Saves all made changes in trasaction
        /// </summary>
        /// <returns>Number of entries written to the base</returns>
        public async Task<int> SaveChangesAsync()
        {
            return await this.Context.SaveChangesAsync();
        }

        public void DeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            foreach (var entity in entities)
            {
                EntityEntry entry = this.Context.Entry(entity);

                if (entry.State == EntityState.Detached)
                {
                    this.DbSet<T>().Attach(entity);
                }

                if (entity.GetType().IsAssignableTo(typeof(ISoftDeletable)))
                {
                    entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
                else
                {
                    entry.State = EntityState.Deleted;
                }
            }
        }

        /// <summary>
        /// Truncate table
        /// </summary>
        /// <param name="table">Table name</param>
        public async Task Truncate(string table)
        {
            await Context.Database.ExecuteSqlAsync($"TRUNCATE TABLE {table} RESTART IDENTITY");
        }

        /// <summary>
        /// Clear change tracker
        /// </summary>
        public void ChangeTrackerClear()
        {
            this.Context.ChangeTracker.Clear();
        }

        /// <summary>
        /// Deletes records imediately from database without tracking
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <param name="search">Predicate</param>
        /// <returns></returns>
        public async Task<int> DeleteAsNoTrackingAsync<T>(Expression<Func<T, bool>> search) where T : class
        {
            int result = 0;
            var collection = this.DbSet<T>()
                .Where(search);

            if (typeof(T).IsAssignableTo(typeof(ISoftDeletable)))
            {
                DateTime now = DateTime.UtcNow;
                bool isDeleted = true;

                result = await collection
                    .Select(c => c as ISoftDeletable)
                    .ExecuteUpdateAsync(c => c
                        .SetProperty(p => p.IsDeleted, isDeleted)
                        .SetProperty(p => p.DeletedAt, now));
            }
            else
            {
                result = await collection.ExecuteDeleteAsync();
            }

            return result;
        }

        public async Task DeleteAsync<T>(Expression<Func<T, bool>> search) where T : class
        {
            List<T> entities = await this.DbSet<T>()
                .Where(search)
                .ToListAsync();

            foreach (var entity in entities)
            {
                EntityEntry entry = this.Context.Entry(entity);

                if (entry.State == EntityState.Detached)
                {
                    this.DbSet<T>().Attach(entity);
                }

                if (entity.GetType().IsAssignableTo(typeof(ISoftDeletable)))
                {
                    entry.Property(nameof(ISoftDeletable.IsDeleted)).CurrentValue = true;
                    entry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
                else
                {
                    entry.State = EntityState.Deleted;
                }
            }
        }

        /// <summary>
        /// Executes raw SQL command
        /// </summary>
        /// <param name="command">Command to execute</param>
        /// <returns>Affected rows</returns>
        public async Task<int> ExecuteCommandAsync(FormattableString command)
        {
            using var transaction = Context.Database.BeginTransaction();
            int rowsAffected = 0;

            try
            {
                rowsAffected = await Context.Database.ExecuteSqlInterpolatedAsync(command);
                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new ArgumentException("Error executing command", ex);
            }

            return rowsAffected;
        }

        /// <summary>
        /// Implementing Dispose pattern
        /// Only managed code have to be disposed
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Context.Dispose();
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposing the context when it is not neede
        /// Don't have to call this method explicitely
        /// Leave it to the IoC container
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
