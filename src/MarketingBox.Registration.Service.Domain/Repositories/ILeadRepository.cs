using System.Threading.Tasks;
using MarketingBox.Registration.Service.Domain.Leads;

namespace MarketingBox.Registration.Service.Domain.Repositories
{
    public interface ILeadRepository
    {
        Task SaveAsync(Lead lead);
        Task<long> GetLeadIdAsync(string tenantId, string generatorId);
        Task<Lead> RestoreAsync(long leadId);
    }
}