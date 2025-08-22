using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DomainObjects;
using DevXpert.Academy.Core.Domain.Messages;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevXpert.Academy.Core.Data
{
    public abstract class SQLDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;
        private readonly IMediatorHandler _mediator;

        public SQLDbContext(IConfiguration configuration, ILoggerFactory loggerFactory, IMediatorHandler mediator)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
            _mediator = mediator;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Debugger.IsAttached)
            {
                optionsBuilder.EnableDetailedErrors()
                              .EnableSensitiveDataLogging();
            }

            if (_loggerFactory != null)
                optionsBuilder.UseLoggerFactory(_loggerFactory);

            if (true) // TODO: debito técnico, será resolvido no futuro, verificar se é ambiente de desenvolvimento
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnectionLite") ?? throw new InvalidOperationException("String de conexão 'DefaultConnectionLite' não encontrada para banco SQLite em ambiente de desenvolvimento.");
                optionsBuilder.UseSqlite(connectionString);

            }
            else
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("String de conexão 'DefaultConnection' não encontrada.");
                optionsBuilder.UseSqlServer(connectionString);
            }

            base.OnConfiguring(optionsBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.IgnoreAny<ValidationResult>();
            configurationBuilder.IgnoreAny<Event>();

            base.ConfigureConventions(configurationBuilder);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => this.SaveChangesAsync(true, cancellationToken);
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries();

            foreach (var entry in entries.Where(p => p.Entity.GetType().GetProperty("DataHoraCriacao") != null
                                                  || p.Entity.GetType().GetProperty("DataHoraAlteracao") != null))
            {
                if (entry.Properties.Any(p => p.Metadata.Name == "DataHoraCriacao"))
                {
                    var dataHoraCriacao = entry.Property("DataHoraCriacao");

                    if (entry.State == EntityState.Added && (dataHoraCriacao.CurrentValue == null || DateTime.MinValue.Equals(dataHoraCriacao.CurrentValue)))
                        dataHoraCriacao.CurrentValue = DateTime.Now;
                    else if (entry.State == EntityState.Modified)
                        dataHoraCriacao.IsModified = false;
                }

                if (entry.Properties.Any(p => p.Metadata.Name == "DataHoraAlteracao"))
                {
                    var dataHoraAlteracao = entry.Property("DataHoraAlteracao");

                    if (entry.State == EntityState.Modified)
                    {
                        dataHoraAlteracao.CurrentValue = DateTime.Now;
                        dataHoraAlteracao.IsModified = true;
                    }
                }
            }

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task CallEvents()
        {
            var domainEntities = ChangeTracker
                .Entries<IEntity>()
                .Where(x => x.Entity.Notifications != null && x.Entity.Notifications.Any());

            if (!domainEntities.Any()) return;

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Notifications)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearEvents());

            if (_mediator == null)
                return;

            foreach (var domainEvent in domainEvents.OrderBy(p => p.Timestamp))
                await _mediator.RaiseEvent(domainEvent);
        }
    }
}
