using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface ILeadService
    {
        [OperationContract]
        Task<LeadCreateResponse> CreateAsync(LeadCreateRequest request);
    }
}
