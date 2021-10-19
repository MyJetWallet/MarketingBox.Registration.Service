using System;
using System.Threading.Tasks;
using MarketingBox.Registration.Postgres.Entities.Lead;
using MarketingBox.Registration.Postgres.Extensions;
using MarketingBox.Registration.Service.Domain.Leads;
using MarketingBox.Registration.Service.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace MarketingBox.Registration.Postgres.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private readonly DbContextOptionsBuilder<DatabaseContext> _dbContextOptionsBuilder;

        public LeadRepository(DbContextOptionsBuilder<DatabaseContext> dbContextOptionsBuilder)
        {
            _dbContextOptionsBuilder = dbContextOptionsBuilder;
        }

        public async Task SaveAsync(Lead lead)
        {
            using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = lead.CreateLeadEntity();
            var rowsCount = await ctx.Leads.Upsert(entity)
                .AllowIdentityMatch()
                .UpdateIf(prev => prev.Sequence < entity.Sequence)
                .RunAsync();

            if (rowsCount == 0)
            {
                throw new Exception($"Lead {lead.LeadId} already updated, try to use most recent version");
            }
        }

        public async Task<long> GetLeadIdAsync(string tenantId, string generatorId)
        {
            await using var ctx = new DatabaseContext(_dbContextOptionsBuilder.Options);
            var entity = new LeadIdGeneratorEntity()
            {
                TenantId = tenantId,
                GeneratorId = generatorId,
            };
            await ctx.LeadIdGenerators.AddAsync(entity);
            await ctx.SaveChangesAsync();
            return entity.LeadId;
        }

        public Task<Lead> RestoreAsync(long leadId)
        {
            throw new System.NotImplementedException();
        }
    }
}