using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.EventSourcing.EventStore.Mappings;
using Microsoft.EntityFrameworkCore;

namespace DevXpert.Academy.Core.EventSourcing.EventStore.Context
{
    public class EventStoreSQLContext : DbContext
    {
        public DbSet<StoredEvent> StoredEvent { get; set; }

        public EventStoreSQLContext(DbContextOptions<EventStoreSQLContext> options)
           : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new StoredEventMap());

            base.OnModelCreating(modelBuilder);
        }
    }
}
