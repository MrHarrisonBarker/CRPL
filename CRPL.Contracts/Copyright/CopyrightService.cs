using System.Numerics;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.ContractHandlers;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;

namespace CRPL.Contracts.Copyright
{
    public partial class CopyrightService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, CopyrightDeployment copyrightDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<CopyrightDeployment>().SendRequestAndWaitForReceiptAsync(copyrightDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, CopyrightDeployment copyrightDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<CopyrightDeployment>().SendRequestAsync(copyrightDeployment);
        }

        public static async Task<CopyrightService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, CopyrightDeployment copyrightDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, copyrightDeployment, cancellationTokenSource);
            return new CopyrightService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public CopyrightService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> ApproveManagerRequestAsync(ApproveManagerFunction approveManagerFunction)
        {
             return ContractHandler.SendRequestAsync(approveManagerFunction);
        }

        public Task<TransactionReceipt> ApproveManagerRequestAndWaitForReceiptAsync(ApproveManagerFunction approveManagerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveManagerFunction, cancellationToken);
        }

        public Task<string> ApproveManagerRequestAsync(string manager, bool hasApproval)
        {
            var approveManagerFunction = new ApproveManagerFunction();
                approveManagerFunction.Manager = manager;
                approveManagerFunction.HasApproval = hasApproval;
            
             return ContractHandler.SendRequestAsync(approveManagerFunction);
        }

        public Task<TransactionReceipt> ApproveManagerRequestAndWaitForReceiptAsync(string manager, bool hasApproval, CancellationTokenSource cancellationToken = null)
        {
            var approveManagerFunction = new ApproveManagerFunction();
                approveManagerFunction.Manager = manager;
                approveManagerFunction.HasApproval = hasApproval;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveManagerFunction, cancellationToken);
        }

        public Task<string> ApproveOneRequestAsync(ApproveOneFunction approveOneFunction)
        {
             return ContractHandler.SendRequestAsync(approveOneFunction);
        }

        public Task<TransactionReceipt> ApproveOneRequestAndWaitForReceiptAsync(ApproveOneFunction approveOneFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveOneFunction, cancellationToken);
        }

        public Task<string> ApproveOneRequestAsync(BigInteger rightId, string approved)
        {
            var approveOneFunction = new ApproveOneFunction();
                approveOneFunction.RightId = rightId;
                approveOneFunction.Approved = approved;
            
             return ContractHandler.SendRequestAsync(approveOneFunction);
        }

        public Task<TransactionReceipt> ApproveOneRequestAndWaitForReceiptAsync(BigInteger rightId, string approved, CancellationTokenSource cancellationToken = null)
        {
            var approveOneFunction = new ApproveOneFunction();
                approveOneFunction.RightId = rightId;
                approveOneFunction.Approved = approved;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveOneFunction, cancellationToken);
        }

        public Task<string> BindRestructureRequestAsync(BindRestructureFunction bindRestructureFunction)
        {
             return ContractHandler.SendRequestAsync(bindRestructureFunction);
        }

        public Task<TransactionReceipt> BindRestructureRequestAndWaitForReceiptAsync(BindRestructureFunction bindRestructureFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bindRestructureFunction, cancellationToken);
        }

        public Task<string> BindRestructureRequestAsync(BigInteger rightId, bool accepted)
        {
            var bindRestructureFunction = new BindRestructureFunction();
                bindRestructureFunction.RightId = rightId;
                bindRestructureFunction.Accepted = accepted;
            
             return ContractHandler.SendRequestAsync(bindRestructureFunction);
        }

        public Task<TransactionReceipt> BindRestructureRequestAndWaitForReceiptAsync(BigInteger rightId, bool accepted, CancellationTokenSource cancellationToken = null)
        {
            var bindRestructureFunction = new BindRestructureFunction();
                bindRestructureFunction.RightId = rightId;
                bindRestructureFunction.Accepted = accepted;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bindRestructureFunction, cancellationToken);
        }

        public Task<CopyrightMetaOutputDTO> CopyrightMetaQueryAsync(CopyrightMetaFunction copyrightMetaFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<CopyrightMetaFunction, CopyrightMetaOutputDTO>(copyrightMetaFunction, blockParameter);
        }

        public Task<CopyrightMetaOutputDTO> CopyrightMetaQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var copyrightMetaFunction = new CopyrightMetaFunction();
                copyrightMetaFunction.RightId = rightId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<CopyrightMetaFunction, CopyrightMetaOutputDTO>(copyrightMetaFunction, blockParameter);
        }

        public Task<CurrentVotesOutputDTO> CurrentVotesQueryAsync(CurrentVotesFunction currentVotesFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<CurrentVotesFunction, CurrentVotesOutputDTO>(currentVotesFunction, blockParameter);
        }

        public Task<CurrentVotesOutputDTO> CurrentVotesQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var currentVotesFunction = new CurrentVotesFunction();
                currentVotesFunction.RightId = rightId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<CurrentVotesFunction, CurrentVotesOutputDTO>(currentVotesFunction, blockParameter);
        }

        public Task<string> GetApprovedQueryAsync(GetApprovedFunction getApprovedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        
        public Task<string> GetApprovedQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var getApprovedFunction = new GetApprovedFunction();
                getApprovedFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        public Task<bool> IsManagerQueryAsync(IsManagerFunction isManagerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsManagerFunction, bool>(isManagerFunction, blockParameter);
        }

        
        public Task<bool> IsManagerQueryAsync(string client, string manager, BlockParameter blockParameter = null)
        {
            var isManagerFunction = new IsManagerFunction();
                isManagerFunction.Client = client;
                isManagerFunction.Manager = manager;
            
            return ContractHandler.QueryAsync<IsManagerFunction, bool>(isManagerFunction, blockParameter);
        }

        public Task<OwnershipOfOutputDTO> OwnershipOfQueryAsync(OwnershipOfFunction ownershipOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<OwnershipOfFunction, OwnershipOfOutputDTO>(ownershipOfFunction, blockParameter);
        }

        public Task<OwnershipOfOutputDTO> OwnershipOfQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var ownershipOfFunction = new OwnershipOfFunction();
                ownershipOfFunction.RightId = rightId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<OwnershipOfFunction, OwnershipOfOutputDTO>(ownershipOfFunction, blockParameter);
        }

        public Task<BigInteger> PortfolioSizeQueryAsync(PortfolioSizeFunction portfolioSizeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PortfolioSizeFunction, BigInteger>(portfolioSizeFunction, blockParameter);
        }

        
        public Task<BigInteger> PortfolioSizeQueryAsync(string owner, BlockParameter blockParameter = null)
        {
            var portfolioSizeFunction = new PortfolioSizeFunction();
                portfolioSizeFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<PortfolioSizeFunction, BigInteger>(portfolioSizeFunction, blockParameter);
        }

        public Task<ProposalOutputDTO> ProposalQueryAsync(ProposalFunction proposalFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<ProposalFunction, ProposalOutputDTO>(proposalFunction, blockParameter);
        }

        public Task<ProposalOutputDTO> ProposalQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var proposalFunction = new ProposalFunction();
                proposalFunction.RightId = rightId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<ProposalFunction, ProposalOutputDTO>(proposalFunction, blockParameter);
        }

        public Task<string> ProposeRestructureRequestAsync(ProposeRestructureFunction proposeRestructureFunction)
        {
             return ContractHandler.SendRequestAsync(proposeRestructureFunction);
        }

        public Task<TransactionReceipt> ProposeRestructureRequestAndWaitForReceiptAsync(ProposeRestructureFunction proposeRestructureFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(proposeRestructureFunction, cancellationToken);
        }

        public Task<string> ProposeRestructureRequestAsync(BigInteger rightId, List<OwnershipStakeContract> restructured)
        {
            var proposeRestructureFunction = new ProposeRestructureFunction();
                proposeRestructureFunction.RightId = rightId;
                proposeRestructureFunction.Restructured = restructured;
            
             return ContractHandler.SendRequestAsync(proposeRestructureFunction);
        }

        public Task<TransactionReceipt> ProposeRestructureRequestAndWaitForReceiptAsync(BigInteger rightId, List<OwnershipStakeContract> restructured, CancellationTokenSource cancellationToken = null)
        {
            var proposeRestructureFunction = new ProposeRestructureFunction();
                proposeRestructureFunction.RightId = rightId;
                proposeRestructureFunction.Restructured = restructured;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(proposeRestructureFunction, cancellationToken);
        }

        public Task<string> RegisterRequestAsync(RegisterFunction registerFunction)
        {
             return ContractHandler.SendRequestAsync(registerFunction);
        }

        public Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(RegisterFunction registerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerFunction, cancellationToken);
        }

        public Task<string> RegisterRequestAsync(List<OwnershipStakeContract> to, Meta meta)
        {
            var registerFunction = new RegisterFunction();
                registerFunction.To = to;
                registerFunction.Meta = meta;
            
             return ContractHandler.SendRequestAsync(registerFunction);
        }

        public Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(List<OwnershipStakeContract> to, Meta meta, CancellationTokenSource cancellationToken = null)
        {
            var registerFunction = new RegisterFunction();
                registerFunction.To = to;
                registerFunction.Meta = meta;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerFunction, cancellationToken);
        }
    }
}
