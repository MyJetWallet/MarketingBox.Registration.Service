using System.ServiceModel;
using System.Threading.Tasks;
using MarketingBox.Registration.Service.Grpc.Models.Deposits.Contracts;

namespace MarketingBox.Registration.Service.Grpc
{
    [ServiceContract]
    public interface IApproveService
    {
        [OperationContract]
        Task<DepositApproveResponse> ApproveDepositAsync(DepositApproveRequest request);
    }
}
