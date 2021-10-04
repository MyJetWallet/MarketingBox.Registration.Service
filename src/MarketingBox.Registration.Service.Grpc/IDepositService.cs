using MarketingBox.Registration.Service.Grpc.Models.Leads;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Messages;
using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Contracts;
using MarketingBox.Registration.Service.Grpc.Models.Leads.Requests;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IDepositService
    {
        [OperationContract]
        Task<DepositCreateResponse> CreateDepositAsync(DepositCreateRequest request);

        //[OperationContract]
        //Task<LeadCreateResponse> UpdateAsync(LeadUpdateRequest request);

        //[OperationContract]
        //Task<LeadCreateResponse> GetAsync(LeadGetRequest request);

        //[OperationContract]
        //Task<LeadCreateResponse> DeleteAsync(LeadDeleteRequest request);
    }
}
