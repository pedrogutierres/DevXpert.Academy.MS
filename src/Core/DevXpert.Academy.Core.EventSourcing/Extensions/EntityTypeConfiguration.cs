using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Core.EventSourcing.EventStore.Extensions
{
    public abstract class EntityTypeConfiguration<TEntity> where TEntity : class
    {
        public abstract void Map(EntityTypeBuilder<TEntity> builder);
    }
}
