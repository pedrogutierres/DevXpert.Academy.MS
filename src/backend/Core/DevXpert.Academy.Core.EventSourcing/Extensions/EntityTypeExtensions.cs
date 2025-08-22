using DevXpert.Academy.Core.Domain.DomainObjects;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DevXpert.Academy.Core.EventSourcing.EventStore.Extensions
{
    public static class EntityTypeExtensions
    {
        public static void IgnoreFluentValidation<TEntity>(this EntityTypeBuilder<TEntity> modelBuilder)
            where TEntity : Entity
        {
            modelBuilder.Ignore(p => p.ValidationResult);
        }
    }
}
