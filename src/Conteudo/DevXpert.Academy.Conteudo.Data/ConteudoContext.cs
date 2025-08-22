using DevXpert.Academy.Core.Data;
using DevXpert.Academy.Core.Domain.Communication.Mediatr;
using DevXpert.Academy.Core.Domain.DataModels;
using DevXpert.Academy.Core.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace DevXpert.Academy.Conteudo.Data
{
    public class ConteudoContext : SQLDbContext, IUnitOfWork
    {
        public ConteudoContext(IConfiguration configuration, ILoggerFactory loggerFactory, IMediatorHandler mediator) : base(configuration, loggerFactory, mediator)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ConteudoContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }

        public async Task<bool> CommitAsync()
        {
            try
            {
                var sucesso = await base.SaveChangesAsync() > 0;
                if (sucesso) await CallEvents();

                return sucesso;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException sqlServerException)
                {
                    switch (sqlServerException.Number)
                    {
                        case 2627: // Unique constraint
                            throw new BusinessException($"Existe algum valor já em uso por outra entidade nas informações fornecidas: {sqlServerException.Message}", ex);

                        case 547: // Constraint violation (FK)
                            throw new BusinessException($"A entidade possui referências em outras entidades.", ex);
                    }
                }
                else if (ex.InnerException is SqliteException sqliteException)
                {
                    switch (sqliteException.SqliteErrorCode)
                    {
                        case 19: // SQLITE_CONSTRAINT
                            throw new BusinessException("Violação de restrição: valor duplicado ou referência inválida.", ex);
                    }
                }

                throw;
            }
            catch (Exception ex)
            {
                if (ex.InnerException is SqlException sqlServerException && sqlServerException.Number == -1)
                {
                    throw new BusinessException("Sem conexão com o servidor de banco de dados.", ex);
                }
                else if (ex.InnerException is SqliteException sqliteException && sqliteException.SqliteErrorCode == 5)
                {
                    throw new BusinessException("Muitas conexões em uso simultâneamente no servidor, tente novamente.", ex);
                }

                throw;
            }
        }

        public void DettachAllEntities()
        {
            ChangeTracker.Clear();
        }
    }
}
