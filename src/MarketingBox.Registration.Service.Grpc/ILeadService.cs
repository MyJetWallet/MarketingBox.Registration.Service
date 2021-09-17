using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Messages;
using System.ServiceModel;
using System.Threading.Tasks;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface ILeadService
    {
        [OperationContract]
        Task<LeadResponse> CreateAsync(LeadCreateRequest request);

        [OperationContract]
        Task<LeadResponse> UpdateAsync(LeadUpdateRequest request);

        [OperationContract]
        Task<LeadResponse> GetAsync(LeadGetRequest request);

        [OperationContract]
        Task<LeadResponse> DeleteAsync(LeadDeleteRequest request);
    }
}
