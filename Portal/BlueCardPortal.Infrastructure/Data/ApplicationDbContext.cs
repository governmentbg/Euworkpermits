using BlueCardPortal.Infrastructure.Constants;
using BlueCardPortal.Infrastructure.Data.Common;
using BlueCardPortal.Infrastructure.Data.Models.Application;
using BlueCardPortal.Infrastructure.Data.Models.Common;
using BlueCardPortal.Infrastructure.Data.Models.Complaint;
using BlueCardPortal.Infrastructure.Data.Models.Nomenclature;
using BlueCardPortal.Infrastructure.Data.Models.SelfDenial;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BlueCardPortal.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            var entityTypesHasSoftDeletion = builder.Model.GetEntityTypes()
                .Where(e => e.ClrType.IsAssignableTo(typeof(ISoftDeletable)));

            foreach (var entityType in entityTypesHasSoftDeletion)
            {
                var isDeletedProperty = entityType.FindProperty(nameof(ISoftDeletable.IsDeleted));
                var parameter = Expression.Parameter(entityType.ClrType, "p");

                if (isDeletedProperty?.PropertyInfo != null && parameter != null)
                {
                    var filter = Expression.Lambda(Expression.Not(Expression.Property(parameter, isDeletedProperty.PropertyInfo)), parameter);

                    entityType.SetQueryFilter(filter);
                }
            }

            base.OnModelCreating(builder);
        }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationItem> ApplicationItems { get; set; }
        public DbSet<ApplicationMessage> ApplicationMessages { get; set; }
        public DbSet<ApplicationItemType> ApplicationItemTypes { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<SelfDenial> SelfDenials { get; set; }
        public DbSet<ApplicationUpdate> ApplicationUpdates { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<UpdateMessage> UpdateMessages { get; set; }
        public DbSet<NomenclatureType> NomenclatureTypes { get; set; }
        public DbSet<CodeableConcept> CodeableConcepts { get; set; }
        public DbSet<AdditionalColumn> AdditionalColumns { get; set; }

        public DbSet<EMail> EMails { get; set; }
    }
}
